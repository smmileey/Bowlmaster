using System;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using Assets.Scripts.Wrappers;
using UnityEngine;

namespace Assets.Scripts
{
    public class ScoreDisplay : MonoBehaviour
    {
        private List<ScoreDisplayWrapper> _scoreDisplays;
        private readonly ScoreDisplayManager _scoreDisplayManager = new ScoreDisplayManager();

        void Start()
        {
        }
    }
}
