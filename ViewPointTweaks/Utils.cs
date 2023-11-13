using UnityEngine;
using System;
using MelonLoader;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace ViewPointTweaks
{
    public static class Utils
    {

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
    }
}
