using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyClustering
{
    public static class PointHelper
    {
        public static bool IsPointInside(this Location location, Bounds bound)
        {
            if (location != null)
            {
                if (bound.East < bound.West)
                {
                    //la longitude de la frontière Ouest est supérieure à celle de la frontière Est
                    if ((-180 <= location.Longitude && location.Longitude <= bound.East)
                        || (bound.West <= location.Longitude && location.Longitude <= 180))
                    {
                        if (bound.South <= location.Latitude && location.Latitude <= bound.North)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //la longitude de la frontière Est est supérieure à celle de la frontière Ouest
                    if (bound.West <= location.Longitude && location.Longitude <= bound.East)
                    {
                        if (bound.South <= location.Latitude && location.Latitude <= bound.North)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
