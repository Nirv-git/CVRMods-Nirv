using System;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.Player;

namespace PersonalGravity
{
    public static class Utils
    {

        public static Vector3 RoundToNearest5(Vector3 vector)
        {
            float x = RoundToNearest(vector.x, .2f);
            float y = RoundToNearest(vector.y, .2f);
            float z = RoundToNearest(vector.z, .2f);

            return new Vector3(x, y, z);
        }

        // Rounds a single float value to the nearest multiple of 'nearest'
        public static float RoundToNearest(float value, float nearest)
        {
            return Mathf.Round(value / nearest) * nearest;
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                return gameObject.AddComponent<T>();
            }
            return component;
        }

        public static string GetPath(this Transform current)
        {//http://answers.unity.com/answers/261847/view.html
            if (current.parent == null)
                return "/" + current.name;
            return current.parent.GetPath() + "/" + current.name;
        }
      

    }
}
