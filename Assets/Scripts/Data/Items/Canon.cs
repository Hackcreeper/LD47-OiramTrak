using System.Linq;
using Interaction;
using Interaction.Cars;
using UnityEngine;

namespace Data.Items
{
    public class Canon : Item
    {
        public override Sprite GetIcon()
        {
            return DiContainer.Instance.GetByName<ItemData>("itemData").canon;
        }

        public override void Activate(Car car)
        {
            var prefab = DiContainer.Instance.GetByName<ItemData>("itemData").bulletPrefab;
            var bulletGo = Object.Instantiate(prefab);
            bulletGo.transform.position = car.canonShot.transform.position;

            var cars = DiContainer.Instance.GetByName<GameFlow>("Game").Cars;
            var target = cars.Where(c => c.GetLeaderboardPosition() >= car.GetLeaderboardPosition())
                .Where(c => c != car)
                .OrderBy(c => c.GetLeaderboardPosition())
                .FirstOrDefault();

            if (target == null)
            {
                target = cars.Where(c => c != car)
                    .OrderBy(c => c.GetLeaderboardPosition())
                    .FirstOrDefault();
            }

            if (target == null)
            {
                car.canon.SetActive(false);
                car.ClearItem();           
            }

            bulletGo.GetComponent<Bullet>().SetTarget(target.transform, car);
            car.canon.SetActive(false);
            car.ClearItem();
        }

        public override void Collect(Car car)
        {
            car.canon.SetActive(true);
        }

        public override void OnTick(Car car)
        {
        }
    }
}