using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI_RC.VideoPlayer.Scripts;
using MelonLoader;
using UnityEngine;

namespace RemoveChairs
{
    public static class Utils
    {
        public static string GetPath(this Transform current)
        { //http://answers.unity.com/answers/261847/view.html
            if (current.parent == null)
                return "World:" + current.name;
            if (current.name.Contains("CVRSpawnable_"))
                return "Prop:";
            return current.parent.GetPath() + "/ " + current.name;
        }

    }
}

