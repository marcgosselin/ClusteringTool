using EasyClustering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Data;

namespace CLustering.Test.Converters
{
    public class LocationToGeoPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Location loca = value as Location;
            Geopoint geo = new Geopoint(new BasicGeoposition() { Latitude = loca.Latitude, Longitude = loca.Longitude });
            return geo;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
