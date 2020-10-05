using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        public Animator cameraAnimator;
        public Animator blackoutAnimator;
        public GameObject pressStartText;

        // ReSharper disable once UnusedMember.Global
        public void OnStart(InputAction.CallbackContext context)
        {
            if (!context.started)
            {
                return;
            }
            
            StartCoroutine(AnimateLobby());
        }

        private IEnumerator AnimateLobby()
        {
            cameraAnimator.SetBool("starting", true);
            pressStartText.SetActive(false);
            
            yield return new WaitForSeconds(0.6f);
            
            blackoutAnimator.SetBool("blackout", true);
            
            yield return new WaitForSeconds(1.2f);

            SceneManager.LoadScene("Lobby");
        }
    }
}
