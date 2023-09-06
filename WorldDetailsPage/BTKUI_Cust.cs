using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using BTKUILib.UIObjects;
using System.Collections.Generic;
using ABI_RC.Core.Savior;
using ABI_RC.Core.InteractionSystem;


namespace WorldDetailsPage
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("NirvMisc", "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldDetailsPage.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon("WorldDetailsPage", "wdp-World", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldDetailsPage.Icons.World.png"));
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
                cat = page.AddCategory("World Details Page", "WorldDetailsPage");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("World Details Page", "WorldDetailsPage");
            }
            mainCat = cat;
            PopulateButtons();
        }

        public static void PopulateButtons()
        {
            mainCat.AddButton($"Open World Details Page", "wdp-World", "Open World Details Page of current world").OnPress += () =>
            {
                ViewManager.Instance.RequestWorldDetailsPage(MetaPort.Instance.CurrentWorldId);
                ViewManager.Instance.UiStateToggle(true);
            };

        }
    }

}