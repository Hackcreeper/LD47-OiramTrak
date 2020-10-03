using Interaction.Cars;
using UnityEngine;

namespace Interaction
{
    public class CarSphere : MonoBehaviour
    {
        private LocalCar _car;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Checkpoint"))
            {
                return;
            }
            
            _car.CheckedWaypoint();
        }

        public void RegisterPlayer(LocalCar car)
        {
            _car = car;
        }
    }
}
