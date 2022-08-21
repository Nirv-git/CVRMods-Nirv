using System;
using MelonLoader;
using UnityEngine;

namespace NamePlateWithQM
{
    public static class Utils
    {

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
