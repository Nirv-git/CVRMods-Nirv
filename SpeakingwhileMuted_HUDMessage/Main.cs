using ABI_RC.Core;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;



[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(SpeakingwhileMuted_HUDMessage.Main), "SpeakingwhileMuted_HUDMessage", SpeakingwhileMuted_HUDMessage.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(SpeakingwhileMuted_HUDMessage.Main.versionStr)]
[assembly: AssemblyFileVersion(SpeakingwhileMuted_HUDMessage.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Cyan)]

namespace SpeakingwhileMuted_HUDMessage
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.1";

        public static MelonPreferences_Category cat;
        private const string catagory = "SpeakingwhileMuted_HUDMessage";
        public static MelonPreferences_Entry<bool> sendHUDmsg;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("SpeakingwhileMuted_HUDMessage", ConsoleColor.Blue);

            cat = MelonPreferences.CreateCategory(catagory, "Speaking while Muted - HUD Message");

            sendHUDmsg = MelonPreferences.CreateEntry(catagory, nameof(sendHUDmsg), true, "Send HUD message when talking while muted");
        }
    }
}

