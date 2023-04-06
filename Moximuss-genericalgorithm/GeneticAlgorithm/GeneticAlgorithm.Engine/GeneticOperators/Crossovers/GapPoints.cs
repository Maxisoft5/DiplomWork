using GeneticAlgorithm.Interfaces;
using GeneticAlgorithm.Extensions;
using System;

namespace GeneticAlgorithm.Crossovers
{
    public class GapPoints<TChromosome> : ICrossover<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        private static readonly Random _random = new Random();

        public int GapPointsCount { get; set; }

        public TChromosome Crossover(TChromosome left, TChromosome right)
        {
            int[] rnd = new int[GapPointsCount];
            int part = left.Genotype.Length / (GapPointsCount + 1);
            for (int k = 0; k < GapPointsCount; k++)
                rnd[k] = part * k + _random.Next() % part;
            int[] child = new int[left.Genotype.Length];
            int i = 0;
            for (int k = 0; k < rnd.Length; k++)
                if (k % 2 != 0)
                    for (; i < rnd[k]; i++)
                        child[i] = left.Genotype[i];
                else
                    for (; i < rnd[k]; i++)
                        child[i] = right.Genotype[i];

            for (; i < left.Genotype.Length; i++)
                child[i] = left.Genotype[i];
            return new TChromosome() { Genotype = child.ToSequence() };
        }
    }
}
