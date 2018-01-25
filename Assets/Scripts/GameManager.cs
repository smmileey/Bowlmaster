using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Wrappers;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private const string PlayerName = "RafaelloLollipop"; //TODO: Introduce score tracking for many players

        private readonly List<int> _currentThrows = new List<int>();
        private PinSetter _pinSetter;
        private Ball _ball;
        private ScoreDisplay _scoreDisplay;
        private readonly ActionMaster _actionMaster = new ActionMaster();
        private readonly ScoreMaster _scoreMaster = new ScoreMaster();

        public delegate void SimpleNotification();
        public event SimpleNotification PinDisplayReset;

        void Start()
        {
            _pinSetter = FindObjectOfType<PinSetter>();
            _ball = FindObjectOfType<Ball>();
            _scoreDisplay = FindObjectOfType<ScoreDisplay>();

            Vaildate();
        }

        public void Score(int score)
        {
            _currentThrows.Add(score);
            ScoreDisplayResolver currentScoreDisplay = _scoreDisplay.Get(PlayerName);
            AfterStrikeAction afterStrikeAction = _actionMaster.NextAction(_currentThrows);
            List<int> frameScores = _scoreMaster.GetFrameScores(_currentThrows);

            TriggerAnimation(afterStrikeAction);
            TriggerPinsResetIfNeccessary(afterStrikeAction);
            currentScoreDisplay.UpdatFrameScores(score, frameScores, afterStrikeAction);
            _ball.Reset();

            Debug.Log($"Score: {score}, frames:{string.Join(",", frameScores.Select(fs => fs.ToString()).ToArray())} , action: {afterStrikeAction}");
        }

        protected virtual void OnPinsReset()
        {
            SimpleNotification handler = PinDisplayReset;
            handler?.Invoke();
        }

        private void Vaildate()
        {
            if (_ball == null) throw new ArgumentNullException(nameof(_ball));
            if (_pinSetter == null) throw new ArgumentNullException(nameof(_pinSetter));
            if (_scoreDisplay == null) throw new ArgumentNullException(nameof(_scoreDisplay));
        }

        private void TriggerAnimation(AfterStrikeAction afterStrikeAction)
        {
            _pinSetter.AnimateSwiper(afterStrikeAction);
        }

        private void TriggerPinsResetIfNeccessary(AfterStrikeAction afterStrikeAction)
        {
            bool resetPinDisplayer = afterStrikeAction == AfterStrikeAction.EndTurn || afterStrikeAction == AfterStrikeAction.Reset || afterStrikeAction == AfterStrikeAction.EndGame;
            if (resetPinDisplayer) OnPinsReset();
        }
    }
}
