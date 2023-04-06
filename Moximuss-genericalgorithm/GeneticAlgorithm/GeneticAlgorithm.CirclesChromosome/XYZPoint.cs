using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.CirclesChromosome
{
    public class XYZPoint
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float ActualX { get; set; }
        public float ActualY { get; set; }
        public bool IsRelative2Circles { get; set; }
    }
}
