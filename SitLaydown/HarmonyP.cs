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
using ABI_RC.Systems.Movement;

namespace SitLaydown
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BetterBetterCharacterController), nameof(BetterBetterCharacterController.Animate))]
        internal static void OnAnimate()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"1-1 OnExitSeat");
            try
            {
                if (Main.inChair)
                {
                    BetterBetterCharacterController.Instance.CurrentAnimatorManager.SetAnimatorParameterBool("Grounded", true);
                    BetterBetterCharacterController.Instance.CurrentAnimatorManager.SetAnimatorParameterBool("Crouching", false);
                    BetterBetterCharacterController.Instance.CurrentAnimatorManager.SetAnimatorParameterBool("Prone", false);
                    BetterBetterCharacterController.Instance.CurrentAnimatorManager.SetAnimatorParameterBool("Flying", false);
                    BetterBetterCharacterController.Instance.CurrentAnimatorManager.SetAnimatorParameterBool("Swimming", false);
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnAnimate patch. \n" + ex.ToString());
            }
            //Main.Logger.Msg(ConsoleColor.Yellow, $"1-10 End and return True");
        }


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
        [HarmonyPatch(typeof(CVRSeat), nameof(CVRSeat.ExitSeat))]
        internal static bool OnExitSeat()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"1-1 OnExitSeat");
            try
            {
                if (Main.inChair && Main.preventLeavingSeat.Value)
                {
                    ViewManager.Instance.UiStateToggle(!ViewManager.Instance.gameMenuView.Enabled);
                    //Main.Logger.Msg(ConsoleColor.Yellow, $"1-5 Not leaving chair, skipping method");
                    if (Main.lastHUDnotif + 300 < Time.time)
                    {
                        CohtmlHud.Instance.ViewDropTextImmediate("SitLaydown", "Prevented Seat Exit", "Mod prevented you from leaving your seat, use the SitLaydown UI or Respawn.");
                        Main.lastHUDnotif = Time.time;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnExitSeat patch. \n" + ex.ToString());
            }
            //Main.Logger.Msg(ConsoleColor.Yellow, $"1-10 End and return True");
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RootLogic), nameof(RootLogic.Respawn), new Type[] { })]
        internal static void OnRespawn()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"2-1 OnRespawn");
            try
            {
                if (Main.inChair)
                {
                    Main.ToggleChair(false);
                    //Main.Logger.Msg(ConsoleColor.Magenta, "Left chair due to Respawn");
                }
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
                if (Main.inChair)
                {
                    Main.ToggleChair(false);
                    //Main.Logger.Msg(ConsoleColor.Magenta, "Left chair due to Respawn");
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnRespawnForced patch. \n" + ex.ToString());
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ABI_RC.Core.Player.PlayerSetup), nameof(PlayerSetup.SetupAvatarGeneral))]
        internal static void OnSetupAvatarGeneral()
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"2-1 OnSetupAvatarGeneral");
            try
            {
                if (Main.inChair)
                {
                    Main.ToggleChair(false);
                    //Main.Logger.Msg(ConsoleColor.Magenta, "Left chair due to Avatar Change");
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnRespawn patch. \n" + ex.ToString());

            }
        }
    }
}
