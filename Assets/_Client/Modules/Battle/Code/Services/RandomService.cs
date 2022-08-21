using System;

namespace Client.Battle.Simulation
{
    public sealed class RandomService
    {
        public Random Random;

        public RandomService(Random random)
        {
            Random = random;
        }
        
        public RandomService(int seed)
        {
            Random = new Random(seed);
        }
    }
}