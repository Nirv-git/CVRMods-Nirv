﻿using System;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.Player;

namespace PortableMirror
{
    public static class Utils
    {
        public static PlayerSetup GetPlayer()
        {
            return ABI_RC.Core.Player.PlayerSetup.Instance;
        }

        //public static KeyCode GetMirrorKeybind()
        //{
        //    string modPrefKeybind = Main._base_MirrorKeybind.Value.Trim();
        //    if (string.IsNullOrWhiteSpace(modPrefKeybind)) modPrefKeybind = "Alpha1";
        //    if (modPrefKeybind.Length == 1)
        //    {
        //        char keybindChar = modPrefKeybind.ToLower()[0];
        //        return (KeyCode)keybindChar;
        //    }
        //    modPrefKeybind = char.ToUpper(modPrefKeybind[0]) + modPrefKeybind.Substring(1);
        //    return Enum.TryParse(modPrefKeybind, out KeyCode keybind) ? keybind : KeyCode.Alpha1;
        //}

        public static bool GetKey(KeyCode key, bool control = false, bool shift = false)
        {
            bool controlFlag = !control;
            bool shiftFlag = !shift;
            if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                controlFlag = true;
            }
            if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                shiftFlag = true;
            }
            return controlFlag && shiftFlag && Input.GetKey(key);
        }

        public static bool GetKeyDown(KeyCode key, bool control = false, bool shift = false)
        {
            bool controlFlag = !control;
            bool shiftFlag = !shift;
            if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                controlFlag = true;
            }
            if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                shiftFlag = true;
            }
            return controlFlag && shiftFlag && Input.GetKeyDown(key);
        }

        public static bool GetKeyUp(KeyCode key, bool control = false, bool shift = false)
        {
            bool controlFlag = !control;
            bool shiftFlag = !shift;
            if (control && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                controlFlag = true;
            }
            if (shift && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                shiftFlag = true;
            }
            return controlFlag && shiftFlag && Input.GetKeyUp(key);
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

        public static string RoundFloatToString(float x)
        {
            if (x >= 10f)
                return x.ToString("F0");
            else
                return x.ToString("F2");
        }

        //https://forum.unity.com/threads/quaternion-smoothdamp.793533/#post-6673789
        public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
        {
            Vector3 c = current.eulerAngles;
            Vector3 t = target.eulerAngles;
            return Quaternion.Euler(
              Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
              Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
              Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
            );
        }


    }
}
