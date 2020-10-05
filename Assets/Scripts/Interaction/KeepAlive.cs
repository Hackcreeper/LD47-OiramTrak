using UnityEngine;

namespace Interaction
{
    public class KeepAlive : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
