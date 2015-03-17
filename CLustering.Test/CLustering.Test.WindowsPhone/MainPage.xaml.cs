using EasyClustering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
        DispatcherTimer timer = new DispatcherTimer();
        double value = 0;
        double valueBis = -1;
        private double zoomLevelDouble = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

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

        public MainPage()
        {
            this.InitializeComponent();
            Bounds = new Bounds();
            items = new EasyClustering.ItemCollection();

            ListPas = new ObservableCollection<Pas>();
            //ListPas.Add(new Pas(1, 1, 1));
            //ListPas.Add(new Pas(2, 2, 0.5));
            //ListPas.Add(new Pas(3, 5, 0.2));
            //ListPas.Add(new Pas(6, 7, 0.1));
            //ListPas.Add(new Pas(8, 9, 0.08));
            //ListPas.Add(new Pas(10, 11, 0.05));
            //ListPas.Add(new Pas(12, 13, 0.03));
            //ListPas.Add(new Pas(14, 14, 0.01));
            //ListPas.Add(new Pas(15, 15, 0.008));
            //ListPas.Add(new Pas(16, 16, 0.005));
            //ListPas.Add(new Pas(17, 17, 0.001));
            //ListPas.Add(new Pas(18, 18, 0.0005));
            //ListPas.Add(new Pas(19, 19, 0.0002));
            //ListPas.Add(new Pas(20, 20, 0.00001));

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
            //MapItemsControl.ItemsSource = clusterToolXaml.CurrentShownItem;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            CenterPoint = false;



            //MapPositionChanged();
            MapPositionChangedComplete();
        }

        private void MapPositionChangedComplete()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_Tick;

            TypedEventHandler<MapControl, object> handler = null;
            handler = (sender, e) =>
            {
                if (!timer.IsEnabled)
                {
                    timer.Start();
                }
                value = DateTime.Now.Ticks;
            };
            map.CenterChanged += handler;

        }

        async void timer_Tick(object sender, object e)
        {
            Debug.WriteLine(string.Format("value {0} valueBis {1}", value, valueBis));
            if (value != valueBis)
            {
                valueBis = value;
            }
            else
            {
                timer.Stop();
                GeoboundingBox geoBox = map.GetBounds();

                Bounds.East = geoBox.SoutheastCorner.Longitude;
                Bounds.North = geoBox.NorthwestCorner.Latitude;
                Bounds.West = geoBox.NorthwestCorner.Longitude;
                Bounds.South = geoBox.SoutheastCorner.Latitude;

                if (zoomLevelDouble != map.ZoomLevel)
                {
                    ZoomLevel = 0;
                    ZoomLevel = (int)map.ZoomLevel;
                }
                ReloadPoint = true;
                //await clusterToolXaml.GenerateClusterData();
                zoomLevelDouble = map.ZoomLevel;
            }
        }


        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            map.Children.Clear();
            foreach (var elem in MockData.GenerateMock(1000))
            {
                Items.Add(elem);
            }
            ZoomLevel = 0;
            ZoomLevel = (int)map.ZoomLevel;
            ReloadPoint = true;
            //await clusterToolXaml.GenerateClusterData();
        }

    }
}
