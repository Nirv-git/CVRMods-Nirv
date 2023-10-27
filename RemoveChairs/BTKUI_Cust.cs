using System.Reflection;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using BTKUILib;
using BTKUILib.UIObjects;
using ABI.CCK.Components;

namespace RemoveChairs
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("NirvMisc", "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("RemoveChairs.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon("RemoveChairs", "remChairs-Chair_en", Assembly.GetExecutingAssembly().GetManifestResourceStream("RemoveChairs.Icons.Chair_en.png"));
            QuickMenuAPI.PrepareIcon("RemoveChairs", "remChairs-Chair_dis", Assembly.GetExecutingAssembly().GetManifestResourceStream("RemoveChairs.Icons.Chair_dis.png"));
        }

        public static Category mainCat;

        public static void InitUi()
        {
            loadAssets();
            Category cat = null;
            if (Main.useNirvMiscPage.Value)
            {
                var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                page.MenuTitle = "Nirv Misc Page";
                page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                cat = page.AddCategory("Remove Chairs", "RemoveChairs");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("Remove Chairs", "RemoveChairs");
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