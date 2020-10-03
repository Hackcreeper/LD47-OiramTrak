using UnityEngine;

namespace Interaction
{
    public class CameraFollow : MonoBehaviour
    {
        private Transform _target;
        
        private void Start()
        {
            _target = transform.parent;
            transform.parent = null;
        }

        private void Update()
        {
            var rotation = Quaternion.Euler(0, _target.rotation.eulerAngles.y, 0);
            
            transform.position = _target.position + (rotation * new Vector3(0f, 6.2f, -6f));
            transform.rotation = Quaternion.Euler(
                34,
                _target.rotation.eulerAngles.y,
                0
            );
        }
    }
}