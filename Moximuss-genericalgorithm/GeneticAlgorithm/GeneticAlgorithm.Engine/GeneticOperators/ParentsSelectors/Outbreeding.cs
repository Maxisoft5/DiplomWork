using GeneticAlgorithm.Extensions;
using GeneticAlgorithm.Interfaces;
using System;
using System.Collections.Generic;

namespace GeneticAlgorithm.ParentsSelectors
{
    public class Outbreeding<TChromosome> : IParentsSelector<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        protected static Random _random = new Random();
        private static List<TChromosome> _source;
        private static int[][] _coefficients;

        public IParentsSelector<TChromosome> IParentsSelector
        {
            get => default;
            set
            {
            }
        }

        public void SelectParents(List<TChromosome> chromosomes, out TChromosome left, out TChromosome right)
        {
            if (_source is null || !Equals(_source, chromosomes))
            {
                _source = chromosomes;
                Preprocessing();
            }
            int leftIndex = _random.Next(0, _coefficients.GetLength(0));
            left = chromosomes[leftIndex];
            int rightIndex = _coefficients[leftIndex].IndexOfMin();
            right = chromosomes[rightIndex];
        }
        private void Preprocessing()
        {
            _coefficients = new int[_source.Count][];
            for (int i = 0; i < _source.Count; i++)
                _coefficients[i] = new int[_source.Count];
            for (int i = 0; i < _source.Count; i++)
                for (int j = 0; j < _source.Count; j++)
                {
                    if (i == j)
                    {
                        _coefficients[i][j] = _source[i].Genotype.Length;
                    }
                    _coefficients[i][j] = DifferenceCoefficient(_source[i], _source[j]);
                    _coefficients[j][i] = _coefficients[i][j];
                }
        }
        private int DifferenceCoefficient(TChromosome first, TChromosome second)
        {
            int coefficient = 0;
            for (int i = 0; i < first.Genotype.Length && i < second.Genotype.Length; i++)
                if (first.Genotype[i] == second.Genotype[i])
                    coefficient++;
            return coefficient;
        }
    }
}
