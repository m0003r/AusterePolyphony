using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using Compositor.Rules.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PitchBase;

namespace NoteRulesTest
{
    [TestClass]
    public class RulesUnitTest
    {
        internal void Satisfy(Predicate<Note> expectedNote,
            Predicate<double> expectedFreq,
            FreqsDict freqs)
        {
            foreach (var freq in
                        from kv in freqs
                        where expectedNote((Note)kv.Key)
                        select kv.Value)
            {
                Assert.IsTrue(expectedFreq(freq));
            }
        }

        internal void AreAllowed(Note expectedNote,
            FreqsDict freqs)
        {
            AreAllowed(n => n.Equals(expectedNote), freqs);
        }

        internal void AreAllowed(Predicate<Note> expectedNote,
            FreqsDict freqs)
        {
            int count = freqs.Count(kv => expectedNote((Note)kv.Key));
            Assert.IsTrue(count > 0);
            Satisfy(expectedNote, x => x > 0.03, freqs);
        }

        internal void AreDenied(Predicate<Note> expectedNote,
            FreqsDict freqs)
        {
            Satisfy(expectedNote, x => x < 0.03, freqs);
        }

        internal List<Note> CreateNotes(Modus m, Clef c, bool perfectTime, out List<Pitch> diapason, params int[] infoList)
        {
            var result = new List<Note>();
            var pf = new PitchFactory(m, c);
            diapason = pf.Pitches;
            Note prev = null;
            Time t = Time.Create(perfectTime);
            int pos = 0;
            bool isPos = true;
            
            foreach (var ni in infoList)
            {
                if (isPos)
                {
                    pos = ni;
                    isPos = false;
                    continue;
                }

                var n = new Note(diapason[pos], t, ni, prev);
                if (prev != null)
                    AreAllowed(n, prev.Freqs);
                t += ni;
                n.Diapason = diapason;
                n.Filter();
                prev = n;
                result.Add(n);
                isPos = true;
            }

            return result;
        }

        internal Voice CreateMelody(uint length, Modus m, Clef c, bool perfectTime, out List<Pitch> diapason, params int[] infoList)
        {
            var notes = CreateNotes(m, c, perfectTime, out diapason, infoList);

            var mel = new Voice(c, m, Time.Create(perfectTime));
            mel.SetLength(length);

            foreach (var n in notes)
            {
                var freqs = mel.Filter();
                AreAllowed(n, freqs);

                mel.AddNote(n);
            }

            return mel;
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void CheckCEH()
        {
            List<Pitch> diapason;
            var nl = CreateNotes(Modus.Aeolian(9), Clef.Treble, false, out diapason, 0, 8, 2, 8);

            var freqs = nl.Last().Filter();

            AreDenied(n => n.Pitch == diapason[6], freqs);
        }

        [TestMethod]
        public void TestSyncopa()
        {
            List<Pitch> diapason;
            Voice m = CreateMelody(500, Modus.Aeolian(9), Clef.Treble, false, out diapason, 5, 8, 1, 2, 2, 2);

            FreqsDict freqs = m.Filter();

            AreDenied(n => n.Duration == 4, freqs);
        }

        [TestMethod]
        public void TestMultiLeaps()
        {
            List<Pitch> diapason;
            Voice m = CreateMelody(500, Modus.Phrygian(4), Clef.Treble, false, out diapason, 2, 8, 3, 8, 4, 8, 3, 8);

            FreqsDict freqs = m.Filter();

            AreDenied(n => n.Pitch == diapason[10], freqs);
        }

        [TestMethod]
        public void TestCadenza()
        {
            List<Pitch> diapason;
            Voice m = CreateMelody(36, Modus.Dorian(2), Clef.Treble, true, out diapason, 5, 12, 4, 4, 3, 4, 2, 4);

            FreqsDict freqs = m.Filter();

            AreAllowed(n => (n.Pitch == diapason[1]) && (n.Duration == 12), freqs);
        }

        [TestMethod]
        public void TestCadenza2()
        {
            List<Pitch> diapason;
            Voice m = CreateMelody(24, Modus.Dorian(2), Clef.Treble, true, out diapason, 5, 4, 8, 6, 7, 2);

            FreqsDict freqs = m.Filter();

            AreAllowed(n => (n.Pitch == diapason[8]) && (n.Duration == 12), freqs);
        }

        [TestMethod]
        public void TestTritone()
        {
            try
            {
                List<Pitch> diapason;
                CreateMelody(24, Modus.Dorian(2), Clef.Treble, true, out diapason, 3, 4, 6, 4);
            }
            catch (AssertFailedException)
            {
                return;
            }

            Assert.Fail("Can't catch expected fail");
        }
    
    }
}
