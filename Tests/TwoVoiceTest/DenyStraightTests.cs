using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PitchBase;

namespace TwoVoiceTest
{
    [TestClass]
    public class DenyStraightTests : TwoVoicesTestBase
    {
        [TestMethod]
        public void Deny1()
        {
            var tw = CreateVoices(64, Modus.Dorian(2), Clef.Treble, Clef.Alto, true, "e1.", "a'2 h c");
            tw.Filter();

            IsDenied(notes => notes.Note1.PitchEquals("d'") && notes.Note2.PitchEquals("g"), tw.Freqs);
        }

        [TestMethod]
        public void Middle1()
        {
            var tw = CreateVoices(64, Modus.Dorian(2), Clef.Treble, Clef.Alto, true, "e2 d c", "a'1.");

            tw.Filter();
            IsMiddle(n => n.Note1.PitchEquals("d'") && n.Note2.PitchEquals("d"), tw.Freqs);
        }
    }
}
