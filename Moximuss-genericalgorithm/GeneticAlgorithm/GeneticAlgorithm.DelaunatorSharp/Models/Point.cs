using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithm.DelaunatorSharp.Interfaces;

namespace GeneticAlgorithm.DelaunatorSharp.Models
{
    public struct Point : IPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
        public override string ToString() => $"{X},{Y}";
    }
}
