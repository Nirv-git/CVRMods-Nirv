using UIExpansionKit.API;
using MelonLoader;
using UnityEngine;
using ConsoleColor = System.ConsoleColor;
using System.Linq;
using System.Net;
using System;
using System.Collections.Generic;
using System.Reflection;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using HarmonyLib;
using ABI_RC.Core.Savior;

[assembly: MelonInfo(typeof(NearClipPlaneAdj.Main), "NearClipPlaneAdj", NearClipPlaneAdj.Main.versionStr, "Nirvash")]
[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonOptionalDependencies("BTKUILib")]

namespace NearClipPlaneAdj
{
    public class Main : MelonMod
    {
        public const string versionStr = "0.5";
        public static MelonLogger.Instance Logger;

        public static MelonPreferences_Entry<bool> changeClipOnLoad;
        public static MelonPreferences_Entry<bool> keybindsEnabled;
        public static MelonPreferences_Entry<bool> smallerDefault;
        public static MelonPreferences_Entry<bool> raiseNearClip;
        //public static MelonPreferences_Entry<bool> changeUIcam;
        public static MelonPreferences_Entry<bool> BTKUILib_en;
        public static MelonPreferences_Entry<bool> defaultChangeBlackList;

        public static Dictionary<string, System.Tuple<bool, string>> blackList; 
        public static float oldNearClip;
        public static float lastSetNearClip;
        //public static Camera UIcamera;

        public static bool firstload = true;

        public static MelonPreferences_Entry<bool> debug;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("NearClipPlaneAdj", ConsoleColor.DarkYellow);

            MelonPreferences.CreateCategory("NearClipAdj", "NearClipPlane Adjuster");
            changeClipOnLoad = MelonPreferences.CreateEntry<bool>("NearClipAdj", "changeClipOnLoad", true, "Change NearClip on world load");
            keybindsEnabled = MelonPreferences.CreateEntry<bool>("NearClipAdj", "Keyboard", true, "Keyboard Shortcuts: '[' - 0.0001, ']' - 0.05");
            smallerDefault = MelonPreferences.CreateEntry<bool>("NearClipAdj", "SmallerDefault", false, "Smaller Default Nearclip on World Change - 0.001 vs 0.01");
            raiseNearClip = MelonPreferences.CreateEntry<bool>("NearClipAdj", "RaiseOnQuickMenu", false, "If using smaller Default Nearclip (0.001) raise to 0.01 when Quick Menu opens.");
            BTKUILib_en = MelonPreferences.CreateEntry<bool>("NearClipAdj", "BTKUILib_en", true, "BTKUILib Support (Requires Restart)");
            defaultChangeBlackList = MelonPreferences.CreateEntry("NearClipAdj", "defaultChangeBlackList", true, "Check a blacklist for worlds to not auto change the NearClip on (Restart Required to Enable)");

            debug = MelonPreferences.CreateEntry<bool>("NearClipAdj", "debug", false, "debug");

            var settings = ExpansionKitApi.GetSettingsCategory("NearClipAdj");

            settings.AddSimpleButton("Set Nearplane to 0.05", (() => ChangeNearClipPlane(.05f, true)));
            settings.AddSimpleButton("Set Nearplane to 0.01", (() => ChangeNearClipPlane(.01f, true)));
            settings.AddSimpleButton("Set Nearplane to 0.001", (() => ChangeNearClipPlane(.001f, true)));
            settings.AddSimpleButton("Set Nearplane to 0.0001", (() => ChangeNearClipPlane(.0001f, true)));


            if (MelonHandler.Mods.Any(m => m.Info.Name == "BTKUILib") && BTKUILib_en.Value)
            {
                CustomBTKUI.InitUi();
            }
            else Logger.Msg("BTKUILib is missing, or setting is toggled off in Mod Settings - Not adding controls to BTKUILib");

            if (defaultChangeBlackList.Value) GetBlackList();
            MelonCoroutines.Start(SampleNearClip());
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

