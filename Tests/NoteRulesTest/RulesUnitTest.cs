using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compositor;
using Compositor.Levels;
using PitchBase;
using System.Collections.Generic;
using System.Linq;


namespace RulesTest
{
    [TestClass]
    public class RulesUnitTest
    {
        internal void Satisfy(Predicate<Note> expectedNote,
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

        internal void AreAllowed(Note expectedNote,
            Dictionary<Note, double> freqs)
        {
            AreAllowed(n => n.Equals(expectedNote), freqs);
        }

        internal void AreAllowed(Predicate<Note> expectedNote,
            Dictionary<Note, double> freqs)
        {
            int count = freqs.Count(kv => expectedNote(kv.Key));
            Assert.IsTrue(count > 0);
            Satisfy(expectedNote, x => x > 0.03, freqs);
        }

        internal void AreDenied(Predicate<Note> expectedNote,
            Dictionary<Note, double> freqs)
        {
            Satisfy(expectedNote, x => x < 0.03, freqs);
        }

        internal List<Note> CreateNotes(Modus m, Clef c, bool perfectTime, out List<Pitch> diapason, params int[] infoList)
        {
            List<Note> result = new List<Note>();
            PitchFactory pf = new PitchFactory(m, c);
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
                else
                {
                    Note n = new Note(diapason[pos], t, ni, prev);
                    if (prev != null)
                        AreAllowed(n, prev.Freqs);
                    t += ni;
                    n.Diapason = diapason;
                    n.Filter();
                    prev = n;
                    result.Add(n);
                    isPos = true;
                }
            }

            return result;
        }

        internal Melody CreateMelody(uint length, Modus m, Clef c, bool perfectTime, out List<Pitch> diapason, params int[] infoList)
        {
            List<Note> notes = CreateNotes(m, c, perfectTime, out diapason, infoList: infoList);

            Melody mel = new Melody(c, m, Time.Create(perfectTime));
            mel.setLength(length);

            foreach (var n in notes)
            {
                var freqs = mel.Filter();
                AreAllowed(n, freqs);
                mel.AddNote(n);
            }

            return mel;
        }

        [TestMethod]
        public void CheckCEH()
        {
            List<Pitch> Diapason;
            List<Note> nl = CreateNotes(Modus.Aeolian(9), Clef.Treble, false, out Diapason, 0, 8, 2, 8);

            Dictionary<Note, double> freqs = nl.Last<Note>().Filter();

            AreDenied(n => n.Pitch == Diapason[6], freqs);
        }

        [TestMethod]
        public void TestSyncopa()
        {
            List<Pitch> Diapason;
            Melody m = CreateMelody(500, Modus.Aeolian(9), Clef.Treble, false, out Diapason, 5, 8, 1, 2, 2, 2);

            Dictionary<Note, double> freqs = m.Filter();

            AreDenied(n => n.Duration == 4, freqs);
        }

        [TestMethod]
        public void TestMultiLeaps()
        {
            List<Pitch> Diapason;
            Melody m = CreateMelody(500, Modus.Phrygian(4), Clef.Treble, false, out Diapason, 2, 8, 3, 8, 4, 8, 3, 8);

            Dictionary<Note, double> freqs = m.Filter();

            AreDenied(n => n.Pitch == Diapason[10], freqs);
        }

        [TestMethod]
        public void TestCadenza()
        {
            List<Pitch> Diapason;
            Melody m = CreateMelody(36, Modus.Dorian(2), Clef.Treble, true, out Diapason, 5, 12, 4, 4, 3, 4, 2, 4);

            Dictionary<Note, double> freqs = m.Filter();

            AreAllowed(n => (n.Pitch == Diapason[1]) && (n.Duration == 12), freqs);
        }

        [TestMethod]
        public void TestCadenza2()
        {
            List<Pitch> Diapason;
            Melody m = CreateMelody(24, Modus.Dorian(2), Clef.Treble, true, out Diapason, 5, 4, 8, 6, 7, 2);

            Dictionary<Note, double> freqs = m.Filter();

            AreAllowed(n => (n.Pitch == Diapason[8]) && (n.Duration == 12), freqs);
        }

        [TestMethod]
        public void TestTritone()
        {
            List<Pitch> Diapason;

            try
            {
                Melody m = CreateMelody(24, Modus.Dorian(2), Clef.Treble, true, out Diapason, 3, 4, 6, 4);
            }
            catch (AssertFailedException)
            {
                return;
            }

            Assert.Fail("Can't catch expected fail");
        }
    
    }
}
