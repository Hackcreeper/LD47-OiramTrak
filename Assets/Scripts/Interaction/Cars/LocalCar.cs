using Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction.Cars
{
    public class LocalCar : Car
    {
        public Camera mainCamera;
        public Rigidbody sphere;
        public float forwardAcceleration = 7f;
        public float reverseAcceleration = 6f;
        public float turnStrength = 180f;
        public float gravityForce = 10f;
        public LayerMask groundMask;
        public float groundRayLength = .5f;
        public Transform groundRaySource;
        public float dragGrounded = 3f;

        private PlayerInput _playerInput;
        private PlayerInfo _player;
        private Vector2 _movement;
        private float _speedAmount;
        private float _breakAmount;
        private float _speedInput;
        private float _turnInput;
        private bool _grounded;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            sphere.transform.parent = null;
        }

        public void Init(PlayerInfo player)
        {
            _player = player;
            _playerInput.SwitchCurrentControlScheme(_player.Device);
        }

        // ReSharper disable once UnusedMember.Global
        public void OnMove(InputAction.CallbackContext context)
        {
            _movement = context.ReadValue<Vector2>();
        }

        // ReSharper disable once UnusedMember.Global
        public void OnBrake(InputAction.CallbackContext context)
        {
            _breakAmount = context.ReadValue<float>();
        }

        // ReSharper disable once UnusedMember.Global
        public void OnSpeed(InputAction.CallbackContext context)
        {
            _speedAmount = context.ReadValue<float>();
        }

        private void Update()
        {
            _speedInput = 0f;
            var forwardMovement = _movement.y;
            if (_player.Type == ControlType.Controller)
            {
                forwardMovement = _speedAmount - _breakAmount;
            }
            
            if (forwardMovement > 0)
            {
                _speedInput = forwardMovement * forwardAcceleration * 1000f;
            }
            else if (forwardMovement < 0)
            {
                _speedInput = forwardMovement * reverseAcceleration * 1000f;
            }

            if (_grounded)
            {
                _turnInput = _movement.x * turnStrength * Time.deltaTime * forwardMovement;
                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles + new Vector3(0f, _turnInput, 0f)
                );
            }

            transform.position = sphere.transform.position;
        }

        private void FixedUpdate()
        {
            _grounded = false;
            RaycastHit hit;

            if (Physics.Raycast(groundRaySource.position, -transform.up, out hit, groundRayLength, groundMask))
            {
                _grounded = true;
                transform.rotation = Quaternion.FromToRotation(
                    transform.up, hit.normal
                ) * transform.rotation;
            }

            if (_grounded)
            {
                sphere.drag = dragGrounded;

                if (Mathf.Abs(_speedInput) > 0)
                {
                    sphere.AddForce(transform.forward * _speedInput);
                }
            }
            else
            {
                sphere.drag = .1f;
                sphere.AddForce(Vector3.up * -gravityForce * 100f);
            }
        }
    }
}