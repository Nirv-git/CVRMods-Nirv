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

namespace PersonalGravity
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CVR_MenuManager), nameof(CVR_MenuManager.ToggleQuickMenu))]
        internal static void OnToggleQuickMenu(bool show)
        {
            try
            {
                BTKUI_Cust.QMtoggle(show);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("Error in OnToggleQuickMenu \n" + ex.ToString());
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(RootLogic), nameof(RootLogic.Respawn), new Type[] { })]
        internal static void OnRespawn()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"2-1 OnRespawn");
            try
            {
                Main.ToggleGrav(false);
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnRespawn patch. \n" + ex.ToString());
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RootLogic), nameof(RootLogic.Respawn), new Type[] { typeof(bool)})]
        internal static void OnRespawnForced()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"2-1 OnRespawn");
            try
            {
                Main.ToggleGrav(false);
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnRespawnForced patch. \n" + ex.ToString());
            }
        }

    }
}
