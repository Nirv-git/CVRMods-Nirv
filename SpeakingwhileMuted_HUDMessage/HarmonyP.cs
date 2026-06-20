using ABI.CCK.Components;
using ABI_RC.Core;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using ABI_RC.Core.PropManagement;
using ABI_RC.Core.Savior;
using ABI_RC.Core.UI;
using ABI_RC.Core.Util;
using ABI_RC.Core.Util.AssetFiltering;
using ABI_RC.Systems.Communications;
using DarkRift;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SpeakingwhileMuted_HUDMessage
{
    [HarmonyPatch]
    class HarmonyPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CohtmlHud), nameof(CohtmlHud.OnMutedSpeechDetected))]
        internal static void OnMutedSpeechDetection()
        {
            try
            {
                if (!Main.sendHUDmsg.Value)
                    return;

                CohtmlHud instance2 = CohtmlHud.Instance;
                if (instance2 != null)
                {
                    instance2.ViewDropTextImmediate("", "Speaking while Muted", "You are trying to speak with your Microphone muted!", "", false);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("Error in OnMutedSpeechDetection \n" + ex.ToString());
            }
        }

    }
}
