using Interaction.Cars;
using UnityEngine;

namespace Interaction
{
    public class CarSphere : MonoBehaviour
    {
        private LocalCar _car;

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
                return;
            }
        }

        public void RegisterPlayer(LocalCar car)
        {
            _car = car;
        }
    }
}