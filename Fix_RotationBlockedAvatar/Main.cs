using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[assembly: MelonInfo(typeof(Fix_RotationBlockedAvatar.Main), "Fix_RotationBlockedAvatar", Fix_RotationBlockedAvatar.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace Fix_RotationBlockedAvatar
{
    public class Main : MelonMod
    {
        public const string versionStr = "0.1";
        public static MelonLogger.Instance Logger;

        public static Main Instance;

        public static MelonPreferences_Entry<bool> useFix;


        public override void OnApplicationStart()
        {
            Instance = this;
            Logger = new MelonLogger.Instance("Fix_RotationBlockedAvatar");

            var cat = MelonPreferences.CreateCategory("Fix_RotationBlockedAvatar", "Fix_RotationBlockedAvatar");
            useFix = cat.CreateEntry(nameof(useFix), true, "useFix");
        }
    }
}


