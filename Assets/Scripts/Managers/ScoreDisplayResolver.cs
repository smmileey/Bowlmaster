using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Wrappers;

namespace Assets.Scripts.Managers
{
    public class ScoreDisplayResolver
    {
        private readonly Queue<ScoreDisplayWrapper> _frameScoreDisplays = new Queue<ScoreDisplayWrapper>();
        private readonly List<ScoreDisplayWrapper> _scoreDisplaysCopy;
        private Queue<ScoreDisplayWrapper> _partialScoreDisplays;

        public ScoreDisplayResolver(List<ScoreDisplayWrapper> partialScoreDisplays)
        {
            if (partialScoreDisplays == null) throw new ArgumentNullException(nameof(partialScoreDisplays));

            _scoreDisplaysCopy = partialScoreDisplays;
            _partialScoreDisplays = new Queue<ScoreDisplayWrapper>(partialScoreDisplays);
        }

        /// <summary>
        /// This method is just for score displaying purpose. It has no validation, so any input that is not valid with basic model bowling game is unpredicted 
        /// </summary>
        /// <param name="pinsHitCount">Pins hit in last throw.</param>
        /// <param name="frameScores">Current frame scores (including current throw).</param>
        /// <param name="action">Last action performed (after current throw). </param>
        public void UpdateScore(int pinsHitCount, List<int> frameScores, AfterStrikeAction action)
        {
            if (_partialScoreDisplays.Count == 0) return;

            ScoreDisplayWrapper nextScoreDisplay = _partialScoreDisplays.Peek();
            if (nextScoreDisplay.FrameIndex == 10)
            {
                switch (action)
                {
                    case AfterStrikeAction.Tidy:
                        nextScoreDisplay.FirstScore.text = pinsHitCount.ToString();
                        nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.SecondRound;
                        break;
                    case AfterStrikeAction.Reset:
                        if (nextScoreDisplay.ScoreDisplayStatus == ScoreDisplayStatus.FirstRound)
                        {
                            nextScoreDisplay.FirstScore.text = IsStrike(pinsHitCount) ? "X" : pinsHitCount.ToString();
                            nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.SecondRound;
                        }
                        else if (nextScoreDisplay.ScoreDisplayStatus == ScoreDisplayStatus.SecondRound)
                        {
                            if (IsStrike(pinsHitCount))
                            {
                                nextScoreDisplay.SecondScore.text = "X";
                                nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.ThirdRound;
                            }
                            else if (IsSpare(pinsHitCount, nextScoreDisplay.FirstScore.text))
                            {
                                nextScoreDisplay.SecondScore.text = "/";
                                nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.ThirdRound;
                            }
                            else
                            {
                                nextScoreDisplay.SecondScore.text = pinsHitCount.ToString();
                                nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.Completed;
                                _frameScoreDisplays.Enqueue(_partialScoreDisplays.Dequeue());
                                Reload();
                            }
                        }
                        break;
                    case AfterStrikeAction.EndGame:
                        if (IsAdditionalRoundGranted(nextScoreDisplay)) nextScoreDisplay.ThirdScore.text = IsStrike(pinsHitCount) ? "X" : pinsHitCount.ToString();
                        nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.Completed;
                        _frameScoreDisplays.Enqueue(_partialScoreDisplays.Dequeue());
                        break;
                }
            }
            else
            {
                switch (action)
                {
                    case AfterStrikeAction.Tidy:
                        nextScoreDisplay.FirstScore.text = pinsHitCount.ToString();
                        nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.SecondRound;
                        break;
                    case AfterStrikeAction.EndTurn:
                        if (IsStrike(pinsHitCount))
                        {
                            nextScoreDisplay.FirstScore.text = "X";
                        }
                        else
                        {
                            nextScoreDisplay.SecondScore.text = IsSpare(pinsHitCount, nextScoreDisplay.FirstScore.text) ? "/" : pinsHitCount.ToString();
                        }
                        nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.Completed;
                        _frameScoreDisplays.Enqueue(_partialScoreDisplays.Dequeue());
                        break;
                    case AfterStrikeAction.Reset:
                        nextScoreDisplay.FirstScore.text = "X";
                        nextScoreDisplay.ScoreDisplayStatus = ScoreDisplayStatus.Completed;
                        _frameScoreDisplays.Enqueue(_partialScoreDisplays.Dequeue());
                        break;
                }
            }
            ProcessFrameCalculation(frameScores);
        }

        private bool IsAdditionalRoundGranted(ScoreDisplayWrapper nextScoreDisplay)
        {
            var firstScoreText = nextScoreDisplay.FirstScore.text;
            var secondScoreText = nextScoreDisplay.FirstScore.text;
            int firstScore, secondScore;
            return firstScoreText.Equals("X") || secondScoreText.Equals("/") 
                || int.TryParse(firstScoreText, out firstScore)
                   && int.TryParse(secondScoreText, out secondScore)
                   && firstScore + secondScore >= 10;
        }

        private bool IsStrike(int pinsHitCount)
        {
            return pinsHitCount == 10;
        }

        private bool IsSpare(int score, string firstFrameScore)
        {
            int firstFrame;
            return int.TryParse(firstFrameScore, out firstFrame) && score + firstFrame == 10;
        }

        private void Reload()
        {
            _partialScoreDisplays = new Queue<ScoreDisplayWrapper>(_scoreDisplaysCopy);
        }

        private void ProcessFrameCalculation(List<int> frameScores)
        {
            if (_frameScoreDisplays.Count == 0) return;

            var nextScoreDisplay = _frameScoreDisplays.Peek();
            if (frameScores.Count >= nextScoreDisplay.FrameIndex)
            {
                nextScoreDisplay.FrameScore.text = frameScores[nextScoreDisplay.FrameIndex - 1].ToString();
                _frameScoreDisplays.Dequeue();
            }
        }
    }
}
