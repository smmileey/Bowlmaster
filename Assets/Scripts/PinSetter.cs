using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts
{
    public class PinSetter : MonoBehaviour
    {
        [Range(0f, 100f)]
        public float PinRaiseOffset = 0.5f;
        public GameObject BowlingPinLayout;

        private readonly PinBehaviorManager _pinBehaviorManager = new PinBehaviorManager();
        private static GameObject _pinSetCopy;

        public static GameObject PinSetCopy => _pinSetCopy;

        void Start()
        {
            _pinSetCopy = Instantiate(BowlingPinLayout, BowlingPinLayout.transform.position, Quaternion.identity);

            Vaildate();
        }

        private void Vaildate()
        {
            if (BowlingPinLayout == null) throw new ArgumentNullException(nameof(BowlingPinLayout));
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
            RecreatePinSet();
        }

        private void RecreatePinSet()
        {
            Destroy(_pinSetCopy);
            _pinSetCopy = Instantiate(BowlingPinLayout, BowlingPinLayout.transform.position, Quaternion.identity);
        }
    }
}
