using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compositor
{
    public static class Timer
    {
        private static Dictionary<string, double> accumulator = new Dictionary<string,double>();
        private static Dictionary<string, DateTime> current = new Dictionary<string,DateTime>();

        public static void Start(string Name)
        {
            if (current.ContainsKey(Name))
                throw new Exception("Timer already started");

            current[Name] = DateTime.Now;
        }

        public static double Stop(string Name)
        {
            if (!current.ContainsKey(Name))
                throw new Exception("Timer wasn't started");

            
            double seconds = (DateTime.Now - current[Name]).TotalSeconds;
            current.Remove(Name);

            if (accumulator.ContainsKey(Name))
                accumulator[Name] += seconds;
            else
                accumulator[Name] = seconds;

            return seconds;
        }

        public static double Total(string Name)
        {
            if (!accumulator.ContainsKey(Name))
                throw new Exception("Timer's name not found");

            return accumulator[Name];
        }

        public static bool Contains(string Name)
        {
            return accumulator.ContainsKey(Name);
        }

        public static void Flush(string Name)
        {
            if (accumulator.ContainsKey(Name))
                accumulator[Name] = 0;
        }
    }
}
