using System.Collections;
using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Mappers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class PinSetter : MonoBehaviour
    {
        [Range(0f, 100f)]
        public float PinRaiseOffset = 0.5f;
        public GameObject BowlingPinLayout;
        public Text StandingPinCountDisplayer;

        private const int PinSettleDownTimeInSeconds = 10;

        private readonly ActionMaster _actionMaster = new ActionMaster();
        private readonly PinBehaviorManager _pinBehaviorManager = new PinBehaviorManager();
        private Ball _ball;
        private Animator _animator;
        private GameObject _pinSetCopy;
        private bool _isBallWithinBounds;
        private bool _isPinSettleDownInProgress;
        private int _lastSettledPinsCount = ActionMaster.MaxPinsCount;

        void Start()
        {
            _ball = FindObjectOfType<Ball>();
            _animator = GetComponent<Animator>();
            _pinSetCopy = Instantiate(BowlingPinLayout, BowlingPinLayout.transform.position, Quaternion.identity);

            if (BowlingPinLayout == null) Debug.LogError("BowlingPinLayout not provided to PinSetter.");
            if (StandingPinCountDisplayer == null) Debug.LogWarning("StandingPinCountDisplayer not set.");
            if (_ball == null) Debug.LogError("Ball not found on the scene.");
        }

        void Update()
        {
            if (_isBallWithinBounds)
            {
                StandingPinCountDisplayer.text = GetStadingPinsCount().ToString();
                if (_isPinSettleDownInProgress) return;

                _isPinSettleDownInProgress = true;
                StartCoroutine(WaitForPinsToSettleDown());
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.GetComponent<Ball>() != null)
            {
                _isBallWithinBounds = true;
                StandingPinCountDisplayer.color = Color.red;
            }
        }

        public int GetStadingPinsCount()
        {
            int standingPinsCount = 0;
            foreach (Transform pinTransform in _pinSetCopy.transform)
            {
                Pin pinComponent = pinTransform.GetComponent<Pin>();
                if (pinComponent == null) continue;

                if (pinComponent.IsStanding()) standingPinsCount++;
            }
            return standingPinsCount;
        }

        public void LowerPins()
        {
            _pinBehaviorManager.HandlePinBehavior(_pinSetCopy.GetComponentsInChildren<Pin>(), PinBehavior.Lower, PinRaiseOffset);
        }

        public void RaisePins()
        {
            _pinBehaviorManager.HandlePinBehavior(_pinSetCopy.GetComponentsInChildren<Pin>(), PinBehavior.Raise, PinRaiseOffset);
        }

        public void RenewPins()
        {
            UpdatePinDisplayer(GetStadingPinsCount(), Color.red);
            RecreatePinSet();
        }

        private IEnumerator WaitForPinsToSettleDown()
        {
            yield return new WaitForSeconds(PinSettleDownTimeInSeconds);

            int standingPinsCount = GetStadingPinsCount();
            UpdatePinDisplayer(standingPinsCount, Color.green);
            PerformAfterStrikeAction(standingPinsCount);
            yield return null;

            PrepareForTheNextScore();
        }


        private void UpdatePinDisplayer(int standingPinsCount, Color color)
        {
            StandingPinCountDisplayer.color = color;
            StandingPinCountDisplayer.text = standingPinsCount.ToString();
        }

        private void RecreatePinSet()
        {
            Destroy(_pinSetCopy);
            _pinSetCopy = Instantiate(BowlingPinLayout, BowlingPinLayout.transform.position, Quaternion.identity);
        }

        private void PerformAfterStrikeAction(int standingPinsCount)
        {
            AfterStrikeAction afterStrikeAction = _actionMaster.Bowl(_lastSettledPinsCount - standingPinsCount);
            _lastSettledPinsCount = standingPinsCount;

            _animator.SetTrigger(AfterStrikeActionToAnimationMapper.Map(afterStrikeAction).TriggerName);
            bool lastSettledPinsCountShouldBeChanged = afterStrikeAction == AfterStrikeAction.EndTurn || afterStrikeAction == AfterStrikeAction.Reset || afterStrikeAction == AfterStrikeAction.EndGame;
            if (lastSettledPinsCountShouldBeChanged) _lastSettledPinsCount = ActionMaster.MaxPinsCount;
        }

        private void PrepareForTheNextScore()
        {
            _isBallWithinBounds = false;
            _isPinSettleDownInProgress = false;
            _ball.Reset();
        }
    }
}
