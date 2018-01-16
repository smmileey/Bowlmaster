using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PinCounter : MonoBehaviour
    {
        public Text StandingPinCountDisplayer;

        private const float TimeBeforeSetlementProcessIsOn = 2;
        private const float PinsFloatingMaxTimeThreshold = 3;
        private const float ResetPinDisplayAfterTime = 3;
        private int _lastSettledPinsCount = ActionMaster.MaxPinsCount;
        private GameManager _gameManager;
        private readonly PinSettlementManager _pinSettlementManager = new PinSettlementManager(PinsFloatingMaxTimeThreshold);

        private bool IsSettlementInProgress { get; set; }
        public bool UpdateScore { get; set; }

        void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();

            ValidateData();
            SetupTriggers();
        }

        void Update()
        {
            if (UpdateScore)
            {
                UpdatePinDisplayer(GetStadingPinsCount(), Color.red);
                if (IsSettlementInProgress) return;

                IsSettlementInProgress = true;
                StartCoroutine(WaitForPinsToSettleDown());
            }
        }

        public int GetStadingPinsCount()
        {
            GameObject pinSetCopy = PinSetter.PinSetCopy;
            if (pinSetCopy == null) throw new ArgumentNullException(nameof(pinSetCopy));

            int standingPinsCount = 0;
            foreach (Transform pinTransform in pinSetCopy.transform)
            {
                Pin pinComponent = pinTransform.GetComponent<Pin>();
                if (pinComponent == null) continue;

                if (pinComponent.IsStanding()) standingPinsCount++;
            }
            return standingPinsCount;
        }

        private void ValidateData()
        {
            if (_gameManager == null) throw new ArgumentNullException(nameof(_gameManager));
            if (StandingPinCountDisplayer == null) throw new ArgumentNullException(nameof(StandingPinCountDisplayer));


        }
        private void SetupTriggers()
        {
            _gameManager.PinDisplayReset += () => Invoke(nameof(ResetPinDisplay), ResetPinDisplayAfterTime);
        }

        private void UpdatePinDisplayer(int standingPinsCount, Color color)
        {
            StandingPinCountDisplayer.color = color;
            StandingPinCountDisplayer.text = standingPinsCount.ToString();
        }

        private IEnumerator WaitForPinsToSettleDown()
        {
            yield return new WaitForSeconds(TimeBeforeSetlementProcessIsOn);
            yield return new WaitUntil(ArePinsSettled());

            int standingPinsCount = GetStadingPinsCount();
            StabilizePins();
            UpdatePinDisplayer(standingPinsCount, Color.green);
            yield return null;

            int score = _lastSettledPinsCount - standingPinsCount;
            _lastSettledPinsCount = standingPinsCount;
            _gameManager.Score(score);
        }

        private Func<bool> ArePinsSettled()
        {
            return () => _pinSettlementManager.ArePinsSettled(GetStadingPinsCount());
        }

        private void StabilizePins()
        {
            UpdateScore = false;
            IsSettlementInProgress = false;
            EstablishRotation();
        }

        private void ResetPinDisplay()
        {
            _lastSettledPinsCount = ActionMaster.MaxPinsCount;
            UpdatePinDisplayer(ActionMaster.MaxPinsCount, Color.black);
        }

        private void EstablishRotation()
        {
            foreach (Transform pinTransform in PinSetter.PinSetCopy.transform)
            {
                if (pinTransform.GetComponent<Pin>().IsStanding()) pinTransform.rotation = Quaternion.Euler(270, 0, 0);
            }
        }
    }
}
