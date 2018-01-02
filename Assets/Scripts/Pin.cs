using UnityEngine;

public class Pin : MonoBehaviour
{
    public float StandingThreshold = 80f;

    void Awake()
    {
        this.GetComponent<Rigidbody>().solverVelocityIterations = 20;
    }

    /// <summary>
    /// Here, we need to adjust eulerAngles.x, because the pin renderer (provided by modeller) is by default in horizontal position.
    /// Thus, it's design rotation against X axis is set to value of 270. 
    /// Now, all the deviation oscilates against that number, not the 270.
    /// </summary>
    /// <param name="angleInDegrees"></param>
    /// <returns></returns>
    public bool IsStanding()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        return !IsThresholdExceeded(270 - eulerAngles.x) && !IsThresholdExceeded(eulerAngles.z);
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
