using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Leaderboard: MonoBehaviour
    {
        private readonly List<LeaderboardPlayer> _entries = new List<LeaderboardPlayer>();

        private void Awake()
        {
            DiContainer.Instance.Register("Leaderboard", this);
        }

        public void Add(LeaderboardPlayer player)
        {
            _entries.Add(player);
            
            player.transform.SetParent(transform, false);
        }

        public List<LeaderboardPlayer> GetEntries() => _entries;
        
        public void Sort()
        {
            _entries.Sort((r1, r2) => r2.Score.CompareTo(r1.Score));

            var position = _entries.Count;
            for (var i = _entries.Count - 1; i >= 0; i--) {
                _entries[i].SetPosition(position--);
                _entries[i].transform.SetSiblingIndex(0);
            }
        }
    }
}