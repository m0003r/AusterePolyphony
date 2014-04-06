using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;

namespace Compositor.ChooseNextStrategy
{
    class DefaultNextStrategy : IChooseNextStrategy
    {
        readonly Random _rand;

        public DefaultNextStrategy(int seed = 0)
        {
            _rand = new Random(seed);
        }

        private double GetNextDouble(double freqSum)
        {
            double result = _rand.NextDouble();
                        
            return result * freqSum;
        }

        public IDeniable ChooseNext(IEnumerable<KeyValuePair<IDeniable, double>> freqs)
        {
            var keyValuePairs = freqs as IList<KeyValuePair<IDeniable, double>> ?? freqs.ToList();
            double freqSum = keyValuePairs.Sum(kv => kv.Value);
            double r = GetNextDouble(freqSum);

            double accumulator = 0;

            foreach (var kv in keyValuePairs)
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
        readonly Random _rand;
        
        public QuadraticNextStrategy(int seed = 0)
        {
            _rand = new Random(seed);
        }

        public IDeniable ChooseNext(IEnumerable<KeyValuePair<IDeniable, double>> freqs)
        {
            var keyValuePairs = freqs as IList<KeyValuePair<IDeniable, double>> ?? freqs.ToList();
            double freqS = keyValuePairs.Sum(kv => Math.Pow(kv.Value, 3));
            double r = _rand.NextDouble() * freqS;
            double accumulator = 0;

            foreach (var kv in keyValuePairs)
            {
                accumulator += Math.Pow(kv.Value, 3);
                if (accumulator > r)
                    return kv.Key;
            }

            throw new Exception("It couldn't happen!");
        }
    }
}
