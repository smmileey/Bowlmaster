using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using NUnit.Framework;

namespace Assets.Editor
{
    internal class ScoreMasterTests
    {   
        [Test]
        public void T01_WhenArgumentNull_ThenArgumentNullExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() => GetSystemUnderTest().GetFrameScores(null));
        }

        [TestCaseSource(nameof(WrongNumberOfElements))]
        [Test]
        public void T02_WhenThrowsListCountIsIncorrect_ThenArgumentOutOfRangeExceptionIsThrown(List<int> throws)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSystemUnderTest().GetFrameScores(throws));
        }

        [TestCaseSource(nameof(IncorectThrowValuesCases))]
        public void T03_WhenThrowsListContainIncorrectValue_ThenArgumentOutOfRangeExceptionIsThrown(List<int> throws)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSystemUnderTest().GetFrameScores(throws));
        }

        [TestCaseSource(nameof(OneElementCases))]
        public void T04_WhenOneThrow_ThenEmptyListReturned(List<int> throws)
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            Assert.IsEmpty(systemUnderTest.GetFrameScores(throws));
        }

        [Test]
        public void T05_WhenSumOfTwoThrowsExceedsTen_ReturnArgumentOutOfRangeException()
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            Assert.Throws<ArgumentOutOfRangeException>(() => systemUnderTest.GetFrameScores(new List<int> { 7, 7 }));
        }

        [Test]
        public void T06_WhenTwoThrowsWithSumBelowTen_ThenOneElementIsReturned()
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            List<int> frameScores = systemUnderTest.GetFrameScores(new List<int> { 2, 7 });
            Assert.AreEqual(1, frameScores.Count);
        }

        [Test]
        public void T07_WhenTwoThrowsWithSumBelowTen_ThenScoreIsCalculatedCorrectly()
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            List<int> frameScores = systemUnderTest.GetFrameScores(new List<int> { 2, 7 });
            Assert.AreEqual(9, frameScores.First());
        }

        [TestCaseSource(nameof(OneStrikeCases))]
        public void T08_WhenStrike_ThenScoreIsNotCalculatedImmediately(List<int> throws)
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            Assert.IsEmpty(systemUnderTest.GetFrameScores(throws));
        }

        [TestCaseSource(nameof(OneSpareCases))]
        public void T09_WhenSpare_ThenScoreIsNotCalculatedImmediately(List<int> throws)
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            Assert.IsEmpty(systemUnderTest.GetFrameScores(throws));
        }

        [TestCaseSource(nameof(StrikeCases))]
        public void T10_WhenStrike_ThenScoreIsCorrectlyReturned(TestData testData)
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            Assert.AreEqual(testData.Score, systemUnderTest.GetFrameScores(testData.Throws).First());
        }

        [TestCaseSource(nameof(SpareCases))]
        public void T11_WhenSpare_ThenScoreIsCorrectlyReturned(TestData testData)
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            Assert.AreEqual(testData.Score, systemUnderTest.GetFrameScores(testData.Throws).First());
        }

        [TestCaseSource(nameof(RoundTenOrdinaryCases))]
        public void T12_WhenRoundTenthWithNoAwardThrow_ThenScoreIsCorrectlyReturned(TestData testData)
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            List<int> lastFrameStartingScores = ProceedToTheLastFrame();
            lastFrameStartingScores.AddRange(testData.Throws);
            Assert.AreEqual(testData.Score, systemUnderTest.GetFrameScores(lastFrameStartingScores).Last());
        }

        [TestCaseSource(nameof(RoundTenAwardThrowCases))]
        public void T13_WhenRoundTenthWithAwardThrow_ThenScoreIsCorrectlyReturned(TestData testData)
        {
            ScoreMaster systemUnderTest = GetSystemUnderTest();
            List<int> lastFrameStartingScores = ProceedToTheLastFrame();
            lastFrameStartingScores.AddRange(testData.Throws);
            Assert.AreEqual(testData.Score, systemUnderTest.GetFrameScores(lastFrameStartingScores).Last());
        }

        private ScoreMaster GetSystemUnderTest()
        {
            return new ScoreMaster();
        }

        private List<int> ProceedToTheLastFrame()
        {
            return Enumerable.Repeat(1, 18).ToList();
        }

        private static IEnumerable<List<int>> WrongNumberOfElements()
        {
            yield return new List<int>();
            yield return new List<int>(Enumerable.Repeat(1, 22));
        }

        private static IEnumerable<List<int>> IncorectThrowValuesCases()
        {
            yield return new List<int> { 2, 4, -1, 5 };
            yield return new List<int> { 2, 7, 11, 5 };
        }

        private static IEnumerable<List<int>> OneElementCases()
        {
            yield return new List<int> { 0 };
            yield return new List<int> { 2 };
            yield return new List<int> { 10 };
        }

        private static IEnumerable<List<int>> OneStrikeCases()
        {
            yield return new List<int> { 10, 0 };
            yield return new List<int> { 10, 2 };
        }

        private static IEnumerable<List<int>> OneSpareCases()
        {
            yield return new List<int> { 3, 7 };
            yield return new List<int> { 0, 10 };
        }

        private static IEnumerable<TestData> StrikeCases()
        {
            yield return new TestData { Throws = new List<int> { 10, 0, 0 }, Score = 10 };
            yield return new TestData { Throws = new List<int> { 10, 0, 1 }, Score = 11 };
            yield return new TestData { Throws = new List<int> { 10, 1, 0 }, Score = 11 };
            yield return new TestData { Throws = new List<int> { 10, 3, 5 }, Score = 18 };
            yield return new TestData { Throws = new List<int> { 10, 3, 7 }, Score = 20 };
            yield return new TestData { Throws = new List<int> { 10, 0, 10 }, Score = 20 };
            yield return new TestData { Throws = new List<int> { 10, 10, 0 }, Score = 20 };
            yield return new TestData { Throws = new List<int> { 10, 10, 10 }, Score = 30 };
        }

        private static IEnumerable<TestData> SpareCases()
        {
            yield return new TestData { Throws = new List<int> { 0, 10, 0 }, Score = 10 };
            yield return new TestData { Throws = new List<int> { 0, 10, 1 }, Score = 11 };
            yield return new TestData { Throws = new List<int> { 0, 10, 10 }, Score = 20 };
            yield return new TestData { Throws = new List<int> { 3, 7, 0 }, Score = 10 };
            yield return new TestData { Throws = new List<int> { 3, 7, 5 }, Score = 15 };
            yield return new TestData { Throws = new List<int> { 3, 7, 10 }, Score = 20 };
            yield return new TestData { Throws = new List<int> { 3, 7, 5, 2 }, Score = 15 };
            yield return new TestData { Throws = new List<int> { 0, 10, 5, 2 }, Score = 15 };
        }

        private static IEnumerable<TestData> RoundTenOrdinaryCases()
        {
            yield return new TestData { Throws = new List<int> { 0, 3 }, Score = 3 };
            yield return new TestData { Throws = new List<int> { 2, 7 }, Score = 9 };
            yield return new TestData { Throws = new List<int> { 3, 3, 3 }, Score = 6 };
        }

        private static IEnumerable<TestData> RoundTenAwardThrowCases()
        {
            yield return new TestData { Throws = new List<int> { 0, 10, 5 }, Score = 15 };
            yield return new TestData { Throws = new List<int> { 10, 3, 5 }, Score = 18 };
            yield return new TestData { Throws = new List<int> { 10, 0, 5 }, Score = 15 };
            yield return new TestData { Throws = new List<int> { 10, 10, 5 }, Score = 25 };
            yield return new TestData { Throws = new List<int> { 10, 10, 10 }, Score = 30 };
        }

        internal class TestData
        {
            public List<int> Throws { get; set; }
            public int Score { get; set; }
        }
    }
}
