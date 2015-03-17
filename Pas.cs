using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyClustering
{
    public class Pas
    {
        private readonly int min;

        public int Min
        {
            get { return min; }
        }

        private readonly int max;

        public int Max
        {
            get { return max; }
        }

        private readonly double value;

        public double Value
        {
            get { return value; }
        }

        public Pas(int min,int max,double value)
        {
            this.min = min;
            this.max = max;
            this.value = value;
        }
    }
}
