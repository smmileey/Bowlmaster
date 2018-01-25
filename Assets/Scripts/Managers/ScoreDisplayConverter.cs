using System;
using System.Collections.Generic;
using Assets.Scripts.Consts;
using Assets.Scripts.Enums;
using Assets.Scripts.Extensions;
using Assets.Scripts.Mappers;
using Assets.Scripts.Wrappers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class ScoreDisplayConverter
    {
        public List<ScoreDisplayWrapper> Convert(GameObject scoreDisplay)
        {
            if (scoreDisplay == null) throw new ArgumentNullException(nameof(scoreDisplay));

            var scoreDisplays = new List<ScoreDisplayWrapper>();
            foreach (Transform frameScoreDisplay in scoreDisplay.transform)
            {
                if (frameScoreDisplay.tag.Equals(Tags.PlayerName) || frameScoreDisplay.tag.Equals(Tags.TotalScore)) continue;
                FillScoreDisplays(scoreDisplays, frameScoreDisplay);
            }

            return scoreDisplays;
        }

        private void FillScoreDisplays(List<ScoreDisplayWrapper> scoreDisplays, Transform frameScoreDisplay)
        {
            var frame = (Frames)Enum.Parse(typeof(Frames), frameScoreDisplay.name, ignoreCase: true);
            scoreDisplays.Add(new ScoreDisplayWrapper
            {
                FirstScore = frameScoreDisplay.gameObject.FindChildWithTag(Tags.FirstScore)
                    ? frameScoreDisplay.gameObject.FindChildWithTag(Tags.FirstScore).GetComponentInChildren<Text>()
                    : null,
                SecondScore = frameScoreDisplay.gameObject.FindChildWithTag(Tags.SecondScore)
                    ? frameScoreDisplay.gameObject.FindChildWithTag(Tags.SecondScore).GetComponentInChildren<Text>()
                    : null,
                ThirdScore = frameScoreDisplay.gameObject.FindChildWithTag(Tags.ThirdScore)
                    ? frameScoreDisplay.gameObject.FindChildWithTag(Tags.ThirdScore).GetComponentInChildren<Text>()
                    : null,
                FrameScore = frameScoreDisplay.gameObject.FindChildWithTag(Tags.FrameScore)
                    ? frameScoreDisplay.gameObject.FindChildWithTag(Tags.FrameScore).GetComponentInChildren<Text>()
                    : null,
                FrameIndex = FramesToFrameIndexMapper.GetFrame(frame)
            });
        }
    }
}
