using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PitchBase;

namespace NoteRulesTest
{
    [TestClass]
    public class BasicLeapTests : MelodyTestBase
    {
        [TestMethod]
        public void GoodLeapsA()
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
    }
}
