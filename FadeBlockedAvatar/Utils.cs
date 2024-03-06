using System;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.Player;
using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core.Savior;
using System.Security.Cryptography;
using System.Text;
using ABI_RC.Core.Player;

namespace FadeBlockedAvatar
{
    public static class Utils
    {

        public static Color GetColorFromID(string id)
        { //PlayerID | {PlayerID}/[PlayerAvatar]"
            float minBrightness = 0.5f;
            byte[] hashBytes;
            using (MD5 md5Hash = MD5.Create())
            {
                hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(id));
            }
            // Use parts of the hash to generate RGB values.
            float r = hashBytes[0] / 255.0f;
            float g = hashBytes[1] / 255.0f;
            float b = hashBytes[2] / 255.0f;
            // Ensure the brightness is above the minimum threshold.
            float brightness = (r + g + b) / 3f;
            if (brightness < minBrightness)
            {
                // Adjust the color to meet the minimum brightness.
                float delta = (minBrightness - brightness) / 3f;
                r += delta;
                g += delta;
                b += delta;
            }
            return new Color(r, g, b);
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
