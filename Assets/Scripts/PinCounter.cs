using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PinCounter : MonoBehaviour
    {
        public Text StandingPinCountDisplayer;

        private const int PinSettleDownTimeInSeconds = 10;
        private int _lastSettledPinsCount = ActionMaster.MaxPinsCount;
        private GameManager _gameManager;

        private bool IsSettlementInProgress { get; set; }

        public bool UpdateScore { get; set; }

        void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();

            Validate();
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

        private void Validate()
        {
            if (_gameManager == null) throw new ArgumentNullException(nameof(_gameManager));
            if (StandingPinCountDisplayer == null) throw new ArgumentNullException(nameof(StandingPinCountDisplayer));

            _gameManager.PinDisplayReset += () => Invoke(nameof(ResetPinDisplay), 3);
        }

        private void UpdatePinDisplayer(int standingPinsCount, Color color)
        {
            StandingPinCountDisplayer.color = color;
            StandingPinCountDisplayer.text = standingPinsCount.ToString();
        }

        private IEnumerator WaitForPinsToSettleDown()
        {
            yield return new WaitForSeconds(PinSettleDownTimeInSeconds);

            int standingPinsCount = GetStadingPinsCount();
            EstablishScore();
            UpdatePinDisplayer(standingPinsCount, Color.green);
            yield return null;

            int score = _lastSettledPinsCount - standingPinsCount;
            _lastSettledPinsCount = standingPinsCount;
            _gameManager.Score(score);
        }

        private void EstablishScore()
        {
            UpdateScore = false;
            IsSettlementInProgress = false;
        }

        private void ResetPinDisplay()
        {
            _lastSettledPinsCount = ActionMaster.MaxPinsCount;
            UpdatePinDisplayer(ActionMaster.MaxPinsCount, Color.black);
        }
    }
}
