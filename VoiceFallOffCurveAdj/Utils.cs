using UnityEngine;
using System;
using MelonLoader;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;


namespace VoiceFallOffCurveAdj
{
    public static class Utils
    {



        public static float Derivative(this AnimationCurve self, float time)
        { //https://forum.unity.com/threads/get-the-angle-of-an-animationcurve.106638/#post-4167574
            if (self == null) return 0.0f;
            for (int i = 0; i < self.length - 1; i++)
            {
                if (time < self[i].time) continue;
                if (time > self[i + 1].time) continue;
                return Derivative(self[i], self[i + 1], (time - self[i].time) / (self[i + 1].time - self[i].time));
            }
            return 0.0f;
        }

        private static float Derivative(Keyframe from, Keyframe to, float lerp)
        {
            float dt = to.time - from.time;

            float m0 = from.outTangent * dt;
            float m1 = to.inTangent * dt;

            float lerp2 = lerp * lerp;

            float a = 6.0f * lerp2 - 6.0f * lerp;
            float b = 3.0f * lerp2 - 4.0f * lerp + 1.0f;
            float c = 3.0f * lerp2 - 2.0f * lerp;
            float d = -a;

            return a * from.value + b * m0 + c * m1 + d * to.value;
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
