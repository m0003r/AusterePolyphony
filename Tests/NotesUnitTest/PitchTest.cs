using PitchBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace NotesUnitTest
{
    
    
    /// <summary>
    ///This is a test class for PitchTest and is intended
    ///to contain all PitchTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PitchTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

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
        [TestMethod()]
        public void isTritoneHighTest()
        {
            int PitchValue = 5;
            Modus PitchModus = Modus.Dorian();
            Pitch target = new Pitch(PitchValue, PitchModus);
            Assert.IsTrue(target.isTritoneHigh);
        }

        /// <summary>
        ///A test for isTritoneLow
        ///</summary>
        [TestMethod()]
        public void isTritoneLowTest()
        {
            int PitchValue = 2; 
            Modus PitchModus = Modus.Dorian();
            Pitch target = new Pitch(PitchValue, PitchModus);
            Assert.IsTrue(target.isTritoneLow);
        }
    }
}
