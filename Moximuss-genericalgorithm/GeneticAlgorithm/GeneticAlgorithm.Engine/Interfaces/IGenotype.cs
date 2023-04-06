using System;
namespace GeneticAlgorithm.Interfaces
{
    public interface IChromosome : IComparable
    {
        int[] Genotype { get; set; }
        double Fitness();
    }
}
