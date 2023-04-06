using GeneticAlgorithm.Interfaces;
using System;

namespace GeneticAlgorithm.Mutators
{
    public class Permutation<TChromosome> : IMutator<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        protected static readonly Random _random = new Random();
        public void Mutation(TChromosome chromosome)
        {
            int firstGene = _random.Next(0, chromosome.Genotype.Length);
            int secondGene = 0;
            do
                secondGene = _random.Next(0, chromosome.Genotype.Length);
            while (firstGene == secondGene);
            int tmpGene = chromosome.Genotype[firstGene];
            chromosome.Genotype[firstGene] = chromosome.Genotype[secondGene];
            chromosome.Genotype[secondGene] = tmpGene;
        }
    }
}
