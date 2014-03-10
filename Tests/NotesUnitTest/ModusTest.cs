using PitchBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NotesUnitTest
{
    
    
    /// <summary>
    ///This is a test class for ModusTest and is intended
    ///to contain all ModusTest Unit Tests
    ///</summary>
    [TestClass]
    public class ModusTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for DiatonicStart
        ///</summary>
        [TestMethod]
        public void DiatonicStartTest()
        {
            Modus target = Modus.Ionian();
            Assert.AreEqual(0, target.DiatonicStart);

            target = Modus.Dorian();
            Assert.AreEqual(0, target.DiatonicStart);

            target = Modus.Phrygian();
            Assert.AreEqual(0, target.DiatonicStart);

            target = Modus.Lydian();
            Assert.AreEqual(0, target.DiatonicStart);

            target = Modus.Mixolydian();
            Assert.AreEqual(0, target.DiatonicStart);

            target = Modus.Aeolian();
            Assert.AreEqual(0, target.DiatonicStart);
        }

        /// <summary>
        ///A test for DiatonicStart2
        ///</summary>
        [TestMethod]
        public void DiatonicStartTest2()
        {
            Modus target = Modus.Ionian(1);
            Assert.AreEqual(1, target.DiatonicStart);

            target = Modus.Dorian(1);
            Assert.AreEqual(0, target.DiatonicStart);

            target = Modus.Phrygian(3);
            Assert.AreEqual(1, target.DiatonicStart);

            target = Modus.Lydian(6);
            Assert.AreEqual(4, target.DiatonicStart);

            target = Modus.Mixolydian(5);
            Assert.AreEqual(3, target.DiatonicStart);

            target = Modus.Aeolian(-2);
            Assert.AreEqual(6, target.DiatonicStart);
        }

        /// <summary>
        ///A test for Keys
        ///</summary>
        [TestMethod]
        public void KeysTest()
        {
            Modus target = Modus.Ionian(1);
            Assert.AreEqual(-5, target.Keys);

            target = Modus.Dorian(1);
            Assert.AreEqual(5, target.Keys);

            target = Modus.Phrygian(3);
            Assert.AreEqual(5, target.Keys);

            target = Modus.Lydian(6);
            Assert.AreEqual(-5, target.Keys);

            target = Modus.Mixolydian(5);
            Assert.AreEqual(-2, target.Keys);

            target = Modus.Aeolian(-2);
            Assert.AreEqual(-5, target.Keys);
        }


        /// <summary>
        ///A test for NotesDelta
        ///</summary>
        [TestMethod]
        public void NotesDeltaTest()
        {

            Modus target = Modus.Ionian();
            Assert.AreEqual(0, target.NotesDelta);

            target = Modus.Dorian();
            Assert.AreEqual(1, target.NotesDelta);

            target = Modus.Phrygian();
            Assert.AreEqual(2, target.NotesDelta);

            target = Modus.Lydian();
            Assert.AreEqual(3, target.NotesDelta);

            target = Modus.Mixolydian();
            Assert.AreEqual(4, target.NotesDelta);

            target = Modus.Aeolian();
            Assert.AreEqual(5, target.NotesDelta);
        }
    }
}
