using System.Reflection;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using BTKUILib;
using BTKUILib.UIObjects;
using ABI.CCK.Components;
using MelonLoader;
using System.Linq;
using Semver;

namespace RemoveChairs
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("RemoveChairs.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "remChairs-Chair_en", Assembly.GetExecutingAssembly().GetManifestResourceStream("RemoveChairs.Icons.Chair_en.png"));
            QuickMenuAPI.PrepareIcon(ModName, "remChairs-Chair_dis", Assembly.GetExecutingAssembly().GetManifestResourceStream("RemoveChairs.Icons.Chair_dis.png"));
        }

        public static string ModName = "NirvBTKUI";
        private static MethodInfo _btkGetCreatePageAdapter;

        public static Category mainCat;

        public static void InitUi()
        {
            if (MelonMod.RegisteredMelons.Any(x => x.Info.Name.Equals("BTKUILib") && x.Info.SemanticVersion.CompareByPrecedence(new SemVersion(1, 9)) > 0))
            {
                //We're working with UILib 2.0.0, let's reflect the get create page function
                _btkGetCreatePageAdapter = typeof(Page).GetMethod("GetOrCreatePage", BindingFlags.Public | BindingFlags.Static);
                Main.Logger.Msg($"BTKUILib 2.0.0 detected, attempting to grab GetOrCreatePage function: {_btkGetCreatePageAdapter != null}");
            }
            if (!Main.useNirvMiscPage.Value)
            {
                ModName = "removeChairsMod";
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
                cat = page.AddCategory("Remove Chairs");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("Remove Chairs", ModName);
            }
            mainCat = cat;
            PopulateButtons();
        }

        public static void PopulateButtons()
        {
            mainCat.AddButton($"Disable Chairs", "remChairs-Chair_dis", "Disables all active chair objects<p>Warning may break some world/prop features").OnPress += () =>
            {
                try
                {
                    int countChange = 0;
                    var objects = Resources.FindObjectsOfTypeAll<CVRInteractable>()
                        .Where(x => x.actions.Any(y => y.operations.Any(z => z.type == CVRInteractableActionOperation.ActionType.SitAtPosition)))
                        .ToList();
                    foreach (var item in objects)
                    {
                        var scene = item.gameObject.scene.name;
                        if (item.gameObject.activeInHierarchy && !(scene == "DontDestroyOnLoad" || scene == "HideAndDontSave")) //Only disable active chairs
                        {
                            countChange++;
                            Main.objectsDisabled.Add(item);
                            item.gameObject.SetActive(false);
                            if(Main.debug.Value) Main.Logger.Msg("Dis " + Utils.GetPath(item.transform));

                        }
                    }
                    QuickMenuAPI.ShowAlertToast($"Disabled {countChange} chairs", 4);
                    Main.Logger.Msg("Disabled " + countChange + " chair objects");
                }
                catch (System.Exception ex) { Main.Logger.Error($"Error disabling chairs\n" + ex.ToString()); }
            };

            mainCat.AddButton($"Enable Chairs", "remChairs-Chair_en", "Enables any previously disabled chairs objects").OnPress += () =>
            {
                try
                {
                    int countChange = 0;
                    foreach (var item in Main.objectsDisabled)
                    {
                        if (item?.Equals(null) ?? true) continue;
                        countChange++;
                        item.gameObject.SetActive(true);
                        if (Main.debug.Value) Main.Logger.Msg("En " + Utils.GetPath(item.transform));
                    }
                    Main.Logger.Msg("Enabled " + countChange + " chair objects");
                    QuickMenuAPI.ShowAlertToast($"Enabled {countChange} chairs", 4);
                    Main.objectsDisabled.Clear();
                }
                catch (System.Exception ex) { Main.Logger.Error($"Error re-enabling chairs\n" + ex.ToString()); }
            };
        }



    }

}