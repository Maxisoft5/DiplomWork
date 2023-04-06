using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithm.DelaunatorSharp.Interfaces;

namespace GeneticAlgorithm.DelaunatorSharp.Models
{
    public struct Triangle : ITriangle
    {
        public int Index { get; set; }

        public IEnumerable<IPoint> Points { get; set; }

        public Triangle(int t, IEnumerable<IPoint> points)
        {
            Points = points;
            Index = t;
        }
    }
}
