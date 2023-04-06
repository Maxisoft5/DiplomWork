namespace GeneticAlgorithm.Interfaces
{
    public interface IMutator<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        void Mutation(TChromosome chromosome);
    }
}
