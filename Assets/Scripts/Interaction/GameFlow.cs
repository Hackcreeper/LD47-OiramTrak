using System.Collections.Generic;
using Data;
using Interaction.Cars;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction
{
    public class GameFlow : MonoBehaviour
    {
        public GameObject carPrefab;
        public CameraSizes[] sizesTwoPlayers;
        public CameraSizes[] sizesThreePlayers;
        public CameraSizes[] sizesFourPlayers;
        public GameObject waypointPrefab;
        public LayerMask layerMaskPlayer1;
        public LayerMask layerMaskPlayer2;
        public LayerMask layerMaskPlayer3;
        public LayerMask layerMaskPlayer4;

        private Dictionary<int, PlayerInfo> _players;
        private GameObject[] _carSpawners;

        private void Start()
        {
            InitPlayers();
            SpawnCars();
        }

        private void SpawnCars()
        {
            _carSpawners = GameObject.FindGameObjectsWithTag("CarSpawner");

            var total = _players.Count;

            foreach (var player in _players)
            {
                var id = player.Key;

                var spawner = _carSpawners[player.Key];

                var car = Instantiate(carPrefab);
                car.transform.position = spawner.transform.position;
                car.layer = LayerMask.NameToLayer("Player" + (id + 1) + "_Car");
                var localCar = car.GetComponent<LocalCar>();
                var waypoints = SpawnWaypoints();
                localCar.Init(player.Value, waypoints);
                localCar.SetName(id + 1);

                switch (id)
                {
                    case 0:
                        localCar.mainCamera.cullingMask = layerMaskPlayer1;
                        break;

                    case 1:
                        localCar.mainCamera.cullingMask = layerMaskPlayer2;
                        break;
                    
                    case 2:
                        localCar.mainCamera.cullingMask = layerMaskPlayer3;
                        break;
                    
                    case 3:
                        localCar.mainCamera.cullingMask = layerMaskPlayer4;
                        break;
                }
                
                foreach (var waypoint in waypoints)
                {
                    waypoint.layer = LayerMask.NameToLayer("Player" + (id+1));
                }
                
                if (total == 2)
                {
                    SetCamera(localCar.mainCamera, sizesTwoPlayers[id]);
                    continue;
                }

                if (total == 3)
                {
                    SetCamera(localCar.mainCamera, sizesThreePlayers[id]);
                    continue;
                }

                if (total == 4)
                {
                    SetCamera(localCar.mainCamera, sizesFourPlayers[id]);
                }
            }
        }

        private GameObject[] SpawnWaypoints()
        {
            var waypoints = Instantiate(waypointPrefab);
            var points = new GameObject[waypoints.transform.childCount];
            for (var i = 0; i < waypoints.transform.childCount; i++)
            {
                points[i] = waypoints.transform.GetChild(i).gameObject;
            }

            return points;
        }

        private static void SetCamera(Camera cam, CameraSizes sizes)
        {
            cam.rect = new Rect(
                sizes.x,
                sizes.y,
                sizes.w,
                sizes.h
            );
        }

        private void InitPlayers()
        {
            var players = DiContainer.Instance.GetByName<Dictionary<int, PlayerInfo>>("players");

            if (players == null)
            {
                Debug.Log("You sneaky boi! Just started the game in editor right?! RIGHT?!");

                players = new Dictionary<int, PlayerInfo>
                {
                    {
                        1, new PlayerInfo(
                            Keyboard.current.device,
                            ControlType.Keyboard
                        )
                    }
                };
            }

            _players = players;
        }
    }
}