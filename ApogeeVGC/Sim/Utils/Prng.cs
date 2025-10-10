namespace ApogeeVGC.Sim.Utils;

public record PrngSeed(int Seed);

public class Prng
{
    public PrngSeed Seed { get; }
    private Random RandomGenerator { get; }

    public Prng(PrngSeed? seed)
    {
        Seed = seed ?? new PrngSeed(Environment.TickCount);
        RandomGenerator = new Random(Seed.Seed);
    }


    public int Random(int? from = null, int? to = null)
    {

    }
}