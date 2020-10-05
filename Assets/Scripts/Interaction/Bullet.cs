using System.Linq;
using Interaction.Cars;
using UnityEngine;
using UnityEngine.AI;

namespace Interaction
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 80f;
        public Car Owner { get; private set; }
        
        private Transform _target;
        private NavMeshPath _path;

        private void Awake()
        {
            _path = new NavMeshPath();;
        }
        
        public void SetTarget(Transform target, Car owner)
        {
            _target = target;
            Owner = owner;
        }

        private void Update()
        {
            if (_target == null)
            {
                return;
            }

            if (_path.corners.Length == 0)
            {
                CalculatePath();
                
                transform.LookAt(_target.position);
                transform.Translate(new Vector3(0, 0, speed * Time.deltaTime), Space.Self);
                
                return;
            }
            
            CalculatePath();
            var corner = _path.corners.Skip(1).First();
            
            transform.LookAt(corner);
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime), Space.Self);
            
            DebugDraw();
        }
        
        private void DebugDraw()
        {
            for (int i = 0; i < _path.corners.Length - 1; i++)
            {
                Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.blue);
            }
        }
        
        private void CalculatePath()
        {
            NavMesh.CalculatePath(
                transform.position, 
                _target.position, 
                NavMesh.AllAreas, 
                _path
            );
        }
    }
}