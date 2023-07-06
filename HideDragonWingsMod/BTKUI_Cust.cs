using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using BTKUILib.UIObjects;
using System.Collections.Generic;

namespace HideDragonWings
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("NirvMisc", "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("HideDragonWings.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon("HideDragonWings", "hdw-Empty", Assembly.GetExecutingAssembly().GetManifestResourceStream("HideDragonWings.Icons.wing-256.png"));
            QuickMenuAPI.PrepareIcon("HideDragonWings", "hdw-Full", Assembly.GetExecutingAssembly().GetManifestResourceStream("HideDragonWings.Icons.wing-full-256.png"));
        }

        public static Category mainCat;
        public static Dictionary<GameObject, Vector3> wingDict = new Dictionary<GameObject, Vector3>();

        public static void InitUi()
        {
            loadAssets();
            Category cat = null;
            if (Main.useNirvMiscPage.Value)
            {
                var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                page.MenuTitle = "Nirv Misc Page";
                page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                cat = page.AddCategory("Hide Dragon Wings", "HideDragonWings");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("Hide Dragon Wings", "HideDragonWings");
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