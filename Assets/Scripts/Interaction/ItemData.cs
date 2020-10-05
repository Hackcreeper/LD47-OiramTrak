using UnityEngine;

namespace Interaction
{
    public class ItemData : MonoBehaviour
    {
        public Sprite turbo;
        public Sprite canon;
        public AudioClip canonAudio;
        public GameObject bulletPrefab;
        public Sprite trap;
        public GameObject trapPrefab;
        public AudioClip trapAudio;
        public Sprite mask;
        
        private void Awake()
        {
            DiContainer.Instance.Register("itemData", this);
        }
    }
}