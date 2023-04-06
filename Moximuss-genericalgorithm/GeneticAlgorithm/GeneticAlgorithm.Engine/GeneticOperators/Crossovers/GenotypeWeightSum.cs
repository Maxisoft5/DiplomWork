using GeneticAlgorithm.Interfaces;
using GeneticAlgorithm.Extensions;
using System;

namespace GeneticAlgorithm.Crossovers
{
    public class GenotypeWeightSum<TChromosome> : ICrossover<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        public TChromosome Crossover(TChromosome left, TChromosome right)
        {
            if (left.Genotype.Length != right.Genotype.Length)
                throw new ArgumentException("Length of genotypes must be same.");
            int[] child = new int[left.Genotype.Length];
            for (int i = 0; i < left.Genotype.Length; i++)
                child[i] = (int)(left.Genotype[i] * 100 / left.Fitness() + right.Genotype[i] * 100 / right.Fitness());
                //child[i] = (byte)(left.Genotype[i] * right.Fitness() + right.Genotype[i] * left.Fitness());
            return new TChromosome() { Genotype = child.ToSequence() };
        }
    }
}
