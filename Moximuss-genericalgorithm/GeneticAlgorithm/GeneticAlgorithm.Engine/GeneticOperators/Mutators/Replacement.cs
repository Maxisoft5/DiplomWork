using GeneticAlgorithm.Extensions;
using GeneticAlgorithm.Interfaces;
using System;

namespace GeneticAlgorithm.Mutators
{
    public class Replacement<TChromosome> : IMutator<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        protected static readonly Random _random = new Random();
        public void Mutation(TChromosome chromosome)
        {
            int geneIndex = _random.Next(0, chromosome.Genotype.Length - 1);
            chromosome.Genotype[geneIndex] = (byte)_random.Next();
            chromosome.Genotype = chromosome.Genotype.ToSequence();
        }
    }
}
