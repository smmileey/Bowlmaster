using Assets.Scripts.Consts;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody), typeof(AudioSource), typeof(MeshRenderer))]
    public class Ball : MonoBehaviour
    {
        public Vector3 Velocity = new Vector3(0, 0, 0);

        public float MaxBallSpeed = 800f;

        private Vector3 _startPosition;
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

        public void MoveHorizontally(float shift)
        {
            if (IsLaunched) return;

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

        public void Reset()
        {
            IsLaunched = false;
            _rigidbody.useGravity = false;
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.rotation = Quaternion.identity;
            transform.position = _startPosition;
        }

        private void SetInitialValues()
        {
            _startPosition = transform.position;
            _rigidbody.useGravity = false;
        }
    }
}
