using TMPro;
using UnityEngine;

namespace UI
{
    public class LeaderboardPlayer : MonoBehaviour
    {
        public static int FinishScore = 1000000;
        
        public TextMeshProUGUI positionLabel;
        public TextMeshProUGUI nameLabel;
        public int Score { get; private set; }
        public float NeededTime { get; private set; }

        public int PlayerId { get; private set; }

        private Leaderboard _leaderboard;
        private int _round;
        private int _position;

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
            PlayerId = playerId;

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
            nameLabel.text = $"Player {PlayerId}   ({_round}/3)";
        }

        public void UpdateScore(int score)
        {
            Score = score;
            if (_leaderboard)
            {
                _leaderboard.Sort();
            }
        }

        public void Lock()
        {
            positionLabel.color = Color.yellow;
            nameLabel.color = Color.yellow;

            Score += FinishScore;
            FinishScore -= 50000;
        }

        public void SetTime(float time)
        {
            NeededTime = time;
        }
    }
}