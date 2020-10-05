using UnityEngine;

namespace Interaction
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 eulers;

        private void Update()
        {
            transform.Rotate(eulers * Time.deltaTime);
        }
    }
}
