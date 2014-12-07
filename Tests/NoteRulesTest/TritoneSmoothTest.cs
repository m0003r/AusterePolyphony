using System;
using System.Collections.Generic;
using System.Linq;
using Compositor.Levels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PitchBase;

namespace NoteRulesTest
{
    [TestClass]
    public class TritoneSmoothTest : MelodyTestBase
    {
        [TestMethod]
        public void InTritone1()
        {
            List<Pitch> diapason;
            var m = CreateMelody(64, Modus.Dorian(2), Clef.Treble, false, out diapason,
                5, 4,
                3, 4,
                4, 4,
                5, 4,
                6, 4); //a2 f g a h

            var freqs = m.Filter();

            AreDenied(n => n.Pitch == diapason[5], freqs);
            AreAllowed(n => n.Pitch == diapason[7], freqs);

            AppendNote(m, diapason[7], 4);

            freqs = m.Filter();
            AreAllowed(n => n.Pitch == diapason[4] && n.Duration == 8, freqs);
        }
    }
}
