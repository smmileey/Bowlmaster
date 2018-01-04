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
        [TestCaseSource(nameof(InvalidPinCountCases))]
        public void T01_WhenArgumentIsBelow0OrAbove10_OutOfRangeExceptionIsThrown(int pinsCount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => GetSystemUnderTest().Bowl(pinsCount));
        }

        [Test]
        public void T02_WhenStrike_ReturnEndTurn()
        {
            Assert.AreEqual(AfterStrikeAction.EndTurn, GetSystemUnderTest().Bowl(10));
        }

        [TestCaseSource(nameof(OneToNinePinsCases))]
        public void T03_WhenNotAllPinsHitOnSingleThrow_ReturnTidy(int pinsCount)
        {
            Assert.AreEqual(AfterStrikeAction.Tidy, GetSystemUnderTest().Bowl(pinsCount));
        }

        [Test]
        public void T04_WhenSpare_ReturnEndTurn()
        {
            var sut = GetSystemUnderTest();
            sut.Bowl(2);
            Assert.AreEqual(AfterStrikeAction.EndTurn, sut.Bowl(8));
        }

        [Test]
        public void T05_WhenLastFrameFirstHitUnderTen_ThenReturnTidy()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.Bowl(2));
        }

        [Test]
        public void T06_WhenLastFrameTwoHitsUnderTen_ThenReturnEndGame()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            sut.Bowl(2);
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.Bowl(3));
        }

        [Test]
        public void T07_WhenLastFrameSpare_ThenReturnReset()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            sut.Bowl(2);
            Assert.AreEqual(AfterStrikeAction.Reset, sut.Bowl(8));
        }

        [Test]
        public void T08_WhenLastFrameFirstHitStrike_ThenReturnReset()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            Assert.AreEqual(AfterStrikeAction.Reset, sut.Bowl(10));
        }

        [Test]
        public void T09_WhenLastFrameFirstHitStrikeThenLessThanTen_ReturnTidy()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            sut.Bowl(10);
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.Bowl(2));
        }

        [Test]
        public void T10_WheLastFrameSpare_AwardThrow_AndReturnEndGame()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            sut.Bowl(4);
            sut.Bowl(6);
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.Bowl(10));
        }

        [Test]
        public void T11_WheLastFrameTwoStrikes_AwardThrow_AndReturnEndGame()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            sut.Bowl(10);
            sut.Bowl(10);
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.Bowl(5));
        }

        [Test]
        public void T12_WhenEndGame_WithNoAwardThrow_ThenNextThrowBelowTenReturnsTidy()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            sut.Bowl(0);
            sut.Bowl(2);
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.Bowl(1));
        }

        [Test]
        public void T13_WhenEndGame_WithAwardThrow_ThenNextThrowBelowTenReturnsTidy()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            sut.Bowl(0);
            sut.Bowl(10);
            sut.Bowl(3);
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.Bowl(1));
        }

        [Test]
        public void T14_WhenStrikeOn19thAndZeroOn20th_ReturnTidy()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);
            sut.Bowl(10);
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.Bowl(0));
        }

        [Test]
        public void T15_WhenNothingHitsInTwentyThrows_ReturnEndGame()
        {
            var sut = GetSystemUnderTest();
            for (int i = 0; i < 19; i++)
            {
                sut.Bowl(0);
            }
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.Bowl(0));
        }

        [Test]
        public void T16_WhenKnockZeroPinsAndTenPins_ShouldBeInFirstFrameSecondThrow()
        {
            var sut = GetSystemUnderTest();
            sut.Bowl(0);
            sut.Bowl(10);
            Assert.AreEqual(3, sut.CurrentThrowNumber);
        }

        [Test]
        public void T17_WhenKnockZeroPinsAndTenPins_NextThrowBelowTenShouldReturnTidy()
        {
            var sut = GetSystemUnderTest();
            sut.Bowl(0);
            sut.Bowl(10);
            Assert.AreEqual(AfterStrikeAction.Tidy, sut.Bowl(5));
        }

        [Test]
        public void T18_WhenKnockZeroPinsAndTenPins_NextTwoThrowWithSumBelowTenReturnEndTurn()
        {
            var sut = GetSystemUnderTest();
            sut.Bowl(0);
            sut.Bowl(10);
            sut.Bowl(2);
            Assert.AreEqual(AfterStrikeAction.EndTurn, sut.Bowl(7));
        }

        [Test]
        public void T19_WhenKnockZeroPinsAndTenPins_ThreeTimes_ShouldBeInFirstFrameFourthThrow()
        {
            var sut = GetSystemUnderTest();
            for (int i = 0; i <= 2; i++)
            {
                sut.Bowl(0);
                sut.Bowl(10);
            }
            Assert.AreEqual(7, sut.CurrentThrowNumber);
        }

        [Test]
        public void T20_WhenThreeStrikesInTenthFrame_ReturnTwoResetsAndEndGame()
        {
            var sut = GetSystemUnderTest();
            ProceedToTheLastFrame(sut);

            Assert.AreEqual(AfterStrikeAction.Reset, sut.Bowl(10));
            Assert.AreEqual(AfterStrikeAction.Reset, sut.Bowl(10));
            Assert.AreEqual(AfterStrikeAction.EndGame, sut.Bowl(10));
        }

        [Test]
        public void T21_WhenTwoHitsUnderTen_ThatExceedsSumOfTen_ThrowsException()
        {
            var sut = GetSystemUnderTest();
            sut.Bowl(3);           

            Assert.Throws<UnityException>(() => sut.Bowl(8));
        }

        private ActionMaster GetSystemUnderTest()
        {
            return new ActionMaster();
        }

        private void ProceedToTheLastFrame(ActionMaster actionMaster)
        {
            for (int i = 0; i <= 8; i++)
            {
                actionMaster.Bowl(10);
            }
        }

        private static IEnumerable<int> InvalidPinCountCases()
        {
            yield return -1;
            yield return 11;
        }

        private static IEnumerable<int> OneToNinePinsCases()
        {
            return Enumerable.Range(1, 9);
        }
    }
}
