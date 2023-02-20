using System;
using ABI_RC.Core;
using ABI_RC.Core.Savior;
using System.Reflection;
using UnityEngine;
using Valve.VR;

namespace PortableMirror
{
    //Credit goes to NotAKid for a lot of this
    //https://github.com/NotAKidOnSteam/PickupPushPull/blob/main/PickupPushPull/InputModules/PickupPushPull_Module.cs

    public class InputSVR : CVRInputModule
    {
        private static readonly FieldInfo im_vrLookAction = typeof(InputModuleSteamVR).GetField("vrLookAction", BindingFlags.NonPublic | BindingFlags.Instance);
        private static SteamVR_Action_Vector2 vrLookAction;
        private static float deadzoneRightValue;
        public static InputSVR Instance;
        public static Vector2 lookVector = Vector2.zero;

        public new void Start()
        {
            Instance = this;
            InputModuleSteamVR inputModuleSteamVR = GetComponent<InputModuleSteamVR>();
            vrLookAction = (SteamVR_Action_Vector2)im_vrLookAction.GetValue(inputModuleSteamVR);
            deadzoneRightValue = (float)MetaPort.Instance.settings.GetSettingInt("ControlDeadZoneRight", 0) / 100f;
        }

        public static Vector2 GetVRLookVector()
        {
            return new Vector2(CVRTools.AxisDeadZone(vrLookAction.GetAxis(SteamVR_Input_Sources.Any).x, deadzoneRightValue, true),
                                                CVRTools.AxisDeadZone(vrLookAction.GetAxis(SteamVR_Input_Sources.Any).y, deadzoneRightValue, true));
        }
    }
}