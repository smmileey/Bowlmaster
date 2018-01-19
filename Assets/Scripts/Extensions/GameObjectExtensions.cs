using System;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class GameObjectExtensions
    {

        public static GameObject FindChildWithTag(this GameObject gameObject, string tag)
        {
            if (gameObject == null) throw new ArgumentNullException(nameof(gameObject));

            foreach (Transform child in gameObject.transform)
            {
                if (child.tag.Equals(tag)) return child.gameObject;
            }

            return null;
        }
    }
}
