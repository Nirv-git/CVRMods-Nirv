using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core;
using ABI_RC.Core.Player;
using HarmonyLib;

namespace VisemeValue
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ABI_RC.Core.Player.PlayerSetup), nameof(PlayerSetup.SetupAvatarVr))]
        internal static void OnSetupAvatarVr()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"OnSetupAvatarVr");
            Main.AvatarReady();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ABI_RC.Core.Player.PlayerSetup), nameof(PlayerSetup.SetupAvatarDesktop))]
        internal static void OnSetupAvatarDesktop()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"OnSetupAvatarDesktop");
            Main.AvatarReady();
        }
    }
}
