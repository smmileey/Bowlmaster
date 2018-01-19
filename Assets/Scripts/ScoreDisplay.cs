using System;
using System.Collections.Generic;
using Assets.Scripts.Consts;
using Assets.Scripts.Extensions;
using Assets.Scripts.Managers;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ScoreDisplay : MonoBehaviour
    {
        public string[] Players;
        public GameObject ScoreDisplayPrefab;

        private readonly Dictionary<string, ScoreDisplayResolver> _scoreDisplays = new Dictionary<string, ScoreDisplayResolver>();
        private readonly ScoreDisplayConverter _scoreDisplayConverter = new ScoreDisplayConverter();

        void Start()
        {
            if (Players == null || Players.Length == 0) throw new ArgumentException(nameof(Players));
            if (ScoreDisplayPrefab == null) throw new ArgumentNullException(nameof(ScoreDisplayPrefab));

            foreach (var player in Players)
            {
                if (_scoreDisplays.ContainsKey(player)) throw new ArgumentException($"Player with name {player} is already in game.");
                InitializeScoreDisplay(player);
            }
        }

        public ScoreDisplayResolver Get(string player)
        {
            ScoreDisplayResolver scoreDisplayWrappers;
            if (!_scoreDisplays.TryGetValue(player, out scoreDisplayWrappers)) throw new ArgumentException($"Player with name {player} does not exist.");

            return scoreDisplayWrappers;
        }

        private void InitializeScoreDisplay(string player)
        {
            GameObject newDisplay = Instantiate(ScoreDisplayPrefab, transform);
            newDisplay.name = player;
            newDisplay.FindChildWithTag(Tags.PlayerName).GetComponent<Text>().text = player;
            _scoreDisplays.Add(player, new ScoreDisplayResolver(_scoreDisplayConverter.Convert(newDisplay)));
        }
    }
}
