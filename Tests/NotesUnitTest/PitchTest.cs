using PitchBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NotesUnitTest
{
    
    
    /// <summary>
    ///This is a test class for PitchTest and is intended
    ///to contain all PitchTest Unit Tests
    ///</summary>
    [TestClass]
    public class PitchTest
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
        ///A test for isTritoneHigh
        ///</summary>
        [TestMethod]
        public void IsTritoneHighTest()
        {
            const int pitchValue = 5;
            var pitchModus = Modus.Dorian();
            var target = new Pitch(pitchValue, pitchModus);
            Assert.IsTrue(target.IsTritoneHigh);
        }

        /// <summary>
        ///A test for isTritoneLow
        ///</summary>
        [TestMethod]
        public void IsTritoneLowTest()
        {
            const int pitchValue = 2; 
            var pitchModus = Modus.Dorian();
            var target = new Pitch(pitchValue, pitchModus);
            Assert.IsTrue(target.IsTritoneLow);
        }
    }
}
