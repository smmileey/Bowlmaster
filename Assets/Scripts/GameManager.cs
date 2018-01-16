using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Enums;
using Assets.Scripts.Mappers;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private readonly List<int> _currentThrows = new List<int>();
        private PinSetter _pinSetter;
        private Ball _ball;
        private readonly ActionMaster _actionMaster = new ActionMaster();
        private readonly ScoreMaster _scoreMaster = new ScoreMaster();

        public delegate void SimpleNotification();
        public event SimpleNotification PinDisplayReset;

        void Start()
        {
            _pinSetter = FindObjectOfType<PinSetter>();
            _ball = FindObjectOfType<Ball>();

            Vaildate();
        }

        public void Score(int score)
        {
            _currentThrows.Add(score);
            AfterStrikeAction afterStrikeAction = _actionMaster.NextAction(_currentThrows);

            TriggerAnimation(afterStrikeAction);
            TriggerPinsResetIfNeccessary(afterStrikeAction);
            //talk to ScoreMaster
            _ball.Reset();
            string s = string.Join(",", _scoreMaster.GetFrameScores(_currentThrows).Select(n => n.ToString()).ToArray());
            Debug.Log(s);
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
