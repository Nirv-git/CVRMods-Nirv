using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI.CCK;
using ABI.CCK.Components;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(RemoveChairs.Main), "RemoveChairs", RemoveChairs.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(RemoveChairs.Main.versionStr)]
[assembly: AssemblyFileVersion(RemoveChairs.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.DarkBlue)]


namespace RemoveChairs
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.1";

        public static MelonPreferences_Category cat;
        private const string catagory = "RemoveChairs";
        public static MelonPreferences_Entry<bool> useNirvMiscPage, debug;

        public static List<CVRInteractable> objectsDisabled = new List<CVRInteractable>();

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("RemoveChairs", ConsoleColor.DarkBlue);

            cat = MelonPreferences.CreateCategory(catagory, "RemoveChairs");
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page.");
            debug = MelonPreferences.CreateEntry(catagory, nameof(debug), false, "Log paths of chairs to console");
            CustomBTKUI.InitUi();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)//Without switch this would run 3 times at world load
            {
                case 0: break;
                case 1: break;
                case 2: break;
                default:
                    objectsDisabled.Clear(); //Clear the list if we change worlds 
                    break;
            }
        }
    }
}








