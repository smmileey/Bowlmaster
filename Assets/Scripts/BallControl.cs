using UnityEngine;

public class BallControl : MonoBehaviour
{
    public Ball Ball;

    private Vector3 _ballToCameraOffset;

	void Start ()
    {
		if (Ball == null) Debug.LogWarning("Ball not initialized!");

        _ballToCameraOffset = new Vector3(transform.position.x, transform.position.y, transform.position.z - Ball.transform.position.z);
    }
	
	void Update ()
	{
	    var zPosition = Mathf.Clamp(Ball.transform.position.z + _ballToCameraOffset.z, -float.MaxValue, Specification.MaxCameraZ);
	    transform.position = new Vector3(_ballToCameraOffset.x, _ballToCameraOffset.y, zPosition);
	}
}
