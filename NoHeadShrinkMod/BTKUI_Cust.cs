using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using BTKUILib.UIObjects;
using System.Collections.Generic;
using MelonLoader;
using System.Linq;
using Semver;

namespace NoHeadShrinkMod
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("NoHeadShrinkMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "Noshrink-Head", Assembly.GetExecutingAssembly().GetManifestResourceStream("NoHeadShrinkMod.Icons.Head.png"));
            QuickMenuAPI.PrepareIcon(ModName, "Noshrink-Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("NoHeadShrinkMod.Icons.Reset.png"));
        }

        public static string ModName = "NirvBTKUI";
        private static MethodInfo _btkGetCreatePageAdapter;

        //This is done to keep BTKUI an optional dependancy 
        public static System.Object mainCat;
        public static System.Object pageSub;

        public static bool dontUpdate = false; //This is hacky

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
                ModName = "noHeadShrinkMod";
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
                cat = page.AddCategory("No Head Shrink");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("No Head Shrink", ModName);
            }
            mainCat = cat;
            PopulateButtons();
        }

        public static void ChangeButtons(bool newValue, bool oldValue)
        {
            if (!dontUpdate) PopulateButtons();
        }
        public static void ChangeButtons(float newValue, float oldValue)
        {
            if (!dontUpdate) PopulateButtons();
        }

        public static void PopulateButtons()
        {
            if (((Category)mainCat).IsGenerated)
                ((Category)mainCat).ClearChildren();

            ((Category)mainCat).AddToggle("Always Disable Head Shrink", "Model's head will never be shrunk", Main.disableHeadShrink.Value).OnValueUpdated += action =>
            {
                dontUpdate = true;
                Main.disableHeadShrink.Value = action;
                dontUpdate = false;
            };

            ((Category)mainCat).AddToggle("Unshrink at distance", "Unshrink when your camera is x distance from head", Main.unshrinkAtDistance.Value).OnValueUpdated += action =>
            {
                dontUpdate = true;
                Main.unshrinkAtDistance.Value = action;
                dontUpdate = false;
            };

            ((Category)mainCat).AddToggle("Scale distance with height", "Scale distance based on avatar height (Height*Distance)", Main.scaleDistance.Value).OnValueUpdated += action =>
            {
                dontUpdate = true;
                Main.scaleDistance.Value = action;
                dontUpdate = false;
            };

            if (pageSub != null)
                ((Page)pageSub).Delete();
            pageSub = ((Category)mainCat).AddPage("Distance & Remeasure", "Noshrink-Head", "Change unshrink distance and Remeasure Avatar Height", ModName);
            var subCat = ((Page)pageSub).AddCategory("");
            subCat.AddButton($"Remeasure Avatar Height", "Noshrink-Reset", "Remeasures current avatar height").OnPress += () =>
            {
                Main.FindScale();
            };
            ((Page)pageSub).AddSlider("Unshink distance", "Unshrink distance value", Main.unshrinkDistance.Value, 0f, 2f).OnValueUpdated += action =>
            {
                dontUpdate = true;
                Main.unshrinkDistance.Value = action;
                dontUpdate = false;
            };
        }  
    }
}