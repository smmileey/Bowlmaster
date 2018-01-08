using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(PinCounter))]
    public class LaneBox : MonoBehaviour
    {
        private PinCounter _pinCounter;

        void Start()
        {
            _pinCounter = FindObjectOfType<PinCounter>();
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.GetComponent<Ball>() != null) _pinCounter.UpdateScore = true;      
        }
    }
}
