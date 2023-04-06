using GeneticAlgorithm.Interfaces;
using GeneticAlgorithm.RectanglesGenotype.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm.RectanglesGenotype
{
    public class RectanglesChromosome : IChromosome
    {
        private static Random _rand = new Random();

        public static float TopBorder = 0;
        public static float BottomBorder = 0;
        public static float LeftBorder = 0;
        public static List<Rectangle> Source { get; set; }

        private List<XYPoint> _positions;
        public List<XYPoint> Positions { get { return _positions; } }

        private double _fitness = -1;
        public int[] Genotype { get; set; }

        public RectanglesChromosome()
        {
            if (Source is null || Source.Count == 0)
            {
                throw new ArgumentNullException("For first set source property!");
            }

            Genotype = new int[Source.Count];

            int indexOfZero = _rand.Next(0, Genotype.Length);

            for (int i = 0; i < Genotype.Length;)
            {
                if (i == indexOfZero)
                {
                    i++;
                    continue;
                }
                int rnd = _rand.Next() % (Genotype.Length - 1) + 1;
                if (Genotype.Contains(rnd))
                    continue;
                Genotype[i++] = rnd;
            }
        }

        public double Fitness()
        {
            if (_fitness == -1)
            {
                _fitness = RectanglePlacingService.PlaceRectangles(Source, Genotype, out _positions, TopBorder, LeftBorder, BottomBorder);
            }
          
            return _fitness;
        }

        public int CompareTo(object other)
        {
            RectanglesChromosome othr = other as RectanglesChromosome;
            if (othr is null)
            {
                throw new InvalidCastException($"RectanglesChromosome.CompareTo: other chromosome is not instance of {nameof(RectanglesChromosome)}");
            }

            if (Fitness() < othr.Fitness())
                return -1;
            if (_fitness == othr._fitness)
                return 0;
            return 1;
        }

        public void ResetFitness()
        {
            _fitness = -1;
        }
    }
}
