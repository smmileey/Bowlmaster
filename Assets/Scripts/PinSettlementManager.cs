using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts
{
    public class PinSettlementManager : IPinsWobbling
    {
        private float _wobblingThreshold;

        private float _lastWobblingTime;
        private int _lastStandingPinsCount;

        /// <summary>
        /// Returns true if pins are wobbling
        /// </summary>
        /// <param name="wobblingThreshold">Threshold for determining pins wobbling in seconds.</param>
        public PinSettlementManager(float wobblingThreshold)
        {
            _wobblingThreshold = wobblingThreshold;
            _lastWobblingTime = float.MaxValue;
        }

        public void ChangeWobblingThreshold(int wobblingThreshold)
        {
            _wobblingThreshold = wobblingThreshold;
        }

        public bool ArePinsSettled(int standingPinsCount)
        {
            float currentTime = Time.realtimeSinceStartup;
            if (standingPinsCount != _lastStandingPinsCount)
            {
                _lastStandingPinsCount = standingPinsCount;
                _lastWobblingTime = Time.realtimeSinceStartup;
                return false;
            }

            bool pinsSettled = Mathf.Abs(currentTime - _lastWobblingTime) >= _wobblingThreshold;
            if (pinsSettled)
            {
                _lastStandingPinsCount = 0;
                _lastWobblingTime = float.MaxValue;
            }

            return pinsSettled;
        }
    }
}
