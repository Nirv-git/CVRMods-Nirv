using UnityEngine;
using System;
using MelonLoader;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace VisemeValue
{
    public static class Utils
    {
        //https://discussions.unity.com/t/is-there-a-way-to-check-if-an-animatorcontroller-has-a-parameter/86194/5
        public static bool ContainsParam(this Animator _Anim, string _ParamName)
        {
            foreach (AnimatorControllerParameter param in _Anim.parameters)
            {
                if (param.name == _ParamName) return true;
            }
            return false;
        }
    }
}
