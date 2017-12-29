using UnityEngine;
using UnityEngine.UI;

public class PinSetter : MonoBehaviour
{
    public GameObject BowlingPinLayout;
    public Text PinCountDisplayer;

    private bool _isBallWithinBounds;

    void Start()
    {
        if (BowlingPinLayout == null) Debug.LogError("BowlingPinLayout not provided to PinSetter.");
        if (PinCountDisplayer == null) Debug.LogWarning("PinCountDisplayer not set.");
    }

    void Update()
    {
        if (_isBallWithinBounds) PinCountDisplayer.text = GetStadingPinsCount().ToString();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Ball>() != null)
        {
            _isBallWithinBounds = true;
            PinCountDisplayer.color = Color.red;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.GetComponent<Pin>() != null)
        {
            Destroy(collider.gameObject);
        }
    }

    public int GetStadingPinsCount()
    {
        int standingPinsCount = 0;
        foreach (Transform pinTransform in BowlingPinLayout.transform)
        {
            Pin pinComponent = pinTransform.GetComponent<Pin>();
            if (pinComponent == null) continue;

            if (pinComponent.IsStanding()) standingPinsCount++;
        }
        return standingPinsCount;
    }
}
