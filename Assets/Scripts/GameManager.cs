using System;
using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Mappers;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        private List<int> _currentThrows = new List<int>();
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
            PerformAfterStrikeAction();
            //talk to ScoreMaster
            _ball.Reset();
            //update game score
        }

        private void PerformAfterStrikeAction()
        {
            AfterStrikeAction afterStrikeAction = _actionMaster.Bowl(_currentThrows);
            var pinSetterAnimator = _pinSetter.GetComponent<Animator>();
            if (pinSetterAnimator == null) throw new ArgumentNullException(nameof(pinSetterAnimator));

            pinSetterAnimator.SetTrigger(AfterStrikeActionToAnimationMapper.Map(afterStrikeAction).TriggerName);

            bool resetPinDisplayer = afterStrikeAction == AfterStrikeAction.EndTurn || afterStrikeAction == AfterStrikeAction.Reset || afterStrikeAction == AfterStrikeAction.EndGame;
            if (resetPinDisplayer) OnPinsReset();
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
    }
}
