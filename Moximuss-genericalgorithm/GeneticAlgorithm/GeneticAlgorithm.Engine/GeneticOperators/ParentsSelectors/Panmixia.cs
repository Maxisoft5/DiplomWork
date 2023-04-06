using GeneticAlgorithm.Interfaces;
using System;
using System.Collections.Generic;

namespace GeneticAlgorithm.ParentsSelectors
{
    public class Panmixia<TChromosome> : IParentsSelector<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        protected static Random _random = new Random();

        public void SelectParents(List<TChromosome> chromosomes, out TChromosome left, out TChromosome right)
        {
            left = chromosomes[_random.Next(0, chromosomes.Count)];
            do
            {
                right = chromosomes[_random.Next(0, chromosomes.Count)];
            } while (Equals(left, right));
        }
    }
}
