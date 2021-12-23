﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BlApi;
using BO;

namespace ConsoleUI_BL
{
    #region enums
    /// <summary>enum for the first dialog </summary>
    enum Options { Insert = 1, Update, DisplaySingle, DisplayList, EXIT }
    /// <summary> enum for InsertrOption</summary>
    enum InsertrOption { BaseStation = 1, Drone, Customer, Parcel }
    /// <summary> enum for UpdatesOption</summary>
    enum UpdatesOption { DroneUpdate=1, BaseStaitonUpdate, CustomerUpdate, InCharging, Outcharging, AssignDrone , PickUp, Deliverd }
    /// <summary>enum for DisplaySingleOption </summary>
    enum DisplaySingleOption { BaseStationView = 1, DroneDisplay, CustomerView, PackageView }
    /// <summary>enum for DisplayListOption </summary>
    enum DisplayListOption
    {
        ListOfBaseStationView = 1, ListOfDronedisplay, ListOfCustomerView,
        ListOfPackageView, ListOfFreePackageView, ListOfBaseStationsWithFreeChargSlots
    }
    #endregion enums

    ///<summary> main class </summary> 
    class Program
    {
        #region fanction of main

        #region Handling insert options
        /// <summary>
        /// The function handles various addition options.
        /// </summary>
        /// <param name="bl">BL object that is passed as a parameter to enable the functions in the bl class</param>
        static public void InsertOptions(BlApi.IBL bl)
        {
            Console.WriteLine(@"
Insert options:

1. Add a base station to the list of stations.
2. Add a drone to the list of existing drones.
3. Admission of a new customer to the customer list.
4. Admission of package for shipment.

Your choice:");
            int choice;
            while(!int.TryParse(Console.ReadLine(), out  choice));

            switch ((InsertrOption)choice)
            {
                case InsertrOption.BaseStation:
                    int newBaseStationID, newchargsSlots;
                    string newName;
                    double newlongitude, newlatitude;

                    Console.WriteLine(@"
You have selected to add a new station.
Please enter an ID number for the new station:");
                    while (!int.TryParse(Console.ReadLine(), out newBaseStationID)) ;
                    Console.WriteLine("Next Please enter the name of the station:");
                    newName = Console.ReadLine();
                    Console.WriteLine("Next Please enter the number of charging slots at the station:");
                    while (!int.TryParse(Console.ReadLine(), out newchargsSlots)) ;
                    Console.WriteLine("Next Please enter the longitude of the station:");
                    while (!double.TryParse(Console.ReadLine(), out newlongitude)) ;
                    Console.WriteLine("Next Please enter the latitude of the station:");
                    while (!double.TryParse(Console.ReadLine(), out newlatitude)) ;

                    Location location = new Location { longitude = newlongitude,latitude = newlatitude};

                    BaseStation newbaseStation = new BaseStation
                    {
                        Id = newBaseStationID,
                        Name = newName,
                        FreeChargeSlots = newchargsSlots,
                        BaseStationLocation = location,
                        DroneInChargsList = new List<DroneInCharg>()     
                    };
                    try
                    {
                        bl.AddStation(newbaseStation);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (AddAnExistingObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case InsertrOption.Drone:
                    int newDroneID, newMaxWeight, firstChargingStation;
                    string newModel;

                    Console.WriteLine(@"
You have selected to add a new Drone.
Please enter an ID number for the new drone:");
                    while (!int.TryParse(Console.ReadLine(), out newDroneID)) ;
                    Console.WriteLine("Next Please enter the model of the Drone:");
                    newModel = Console.ReadLine();
                    Console.WriteLine("Next enter the weight category of the new Drone: 0 for light, 1 for medium and 2 for heavy");
                    while (!int.TryParse(Console.ReadLine(), out newMaxWeight));
                    Console.WriteLine("Next enter the ID of the Station to Put the drone for first charge");
                    while (!int.TryParse(Console.ReadLine(), out firstChargingStation)) ;

                    DroneToList newdrone = new DroneToList
                    {
                        Id = newDroneID,
                        Model = newModel,
                        MaxWeight = (WeightCategories)newMaxWeight
                    };

                    try
                    {
                        bl.AddDrone(newdrone, firstChargingStation);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (AddAnExistingObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (NoFreeChargingStations ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (NonExistentEnumException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case InsertrOption.Customer:
                    int newCustomerID;
                    string newCustomerName, newPhoneNumber;
                    double newCustomerLongitude, newCustomerLatitude;

                    Console.WriteLine(@"
You have selected to add a new Customer.
Please enter an ID number for the new Customer:");
                    while (!int.TryParse(Console.ReadLine(), out newCustomerID)) ;
                    Console.WriteLine("Next Please enter the name of the customer:");
                    newCustomerName = Console.ReadLine();
                    Console.WriteLine("Next enter the phone number of the new customer:");
                    newPhoneNumber = Console.ReadLine();
                    Console.WriteLine("Next Please enter the longitude of the Customer:");
                    while (!double.TryParse(Console.ReadLine(), out newCustomerLongitude)) ;
                    Console.WriteLine("Next Please enter the latitude of the Customer:");
                    while (!double.TryParse(Console.ReadLine(), out newCustomerLatitude));

                    Location customerlocation = new Location
                    {
                        longitude = newCustomerLongitude,
                        latitude = newCustomerLatitude
                    };

                    Customer newCustomer = new Customer
                    {
                        Id = newCustomerID,
                        Name = newCustomerName,
                        PhoneNumber = newPhoneNumber,
                        LocationOfCustomer=customerlocation
                    };

                    try
                    {
                        bl.AddCustomer(newCustomer);
                        Console.WriteLine("The operation was successful");
                    }

                    catch (AddAnExistingObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case InsertrOption.Parcel:
                    int newSenderId, newTargetId, newWeight, newPriorities;

                    Console.WriteLine(@"
You have selected to add a new Parcel.
Next Please enter the sender ID number:");
                    while (!int.TryParse(Console.ReadLine(), out newSenderId)) ;
                    Console.WriteLine("Next Please enter the target ID number:");
                    while (!int.TryParse(Console.ReadLine(), out newTargetId)) ;
                    Console.WriteLine("Next enter the weight category of the new Parcel: 0 for light, 1 for medium and 2 for heavy ");
                    while (!int.TryParse(Console.ReadLine(), out newWeight)) ;
                    Console.WriteLine("Next enter the priorities of the new Parcel: 0 for regular, 1 for fast and 2 for urgent");
                    while (!int.TryParse(Console.ReadLine(), out newPriorities)) ;


                    CustomerInDelivery newSender = new CustomerInDelivery{Id = newSenderId,};
                    CustomerInDelivery newReciver = new CustomerInDelivery {Id = newTargetId,};

                    Parcel newParcel = new Parcel
                    {
                        Sender = newSender,
                        Receiver= newReciver,
                        Weight = (WeightCategories)newWeight,
                        Prior = (Priorities)newPriorities
                    };

                    try
                    {
                        bl.AddParcel(newParcel);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                default:
                    Console.WriteLine(@"you entered a wrong number.
please choose again");
                    break;
            }
        }
        #endregion Handling insert options

        #region Handling update options
        /// <summary>
        /// The function handles various update options.
        /// </summary>
        /// <param name="bl">bl object that is passed as a parameter to enable the functions in the BL class</param>
        static public void UpdateOptions(BlApi.IBL bl)
        {
            Console.WriteLine(@"
Update options:
1. Drone        (new modal name)
2. Base station (new name or new chrage slots number )
3. Customer     (new name or new phone number)
4. Sending a drone for charging at a base station
5. Release drone from charging at base station
6. Assigning a package to a drone  
7. Collection of a package by drone
8. Delivery package to customer
Your choice:");
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice));

            int  droneId, baseStationId, customerId;
            string phoneNumber, droneName, baseName, customerName, chargeslots;
            DateTime time;

            switch ((UpdatesOption)choice)
            {
                case UpdatesOption.DroneUpdate:
                    Console.WriteLine("please enter drone ID for update:");
                    while(!int.TryParse(Console.ReadLine(), out droneId));
                    Console.WriteLine("Next Please enter the new Modal name:");
                    droneName = Console.ReadLine();

                    try
                    {
                        bl.UpdateDroneName(droneId, droneName);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case UpdatesOption.BaseStaitonUpdate:
                    Console.WriteLine("please enter base station ID for update:");
                    while (!int.TryParse(Console.ReadLine(), out baseStationId));
                    Console.WriteLine("Next Please enter the new base station name if not send empty line:");
                    baseName = Console.ReadLine();
                    Console.WriteLine("please enter update for the Charge slots number:");
                    chargeslots = Console.ReadLine();
                    try
                    {
                        bl.UpdateBaseStaison(baseStationId, baseName, chargeslots);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (MoreDroneInChargingThanTheProposedChargingStations ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case UpdatesOption.CustomerUpdate:
                    Console.WriteLine("please enter Customer ID for update:");
                    while (!int.TryParse(Console.ReadLine(), out customerId));
                    Console.WriteLine("Next Please enter the new customer Name:");
                    customerName = Console.ReadLine();
                    Console.WriteLine("please enter update for the new phone Number:");
                    phoneNumber = Console.ReadLine();

                    try
                    {
                        bl.UpdateCustomer(customerId, customerName, phoneNumber);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case UpdatesOption.InCharging:
                    Console.WriteLine("please enter Drone ID:");
                    while (!int.TryParse(Console.ReadLine(), out droneId)); 

                    try
                    {
                        bl.SendingDroneforCharging(droneId);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (TheDroneCanNotBeSentForCharging ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;

                case UpdatesOption.Outcharging:
                    Console.WriteLine("please enter Drone ID:");
                    while (!int.TryParse(Console.ReadLine(), out droneId));
                    Console.WriteLine("Please enter the length of time the drone has been charging:");
                    DateTime.TryParse(Console.ReadLine(), out time);

                    try
                    {
                        bl.ReleaseDroneFromCharging(droneId);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (OnlyMaintenanceDroneWillBeAbleToBeReleasedFromCharging ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case UpdatesOption.AssignDrone:          
                    Console.WriteLine("please enter Drone ID:");
                    while (!int.TryParse(Console.ReadLine(), out droneId));

                    try
                    {
                        bl.AssignPackageToDdrone(droneId);
                        Console.WriteLine("The operation was successful");
                    } 
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (NoSuitablePsrcelWasFoundToBelongToTheDrone ex) 
                    {
                        Console.WriteLine(ex);
                    }
                    catch(DroneCantBeAssigend ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case UpdatesOption.PickUp:
                    Console.WriteLine("please enter Drone ID:");
                    while (!int.TryParse(Console.ReadLine(), out droneId));

                    try
                    {
                        bl.PickedUpPackageByTheDrone(droneId);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (UnableToCollectParcel ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case UpdatesOption.Deliverd:
                    Console.WriteLine("please enter Drone ID:");
                    while (!int.TryParse(Console.ReadLine(), out droneId));

                    try
                    {
                        bl.DeliveryPackageToTheCustomer(droneId);
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    catch (DeliveryCannotBeMade ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;
                default:
                    Console.WriteLine(@"you entered a wrong number.
please choose again");
                    break;
            }
        }
        #endregion Handling update options

        #region Handling display options
        /// <summary>
        /// The function handles display options.
        /// </summary>
        /// <param name="bl">BL object that is passed as a parameter to enable the functions in the bl class</param>
        static public void DisplaySingleOptions(BlApi.IBL bl)
        {
            Console.WriteLine(@"
Display options(single):

1. Base station view.
2. Drone display.
3. Customer view.
4. Package view.

Your choice:");
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice));

            int idForDisplayObject;

            switch ((DisplaySingleOption)choice)
            {
                case DisplaySingleOption.BaseStationView:
                    Console.WriteLine("Insert ID number of base station:");
                    while (!int.TryParse(Console.ReadLine(), out idForDisplayObject));

                    try
                    {
                        Console.WriteLine(bl.GetBaseStation(idForDisplayObject).ToString());
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case DisplaySingleOption.DroneDisplay:
                    Console.WriteLine("Insert ID number of requsted drone:");
                    while (!int.TryParse(Console.ReadLine(), out idForDisplayObject));

                    try
                    {
                        Console.WriteLine(bl.GetDrone(idForDisplayObject).ToString());
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case DisplaySingleOption.CustomerView:
                    Console.WriteLine("Insert ID number of requsted Customer:");
                    while (!int.TryParse(Console.ReadLine(), out idForDisplayObject));

                    try
                    {
                        Console.WriteLine(bl.GetCustomer(idForDisplayObject).ToString());
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }
                    break;

                case DisplaySingleOption.PackageView:
                    Console.WriteLine("Insert ID number of reqused parcel:");
                    while (!int.TryParse(Console.ReadLine(), out idForDisplayObject));

                    try
                    {
                        Console.WriteLine(bl.GetParcel(idForDisplayObject).ToString());
                        Console.WriteLine("The operation was successful");
                    }
                    catch (NonExistentObjectException ex)
                    {
                        Console.WriteLine(ex);
                    }

                    break;
                default:
                    Console.WriteLine(@"you entered a wrong number.
please choose again");
                    break;
            }
        }
        #endregion Handling display options

        #region Handling the list display options
        /// <summary>
        /// The function prints the data array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToPrint"></param>
        public static void printTheList<T>(List<T> listToPrint) 
        {
            //Console.WriteLine(String.Join(" ",listToPrint));
            foreach (T item in listToPrint)
            {
                Console.WriteLine(item);
            }
        }
        /// <summary>
        /// The function handles list view options.
        /// </summary>
        /// <param name="bl">BL object that is passed as a parameter to enable the functions in the bl class</param>
        static public void DisplayListOptions(BlApi.IBL bl)
        {
            Console.WriteLine(@"
Display options (for the whole list):

1. Displays a list of base stations.
2. Displays the list of drone.
3. View the customer list.
4. Displays the list of packages.
5. Displays a list of packages that have not yet been assigned to the drone.
6. Display base stations with available charging stations.

Your choice:");
            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice));

            switch ((DisplayListOption)choice)
            {
                case DisplayListOption.ListOfBaseStationView:
                    printTheList(bl.GetBaseStationList().ToList());
                    break;

                case DisplayListOption.ListOfDronedisplay:
                    printTheList(bl.GetDroneList().ToList());
                    break;

                case DisplayListOption.ListOfCustomerView:
                    printTheList(bl.GetCustomerList().ToList());
                    break;

                case DisplayListOption.ListOfPackageView:
                    printTheList(bl.GetParcelList().ToList());
                    break;

                case DisplayListOption.ListOfFreePackageView:
                    printTheList(bl.GetParcelList(x => x.Status == DeliveryStatus.created).ToList());
                    break;

                case DisplayListOption.ListOfBaseStationsWithFreeChargSlots:
                    printTheList(bl.GetBaseStationList(x => x.FreeChargeSlots > 0).ToList());
                    break;

                default:
                    Console.WriteLine(@"you entered a wrong number.
please choose again");
                    break;
            }

        }
        #endregion Handling the list display options

        #endregion fanction of main

        static void Main(string[] args)
        {
            BlApi.IBL BLObject = BlFactory.GetBL();

            int choice;
            do
            {
                Console.WriteLine(@"
choose from the following options (type the selected number): 

1. Insert options.
2. Update options.
3. Display options(singel).
4. Display options (for the whole list).
5. EXIT.
Your choice:");
                while (!int.TryParse(Console.ReadLine(), out choice));

                switch ((Options)choice)
                {
                    case Options.Insert:
                        InsertOptions(BLObject);
                        break;

                    case Options.Update:
                        UpdateOptions(BLObject);
                        break;

                    case Options.DisplaySingle:
                        DisplaySingleOptions(BLObject);
                        break;

                    case Options.DisplayList:
                        DisplayListOptions(BLObject);
                        break;
                 
                    case Options.EXIT:
                        Console.WriteLine("Have a good day");
                        break;

                    default:
                        Console.WriteLine(@"you entered a wrong number.
please choose again");
                        break;
                }
            } while (!(choice == 5));
        }
    }
}
