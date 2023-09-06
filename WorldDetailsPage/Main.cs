using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(WorldDetailsPage.Main), "WorldDetailsPage", WorldDetailsPage.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(WorldDetailsPage.Main.versionStr)]
[assembly: AssemblyFileVersion(WorldDetailsPage.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Yellow)]

namespace WorldDetailsPage
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.1";

        public static MelonPreferences_Category cat;
        private const string catagory = "WorldDetailsPage";
        public static MelonPreferences_Entry<bool> useNirvMiscPage;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("WorldDetailsPage", ConsoleColor.DarkYellow);

            cat = MelonPreferences.CreateCategory(catagory, "WorldDetailsPage"); 
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page.");
            CustomBTKUI.InitUi();
        }

    }
}



