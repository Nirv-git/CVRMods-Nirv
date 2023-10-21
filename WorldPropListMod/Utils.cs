using UnityEngine;
using System;
using MelonLoader;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;



namespace WorldPropListMod
{
    public static class Utils
    {
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
            return value.ToString("F2").TrimEnd('0');
        }

        public static string NumFormat(float value, int legnth)
        {
            return value.ToString($"F{legnth}").TrimEnd('0');
        }

        //https://stackoverflow.com/a/38700070
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

        public static DateTime ParseDate(string value, string format)
        {
            if (DateTime.TryParseExact(value, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var dt))
            {
                return dt;
            }
            MelonLogger.Msg(ConsoleColor.Red, $"Date Error: {value}");
            return DateTime.MinValue;
        }

        public static bool IsRepeat(string propGUID, List<(string, string, string)> list)
        {
            var last = list.LastOrDefault();
            if (last.Item1 == propGUID && Utils.ParseDate(last.Item3, "yyyy-MM-dd HH:mm:ss") > DateTime.Now.AddSeconds(-5))
                return true;
            return false;
        }
    }
}
