using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Compositor.Levels;

namespace Compositor
{
    class DefaultNextStrategy : IChooseNextStrategy
    {
        Random rand;
        int Seed;

        public DefaultNextStrategy(int seed = 0)
        {
            if (seed == 0)
                this.Seed = (int)DateTime.Now.Ticks;
            else
                this.Seed = seed;

            rand = new Random(this.Seed);
        }

        public Note ChooseNext(IEnumerable<KeyValuePair<Note, double>> allowed)
        {
            double freqS = allowed.Sum(kv => kv.Value);
            double r = rand.NextDouble() * freqS;
            double accumulator = 0;

            foreach (KeyValuePair<Note, double> kv in allowed)
            {
                accumulator += kv.Value;
                if (accumulator > r)
                    return kv.Key;
            }

            throw new Exception("It couldn't happen!");
        }
    }
}
