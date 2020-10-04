using Interaction;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class WinScreen : MonoBehaviour
    {
        private void Awake()
        {
            DiContainer.Instance.Register("Win", this);
        }

        public void OnRestart(InputAction.CallbackContext context)
        {
            if (!context.started || !DiContainer.Instance.GetByName<GameFlow>("Game").IsFinished)
            {
                return;
            }

            SceneManager.LoadScene("Lobby");
        }
    }
}