using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Interaction.Cars
{
    public class BotCar : Car
    {
        private NavMeshPath _path;
        private float _inactiveTimer = 0f;
        private bool _waitForGravity = false;
        private void Awake()
        {
            _path = new NavMeshPath();;
        }

        protected override void Start()
        {
            base.Start();
            
            CalculatePath();
        }
        
        protected override void Update()
        {
            if (_inactiveTimer > 0f)
            {
                _inactiveTimer -= Time.deltaTime;
                base.Update();
                return;
            }

            if (_waitForGravity)
            {
                base.Update();
                return;
            }
            
            if (blocked || Finished)
            {
                base.Update();
                return;
            }
            
            Movement.y = 1f;
            
            CalculatePath();
            
            if (_path.corners.Length == 0)
            {
                StartCoroutine(WaitForIt());
                _waitForGravity = true;
                return;
            }
            
            var corner = _path.corners.Skip(1).First();

            var oldRotation = transform.rotation;

            var idx = 11;
            var a = _path.corners.Skip(1).Take(5).Sum(c =>
            {
                idx--;
                transform.LookAt(c);

                var newRotation = transform.rotation.eulerAngles.y;
                transform.rotation = oldRotation;

                var difference = oldRotation.eulerAngles.y - newRotation;
                
                if (Mathf.Abs(difference) < 25)
                {
                    return 0;
                }
                
                return 0.1f * idx;
            });
            
            Movement.y = Mathf.Clamp(Movement.y - a, 0.5f, 1f);
            
            transform.LookAt(corner);

            var finalNewRotation = transform.rotation.eulerAngles.y;
            transform.rotation = oldRotation;

            var finalDifference = oldRotation.eulerAngles.y - finalNewRotation;
            
            if ((finalDifference > 0 && finalDifference < 180) || finalDifference < -180)
            {
                Movement.x = -2;
            }
            else if (finalDifference < 0 || finalDifference > 180)
            {
                Movement.x = 2;
            }
            else
            {
                Movement.x = 0;
            }

            DebugDraw();
            
            base.Update();
        }

        private void DebugDraw()
        {
            for (int i = 0; i < _path.corners.Length - 1; i++)
            {
                Debug.DrawLine(_path.corners[i], _path.corners[i + 1], new Color(i * 0.1f, 1 - i * 0.1f, i * 0.1f));
            }
        }

        private void CalculatePath()
        {
            var nextWaypoint = NextWaypoint+1;
            if (nextWaypoint >= Waypoints.Length)
            {
                nextWaypoint = 0;
            }
            
            NavMesh.CalculatePath(
                transform.position, 
                Waypoints[nextWaypoint].transform.position, 
                NavMesh.AllAreas, 
                _path
            );
        }

        protected override void ResetPosition()
        {
            base.ResetPosition();

            _inactiveTimer = 2f;
        }

        private IEnumerator WaitForIt()
        {
            // Movement.y = 0;
            // Movement.x = 0;
            
            for (var i = 0; i < 2; i++)
            {
                CalculatePath();
                if (_path.corners.Length > 0)
                {
                    _waitForGravity = false;
                    yield break;
                }

                yield return new WaitForSeconds(1f);
            }
            
            ResetPosition();
            _waitForGravity = false;
        }

        protected override IEnumerator HandleMask()
        {
            forwardAcceleration *= 0.7f;
            yield return new WaitForSeconds(5f);
            forwardAcceleration /= 0.7f;
        }
    }
}