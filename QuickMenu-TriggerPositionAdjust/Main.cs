using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using UIExpansionKit.API;
using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.UI;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(QuickMenuTriggerPositionAdjust.Main), "QuickMenuTriggerPositionAdjust", QuickMenuTriggerPositionAdjust.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(QuickMenuTriggerPositionAdjust.Main.versionStr)]
[assembly: AssemblyFileVersion(QuickMenuTriggerPositionAdjust.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Cyan)]

namespace QuickMenuTriggerPositionAdjust
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.1";

        public static MelonPreferences_Category cat;
        private const string catagory = "QuickMenuTriggerPositionAdjust";
        public static MelonPreferences_Entry<bool> spacer;
        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("QuickMenuTriggerPositionAdjust", ConsoleColor.DarkCyan);
            cat = MelonPreferences.CreateCategory(catagory, "QuickMenu-TriggerPositionAdjust");
            spacer = cat.CreateEntry(nameof(spacer), true, "Make sure QuickMenu is open and press the buttons below to enter or leave Reposition Mode");
            var settings = ExpansionKitApi.GetSettingsCategory("QuickMenuTriggerPositionAdjust");
            settings.AddSimpleButton("EnterRepositionMode", () => {
                if (CVR_MenuManager.Instance._quickMenuOpen)
                { CVR_MenuManager.Instance.EnterRepositionMode(); }
                else
                { CohtmlHud.Instance.ViewDropText("Mod: QuickMenu-TriggerPositionAdjust", "QuickMenu needs to be open", "To enter Reposition Mode the QuickMenu must be open"); } 
            });
            settings.AddSimpleButton("ExitRepositionMode", () => {
                if (CVR_MenuManager.Instance.coreData.menuParameters.quickMenuInGrabMode)
                { CVR_MenuManager.Instance.ExitRepositionMode(); }
                else
                { CohtmlHud.Instance.ViewDropText("Mod: QuickMenu-TriggerPositionAdjust", "QuickMenu not in Reposition Mode", "Can not exit Reposition Mode as the QuickMenu isn't in it"); }
            });
        }
    }
}








