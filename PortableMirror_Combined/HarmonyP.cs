using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util;
using ABI.CCK.Components;
using HarmonyLib;
using ABI_RC.Core.UI;
using System.Threading;
using ABI_RC.Core.InteractionSystem;

namespace PortableMirror
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ABI.CCK.Components.CVRPickupObject), nameof(CVRPickupObject.Grab))]
        internal static bool OnGrab(Transform handTransform, ControllerRay controllerRay, Vector3 hitPoint)
        {
            if (controllerRay == PlayerSetup.Instance.vrRayRight && (Mirrors.hitFound || Mirrors.held))
                return false;
            return true;
        }
    }
}
