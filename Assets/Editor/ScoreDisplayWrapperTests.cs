using System.Collections;
using Assets.Scripts.Consts;
using Assets.Scripts.Managers;
using Assets.Scripts.Wrappers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Assets.Editor
{
    public class ScoreDisplayWrapperTests
    {

        [Test]
        public void ScoreDisplayWrapperTestsSimplePasses()
        {
            GameObject scoreDisplay = new GameObject();

            GameObject f1 = new GameObject("F1");
            f1.transform.SetParent(scoreDisplay.transform);

            GameObject one = new GameObject("1") { tag = Tags.FirstScore };
            one.transform.SetParent(f1.transform);

            GameObject text = new GameObject("text");
            text.AddComponent(typeof(Text));
            text.transform.SetParent(one.transform);

            ScoreDisplayManager sut = new ScoreDisplayManager();
            var scoreDisplayWrappers = sut.Convert(scoreDisplay);
        }
    }
}
