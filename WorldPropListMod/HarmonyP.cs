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
using System.Threading;



namespace WorldPropListMod
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVRSelfModerationManager), nameof(CVRSelfModerationManager.GetPropVisibility))]
        internal static bool OnGetPropVisibility(string userId, string propId, out bool wasForceHidden, out bool wasForceShown)
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"9-15 OnGetPropVisibility");
            wasForceHidden = false;
            wasForceShown = false;
            try
            {
                //Main.Logger.Msg(ConsoleColor.Cyan, $"propId: {propId}");
                string propGUID = propId.StartsWith("p+") ? propId.Substring(2) : propId;
                if (propGUID.IndexOf('~') != -1)
                    propGUID = propGUID.Substring(0, propGUID.IndexOf('~'));

                //Main.Logger.Msg(ConsoleColor.Cyan, $"propGUID: {propGUID}");

                Main.FindPropAPIname(propGUID);
                Main.FindPlayerAPIname(userId);
                if (Main.usePropBlockList.Value && Main.blockedProps.ContainsKey(propGUID))
                {
                    if (userId == "SYSTEM" || userId == "LocalServer")
                    {
                        Main.Logger.Msg(ConsoleColor.Yellow, $"Can not block prop: {Main.blockedProps[propGUID]} - Spawned by: {userId}, ID:{propGUID}");
                        if (!Utils.IsRepeat(propGUID, Main.PropsThisSession))
                            Main.PropsThisSession.Add((propGUID, userId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                        return true;
                    }
                    var msg = $"Mod Blocking Prop: {Main.blockedProps[propGUID]}, SpawnedBy: {(Main.PlayerNamesCache.TryGetValue(userId, out var obj) ? obj.Item1 : userId)}";
                    Main.Logger.Msg(ConsoleColor.Magenta, ">>>> PROP BLOCKED <<<<");
                    Main.Logger.Msg(ConsoleColor.Magenta, msg + $" - {userId}, ID:{propGUID}");
                    if (Main.showHUDNotification.Value)
                    {
                        if (!Main.Instance.IsOnMainThread(Thread.CurrentThread))
                        {
                            //Main.Logger.Msg(ConsoleColor.Yellow, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"); //me_irl
                            Main.Instance.MainThreadQueue.Enqueue(() =>
                            {
                                CohtmlHud.Instance.ViewDropText("Prop blocked", msg);
                            });
                        }
                        else
                            CohtmlHud.Instance.ViewDropText("Prop blocked", msg);
                    }
                    if (!Utils.IsRepeat(propGUID, Main.BlockedThisSession))
                        Main.BlockedThisSession.Add((propGUID, userId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                    wasForceHidden = true;
                    return false;
                }
                else
                {   //GUID,PlayerGUID,Time
                    if (!Utils.IsRepeat(propGUID, Main.PropsThisSession))
                        Main.PropsThisSession.Add((propGUID, userId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                    //Main.Logger.Msg($"ObjectId {ObjectId} InstanceId {InstanceId} SpawnedBy {SpawnedBy}");
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnGetPropVisibility patch. Possible corruption in lists, clearing. \n" + ex.ToString());
                Main.BlockedThisSession.Clear();
                Main.PropsThisSession.Clear();
            }
            //Main.Logger.Msg(ConsoleColor.Yellow, $"9-1 50");
            return true;
        }

        

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVRSyncHelper), nameof(CVRSyncHelper.SpawnProp))]
        internal static bool OnSpawnProp(string propGuid, float PosX, float PosY, float PosZ)
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"9-2 SpawnProp");
            //Main.Logger.Msg($"propGuid {propGuid}");
            try
            {
                Main.FindPropAPIname(propGuid);
                if (Main.usePropBlockList.Value && Main.blockedProps.ContainsKey(propGuid))
                {
                    var msg = $"Mod Blocking Prop: {Main.blockedProps[propGuid]}, SpawnedBy: Self";
                    Main.Logger.Msg(ConsoleColor.Magenta, ">>>> PROP BLOCKED <<<<");
                    Main.Logger.Msg(ConsoleColor.Magenta, msg + $" - ID:{propGuid}");
                    if (Main.showHUDNotification.Value) CohtmlHud.Instance.ViewDropText("Prop blocked", msg);
                    if (!Utils.IsRepeat(propGuid, Main.BlockedThisSession))
                        Main.BlockedThisSession.Add((propGuid, MetaPort.Instance.ownerId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error in OnSpawnProp patch. Possible corruption in lists, clearing. \n" + ex.ToString());
                Main.BlockedThisSession.Clear();
                Main.PropsThisSession.Clear();
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
