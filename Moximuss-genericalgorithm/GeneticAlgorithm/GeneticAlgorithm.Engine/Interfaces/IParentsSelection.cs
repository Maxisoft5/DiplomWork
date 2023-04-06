using System.Collections.Generic;

namespace GeneticAlgorithm.Interfaces
{
    public interface IParentsSelector<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        void SelectParents(List<TChromosome> chromosomes, out TChromosome left, out TChromosome right);
    }
}
