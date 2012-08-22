using Notes;
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
        ///A test for Degree
        ///</summary>
        [TestMethod()]
        public void DegreeTest()
        {
            int PitchValue = 0; // TODO: Initialize to an appropriate value
            Modus PitchModus = null; // TODO: Initialize to an appropriate value
            Pitch target = new Pitch(PitchValue, PitchModus); // TODO: Initialize to an appropriate value
            uint actual;
            actual = target.Degree;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FromBase
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Notes.dll")]
        public void FromBaseTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            Pitch_Accessor target = new Pitch_Accessor(param0); // TODO: Initialize to an appropriate value
            Interval actual;
            actual = target.FromBase;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InDiatonic
        ///</summary>
        [TestMethod()]
        public void InDiatonicTest()
        {
            int PitchValue = 0; // TODO: Initialize to an appropriate value
            Modus PitchModus = null; // TODO: Initialize to an appropriate value
            Pitch target = new Pitch(PitchValue, PitchModus); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.InDiatonic;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModusOctave
        ///</summary>
        [TestMethod()]
        public void ModusOctaveTest()
        {
            int PitchValue = 0; // TODO: Initialize to an appropriate value
            Modus PitchModus = null; // TODO: Initialize to an appropriate value
            Pitch target = new Pitch(PitchValue, PitchModus); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.ModusOctave;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PitchModus
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Notes.dll")]
        public void PitchModusTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            Pitch_Accessor target = new Pitch_Accessor(param0); // TODO: Initialize to an appropriate value
            Modus expected = null; // TODO: Initialize to an appropriate value
            Modus actual;
            target.PitchModus = expected;
            actual = target.PitchModus;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PitchValue
        ///</summary>
        [TestMethod()]
        public void PitchValueTest()
        {
            int PitchValue = 0; // TODO: Initialize to an appropriate value
            Modus PitchModus = null; // TODO: Initialize to an appropriate value
            Pitch target = new Pitch(PitchValue, PitchModus); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.PitchValue = expected;
            actual = target.PitchValue;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RealAlteration
        ///</summary>
        [TestMethod()]
        public void RealAlterationTest()
        {
            int PitchValue = 0; // TODO: Initialize to an appropriate value
            Modus PitchModus = null; // TODO: Initialize to an appropriate value
            Pitch target = new Pitch(PitchValue, PitchModus); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.RealAlteration;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RealOctave
        ///</summary>
        [TestMethod()]
        public void RealOctaveTest()
        {
            int PitchValue = 0; // TODO: Initialize to an appropriate value
            Modus PitchModus = null; // TODO: Initialize to an appropriate value
            Pitch target = new Pitch(PitchValue, PitchModus); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.RealOctave;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for StringForm
        ///</summary>
        [TestMethod()]
        public void StringFormTest()
        {
            int PitchValue = 0; // TODO: Initialize to an appropriate value
            Modus PitchModus = null; // TODO: Initialize to an appropriate value
            Pitch target = new Pitch(PitchValue, PitchModus); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.StringForm;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
