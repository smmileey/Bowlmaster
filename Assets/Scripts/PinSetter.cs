using UnityEngine;
using UnityEngine.UI;

public class PinSetter : MonoBehaviour
{
    public GameObject BowlingPinLayout;
    public Text PinCountDisplayer;

    void Start()
    {
        if (BowlingPinLayout == null) Debug.LogError("BowlingPinLayout not provided to PinSetter.");
        if (PinCountDisplayer == null) Debug.LogWarning("PinCountDisplayer not set.");
    }

    void Update()
    {
        PinCountDisplayer.text = GetStadingPinsCount().ToString();
    }

    public int GetStadingPinsCount()
    {
        int standingPinsCount = 0;
        foreach (Transform pinTransform in BowlingPinLayout.transform)
        {
            Pin pinComponent = pinTransform.gameObject.GetComponent<Pin>();
            if (pinComponent == null) continue;

            if (pinComponent.IsStanding()) standingPinsCount++;
        }
        return standingPinsCount;
    }
}
