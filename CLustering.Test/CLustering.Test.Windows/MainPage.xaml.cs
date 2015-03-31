using Bing.Maps;
using EasyClustering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CLustering.Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        #region Fields
        private double zoomLevelDouble = 0;
        #endregion

        #region Properties
        private int zoom;

        public int ZoomLevel
        {
            get { return zoom; }
            set
            {
                zoom = value;
                RaisePropertyChanged("ZoomLevel");
            }
        }

        private Bounds _bounds;

        public Bounds Bounds
        {
            get { return _bounds; }
            set { _bounds = value; RaisePropertyChanged("Bounds"); }
        }

        public EasyClustering.ItemCollection Items
        {
            get
            {
                return items;
            }
        }

        private readonly EasyClustering.ItemCollection items;

        private bool reloadPoint;

        public bool ReloadPoint
        {
            get { return reloadPoint; }
            set
            {
                reloadPoint = value;
                RaisePropertyChanged("ReloadPoint");
                if (ReloadPoint)
                    reloadPoint = false;
            }
        }

        private bool centerPoint;

        public bool CenterPoint
        {
            get { return centerPoint; }
            set { centerPoint = value; RaisePropertyChanged("CenterPoint"); }
        }


        private ObservableCollection<Pas> listPas;

        public ObservableCollection<Pas> ListPas
        {
            get { return listPas; }
            set { listPas = value; }
        }
        #endregion


        public MainPage()
        {
            this.InitializeComponent();
            Bounds = new EasyClustering.Bounds();
            items = new EasyClustering.ItemCollection();

            //mapItemControlPin.ItemsSource = clusterToolXaml.CurrentShownItem;
            CenterPoint = false;

            ListPas = new ObservableCollection<Pas>();

            ListPas.Add(new Pas(1, 2, 100));
            ListPas.Add(new Pas(3, 5, 10));
            ListPas.Add(new Pas(6, 8, 1));
            ListPas.Add(new Pas(9, 10, 0.3));
            ListPas.Add(new Pas(11, 12, 0.1));
            ListPas.Add(new Pas(13, 13, 0.05));
            ListPas.Add(new Pas(14, 15, 0.01));
            ListPas.Add(new Pas(16, 19, 0.0005));
            ListPas.Add(new Pas(20, 20, 0.0001));
            ListPas.Add(new Pas(16, 16, 0.005));
            ListPas.Add(new Pas(17, 17, 0.001));
            ListPas.Add(new Pas(18, 18, 0.0005));
            ListPas.Add(new Pas(19, 19, 0.0002));
            ListPas.Add(new Pas(20, 20, 0.00001));
            DataContext = this;
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (var elem in MockData.GenerateMock(10000))
            {
                Items.Add(elem);
            }
            ZoomLevel = 0;
            ZoomLevel = (int)map.ZoomLevel;
            //await clusterToolXaml.GenerateClusterData();
        }



        private async void map_ViewChangeEnded(object sender, ViewChangeEndedEventArgs e)
        {
            Debug.WriteLine("map_ViewChangeEnded " + DateTimeOffset.Now.Ticks);



            Bounds.East = (sender as Bing.Maps.Map).Bounds.East;
            Bounds.North = (sender as Bing.Maps.Map).Bounds.North;
            Bounds.West = (sender as Bing.Maps.Map).Bounds.West;
            Bounds.South = (sender as Bing.Maps.Map).Bounds.South;

            if (zoomLevelDouble != map.ZoomLevel)
            {
                ZoomLevel = 0;
                //map.Children.Clear();
                ZoomLevel = (int)map.ZoomLevel;
            }
            ReloadPoint = true;
            //await clusterToolXaml.GenerateClusterData();
            zoomLevelDouble = map.ZoomLevel;
        }


        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var current = sender as Pushpin;

            ClusterObjet cluster = current.DataContext as ClusterObjet;

            map.SetView(
                new LocationRect(
                    new Bing.Maps.Location()
                    {
                        Latitude = cluster.Boundaries.North,
                        Longitude = cluster.Boundaries.West
                    },
                    new Bing.Maps.Location()
                    {
                        Latitude = cluster.Boundaries.South,
                        Longitude = cluster.Boundaries.East 
                    }));

            //Debug.WriteLine(current.Text);
            //Debug.WriteLine(String.Format("Count: {0} ; Latitude: {1} ; Longitude: {2}",
            //    current.Text,
            //    (current.DataContext as ItemObjet).Location.Latitude,
            //    (current.DataContext as ItemObjet).Location.Longitude));
        }

        public double CalculateZoomLevel(LocationRect boundingBox, double buffer, Map map)
        {
            double zoom1 = 0, zoom2 = 0;

            //best zoom level based on map width
            zoom1 = Math.Log(360.0 / 256.0 * (map.ActualWidth - 2 * buffer) / boundingBox.Width) / Math.Log(2);

            //best zoom level based on map height
            zoom2 = Math.Log(180.0 / 256.0 * (map.ActualHeight - 2 * buffer) / boundingBox.Height) / Math.Log(2);

            //use the most zoomed out of the two zoom levels
            var zoomLevel = (zoom1 < zoom2) ? zoom1 : zoom2;

            return zoomLevel;
        }

        #region INotify

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            Animation.Grow(sender as UIElement);
        }
    }
}
