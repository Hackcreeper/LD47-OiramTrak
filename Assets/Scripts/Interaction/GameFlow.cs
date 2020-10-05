using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using Interaction.Cars;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Interaction
{
    public class GameFlow : MonoBehaviour
    {
        public bool IsFinished { get; private set; }

        public GameObject carPrefab;
        public GameObject botCarPrefab;
        public CameraSizes[] sizesTwoPlayers;
        public CameraSizes[] sizesThreePlayers;
        public CameraSizes[] sizesFourPlayers;
        public GameObject waypointPrefab;
        public LayerMask layerMaskPlayer1;
        public LayerMask layerMaskPlayer2;
        public LayerMask layerMaskPlayer3;
        public LayerMask layerMaskPlayer4;
        public TextMeshProUGUI roundCounter;
        public TextMeshProUGUI roundTitle;
        public GameObject leaderboardPlayerPrefab;
        public MeshRenderer lightsRenderer;
        public Material inactiveLight;
        public Material startMaterial;
        public Material whiteMaterial;
        public Material redLightMaterial;
        public Material yellowLightMaterial;
        public Material greenLightMaterial;
        public GameObject[] redLights;
        public GameObject[] yellowLights;
        public GameObject[] greenLights;
        public GameObject winScreen;
        public GameObject largeLeaderboardPlayerPrefab;
        public Transform largeLeaderboard;
        public VerticalLayoutGroup verticalLayoutGroup;
        public readonly List<Car> Cars = new List<Car>();

        private Dictionary<int, PlayerInfo> _players;
        private GameObject[] _carSpawners;
        private Leaderboard _leaderboard;

        private void Awake()
        {
            DiContainer.Instance.Register("Game", this);
        }

        private void Start()
        {
            _leaderboard = DiContainer.Instance.GetByName<Leaderboard>("Leaderboard");

            InitPlayers();
            SpawnCars();
            StartCoroutine(StartRound());
        }

        private IEnumerator StartRound()
        {
            roundCounter.text = "3";
            yield return new WaitForSeconds(1);

            roundCounter.text = "2";
            redLights.ToList().ForEach(l => l.SetActive(true));

            lightsRenderer.materials = new[]
            {
                startMaterial,
                inactiveLight,
                inactiveLight,
                redLightMaterial,
                whiteMaterial
            };

            yield return new WaitForSeconds(1);

            roundCounter.text = "1";

            redLights.ToList().ForEach(l => l.SetActive(false));
            yellowLights.ToList().ForEach(l => l.SetActive(true));

            lightsRenderer.materials = new[]
            {
                startMaterial,
                inactiveLight,
                yellowLightMaterial,
                inactiveLight,
                whiteMaterial
            };

            yield return new WaitForSeconds(1);

            roundCounter.text = "Go!";

            yellowLights.ToList().ForEach(l => l.SetActive(false));
            greenLights.ToList().ForEach(l => l.SetActive(true));

            lightsRenderer.materials = new[]
            {
                startMaterial,
                greenLightMaterial,
                inactiveLight,
                inactiveLight,
                whiteMaterial
            };

            Cars.ForEach(car => car.blocked = false);

            var alpha = roundCounter.color.a;
            while (alpha > 0f)
            {
                alpha = roundCounter.color.a - 0.8f * Time.deltaTime;

                roundCounter.color = new Color(roundCounter.color.r, roundCounter.color.g, roundCounter.color.b, alpha);
                roundTitle.color = new Color(roundTitle.color.r, roundTitle.color.g, roundTitle.color.b, alpha);

                yield return new WaitForEndOfFrame();
            }

            roundCounter.gameObject.SetActive(false);
        }

        private void SpawnCars()
        {
            _carSpawners = GameObject.FindGameObjectsWithTag("CarSpawner");

            var total = _players.Sum(player => player.Value.IsPlayer ? 1 : 0);

            foreach (var player in _players)
            {
                var id = player.Key;

                var spawner = _carSpawners[player.Key];

                var car = Instantiate(player.Value.IsPlayer ? carPrefab : botCarPrefab);
                car.transform.position = spawner.transform.position;

                car.layer = LayerMask.NameToLayer("Player" + (id + 1) + "_Car");

                var carComp = car.GetComponent<Car>();
                var waypoints = SpawnWaypoints();

                carComp.Init(player.Value, waypoints);
                carComp.SetName(id + 1);

                var entry = Instantiate(leaderboardPlayerPrefab);
                var component = entry.GetComponent<LeaderboardPlayer>();

                component.SetData(player.Key + 1, player.Key + 1, !player.Value.IsPlayer);
                _leaderboard.Add(component);

                carComp.SetLeaderboardEntry(component);

                Cars.Add(carComp);

                if (player.Value.IsPlayer)
                {
                    var localCar = car.GetComponent<LocalCar>();
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

                    switch (total)
                    {
                        case 2:
                            SetCamera(localCar.mainCamera, sizesTwoPlayers[id]);
                            break;
                        case 3:
                            SetCamera(localCar.mainCamera, sizesThreePlayers[id]);
                            break;
                        case 4:
                            SetCamera(localCar.mainCamera, sizesFourPlayers[id]);
                            break;
                    }
                }

                foreach (var waypoint in waypoints)
                {
                    waypoint.layer = LayerMask.NameToLayer("Player" + (id + 1));
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

                DiContainer.Instance.Register("rounds", 1);

                players = new Dictionary<int, PlayerInfo>
                {
                    {
                        0, new PlayerInfo(
                            Keyboard.current.device,
                            ControlType.Keyboard
                        )
                    },
                    {
                        1, new PlayerInfo(
                            null,
                            ControlType.Bot,
                            false
                        )
                    }
                };
            }

            _players = players;
        }

        public void CheckFinish()
        {
            if (!Cars.All(car => car.IsFinished()))
            {
                return;
            }

            IsFinished = true;
            winScreen.SetActive(true);

            // Create all leaderboard entries
            _leaderboard.GetEntries().ForEach(entry =>
            {
                var lead = Instantiate(largeLeaderboardPlayerPrefab, largeLeaderboard, false);
                var component = lead.GetComponent<LargeLeaderboardPlayer>();

                var seconds = Mathf.FloorToInt(entry.NeededTime);
                var minutes = Mathf.FloorToInt(seconds / 60f);
                seconds -= minutes * 60;

                component.positionLabel.text = entry.positionLabel.text;
                component.nameLabel.text = $"Player {entry.PlayerId} ({minutes:00}:{seconds:00})";
            });

            verticalLayoutGroup.CalculateLayoutInputVertical();
        }
    }
}