using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.DelaunatorSharp.Interfaces
{
    public interface ITriangle
    {
        IEnumerable<IPoint> Points { get; }
        int Index { get; }
    }
}
