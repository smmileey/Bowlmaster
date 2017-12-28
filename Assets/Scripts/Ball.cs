using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource), typeof(MeshRenderer))]
public class Ball : MonoBehaviour
{
    public Vector3 Velocity = new Vector3(0, 0, 0);

    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private MeshRenderer _meshRenderer;

    public bool IsLaunched { get; set; }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _meshRenderer = GetComponent<MeshRenderer>();

        SetInitialValues();
    }

    public void Move(float shift)
    {
        if(IsLaunched) return;

        float allowedWidthShift = Specification.FloorWidth / 2 - _meshRenderer.bounds.size.x / 2;
        float constrainedPositionX = Mathf.Clamp(transform.position.x + shift, -allowedWidthShift, allowedWidthShift);
        transform.position = new Vector3(constrainedPositionX, transform.position.y, transform.position.z);
    }

    public void Launch(Vector3 velocity)
    {
        if (IsLaunched) return;

        _rigidbody.useGravity = true;
        _rigidbody.velocity = velocity;
        _audioSource.Play();
        IsLaunched = true;
    }

    private void SetInitialValues()
    {
        _rigidbody.useGravity = false;
    }
}
