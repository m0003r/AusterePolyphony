using System.Collections.Generic;
using Compositor.Levels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PitchBase;

namespace TwoVoiceTest
{
    [TestClass]
    public class SimpleTwoVoiceTest : TwoVoicesTestBase
    {
        [TestMethod]
        public void SimpleTwoVoices()
        {
            CreateVoices(64, Modus.Dorian(2), Clef.Treble, Clef.Alto, false, "c1 d", "e1 d");
        }

        [TestMethod]
        public void SimplePassingTest()
        {
            CreateVoices(64, Modus.Phrygian(4), Clef.Treble, Clef.Alto, false, "g'4 f e d c2 e", "e2 g a4 g8 f e2");
        }

        [TestMethod]
        public void SimplePassingTest2()
        {
            CreateVoices(128, Modus.Phrygian(4), Clef.Treble, Clef.Tenor, false, "d1 e c4 d e f g1 e2", "h' a g f e1 c4 d8 e f2 g2");
        }


        [TestMethod]
        public void SimplePassingTest3()
        {
            CreateVoices(256, Modus.Aeolian(4), Clef.Treble, Clef.Tenor, true, "e2 fis g a1. g1 c2 h4", "e1. fis4 g a h c d e2 d4 c8 h a2 g4");
        }
    }
}
