using System.Reflection;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using HarmonyLib;
using System;
using System.Linq;
using System.Collections;

[assembly: MelonInfo(typeof(NoPlayerSelectWithQM.Main), "NoPlayerSelectWithQM", NoPlayerSelectWithQM.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace NoPlayerSelectWithQM
{

    public class Main : MelonMod
    {
        public const string versionStr = "0.0.3";
        public static MelonLogger.Instance Logger;

        public static bool firstload = true;
        public static MelonPreferences_Entry<bool> disPlayerSelectQM;
        public static bool QMstate = false;
        public static bool LargeMenuState = false;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("NoPlayerSelectWithQM");

            MelonPreferences.CreateCategory("NoPlayerSelectWithQM", "NoPlayerSelectWithQM");
            disPlayerSelectQM = MelonPreferences.CreateEntry<bool>("NoPlayerSelectWithQM", "disPlayerSelectQM", true, "Disable Player Selection Collider when Quick Menu is open");
            disPlayerSelectQM.OnValueChanged += ReEnableColliders;
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
                        HarmonyInstance.Patch(typeof(ViewManager).GetMethods().First(m => m.Name == nameof(ViewManager.UiStateToggle) && m.GetParameters().Length == 1), null, new HarmonyMethod(typeof(Main).GetMethod(nameof(LargeMenuToggle), BindingFlags.NonPublic | BindingFlags.Static)));
                        //Logger.Msg("default" + buildIndex);
                        firstload = false;
                    }
                    break;
            }
        }

        private static void QMtoggle(bool __0)
        {
            //Logger.Msg($"Small Menu {__0}");
            QMstate = __0;
            if (LargeMenuState) //Don't disable colliders when large menu is open
                return;
            if (disPlayerSelectQM.Value) ToggleColliders(!__0);
        }
        private static void LargeMenuToggle(bool __0)
        {
            //Logger.Msg($"Large Menu {__0}");
            LargeMenuState = __0;
            if (disPlayerSelectQM.Value) ToggleColliders(__0);
        }
        private static void ReEnableColliders(bool oldValue, bool newValue)
        { //If you disable the Melonpref that toggles colliders on and off, make sure colliders are reenabled
            if(newValue == false)
                ToggleColliders(true);
        }

        private static void ToggleColliders(bool setTo)
        {
            var players = GameObject.FindObjectsOfType<PlayerDescriptor>();
            if (players.Length == 0)
                return;

            for (int i = 0; i < players.Length; i++)
            {
                try
                {
                    if (players[i].gameObject == PlayerSetup.Instance.gameObject)
                        continue;

                    players[i].gameObject.GetComponent<CapsuleCollider>().enabled = setTo;
                }
                catch (System.Exception ex) { Logger.Msg($"Error for {i} | SetTo {setTo} - QM {QMstate} - LM {LargeMenuState}\n" + ex.ToString()); }
            }
        }
    }
}


