using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using Assets.Scripts.Mappers;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Animator))]
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

        /// <summary>
        /// Called in animator.
        /// </summary>
        public void LowerPins()
        {
            _pinBehaviorManager.HandlePinBehavior(_pinSetCopy.GetComponentsInChildren<Pin>(), PinBehavior.Lower, PinRaiseOffset);
        }

        /// <summary>
        /// Called in animator.
        /// </summary>
        public void RaisePins()
        {
            _pinBehaviorManager.HandlePinBehavior(_pinSetCopy.GetComponentsInChildren<Pin>(), PinBehavior.Raise, PinRaiseOffset);
        }

        /// <summary>
        /// Called in animator.
        /// </summary>
        public void RenewPins()
        {
            RecreatePinSet();
        }

        public void AnimateSwiper(AfterStrikeAction afterStrikeAction)
        {
            var pinSetterAnimator = GetComponent<Animator>();
            pinSetterAnimator.SetTrigger(AfterStrikeActionToAnimationMapper.Map(afterStrikeAction).TriggerName);
        }

        private void RecreatePinSet()
        {
            Destroy(_pinSetCopy);
            _pinSetCopy = Instantiate(BowlingPinLayout, BowlingPinLayout.transform.position, Quaternion.identity);
        }

        private void Vaildate()
        {
            if (BowlingPinLayout == null) throw new ArgumentNullException(nameof(BowlingPinLayout));
        }
    }
}
