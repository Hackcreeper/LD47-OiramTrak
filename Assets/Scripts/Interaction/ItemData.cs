using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Interaction
{
    public class ItemData : MonoBehaviour
    {
        public Sprite turbo;
        public PostProcessProfile turboProfile;
        
        private void Awake()
        {
            DiContainer.Instance.Register("itemData", this);
        }
    }
}