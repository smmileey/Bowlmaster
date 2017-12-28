using UnityEngine;

public class Pin : MonoBehaviour
{
    public float StandingThreshold = 10f;

    public bool IsStanding()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        return !IsThresholdExceeded(eulerAngles.x) && !IsThresholdExceeded(eulerAngles.z);
    }

    private bool IsThresholdExceeded(float angleInDegrees)
    {
        float absoluteAngle = Mathf.Abs(angleInDegrees);
        float deviation = absoluteAngle <= 180
            ? absoluteAngle
            : 360 - absoluteAngle;

        return deviation >= StandingThreshold;
    }
}
