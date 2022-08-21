using System.Reflection;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using HarmonyLib;

[assembly: MelonInfo(typeof(NamePlateWithQM.Main), "NamePlateWithQM", "0.1", "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace NamePlateWithQM
{

    public class Main : MelonMod
    { 
        public static MelonLogger.Instance Logger;

        public static bool firstload = true;

        public static MelonPreferences_Entry<bool> enNamePlateWithQM;

        public static bool changedQMState = false;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("NamePlateWithQM");

            MelonPreferences.CreateCategory("NamePlateWithQM", "NamePlateWithQM");
            enNamePlateWithQM = MelonPreferences.CreateEntry<bool>("NamePlateWithQM", "enNamePlateWithQM", true, "Enable Nameplates when Quickmenu is open");
            
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

            void setState(PlayerNameplate[] array, bool value)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    try
                    {
                        array[i].transform.GetChild(0).gameObject.SetActive(value);
                        //Logger.Msg(Utils.GetPath(array[i].transform));
                    }
                    catch (System.Exception ex) { Logger.Msg($"Error for {i} | {value}\n" + ex.ToString()); }
                }
            }

            var plates = GameObject.FindObjectsOfType<PlayerNameplate>();
            if (plates.Length == 0)
                return;

            if(__0)
            {//If QM is being opened, try to enable
                if (plates[0].transform.GetChild(0).gameObject.activeSelf)
                    return; //Canvas is already active, no need to change
                setState(plates, true);
                changedQMState = true;
            }else if (changedQMState)
            {//Check if we enabled them, and if so, disable them
                setState(plates, false);
                changedQMState = false;
            } 
        }
    }
}


