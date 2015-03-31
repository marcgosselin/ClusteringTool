using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyClustering
{
    public class ClusterObjet
    {
        public Bounds Boundaries { get; set; }

        public Location Center { get; set; }

        public int Count { get; set; }
    }
}
