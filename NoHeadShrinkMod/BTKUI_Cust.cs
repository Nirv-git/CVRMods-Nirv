using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using BTKUILib.UIObjects;
using System.Collections.Generic;

namespace NoHeadShrinkMod
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("NirvMisc", "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("NoHeadShrinkMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon("NoHeadShrinkMod", "Noshrink-Head", Assembly.GetExecutingAssembly().GetManifestResourceStream("NoHeadShrinkMod.Icons.Head.png"));
            QuickMenuAPI.PrepareIcon("NoHeadShrinkMod", "Noshrink-Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("NoHeadShrinkMod.Icons.Reset.png"));
        }

        //This is done to keep BTKUI an optional dependancy 
        public static System.Object mainCat;
        public static System.Object pageSub;

        public static bool dontUpdate = false; //This is hacky

        public static void InitUi()
        {
            loadAssets();
            Category cat = null;
            if (Main.useNirvMiscPage.Value)
            {
                var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                page.MenuTitle = "Nirv Misc Page";
                page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                cat = page.AddCategory("No Head Shrink", "NoHeadShrinkMod");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("No Head Shrink", "NoHeadShrinkMod");
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
            pageSub = ((Category)mainCat).AddPage("Distance & Remeasure", "Noshrink-Head", "Change unshrink distance and Remeasure Avatar Height", "NoHeadShrinkMod");
            var subCat = ((Page)pageSub).AddCategory("");
            subCat.AddButton($"Remeasure Avatar Height", "Noshrink-Reset", "Remeasures current avatar height").OnPress += () =>
            {
                Main.FindScale();
            };
            ((Page)pageSub).AddSlider("Unshink distance", "Unshink distance value", Main.unshrinkDistance.Value, 0f, 2f).OnValueUpdated += action =>
            {
                dontUpdate = true;
                Main.unshrinkDistance.Value = action;
                dontUpdate = false;
            };
        }  
    }
}