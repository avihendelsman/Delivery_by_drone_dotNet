﻿using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for ListView.xaml
    /// </summary>
    public partial class ListView : Window
    {
        public BlApi.IBL AccessIbl = BlApi.BlFactory.GetBL();

        /// <summary> crate a observab list of type IBL.BO.DroneToList (to see changes in live) </summary>
        public ObservableCollection<BO.DroneToList> droneToLists;

        /// <summary> a bool to help us disable the x bootum  </summary>
        public bool ClosingWindow { get; private set; } = true;

        /// <summary>
        /// constractor for the ListWindow that will start the InitializeComponent ans fill the Observable Collection.
        /// </summary>
        /// <param name="bl">get AccessIbl from main win</param>
        public ListView()
        {
            InitializeComponent();

            //AccessIbl = bl;

            //craet observer and set the list accordale to ibl drone list 
            droneToLists = new ObservableCollection<DroneToList>();
            List<BO.DroneToList> drones = AccessIbl.GetDroneList().ToList();
            foreach (var item in drones)
            {
                droneToLists.Add(item);
            }

            //new event that will call evre time that the ObservableCollection didact a change 
            droneToLists.CollectionChanged += DroneToLists_CollectionChanged;

            //display the defult list 
            listOfDrones.ItemsSource = droneToLists;

            //combobox of dronesList handling.
            CBStatusSelector.ItemsSource = Enum.GetValues(typeof(DroneStatuses));
            CBWeightSelctor.ItemsSource = Enum.GetValues(typeof(WeightCategories));
        }

        /// <summary>
        /// /// a new event that we crate in the intaklizer :DroneToLists_CollectionChanged:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>   
        private void DroneToLists_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StatusSelectorChanged();
        }

        #region combobox of dronesList handling
        /// <summary>
        /// help fanction to choose what to show on the user side accordingly to user cohises ( bonous)
        /// </summary>
        public void StatusSelectorChanged() //אולי לעשות סוויץ
        {
            if (CBWeightSelctor.SelectedItem == null && CBStatusSelector.SelectedItem == null)
            {
                listOfDrones.ItemsSource = droneToLists.ToList();
            }
            else if (CBWeightSelctor.SelectedItem == null)
            {
                listOfDrones.ItemsSource = droneToLists.ToList().FindAll(x => x.Statuses == (DroneStatuses)CBStatusSelector.SelectedIndex);
            }
            else if (CBStatusSelector.SelectedItem == null)
            {
                listOfDrones.ItemsSource = droneToLists.ToList().FindAll(x => x.MaxWeight == (WeightCategories)CBWeightSelctor.SelectedIndex);
            }
            else //
            {
                listOfDrones.ItemsSource = droneToLists.ToList().FindAll(x => x.Statuses == (DroneStatuses)CBStatusSelector.SelectedIndex && x.MaxWeight == (WeightCategories)CBWeightSelctor.SelectedIndex);
            }
        }

        /// <summary>
        /// show on the user side accordingly to user cohises
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CBStatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StatusSelectorChanged();
        }

        /// <summary>
        /// show on the user side accordingly to user cohises
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CBWeightSelctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StatusSelectorChanged();
        }

        /// <summary>
        /// restart modem to set option to default
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BResetStatus_Click(object sender, RoutedEventArgs e)
        {
            CBStatusSelector.SelectedItem = null;
            StatusSelectorChanged();
        }

        /// <summary>
        /// restart modem to set option to default
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BResetWeight_Click(object sender, RoutedEventArgs e)
        {
            CBWeightSelctor.SelectedItem = null;
            StatusSelectorChanged();
        }




        #endregion combobox of dronesList handling

        /// <summary>
        /// open drone window in acction when didect double clik
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lists_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DroneToList drone = (DroneToList)listOfDrones.SelectedItem;
            if (drone != null)// if the user click on empty space in the view list we donr open anything
            {
                int indexDrone = listOfDrones.SelectedIndex;
                this.IsEnabled = false; // to privent anotur click on the list window chosse we donr want alot of windows togter.
                new DroneWindow(AccessIbl, this, drone, indexDrone).Show();//open the drone windowon acction
            }
        }

        #region close
        /// <summary>
        /// cancel the option to clik x to close the window 
        /// </summary>
        /// <param name="e">close window</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = ClosingWindow;
        }

        /// <summary>
        /// to aloow closing again but just in the spcific close boutoon 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Bclose_Click_1(object sender, RoutedEventArgs e)
        {
            ClosingWindow = false; // we alowd the close option
            Close();
        }
        #endregion close

        /// <summary>
        /// Add drone button which activates the builder in the Add drone window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            // we send ""this"" window becuse we want to use it in the new window
            new DroneWindow(AccessIbl, this).Show();
            this.IsEnabled = false;
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
//using BO;

