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
        public const string versionStr = "0.7.1";

        public static string tempString = "N/A";
        public static MelonPreferences_Category cat;
        private const string catagory = "IKpresets";
        //public static MelonPreferences_Entry<bool> saveWithEveryChange;
        public static MelonPreferences_Entry<bool> useNirvMiscPage;
        public static MelonPreferences_Entry<string> savedPrefs;
        public static MelonPreferences_Entry<string> savedPrefNames;
        public static MelonPreferences_Entry<string> savedAvatarPrefs;
        public static MelonPreferences_Entry<bool> autoLoadAvatarPresets;

        public static AvatarConfig config_Slots;
        public static AvatarConfig config_Avatars;

        public static string config_path = "UserData/IKpresets";
        public static string config_Slots_file = "ikpresets_slots.json";
        public static string config_Avatars_file = "ikpresets_avatars.json";

        public static string avatarGUID, avatarName;



        public class AvatarConfig
        {
            public Dictionary<string, AvatarSettings> Settings;

            public class AvatarSettings
            {
                public bool IKPitchYawShoulders = true;
                public bool IKPlantFeet = false;
                public bool IKHipPinned = true;
                public bool IKStraightenNeck = false;
                public bool IKHipShifting = true;
                public bool IKPreStraightenSpine = false;
                public int IKSpineRelaxIterations = 10;
                public int IKMaxSpineAngleFwd = 30;
                public int IKMaxSpineAngleBack = 30;
                public int IKMaxNeckAngleFwd = 30;
                public int IKMaxNeckAngleBack = 15;
                public int IKNeckPriority = 2;
                public int IKStraightSpineAngle = 15;
                public int IKStraightSpinePower = 2;
                public float IKTrackingSmoothing = 5f;
                public bool GeneralEnableRunningAnimationFullBody = false;
                public int GeneralPlayerHeight = -1;
                public int IKCalibrationMode = -1; //0 InPlace, 1 FollowHead, 2FullyFollowHead
                public string AvatarName = "N/A";
                public string SlotName = "N/A";
            }
        }

        public override void OnApplicationStart()
        {
            

            Logger = new MelonLogger.Instance("IKpresets", ConsoleColor.DarkRed);

            cat = MelonPreferences.CreateCategory(catagory, "IKpresets");
            //saveWithEveryChange = MelonPreferences.CreateEntry(catagory, nameof(saveWithEveryChange), true, "MelonPreferences.Save with every edit in EditMenu");
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page. (Restart req)");
            autoLoadAvatarPresets = MelonPreferences.CreateEntry(catagory, nameof(autoLoadAvatarPresets), true, "Auto load specific avatar presets");

            savedPrefs = MelonPreferences.CreateEntry(catagory, nameof(savedPrefs), "Migrated__null", "savedPrefs", "", true);
            savedPrefNames = MelonPreferences.CreateEntry(catagory, nameof(savedPrefNames), "Migrated__null", "savedSlotNames", "", true);
            savedAvatarPrefs = MelonPreferences.CreateEntry(catagory, nameof(savedAvatarPrefs), "Migrated__null", "savedPrefs", "", true);

            SaveSlots.LoadConfigSlots();
            SaveSlots.LoadConfigAvatars();

            if (!savedPrefs.Value.Contains("Migrated__"))
            {
                Logger.Msg(ConsoleColor.Magenta, "Starting data migration from 0.5.x to 0.6.x format for slots");
                SaveSlots.MigrateOldDataSlots();
            }
            if (!savedAvatarPrefs.Value.Contains("Migrated__"))
            {
                Logger.Msg(ConsoleColor.Magenta, "Starting data migration from 0.5.x to 0.6.x format for avatars");
                SaveSlots.MigrateOldDataAvatars();
            }

            BTKUI_Cust.SetupUI();



        }

        //I didn't realize that just getting an avatar name would be so complicated....
        private static readonly Dictionary<string, string> AvatarsNamesCache = new Dictionary<string, string>();
        public static void OnAvatarDetailsReceived(string guid, string name)
        {
            //Logger.Msg($"{guid} - {name}");
            AvatarsNamesCache[guid] = name;
            avatarGUID = guid;
            avatarName = name;
            if (autoLoadAvatarPresets.Value)
            {
                if (config_Avatars.Settings.ContainsKey(guid))
                {
                    Logger.Msg($"Auto loading IK settings for:{name} - {guid}");
                    SaveSlots.LoadAvatars(guid);
                    BTKUI_Cust.SaveChanges();
                }
            }
        }

        internal static async void OnAnimatorManagerUpdate(CVRAnimatorManager animatorManager)
        {
            //Logger.Msg($"OnAnimatorManagerUpdate");
            var avatarGuid = MetaPort.Instance.currentAvatarGuid;
            string avatarName = null;
            if (AvatarsNamesCache.ContainsKey(avatarGuid)) avatarName = AvatarsNamesCache[avatarGuid];
            if (avatarName == null)
            {
                avatarName = Utils.ReturnCleanASCII(await ApiRequests.RequestAvatarDetailsPageTask(avatarGuid));
            }
            //Logger.Msg($"OnAnimatorManagerUpdate - {avatarName}");
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
                Main.Logger.Error($"[API] Fetching avatar {guid} name has Failed!");
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
    }
    //https://github.com/kafeijao/Kafe_CVR_Mods/blob/6e2b44b2ed3db22d21096ca53177be3a298a4f46/OSC/HarmonyPatches.cs#L24
    [HarmonyPatch]
    internal class HarmonyPatches
    {
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

