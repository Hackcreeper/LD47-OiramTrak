using Interaction;
using Interaction.Cars;
using UnityEngine;

namespace Data.Items
{
    public class Trap : Item
    {
        public override Sprite GetIcon()
        {
            return DiContainer.Instance.GetByName<ItemData>("itemData").trap;
        }

        public override void Activate(Car car)
        {
            car.Immune();
            
            var prefab = DiContainer.Instance.GetByName<ItemData>("itemData").trapPrefab;
            var trapGo = Object.Instantiate(prefab);
            trapGo.transform.position = car.transform.position;
            
            car.ClearItem();
        }

        public override void Collect(Car car)
        {
            
        }

        public override void OnTick(Car car)
        {
            
        }
    }
}