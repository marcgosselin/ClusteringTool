using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Windows.UI.Core;
using System.Collections.ObjectModel;

namespace EasyClustering
{
    public class ClusterItem : UserControl
    {
        #region Fields
        private bool isZoomChanged = false;
        private Bounds _oldBound = new Bounds() { East = -200, North = -200, South = -200, West = -200 };
        private double pas = 100;

        private static DispatcherTimer timer = new DispatcherTimer();
        static double value = 0;
        static double valueBis = -1;
        #endregion

        #region Dependency Properties



        public bool CenterPoint
        {
            get { return (bool)GetValue(CenterPointProperty); }
            set { SetValue(CenterPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CenterPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CenterPointProperty =
            DependencyProperty.Register("CenterPoint", typeof(bool), typeof(ClusterItem), new PropertyMetadata(true));



        public ItemCollection collection
        {
            get
            {
                return (ItemCollection)GetValue(collectionProperty);
            }
            set
            {
                SetValue(collectionProperty, value);
                //GenerateClusterData();
            }
        }

        // Using a DependencyProperty as the backing store for collection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty collectionProperty =
            DependencyProperty.Register("collection",
            typeof(ItemCollection),
            typeof(ClusterItem),
            new PropertyMetadata(null, new PropertyChangedCallback((d, e)
                =>
            {
                try
                {
                    var sender = d as ClusterItem;
                    sender.collection.CollectionChanged += collection_CollectionChanged;
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            })));

        static void collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal, () =>
                        {
                            if (!timer.IsEnabled)
                            {
                                timer.Start();
                            }
                            value = DateTime.Now.Ticks;
                        });
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public bool ReloadPoint
        {
            get { return (bool)GetValue(ReloadPointProperty); }
            set
            {
                SetValue(ReloadPointProperty, value);
                if (value)
                {
                    GenerateClusterData();
                }
            }
        }

        public static readonly DependencyProperty ReloadPointProperty =
            DependencyProperty.Register("ReloadPoint",
            typeof(bool),
            typeof(ClusterItem),
            new PropertyMetadata(false,new PropertyChangedCallback(ReloadPointChangedCallBack)));

        public ItemCollection CurrentShownItem
        {
            get
            {
                return (ItemCollection)GetValue(CurrentShownItemProperty);
            }
            set
            {
                SetValue(CurrentShownItemProperty, value);
            }
        }

        public static readonly DependencyProperty CurrentShownItemProperty =
            DependencyProperty.Register("CurrentShownItem", typeof(ItemCollection), typeof(ClusterItem),
                new PropertyMetadata(null));

        public int Zoom
        {
            get
            {
                return (int)GetValue(ZoomProperty);
            }
            set
            {
                SetValue(ZoomProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Zoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register("Zoom",
            typeof(int), typeof(ClusterItem),
            new PropertyMetadata(default(int), new PropertyChangedCallback(ZoomChangedCallBack)));



        public Bounds Boundaries
        {
            get { return (Bounds)GetValue(BoundariesProperty); }
            set { SetValue(BoundariesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Boundaries.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoundariesProperty =
            DependencyProperty.Register("Boundaries",
            typeof(Bounds),
            typeof(ClusterItem),
            new PropertyMetadata(0));



        public ObservableCollection<Pas> ListPas
        {
            get { return (ObservableCollection<Pas>)GetValue(ListPasProperty); }
            set { SetValue(ListPasProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ListPas.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ListPasProperty =
            DependencyProperty.Register("ListPas", typeof(ObservableCollection<Pas>), typeof(ClusterItem), new PropertyMetadata(null));

        

        #endregion


        public ClusterItem()
        {
            CurrentShownItem = new ItemCollection();
            ConfigureTimer();
        }

        private void ConfigureTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_Tick;
        }

        private async void timer_Tick(object sender, object e)
        {
            try
            {
                if (value != valueBis)
                {
                    valueBis = value;
                }
                else
                {
                    timer.Stop();
                    isZoomChanged = true;
                    CurrentShownItem.Clear();
                    GenerateClusterData();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("timer_Tick " + ex.Message);
            }
        }

        public async static void ReloadPointChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var clusterItem = d as ClusterItem;
            if (clusterItem.ReloadPoint)
            {
                clusterItem.ReloadPoint = ! clusterItem.ReloadPoint;
                await clusterItem.GenerateClusterData();
            }
        }

        private static void ZoomChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                var item = d as ClusterItem;
                item.isZoomChanged = true;
                item.pas = (from p in item.ListPas
                           where p.Min >= item.Zoom && item.Zoom <= p.Max
                           select p.Value).FirstOrDefault();
            }
            catch (Exception ex)
            {

            }
        }

        
        public async Task GenerateClusterData()
        {
            Debug.WriteLine("GenerateClusterData " + DateTimeOffset.Now.Ticks);


            if (collection != null && collection.Count > 0)
            {
                if (isZoomChanged)
                {
                    var _col = collection;
                    isZoomChanged = false;
                    Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal, () =>
                        {
                            CurrentShownItem.Clear();
                        });
                    await GenerateWithZoomChange(_col).ContinueWith((continuation) =>
                   {

                       Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                           CoreDispatcherPriority.Normal, () =>
                           {

                               _oldBound.East = this.Boundaries.East;
                               _oldBound.North = this.Boundaries.North;
                               _oldBound.West = this.Boundaries.West;
                               _oldBound.South = this.Boundaries.South;
                           });

                   });
                }
                else
                {
                    //redessin partiel
                    await GenerateWithoutZoomChange().ContinueWith((continuation) =>
                     {
                         Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                         {

                             _oldBound.East = this.Boundaries.East;
                             _oldBound.North = this.Boundaries.North;
                             _oldBound.South = this.Boundaries.South;
                             _oldBound.West = this.Boundaries.West;

                         });
                     });
                }
            }
        }

        private async Task GenerateWithoutZoomChange()
        {
            var _col = collection;
            var _zoom = Zoom;

            await RunLogicClusterWithoutZoom(_zoom, _col, pas);
        }

        private async Task GenerateWithZoomChange(IEnumerable<ItemObjet> _col)
        {

            var _bound = Boundaries;
            var _zoom = Zoom;
            //calcul de la moyenne des latitude
            var latAverage = _bound.South + _bound.North / 2;

            var minBound = Math.Min(_bound.South, _bound.North);
            var maxBound = Math.Max(_bound.South, _bound.North);

            //attention au longitude, si on montre plus 
            var isMapBig = false;
            if (_bound.West > _bound.East)
            {
                isMapBig = true;
            }

            var minBoundWE = Math.Min(_bound.West, _bound.East);
            var maxBoundWE = Math.Max(_bound.West, _bound.East);



            Debug.WriteLine(string.Format("West {0} East {1}", _bound.West, _bound.East));

            var center = CenterPoint;

            if (!isMapBig)
            {
                await Task.Run(() =>
                {
                    for (double iLatitude = minBound; iLatitude <= maxBound; iLatitude = iLatitude + pas)
                    {
                        for (double iLongitude = minBoundWE; iLongitude <= maxBoundWE; iLongitude = iLongitude + pas)
                        {
                            RunLogicCluster(iLatitude, iLongitude, _zoom, _col, pas, center);
                        }
                    }
                });
            }
            else
            {
                await Task.Run(() =>
                {
                    for (double iLatitude = minBound; iLatitude <= maxBound; iLatitude = iLatitude + pas)
                    {
                        for (double iLongitude = -180; iLongitude < _bound.East; iLongitude = iLongitude + pas)
                        {
                            RunLogicCluster(iLatitude, iLongitude, _zoom, _col, pas, center);
                        }



                        for (double iLongitude = _bound.West; iLongitude <= 180; iLongitude = iLongitude + pas)
                        {
                            RunLogicCluster(iLatitude, iLongitude, _zoom, _col, pas, center);
                        }
                    }
                });
            }


            //OnGenerateClusterPushPin(ClusterPin);
            //OnGeneratePushPin(Pin);
        }

        private async Task RunLogicCluster(double iLatitude, double iLongitude, int _zoom, IEnumerable<ItemObjet> _col, double pas,bool centerCluster)
        {

            //first run
            var queryfirst = (from item in _col
                              where (item.Location.Latitude >= iLatitude && item.Location.Latitude < iLatitude + pas) &&
                              (item.Location.Longitude >= iLongitude && item.Location.Longitude < iLongitude + pas)
                              select item).ToList();
            //calcul de la valeur moyenne
            if (queryfirst.Count > 1 && _zoom < 20)
            {
                double latitude=0;
                double longitude=0;
                if (centerCluster)
                {
                    latitude = (iLatitude + pas / 2) > 90 ? iLatitude : (iLatitude + pas / 2);
                    longitude = (iLongitude + pas / 2) > 180 ? iLongitude : (iLongitude + pas / 2);
                }
                else
                {
                    foreach (var item in queryfirst)
                    {
                        latitude += item.Location.Latitude;
                        longitude += item.Location.Longitude;
                    }
                    latitude = latitude/queryfirst.Count;
                    longitude = longitude/queryfirst.Count;
                }


                var itemObjet = new ItemObjet()
                {
                    item = queryfirst.Count,
                    Location = new Location() { Latitude = latitude, Longitude = longitude }
                };
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CurrentShownItem.Add(itemObjet);
                });

            }
            if (queryfirst.Count == 1)
            {
                double latitude = 0;
                double longitude = 0;
                foreach (var item in queryfirst)
                {
                    latitude += item.Location.Latitude;
                    longitude += item.Location.Longitude;
                }
                var itemObjet = new ItemObjet()
                {
                    item = queryfirst.First(),
                    Location = new Location() { Latitude = latitude, Longitude = longitude }
                };
                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CurrentShownItem.Add(itemObjet);
                });
            }
            if (queryfirst.Count >= 1 && _zoom == 20)
            {
                double latitude = 0;
                double longitude = 0;
                foreach (var item in queryfirst)
                {
                    latitude = item.Location.Latitude;
                    longitude = item.Location.Longitude;
                    var itemObjet = new ItemObjet()
                    {
                        item = queryfirst.First(),
                        Location = new Location() { Latitude = latitude, Longitude = longitude }
                    };
                    Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {

                        CurrentShownItem.Add(itemObjet);
                    });
                }
            }
        }

        private async Task RunLogicClusterWithoutZoom(int _zoom, ItemCollection _col, double pas)
        {
            List<ItemObjet> itemToDelete = new List<ItemObjet>();
            // on veut les items qui sont seulement present dans la nouvelle zone
            //pas les elements present dans l'intersection
            var _bound = Boundaries;
            var queryItem = (from item in _col
                             where item.Location.IsPointInside(_bound) && !item.Location.IsPointInside(_oldBound)
                             select item).ToList();

            var queryToClear = (from item in CurrentShownItem
                                where !item.Location.IsPointInside(_bound)
                                select item).ToList();

            foreach (var push in queryToClear)
            {
                itemToDelete.Add(push);
            }

            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var item in itemToDelete)
                    {
                        CurrentShownItem.Remove(item);
                    }
                });
            await GenerateWithZoomChange(queryItem);

            //if (OnDeletePushPin(itemToDelete))
            //{
            //    await GenerateWithZoomChange(queryItem);
            //}
        }
    }
}
