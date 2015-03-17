using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;

namespace CLustering.Test
{
    public static class Extensions
    {
        public static GeoboundingBox GetBounds(this MapControl map)
        {
            Geopoint topLeft = null;

            try
            {
                map.GetLocationFromOffset(new Windows.Foundation.Point(0, 0), out topLeft);
            }
            catch
            {
                var topOfMap = new Geopoint(new BasicGeoposition()
                {
                    Latitude = 85,
                    Longitude = 0
                });

                Windows.Foundation.Point topPoint;
                map.GetOffsetFromLocation(topOfMap, out topPoint);
                map.GetLocationFromOffset(new Windows.Foundation.Point(0, topPoint.Y), out topLeft);
            }

            Geopoint bottomRight = null;
            try
            {
                map.GetLocationFromOffset(new Windows.Foundation.Point(map.ActualWidth, map.ActualHeight), out bottomRight);
            }
            catch
            {
                var bottomOfMap = new Geopoint(new BasicGeoposition()
                {
                    Latitude = -85,
                    Longitude = 0
                });

                Windows.Foundation.Point bottomPoint;
                map.GetOffsetFromLocation(bottomOfMap, out bottomPoint);
                map.GetLocationFromOffset(new Windows.Foundation.Point(0, bottomPoint.Y), out bottomRight);
            }

            if (topLeft != null && bottomRight != null)
            {
                return new GeoboundingBox(topLeft.Position, bottomRight.Position);
            }

            return null;
        }
    }
}
