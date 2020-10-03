using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class Lobby : MonoBehaviour
    {
        public int maxPlayers;
        public GameObject playerBoxPrefab;
        public RectTransform playerBoxParent;
        public Button startGameButton;

        private readonly Dictionary<int, PlayerInfo> _players = new Dictionary<int, PlayerInfo>();
        private readonly List<LobbyPlayerBox> _playerBoxes = new List<LobbyPlayerBox>();

        private void Start()
        {
            for (var i = 0; i < maxPlayers; i++)
            {
                var box = Instantiate(playerBoxPrefab, playerBoxParent);
                var boxComponent = box.GetComponent<LobbyPlayerBox>();
                boxComponent.Create(i+1);
                
                _playerBoxes.Add(boxComponent);
            }
        }
        
        // ReSharper disable once UnusedMember.Global
        public void OnActivate(InputAction.CallbackContext context)
        {
            var deviceId = context.control.device.deviceId;
            if (_players.Any(player => player.Value.Device.deviceId == deviceId))
            {
                return;
            }
            
            if (_players.Count >= maxPlayers)
            {
                return;
            }
            
            Debug.Log("Adding player: " + deviceId + " (" + context.control.device.displayName + ")");
            
            var info = new PlayerInfo(
                context.control.device,
                context.control.device.layout == "Keyboard" ? ControlType.Keyboard : ControlType.Controller
            );

            for (var i = 0; i < maxPlayers; i++)
            {
                if (_players.ContainsKey(i))
                {
                    continue;
                }
                
                _players.Add(i, info);
                _playerBoxes[i].SetPlayer(info);

                startGameButton.interactable = true;
                startGameButton.GetComponent<Image>().color = Color.green;
                
                break;
            }
        }

        public void StartGame()
        {
            DiContainer.Instance.Register("players", _players);
            SceneManager.LoadScene("Game");
        }
    }
}
