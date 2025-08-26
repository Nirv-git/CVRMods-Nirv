using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using ABI_RC.Core.UI;
using ABI_RC.Core;
using ABI_RC.Core.Player;

namespace VoiceConnectionStatus
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ABI_RC.Core.UI.CohtmlHud), nameof(CohtmlHud.SetCommsIndicator))]
        internal static void AfterSetCommsIndicator(bool shown)
        {
            var connected = !shown;
            Main.LogLine($"COMS: Status changed to: {(connected ? "Connected" : "Disconnected")}");

            //Kill the waitReconnect if any new message comes in
            if (Main.waitReconnect_Rout != null) MelonCoroutines.Stop(Main.waitReconnect_Rout);
            if (connected)
            { //Wait to make sure the connection is stable
                Main.waitReconnect_Rout = MelonCoroutines.Start(Main.VerifyReconnect());
            }
            else if (Main.currentVoiceState != false) //Disconnected
            {
                Main.HandleConnection(false);
            }
        }

        //// Avatar
        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(ABI_RC.Core.Player.PlayerSetup), nameof(PlayerSetup.SetupAvatarGeneral))]
        //internal static void AfterSetupAvatarGeneral()
        //{
        //    //Main.Logger.Msg($"9-1");
        //    Main.OnSetupAvatarGeneral();
        //}


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ABI_RC.Core.CommonTools), nameof(CommonTools.Log))]
        internal static void AfterLog(CommonTools.LogLevelType_t t, string message, string code)
        {
            if (code != "COMMUNICATIONS") return;
            Main.LogLine($"GAMELOG: Code:{code} | Type:{t} | Msg:{message}");
        }
    }

}
