using System.Collections;
using System.Linq;
using Data;
using Data.Items;
using TMPro;
using UI;
using UnityEngine;

namespace Interaction.Cars
{
    public class Car : MonoBehaviour
    {
        public TextMeshPro nameTag;
        public Rigidbody sphere;
        public float forwardAcceleration = 7f;
        public float reverseAcceleration = 6f;
        public float turnStrength = 180f;
        public float gravityForce = 10f;
        public LayerMask groundMask;
        public LayerMask trackMask;
        public float groundRayLength = .5f;
        public Transform[] trackRaySources;
        public float dragGrounded = 3f;
        public bool blocked = true;
        public MeshRenderer carRenderer;

        protected PlayerInfo Player;
        protected Vector2 Movement;
        protected bool Finished;
        protected GameObject[] Waypoints;
        protected int NextWaypoint;
        protected Item CurrentItem;

        private float _speedInput;
        private float _turnInput;
        private bool _grounded;
        private int _round = 1;
        private LeaderboardPlayer _boardEntry;
        private float _time;

        protected virtual void Start()
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

        public virtual void Init(PlayerInfo player, GameObject[] waypoints)
        {
            Player = player;
            Waypoints = waypoints;

            sphere.gameObject.layer = gameObject.layer;
        }

        public void SetLeaderboardEntry(LeaderboardPlayer entry)
        {
            _boardEntry = entry;
        }

        protected void EnableActiveWaypoint()
        {
            DisableAllWaypoints();

            Waypoints[NextWaypoint].SetActive(true);
        }

        protected void DisableAllWaypoints()
        {
            foreach (var waypoint in Waypoints)
            {
                waypoint.SetActive(false);
            }
        }

        public void SetName(int id)
        {
            var playerName = Player.IsPlayer ? "Player" : "Bot";

            nameTag.text = $"{playerName} {id}";
        }

        protected virtual void ResetPosition()
        {
            if (blocked || Finished)
            {
                return;
            }

            if (NextWaypoint == 0)
            {
                // tbd: spawn before start
                return;
            }

            var position = Waypoints[NextWaypoint - 1].transform.position;
            sphere.transform.position = position + new Vector3(0, 2, 0);
            transform.rotation = Waypoints[NextWaypoint - 1].transform.rotation;
            transform.Rotate(0, 90, 0);

            StartCoroutine(Block());
        }

        protected virtual void Update()
        {
            if (!Finished)
            {
                _time += Time.deltaTime;
                CalculateScore();

                CurrentItem?.OnTick(this);
            }

            if (blocked || Finished)
            {
                MoveToSphere();
                return;
            }

            HandleInput();
            MoveToSphere();
        }

        private void CalculateScore()
        {
            _boardEntry.UpdateScore(GetLeaderboardPosition());
            _boardEntry.SetRound(_round);
            _boardEntry.SetTime(_time);
        }

        private void MoveToSphere()
        {
            transform.position = sphere.transform.position;
        }

        private void HandleInput()
        {
            _speedInput = 0f;

            if (Movement.y > 0)
            {
                _speedInput = Movement.y * forwardAcceleration * 1000f;
            }
            else if (Movement.y < 0)
            {
                _speedInput = Movement.y * reverseAcceleration * 1000f;
            }

            if (!_grounded)
            {
                return;
            }

            var direction = Movement.y < 0 ? -1 : 1;

            _turnInput = Movement.x * turnStrength * Time.deltaTime *
                         Mathf.Clamp(sphere.velocity.sqrMagnitude, -1, 1) * direction;
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles + new Vector3(0f, _turnInput, 0f)
            );
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

        protected virtual bool CheckTrack()
        {
            var hitTrack = trackRaySources.Any(source =>
                Physics.Raycast(source.position, -transform.up, out _, groundRayLength * 10, trackMask));

            if (!hitTrack && Mathf.Abs(_speedInput) > 0)
            {
                _speedInput /= 3f;
            }

            return hitTrack;
        }

        private void CheckWater()
        {
            var onWater = true;

            foreach (var source in trackRaySources)
            {
                RaycastHit hit;

                if (!Physics.Raycast(source.position, -transform.up, out hit, groundRayLength * 10, groundMask))
                {
                    continue;
                }

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground_Water"))
                {
                    continue;
                }

                onWater = false;
                break;
            }

            if (onWater)
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
            NextWaypoint++;

            if (NextWaypoint >= Waypoints.Length)
            {
                DisableAllWaypoints();
            }
            else
            {
                EnableActiveWaypoint();
            }
        }

        private IEnumerator Block()
        {
            blocked = true;
            yield return new WaitForSeconds(1.5f);
            blocked = false;
        }

        public int GetLeaderboardPosition()
        {
            var score = _round * 2500000;
            score += NextWaypoint * 100000;

            if (NextWaypoint < Waypoints.Length)
            {
                score += 1000 -
                         (int) Vector3.Distance(transform.position, Waypoints[NextWaypoint].transform.position);
            }

            return score;
        }

        public void NextRound()
        {
            if (NextWaypoint < Waypoints.Length)
            {
                return;
            }

            var maxRounds = DiContainer.Instance.GetByName<int>("rounds");

            if (_round == maxRounds)
            {
                Finished = true;
                Movement = Vector2.zero;
                DisableAllWaypoints();
                _boardEntry.Lock();

                DiContainer.Instance.GetByName<GameFlow>("Game").CheckFinish();

                return;
            }

            _round++;
            NextWaypoint = 0;
            EnableActiveWaypoint();
        }

        public bool IsFinished() => Finished;

        public void GetRandomItem(ItemPickup pickup)
        {
            if (!pickup.IsActive() || CurrentItem != null)
            {
                return;
            }

            pickup.Taken();
            CurrentItem = Item.GetRandomItem();

            OnNewItem();
        }

        public void ClearItem()
        {
            CurrentItem = null;
            
            OnRemoveItem();
        }

        protected virtual void OnNewItem()
        {
        }
        
        protected virtual void OnRemoveItem()
        {
        }
    }
}