﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using IBL.BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        public IBL.IBL AccessIbl;
        /// <summary> a bool to help us disable the x bootum  </summary>
        public bool ClosingWindow { get; private set; } = true;
        /// <summary> the calling window, becuse we want to use it here </summary>
        /// 
        private DroneListWindow DroneListWindow;
        /// <summary>
        /// consractor for add drone option 
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="_DroneListWindow"></param>
        public DroneWindow(IBL.IBL bl, DroneListWindow _DroneListWindow)
        {
            InitializeComponent();
            addDrone.Visibility = Visibility.Visible;
            AccessIbl = bl;
            DroneListWindow = _DroneListWindow;
            // the combobox use it to show the Weight Categories
            TBWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            // the combobox use it to show the BaseStation ID
            BaseStationID.ItemsSource = AccessIbl.GetBaseStationList(x => x.FreeChargeSlots > 0);
            BaseStationID.DisplayMemberPath = "Id";
            //if (!AccessIbl.GetBaseStationList(x => x.FreeChargeSlots == 0).Any())
            //{
            //    MessageBox.Show("אין תחנות עם עמדות הטענה פנויות ","מידע", MessageBoxButton.OK, MessageBoxImage.None);              
            //}
        }


        /// <summary>
        /// disable the non numbers keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBID_KeyDown(object sender, KeyEventArgs e)
        {
            TBID.BorderBrush = Brushes.Gray;
            // take only the kyes we alowed 
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {
                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9) // we want keys from the num pud too
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            if (TBID.Text.Length > 8)
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// linited the langth of the text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBModel_KeyDown(object sender, KeyEventArgs e)
        {
            TBID.BorderBrush = Brushes.Gray;
            if (TBModel.Text.Length > 5)
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// A function that sends the new drone and adds it to the data after tests in the logical layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendToBl_Click(object sender, RoutedEventArgs e)
        {
            // If all the fields are full
            if (TBModel.Text.Length != 0 && TBID.Text.Length != 0 && BaseStationID.SelectedItem != null && TBWeight.SelectedItem != null)
            {
                DroneToList newdrone = new DroneToList
                {
                    Id = int.Parse(TBID.Text),
                    Model = TBModel.Text,
                    MaxWeight = (WeightCategories)TBWeight.SelectedIndex,
                };
                // try to add the drone if fals return a MessageBox
                try
                {
                    AccessIbl.AddDrone(newdrone, ((BaseStationsToList)BaseStationID.SelectedItem).Id);
                    MessageBoxResult result = MessageBox.Show("The operation was successful", "info", MessageBoxButton.OK, MessageBoxImage.Information);//לטלפל בX
                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            newdrone = AccessIbl.GetDroneList().ToList().Find(i => i.Id == newdrone.Id);
                            DroneListWindow.droneToLists.Add(newdrone);
                            ClosingWindow = false;
                            Close();
                            break;
                        default:
                            break;
                    }
                }
                catch (AddAnExistingObjectException ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    TBID.Text = "";
                    TBID.BorderBrush = Brushes.Red; //בונוס 
                }
                catch (NonExistentObjectException ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    TBID.Text = "";
                    TBID.BorderBrush = Brushes.Red;//בונוס 
                }
                catch (NoFreeChargingStations ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (NonExistentEnumException ex)
                {
                    MessageBox.Show(ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("נא ודאו שכל השדות מלאים", "!שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        /// <summary>
        /// to aloow closing again but just in the spcific close boutoon 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bclose_Click(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false;
            Close();
        }
        /// <summary>
        /// cancel the option to clik x to close the window 
        /// </summary>
        /// <param name="e">close window</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = ClosingWindow;
        }

        ///~~~~~~~~~~~~~~~~~~~~~~~~~~~~רחפן בפעולות~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~///

        public DroneWindow(IBL.IBL bl, DroneListWindow _DroneListWindow, int id)
        {
            InitializeComponent();
            updateDrone.Visibility = Visibility.Visible;
            AccessIbl = bl;
            Drone drone = bl.GetDrone(id);
            TBID2.Text = drone.Id.ToString();
            TBmodel.Text = drone.Model.ToString();
            TBWeightCategories.Text = drone.MaxWeight.ToString();
            TBBatrryStatuses.Text = drone.BatteryStatus.ToString();
            TBDroneStatuses.Text = drone.Statuses.ToString();
            TBLocation.Text = drone.CurrentLocation.ToString(); 
            TBparcelInDelivery.Text = drone.Delivery.ToString();

            //DroneListWindow = _DroneListWindow;
            //// the combobox use it to show the Weight Categories
            //TBWeight.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            //// the combobox use it to show the BaseStation ID
            //BaseStationID.ItemsSource = AccessIbl.GetBaseStationList(x => x.FreeChargeSlots > 0);
            //BaseStationID.DisplayMemberPath = "Id";
            ////if (!AccessIbl.GetBaseStationList(x => x.FreeChargeSlots == 0).Any())
            ////{
            ////    MessageBox.Show("אין תחנות עם עמדות הטענה פנויות ","מידע", MessageBoxButton.OK, MessageBoxImage.None);              
            ////}
        }


    }
}
