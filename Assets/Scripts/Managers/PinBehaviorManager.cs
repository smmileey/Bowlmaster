using System;
using Assets.Scripts.Enums;
using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class PinBehaviorManager : IPinBehavior
    {
        public void HandlePinBehavior(Pin[] pins, PinBehavior pinBehavior, float offsetMultiplier)
        {
            if (pins == null) throw new ArgumentNullException(nameof(pins));

            foreach (var pin in pins)
            {
                MovePin(pin, pinBehavior, offsetMultiplier);
            }
        }

        private void MovePin(Pin pin, PinBehavior pinBehavior, float offsetMultiplier)
        {
            switch (pinBehavior)
            {
                case PinBehavior.Lower:
                    pin.transform.Translate(Vector3.down * offsetMultiplier, Space.World);
                    pin.GetComponent<Rigidbody>().useGravity = true;
                    break;
                case PinBehavior.Raise:
                    if (pin.IsStanding())
                    {
                        pin.GetComponent<Rigidbody>().useGravity = false;
                        pin.transform.Translate(Vector3.up * offsetMultiplier, Space.World);
                    }
                    break;
                default:
                    Debug.LogError($"Behavior {pinBehavior} not handled in PinBehaviorManager.");
                    break;
            }
        }
    }
}
