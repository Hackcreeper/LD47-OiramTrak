using System;
using System.Linq;
using Data;
using TMPro;
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
        public LayerMask trackMask;
        public float groundRayLength = .5f;
        public Transform groundRaySource;
        public Transform[] trackRaySources;
        public float dragGrounded = 3f;
        public TextMeshPro resetWarning;

        private PlayerInput _playerInput;
        private PlayerInfo _player;
        private Vector2 _movement;
        private float _speedAmount;
        private float _breakAmount;
        private float _speedInput;
        private float _turnInput;
        private bool _grounded;
        private float _warningTimer = 2f;
        private GameObject[] _waypoints;
        private int _nextWaypoint = 0;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            sphere.transform.parent = null;
            sphere.GetComponent<CarSphere>().RegisterPlayer(this);
        }

        public void Init(PlayerInfo player, GameObject[] waypoints)
        {
            _player = player;
            _playerInput.SwitchCurrentControlScheme(_player.Device);
            
            resetWarning.text = player.Type == ControlType.Keyboard
                ? "Press <b>[R]</b> to reset"
                : "Press <b>[Y]</b> to reset";

            _waypoints = waypoints;
            sphere.gameObject.layer = gameObject.layer;
            
            EnableActiveWaypoint();
        }

        private void EnableActiveWaypoint()
        {
            foreach (var waypoint in _waypoints)
            {
                waypoint.SetActive(false);
            }

            _waypoints[_nextWaypoint].SetActive(true);
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

        // ReSharper disable once UnusedMember.Global
        public void OnReset(InputAction.CallbackContext context)
        {
            if (!context.started)
            {
                return;
            }

            if (_nextWaypoint == 0)
            {
                // tbd: spawn before start
                return;
            }

            var position = _waypoints[_nextWaypoint - 1].transform.position;
            sphere.transform.position = position + new Vector3(0, 2, 0);
            transform.rotation = _waypoints[_nextWaypoint - 1].transform.rotation;
            transform.Rotate(0, 90, 0);
        }
        
        private void Update()
        {
            HandleInput();
            HandleWarning();
        }

        private void HandleWarning()
        {
            resetWarning.gameObject.SetActive(_warningTimer <= 0f);
        }

        private void HandleInput()
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
            CheckTrack();
            Move();
        }

        private void CheckTrack()
        {
            var hitTrack = trackRaySources.Any(source => Physics.Raycast(source.position, -transform.up, out _, groundRayLength * 10, trackMask));

            if (!hitTrack)
            {
                _warningTimer -= Time.fixedDeltaTime;
            }
            else
            {
                _warningTimer = 2f;
            }
        }

        private void Move()
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

        public void CheckedWaypoint()
        {
            _nextWaypoint++;
            EnableActiveWaypoint();
        }
    }
}