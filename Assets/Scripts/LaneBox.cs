using System;
using Assets.Scripts;
using UnityEngine;

public class LaneBox : MonoBehaviour
{
    private PinSetter _pinSetter;

    void Start()
    {
        _pinSetter = FindObjectOfType<PinSetter>();

        ValidateData();
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Ball>() != null)
        {
            _pinSetter.ShouldBallBeReset = true;
            _pinSetter.StandingPinCountDisplayer.color = Color.red;
        }        
    }

    private void ValidateData()
    {
        if (_pinSetter == null) throw new ArgumentNullException(nameof(_pinSetter));
    }
}
