using System;
using MelonLoader;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using BTKUILib;
using BTKUILib.UIObjects;
using Semver;

namespace QMShutdownOptionsMod
{
    class BTKUI_Cust
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "qmShut-Block", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.Block.png"));
            QuickMenuAPI.PrepareIcon(ModName, "qmShut-Hibernate", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.Hibernate.png"));
            QuickMenuAPI.PrepareIcon(ModName, "qmShut-Shutdown", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.Shutdown.png"));
            QuickMenuAPI.PrepareIcon(ModName, "qmShut-Sleep", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons.Sleep.png"));
            //QuickMenuAPI.PrepareIcon(ModName, "", Assembly.GetExecutingAssembly().GetManifestResourceStream("QMShutdownOptionsMod.Icons..png"));
        }

        public static string ModName = "NirvBTKUI";
        private static MethodInfo _btkGetCreatePageAdapter;

        public static void SetupUI()
        {
            if (MelonMod.RegisteredMelons.Any(x => x.Info.Name.Equals("BTKUILib") && x.Info.SemanticVersion.CompareByPrecedence(new SemVersion(1, 9)) > 0))
            {
                //We're working with UILib 2.0.0, let's reflect the get create page function
                _btkGetCreatePageAdapter = typeof(Page).GetMethod("GetOrCreatePage", BindingFlags.Public | BindingFlags.Static);
                Main.Logger.Msg($"BTKUILib 2.0.0 detected, attempting to grab GetOrCreatePage function: {_btkGetCreatePageAdapter != null}");
            }
            if (!Main.useNirvMiscPage.Value)
            {
                ModName = "shutdownOptionsMod";
            }

            loadAssets();

            Category cat = null;
            if (Main.useNirvMiscPage.Value)
            {
                //var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                Page page = null;
                if (_btkGetCreatePageAdapter != null)
                    page = (Page)_btkGetCreatePageAdapter.Invoke(null, new object[] { ModName, "Nirv Misc Page", true, "NirvMisc", null, false });
                else
                    page = new Page(ModName, "Nirv Misc Page", true, "NirvMisc");
                page.MenuTitle = "Nirv Misc Page";
                page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                cat = page.AddCategory("Shutdown Options");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("Shutdown Options", ModName);
            }

            cat.AddButton("Shutdown in 1 minute", "qmShut-Shutdown", "This will shutdown your computer in 1 minute").OnPress += () =>
            {
                QuickMenuAPI.ShowConfirm("Shutdown your computer?", "This will shutdown your computer in 1 minute", () => {
                    Main.Logger.Msg(ConsoleColor.Yellow, "Shutting down computer");
                    Main.runProgram("CMD.exe", "/C shutdown /s /f /t 60 /c \"Shutdown triggered from QMShutdownOptionsMod for ChilloutVR\"");
                }, () => { }, "Yes", "No");
            };
            cat.AddButton("Hibernate", "qmShut-Hibernate", "This will hibernate your computer").OnPress += () =>
            { 
                QuickMenuAPI.ShowConfirm("Hibernate your computer?", "This will Hibernate your computer (If enabled)<p><p>This can not be aborted.", () => {
                    Main.Logger.Msg(ConsoleColor.Yellow, "Hibernating computer");
                    Main.runProgram("CMD.exe", "/C shutdown /h /f");
                }, () => { }, "Yes", "No");
            };
            cat.AddButton("Sleep", "qmShut-Sleep", "This will sleep your computer").OnPress += () =>
            { //https://superuser.com/a/1310274
              //This is a cursed solution 
                QuickMenuAPI.ShowConfirm("Sleep your computer?", "This will sleep your computer (If enabled)<p><p>This can not be aborted.", () => {
                    Main.Logger.Msg(ConsoleColor.Yellow, "Sleep computer");
                    Main.runProgram("CMD.exe", "/C powershell.exe -C \"$m='[DllImport(\\\"Powrprof.dll\\\",SetLastError=true)]static extern bool SetSuspendState(bool hibernate,bool forceCritical,bool disableWakeEvent);public static void PowerSleep(){SetSuspendState(false,false,false); }';add-type -name Import -member $m -namespace Dll; [Dll.Import]::PowerSleep();\"");
                }, () => { }, "Yes", "No");
            };
            cat.AddButton("Abort Shutdown", "qmShut-Block", "Aborts any current shutdown commands").OnPress += () =>
            {
                Main.Logger.Msg(ConsoleColor.Yellow, "Attempting to abort shutdown");
                Main.runProgram("CMD.exe", "/C shutdown /a");
            };
        }
    }
}


