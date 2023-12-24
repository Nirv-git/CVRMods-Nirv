using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(HideDragonWings.Main), "HideDragonWings", HideDragonWings.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(HideDragonWings.Main.versionStr)]
[assembly: AssemblyFileVersion(HideDragonWings.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Yellow)]

namespace HideDragonWings
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.7.1";

        public static MelonPreferences_Category cat;
        private const string catagory = "HideDragonWings";
        public static MelonPreferences_Entry<bool> useNirvMiscPage;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("HideDragonWings", ConsoleColor.DarkYellow);

            cat = MelonPreferences.CreateCategory(catagory, "HideDragonWings"); 
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page.");
            CustomBTKUI.InitUi();
        }

    }
}



