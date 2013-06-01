using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compositor.Levels;

namespace AssertExtension
{
    public static class MyAssertExtensions
    {
        public static void Satisfy(this Assert ast,
            Predicate<Note> expectedNote, 
            Predicate<double> expectedFreq,
            Dictionary<Note, double> freqs)
        {
            foreach (var freq in
                        from kv in freqs
                        where expectedNote(kv.Key)
                        select kv.Value)
            {
                Assert.IsTrue(expectedFreq(freq));
            }
        }

        public static void AreAllowed(this Assert ast,
            Predicate<Note> expectedNote,
            Dictionary<Note, double> freqs)
        {
            Satisfy(ast, expectedNote, x => x > 0.03, freqs);
        }

        public static void AreDenied(this Assert ast,
            Predicate<Note> expectedNote,
            Dictionary<Note, double> freqs)
        {
            Satisfy(ast, expectedNote, x => x < 0.03, freqs);
        }
    }
}