//namespace PL
//{
//    /// <summary>
//    /// Interaction  logic for DroneListWindow.xaml
//    /// </summary>
//    public partial class DroneListWindow : Window
//    {
//        #region drone list view
//        public BlApi.IBL AccessIbl;

//        /// <summary> crate a observab list of type IBL.BO.DroneToList (to see changes in live) </summary>
//        public ObservableCollection<BO.DroneToList> droneToLists;

//        /// <summary> a bool to help us disable the x bootum  </summary>
//        public bool ClosingWindow { get; private set; } = true;

//        /// <summary>
//        /// constractor for the DroneListWindow that will start the InitializeComponent ans fill the Observable Collection
//        /// </summary>
//        /// <param name="bl">get AccessIbl from main win</param>
//        public DroneListWindow(BlApi.IBL bl)
//        {
//            InitializeComponent();
//            AccessIbl = bl;
//            //craet observer and set the list accordale to ibl drone list 
//            droneToLists = new ObservableCollection<DroneToList>();
//            List<BO.DroneToList> drones = bl.GetDroneList().ToList();
//            foreach (var item in drones)
//            {
//                droneToLists.Add(item);
//            }
//            //new event that will call evre time that the ObservableCollection didact a change 
//            droneToLists.CollectionChanged += DroneToLists_CollectionChanged;
//            //display the defult list 
//            DroneListView.ItemsSource = droneToLists;
//            StatusSelector.ItemsSource = Enum.GetValues(typeof(DroneStatuses));
//            WeightSelctor.ItemsSource = Enum.GetValues(typeof(WeightCategories));
//        }

//        /// <summary>
//        /// a new event that we crate in the intaklizer :DroneToLists_CollectionChanged:
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void DroneToLists_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
//        {
//            StatusSelectorChanged();
//        }

//        /// <summary>
//        /// help fanction to choose what to show on the user side accordingly to user cohises ( bonous)
//        /// </summary>
//        public void StatusSelectorChanged()
//        {
//            if (WeightSelctor.SelectedItem == null && StatusSelector.SelectedItem == null)
//            {
//                DroneListView.ItemsSource = droneToLists.ToList();
//            }
//            else if (WeightSelctor.SelectedItem == null)
//            {
//                DroneListView.ItemsSource = droneToLists.ToList().FindAll(x => x.Statuses == (DroneStatuses)StatusSelector.SelectedIndex);
//            }
//            else if (StatusSelector.SelectedItem == null)
//            {
//                DroneListView.ItemsSource = droneToLists.ToList().FindAll(x => x.MaxWeight == (WeightCategories)WeightSelctor.SelectedIndex);
//            }
//            else
//            {
//                DroneListView.ItemsSource = droneToLists.ToList().FindAll(x => x.Statuses == (DroneStatuses)StatusSelector.SelectedIndex && x.MaxWeight == (WeightCategories)WeightSelctor.SelectedIndex);
//            }
//        }

//        /// <summary>
//        /// show on the user side accordingly to user cohises
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            StatusSelectorChanged();
//        }

//        /// <summary>
//        /// show on the user side accordingly to user cohises
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void weightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            StatusSelectorChanged();     
//        }

//        /// <summary>
//        /// restart modem to set all options to default
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void res_Click(object sender, RoutedEventArgs e)
//        {
//            StatusSelector.SelectedItem = null;
//            WeightSelctor.SelectedItem = null;
//            StatusSelectorChanged();
//        }

//        /// <summary>
//        /// Add drone button which activates the builder in the Add drone window
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void BAddDrone_Click(object sender, RoutedEventArgs e)
//        {
//            // we send ""this"" window becuse we want to use it in the new window
//            new DroneWindow(AccessIbl, this).Show();
//            this.IsEnabled = false;
//        }

//        /// <summary>
//        /// cancel the option to clik x to close the window 
//        /// </summary>
//        /// <param name="e">close window</param>
//        protected override void OnClosing(CancelEventArgs e)
//        {
//            e.Cancel = ClosingWindow;
//        }

//        /// <summary>
//        /// to aloow closing again but just in the spcific close boutoon 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void Bclose_Click(object sender, RoutedEventArgs e)
//        {
//            ClosingWindow = false; // we alowd the close option
//            Close();
//        }
//        /// <summary>
//        /// open drone window in acction when didect double clik
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void DroneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//        {
//            DroneToList drone = (DroneToList)DroneListView.SelectedItem;
//            if (drone != null)// if the user click on empty space in the view list we donr open anything
//            {
//                int indexDrone = DroneListView.SelectedIndex;
//                this.IsEnabled = false; // to privent anotur click on the list window chosse we donr want alot of windows togter.
//                new DroneWindow(AccessIbl, this,drone, indexDrone).Show();//open the drone windowon acction
//            }
//        }
//        #endregion
//    }
//}
