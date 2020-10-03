using TMPro;
using UnityEngine;

namespace Interaction.Cars
{
    public class Car : MonoBehaviour
    {
        public TextMeshPro nameTag;

        public void SetName(int id)
        {
            nameTag.text = $"Player {id}";
        }
    }
}
