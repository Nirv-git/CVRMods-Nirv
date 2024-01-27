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
using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Networking.API.Responses;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(ViewPointTweaks.Main), "ViewPointTweaks", ViewPointTweaks.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(ViewPointTweaks.Main.versionStr)]
[assembly: AssemblyFileVersion(ViewPointTweaks.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Green)]

namespace ViewPointTweaks
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.7.5";

        public static MelonPreferences_Category cat;
        private const string catagory = "ViewPointTweaks";
        public static MelonPreferences_Entry<bool> useNirvMiscPage;
        public static MelonPreferences_Entry<string> savedAvatarPrefs;
        public static MelonPreferences_Entry<bool> autoLoadAvatarPresets;

        public static bool init = false;
        public static Transform VRcam, VRheadPoint;

        public static float currentScaleAdjustment = 0;
        public static Vector3 currentPosAdjustment = Vector3.zero;
        public static float currentRotAdjustment = 0;
        public static float lastScaleAdjustment = 0;
        public static Vector3 lastPosAdjustment = Vector3.zero;
        public static float lastRotAdjustment = 0;


        private static readonly Dictionary<string, string> AvatarsNamesCache = new Dictionary<string, string>();
        public static string avatarGUID, avatarName;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("ViewPointTweaks", ConsoleColor.Green);

            cat = MelonPreferences.CreateCategory(catagory, "ViewPointTweaks"); 
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page.");
            autoLoadAvatarPresets = MelonPreferences.CreateEntry(catagory, nameof(autoLoadAvatarPresets), false, "Auto load specific avatar presets");
            savedAvatarPrefs = MelonPreferences.CreateEntry(catagory, nameof(savedAvatarPrefs), "", "savedPrefs", "", true);
            CustomBTKUI.InitUi();    
        }

        public static void SetupPoints()
        {
            if (MetaPort.Instance.isUsingVr)
            {
                VRcam = GameObject.Find("_PLAYERLOCAL/[CameraRigVR]/[Offset] User PlaySpace/[Offset] Seated Play/Camera/").transform;
                VRheadPoint = GameObject.Find("_PLAYERLOCAL/[CameraRigVR]/[Offset] User PlaySpace/[Offset] Seated Play/Camera/VRTargetHead").transform;
            }
        }

        public static void ChangeOffsets(float scale, Vector3 position, float rot = 0f)
        {
            SetupPoints();
            if (VRcam == null || VRheadPoint == null) return;
            if (scale != 0)
            {
                var vrHeadPos = VRheadPoint.position;
                var newScale = VRcam.localScale + new Vector3(scale, scale, scale);
                if (newScale.z > 0f)
                {
                    currentScaleAdjustment += scale;
                    VRcam.localScale = newScale;
                    VRheadPoint.position = vrHeadPos;
                }
            }
            if (position != Vector3.zero)
            {
                var curOff = currentScaleAdjustment;
                if (curOff != 0)
                    ChangeOffsets(-curOff, Vector2.zero);
                //Undo scaling before moving position, kinda hacky
                currentPosAdjustment += position;
                VRheadPoint.localPosition += position;
                if (curOff != 0)
                    ChangeOffsets(curOff, Vector2.zero);
            }

            if (rot != 0)
            {
                currentRotAdjustment += rot;
                VRheadPoint.transform.Rotate(Vector3.right, rot);
            }
        }

        public static void ResetOffsets()
        {
            SetupPoints();
            if (VRcam == null || VRheadPoint == null) return;
            lastScaleAdjustment = currentScaleAdjustment;
            lastPosAdjustment = currentPosAdjustment;
            lastRotAdjustment = currentRotAdjustment;


            if (currentScaleAdjustment != 0)
            {
                var vrHeadPos = VRheadPoint.position;
                var newScale = VRcam.localScale - new Vector3(currentScaleAdjustment, currentScaleAdjustment, currentScaleAdjustment);
                currentScaleAdjustment -= currentScaleAdjustment; //Should be 0
                VRcam.localScale = newScale;
                VRheadPoint.position = vrHeadPos;
            }
            if (currentPosAdjustment != Vector3.zero)
            {
                
                VRheadPoint.localPosition -= currentPosAdjustment;
                currentPosAdjustment -= currentPosAdjustment;
            }
            if (currentRotAdjustment != 0)
            {
                VRheadPoint.transform.Rotate(Vector3.right, -currentRotAdjustment);
                currentRotAdjustment -= currentRotAdjustment;

            }

            if(Main.currentScaleAdjustment != 0 || Main.currentPosAdjustment != Vector3.zero || Main.currentRotAdjustment != 0)
            {
                Logger.Error("Item not reset, report to mod creator" +
                    $"Offset: X:{Main.currentPosAdjustment.x.ToString("F3").TrimEnd('0')} Y:{Main.currentPosAdjustment.y.ToString("F3").TrimEnd('0')}" +
                    $" Z:{(Main.currentPosAdjustment.z.ToString("F3").TrimEnd('0'))}<p>Rot: {(Main.currentRotAdjustment.ToString("F3").TrimEnd('0'))} Scale: {(Main.currentScaleAdjustment.ToString("F3").TrimEnd('0'))}");
            }
        }


        public static void OnAvatarDetailsReceived(string guid, string name)
        {
            //Logger.Msg($"{guid} - {name}");
            SetupPoints();
            AvatarsNamesCache[guid] = name;
            avatarGUID = guid;
            avatarName = Utils.ReturnCleanASCII(name);

           Main.Logger.Msg($"Avatar changed, Pos X:{VRheadPoint.localPosition.x} Y:{VRheadPoint.localPosition.y} Z:{VRheadPoint.localPosition.z} Scale:{VRcam.localScale} ");

            if (autoLoadAvatarPresets.Value)
            {
                if (SaveSlots.AvatarGetSaved().ContainsKey(guid))
                {
                    Logger.Msg($"Auto loading view position settings for:{name} - {guid}");
                    SaveSlots.AvatarLoadSlot(guid);
                }
            }
        }

        internal static async void OnSetupAvatarGeneral()
        {
            //Logger.Msg($"OnAnimatorManagerUpdate");
            var avatarGuid = MetaPort.Instance.currentAvatarGuid;
            string avatarName = null;
            if (AvatarsNamesCache.ContainsKey(avatarGuid)) avatarName = AvatarsNamesCache[avatarGuid];
            if (avatarName == null)
            {
                avatarName = await ApiRequests.RequestAvatarDetailsPageTask(avatarGuid);
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
}



