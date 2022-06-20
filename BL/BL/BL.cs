using System;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace BL
{

    /// <summary>
    /// These partial classes will implement the IBL interface and
    /// take care of updating the data layer, and in addition a BL object will maintain a drone list.
    /// </summary>
    partial class BL : BlApi.IBL
    {
        static BL() { }// static ctor to ensure instance init is done just before first usage

        internal static BL Instance { get; } = new BL();// The internal Instance property to use

        public DalApi.IDal AccessIdal; //Create an object that we will use to access the data layer. 

        public List<DroneToList> DronesBL; //Creating a list of dronse.

        //Creating fields of power consumption per kilometer and on the rate of drone charging
        public double Free;
        public double LightWeightCarrier;
        public double MediumWeightBearing;
        public double CarriesHeavyWeight;
        public double DroneLoadingRate;

        //Create a Random object to be used to draw the battery status and Location of the drones.
        public Random random = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The constructor will build the list of dronse and especially will calculate the location and battery status of the drone.
        /// </summary>
        private BL()
        {
            //initialization an object that will serve as an access point to methods in DAL.
            AccessIdal = DalApi.DalFactory.GetDL();

            //Placement in the fields of power consumption and charging rate.
            double[] arr = AccessIdal.RequestPowerConsumptionByDrone();
            Free = arr[0];
            LightWeightCarrier = arr[1];
            MediumWeightBearing = arr[2];
            CarriesHeavyWeight = arr[3];
            DroneLoadingRate = arr[4];

            //Conversion of a drone list from the data layer to a Drone list of the BL layer.
            DronesBL = (from item in AccessIdal.GetDroneList()
                        select new DroneToList()
                        {
                            Id = item.Id,
                            Model = item.Model,
                            MaxWeight = (WeightCategories)item.MaxWeight
                        }).ToList();

            //Convert a customer list from the data layer to a customer Collection of the BL layer.
            IEnumerable<Customer> CustomerBL = from item in AccessIdal.GetCustomerList()
                                               select new Customer()
                                               {
                                                   Id = item.Id,
                                                   Name = item.Name,
                                                   PhoneNumber = item.PhoneNumber,
                                                   LocationOfCustomer = new Location()
                                                   { longitude = item.Longitude, latitude = item.Latitude }
                                               };

            //Converts a list of base stations from the data layer to a list of base stations of the BL layer.
            List<BaseStation> baseStationBL = (from item in AccessIdal.GetBaseStationList()
                                               select new BaseStation()
                                               {
                                                   Id = item.Id,
                                                   Name = item.StationName,
                                                   FreeChargeSlots = item.FreeChargeSlots,
                                                   BaseStationLocation = new Location() { longitude = item.Longitude, latitude = item.Latitude },
                                                   DroneInChargsList = new List<DroneInCharg>()
                                               }).ToList();

            //bring the list of package from the data layer.
            List<DO.Parcel> holdDalParcels = AccessIdal.GetParcelList(i => i.DroneId != 0).ToList();//We made a list because we had to later use the index

            //The loop will go through the dronesBL list and check if the drone is associated with the package
            //or if it does not makes a delivery. and will update its status, location and battery status.
            foreach (var item in DronesBL)
            {
                int index = holdDalParcels.FindIndex(x => x.DroneId == item.Id && x.Delivered == null); //Finding the package linked to the drone.

                if (index != -1) //If the drone is indeed associated with one of the Parcels in the list.
                {
                    item.Statuses = DroneStatuses.busy; //Placement drone status for shipping operation.

                    Location locationOfsender = CustomerBL.First(x => x.Id == holdDalParcels[index].SenderId).LocationOfCustomer;
                    Location locationOfReceiver = CustomerBL.First(x => x.Id == holdDalParcels[index].TargetId).LocationOfCustomer;

                    //Distance between sender and receiver.
                    double distanceBetweenSenderAndReceiver = GetDistance(locationOfsender, locationOfReceiver);

                    //Distance between the receiver and the nearest base station.
                    double DistanceBetweenReceiverAndNearestBaseStation = minDistanceBetweenBaseStationsAndLocation
                        (baseStationBL, locationOfReceiver).Item2;

                    double electricityUse = DistanceBetweenReceiverAndNearestBaseStation * Free; //Power consumption from the destination to the nearest station.

                    //Power consumption from the sender to the destination according to the size of the package.
                    switch ((WeightCategories)holdDalParcels[index].Weight)
                    {
                        case WeightCategories.light:
                            electricityUse += distanceBetweenSenderAndReceiver * LightWeightCarrier;
                            break;
                        case WeightCategories.medium:
                            electricityUse += distanceBetweenSenderAndReceiver * MediumWeightBearing;
                            break;
                        case WeightCategories.heavy:
                            electricityUse += distanceBetweenSenderAndReceiver * CarriesHeavyWeight;
                            break;
                        default:
                            break;
                    }

                    //Placement a location of drone on the map.
                    if (holdDalParcels[index].PickedUp == null)//Check if the Parcel has already been PickedUped.
                    {
                        item.CurrentLocation = minDistanceBetweenBaseStationsAndLocation(baseStationBL, locationOfsender).Item1;

                        //Need to add the electricity use between the drone position and the sender position
                        electricityUse += GetDistance(item.CurrentLocation, locationOfsender) * Free; 
                    }
                    else //If the package was PickedUped.
                    {
                        item.CurrentLocation = locationOfsender;
                    }

                    //random number battery status between minimum charge to make the shipment and full charge.     
                    item.BatteryStatus = (float)((float)(random.NextDouble() * (100 - electricityUse)) + electricityUse);

                    item.NumberOfLinkedParcel = holdDalParcels[index].Id; //Placement field in BL drone.
                }
                else //If the drone is not associated with one of the parcels on the list and is actually available and does not ship.
                {
                    item.Statuses = (DroneStatuses)random.Next(0, 2);//Lottery drone mode between maintenance mode and free mode.

                    //Placement a location and battery of drone on the map. 
                    if (item.Statuses == DroneStatuses.inMaintenance)
                    {
                        BaseStation baseStation = baseStationBL[random.Next(0, baseStationBL.Count)];
                        item.CurrentLocation = baseStation.BaseStationLocation;

                        //Update the data layer that the drone is in Charg.
                        AccessIdal.SendingDroneforChargingAtBaseStation(baseStation.Id, item.Id);
                        AccessIdal.UpdateMinusChargeSlots(baseStation.Id);

                        item.BatteryStatus = random.Next(0, 21);
                    }
                    else //item.Statuses == DroneStatuses.free
                    {
                        //List of parcel provided by the drone.
                        List<DO.Parcel> DeliveredAndSameDroneID = holdDalParcels.FindAll(x => x.DroneId == item.Id && x.Delivered != null);

                        if (DeliveredAndSameDroneID.Any())//if the List is not empty. Placement its location to the location of the destination of one of the packages sent by the drone And the battery accordingly.
                        {
                            item.CurrentLocation = CustomerBL.First(x => x.Id == DeliveredAndSameDroneID[random.Next(0, DeliveredAndSameDroneID.Count)].TargetId).LocationOfCustomer;
                            double electricityUse = minDistanceBetweenBaseStationsAndLocation(baseStationBL, item.CurrentLocation).Item2 * Free;
                            item.BatteryStatus = (float)((float)(random.NextDouble() * (100 - electricityUse)) + electricityUse);
                        }
                        else //if the List is empty. grill its location to the location of one of the stations. And the battery between 0 and 100
                        {
                            item.CurrentLocation = baseStationBL[random.Next(0, baseStationBL.Count)].BaseStationLocation;
                            item.BatteryStatus = random.Next(0, 101);
                        }
                    }
                }
            }
        }


        #region Function of finding the location of the base station closest to the location
        /// <summary>
        /// The function calculates the distance between a particular location and base stations.
        /// </summary>
        /// <param name="baseStationBL">baseStationBL List</param>
        /// <param name="location">location</param>
        /// <returns>The location of the base station closest to the location and the min distance</returns>
        public (Location, double) minDistanceBetweenBaseStationsAndLocation(IEnumerable<BaseStation> baseStationBL, Location location)
        {
            IEnumerable<Location> locations = from item in baseStationBL
                                              orderby GetDistance(location, item.BaseStationLocation)
                                              select item.BaseStationLocation;
            Location locationOfNearestStation = locations.First();
            return (locationOfNearestStation, GetDistance(location, locationOfNearestStation));              
        }
        #endregion Function of finding the location of the base station closest to the location

        #region Function of calculating distance between points

        /// <summary>
        /// A function that calculates the distance between points.
        /// </summary>
        /// <param name="location1">location 1</param>
        /// <param name="location2">location 2</param>
        /// <returns>the distence between the points</returns>
        private double GetDistance(Location location1, Location location2)
        {
            //For the calculation we calculate the earth into a circle (ellipse) Divide its 360 degrees by half
            //180 for each longitude / latitude and then make a pie on each half to calculate the radius for
            //the formula below
            var num1 = location1.longitude * (Math.PI / 180.0);
            var d1 = location1.latitude * (Math.PI / 180.0);
            var num2 = location2.longitude * (Math.PI / 180.0) - num1;
            var d2 = location2.latitude * (Math.PI / 180.0);

            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) + Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0); //https://iw.waldorf-am-see.org/588999-calculating-distance-between-two-latitude-QPAAIP
                                                                                                                                   //We calculate the distance according to a formula that
                                                                                                                                   // also takes into account the curvature of the earth
            return ((double)(6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3))))) / 1000;
        }
        #endregion Function of calculating distance between points
    }
}
