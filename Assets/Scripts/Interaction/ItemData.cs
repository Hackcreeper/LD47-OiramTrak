using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Interaction
{
    public class ItemData : MonoBehaviour
    {
        public Sprite turbo;
        public PostProcessProfile turboProfile;
        public Sprite canon;
        public GameObject bulletPrefab;
        
        private void Awake()
        {
            DiContainer.Instance.Register("itemData", this);
        }
    }
}