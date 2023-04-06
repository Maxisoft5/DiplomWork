namespace GeneticAlgorithm.Interfaces
{
    public interface ICrossover<TChromosome>
        where TChromosome : class, IChromosome, new()
    {
        TChromosome Crossover(TChromosome left, TChromosome right);
    }
}
