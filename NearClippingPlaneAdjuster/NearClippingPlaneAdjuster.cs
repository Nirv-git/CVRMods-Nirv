using UIExpansionKit.API;
using MelonLoader;
using UnityEngine;
using ConsoleColor = System.ConsoleColor;
using System.Linq;
using System.Net;
using System;
using System.Collections.Generic;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI.CCK.Components;
using HarmonyLib;


[assembly: MelonInfo(typeof(NearClipPlaneAdj.Main), "NearClipPlaneAdj", NearClipPlaneAdj.Main.versionStr, "Nirvash")]
[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonOptionalDependencies("BTKUILib")]

namespace NearClipPlaneAdj
{
    public class Main : MelonMod
    {
        public const string versionStr = "0.7.8";
        public static MelonLogger.Instance Logger;

        public static MelonPreferences_Entry<bool> useNirvMiscPage;
        public static MelonPreferences_Entry<bool> changeClipOnLoad;
        public static MelonPreferences_Entry<bool> keybindsEnabled;
        public static MelonPreferences_Entry<bool> smallerDefault;
        public static MelonPreferences_Entry<bool> defaultChangeBlackList;
        public static MelonPreferences_Entry<bool> BTKUILib_en;
        public static MelonPreferences_Entry<bool> replace05withNumpad;
        public static MelonPreferences_Entry<bool> farClipControl;

        public static Dictionary<string, System.Tuple<bool, string>> blackList; 
        public static float oldNearClip;
        public static float lastSetNearClip;

        public static MelonPreferences_Entry<bool> debug;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("NearClipPlaneAdj", ConsoleColor.DarkYellow);

