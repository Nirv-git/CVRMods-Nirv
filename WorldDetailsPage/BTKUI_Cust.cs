using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using BTKUILib.UIObjects;
using System.Collections.Generic;
using ABI_RC.Core.Savior;
using ABI_RC.Core.InteractionSystem;
using MelonLoader;
using System.Linq;
using Semver;

namespace WorldDetailsPage
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldDetailsPage.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "wdp-World", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldDetailsPage.Icons.World.png"));
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
                ModName = "worldDetailsMod";
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
                cat = page.AddCategory("World Details Page");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("World Details Page", ModName);
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