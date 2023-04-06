using GeneticAlgorithm.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public sealed class GeneticEngine<TChromosome> : IEnumerator<TChromosome>, IEnumerable<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        private Task _calculatingTask = null;
        private CancellationTokenSource _tokenSource;
        private bool _isRunning = false;
        private readonly Stopwatch _stopwatch;
        private readonly Random _random;
        private readonly object _locker = new object();
        private readonly List<TChromosome> _population;
        private readonly ISelector<TChromosome> _selector;
        private readonly IParentsSelector<TChromosome> _parentsSelector;
        private readonly ICrossover<TChromosome> _crossover;
        private readonly IMutator<TChromosome> _mutator;
        private readonly short _populationSize;
        private readonly short _remainIndividualsCount;
        private readonly long _breakCriterionValue;
        private readonly byte _mutationPercent;

        public long Iteration { get; private set; }
        public long IterationOnWhichBestFound { get; private set; }
        public long TotalIndividualsGenerated { get; private set; }
        public TimeSpan ElapsedTime { get { return _stopwatch.Elapsed; } }
        public TChromosome Current { get; private set; }
        object IEnumerator.Current => Current;

        public GeneticEngineBuilder<TChromosome> GeneticEngineBuilder
        {
            get => default;
            set
            {
            }
        }

        public GeneticEngine(GeneticEngineBuilder<TChromosome> builder)
        {
            _stopwatch = new Stopwatch();
            _population = new List<TChromosome>();
            _random = new Random();
            _selector = builder.Selector;
            _parentsSelector = builder.ParentsSelector;
            _crossover = builder.Crossover;
            _mutator = builder.Mutator;
            _populationSize = builder.PopulationSize;
            _remainIndividualsCount = builder.RemainIndividualsCount;
            _breakCriterionValue = builder.BreakCriterionValue;
            _mutationPercent = builder.MutationPercent;
        }

        public TChromosome ToSolve()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Algorithm is running. Wait for the engine to complete or cancel operation.");
            }
            _tokenSource = new CancellationTokenSource();
            return Solve();
        }

        private TChromosome Solve()
        {
            _isRunning = true;
            _stopwatch.Reset();
            _stopwatch.Start();
            Reset();
            Current = _population[0];

            for (Iteration = 0; Iteration < _breakCriterionValue && (_tokenSource is null || !_tokenSource.IsCancellationRequested); Iteration++)
            {
                Reproduction();
                TotalIndividualsGenerated += _populationSize - _remainIndividualsCount;
                Mutation();
                Selection();
                if (Current.CompareTo(_population[0]) > 0)
                {
                    Current = _population[0];
                    IterationOnWhichBestFound = Iteration;
                }
            }
            _stopwatch.Stop();
            _isRunning = false;
            return Current;
        }

        public Task<TChromosome> ToSolveAsync()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Algorithm is running. Wait for the engine to complete or cancel operation.");
            }
            _tokenSource = new CancellationTokenSource();

            return Task.Run(() =>
            {
                return Solve();
            }, _tokenSource.Token);
        }

        public void Cancel()
        {
            _tokenSource.Cancel();
            _isRunning = false;
        }

        public void Reset()
        {
            _population.Clear();
            TotalIndividualsGenerated = 0;
            Iteration = -1;
            for (int i = 0; i < _remainIndividualsCount; i++)
            {
                _population.Add(new TChromosome());
            }
            TotalIndividualsGenerated = _remainIndividualsCount;
        }

        public bool MoveNext()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Enumerator is locked. Wait for the engine to complete or cancel operation.");
            }
            lock (_locker)
            {
                if (Iteration == _breakCriterionValue)
                {
                    Reset();
                    return false;
                }
                if (_calculatingTask?.Status == TaskStatus.Running)
                    _calculatingTask.Wait();
                if (Current != _population[0])
                {
                    Current = _population[0];
                    IterationOnWhichBestFound = Iteration;
                }
                _calculatingTask = Task.Run(() => Next());
                Iteration++;
                return true;
            }
        }
        public void Dispose()
        {
            if (!(_calculatingTask is null))
            {
                _calculatingTask.Dispose();
                _calculatingTask = null;
            }
            if (!(_tokenSource is null))
            {
                _tokenSource.Dispose();
                _tokenSource = null;
            }
            return;
        }
        public IEnumerator<TChromosome> GetEnumerator()
        {
            return this;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        private void Next()
        {
            Reproduction();
            TotalIndividualsGenerated += _populationSize - _remainIndividualsCount;
            Mutation();
            Selection();
        }

        private void Reproduction()
        {
            for (int i = _population.Count; i < _populationSize; i++)
            {
                _parentsSelector.SelectParents(_population, out TChromosome left, out TChromosome right);
                if (left is null || right is null)
                {
                    throw new NullReferenceException("One of SelectParents out parameter is null.");
                }
                TChromosome newChromosome = _crossover.Crossover(left, right);
                if (newChromosome is null)
                {
                    throw new NullReferenceException("Crossover returns null.");
                }
                _population.Add(newChromosome);
            }
        }

        private void Selection()
        {
            _selector.Selection(_population, _remainIndividualsCount);
        }

        private void Mutation()
        {
            if (_mutationPercent <= 0)
                return;

            for (int k = _remainIndividualsCount; k < _population.Count; k++)
                if (_random.Next(0, 100) < _mutationPercent)
                {
                    _mutator.Mutation(_population[k]);
                    if (_population[k] is null)
                    {
                        throw new NullReferenceException("Chromosome after mutation is null.");
                    }
                }
        }
    }
}
