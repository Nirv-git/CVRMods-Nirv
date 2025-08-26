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

namespace FadeBlockedAvatar
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PuppetMaster), nameof(PuppetMaster.OnSetupAvatar))]
        internal static void OnAvatarInstantiated(PuppetMaster __instance)
        {
            Main.SetupAvatar(__instance);
        }

    }
}
