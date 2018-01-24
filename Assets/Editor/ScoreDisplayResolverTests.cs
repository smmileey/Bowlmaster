using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Consts;
using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Wrappers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Editor
{
    public class ScoreDisplayResolverTests
    {
        [Test]
        public void T01_FirstHitInFrame_FirstFrameUpdate()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(3, new List<int>(), AfterStrikeAction.Tidy);
            Assert.AreEqual("3", scoreDisplayWrappers.First().FirstScore.text);
            Assert.AreEqual(ScoreDisplayStatus.SecondRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T02_SecondHitInFrame_SecondFrameUpdate()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(3, new List<int>(), AfterStrikeAction.Tidy);
            sut.UpdateScore(5, new List<int> { 8 }, AfterStrikeAction.EndTurn);
            Assert.AreEqual("5", scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual("8", scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T03_Spare_FirstFrameUpdate_WithSpareSpecialSign()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(4, new List<int>(), AfterStrikeAction.Tidy);
            sut.UpdateScore(6, new List<int>(), AfterStrikeAction.EndTurn);
            Assert.AreEqual("/", scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T04_Strike_FirstFrameUpdate_WithStrikeSpecialSign()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(10, new List<int>(), AfterStrikeAction.EndTurn);
            Assert.AreEqual("X", scoreDisplayWrappers.First().FirstScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T05_LastRound_FirstHitInFrame_FirstFrameUpdate()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(3, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Tidy);
            Assert.AreEqual("3", scoreDisplayWrappers.First().FirstScore.text);
            Assert.AreEqual(ScoreDisplayStatus.SecondRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T06_LastRound_TwoHitsUnderTen_FrameScoreUpdated_AndGameIsCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(3, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Tidy);
            sut.UpdateScore(4, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2, 7 }, AfterStrikeAction.EndGame);
            Assert.AreEqual("7", scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().ThirdScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T07_LastRound_StrikeInFirstFrame_FirstFrameUpdatedWithStrikeSpecialSign_AndGameNotCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(10, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            Assert.AreEqual("X", scoreDisplayWrappers.First().FirstScore.text);
            Assert.AreEqual(ScoreDisplayStatus.SecondRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T08_LastRound_StrikeInSecondFrame_SecondFrameUpdatedWithStrikeSpecialSign_AndGameNotCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(10, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            sut.UpdateScore(10, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            Assert.AreEqual("X", scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.ThirdRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T09_LastRound_AdditionalRoundBySpare_ThirdScoreUpdate_AndGameNotCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(3, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Tidy);
            sut.UpdateScore(7, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            Assert.AreEqual("/", scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.ThirdRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T10_LastRound_AdditionalRoundByStrike_ThirdScoreUpdate_AndGameNotCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            sut.UpdateScore(10, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            sut.UpdateScore(3, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            sut.UpdateScore(5, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2, 18 }, AfterStrikeAction.EndGame);
            Assert.AreEqual("5", scoreDisplayWrappers.First().ThirdScore.text);
            Assert.AreEqual("18", scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        //test cases checked while play-testing defects discovered
        [Test]
        public void T11_WhenLesThanTen_AfterStrike_FirstScoreAndSecondScoreIsCorrectlyPopulated()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1", "F2", "F3", "F4");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers);

            //Frame 1
            sut.UpdateScore(6, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdateScore(2, new List<int> { 8 }, AfterStrikeAction.EndTurn);
            //Frame 2
            sut.UpdateScore(9, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdateScore(0, new List<int> { 8, 9 }, AfterStrikeAction.EndTurn);
            //Frame 3 (strike)
            sut.UpdateScore(10, new List<int> { 8, 9 }, AfterStrikeAction.EndTurn);
            //Frame 4
            sut.UpdateScore(7, new List<int> { 8, 9 }, AfterStrikeAction.Tidy);
            sut.UpdateScore(2, new List<int> { 8, 9, 19, 9 }, AfterStrikeAction.EndTurn);
            Assert.AreEqual("X", scoreDisplayWrappers[2].FirstScore.text);
            Assert.AreEqual("19", scoreDisplayWrappers[2].FrameScore.text);
            Assert.AreEqual("7", scoreDisplayWrappers[3].FirstScore.text);
            Assert.AreEqual("2", scoreDisplayWrappers[3].SecondScore.text);
            Assert.AreEqual("9", scoreDisplayWrappers[3].FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[2].ScoreDisplayStatus);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[3].ScoreDisplayStatus);
        }


        /// <summary>
        /// Text is not mockable, and there is no possibility to create instance of it using ctor. Thus, I'm creating that with my ScoreDisplayResolver class that has its own unit tests implemented.
        /// </summary>
        /// <param name="frameIndexes"></param>
        /// <returns></returns>
        private List<ScoreDisplayWrapper> GetScoreDisplayWrappers(params string[] frameIndexes)
        {
            GameObject scoreDisplay = new GameObject();
            foreach (var frameIndex in frameIndexes) { GetScoreDisplayWrapper(frameIndex, scoreDisplay); }
            return new ScoreDisplayConverter().Convert(scoreDisplay);
        }

        private static void GetScoreDisplayWrapper(string frameIndex, GameObject scoreDisplay)
        {
            AddChild(frameIndex, scoreDisplay);
        }

        private static void AddChild(string frameIndex, GameObject scoreDisplay)
        {
            GameObject f1 = new GameObject(frameIndex);
            f1.transform.SetParent(scoreDisplay.transform);

            GameObject one = new GameObject("1") { tag = Tags.FirstScore };
            one.transform.SetParent(f1.transform);
            GameObject two = new GameObject("2") { tag = Tags.SecondScore };
            two.transform.SetParent(f1.transform);
            GameObject three = new GameObject("3") { tag = Tags.ThirdScore };
            three.transform.SetParent(f1.transform);
            GameObject sum = new GameObject("Sum") { tag = Tags.FrameScore };
            sum.transform.SetParent(f1.transform);

            GameObject text = new GameObject("text");
            text.AddComponent(typeof(Text));
            text.transform.SetParent(one.transform);
            GameObject textTwo = new GameObject("text");
            textTwo.AddComponent(typeof(Text));
            textTwo.transform.SetParent(two.transform);
            GameObject textThree = new GameObject("text");
            textThree.AddComponent(typeof(Text));
            textThree.transform.SetParent(three.transform);
            GameObject textSum = new GameObject("text");
            textSum.AddComponent(typeof(Text));
            textSum.transform.SetParent(sum.transform);
        }
    }
}
