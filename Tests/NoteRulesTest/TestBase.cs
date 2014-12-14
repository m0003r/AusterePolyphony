using System;
using System.Linq;
using Compositor.Levels;
using Compositor.Rules.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NoteRulesTest
{
    public class TestBase<T> where T: IDeniable, ITemporal
    {
        protected static void Satisfy(Predicate<T> expected,
            Predicate<double> expectedFreq,
            FreqsDict freqs)
        {
            foreach (var kv in freqs)
            {
                var actual = (T)kv.Key;
                if (!expected(actual)) continue;

                var freq = kv.Value;
                Console.WriteLine("{0} at {1} => {2}", actual, actual.TimeStart, freq);
                if (actual.AppliedRules != null)
                    foreach (var p in actual.AppliedRules)
                        Console.WriteLine("\t{0}: {1}", p.Item1.GetType().Name, p.Item2);

                Assert.IsTrue(expectedFreq(freq));
            }
        }

        protected static void IsAllowed(T expectedNote,
            FreqsDict freqs)
        {
            Console.WriteLine("Is allowed {0} at {1}?", expectedNote, expectedNote.TimeStart);
            IsAllowed(n => n.Equals(expectedNote), freqs);
        }

        protected static void IsAllowed(Predicate<T> expected,
            FreqsDict freqs)
        {
            var count = freqs.Count(kv => expected((T)kv.Key));
            Assert.IsTrue(count > 0);
            
            Satisfy(expected, x => x >= 0.03, freqs);
        }

        protected static void IsOneAllowed(Predicate<T> expected, FreqsDict freqs)
        {
            IsAllowed(expected, freqs);
            IsDenied(n => !expected(n), freqs);
        }

        protected static void IsOneAllowed(T expected, FreqsDict freqs)
        {
            IsOneAllowed(n => n.Equals(expected), freqs);
        }

        protected static void IsDenied(Predicate<T> expected,
            FreqsDict freqs)
        {
            Satisfy(expected, x => x < 0.03, freqs);
        }
    }
}