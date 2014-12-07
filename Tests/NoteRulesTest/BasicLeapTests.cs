using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PitchBase;

namespace NoteRulesTest
{
    [TestClass]
    public class BasicLeapTests : MelodyTestBase
    {
        [TestMethod]
        public void GoodLeapsA1()
        {
            List<Pitch> diapason;

            CreateMelody(64, Modus.Dorian(2), Clef.Treble, false, out diapason,
                1, 12,
                0, 2,
                1, 2,
                2, 4,
                1, 4,
                4, 4,
                3, 2,
                4, 2,
                5, 4); //d1. c4 d | e2 d g f4 g | a2
        }

        [TestMethod]
        public void GoodLeapsA2()
        {
            List<Pitch> diapason;

            CreateMelody(64, Modus.Dorian(2), Clef.Treble, false, out diapason,
                8, 12,
                7, 2,
                6, 2,
                7, 4,
                3, 8,
                4, 2,
                3, 2,
                2, 4); //d1. c4 h | c2 f,1 g4 f | e2
        }

        [TestMethod]
        public void NormalLeap1()
        {
            List<Pitch> diapason;

            CreateMelody(64, Modus.Lydian(5), Clef.Treble, true, out diapason,
                3, 8,
                4, 2,
                3, 2,
                2, 4,
                7, 2,
                6, 2,
                5, 2,
                4, 2,
                5, 4);
        }

        [TestMethod]
        public void WorseLeap()
        {
            List<Pitch> diapason;

            var m = CreateMelody(64, Modus.Dorian(2), Clef.Treble, true, out diapason,
                1, 8,
                2, 2,
                3, 2,
                2, 4,
                7, 4,
                6, 4,
                7, 4);

            Satisfy(n => (n.Pitch == diapason[7] && n.Duration == 4), f => f < 0.5, m.Notes[3].Freqs);
            Satisfy(n => (n.Pitch == diapason[7] && n.Duration == 4), f => f < 0.5, m.Notes[5].Freqs);
        }

        [TestMethod]
        public void NormalLeap2()
        {
            List<Pitch> diapason;

            var m = CreateMelody(64, Modus.Phrygian(4), Clef.Treble, true, out diapason,
                2, 8,
                5, 2,
                4, 2,
                3, 4,
                10, 2,
                9, 2,
                8, 2,
                7, 2);

            m.Filter();

            IsAllowed(n => (n.Pitch == diapason[8] && n.Duration == 4), m.Freqs);
        }

        [TestMethod]
        public void BadLeaps()
        {
            try
            {
                List<Pitch> diapason;
                CreateMelody(64, Modus.Phrygian(4), Clef.Treble, true, out diapason,
                    2, 4,
                    3, 4,
                    4, 4,
                    3, 4,
                    10, 2,
                    9, 2);

                Assert.Fail("Can't catch expected fail!");

            }
            catch (AssertFailedException) { }
        }
    }
}
