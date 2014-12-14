using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PitchBase;

namespace NoteRulesTest
{
    [TestClass]
    public class OldMelodyTests : MelodyTestBase
    {
        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void CheckCEH()
        {
            List<Pitch> diapason;
            var nl = CreateNotes(Modus.Aeolian(9), Clef.Treble, false, out diapason, 0, 8, 2, 8);

            var freqs = nl.Last().Filter();

            IsDenied(n => n.Pitch == diapason[6], freqs);
        }

        [TestMethod]
        public void TestSyncopa()
        {
            List<Pitch> diapason;
            var m = CreateMelody(500, Modus.Aeolian(9), Clef.Treble, false, out diapason, 5, 8, 1, 2, 2, 2);

            var freqs = m.Filter();

            IsDenied(n => n.Duration == 4, freqs);
        }

        [TestMethod]
        public void TestMultiLeaps()
        {
            List<Pitch> diapason;
            var m = CreateMelody(500, Modus.Phrygian(4), Clef.Treble, false, out diapason, 2, 8, 3, 8, 4, 8, 3, 8);

            var freqs = m.Filter();

            IsDenied(n => n.Pitch == diapason[10], freqs);
        }

        [TestMethod]
        public void TestCadenza()
        {
            List<Pitch> diapason;
            var m = CreateMelody(36, Modus.Dorian(2), Clef.Treble, true, out diapason, "a'1. g2 f e");

            var freqs = m.Filter();
            
            IsOneAllowed(n => (n.PitchEquals("d'") && (n.Duration == 12)), freqs);
        }

        [TestMethod]
        public void TestCadenza2()
        {
            List<Pitch> diapason;
            var m = CreateMelody(24, Modus.Dorian(2), Clef.Treble, true, out diapason, 5, 4, 8, 6, 7, 2);

            var freqs = m.Filter();

            Console.WriteLine("Expecting d'2.");
            IsOneAllowed(n => (n.Pitch == diapason[8]) && (n.Duration == 12), freqs);
        }

        [TestMethod]
        public void TestTritone()
        {
            try
            {
                List<Pitch> diapason;
                CreateMelody(24, Modus.Lydian(5), Clef.Treble, true, out diapason, "f2 h2");
            }
            catch (AssertFailedException)
            {
                return;
            }

            Assert.Fail("Can't catch expected fail");
        }
    
    }
}
