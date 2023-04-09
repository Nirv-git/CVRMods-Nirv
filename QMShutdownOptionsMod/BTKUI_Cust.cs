using System;
using MelonLoader;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using BTKUILib;
using BTKUILib.UIObjects;

namespace QMShutdownOptionsMod
{
    class BTKUI_Cust
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("NirvMisc", "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon("QMShutdownOptionsMod", "Block", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.Block.png"));
            QuickMenuAPI.PrepareIcon("QMShutdownOptionsMod", "Hibernate", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.Hibernate.png"));
            QuickMenuAPI.PrepareIcon("QMShutdownOptionsMod", "Shutdown", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.Shutdown.png"));
            QuickMenuAPI.PrepareIcon("QMShutdownOptionsMod", "Sleep", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.Sleep.png"));
            //QuickMenuAPI.PrepareIcon("QMShutdownOptionsMod", "", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons..png"));
        }

        public static void SetupUI()
        {
            loadAssets();

            Category cat = null;
            if (Main.useNirvMiscPage.Value)
            {
                var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                page.MenuTitle = "Nirv Misc Page";
                page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                cat = page.AddCategory("Shutdown Options", "QMShutdownOptionsMod");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("Shutdown Options", "QMShutdownOptionsMod");
            }

            cat.AddButton("Shutdown in 1 min", "Shutdown", "This will shutdown your computer in 1 minute").OnPress += () =>
            {
                QuickMenuAPI.ShowConfirm("Shutdown your computer?", "This will shutdown your computer in 1 minute", () => {
                    Main.Logger.Msg(ConsoleColor.Yellow, "Shutting down computer");
                    Main.runProgram("CMD.exe", "/C shutdown /s /f /t 60 /c \"Shutdown triggered from QMShutdownOptionsMod for ChilloutVR\"");
                }, () => { }, "Yes", "No");
            };
            cat.AddButton("Hibernate", "Hibernate", "This will hibernate your computer").OnPress += () =>
            { 
                QuickMenuAPI.ShowConfirm("Hibernate your computer?", "This will Hibernate your computer (If enabled)<p><p>This can not be aborted.", () => {
                    Main.Logger.Msg(ConsoleColor.Yellow, "Hibernating computer");
                    Main.runProgram("CMD.exe", "/C shutdown /h /f");
                }, () => { }, "Yes", "No");
            };
            cat.AddButton("Sleep", "Sleep", "This will sleep your computer").OnPress += () =>
            { //https://superuser.com/a/1310274
                QuickMenuAPI.ShowConfirm("Sleep your computer?", "This will sleep your computer (If enabled)<p><p>This can not be aborted.", () => {
                    Main.Logger.Msg(ConsoleColor.Yellow, "Sleep computer");
                    Main.runProgram("CMD.exe", "/C powershell.exe -C \"$m='[DllImport(\\\"Powrprof.dll\\\",SetLastError=true)]static extern bool SetSuspendState(bool hibernate,bool forceCritical,bool disableWakeEvent);public static void PowerSleep(){SetSuspendState(false,false,false); }';add-type -name Import -member $m -namespace Dll; [Dll.Import]::PowerSleep();\"");
                }, () => { }, "Yes", "No");
            };
            cat.AddButton("Abort Shutdown", "Block", "Aborts any current shutdown commends").OnPress += () =>
            {
                Main.Logger.Msg(ConsoleColor.Yellow, "Attempting to abort shutdown");
                Main.runProgram("CMD.exe", "/C shutdown /a");
            };
        }
    }
}


