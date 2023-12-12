using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core;


[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(CVRCameraDontOverrender.Main), "CVRCameraDontOverrender", CVRCameraDontOverrender.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(CVRCameraDontOverrender.Main.versionStr)]
[assembly: AssemblyFileVersion(CVRCameraDontOverrender.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Gray)]


namespace CVRCameraDontOverrender
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.1";

        public static MelonPreferences_Category cat;
        private const string catagory = "CVRCameraDontOverrender";
        public static MelonPreferences_Entry<bool> dontOverrender;

        public static bool firstload = true;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("CVRCameraDontOverrender", ConsoleColor.DarkCyan);

            cat = MelonPreferences.CreateCategory(catagory, "CVRCameraDontOverrender");
            dontOverrender = MelonPreferences.CreateEntry(catagory, nameof(dontOverrender), true, "CVR Camera doesn't over render your local player");
            dontOverrender.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) =>
            {
                SetCamLayer();
            });
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)
            {
                case 0: break;
                case 1: break;
                case 2: break;
                default:
                    if (firstload)
                    {
                        SetCamLayer();
                        firstload = false;
                    }
                    break;
            }
        }

        public static void SetCamLayer()
        {
            var layer = dontOverrender.Value ? CVRLayers.UI : CVRLayers.UIInternal;
            GameObject.Find("_PLAYERLOCAL/CameraSpawn/CVR Camera 2.0/Content/Camera Canvas").layer = layer;
        }
    }
}








