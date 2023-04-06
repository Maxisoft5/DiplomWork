using GeneticAlgorithm.Interfaces;
using System;
using System.Collections.Generic;

namespace GeneticAlgorithm.Selectors
{
    public class RouleteWheel<TChromosome> : ISelector<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        private static readonly Random _random = new Random();

        public ISelector<TChromosome> ISelector
        {
            get => default;
            set
            {
            }
        }

        public void Selection(List<TChromosome> chromosomes, int count)
        {
            List<TChromosome> result = new List<TChromosome>();
            var arr = new int[chromosomes.Count];
            arr[0] = (int)chromosomes[0].Fitness();
            for (int i = 1; i < chromosomes.Count; i++)
                arr[i] = arr[i - 1] + (int)chromosomes[i].Fitness();
            for (int i = 0; i < count; i++)
            {
                var rndValue = _random.Next(0, arr[arr.Length - 1]);
                var selectedChromosome = chromosomes[BinSearch(arr, rndValue)];
                result.Add(selectedChromosome);
            }
            chromosomes.Clear();
            chromosomes.AddRange(result);
        }
        private int BinSearch(int[] arr, int value)
        {
            int left = 0;
            int right = arr.Length;
            while (left <= right)
            {
                int tmp = (right + left) / 2;
                if (value <= arr[tmp] && (tmp == 0 || value > arr[tmp - 1]))
                    return tmp;
                if (value < arr[tmp])
                    right = tmp;
                if (value > arr[tmp])
                    left = tmp;
            }
            return -1;
        }
    }
}
