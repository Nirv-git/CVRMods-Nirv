using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core;
using ABI_RC.Core.Networking.IO.UserGeneratedContent;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util;
using ABI_RC.Systems.MovementSystem;
using ABI.CCK.Components;
using HarmonyLib;
using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Networking.API.Responses;



[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(IKpresetsMod.Main), "IKpresets", IKpresetsMod.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(IKpresetsMod.Main.versionStr)]
[assembly: AssemblyFileVersion(IKpresetsMod.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Cyan)]

namespace IKpresetsMod
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.5";

        public static string tempString = "N/A";
        public static MelonPreferences_Category cat;
        private const string catagory = "IKpresets";
        //public static MelonPreferences_Entry<bool> saveWithEveryChange;
        public static MelonPreferences_Entry<string> savedPrefs;
        public static MelonPreferences_Entry<string> savedPrefNames;
        public static MelonPreferences_Entry<string> savedAvatarPrefs;
        public static MelonPreferences_Entry<bool> autoLoadAvatarPresets;

        public static string avatarGUID, avatarName;


        public static class Config
        {   //Default settings
            static public int IKCalibrationMode = 0; //0 InPlace, 1 FollowHead, 2FullyFollowHead
            static public bool IKPitchYawShoulders = true;
            static public bool IKPlantFeet = false;
            static public bool IKHipPinned = true;
            static public bool IKStraightenNeck = false;
            static public bool IKHipShifting = true;
            static public bool IKPreStraightenSpine = false;
            static public int IKSpineRelaxIterations = 10;
            static public int IKMaxSpineAngleFwd = 30;
            static public int IKMaxSpineAngleBack = 30;
            static public int IKMaxNeckAngleFwd = 30;
            static public int IKMaxNeckAngleBack = 15;
            static public int IKNeckPriority = 2;
            static public int IKStraightSpineAngle = 15;
            static public int IKStraightSpinePower = 2;
            static public float IKTrackingSmoothing = 5f;
            static public bool GeneralEnableRunningAnimationFullBody = false;
        }

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("IKpresets", ConsoleColor.DarkRed);

            cat = MelonPreferences.CreateCategory(catagory, "IKpresets");
            //saveWithEveryChange = MelonPreferences.CreateEntry(catagory, nameof(saveWithEveryChange), true, "MelonPreferences.Save with every edit in EditMenu");
            savedPrefs = MelonPreferences.CreateEntry("IKTpresets", nameof(savedPrefs), "1,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;2,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;3,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;4,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;5,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;6,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;7,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;8,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;9,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;10,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;11,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;12,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;13,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;14,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;15,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;16,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false", "savedPrefs", "", true);
            savedPrefNames = MelonPreferences.CreateEntry("IKTpresetsNames", nameof(savedPrefNames), "1,N/A;2,N/A;3,N/A;4,N/A;5,N/A;6,N/A;7,N/A;8,N/A;9,N/A;10,N/A;11,N/A;12,N/A;13,N/A;14,N/A;15,N/A;16,N/A", "savedSlotNames", "", true);

            autoLoadAvatarPresets = MelonPreferences.CreateEntry("IKTpresets", nameof(autoLoadAvatarPresets), true, "Auto load specific avatar presets");
            savedAvatarPrefs = MelonPreferences.CreateEntry("IKTpresets", nameof(savedAvatarPrefs), "", "savedPrefs", "", true);

            BTKUI_Cust.SetupUI();



        }

        //I didn't realize that just getting an avatar name would be so complicated....
        private static readonly Dictionary<string, string> AvatarsNamesCache = new Dictionary<string, string>();
        public static void OnAvatarDetailsReceived(string guid, string name)
        {
            Logger.Msg($"{guid} - {name}");
            AvatarsNamesCache[guid] = name;
            avatarGUID = guid;
            avatarName = name;
            if (autoLoadAvatarPresets.Value)
            {
                if (SaveSlots.AvatarGetSaved().ContainsKey(guid))
                {
                    Logger.Msg($"Auto loading IK settings for:{name} - {guid}");
                    SaveSlots.AvatarLoadSlot(guid);
                    BTKUI_Cust.SaveChanges();
                }
            }
        }

        internal static async void OnAnimatorManagerUpdate(CVRAnimatorManager animatorManager)
        {
            Logger.Msg($"OnAnimatorManagerUpdate");
            var avatarGuid = MetaPort.Instance.currentAvatarGuid;
            string avatarName = null;
            if (AvatarsNamesCache.ContainsKey(avatarGuid)) avatarName = AvatarsNamesCache[avatarGuid];
            if (avatarName == null)
            {
                avatarName = await ApiRequests.RequestAvatarDetailsPageTask(avatarGuid);
            }
            Logger.Msg($"OnAnimatorManagerUpdate - {avatarName}");
            Main.OnAvatarDetailsReceived(avatarGuid, avatarName);
        }
    }

    //https://github.com/kafeijao/Kafe_CVR_Mods/blob/6e2b44b2ed3db22d21096ca53177be3a298a4f46/OSC/Utils/ApiRequests.cs#L7
    internal static class ApiRequests
    {
        internal static async System.Threading.Tasks.Task<string> RequestAvatarDetailsPageTask(string guid)
        {
            Main.Logger.Msg($"[API] Fetching avatar {guid} name...");
            BaseResponse<AvatarDetailsResponse> response;
            try
            {
                var payload = new { avatarID = guid };
                response = await ApiConnection.MakeRequest<AvatarDetailsResponse>(ApiConnection.ApiOperation.AvatarDetail, payload);
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"[API] Fetching avatar {guid} name has Failed! Location: OSC.Utils.ApiRequests.cs");
                Main.Logger.Error(ex);
                return null;
            }
            if (response == null)
            {
                Main.Logger.Msg($"[API] Fetching avatar {guid} name has Failed! Response came back empty.");
                return null;
            }
            Main.Logger.Msg($"[API] Fetched avatar {guid} name successfully! Name: {response.Data.Name}");
            return response.Data.Name;
        }
        //https://github.com/kafeijao/Kafe_CVR_Mods/blob/6e2b44b2ed3db22d21096ca53177be3a298a4f46/OSC/HarmonyPatches.cs#L24
        [HarmonyPatch]
        internal class HarmonyPatches
        {
            // Avatar
            [HarmonyPrefix]
            [HarmonyPatch(typeof(AvatarDetails_t), "Recycle")]
            internal static void BeforeAvatarDetailsRecycle(AvatarDetails_t __instance)
            {
                Main.Logger.Msg($"9-0");

                Main.OnAvatarDetailsReceived(__instance.AvatarId, __instance.AvatarName);
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(MovementSystem), nameof(MovementSystem.UpdateAnimatorManager))]
            internal static void AfterUpdateAnimatorManager(CVRAnimatorManager manager)
            {
                Main.Logger.Msg($"9-1");

                Main.OnAnimatorManagerUpdate(manager);
            }
        }
    }
}

