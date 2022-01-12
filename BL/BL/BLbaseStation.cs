﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using BO;

namespace BL
{
    partial class BL
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(BaseStation newbaseStation)
        {

            DO.BaseStation newStation = new DO.BaseStation()
            {
                Id = newbaseStation.Id,
                StationName = newbaseStation.Name,
                FreeChargeSlots = newbaseStation.FreeChargeSlots,
                Longitude = newbaseStation.BaseStationLocation.longitude,
                Latitude = newbaseStation.BaseStationLocation.latitude
            };

            try    // throw if the id is exsist
            {
                AccessIdal.AddStation(newStation);
            }
            catch (DO.AddAnExistingObjectException)
            {
                throw new AddAnExistingObjectException();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateBaseStaison(int baseStationId, string baseName, string chargeslots)
        {
            DO.BaseStation newbase = new DO.BaseStation();
            try //Check if there is such a station in the database.
            {
                newbase = AccessIdal.GetBaseStation(baseStationId);
            }
            catch (DO.NonExistentObjectException)
            {
                throw new NonExistentObjectException("BaseStation");
            }

            if (baseName != "") //if it is not empty.
            {
                newbase.StationName = baseName;
            }

            if (chargeslots != "") ////if it is not empty.
            {
                int.TryParse(chargeslots, out int totalQuantityChargeSlots);

                int numOfBuzeChargeslots = AccessIdal.GetBaseChargeList(x => x.StationId == baseStationId).Count();

                //chaeck if More Drone In Charging Than The Proposed Charging Stations
                if (totalQuantityChargeSlots - numOfBuzeChargeslots < 0)
                {
                    throw new MoreDroneInChargingThanTheProposedChargingStations();
                }
                newbase.FreeChargeSlots = totalQuantityChargeSlots - numOfBuzeChargeslots; //else
            }

            AccessIdal.UpdateBaseStation(newbase); //Update in the database.
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public BaseStation GetBaseStation(int idForDisplayObject)
        {
            DO.BaseStation printBase = new DO.BaseStation();
            // check if Non Existent BaseStation 
            try
            {
                printBase = AccessIdal.GetBaseStation(idForDisplayObject);
            }
            catch (DO.NonExistentObjectException)
            {
                throw new NonExistentObjectException("BaseStation");
            }

            BaseStation blBase = new BaseStation()
            {
                Id = printBase.Id,
                Name = printBase.StationName,
                BaseStationLocation
                = new Location() { longitude = printBase.Longitude, latitude = printBase.Latitude },
                FreeChargeSlots = printBase.FreeChargeSlots
            };

            blBase.DroneInChargsList = from item in AccessIdal.GetBaseChargeList(x => x.StationId == idForDisplayObject)
                                       select new DroneInCharg { Id = item.DroneId, BatteryStatus = DronesBL.Find(x => x.Id == item.DroneId).BatteryStatus };
            return blBase;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<BaseStationsToList> GetBaseStationList(Predicate<BaseStationsToList> predicate = null)
        {
            IEnumerable<BaseStationsToList> baseStationBL = from item in AccessIdal.GetBaseStationList()
                                                            select new BaseStationsToList()
                                                            {
                                                                Id = item.Id,
                                                                Name = item.StationName,
                                                                FreeChargeSlots = item.FreeChargeSlots,
                                                                BusyChargeSlots = AccessIdal.GetBaseChargeList(x => x.StationId == item.Id).Count()
                                                            };
            return baseStationBL.Where(x => predicate == null ? true : predicate(x));
        }
    }
}
