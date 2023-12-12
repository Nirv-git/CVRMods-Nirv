using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core.Savior;
using System.Security.Cryptography;
using System.Text;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(ShrinkOtherHeads.Main), "ShrinkOtherHeads", ShrinkOtherHeads.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(ShrinkOtherHeads.Main.versionStr)]
[assembly: AssemblyFileVersion(ShrinkOtherHeads.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.DarkCyan)]


namespace ShrinkOtherHeads
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.1";

        public static MelonPreferences_Category cat;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("ShrinkOtherHeads", ConsoleColor.DarkCyan);

            CustomBTKUI.InitUi();
        }

    }
}








