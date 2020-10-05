using System.Linq;
using Interaction;
using Interaction.Cars;
using UnityEngine;

namespace Data.Items
{
    public class Mask : Item
    {
        public override Sprite GetIcon()
        {
            return DiContainer.Instance.GetByName<ItemData>("itemData").mask;
        }

        public override void Activate(Car car)
        {
            DiContainer.Instance.GetByName<GameFlow>("Game")
                .Cars
                .Where(c => c != car)
                .ToList()
                .ForEach(c => c.EnableMask());
            
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