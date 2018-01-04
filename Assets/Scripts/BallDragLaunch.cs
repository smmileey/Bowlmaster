using UnityEngine;

[RequireComponent(typeof(Ball))]
public class BallDragLaunch : MonoBehaviour
{
    [Range(0f, 1f)]
    public float XDragFactor = 0.1f;

    private Ball _ball;
    private Vector3 _dragStartPosition;
    private float _dragStartTime;

    void Start()
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
        ballVelocity.z = Mathf.Clamp(dragOnScreenDirection.y, 0f, _ball.MaxBallSpeed);

        _ball.Launch(ballVelocity);
    }
}
