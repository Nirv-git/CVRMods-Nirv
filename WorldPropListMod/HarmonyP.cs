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
using DarkRift;
using BTKUILib;
using ABI_RC.Core.UI;


namespace WorldPropListMod
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVRSelfModerationManager), nameof(CVRSelfModerationManager.GetPropVisibility))]
        internal static bool OnGetPropVisibility(string userId, string propId, out bool wasForceHidden, out bool wasForceShown)
        {
            wasForceHidden = false;
            wasForceShown = false;

            try
            {
                //Main.Logger.Msg(ConsoleColor.Yellow, $"9-15 OnGetPropVisibility");
                Main.FindPropAPIname(propId);
                Main.FindPlayerAPIname(userId);
                if (Main.usePropBlockList.Value && Main.blockedProps.ContainsKey(propId))
                {
                    if (userId == "SYSTEM" || userId == "LocalServer")
                    {
                        Main.Logger.Msg(ConsoleColor.Yellow, $"Can not block prop: {Main.blockedProps[propId]} - Spawned by: {userId}, ID:{propId}");
                        Main.PropsThisSession.Add((propId, userId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                        return true;
                    }
                    var msg = $"Mod Blocking Prop: {Main.blockedProps[propId]}, SpawnedBy: {(Main.PlayerNamesCache.TryGetValue(userId, out var obj) ? obj.Item1 : userId)}";
                    Main.Logger.Msg(ConsoleColor.Magenta, ">>>> PROP BLOCKED <<<<");
                    Main.Logger.Msg(ConsoleColor.Magenta, msg + $" - {userId}, ID:{propId}");
                    QuickMenuAPI.ShowAlertToast(msg, 3);
                    if (Main.showHUDNotification.Value) CohtmlHud.Instance.ViewDropText("Prop blocked", msg);
                    Main.BlockedThisSession.Add((propId, userId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                    wasForceHidden = true;
                    return false;
                }
                else
                {   //GUID,PlayerGUID,Time
                    Main.PropsThisSession.Add((propId, userId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                    //Main.Logger.Msg($"ObjectId {ObjectId} InstanceId {InstanceId} SpawnedBy {SpawnedBy}");
                }
            }
            catch (Exception ex) { Main.Logger.Error("Error writing prop blocklist \n" + ex.ToString()); }
            //Main.Logger.Msg(ConsoleColor.Yellow, $"9-1 50");
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVRSyncHelper), nameof(CVRSyncHelper.SpawnProp))]
        internal static bool OnSpawnProp(string propGuid, float PosX, float PosY, float PosZ)
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"9-2 SpawnProp");
            //Main.Logger.Msg($"propGuid {propGuid}");
            Main.FindPropAPIname(propGuid);
            if (Main.usePropBlockList.Value && Main.blockedProps.ContainsKey(propGuid))
            {
                var msg = $"Mod Blocking Prop: {Main.blockedProps[propGuid]}, SpawnedBy: Self";
                Main.Logger.Msg(ConsoleColor.Magenta, ">>>> PROP BLOCKED <<<<");
                Main.Logger.Msg(ConsoleColor.Magenta, msg + $" - ID:{propGuid}");
                QuickMenuAPI.ShowAlertToast(msg, 3);
                if (Main.showHUDNotification.Value) CohtmlHud.Instance.ViewDropText("Prop blocked", msg);
                Main.BlockedThisSession.Add((propGuid, MetaPort.Instance.ownerId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                return false;
            }
            return true;
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(CVRSyncHelper), nameof(CVRSyncHelper.DeleteAllProps))]
        //internal static void AfterDeleteAllProps()
        //{
            //Main.Logger.Msg(ConsoleColor.Yellow, $"9-5 DeleteAllProps");
        //}
    }
}
