using System;
using ABI_RC.Core;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.InputManagement;
using System.Reflection;
using UnityEngine;
using Valve.VR;

namespace PortableMirror
{
    //Credit goes to NotAKid for a lot of this
    //https://github.com/NotAKidOnSteam/PickupPushPull/blob/main/PickupPushPull/InputModules/PickupPushPull_Module.cs

    public class InputSVR
    {
        public static Vector2 GetVRLookVector()
        {
            //Main.Logger.Msg($":{CVRInputManager.Instance.floatDirection}");
            return new Vector2(0f, CVRInputManager.Instance.floatDirection);
        }
    }
}