using Interaction.Cars;
using UnityEngine;

namespace Interaction
{
    public class CameraFollow : MonoBehaviour
    {
        private Car _car;

        private void Start()
        {
            _car = DiContainer.Instance.GetByName<Car>("main_car");

            Transform transform1;
            (transform1 = transform).SetParent(_car.transform);
            transform1.localPosition = new Vector3(0, 6.2f, -6f);
            transform1.rotation = Quaternion.Euler(
                38,
                0,
                0
            );
        }
    }
}