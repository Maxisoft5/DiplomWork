using System.Collections.Generic;

namespace GeneticAlgorithm.Interfaces
{
    public interface ISelector<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        void Selection(List<TChromosome> chromosomes, int count);
    }
}
