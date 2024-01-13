using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using ABI_RC.Core;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util;
using ABI_RC.Core.Util.AssetFiltering;
using ABI.CCK.Components;
using HarmonyLib;
using DarkRift;
using BTKUILib;
using ABI_RC.Core.UI;
using System.Threading;



namespace WorldPropListMod
{
    [HarmonyPatch]
    class HarmonyPatches
    {
        private static float lastBlock = 0;


        [HarmonyPrefix]
        [HarmonyPatch(typeof(CohtmlHud), nameof(CohtmlHud.ViewDropTextImmediate))]
        internal static bool OnViewDropTextImmediate(string cat, string headline, string small)
        {
            if (cat == "(Local) Client" && headline == "Cannot spawn prop" && small == "." && lastBlock + 2.5f >= Time.time)
            {
                Main.Logger.Msg(ConsoleColor.Yellow, $"Skipping ViewDropTextImmediate due to blocked prop");
                return false;
            } else if (cat == "(Local) Client" && headline == "Cannot spawn prop" && small == ".")
            {
                Main.Logger.Msg(ConsoleColor.Yellow, $"Skipping ViewDropTextImmediate due to blocked user");
                return false;
            }
            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVRSelfModerationManager), nameof(CVRSelfModerationManager.GetPropVisibility))]
        internal static bool OnGetPropVisibility(string userId, string propId, out bool wasForceHidden, out bool wasForceShown)//, ref bool __result)
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

                var props_Repeat = Utils.IsRepeat(propGUID, Main.PropsThisSession) || Utils.IsRepeat(propGUID, Main.BlockedThisSession);

                Main.FindPropAPIname(propGUID);
                Main.FindPlayerAPIname(userId);
                if (Main.usePropBlockList.Value && Main.blockedProps.ContainsKey(propGUID))
                {
                    if (userId == "SYSTEM" || userId == "LocalServer")
                    {
                        Main.Logger.Msg(ConsoleColor.Yellow, $"Can not block prop: {Main.blockedProps[propGUID]} - Spawned by: {userId}, ID:{propGUID}");
                        if (!props_Repeat)
                            Main.PropsThisSession.Add((propGUID, userId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                        return true;
                    }
                    var msg = $"Mod Blocking Prop: {Main.blockedProps[propGUID]}, SpawnedBy: {(Main.PlayerNamesCache.TryGetValue(userId, out var obj) ? obj.Item1 : userId)}";
                    Main.Logger.Msg(ConsoleColor.Magenta, ">>>> PROP BLOCKED <<<<");
                    Main.Logger.Msg(ConsoleColor.Magenta, msg + $" - {userId}, ID:{propGUID}");
                    if (Main.showHUDNotification.Value && !props_Repeat)
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
                    if (!props_Repeat)
                        Main.BlockedThisSession.Add((propGUID, userId, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss")));
                    wasForceHidden = true;
                    lastBlock = Time.time;
                    //__result = false;
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


        //public static IEnumerable<CodeInstruction> FilterPropTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        //{//Without this you will get a 'Cannot spawn prop .' message because my patch to OnGetPropVisibility results in forceBlock being True, and then
         //'AssetFilter.FilterProp' gets called from 'InstantiateSpawnableFromExistingPrefab' which then called 'GetPropFilterStatus' which if 'forceBlock' is True returns False with no message

            //void FilterProp(
            //string objectId,                                      // Index 0
            //string collectionId,                                  // Index 1
            //class [UnityEngine.CoreModule]
            //UnityEngine.GameObject prop,                          // Index 2
            //valuetype ABI_RC.Core.EventSystem.AssetManagement/PropTags tags, // Index 3
            //bool isLegacy,                                        // Index 4
            //bool isFriend,                                        // Index 5
            //bool isVisible,                                       // Index 6
            //bool forceShow,                                       // Index 7
            //bool forceBlock,                                      // Index 8
            //valuetype ABI_RC.Core.Networking.API.Responses.CompatibilityVersions compatibilityVersion // Index 9

            //var code = new List<CodeInstruction>(instructions);
            //Label continueCode = il.DefineLabel();
            //int insertionIndex = -1;
            //var items = code.Count;
            //Main.Logger.Msg("Items: " + items);

            //for (int i = 0; i < code.Count - 1; i++) // -1 since we will be checking i + 1
            //{
            //    if (code[i].opcode == OpCodes.Ldarg_S)
            //    {
            //        if (code[i].operand is byte argumentIndex && argumentIndex == 6)
            //        { //Inserts before the 'bool flag1 = !isVisible'
            //            insertionIndex = i;
            //            Main.Logger.Msg("----------------");
            //            Main.Logger.Msg("Index - " + insertionIndex);
            //            Main.Logger.Msg("----------------Debug no label");

            //            //code[i].labels.Add(continueCode);
            //            break;
            //        }
            //    }
            //}
            //if (items == 866 && insertionIndex == 97)
            //{
            //    var instructionsToInsert = new List<CodeInstruction>();
            //    instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_S, 8));
            //    instructionsToInsert.Add(new CodeInstruction(OpCodes.Brfalse, continueCode));
            //    instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldarg_2));//Load Prop Obj
            //    instructionsToInsert.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UnityEngine.Object), "Destroy", new Type[] { typeof(UnityEngine.Object) })));
            //    instructionsToInsert.Add(new CodeInstruction(OpCodes.Ret));

            //    Main.Logger.Msg("----------------Debug not inserting");
            //    //code.InsertRange(insertionIndex, instructionsToInsert);
            //}
            //else
            //    Main.Logger.Error($"Instruction count incorrect or Index is wrong! Not patching code. Instructions:{items} Index:{insertionIndex}\nPlease contact mod author!!!");

            //return code;
        //    return instructions;
        //}


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
                    var msg = $"Mod Blocking Prop:0 {Main.blockedProps[propGuid]}, SpawnedBy: Self";
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
