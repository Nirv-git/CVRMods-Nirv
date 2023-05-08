using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using BTKUILib.UIObjects;
using System.Collections.Generic;

namespace NearClipPlaneAdj
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("NirvMisc", "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "0001", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.n0001.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "001", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.n001.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "01", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.n01.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "05", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.n05.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "nearclip-Keypad", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.Keypad.png"));
        }

        //This is done to keep BTKUI an optional dependancy 
        public static System.Object mainCat;
        public static void InitUi()
        {
            loadAssets();
            Category cat = null;
            if (Main.useNirvMiscPage.Value)
            {
                var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                page.MenuTitle = "Nirv Misc Page";
                page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                cat = page.AddCategory("Nearclip Plane Adjust", "NearClipPlaneAdj");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("Nearclip Plane Adjust", "NearClipPlaneAdj");
            }
            mainCat = cat;
            PopulateButtons();
        }

        public static void ChangeButtons(bool newValue, bool oldValue)
        {
            PopulateButtons();
        }

        public static void PopulateButtons()
        {
            if (((Category)mainCat).IsGenerated)
                ((Category)mainCat).ClearChildren();

            var clipList = new float[] {
                .05f,
                .01f,
                .001f,
                .0001f
            };

            foreach (var clip in clipList)
            {
                if (clip == .05f && Main.replace05withNumpad.Value)
                {
                    ((Category)mainCat).AddButton($"Custom Nearclip", "nearclip-Keypad", "Set a custom Nearclip. Be cafeful when setting to values > 0.05<p>Range 0.0001 - .5 meters").OnPress += () =>
                    {
                        QuickMenuAPI.OpenNumberInput("Custom Nearclip", 0f, (action) =>
                        { //Make this default to .05 in the future, after bug with BTKUI is resolved~ https://github.com/BTK-Development/BTKUILib/issues/11
                            var value = (action < .0001f) ? .0001f : (action > .5f) ? .5f : action;
                            Main.ChangeNearClipPlane(value, true);
                        });
                    };
                    continue;
                }

                var butt = ((Category)mainCat).AddButton($"{clip}", clip.ToString().Replace("0.", ""), $"Sets Nearclipping plane to {clip}");
                butt.OnPress += () =>
                {
                    Main.ChangeNearClipPlane(clip, true);
                };
            }
        }
    }
}