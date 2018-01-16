using System.Collections;
using System.Threading;
using Assets.Scripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Assets.Editor
{
    public class PinSettlementManagerTests
    {

        [Test]
        public void T01_WhenNoPinsStanding_PinsAreSettled()
        {
            float wobblingThreshold = 1f;
            PinSettlementManager sut = GetSystemUnderTest(wobblingThreshold);
            sut.ArePinsSettled(0);
            Thread.Sleep(1000);
            Assert.IsTrue(sut.ArePinsSettled(0));
        }

        [Test]
        public void T02_WhenThresholdNotMet_PinsAreWobbling()
        {
            float wobblingThreshold = 10f;
            PinSettlementManager sut = GetSystemUnderTest(wobblingThreshold);
            Assert.IsFalse(sut.ArePinsSettled(1));
        }

        [Test]
        public void T03_WhenThresholdMet_PinsAreSettled()
        {
            float wobblingThreshold = 1f;
            PinSettlementManager sut = GetSystemUnderTest(wobblingThreshold);
            sut.ArePinsSettled(1);
            Thread.Sleep(1000);
            Assert.IsTrue(sut.ArePinsSettled(1));
        }

        [Test]
        public void T04_WhenPinSettled_AndThenStrike_PinsAreSettled()
        {
            float wobblingThreshold = 1f;
            PinSettlementManager sut = GetSystemUnderTest(wobblingThreshold);
            sut.ArePinsSettled(1);
            Thread.Sleep(1000);
            sut.ArePinsSettled(1);
            sut.ArePinsSettled(0);
            Thread.Sleep(1000);
            Assert.IsTrue(sut.ArePinsSettled(0));
        }

        [Test]
        public void T05_WhenPinSettled_AndThenHitBelowThen_AndThresholdNotMet_PinsAreWobbling()
        {
            float wobblingThreshold = 1f;
            PinSettlementManager sut = GetSystemUnderTest(wobblingThreshold);
            sut.ArePinsSettled(1);
            Thread.Sleep(1000);
            sut.ArePinsSettled(1);
            Assert.IsFalse(sut.ArePinsSettled(2));
        }

        private PinSettlementManager GetSystemUnderTest(float wobblingThreshold)
        {
            return new PinSettlementManager(wobblingThreshold);
        }
    }
}
