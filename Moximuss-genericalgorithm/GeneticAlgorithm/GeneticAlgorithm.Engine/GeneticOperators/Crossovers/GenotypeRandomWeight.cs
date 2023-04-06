using GeneticAlgorithm.Interfaces;
using GeneticAlgorithm.Extensions;
using System;

namespace GeneticAlgorithm.Crossovers
{
    public class GenotypeRandomWeight<TChromosome> : ICrossover<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        private static readonly Random _random = new Random();
        public TChromosome Crossover(TChromosome left, TChromosome right)
        {
            if (left.Genotype.Length != right.Genotype.Length)
            {
                throw new ArgumentException("Length of genotypes must be same.");
            }
            int[] child = new int[left.Genotype.Length];
            double p = _random.Next(0, 100);
            for (int i = 0; i < left.Genotype.Length; i++)
            {
                child[i] = (int)(left.Genotype[i] * right.Fitness() * p + right.Genotype[i] * left.Fitness() * (1 - p));
                //child[i] = (byte)(left.Genotype[i] / left.Fitness() * p + (right.Genotype[i] / right.Fitness()) * (1 - p)); 
            }
            return new TChromosome() { Genotype = child.ToSequence() };
        }
    }
}
