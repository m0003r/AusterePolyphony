using System;
using System.Collections.Generic;
using System.Linq;

namespace Compositor.ChooseNextStrategy
{
    class DefaultNextStrategy<T> : IChooseNextStrategy<T>
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

        public T ChooseNext(IEnumerable<KeyValuePair<T, double>> freqs)
        {
            double freqSum = freqs.Sum(kv => kv.Value);
            double r = GetNextDouble(freqSum);

            double accumulator = 0;

            foreach (KeyValuePair<T, double> kv in freqs)
            {
                accumulator += kv.Value;
                if (accumulator >= r)
                    return kv.Key;
            }

            throw new Exception("It couldn't happen!");
        }
    }

    public class QuadraticNextStrategy<T> : IChooseNextStrategy<T>
    {
        readonly Random _rand;
        
        public QuadraticNextStrategy(int seed = 0)
        {
            _rand = new Random(seed);
        }

        public T ChooseNext(IEnumerable<KeyValuePair<T, double>> freqs)
        {
            double freqS = freqs.Sum(kv => Math.Pow(kv.Value, 3));
            double r = _rand.NextDouble() * freqS;
            double accumulator = 0;

            foreach (KeyValuePair<T, double> kv in freqs)
            {
                accumulator += Math.Pow(kv.Value, 3);
                if (accumulator > r)
                    return kv.Key;
            }

            throw new Exception("It couldn't happen!");
        }
    }
}
