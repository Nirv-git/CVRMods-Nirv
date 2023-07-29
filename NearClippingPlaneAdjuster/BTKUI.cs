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
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "f500", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.f500.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "f1000", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.f1000.png"));
            QuickMenuAPI.PrepareIcon("NearClipPlaneAdj", "f20000", Assembly.GetExecutingAssembly().GetManifestResourceStream("NearClipPlaneAdj.Icons.btk.f20000.png"));
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
                        QuickMenuAPI.OpenNumberInput("Custom Nearclip", .05f, (action) =>
                        { //Make this default to .05 in the future, after bug with BTKUI is resolved~ https://github.com/BTK-Development/BTKUILib/issues/11
                            //Done, 2023-07-25
                            var value = (action < .0001f) ? .0001f : (action > .5f) ? .5f : action;
                            var valValue = Main.ValidateNewNearClipPlane(value);
                            if (valValue == -1) return;
                            if (valValue != value)
                            {
                                QuickMenuAPI.ShowAlertToast($"Far to Near clip ratio too high, limting Near to: {valValue}", 5);
                                Main.ChangeNearClipPlane(valValue, true);
                            }
                            else
                                Main.ChangeNearClipPlane(value, true);
                        });
                    };
                    continue;
                }

                var butt = ((Category)mainCat).AddButton($"{clip}", clip.ToString().Replace("0.", ""), $"Sets Nearclipping plane to {clip}");
                butt.OnPress += () =>
                {
                    var valValue = Main.ValidateNewNearClipPlane(clip);
                    if (valValue == -1) return;
                    if (valValue != clip)
                    {
                        QuickMenuAPI.ShowAlertToast($"Far to Near clip ratio too high, limting Near to: {valValue}", 5);
                        Main.ChangeNearClipPlane(valValue, true);
                    }
                    else
                        Main.ChangeNearClipPlane(clip, true);
                };
            }

            if (Main.farClipControl.Value)
            {
                var farclipList = new float[] {
                1f,
                500f,
                1000f,
                20000f
                };

                foreach (var clip in farclipList)
                {
                    if (clip == 1)
                    {
                        ((Category)mainCat).AddButton($"Custom Farclip", "nearclip-Keypad", "Set a custom Farclip (1-9999)").OnPress += () =>
                        {
                            QuickMenuAPI.OpenNumberInput("Custom Farclip", 1f, (action) =>
                            {
                            var value = (action < 1f) ? 1f : action;
                            var valValue = Main.ValidateNewFarClipPlane(value);
                            if (valValue == -1) return;
                            if (valValue != value)
                            {
                                QuickMenuAPI.ShowAlertToast($"Far to Near clip ratio too high, limting Far to: {valValue}", 5);
                                Main.ChangeFarClipPlane(valValue, true);
                            }
                            else
                                Main.ChangeFarClipPlane(value, true);
                            });
                        };
                        continue;
                    }
                    var butt = ((Category)mainCat).AddButton($"Farclip: {clip}", $"f{clip}" , $"Sets Farclipping plane to {clip}");
                    butt.OnPress += () =>
                    {
                        var valValue = Main.ValidateNewFarClipPlane(clip);
                        if (valValue == -1) return;
                        if (valValue != clip)
                        {
                            QuickMenuAPI.ShowAlertToast($"Far to Near clip ratio too high, limting Far to: {valValue}", 5);
                            Main.ChangeFarClipPlane(valValue, true);
                        }
                        else
                            Main.ChangeFarClipPlane(clip, true);
                    };
                }
            }

        }
    }
}