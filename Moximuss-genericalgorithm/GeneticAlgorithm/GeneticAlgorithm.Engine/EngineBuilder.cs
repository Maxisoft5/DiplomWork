using GeneticAlgorithm.Interfaces;
using System;
using System.Text;

namespace GeneticAlgorithm
{
    public sealed class GeneticEngineBuilder<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        public ISelector<TChromosome> Selector { get; private set; }
        public IParentsSelector<TChromosome> ParentsSelector { get; private set; }
        public ICrossover<TChromosome> Crossover { get; private set; }
        public IMutator<TChromosome> Mutator { get; private set; }
        public short PopulationSize { get; private set; }
        public short RemainIndividualsCount { get; private set; }
        public long BreakCriterionValue { get; private set; }
        public byte MutationPercent { get; private set; }

        public GeneticEngineBuilder<TChromosome> UseSelector(ISelector<TChromosome> selector)
        {
            if (selector is null)
            {
                throw new ArgumentNullException("Selector is null.");
            }
            Selector = selector;
            return this;
        }
        public GeneticEngineBuilder<TChromosome> UseParentsSelector(IParentsSelector<TChromosome> parentsSelector)
        {
            if (parentsSelector is null)
            {
                throw new ArgumentNullException("Parents selector is null.");
            }
            ParentsSelector = parentsSelector;
            return this;
        }
        public GeneticEngineBuilder<TChromosome> UseCrossover(ICrossover<TChromosome> crossover)
        {
            if (crossover is null)
            {
                throw new ArgumentNullException("Crossover is null.");
            }
            Crossover = crossover;
            return this;
        }
        public GeneticEngineBuilder<TChromosome> UseMutator(IMutator<TChromosome> mutator)
        {
            if (mutator is null)
            {
                throw new ArgumentNullException("Mutator is null.");
            }
            Mutator = mutator;
            return this;
        }
        public GeneticEngineBuilder<TChromosome> UsePopulationSize(short count)
        {
            if (count < 2)
            {
                throw new ArgumentException("Too small value for parameter 'Population size'.");
            }
            PopulationSize = count;
            return this;
        }
        public GeneticEngineBuilder<TChromosome> UseRemainIndividualsCount(short count)
        {
            if (count < 1)
            {
                throw new ArgumentException("Too small value for parameter 'Remain individuals count'.");
            }
            RemainIndividualsCount = count;
            return this;
        }
        public GeneticEngineBuilder<TChromosome> UseBreakCriterionValue(long value)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Invalid value for parameter 'Break criterion value'.");
            }
            BreakCriterionValue = value;
            return this;
        }

        public GeneticEngineBuilder<TChromosome> UseMutationPercent(byte percent)
        {
            if (percent < 0 || percent > 100)
            {
                throw new ArgumentException("Percent must be in range between 0 and 100.");
            }
            MutationPercent = percent;
            return this;
        }

        private bool IsValidBuilder(out string reason)
        {
            bool result = true;
            StringBuilder reasonBuilder = new StringBuilder();

            if (Selector is null || ParentsSelector is null || Crossover is null || Mutator is null)
            {
                reasonBuilder.Append(" One of the genetic operators not set to an instance.");
                result = false;
            }
            if (PopulationSize < 2)
            {
                reasonBuilder.Append(" Wrong Count of individuals value.");
                result = false;
            }
            if (RemainIndividualsCount < 1)
            {
                reasonBuilder.Append(" Wrong remain individuals count value.");
                result = false;
            }
            if (BreakCriterionValue <= 0)
            {
                reasonBuilder.Append(" Wrong break criterion value.");
                result = false;
            }
            reason = reasonBuilder.ToString();
            return result;
        }

        public static implicit operator GeneticEngine<TChromosome>(GeneticEngineBuilder<TChromosome> builder)
        {
            if (!builder.IsValidBuilder(out string reason))
            {
                throw new InvalidOperationException(reason);
            }
            return new GeneticEngine<TChromosome>(builder);
        }
    }
}
