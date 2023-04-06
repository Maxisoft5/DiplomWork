using GeneticAlgorithm.Interfaces;
using System.Collections.Generic;

namespace GeneticAlgorithm.Selectors
{
    public class Fittest<TChromosome> : ISelector<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        public ISelector<TChromosome> ISelector
        {
            get => default;
            set
            {
            }
        }

        public void Selection(List<TChromosome> chromosomes, int count)
        {
            Sort(chromosomes, count);
            chromosomes.RemoveRange(count - 1, chromosomes.Count - count);
        }
        private void Sort(List<TChromosome> chromosomes, int count)
        {
            for (int i = 0; i < count; i++)
                for (int j = i + 1; j < chromosomes.Count; j++)
                    if (chromosomes[j].CompareTo(chromosomes[i]) == -1)
                    {
                        var tmp = chromosomes[i];
                        chromosomes[i] = chromosomes[j];
                        chromosomes[j] = tmp;
                    }
        }
    }
}
