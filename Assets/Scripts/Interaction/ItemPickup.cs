using UnityEngine;

namespace Interaction
{
    public class ItemPickup : MonoBehaviour
    {
        public Material normalMaterial;
        public Material inactiveMaterial;
        public MeshRenderer[] meshRenderers;

        private float _inactiveTimer = 0f;
        private bool _active = true;
            
        public void Taken()
        {
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material = inactiveMaterial;
            }

            _inactiveTimer = 5f;
            _active = false;
        }

        private void Update()
        {
            if (_active)
            {
                return;
            }
            
            if (_inactiveTimer > 0f)
            {
                _inactiveTimer -= Time.deltaTime;
                return;
            }

            _active = true;
            foreach (var meshRenderer in meshRenderers)
            {
                meshRenderer.material = normalMaterial;
            }
        }

        public bool IsActive() => _active;
    }
}
