using Interaction.Cars;
using UnityEngine;

namespace Interaction
{
    public class CarSphere : MonoBehaviour
    {
        public AudioSource trapSound;
        
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
                return;
            }

            if (other.CompareTag("ItemPickup"))
            {
                _car.GetRandomItem(other.GetComponent<ItemPickup>());
                return;
            }

            if (other.CompareTag("Bullet"))
            {
                if (other.GetComponent<Bullet>().Owner == _car)
                {
                    return;
                }
                
                trapSound.Play();
                
                Destroy(other.gameObject);
                _car.Hit();
                return;
            }

            if (other.CompareTag("Trap"))
            {
                if (_car.IsImmune())
                {
                    return;
                }

                trapSound.Play();

                Destroy(other.gameObject);
                _car.Hit();
                return;
            }
        }

        public void RegisterPlayer(Car car)
        {
            _car = car;
        }
    }
}