using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using BTKUILib.UIObjects;
using System.Collections.Generic;
using MelonLoader;
using System.Linq;
using Semver;

namespace HideDragonWings
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("HideDragonWings.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "hdw-Empty", Assembly.GetExecutingAssembly().GetManifestResourceStream("HideDragonWings.Icons.wing-256.png"));
            QuickMenuAPI.PrepareIcon(ModName, "hdw-Full", Assembly.GetExecutingAssembly().GetManifestResourceStream("HideDragonWings.Icons.wing-full-256.png"));
        }

        public static string ModName = "NirvBTKUI";
        private static MethodInfo _btkGetCreatePageAdapter;

        public static Category mainCat;
        public static Dictionary<GameObject, Vector3> wingDict = new Dictionary<GameObject, Vector3>();

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
                ModName = "hideDragonWingsMod";
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
                cat = page.AddCategory("Hide Dragon Wings");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("Hide Dragon Wings", ModName);
            }
            mainCat = cat;
            PopulateButtons();
        }

        public static void PopulateButtons()
        {
            mainCat.AddButton($"Disable Wings", "hdw-Empty", "Shrinks wing bone to 0,0,0").OnPress += () =>
            {
                //Undo any previous 
                RestoreWings();

                foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
                {
                    if (obj.name == "Wing Shoulder L" && obj.transform?.GetChild(0)?.name == "Wing Arm L")
                    {
                        wingDict.Add(obj, obj.transform.localScale);
                        obj.transform.localScale = Vector3.zero;
                    } 
                    else if (obj.name == "Wing Shoulder R" && obj.transform?.GetChild(0)?.name == "Wing Arm R")
                    {
                        wingDict.Add(obj, obj.transform.localScale);
                        obj.transform.localScale = Vector3.zero;
                    }
                }
            };

            mainCat.AddButton($"Enable Wings", "hdw-Full", "Restores wings to original scale").OnPress += () =>
            {
                RestoreWings();
            };
        }

        public static void RestoreWings()
        {
            foreach (var entry in wingDict)
            {
                if(entry.Key != null)
                    entry.Key.transform.localScale = entry.Value;
            }
            wingDict.Clear();
        }
    }

}