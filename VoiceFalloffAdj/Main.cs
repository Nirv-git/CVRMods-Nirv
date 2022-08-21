using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.IO;
using ABI.CCK.Components;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using HarmonyLib;

[assembly: MelonInfo(typeof(VoiceFalloffAdj.Main), "VoiceFalloffAdj", "0.1", "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace VoiceFalloffAdj
{

    public class Main : MelonMod
    { 
        public static MelonLogger.Instance Logger;

        public static bool firstload = true;

        public static MelonPreferences_Entry<bool> adjustVoiceMax;
        public static MelonPreferences_Entry<float> voiceMax;
        public static MelonPreferences_Entry<int> QMposition;
        public static MelonPreferences_Entry<int> QMhighlightColor;

        public static GameObject audioController;

        public static object sliderCoroutine;

        public static bool voiceChangeEnabled = false;
        public static float lastVoiceMax = 0f;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("VoiceFalloffAdj");

            loadAssets();

            MelonPreferences.CreateCategory("VoiceFalloffAdj", "VoiceFalloffAdj");
            adjustVoiceMax = MelonPreferences.CreateEntry<bool>("VoiceFalloffAdj", "adjustVoiceMax", false, "Adjust max voice distance of users");
            voiceMax = MelonPreferences.CreateEntry<float>("VoiceFalloffAdj", "voiceMax", 7.5f, "Voice Distance (1-14.5)");
            QMposition = MelonPreferences.CreateEntry<int>("VoiceFalloffAdj", "QMposition", 0, "QuickMenu Position (0=Right, 1=Top, 2=Left)");
            QMhighlightColor = MelonPreferences.CreateEntry<int>("VoiceFalloffAdj", "QMhighlightColor", 0, "Enabled color for QuickMenu items (0=Orange, 1=Yellow, 2=Pink)");

            lastVoiceMax = voiceMax.Value;
            OnPreferencesSaved();
        }

        public static IEnumerator DelaySlider()
        {
            yield return new WaitForSeconds(1f);
            //Logger.Msg("Co Update");
            Main main = new Main(); main.OnPreferencesSaved();            
        }

        public override void OnPreferencesSaved()
        {
            if (QM.settings != null)
            {
                switch (Main.QMposition.Value)
                {
                    case 1: QM.settingsRight.SetActive(false); QM.settingsTop.SetActive(true); QM.settingsLeft.SetActive(false); QM.settingsCanvas = QM.settings.transform.Find("MenuTop/SettingsMenuCanvas").gameObject; break; //Top
                    case 2: QM.settingsRight.SetActive(false); QM.settingsTop.SetActive(false); QM.settingsLeft.SetActive(true); QM.settingsCanvas = QM.settings.transform.Find("MenuLeft/SettingsMenuCanvas").gameObject; break; //Left
                    default: QM.settingsRight.SetActive(true); QM.settingsTop.SetActive(false); QM.settingsLeft.SetActive(false); QM.settingsCanvas = QM.settings.transform.Find("MenuRight/SettingsMenuCanvas").gameObject; break; //Right
                }
                QM.ParseSettings();
            }

            if(!firstload && (voiceChangeEnabled != adjustVoiceMax.Value || lastVoiceMax != voiceMax.Value))
            {
                if (adjustVoiceMax.Value)
                    SetForAllPlayers(true);
                else
                    SetForAllPlayers(false);
                voiceChangeEnabled = adjustVoiceMax.Value;
                lastVoiceMax = voiceMax.Value;
            }  
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)//Without switch this would run 3 times at world load
            {
                case 0: break;
                case 1: break;
                case 2: break;
                default:
                    if (firstload)
                    {
                        HarmonyInstance.Patch(typeof(CVR_MenuManager).GetMethod(nameof(CVR_MenuManager.ToggleQuickMenu)), null, new HarmonyMethod(typeof(Main).GetMethod(nameof(QMtoggle), BindingFlags.NonPublic | BindingFlags.Static)));
                        HarmonyInstance.Patch(AccessTools.Constructor(typeof(PlayerDescriptor)), null, new HarmonyMethod(typeof(Main).GetMethod(nameof(OnPlayerJoined), BindingFlags.NonPublic | BindingFlags.Static)));
                        //Logger.Msg("default" + buildIndex);
                        firstload = false;
                        audioController = GameObject.Find("DissonanceHead/DissonanceSetup");
                        QM.CreateQuickMenuButton();
                    }
                    else
                    {
                        if (adjustVoiceMax.Value)
                            MelonCoroutines.Start(DelaySetAll());
                    }
                    QM.ParseSettings();
                    break;
            }
        }
        private static void QMtoggle(bool __0)
        {
            MelonCoroutines.Start(DelayQM(__0));
        }

        public static IEnumerator DelayQM(bool value)
        {
            yield return new WaitForSeconds(.22f);
            QM.settings.SetActive(value);
        }

        private static void OnPlayerJoined(PlayerDescriptor __instance)
        {
            if(adjustVoiceMax.Value) MelonCoroutines.Start(DelayPlayerInfo(__instance));
        }

        public static IEnumerator DelayPlayerInfo(PlayerDescriptor value)
        {
            yield return new WaitForSeconds(2.222f);
            audioController.transform.Find("Player " + value.gameObject.name + " voice comms").GetComponent<AudioSource>().maxDistance = voiceMax.Value;
        }

        public static IEnumerator DelaySetAll()
        {
            yield return new WaitForSeconds(5f);
            SetForAllPlayers(true);
        }

        public static void SetForAllPlayers(bool en)
        {
            for(int i = 0; i < audioController.transform.childCount; i++)
            {
                try
                {
                    audioController.transform.GetChild(i).GetComponent<AudioSource>().maxDistance = (en ? voiceMax.Value : 7.5f);
                }
                catch (System.Exception ex) { Logger.Msg($"Error setting for {i}\n" + ex.ToString()); }
            }
        }


        private void loadAssets()
        {//https://github.com/ddakebono/BTKSASelfPortrait/blob/master/BTKSASelfPortrait.cs
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("VoiceFalloffAdjMod.voiceassetbun"))
            {
                using (var tempStream = new MemoryStream((int)assetStream.Length))
                {
                    assetStream.CopyTo(tempStream);
                    assetBundle = AssetBundle.LoadFromMemory(tempStream.ToArray(), 0);
                    assetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                }
            }
            if (assetBundle != null)
            {
                voiceSettingsPrefab = assetBundle.LoadAsset<GameObject>("VoiceSettings");
                voiceSettingsPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
            else Logger.Error("Bundle was null");
        }


        public static AssetBundle assetBundle;
        public static GameObject voiceSettingsPrefab;


    }
}


