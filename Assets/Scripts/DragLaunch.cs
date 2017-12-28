using UnityEngine;

[RequireComponent(typeof(Ball))]
public class DragLaunch : MonoBehaviour
{
    private Ball _ball;
    private Vector3 _dragStartPosition;
    private float _dragStartTime;

    private const float XDragFactor = 0.01f;
    private const float MaxSpeed = 800f;

    void Start ()
	{
	    _ball = GetComponent<Ball>();
	}

    public void DragStart()
    {
        _dragStartTime = Time.time;
        _dragStartPosition = Input.mousePosition;
    }

    public void DragEnd()
    {
        float draggingTime = Time.time - _dragStartTime;
        Vector3 dragOnScreenDirection = Input.mousePosition - _dragStartPosition;
        Vector3 ballVelocity = new Vector3(dragOnScreenDirection.x * XDragFactor, 0f, dragOnScreenDirection.y) / draggingTime;
        ballVelocity.z = Mathf.Clamp(dragOnScreenDirection.y, 0f, MaxSpeed);

        _ball.Launch(ballVelocity);
    }
}
    