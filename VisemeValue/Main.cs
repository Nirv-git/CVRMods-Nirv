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

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(VisemeValue.Main), "VisemeValue", VisemeValue.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(VisemeValue.Main.versionStr)]
[assembly: AssemblyFileVersion(VisemeValue.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.DarkGreen)]

namespace VisemeValue
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "1.1";

        public static MelonPreferences_Category cat;
        private const string catagory = "VisemeValue";
        public static MelonPreferences_Entry<bool> modEnabled;
        public static MelonPreferences_Entry<int> updateRate;
        public static MelonPreferences_Entry<bool> driveValuePref;
        public static MelonPreferences_Entry<bool> driveVisemePref;
        public static MelonPreferences_Entry<bool> driveIntensityPref;
        public static MelonPreferences_Entry<bool> driveIndivPref;
        public static MelonPreferences_Entry<bool> spacer;
        public static MelonPreferences_Entry<bool> info;

        public static bool init = false;
        public static object updateRoutine; 

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("VisemeValue", ConsoleColor.Green);

            cat = MelonPreferences.CreateCategory(catagory, "VisemeValue");
            modEnabled = MelonPreferences.CreateEntry(catagory, nameof(modEnabled), true, "Attempt to Drive Viseme Parameters if they exists");
            updateRate = MelonPreferences.CreateEntry(catagory, nameof(updateRate), 10, "Update Rate (ms)");
            driveValuePref = MelonPreferences.CreateEntry(catagory, nameof(driveValuePref), true, "Use 'VisemeMod_Value' (int) for current Viseme");
            driveVisemePref = MelonPreferences.CreateEntry(catagory, nameof(driveVisemePref), false, "Use 'Viseme' (int) for current Viseme");
            driveIntensityPref = MelonPreferences.CreateEntry(catagory, nameof(driveIntensityPref), true, "Use 'VisemeMod_Level' (float) for current Intensity");
            driveIndivPref = MelonPreferences.CreateEntry(catagory, nameof(driveIndivPref), false, "Individual Parameters 'VisemeMod_xx' (float) for all possible visems 0-14 (sil must exist)");
            spacer = MelonPreferences.CreateEntry(catagory, nameof(spacer), false, "--Info--");
            info = MelonPreferences.CreateEntry(catagory, nameof(info), false, "Info: Avatar needs a Face Mesh defined and Use Lip Sync checked. It doesn't need any visemes selected");


            modEnabled.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) =>{ AvatarReady(); });
            driveValuePref.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { AvatarReady(); });
            driveVisemePref.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { AvatarReady(); });
            driveIntensityPref.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { AvatarReady(); });
            driveIndivPref.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { AvatarReady(); });
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            //Logger.Msg($"name: {sceneName}, index: {buildIndex}");
            switch (buildIndex)
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
                    if (!init) init = true;
                    break;
            }
        }

        public static Dictionary<int, string> visemes = new Dictionary<int, string>
        {
            { 0, "sil" },
            { 1, "pp" },
            { 2, "ff" },
            { 3, "th" },
            { 4, "dd" },
            { 5, "kk" },
            { 6, "ch" },
            { 7, "ss" },
            { 8, "nn" },
            { 9, "rr" },
            { 10, "aa" },
            { 11, "e" },
            { 12, "i" },
            { 13, "o" },
            { 14, "u" }
        };

        public static IEnumerator SetValues()
        {
            var driveValue = false;
            var driveViseme = false;
            var driveLevel = false;
            var driveIndiv = false;
            if (!init)
            {
                while (!init)
                { //Wait for game to load for the first time
                    yield return new WaitForSeconds(1f);
                }
            }
            else //Slight delay to make sure avatar init
                yield return new WaitForSeconds(2f);

            //foreach (AnimatorControllerParameter param in PlayerSetup.Instance.animatorManager.animator.parameters)
            //{
            //    Logger.Msg($"{param.name}");
            //}

            //if (Utils.ContainsParam(LocalPlayerAnimatorManager.animator, "VisemeMod_Value"))
            if (PlayerSetup.Instance.animatorManager.animatorParameterNameHashes.ContainsKey("VisemeMod_Value") && driveValuePref.Value)
                driveValue = true;
            if (PlayerSetup.Instance.animatorManager.animatorParameterNameHashes.ContainsKey("Viseme") && driveVisemePref.Value)
                driveViseme = true;
            if (PlayerSetup.Instance.animatorManager.animatorParameterNameHashes.ContainsKey("VisemeMod_Level") && driveIntensityPref.Value)
                driveLevel = true;
            if (PlayerSetup.Instance.animatorManager.animatorParameterNameHashes.ContainsKey("VisemeMod_sil") && driveIndivPref.Value) //.Any(item => item.Key.StartsWith("VisemeMod"));
                driveIndiv = true;
            if (!driveValue && !driveViseme && !driveLevel && !driveIndiv)
                yield break;
         
            var visCon = PlayerSetup.Instance.GetComponentInChildren<ABI_RC.Systems.LipSync.CVRLipSyncManager>();
            var context = PlayerSetup.Instance.transform.Find("CVRLipSyncVisemes").GetComponentInChildren<CVROculusLipSyncContext>();
         
            while (!visCon.Equals(null) && !context.Equals(null))
            {
                //Main.Logger.Msg(ConsoleColor.Blue, $"---- {visCon.visemeLoudness:F2}");
                var frame = context.Frame;
                int loudestViseme = 0;
                float loudestValue = 0f;
                for (int i = 0; i < frame.Visemes.Length; ++i)
                {

                    float level = frame.Visemes[i];
                    if (level > 0.05 && level > loudestValue)
                    {
                        loudestViseme = i;
                        loudestValue = level;
                    }

                    if(driveIndiv)
                    {
                        PlayerSetup.Instance.animatorManager.SetAnimatorParameterFloat($"VisemeMod_{visemes[i]}", level);
                    }
                }
                //Main.Logger.Msg(ConsoleColor.Blue, $"Index: {loudestViseme} Name: {visemes[loudestViseme]} Loudness: {loudestValue:F2} CVRLoud: {visCon._visemeLoudness:F2}");

                if (driveValue)
                    PlayerSetup.Instance.animatorManager.SetAnimatorParameterInt("VisemeMod_Value", loudestViseme);
                if (driveViseme)
                    PlayerSetup.Instance.animatorManager.SetAnimatorParameterInt("Viseme", loudestViseme);
                if (driveLevel)
                    PlayerSetup.Instance.animatorManager.SetAnimatorParameterFloat("VisemeMod_Level", visCon._visemeLoudness);

                yield return new WaitForSeconds(updateRate.Value/1000);
            }
        }

        internal static void AvatarReady()
        {
            //Logger.Msg($"AvatarReady");
            //var avatarGuid = MetaPort.Instance.currentAvatarGuid;
            //Logger.Msg($"{avatarGuid}");
            if (updateRoutine != null) MelonCoroutines.Stop(updateRoutine);
            if (modEnabled.Value) updateRoutine = MelonCoroutines.Start(SetValues());
        }
    }
}



