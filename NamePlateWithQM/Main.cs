using System.Reflection;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using ABI_RC.Core.Base;
using HarmonyLib;
using System;
using System.Linq;
using System.Collections;
using ABI_RC.Core.Savior;

[assembly: MelonInfo(typeof(NamePlateWithQM.Main), "NamePlateWithQM", "0.1.1", "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace NamePlateWithQM
{

    public class Main : MelonMod
    { 
        public static MelonLogger.Instance Logger;

        public static bool firstload = true;
        public static bool plateState = false;
        public static MelonPreferences_Entry<bool> enNamePlateWithQM;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("NamePlateWithQM");

            MelonPreferences.CreateCategory("NamePlateWithQM", "NamePlateWithQM");
            enNamePlateWithQM = MelonPreferences.CreateEntry<bool>("NamePlateWithQM", "enNamePlateWithQM", true, "Enable/Disable Nameplates when Quickmenu is open");
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
                    }
                    break;
            }
        }

        private static void QMtoggle(bool __0)
        {
            if (!enNamePlateWithQM.Value)
                return;

            if (!MetaPort.Instance.settings.GetSettingsBool("GeneralShowNameplates"))
            {
                //Logger.Msg("Nameplates must be enabled in CVR General settings for Mod to function. You can disable this mod with a MelonPref."); //I would force enable it, but that causes crashes in testing
                return;
            }

            var plates = GameObject.FindObjectsOfType<PlayerNameplate>();
            if (plates.Length == 0)
                return;

            for (int i = 0; i < plates.Length; i++)
            {
                try
                {
                    plates[i].transform.GetChild(0).gameObject.SetActive(__0);
                }
                catch (System.Exception ex) { Logger.Msg($"Error for {i} | {__0}\n" + ex.ToString()); }
            }
            plateState = __0;     
        }


        private static void OnPlayerJoined(PlayerDescriptor __instance)
        {
            if (enNamePlateWithQM.Value) MelonCoroutines.Start(DelayPlayerInfo(__instance));
        }


        public static IEnumerator DelayPlayerInfo(PlayerDescriptor value)
        {
            yield return new WaitForSeconds(2.222f);
            value.transform.Find("[NamePlate]").GetChild(0).gameObject.SetActive(plateState);
        }  
    }
}


