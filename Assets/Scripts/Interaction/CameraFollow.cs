using Interaction.Cars;
using UnityEngine;

namespace Interaction
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;

        private void Update()
        {
            transform.localPosition = target.position + new Vector3(0, 6.2f, -6f);
            transform.rotation = Quaternion.Euler(
                38,
                0,
                0
            );
        }
    }
}