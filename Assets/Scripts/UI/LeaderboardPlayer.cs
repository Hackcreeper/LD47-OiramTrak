using TMPro;
using UnityEngine;

namespace UI
{
    public class LeaderboardPlayer : MonoBehaviour
    {
        public TextMeshProUGUI positionLabel;
        public TextMeshProUGUI nameLabel;
        public int Score { get; private set; }

        private Leaderboard _leaderboard;
        private int _round;
        private int _position;
        private int _playerId;

        private void Start()
        {
            _leaderboard = DiContainer.Instance.GetByName<Leaderboard>("Leaderboard");
        }

        public void SetRound(int round)
        {
            _round = round;
            
            Render();
        }

        public void SetPosition(int position)
        {
            _position = position;
            
            Render();
        }

        public void SetData(int position, int playerId)
        {
            _position = position;
            _playerId = playerId;

            Render();
        }

        private void Render()
        {
            var positionString = _position.ToString();
            switch (_position)
            {
                case 1:
                    positionString += "st";
                    break;

                case 2:
                    positionString += "nd";
                    break;
                
                case 3:
                    positionString += "rd";
                    break;
                
                default:
                    positionString += "th";
                    break;
            }

            positionLabel.text = positionString;
            nameLabel.text = $"Player {_playerId}   ({_round}/3)";
        }

        public void UpdateScore(int score)
        {
            Score = score;
            _leaderboard.Sort();
        }
    }
}