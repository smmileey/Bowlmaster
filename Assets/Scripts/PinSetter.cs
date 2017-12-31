using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PinSetter : MonoBehaviour
{
    public GameObject BowlingPinLayout;
    public Text StandingPinCountDisplayer;

    private Ball _ball;
    private bool _isBallWithinBounds;
    private bool _isPinSettleDownInProgress;
    private const int PinSettleDownTimeInSeconds = 5;

    void Start()
    {
        _ball = FindObjectOfType<Ball>();
        if (BowlingPinLayout == null) Debug.LogError("BowlingPinLayout not provided to PinSetter.");
        if (StandingPinCountDisplayer == null) Debug.LogWarning("StandingPinCountDisplayer not set.");
        if (_ball == null) Debug.LogError("Ball not found on the scene.");
    }

    void Update()
    {
        if (_isBallWithinBounds)
        {
            StandingPinCountDisplayer.text = GetStadingPinsCount().ToString();
            if (_isPinSettleDownInProgress) return;

            _isPinSettleDownInProgress = true;
            StartCoroutine(WaitForPinsToSettleDown());
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<Ball>() != null)
        {
            _isBallWithinBounds = true;
            StandingPinCountDisplayer.color = Color.red;
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

    private IEnumerator WaitForPinsToSettleDown()
    {
        yield return new WaitForSeconds(PinSettleDownTimeInSeconds);
        EstablishScore();
    }

    private void EstablishScore()
    {
        //stabilize pins = calibrate physics
        StandingPinCountDisplayer.color = Color.green;
        _isBallWithinBounds = false;
        _isPinSettleDownInProgress = false;
        _ball.Reset();
    }
}
