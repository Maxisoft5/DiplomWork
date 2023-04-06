using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.DelaunatorSharp.Interfaces
{
    public interface IEdge
    {
        IPoint P { get; }
        IPoint Q { get; }
        int Index { get; }
    }
}
