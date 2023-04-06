using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.CirclesChromosome
{
    public sealed class Circle : XYZPoint
    {
        public float Radius { get; set ; }
        public object Mark { get; set; }

    }
}
