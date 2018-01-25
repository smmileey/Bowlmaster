using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Wrappers;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class ScoreDisplayResolver
    {
        private readonly Queue<ScoreDisplayWrapper> _frameScoreDisplays = new Queue<ScoreDisplayWrapper>();
        private readonly List<ScoreDisplayWrapper> _scoreDisplaysCopy;
        private readonly Text _finalScore;
        private Queue<ScoreDisplayWrapper> _partialScoreDisplays;

        public ScoreDisplayResolver(List<ScoreDisplayWrapper> partialScoreDisplays, Text finalScore)
        {
            if (partialScoreDisplays == null) throw new ArgumentNullException(nameof(partialScoreDisplays));
            if (finalScore == null) throw new ArgumentNullException(nameof(finalScore));

            _scoreDisplaysCopy = partialScoreDisplays;
            _finalScore = finalScore;
            _partialScoreDisplays = new Queue<ScoreDisplayWrapper>(partialScoreDisplays);

            _finalScore.text = "0";
        }

        /// <summary>
        /// This method is just for score displaying purpose. It has no validation, so any input that is not valid with basic model bowling game is unpredicted 
        /// </summary>
        /// <param name="pinsHitCount">Pins hit in last throw.</param>
        /// <param name="frameScores">Current frame scores (including current throw).</param>
        /// <param name="action">Last action performed (after current throw). </param>
        public void UpdatFrameScores(int pinsHitCount, List<int> frameScores, AfterStrikeAction action)
        {
            if (_partialScoreDisplays.Count == 0) return;

            ScoreDisplayWrapper nextScoreDisplay = _partialScoreDisplays.Peek();
            int? firstRoundScore = GetFirstRoundScore(nextScoreDisplay);

            if (nextScoreDisplay.FrameIndex == 10) { HandleLastFrameScore(nextScoreDisplay, pinsHitCount, firstRoundScore, action); }
            else { HandleBaseRoundsScore(nextScoreDisplay, pinsHitCount, firstRoundScore, action); }

            _finalScore.text = frameScores.Sum().ToString();
            ProcessFrameScoreCalculation(frameScores);
        }

        private void HandleBaseRoundsScore(ScoreDisplayWrapper nextScoreDisplay, int pinsHitCount, int? firstRoundScore, AfterStrikeAction action)
        {
            switch (action)
            {
                case AfterStrikeAction.Tidy:
                    nextScoreDisplay.FirstScore.text = GetScore(pinsHitCount, firstRoundScore);
                    nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.SecondRound;
                    break;
                case AfterStrikeAction.Reset:
                    nextScoreDisplay.FirstScore.text = GetScore(pinsHitCount, firstRoundScore);
                    nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.Completed;
                    _frameScoreDisplays.Enqueue(_partialScoreDisplays.Dequeue());
                    break;
                case AfterStrikeAction.EndTurn:
                    Text scoreText = pinsHitCount == 10 ? nextScoreDisplay.FirstScore : nextScoreDisplay.SecondScore;
                    scoreText.text = GetScore(pinsHitCount, firstRoundScore);
                    nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.Completed;
                    _frameScoreDisplays.Enqueue(_partialScoreDisplays.Dequeue());
                    break;
            }
        }

        private void HandleLastFrameScore(ScoreDisplayWrapper nextScoreDisplay, int pinsHitCount, int? firstRoundScore, AfterStrikeAction action)
        {
            switch (action)
            {
                case AfterStrikeAction.Tidy:
                    nextScoreDisplay.FirstScore.text = GetScore(pinsHitCount, firstRoundScore);
                    nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.SecondRound;
                    break;
                case AfterStrikeAction.Reset:
                    if (nextScoreDisplay.ScoreDisplayStatus == ScoreDisplayStatus.FirstRound)
                    {
                        nextScoreDisplay.FirstScore.text = GetScore(pinsHitCount, firstRoundScore);
                        nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.SecondRound;
                    }
                    else
                    {
                        nextScoreDisplay.SecondScore.text = GetScore(pinsHitCount, firstRoundScore);

                        if (IsAdditionalRoundGranted(nextScoreDisplay)) { nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.ThirdRound; }
                        else
                        {
                            nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.Completed;
                            _frameScoreDisplays.Enqueue(_partialScoreDisplays.Dequeue());
                            Reload();
                        }
                    }
                    break;
                case AfterStrikeAction.EndGame:
                    string score = GetScore(pinsHitCount, firstRoundScore);
                    if (IsAdditionalRoundGranted(nextScoreDisplay)) { nextScoreDisplay.ThirdScore.text = score; }
                    else { nextScoreDisplay.SecondScore.text = score; }
                    nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.Completed;
                    _frameScoreDisplays.Enqueue(_partialScoreDisplays.Dequeue());
                    break;
            }
        }

        private void ProcessFrameScoreCalculation(List<int> frameScores)
        {
            while (true)
            {
                if (_frameScoreDisplays.Count == 0) return;

                ScoreDisplayWrapper nextScoreDisplay = _frameScoreDisplays.Peek();
                if (frameScores.Count >= nextScoreDisplay.FrameIndex)
                {
                    nextScoreDisplay.FrameScore.text = frameScores[nextScoreDisplay.FrameIndex - 1].ToString();
                    _frameScoreDisplays.Dequeue();
                    continue;
                }
                break;
            }
        }

        private int? GetFirstRoundScore(ScoreDisplayWrapper nextScoreDisplay)
        {
            if (nextScoreDisplay.FirstScore.text.Equals("X")) return 10;
            if (string.IsNullOrEmpty(nextScoreDisplay.FirstScore.text)) return null;

            return int.Parse(nextScoreDisplay.FirstScore.text);
        }

        private string GetScore(int throwScore, int? firstRoundScore)
        {
            bool isStrike = throwScore == 10;
            if (!firstRoundScore.HasValue) return isStrike ? "X" : throwScore.ToString();
            if (throwScore == 10) return "X";

            return throwScore + firstRoundScore.Value == 10 ? "/" : throwScore.ToString();
        }

        private bool IsAdditionalRoundGranted(ScoreDisplayWrapper nextScoreDisplay)
        {
            var firstScoreText = nextScoreDisplay.FirstScore.text;
            var secondScoreText = nextScoreDisplay.SecondScore.text;
            int firstScore, secondScore;
            return firstScoreText.Equals("X") || secondScoreText.Equals("/")
                || int.TryParse(firstScoreText, out firstScore)
                   && int.TryParse(secondScoreText, out secondScore)
                   && firstScore + secondScore >= 10;
        }

        private void Reload()
        {
            _partialScoreDisplays = new Queue<ScoreDisplayWrapper>(_scoreDisplaysCopy);
            _finalScore.text = "0";
        }
    }
}
