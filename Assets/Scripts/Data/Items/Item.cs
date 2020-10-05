using Interaction.Cars;
using UnityEngine;

namespace Data.Items
{
    public abstract class Item
    {
        public abstract Sprite GetIcon();
        public abstract void Activate(Car car);

        public abstract void OnTick(Car car);
        
        public static Item GetRandomItem()
        {
            return new Turbo();
        }
    }
}