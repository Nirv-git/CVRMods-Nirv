using UnityEngine;
using System;
using MelonLoader;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;


namespace IKpresetsMod
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
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static string NumFormat(float value)
        {
            return value.ToString("F3").TrimEnd('0');
        }

        public static string RandomString(int length)
        {
            var chars = "123456789";
            var stringChars = new char[length];
            var random = new System.Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            string temp = new String(stringChars);
            //Main.Logger.Msg("RandomString" + temp);
            return temp;
        }

        public static string CompactTF(bool value)
        {
            return value ? "T" : "F";
        }

        public static string ReturnCleanASCII(string s)
        {
            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if ((int)c > 127) // you probably don't want 127 either
                    continue;
                if ((int)c < 32)  // I bet you don't want control characters 
                    continue;
                //if (c == ',')
                //    continue;
                //if (c == '"')
                //    continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

    }
}
