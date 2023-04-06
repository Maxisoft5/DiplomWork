using GeneticAlgorithm.Interfaces;
using GeneticAlgorithm.Extensions;
using System;

namespace GeneticAlgorithm.Crossovers
{
    public class GenotypeSum<TChromosome> : ICrossover<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        public IChromosome IChromosome
        {
            get => default;
            set
            {
            }
        }

        public TChromosome Crossover(TChromosome left, TChromosome right)
        {
            if (left.Genotype.Length != right.Genotype.Length)
                throw new ArgumentException("Length of genotypes must be same.");
            int[] child = new int[left.Genotype.Length];
            for (int i = 0; i < left.Genotype.Length; i++)
                child[i] = left.Genotype[i] + right.Genotype[i];
            return new TChromosome() { Genotype = child.ToSequence() };
        }
    }
}