        public override void OnPreferencesSaved()
        {
           
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Logger.Msg($"name: {sceneName}, index: {buildIndex}");
            switch (buildIndex)//Without switch this would run 3 times at world load
            {
                case 0: //Prep
                    break;
                case 1: //Login
                    break;
                case 2: //Init
                    break;
                case 3: //HQ
                    break;
                default:

                    if (firstload)
                    {
                        HarmonyInstance.Patch(typeof(CVR_MenuManager).GetMethod(nameof(CVR_MenuManager.ToggleQuickMenu)), null, new HarmonyMethod(typeof(Main).GetMethod(nameof(QMtoggle), BindingFlags.NonPublic | BindingFlags.Static)));
                        //Logger.Msg("default" + buildIndex);
                        firstload = false;
                    }
                    if (changeClipOnLoad.Value) MelonCoroutines.Start(SetNearClipPlane(0.01f));
                    break;
            }

        }


        private static void QMtoggle(bool __0)
        {//Will send false when Big menu opens, check for bugs related
            if (debug.Value) Logger.Msg($"QMtoggle: {__0}");
            if (!raiseNearClip.Value) return;
            if (oldNearClip == 0 ) return;

            var screenCamera = GetScreenCam();
            if (screenCamera is null) return;

            if (__0)
            {
                float clipValue = screenCamera.nearClipPlane;
                if (clipValue < .0011f && clipValue > .0009f)
                    ChangeNearClipPlane(.01f, false);
                oldNearClip = clipValue;
            }
            else
            {
                float clipValue = screenCamera.nearClipPlane;
                if (clipValue != oldNearClip)
                    ChangeNearClipPlane(oldNearClip, false);
            }
        }


        private static Camera GetScreenCam()
        {
            Camera screenCamera = PlayerSetup.Instance.GetActiveCamera().GetComponent<Camera>();
            return screenCamera;
        }

        public static void ChangeNearClipPlane(float value, bool printMsg)
        {
            var screenCamera = GetScreenCam();
            if (screenCamera is null) return;
            float oldvalue = screenCamera.nearClipPlane;
            screenCamera.nearClipPlane = value;
            if (printMsg) Logger.Msg($"Nearplane chnaged. Old: {oldvalue}, New: {value} {(keybindsEnabled.Value ? "- Keyboard Hotkeys: '[' - 0.0001, ']' - 0.05" : "")}");
            //ChangePhotoCameraNearField(value);
            oldNearClip = screenCamera.nearClipPlane;

            //try
            //{
            //    if (changeUIcam.Value)
            //            UIcamera.nearClipPlane = value;
            //}
            //catch (System.Exception ex) { Logger.Error($"Error changing UI Camera nearclip\n" + ex.ToString()); }
        }


       
        
        System.Collections.IEnumerator SetNearClipPlane(float znear)
        {
            printPlane();

            yield return new WaitForSecondsRealtime(15); //Wait 15 seconds after world load before setting the clipping value. Waiting for the next/first frame does not work
            if (defaultChangeBlackList.Value && MetaPort.Instance.CurrentWorldId != null && blackList != null)
            { //Check if world is blacklisted from auto change
                var worldID = MetaPort.Instance.CurrentWorldId;
                
                if (debug.Value)
                {
                    Logger.Msg($"ID IS {worldID}");
                    foreach (var worldEnt in blackList)
                    {
                        Logger.Msg($"World {worldEnt.Key}, item1 {worldEnt.Value.Item1} item2 {worldEnt.Value.Item2}");
                    }
                }
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
            if (!keybindsEnabled.Value) return;

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


        System.Collections.IEnumerator SampleNearClip()
        {
            while (true) 
            {
                try
                {
                    if (debug.Value)
                    {
                        printPlane();
                    }
                }
                catch { }
                yield return new WaitForSecondsRealtime(2);
            }
        }

        public static void printPlane()
        {
            var screenCamera = GetScreenCam();
            if (screenCamera is null) return;
            float value = screenCamera.nearClipPlane;
            Logger.Msg($"Near plane cur: {value}");

        }



    }
}
