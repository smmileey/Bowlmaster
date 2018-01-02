using System.Collections;
using Assets.Scripts.Enums;
using Assets.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

public class PinSetter : MonoBehaviour
{
    [Range(0f, 100f)]
    public float PinRaiseOffset = 0.5f;
    public GameObject BowlingPinLayout;
    public Text StandingPinCountDisplayer;

    private const int PinSettleDownTimeInSeconds = 5;

    private Ball _ball;
    private PinBehaviorManager _pinBehaviorManager;
    private GameObject _pinSetCopy;
    private bool _isBallWithinBounds;
    private bool _isPinSettleDownInProgress;

    void Start()
    {
        _ball = FindObjectOfType<Ball>();
        _pinBehaviorManager = new PinBehaviorManager();
        _pinSetCopy = Instantiate(BowlingPinLayout, BowlingPinLayout.transform.position, Quaternion.identity);

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

    public int GetStadingPinsCount()
    {
        int standingPinsCount = 0;
        foreach (Transform pinTransform in _pinSetCopy.transform)
        {
            Pin pinComponent = pinTransform.GetComponent<Pin>();
            if (pinComponent == null) continue;

            if (pinComponent.IsStanding()) standingPinsCount++;
        }
        return standingPinsCount;
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
        Destroy(_pinSetCopy);
        _pinSetCopy = Instantiate(BowlingPinLayout, BowlingPinLayout.transform.position, Quaternion.identity);
        StandingPinCountDisplayer.text = GetStadingPinsCount().ToString();
        StandingPinCountDisplayer.color = Color.red;
    }

    private IEnumerator WaitForPinsToSettleDown()
    {
        yield return new WaitForSeconds(PinSettleDownTimeInSeconds);
        EstablishScore();
    }

    private void EstablishScore()
    {
        StandingPinCountDisplayer.color = Color.green;
        _isBallWithinBounds = false;
        _isPinSettleDownInProgress = false;
        _ball.Reset();
    }
}
