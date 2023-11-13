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
using ABI_RC.Systems.MovementSystem;




namespace ViewPointTweaks
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {
        internal static bool wasReset = false; //True if we should restore offsets


        [HarmonyPrefix]
        [HarmonyPatch(typeof(ABI_RC.Core.Player.PlayerSetup), nameof(PlayerSetup.SetViewPointOffset))]
        internal static bool OnSetViewPointOffset()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"OnSetViewPointOffset");
            try
            {//Undo transforms 
                if (Main.currentScaleAdjustment != 0 || Main.currentPosAdjustment != Vector3.zero || Main.currentRotAdjustment != 0)
                {
                    //Main.Logger.Msg($"Resetting Offsets");
                    Main.ResetOffsets();
                    wasReset = true;
                }   
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnSetViewPointOffset \n" + ex.ToString());
            }
            return true;
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(ABI_RC.Core.Player.PlayerSetup), nameof(PlayerSetup.SetViewPointOffset))]
        internal static void AfterSetViewPointOffset()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"AfterSetViewPointOffset");
            try
            {//Undo transforms 
                if (wasReset)
                {
                    //Main.Logger.Msg($"Was Reset. Setting to, Scale:{Main.lastScaleAdjustment} Pos X:{Main.lastPosAdjustment.x} Y:{Main.lastPosAdjustment.y} Z:{Main.lastPosAdjustment.z} Rot{Main.lastRotAdjustment}");
                    Main.ChangeOffsets(Main.lastScaleAdjustment, Main.lastPosAdjustment, Main.lastRotAdjustment);
                    wasReset = false;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in AfterSetViewPointOffset \n" + ex.ToString());
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(ABI_RC.Core.Player.PlayerSetup), nameof(PlayerSetup.SetupAvatarVr))]
        internal static bool OnSetupAvatarVr()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"OnSetupAvatarVr");
            try
            {//Undo transforms 
                if (Main.currentScaleAdjustment != 0 || Main.currentPosAdjustment != Vector3.zero || Main.currentRotAdjustment != 0)
                {
                    //Main.Logger.Msg($"Resetting Offsets");
                    Main.ResetOffsets();
                    wasReset = false;
                }

            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnSetupAvatarVr \n" + ex.ToString());
            }
            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(ABI_RC.Core.Player.PlayerSetup), nameof(PlayerSetup.SetupAvatarDesktop))]
        internal static bool OnSetupAvatarDesktop()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"OnSetupAvatarDesktop");
            try
            {//Undo transforms 
                if (Main.currentScaleAdjustment != 0 || Main.currentPosAdjustment != Vector3.zero || Main.currentRotAdjustment != 0)
                {
                   //Main.Logger.Msg($"Resetting Offsets");
                    Main.ResetOffsets();
                    wasReset = false;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnSetupAvatarDesktop \n" + ex.ToString());
            }
            return true;
        }

        //https://github.com/kafeijao/Kafe_CVR_Mods/blob/6e2b44b2ed3db22d21096ca53177be3a298a4f46/OSC/HarmonyPatches.cs#L24
        // Avatar
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MovementSystem), nameof(MovementSystem.UpdateAnimatorManager))]
        internal static void AfterUpdateAnimatorManager(CVRAnimatorManager manager)
        {
            //Main.Logger.Msg($"9-1");
            Main.OnAnimatorManagerUpdate(manager);
        }

    }
}
