using UnityEngine;

namespace UI
{
    public class RandomRotation : MonoBehaviour
    {
        public Vector3 from;
        public Vector3 to;

        private void Start()
        {
            transform.localRotation = Quaternion.Euler(
                Random.Range(from.x, to.x),
                Random.Range(from.y, to.y),
                Random.Range(from.z, to.z)
            );
        }
    }
}