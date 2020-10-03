using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LobbyPlayerBox : MonoBehaviour
    {
        public TextMeshProUGUI title;
        public GameObject connectScreen;
        public GameObject playerInfoScreen;
        public TextMeshProUGUI deviceName;
        public Image deviceIcon;

        public Sprite iconController;
        public Sprite iconKeyboard;

        public void Create(int id)
        {
            title.text = $"Player {id}";
        }

        public void SetPlayer(PlayerInfo player)
        {
            deviceName.text = player.Device.displayName;
            deviceIcon.sprite = player.Type == ControlType.Controller ? iconController : iconKeyboard;
            
            connectScreen.SetActive(false);
            playerInfoScreen.SetActive(true);
        }
    }
}