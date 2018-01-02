using System;
using System.Collections.Generic;
using Assets.Scripts;
using NUnit.Framework;

namespace Assets.Editor
{
    public class ActionMasterTests
    {

        private readonly ActionMaster _systemUnderTest;

        public ActionMasterTests()
        {
            _systemUnderTest = new ActionMaster();
        }

        [TestCaseSource(nameof(InvalidPinCountCases))]
        public void T01_WhenArgumentIsBelow0OrAbove10_OutOfRangeExceptionIsThrown(int pins)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _systemUnderTest.Bowl(pins));
        }

        [Test]
        public void T02_WhenStrike_EndTurn()
        {
            Assert.AreEqual(AfterStrikeAction.EndTurn, _systemUnderTest.Bowl(10));
        }

        private static IEnumerable<int> InvalidPinCountCases()
        {
            yield return -1;
            yield return 11;
        }
    }
}
