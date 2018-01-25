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
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(3, new List<int>(), AfterStrikeAction.Tidy);
            Assert.AreEqual("3", scoreDisplayWrappers.First().FirstScore.text);
            Assert.AreEqual(ScoreDisplayStatus.SecondRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T02_SecondHitInFrame_SecondFrameUpdate()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(3, new List<int>(), AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(5, new List<int> { 8 }, AfterStrikeAction.EndTurn);
            Assert.AreEqual("5", scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual("8", scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T03_Spare_FirstFrameUpdate_WithSpareSpecialSign()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(4, new List<int>(), AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(6, new List<int>(), AfterStrikeAction.EndTurn);
            Assert.AreEqual("/", scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T04_Strike_FirstFrameUpdate_WithStrikeSpecialSign()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(10, new List<int>(), AfterStrikeAction.EndTurn);
            Assert.AreEqual("X", scoreDisplayWrappers.First().FirstScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T05_LastRound_FirstHitInFrame_FirstFrameUpdate()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(3, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Tidy);
            Assert.AreEqual("3", scoreDisplayWrappers.First().FirstScore.text);
            Assert.AreEqual(ScoreDisplayStatus.SecondRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T06_LastRound_TwoHitsUnderTen_FrameScoreUpdated_AndGameIsCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(3, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(4, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2, 7 }, AfterStrikeAction.EndGame);
            Assert.AreEqual("7", scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().ThirdScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T07_LastRound_StrikeInFirstFrame_FirstFrameUpdatedWithStrikeSpecialSign_AndGameNotCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(10, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            Assert.AreEqual("X", scoreDisplayWrappers.First().FirstScore.text);
            Assert.AreEqual(ScoreDisplayStatus.SecondRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T08_LastRound_StrikeInSecondFrame_SecondFrameUpdatedWithStrikeSpecialSign_AndGameNotCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(10, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(10, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            Assert.AreEqual("X", scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.ThirdRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T09_LastRound_AdditionalRoundBySpare_ThirdScoreUpdate_AndGameNotCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(3, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(7, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            Assert.AreEqual("/", scoreDisplayWrappers.First().SecondScore.text);
            Assert.AreEqual(string.Empty, scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.ThirdRound, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        [Test]
        public void T10_LastRound_AdditionalRoundByStrike_ThirdScoreUpdate_AndGameNotCompleted()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            sut.UpdatFrameScores(10, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(3, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(5, new List<int> { 2, 2, 2, 2, 2, 2, 2, 2, 2, 18 }, AfterStrikeAction.EndGame);
            Assert.AreEqual("5", scoreDisplayWrappers.First().ThirdScore.text);
            Assert.AreEqual("18", scoreDisplayWrappers.First().FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers.First().ScoreDisplayStatus);
        }

        //test cases checked while play-testing defects discovered
        [Test]
        public void T11_WhenLesThanTen_AfterStrike_FirstScoreAndSecondScoreIsCorrectlyPopulated()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1", "F2", "F3", "F4");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            //Frame 1
            sut.UpdatFrameScores(6, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(2, new List<int> { 8 }, AfterStrikeAction.EndTurn);
            //Frame 2
            sut.UpdatFrameScores(9, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 8, 9 }, AfterStrikeAction.EndTurn);
            //Frame 3 (strike)
            sut.UpdatFrameScores(10, new List<int> { 8, 9 }, AfterStrikeAction.EndTurn);
            //Frame 4
            sut.UpdatFrameScores(7, new List<int> { 8, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(2, new List<int> { 8, 9, 19, 9 }, AfterStrikeAction.EndTurn);
            Assert.AreEqual("X", scoreDisplayWrappers[2].FirstScore.text);
            Assert.AreEqual("19", scoreDisplayWrappers[2].FrameScore.text);
            Assert.AreEqual("7", scoreDisplayWrappers[3].FirstScore.text);
            Assert.AreEqual("2", scoreDisplayWrappers[3].SecondScore.text);
            Assert.AreEqual("9", scoreDisplayWrappers[3].FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[2].ScoreDisplayStatus);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[3].ScoreDisplayStatus);
        }

        [Test]
        public void T12_LastRound_LessThanThen_SecondScoreShouldBePopulated()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            //Frame 1
            sut.UpdatFrameScores(9, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9 }, AfterStrikeAction.EndTurn);
            //Frame 2
            sut.UpdatFrameScores(9, new List<int> { 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 3 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 4
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(2, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 5 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 6 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.EndTurn);
            //Frame 7 (strike)
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.EndTurn);
            //Frame 8
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.EndTurn);
            //Frame 9
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.EndTurn);
            //Frame 10
            sut.UpdatFrameScores(9, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8, 9 }, AfterStrikeAction.EndGame);

            Assert.AreEqual("9", scoreDisplayWrappers[9].FirstScore.text);
            Assert.AreEqual("0", scoreDisplayWrappers[9].SecondScore.text);
            Assert.AreEqual("9", scoreDisplayWrappers[9].FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[9].ScoreDisplayStatus);
        }

        [Test]
        public void T13_LastRound_AdditionalThrowGrantedBySpare_ThenLessThanTen_ThirdScoreShouldBePopulated()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            //Frame 1
            sut.UpdatFrameScores(9, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9 }, AfterStrikeAction.EndTurn);
            //Frame 2
            sut.UpdatFrameScores(9, new List<int> { 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 3 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 4
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(2, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 5 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 6 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.EndTurn);
            //Frame 7 (strike)
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.EndTurn);
            //Frame 8
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.EndTurn);
            //Frame 9
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.EndTurn);
            //Frame 10 (spare + bonus throw)
            sut.UpdatFrameScores(9, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(1, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8, 13 }, AfterStrikeAction.EndGame);

            Assert.AreEqual("9", scoreDisplayWrappers[9].FirstScore.text);
            Assert.AreEqual("/", scoreDisplayWrappers[9].SecondScore.text);
            Assert.AreEqual("3", scoreDisplayWrappers[9].ThirdScore.text);
            Assert.AreEqual("13", scoreDisplayWrappers[9].FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[9].ScoreDisplayStatus);
        }

        [Test]
        public void T14_LastRound_AdditionalThrowGrantedBySpare_ThenStrike_ThirdScoreShouldBePopulated()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            //Frame 1
            sut.UpdatFrameScores(9, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9 }, AfterStrikeAction.EndTurn);
            //Frame 2
            sut.UpdatFrameScores(9, new List<int> { 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 3 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 4
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(2, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 5 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 6 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.EndTurn);
            //Frame 7 (strike)
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.EndTurn);
            //Frame 8
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.EndTurn);
            //Frame 9
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.EndTurn);
            //Frame 10 (spare + bonus throw)
            sut.UpdatFrameScores(9, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(1, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8, 20 }, AfterStrikeAction.EndGame);

            Assert.AreEqual("9", scoreDisplayWrappers[9].FirstScore.text);
            Assert.AreEqual("/", scoreDisplayWrappers[9].SecondScore.text);
            Assert.AreEqual("X", scoreDisplayWrappers[9].ThirdScore.text);
            Assert.AreEqual("20", scoreDisplayWrappers[9].FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[9].ScoreDisplayStatus);
        }

        [Test]
        public void T15_LastRound_AdditionalThrowGrantedByStrikeAndLessThanTen_ThirdScoreShouldBePopulated()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            //Frame 1
            sut.UpdatFrameScores(9, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9 }, AfterStrikeAction.EndTurn);
            //Frame 2
            sut.UpdatFrameScores(9, new List<int> { 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 3 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 4
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(2, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 5 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 6 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.EndTurn);
            //Frame 7 (strike)
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.EndTurn);
            //Frame 8
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.EndTurn);
            //Frame 9
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.EndTurn);
            //Frame 10 (strike + less than ten + bonus throw)
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(2, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8, 15 }, AfterStrikeAction.EndGame);

            Assert.AreEqual("X", scoreDisplayWrappers[9].FirstScore.text);
            Assert.AreEqual("3", scoreDisplayWrappers[9].SecondScore.text);
            Assert.AreEqual("2", scoreDisplayWrappers[9].ThirdScore.text);
            Assert.AreEqual("15", scoreDisplayWrappers[9].FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[9].ScoreDisplayStatus);
        }

        [Test]
        public void T16_LastRound_AdditionalThrowGrantedByDoubleStrike_AndBonusThrowLessThanTen_ThirdScoreShouldBePopulated()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            //Frame 1
            sut.UpdatFrameScores(9, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9 }, AfterStrikeAction.EndTurn);
            //Frame 2
            sut.UpdatFrameScores(9, new List<int> { 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 3 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 4
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(2, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 5 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 6 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.EndTurn);
            //Frame 7 (strike)
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.EndTurn);
            //Frame 8
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.EndTurn);
            //Frame 9
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.EndTurn);
            //Frame 10 (strike + strike + bonus throw)
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(2, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8, 22 }, AfterStrikeAction.EndGame);

            Assert.AreEqual("X", scoreDisplayWrappers[9].FirstScore.text);
            Assert.AreEqual("X", scoreDisplayWrappers[9].SecondScore.text);
            Assert.AreEqual("2", scoreDisplayWrappers[9].ThirdScore.text);
            Assert.AreEqual("22", scoreDisplayWrappers[9].FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[9].ScoreDisplayStatus);
        }

        [Test]
        public void T17_LastRound_Turkey_ThirdScoreShouldBePopulated()
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10");
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, GetEmptyTextObject().GetComponent<Text>());

            //Frame 1
            sut.UpdatFrameScores(9, new List<int> { }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9 }, AfterStrikeAction.EndTurn);
            //Frame 2
            sut.UpdatFrameScores(9, new List<int> { 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 3 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9 }, AfterStrikeAction.EndTurn);
            //Frame 4
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(2, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 5 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9 }, AfterStrikeAction.EndTurn);
            //Frame 6 (spare)
            sut.UpdatFrameScores(7, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(3, new List<int> { 9, 9, 17, 9, 17 }, AfterStrikeAction.EndTurn);
            //Frame 7 (strike)
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.EndTurn);
            //Frame 8
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.EndTurn);
            //Frame 9
            sut.UpdatFrameScores(8, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8 }, AfterStrikeAction.Tidy);
            sut.UpdatFrameScores(0, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.EndTurn);
            //Frame 10 (turkey)
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8 }, AfterStrikeAction.Reset);
            sut.UpdatFrameScores(10, new List<int> { 9, 9, 17, 9, 17, 20, 18, 8, 8, 30 }, AfterStrikeAction.EndGame);

            Assert.AreEqual("X", scoreDisplayWrappers[9].FirstScore.text);
            Assert.AreEqual("X", scoreDisplayWrappers[9].SecondScore.text);
            Assert.AreEqual("X", scoreDisplayWrappers[9].ThirdScore.text);
            Assert.AreEqual("30", scoreDisplayWrappers[9].FrameScore.text);
            Assert.AreEqual(ScoreDisplayStatus.Completed, scoreDisplayWrappers[9].ScoreDisplayStatus);
        }

        [TestCase("9", 2, 3, 4)]
        [TestCase("11", 10, 0, 1)]
        [TestCase("0")]
        public void T18_WhenUpdateScore_TotalScoreIsUpdated(string expectedTotalScore, params int[] frameScores)
        {
            List<ScoreDisplayWrapper> scoreDisplayWrappers = GetScoreDisplayWrappers("F1");
            Text finalScore = GetEmptyTextObject().GetComponent<Text>();
            ScoreDisplayResolver sut = new ScoreDisplayResolver(scoreDisplayWrappers, finalScore);

            sut.UpdatFrameScores(2, frameScores.ToList(), AfterStrikeAction.EndGame);

            Assert.AreEqual(expectedTotalScore, finalScore.text);
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

            GameObject text = GetEmptyTextObject();
            text.transform.SetParent(one.transform);
            GameObject textTwo = GetEmptyTextObject();
            textTwo.transform.SetParent(two.transform);
            GameObject textThree = GetEmptyTextObject();
            textThree.transform.SetParent(three.transform);
            GameObject textSum = GetEmptyTextObject();
            textSum.transform.SetParent(sum.transform);
        }

        private static GameObject GetEmptyTextObject()
        {
            GameObject text = new GameObject("text");
            text.AddComponent(typeof(Text));
            return text;
        }
    }
}
