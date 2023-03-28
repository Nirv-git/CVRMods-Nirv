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

namespace WorldPropListMod
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVRSyncHelper), nameof(CVRSyncHelper.SpawnPropFromNetwork))]
        internal static bool OnSpawnPropFromNetwork(Message message)
        {
            try { 
                //Main.Logger.Msg(ConsoleColor.Yellow, $"9-1 SpawnPropFromNetwork");
                using (DarkRiftReader reader = message.GetReader())
                {
                    float[] CustomFloats = new float[40];
                    var ObjectId = reader.ReadString();
                    var InstanceId = reader.ReadString();
                    var FileKey = reader.ReadString();
                    var PositionX = reader.ReadSingle();
                    var PositionY = reader.ReadSingle();
                    var PositionZ = reader.ReadSingle();
                    var RotationX = reader.ReadSingle();
                    var RotationY = reader.ReadSingle();
                    var RotationZ = reader.ReadSingle();
                    var ScaleX = reader.ReadSingle();
                    var ScaleY = reader.ReadSingle();
                    var ScaleZ = reader.ReadSingle();
                    var CustomFloatsAmount = reader.ReadInt32();
                    for (int index = 0; index < CustomFloatsAmount; ++index)
                        CustomFloats[index] = reader.ReadSingle();
                    var SpawnedBy = reader.ReadString();
                    if (Main.usePropBlockList.Value && Main.blockedProps.ContainsKey(ObjectId))
                    {
                        Main.FindPropAPIname(ObjectId);
                        Main.FindPlayerAPIname(SpawnedBy);
                        var msg = $"Mod Blocking Prop: {Main.blockedProps[ObjectId]}, SpawnedBy: {(Main.PlayerNamesCache.TryGetValue(SpawnedBy, out var obj) ? obj.Item1 : SpawnedBy)}";
                        Main.Logger.Msg(ConsoleColor.Magenta, ">>>> PROP BLOCKED <<<<");
                        Main.Logger.Msg(ConsoleColor.Magenta, msg + $" - {SpawnedBy}, ID:{ObjectId}");
                        QuickMenuAPI.ShowAlertToast(msg, 3);
                        Main.BlockedThisSession.Add((ObjectId, SpawnedBy, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                        return false;
                    }
                    else
                    {   //GUID,PlayerGUID,Time
                        Main.PropsThisSession.Add((ObjectId, SpawnedBy, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                        //Main.Logger.Msg($"ObjectId {ObjectId} InstanceId {InstanceId} SpawnedBy {SpawnedBy}");
                        Main.FindPropAPIname(ObjectId);
                        Main.FindPlayerAPIname(SpawnedBy); 
                    }
                }
            }
            catch (Exception ex) { Main.Logger.Error("Error writing prop blocklist \n" + ex.ToString()); }
            //Main.Logger.Msg(ConsoleColor.Yellow, $"9-1 50");
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CVRSyncHelper), nameof(CVRSyncHelper.SpawnProp))]
        internal static void OnSpawnProp(string propGuid, float PosX, float PosY, float PosZ)
        {
            //Main.Logger.Msg(ConsoleColor.Yellow, $"9-2 SpawnProp");
            //Main.Logger.Msg($"propGuid {propGuid}");
            Main.FindPropAPIname(propGuid);
        }

        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(CVRSyncHelper), nameof(CVRSyncHelper.DeleteAllProps))]
        //internal static void AfterDeleteAllProps()
        //{
            //Main.Logger.Msg(ConsoleColor.Yellow, $"9-5 DeleteAllProps");
        //}
    }
}