            MelonPreferences.CreateCategory("NearClipAdj", "NearClipPlane Adjuster");
            useNirvMiscPage = MelonPreferences.CreateEntry("NearClipAdj", nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page. (Restart req)");
            changeClipOnLoad = MelonPreferences.CreateEntry<bool>("NearClipAdj", "changeClipOnLoad", true, "Change NearClip on world load");
            keybindsEnabled = MelonPreferences.CreateEntry<bool>("NearClipAdj", "Keyboard", true, "Keyboard Shortcuts: '[' - 0.0001, ']' - 0.05");
            smallerDefault = MelonPreferences.CreateEntry<bool>("NearClipAdj", "SmallerDefault", false, "Smaller Default Nearclip on World Change - 0.001 vs 0.01");
            defaultChangeBlackList = MelonPreferences.CreateEntry("NearClipAdj", "defaultChangeBlackList", true, "Check a blacklist for worlds to not auto change the NearClip on (Restart Required to Enable)");
            BTKUILib_en = MelonPreferences.CreateEntry<bool>("NearClipAdj", "BTKUILib_en", true, "BTKUILib Support (Requires Restart)");
            replace05withNumpad = MelonPreferences.CreateEntry<bool>("NearClipAdj", "replace05withNumpad", false, "BTKUI Replace .05 button with numberpad");
            farClipControl = MelonPreferences.CreateEntry<bool>("NearClipAdj", "farClipControl", false, "Add controls for Far Clipping Plane");
            //debug = MelonPreferences.CreateEntry<bool>("NearClipAdj", "debug", false, "debug");

            var settings = ExpansionKitApi.GetSettingsCategory("NearClipAdj");
            settings.AddSimpleButton("Set Nearplane to 0.05", (() => ChangeNearClipPlane(.05f, true)));
            settings.AddSimpleButton("Set Nearplane to 0.01", (() => ChangeNearClipPlane(.01f, true)));
            settings.AddSimpleButton("Set Nearplane to 0.001", (() => ChangeNearClipPlane(.001f, true)));
            settings.AddSimpleButton("Set Nearplane to 0.0001", (() => ChangeNearClipPlane(.0001f, true)));


            if (MelonHandler.Mods.Any(m => m.Info.Name == "BTKUILib") && BTKUILib_en.Value)
            {
                CustomBTKUI.InitUi();
                replace05withNumpad.OnValueChanged += CustomBTKUI.ChangeButtons;
                farClipControl.OnValueChanged += CustomBTKUI.ChangeButtons;
            }
            else Logger.Msg("BTKUILib is missing, or setting is toggled off in Mod Settings - Not adding controls to BTKUILib");

            if (defaultChangeBlackList.Value) GetBlackList();
            //MelonCoroutines.Start(SampleNearClip()); //debug
        }
        private void GetBlackList()
        {
            try
            {
                blackList = new Dictionary<string, System.Tuple<bool, string>>();
                string url = "https://raw.githubusercontent.com/Nirv-git/CVRMods-Nirv/main/NearClippingPlaneAdjuster/blacklist";
                WebClient client = new WebClient();
                var temp = client.DownloadString(url);
                using (System.IO.StringReader reader = new System.IO.StringReader(temp))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var sub = line.Split(',');
                        if(sub.Length == 3) blackList.Add(sub[0], new System.Tuple<bool, string>(bool.Parse(sub[1]), sub[2]));
                    }
                }
            }
            catch (Exception ex) { Logger.Error($"GetBlackList error\n" + ex.ToString()); }
        }

        //public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        //{
        //    //Logger.Msg($"name: {sceneName}, index: {buildIndex}");
        //    switch (buildIndex)
        //    {
        //        case 0: //Prep
        //            break;
        //        case 1: //Login
        //            break;
        //        case 2: //Init
        //            break;
        //        case 3: //HQ
        //            break;
        //        default:
        //            if (changeClipOnLoad.Value) MelonCoroutines.Start(SetNearClipPlane(0.01f));
        //            break;
        //    }
        //}

        private static Camera GetScreenCam()
        {
            Camera screenCamera = PlayerSetup.Instance.GetActiveCamera().GetComponent<Camera>();
            return screenCamera;
        }

        public static float ValidateNewNearClipPlane(float value)
        {
            var screenCamera = GetScreenCam();
            if (screenCamera is null) return -1;
            return ((screenCamera.farClipPlane / value) > 10000000.0) ? screenCamera.farClipPlane / 10000000f : value;
        }

        public static void ChangeNearClipPlane(float value, bool printMsg)
        {
            var screenCamera = GetScreenCam();
            if (screenCamera is null) return;
            float oldvalue = screenCamera.nearClipPlane;
            var valValue = ValidateNewNearClipPlane(value);
            if (valValue == -1) return;
            if (valValue != value) Main.Logger.Msg($"New value: {value} exceeds ratio between Far and Near clips, limiting Near to {valValue}");
            screenCamera.nearClipPlane = valValue;
            if (printMsg) Logger.Msg($"New: {valValue}, Old: {oldvalue} {(keybindsEnabled.Value ? "- Keyboard Hotkeys: '[' - 0.0001, ']' - 0.05" : "")}");
            oldNearClip = screenCamera.nearClipPlane;
        }

        public static float ValidateNewFarClipPlane(float value)
        {
            var screenCamera = GetScreenCam();
            if (screenCamera is null) return -1;
            return ((value / screenCamera.nearClipPlane) > 10000000.0) ? screenCamera.nearClipPlane * 10000000f : value;
        }

        public static void ChangeFarClipPlane(float value, bool printMsg)
        {
            var screenCamera = GetScreenCam();
            if (screenCamera is null) return;
            float oldvalue = screenCamera.farClipPlane;
            var valValue = ValidateNewFarClipPlane(value);
            if (valValue == -1) return;
            if (valValue != value) Main.Logger.Msg($"New value: {value} exceeds ratio between Far and Near clips, limiting Far to {valValue}");
            screenCamera.farClipPlane = valValue;
            if (printMsg) Logger.Msg(ConsoleColor.DarkYellow, $"Farclip changed - New: {valValue}, Old: {oldvalue}");
        }

        public static System.Collections.IEnumerator SetNearClipPlane(float znear)
        {
            //printPlane(); //debug

            yield return new WaitForSecondsRealtime(2); 
            if (defaultChangeBlackList.Value && MetaPort.Instance.CurrentWorldId != null && blackList != null)
            { //Check if world is blacklisted from auto change
                var worldID = MetaPort.Instance.CurrentWorldId;
                
                //if (debug.Value)
                //{
                //    Logger.Msg($"ID IS {worldID}");
                //    foreach (var worldEnt in blackList)
                //    {
                //        Logger.Msg($"World {worldEnt.Key}, item1 {worldEnt.Value.Item1} item2 {worldEnt.Value.Item2}");
                //    }
                //}
                if (blackList.TryGetValue(worldID, out var world))
                {
                    if (world.Item1)
                    {
                        Logger.Msg(ConsoleColor.Yellow, $"Not auto adjusting NearClipping plane, world blacklisted for 0.01 and lower values. \nDebug-{worldID}, {world.Item2}, {world.Item1}");
                        yield break;
                    }
                    else if(smallerDefault.Value)
                    {
                        Logger.Msg(ConsoleColor.Yellow, $"Not auto adjusting NearClipping plane, world blacklisted for 0.001 value. \nDebug-{worldID}, {world.Item2}, {world.Item1}");
                        yield break;
                    }
                }
            }
            
            if (smallerDefault.Value) znear = 0.001f;
            ChangeNearClipPlane(znear, true);
            oldNearClip = znear;
            Logger.Msg("Near plane adjusted after world load");
        }


        public override void OnUpdate()
        {
            if (!keybindsEnabled?.Value ?? true) return;

            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                Logger.Msg(ConsoleColor.DarkCyan, "NearClip set to smallest value on keypress: 0.0001");
                ChangeNearClipPlane(.0001f, true);
            }
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                Logger.Msg(ConsoleColor.DarkCyan, "NearClip set to largest value on keypress: 0.05");
                ChangeNearClipPlane(.05f, true);
            }
        }

        //System.Collections.IEnumerator SampleNearClip()
        //{
        //    while (true) 
        //    {
        //        try
        //        {
        //            if (debug.Value)
        //            {
        //                printPlane();
        //            }
        //        }
        //        catch { }
        //        yield return new WaitForSecondsRealtime(2);
        //    }
        //}

        //public static void printPlane()
        //{
        //    var screenCamera = GetScreenCam();
        //    if (screenCamera is null) return;
        //    float value = screenCamera.nearClipPlane;
        //    Logger.Msg($"Near plane cur: {value}");
        //}
    }

    [HarmonyPatch]
    internal class HarmonyPatches
    {
        // Avatar
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ABI.CCK.Components.CVRWorld), nameof(CVRWorld.CopyRefCamValues))]
        internal static void AfterSetupAvatarGeneral()
        {
            //Main.Logger.Msg($"9-1");
            MelonCoroutines.Start(Main.SetNearClipPlane(0.01f));
        }
    }
}
