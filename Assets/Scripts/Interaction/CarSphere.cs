using Interaction.Cars;
using UnityEngine;

namespace Interaction
{
    public class CarSphere : MonoBehaviour
    {
        private Car _car;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Checkpoint"))
            {
                _car.CheckedWaypoint();
                return;
            }
            
            if (other.CompareTag("Goal"))
            {
                _car.NextRound();
            }
        }

        public void RegisterPlayer(Car car)
        {
            _car = car;
        }
    }
}