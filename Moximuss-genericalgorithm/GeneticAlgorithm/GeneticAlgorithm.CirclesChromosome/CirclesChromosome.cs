using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithm.CirclesChromosome.Services;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.CirclesChromosome
{
    public class CirclesChromosome : IChromosome
    {
        private delegate CirclesChromosome CrossoverDelegate(CirclesChromosome left, CirclesChromosome right);

        public static int CountGapPoints = 4;
        private static CrossoverDelegate _crossover;
        public static List<Circle> Resource;
        private static Random _rand = new Random();
        public static float TopBorder { get; set; } = 100;
        public static float LeftBorder { get; set; } = 0;
        public static float BottomBorder { get; set; } = 0;
        static CirclesChromosome()
        {
            _crossover = new CrossoverDelegate(GapPointsCrossover);
        }

        private Task _fitnessCalculating;
        public int[] Sequence { get; private set; }
        public int[] Genotype { get; set; }

        private double _fitness = -1;

        public CirclesChromosome()
        {
            if (Resource == null || Resource.Count == 0)
            {
                throw new Exception("For first - set resource property!");
            }

            Sequence = new int[Resource.Count];

            for (int i = 1; i < Sequence.Length;)
            {
                int rnd = _rand.Next() % (Sequence.Length - 1) + 1;
                if (Sequence.Contains(rnd))
                    continue;
                Sequence[i++] = rnd;
            }
            CalculateFitness();
        }
        public CirclesChromosome(int[] sequence)
        {
            if (sequence.Length != Resource.Count)
                throw new ArgumentException("Length of route must be equals count of points in graph!");
            Sequence = sequence;
            CalculateFitness();
        }
        private CirclesChromosome Normalize()
        {
            bool[] flags = new bool[Sequence.Length];

            for (int l = 0; l < Sequence.Length; l++)
            {
                if (flags[Sequence[l]])
                {
                    int temp = Sequence[l];
                    for (int k = 1; temp - k >= 0 || temp + k < Sequence.Length; k++)
                    {
                        if (temp - k >= 0 && !flags[temp - k])
                        {
                            flags[temp - k] = true;
                            Sequence[l] = temp - k;
                            break;
                        }
                        if (temp + k < Sequence.Length && !flags[temp + k])
                        {
                            flags[temp + k] = true;
                            Sequence[l] = temp + k;
                            break;
                        }
                    }
                    continue;
                }
                flags[Sequence[l]] = true;
            }
            return this;
        }

        public int CompareTo(object obj)
        {
            CirclesChromosome g = obj as CirclesChromosome;
            if (_fitness < g._fitness)
                return -1;
            if (_fitness == g._fitness)
                return 0;
            return 1;
        }
        public void Mutation()
        {
            int i = _rand.Next(1, Sequence.Length);
            int j = _rand.Next(1, Sequence.Length);

            int tmp = Sequence[i];
            Sequence[i] = Sequence[j];
            Sequence[j] = tmp;
            CalculateFitness();
        }

        public double Fitness()
        {
            if (_fitnessCalculating.Status == TaskStatus.Running)
                _fitnessCalculating.Wait();
            return _fitness;
        }
        public IChromosome Crossover(IChromosome other)
        {
            if (!(other is CirclesChromosome))
            {
                throw new Exception();
            }
            var othr = other as CirclesChromosome;
            return _crossover.Invoke(this, othr);
        }
        private static CirclesChromosome GapPointsCrossover(CirclesChromosome current, CirclesChromosome other)
        {
            if (other.Sequence.Length != current.Sequence.Length)
                throw new ArgumentException("Нельзя");

            int[] rnd = new int[CountGapPoints];

            int part = current.Sequence.Length / (CountGapPoints + 1);

            for (int k = 0; k < CountGapPoints; k++)
                rnd[k] = part * k + _rand.Next() % part;

            int[] tmp = new int[current.Sequence.Length];
            int i = 0;

            for (int k = 0; k < rnd.Length; k++)
                for (; i < rnd[k]; i++)
                    if (k % 2 == 0)
                        tmp[i] = current.Sequence[i];
                    else
                        tmp[i] = other.Sequence[i];
            for (; i < current.Sequence.Length; i++)
                tmp[i] = other.Sequence[i];

            return new CirclesChromosome(tmp).Normalize();
        }

        private void CalculateFitness()
        {
            _fitnessCalculating = Task.Run(async () =>
            {
                _fitness = await CirclePlacingService.PlaceCircles(Resource, Sequence, TopBorder, LeftBorder, BottomBorder);
            });
        }
    }
}
