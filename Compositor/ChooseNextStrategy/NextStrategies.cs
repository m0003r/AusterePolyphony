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

        public DefaultNextStrategy(int seed = 0)
        {
            rand = new Random(seed);
        }

        private double getNextDouble(double freqSum)
        {
            double result = rand.NextDouble();
                        
            return result * freqSum;
        }

        public Note ChooseNext(IEnumerable<KeyValuePair<Note, double>> allowed)
        {
            double freqSum = allowed.Sum(kv => kv.Value);
            double r = getNextDouble(freqSum);
            double accumulator = 0;

            foreach (KeyValuePair<Note, double> kv in allowed)
            {
                accumulator += kv.Value;
                if (accumulator >= r)
                    return kv.Key;
            }

            throw new Exception("It couldn't happen!");
        }
    }

    public class QuadraticNextStrategy : IChooseNextStrategy
    {
        Random rand;
        
        public QuadraticNextStrategy(int seed = 0)
        {
            rand = new Random(seed);
        }

        public Note ChooseNext(IEnumerable<KeyValuePair<Note, double>> allowed)
        {
            double freqS = allowed.Sum(kv => Math.Pow(kv.Value, 3));
            double r = rand.NextDouble() * freqS;
            double accumulator = 0;

            foreach (KeyValuePair<Note, double> kv in allowed)
            {
                accumulator += Math.Pow(kv.Value, 3);
                if (accumulator > r)
                    return kv.Key;
            }

            throw new Exception("It couldn't happen!");
        }
    }
}
