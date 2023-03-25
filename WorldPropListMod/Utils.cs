using UnityEngine;
using System;
using MelonLoader;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;



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
    }
}
