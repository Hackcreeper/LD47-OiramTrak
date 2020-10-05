using UnityEngine;

namespace Interaction
{
    public class SinMover : MonoBehaviour
    {
        public float sinMultiplier = 1f;
        public float speedMultiplier = 1f;
        
        private float _basePosition;

        private void Start()
        {
            _basePosition = transform.position.y;
        }

        private void Update()
        {
            transform.position = new Vector3(
                transform.position.x,
                _basePosition + Mathf.Sin(Time.time * speedMultiplier) * sinMultiplier,
                transform.position.z
            );
        }
    }
}