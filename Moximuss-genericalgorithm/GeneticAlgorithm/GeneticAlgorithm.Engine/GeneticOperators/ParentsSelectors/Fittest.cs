using GeneticAlgorithm.Interfaces;
using System.Collections.Generic;

namespace GeneticAlgorithm.ParentsSelectors
{
    public class Fittest<TChromosome> : IParentsSelector<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        private static List<TChromosome> _source;
        private static int _leftIndex = 0;
        private static int _rightIndex = 0;
        public void SelectParents(List<TChromosome> chromosomes, out TChromosome left, out TChromosome right)
        {
            if (_source is null || !Equals(chromosomes, _source))
            {
                _source = chromosomes;
                _source.Sort();
                _leftIndex = 0;
                _rightIndex = 0;
            }
            if (_leftIndex >= _source.Count)
                _leftIndex = 0;
            if (_rightIndex >= _source.Count)
                _rightIndex = 0;
            left = _source[_leftIndex];
            right = _source[_rightIndex];
        }
    }
}
