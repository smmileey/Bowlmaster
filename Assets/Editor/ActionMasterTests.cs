using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Enums;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Editor
{
    public class ActionMasterTests
    {
        [Test]
        public void T01_WhenArgumentIsNull_NullReferenceExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() => GetSystemUnderTest().NextAction(null));
        }

        [Test]
        public void T02_WhenStrike_ReturnEndTurn()
        {
            Assert.AreEqual(AfterStrikeAction.EndTurn, GetSystemUnderTest().NextAction(new List<int> { 10 }));
        }

        [Test]
        public void T02a_WhenStrikeAndHitBelowTen_ReturnTidy()
        {
            ActionMaster systemUnderTest = GetSystemUnderTest();
            Assert.AreEqual(AfterStrikeAction.Tidy, systemUnderTest.NextAction(new List<int> { 10, 2 }));
        }

        [TestCaseSource(nameof(OneToNinePinsCases))]
        public void T03_WhenNotAllPinsHitOnSingleThrow_ReturnTidy(List<int> throws)
        {
            Assert.AreEqual(AfterStrikeAction.Tidy, GetSystemUnderTest().NextAction(throws));
        }

        [Test]
        public void T04_WhenSpare_ReturnEndTurn()
        {
            var sut = GetSystemUnderTest();
            Assert.AreEqual(AfterStrikeAction.EndTurn, sut.NextAction(new List<int> { 2, 8 }));
        }

        [Test]
        public void T05_WhenLastFrameFirstHitUnderTen_ThenReturnTidy()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();
            lastFrameStartList.Add(2);
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T06_WhenLastFrameTwoHitsUnderTen_ThenReturnEndGame()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();
            lastFrameStartList.AddRange(new[] { 2, 3 });
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T07_WhenLastFrameSpare_ThenReturnReset()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();
            lastFrameStartList.AddRange(new[] { 2, 8 });
            Assert.AreEqual(AfterStrikeAction.Reset, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T08_WhenLastFrameFirstHitStrike_ThenReturnReset()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();
            lastFrameStartList.Add(10);
            Assert.AreEqual(AfterStrikeAction.Reset, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T09_WhenLastFrameFirstHitStrikeThenLessThanTen_ReturnTidy()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();
            lastFrameStartList.AddRange(new[] { 10, 2 });
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T10_WheLastFrameSpare_AwardThrow_AndReturnEndGame()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();
            lastFrameStartList.AddRange(new[] { 4, 6, 10 });
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T11_WheLastFrameTwoStrikes_AwardThrow_AndReturnEndGame()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();
            lastFrameStartList.AddRange(new[] { 10, 10, 5 });
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T12_GameHasFinished_WithNoAwardThrow_ThenEndGameReturned()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();
            lastFrameStartList.AddRange(new[] { 0, 2 });
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T13_WhenStrikeOn19thAndZeroOn20th_ReturnTidy()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();
            lastFrameStartList.AddRange(new[] { 10, 0 });
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T14_WhenNothingHitsInTwentyThrows_ReturnEndGame()
        {
            var sut = GetSystemUnderTest();
            var lastFramewStartList = new List<int>(Enumerable.Repeat(0, 20));
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.NextAction(lastFramewStartList));
        }

        [Test]
        public void T15_WhenKnockZeroPinsAndTenPins_ShouldBeInFirstFrameSecondThrow()
        {
            var sut = GetSystemUnderTest();
            var lastFramewStartList = new List<int> { 0, 10, 3 };
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.NextAction(lastFramewStartList));
        }

        [Test]
        public void T16_WhenKnockZeroPinsAndTenPins_NextTwoThrowWithSumBelowTenReturnEndTurn()
        {
            var sut = GetSystemUnderTest();
            var lastFramewStartList = new List<int> { 0, 10, 2, 7 };
            Assert.AreEqual(AfterStrikeAction.EndTurn, sut.NextAction(lastFramewStartList));
        }

        [Test]
        public void T17_WhenKnockZeroPinsAndTenPins_ThreeTimes_ShouldBeInFirstFrameFourthThrow()
        {
            var sut = GetSystemUnderTest();
            var lastFramewStartList = new List<int> { 0, 10, 0, 10, 0, 10, 2 };
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.NextAction(lastFramewStartList));
        }

        [Test]
        public void T18_WhenThreeStrikesInTenthFrame_ReturnTwoResetsAndEndGame()
        {
            var sut = GetSystemUnderTest();
            List<int> lastFrameStartList = ProceedToTheLastFrame();

            lastFrameStartList.Add(10);
            Assert.AreEqual(AfterStrikeAction.Reset, sut.NextAction(lastFrameStartList));
            lastFrameStartList.Add(10);
            Assert.AreEqual(AfterStrikeAction.Reset, sut.NextAction(lastFrameStartList));
            lastFrameStartList.Add(10);
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.NextAction(lastFrameStartList));
        }

        [Test]
        public void T19_WhenTwoHitsUnderTen_ThatExceedsSumOfTen_ThrowsException()
        {
            var sut = GetSystemUnderTest();
            var lastFramewStartList = new List<int> { 3, 8 };
            Assert.Throws<UnityException>(() => sut.NextAction(lastFramewStartList));
        }

        [TestCaseSource(nameof(InvalidThrowsCases))]
        public void T20_WhenIncorrectArgument_ArgumentOutOfRangeIsThrown(List<int> throws)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSystemUnderTest().NextAction(throws));
        }

        private ActionMaster GetSystemUnderTest()
        {
            return new ActionMaster();
        }

        private List<int> ProceedToTheLastFrame()
        {
            return Enumerable.Repeat(1, 18).ToList();
        }

        private static IEnumerable<List<int>> InvalidThrowsCases()
        {
            yield return new List<int>();
            yield return new List<int>(Enumerable.Repeat(1, 22));
        }

        private static IEnumerable<List<int>> OneToNinePinsCases()
        {
            for (int i = 1; i <= 9; i++)
            {
                yield return new List<int> { i };
            }
        }
    }
}
