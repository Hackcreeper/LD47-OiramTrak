using UnityEngine;

namespace Interaction
{
    public class ItemData : MonoBehaviour
    {
        public Sprite turbo;
        public Sprite canon;
        public GameObject bulletPrefab;
        public Sprite trap;
        public GameObject trapPrefab;
        public Sprite mask;
        
        private void Awake()
        {
            DiContainer.Instance.Register("itemData", this);
        }
    }
}