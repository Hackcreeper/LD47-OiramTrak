using System;
using System.Collections;
using System.Linq;
using Data;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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
        public LayerMask waterMask;
        public float groundRayLength = .5f;
        public Transform[] trackRaySources;
        public float dragGrounded = 3f;
        public TextMeshPro resetWarning;
        public bool blocked = true;
        public MeshRenderer carRenderer;

        private PlayerInput _playerInput;
        private PlayerInfo _player;
        private Vector2 _movement;
        private float _speedAmount;
        private float _breakAmount;
        private float _speedInput;
        private float _turnInput;
        private bool _grounded;
        private bool _onWater = false;
        private float _warningTimer = 2f;
        private GameObject[] _waypoints;
        private int _nextWaypoint = 0;
        private int _round = 1;
        private LeaderboardPlayer _leaderboardEntry;
        private bool _finished = false;
        private float _time = 0f;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            sphere.transform.parent = null;
            sphere.GetComponent<CarSphere>().RegisterPlayer(this);

            carRenderer.material.color = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );

            carRenderer.materials[2].color = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );
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

        public void SetLeaderboardEntry(LeaderboardPlayer entry)
        {
            _leaderboardEntry = entry;
        }

        private void EnableActiveWaypoint()
        {
            DisableAllWaypoints();

            _waypoints[_nextWaypoint].SetActive(true);
        }

        private void DisableAllWaypoints()
        {
            foreach (var waypoint in _waypoints)
            {
                waypoint.SetActive(false);
            }
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
            if (!context.started || blocked || _finished)
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

            StartCoroutine(Block());
        }

        // ReSharper disable once UnusedMember.Global
        public void OnRestart(InputAction.CallbackContext context)
        {
            DiContainer.Instance.GetByName<WinScreen>("Win").OnRestart(context);
        }

        private void Update()
        {
            if (!_finished)
            {
                _time += Time.deltaTime;
                CalculateScore();
            }

            if (blocked || _finished)
            {
                MoveToSphere();
                return;
            }

            HandleInput();
            HandleWarning();

            MoveToSphere();
        }

        private void CalculateScore()
        {
            _leaderboardEntry.UpdateScore(GetLeaderboardPosition());
            _leaderboardEntry.SetRound(_round);
            _leaderboardEntry.SetTime(_time);
        }

        private void MoveToSphere()
        {
            transform.position = sphere.transform.position;
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
                var direction = forwardMovement < 0 ? -1 : 1;

                _turnInput = _movement.x * turnStrength * Time.deltaTime *
                             Mathf.Clamp(sphere.velocity.sqrMagnitude, -1, 1) * direction;
                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles + new Vector3(0f, _turnInput, 0f)
                );
            }
        }

        private void FixedUpdate()
        {
            if (blocked)
            {
                sphere.velocity = Vector3.zero;
                sphere.angularVelocity = Vector3.zero;
                return;
            }

            CheckTrack();
            CheckWater();
            Move();
        }

        private void CheckTrack()
        {
            var hitTrack = trackRaySources.Any(source =>
                Physics.Raycast(source.position, -transform.up, out _, groundRayLength * 10, trackMask));

            if (!hitTrack)
            {
                _warningTimer -= Time.fixedDeltaTime;
                if (Mathf.Abs(_speedInput) > 0)
                {
                    _speedInput /= 3f;
                }
            }
            else
            {
                _warningTimer = 2f;
            }
        }

        private void CheckWater()
        {
            _onWater = true;
            
            foreach (var source in trackRaySources)
            {
                RaycastHit hit;

                if (Physics.Raycast(source.position, -transform.up, out hit, groundRayLength * 10, groundMask))
                {
                    Debug.Log(hit.collider.name);
                    if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Ground_Water"))
                    {
                        _onWater = false;
                        break;
                    }
                }
            }
            ;

            if (_onWater)
            {
                _speedInput *= 0f;
            }
        }

        private void Move()
        {
            _grounded = false;

            foreach (var source in trackRaySources)
            {
                RaycastHit hit;

                if (!Physics.Raycast(source.position, -transform.up, out hit, groundRayLength, groundMask))
                {
                    continue;
                }

                _grounded = true;
                transform.rotation = Quaternion.FromToRotation(
                    transform.up, hit.normal
                ) * transform.rotation;

                break;
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

            if (_nextWaypoint >= _waypoints.Length)
            {
                DisableAllWaypoints();
            }
            else
            {
                EnableActiveWaypoint();
            }
        }

        public IEnumerator Block()
        {
            blocked = true;
            yield return new WaitForSeconds(1.5f);
            blocked = false;
        }

        public int GetLeaderboardPosition()
        {
            var score = _round * 2500000;
            score += _nextWaypoint * 100000;

            if (_nextWaypoint < _waypoints.Length)
            {
                score += 1000 -
                         (int) Vector3.Distance(transform.position, _waypoints[_nextWaypoint].transform.position);
            }

            return score;
        }

        public void NextRound()
        {
            if (_nextWaypoint < _waypoints.Length)
            {
                return;
            }

            var maxRounds = DiContainer.Instance.GetByName<int>("rounds");

            if (_round == maxRounds)
            {
                _finished = true;
                _movement = Vector2.zero;
                DisableAllWaypoints();
                _leaderboardEntry.Lock();

                DiContainer.Instance.GetByName<GameFlow>("Game").CheckFinish();

                return;
            }

            _round++;
            _nextWaypoint = 0;
            EnableActiveWaypoint();
        }

        public bool IsFinished() => _finished;
    }
}