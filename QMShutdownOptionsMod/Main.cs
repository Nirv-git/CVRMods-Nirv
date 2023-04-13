using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;



[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(QMShutdownOptionsMod.Main), "QMShutdownOptionsMod", QMShutdownOptionsMod.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(QMShutdownOptionsMod.Main.versionStr)]
[assembly: AssemblyFileVersion(QMShutdownOptionsMod.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Cyan)]

namespace QMShutdownOptionsMod
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.6";

        public static MelonPreferences_Category cat;
        private const string catagory = "QMShutdownOptions";
        public static MelonPreferences_Entry<bool> useNirvMiscPage;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("QMShutdownOptionsMod", ConsoleColor.Cyan);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MelonLogger.Msg(ConsoleColor.Red, "QMShutdownOptions is only available for Windows");
                return;
            }
            cat = MelonPreferences.CreateCategory(catagory, "QMShutdownOptionsMod");
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page. (Restart req)");
            
            BTKUI_Cust.SetupUI();      
        }

        public static void runProgram(string exe, string cmd)
        { //Could make this not show the CMD window, but why bother? https://stackoverflow.com/a/1469790
            System.Diagnostics.Process.Start(exe, cmd);
        }
    }
}

