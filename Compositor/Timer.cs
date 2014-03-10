using System;
using System.Collections.Generic;

namespace Compositor
{
    public static class Timer
    {
        private static readonly Dictionary<string, double> Accumulator = new Dictionary<string,double>();
        private static readonly Dictionary<string, DateTime> Current = new Dictionary<string,DateTime>();

        public static void Start(string name)
        {
            if (Current.ContainsKey(name))
                throw new Exception("Timer already started");

            Current[name] = DateTime.Now;
        }

        public static double Stop(string name)
        {
            if (!Current.ContainsKey(name))
                throw new Exception("Timer wasn't started");

            
            double seconds = (DateTime.Now - Current[name]).TotalSeconds;
            Current.Remove(name);

            if (Accumulator.ContainsKey(name))
                Accumulator[name] += seconds;
            else
                Accumulator[name] = seconds;

            return seconds;
        }

        public static double Total(string name)
        {
            if (!Accumulator.ContainsKey(name))
                throw new Exception("Timer's name not found");

            return Accumulator[name];
        }

        public static bool Contains(string name)
        {
            return Accumulator.ContainsKey(name);
        }

        public static void Flush(string name)
        {
            if (Accumulator.ContainsKey(name))
                Accumulator[name] = 0;
        }
    }
}
