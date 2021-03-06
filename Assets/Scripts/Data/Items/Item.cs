using Interaction.Cars;
using UnityEngine;

namespace Data.Items
{
    public abstract class Item
    {
        public abstract Sprite GetIcon();
        public abstract void Activate(Car car);

        public abstract AudioClip GetAudio();

        public abstract void Collect(Car car);

        public abstract void OnTick(Car car);
        
        public static Item GetRandomItem()
        {
            var possible = new Item[]
            {
                new Turbo(),
                new Canon(),
                new Trap(), 
                new Mask(),
            };

            return possible[Random.Range(0, possible.Length)];
        }
    }
}