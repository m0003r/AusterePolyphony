using Microsoft.VisualStudio.TestTools.UnitTesting;
using PitchBase;

namespace NotesUnitTest
{
    [TestClass]
    public class TimeTest
    {
        [TestMethod]
        public void TimeStrongnessTest()
        {
            var t = Time.Create();
            t.Position = 0;
            Assert.AreEqual(t.Strongness, (uint)4);

            t.Position = 4;
            Assert.AreEqual(t.Strongness, (uint)2);

            t.Position = 7;
            Assert.AreEqual(t.Strongness, (uint)0);

            t.Position = 12;
            Assert.AreEqual(t.Strongness, (uint)2);

            t.Position = 8;
            Assert.AreEqual(t.Strongness, (uint)3);
        }
    }
}
