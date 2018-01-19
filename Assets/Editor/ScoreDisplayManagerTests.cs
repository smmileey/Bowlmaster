using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Consts;
using Assets.Scripts.Managers;
using Assets.Scripts.Wrappers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Editor
{
    public class ScoreDisplayManagerTests
    {
        [Test]
        public void T01_WhenArgumentNullArgumentNullExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new ScoreDisplayManager().Convert(null));
        }

        [Test]
        public void T02_WhenNoChildObjects_EmptyListReturned()
        {
            Assert.AreEqual(0, new ScoreDisplayManager().Convert(new GameObject()).Count);
        }

        [Test]
        public void T03_WhenChildNameDoesNotMatchFramesEnum_ArgumentExceptionIsThrown()
        {
            GameObject scoreDisplay = new GameObject();

            GameObject f1 = new GameObject("FF");
            f1.transform.SetParent(scoreDisplay.transform);

            var sut = new ScoreDisplayManager();
            Assert.Throws<ArgumentException>(() => sut.Convert(scoreDisplay));
        }

        [Test]
        public void T04_WhenConvertingObjectWithOneChild_ThatHasOneFirstScore_OnlyOneIsReturned()
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
            List<ScoreDisplayWrapper> scoreDisplayWrappers = sut.Convert(scoreDisplay);

            Assert.AreEqual(1, scoreDisplayWrappers.Count);
            Assert.NotNull(scoreDisplayWrappers.First().FirstScore);
            Assert.AreEqual(1, scoreDisplayWrappers.First().FrameIndex);
        }

        [Test]
        public void T05_WhenConvertingObjectWithTwoChilds_ThatHasFirstScoreAndSecondScore_BothAreReturned()
        {
            GameObject scoreDisplay = new GameObject();

            GameObject f1 = new GameObject("F1");
            f1.transform.SetParent(scoreDisplay.transform);

            GameObject one = new GameObject("1") { tag = Tags.FirstScore };
            one.transform.SetParent(f1.transform);
            GameObject two = new GameObject("2") { tag = Tags.SecondScore };
            two.transform.SetParent(f1.transform);

            GameObject text = new GameObject("text");
            text.AddComponent(typeof(Text));
            text.transform.SetParent(one.transform);
            GameObject textTwo = new GameObject("text");
            textTwo.AddComponent(typeof(Text));
            textTwo.transform.SetParent(two.transform);

            ScoreDisplayManager sut = new ScoreDisplayManager();
            List<ScoreDisplayWrapper> scoreDisplayWrappers = sut.Convert(scoreDisplay);

            Assert.AreEqual(1, scoreDisplayWrappers.Count);
            Assert.NotNull(scoreDisplayWrappers.First().FirstScore);
            Assert.NotNull(scoreDisplayWrappers.First().SecondScore);
            Assert.AreEqual(1, scoreDisplayWrappers.First().FrameIndex);
        }

        [Test]
        public void T05_WhenConvertingObjectWithThreeChilds_ThatHasFirstScoreAndSecondScoreAndThridScore_ThreeAreReturned()
        {
            GameObject scoreDisplay = new GameObject();

            GameObject f1 = new GameObject("F1");
            f1.transform.SetParent(scoreDisplay.transform);

            GameObject one = new GameObject("1") { tag = Tags.FirstScore };
            one.transform.SetParent(f1.transform);
            GameObject two = new GameObject("2") { tag = Tags.SecondScore };
            two.transform.SetParent(f1.transform);
            GameObject three = new GameObject("3") { tag = Tags.ThirdScore };
            three.transform.SetParent(f1.transform);

            GameObject text = new GameObject("text");
            text.AddComponent(typeof(Text));
            text.transform.SetParent(one.transform);
            GameObject textTwo = new GameObject("text");
            textTwo.AddComponent(typeof(Text));
            textTwo.transform.SetParent(two.transform);
            GameObject textThree = new GameObject("text");
            textThree.AddComponent(typeof(Text));
            textThree.transform.SetParent(three.transform);

            ScoreDisplayManager sut = new ScoreDisplayManager();
            List<ScoreDisplayWrapper> scoreDisplayWrappers = sut.Convert(scoreDisplay);

            Assert.AreEqual(1, scoreDisplayWrappers.Count);
            Assert.NotNull(scoreDisplayWrappers.First().FirstScore);
            Assert.NotNull(scoreDisplayWrappers.First().SecondScore);
            Assert.NotNull(scoreDisplayWrappers.First().ThirdScore);
            Assert.AreEqual(1, scoreDisplayWrappers.First().FrameIndex);
        }

        [Test]
        public void T06_WhenConvertingObjectWithTwoChilds_ThreeAreReturned()
        {
            GameObject scoreDisplay = new GameObject();

            GameObject f1 = new GameObject("F1");
            f1.transform.SetParent(scoreDisplay.transform);
            GameObject f2 = new GameObject("F2");
            f2.transform.SetParent(scoreDisplay.transform);

            ScoreDisplayManager sut = new ScoreDisplayManager();
            List<ScoreDisplayWrapper> scoreDisplayWrappers = sut.Convert(scoreDisplay);

            Assert.AreEqual(2, scoreDisplayWrappers.Count);
            Assert.AreEqual(1, scoreDisplayWrappers.First().FrameIndex);
            Assert.AreEqual(2, scoreDisplayWrappers.Last().FrameIndex);
        }
    }
}
