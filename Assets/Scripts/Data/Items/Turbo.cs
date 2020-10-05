using Interaction;
using Interaction.Cars;
using UnityEngine;

namespace Data.Items
{
    public class Turbo : Item
    {
        private bool active;
        private float activeTimer = 3f;

        public override Sprite GetIcon() => DiContainer.Instance.GetByName<ItemData>("itemData").turbo;
        
        public override void Activate(Car car)
        {
            if (active)
            {
                return;
            }

            active = true;
            car.forwardAcceleration *= 1.3f;
            car.reverseAcceleration *= 1.3f;
        }

        public override void Collect(Car car)
        {
            
        }

        public override void OnTick(Car car)
        {
            if (!active)
            {
                return;
            }

            activeTimer -= Time.deltaTime;
            
            if (activeTimer > 0)
            {
                return;
            }
            
            car.forwardAcceleration /= 1.3f;
            car.reverseAcceleration /= 1.3f;
            
            active = false;
            
            car.ClearItem();
        }
    }
}