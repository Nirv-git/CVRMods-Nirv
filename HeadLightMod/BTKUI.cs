using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using BTKUILib.UIObjects;
using System.Collections.Generic;
using MelonLoader;
using System.Linq;
using Semver;

namespace HeadLightMod
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Settings", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Settings.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-flashLightColors", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.flashLight.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-AngleMinus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.AngleMinus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-AnglePlus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.AnglePlus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-BrightnessHigher", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.BrightnessHigher.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-BrightnessLower", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.BrightnessLower.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-SizeMinus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.SizeMinus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-SizePlus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.SizePlus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Reset.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Red-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Red-Minus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Red-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Red-Plus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Green-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Green-Minus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Green-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Green-Plus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Blue-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Blue-Minus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Blue-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Blue-Plus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-White-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.White-Minus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-White-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.White-Plus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Color-Blue", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Blue.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Color-Cyan", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Cyan.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Color-Green", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Green.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Color-Magenta", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Magenta.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Color-Red", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Red.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Color-Yellow", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Yellow.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Color-White", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-White.png"));
            QuickMenuAPI.PrepareIcon(ModName, "headLight-Color-Black", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Black.png"));
        }

        public static string ModName = "NirvBTKUI";
        private static MethodInfo _btkGetCreatePageAdapter;

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
                ModName = "headLightMod";
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
                cat = page.AddCategory("Head Light");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("Head Light", ModName);
            }
            var toggle = cat.AddToggle("Toggle Light", "Enable/disables head mounted light", false);
            toggle.OnValueUpdated += action =>
            {
                Main.ToggleLight(action);
            };
            var subPageSettings = cat.AddPage("Settings", "headLight-Settings", "Settings for light", ModName);
            var subPageColors = cat.AddPage("Colors", "headLight-flashLightColors", "Colors for light", ModName);

            subPageColors.MenuTitle = "Headlight Color";
            subPageSettings.MenuTitle = "Headlight Settings";

            void SetSettingsSub()
            {
                subPageSettings.MenuSubtitle = $"Intensity:{Utils.NumFormat(Main.Config.lightIntensity)}" +
                    $" Range:{Utils.NumFormat(Main.Config.lightRange)} SpotAngle:{Utils.NumFormat(Main.Config.lightSpotAngle)}" +
                    $" Spot/Point:{(Main.Config.lightType == LightType.Point ? "Point" : "Spot")}";
            }
            SetSettingsSub();

            var catSettings1 = subPageSettings.AddCategory("");
            var catSettings2 = subPageSettings.AddCategory("");
            var catSettings3 = subPageSettings.AddCategory("");    
            //Settings
            {
                var buttonIntenPlus = catSettings1.AddButton("Intensity +", "headLight-BrightnessHigher", "Brighten");
                buttonIntenPlus.OnPress += () =>
                {
                    Main.Config.lightIntensity += .1f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonIntenMinus = catSettings2.AddButton("Intensity -", "headLight-BrightnessLower", "Dim");
                buttonIntenMinus.OnPress += () =>
                {
                    Main.Config.lightIntensity = Utils.Clamp(Main.Config.lightIntensity - .1f, 0, 1000); Main.UpdateLight(); SetSettingsSub();
                };          
                var buttonRangePlus = catSettings1.AddButton("Range +", "headLight-SizePlus", "Increase Range");
                buttonRangePlus.OnPress += () =>
                {
                    Main.Config.lightRange += 1f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonRangeMinus = catSettings2.AddButton("Range -", "headLight-SizeMinus", "Lower Range");
                buttonRangeMinus.OnPress += () =>
                {
                    Main.Config.lightRange = Utils.Clamp(Main.Config.lightRange - 1f, 0, 2000); Main.UpdateLight(); SetSettingsSub();
                };
                var buttonAnglePlus = catSettings1.AddButton("Angle +", "headLight-AnglePlus", "Widen Angle");
                buttonAnglePlus.OnPress += () =>
                {
                    Main.Config.lightSpotAngle += 5f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonAngleMinus = catSettings2.AddButton("Angle -", "headLight-AngleMinus", "Narrow Angle");
                buttonAngleMinus.OnPress += () =>
                {
                    Main.Config.lightSpotAngle = Utils.Clamp(Main.Config.lightSpotAngle - 5f, 0, 2000); Main.UpdateLight(); SetSettingsSub();
                };
                var buttonResetIntense = catSettings3.AddButton("Reset Brightness", "headLight-Reset", "Reset Brightness");
                buttonResetIntense.OnPress += () =>
                {
                    Main.Config.lightIntensity = 1f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonResetRange = catSettings3.AddButton("Reset Range", "headLight-Reset", "Reset Range");
                buttonResetRange.OnPress += () =>
                {
                    Main.Config.lightRange = 10f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonResetAngle = catSettings3.AddButton("Reset Angle", "headLight-Reset", "Reset Angle");
                buttonResetAngle.OnPress += () =>
                {
                    Main.Config.lightSpotAngle = 40f; Main.UpdateLight(); SetSettingsSub();
                };

                var toggleSpotPoint = catSettings1.AddToggle("Point/Spot", "False-Spot Light | True-Point Light", Main.Config.lightType == LightType.Point);
                toggleSpotPoint.OnValueUpdated += action =>
                {
                    if (action)
                        Main.Config.lightType = LightType.Point;
                    else
                        Main.Config.lightType = LightType.Spot;
                    Main.UpdateLight(); SetSettingsSub();
                };
            }


            var catColors = subPageColors.AddCategory("Colors");
            void SetCustomColorsSub()
            {
                subPageColors.MenuSubtitle = $"Current color - R:{Utils.NumFormat(Main.Config.lightColor.r)}" +
                    $" G:{Utils.NumFormat(Main.Config.lightColor.g)} B:{Utils.NumFormat(Main.Config.lightColor.b)}";
            }
            subPageColors.MenuTitle = "Headlight Color";
            SetCustomColorsSub();
            //Color Presets
            {
                var buttonWhite = catColors.AddButton("White", "headLight-Color-White", "");
                buttonWhite.OnPress += () =>
                {
                    Main.Config.lightColor = Color.white; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonRed = catColors.AddButton("Red", "headLight-Color-Red", "");
                buttonRed.OnPress += () =>
                {
                    Main.Config.lightColor = Color.red; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonGreen = catColors.AddButton("Green", "headLight-Color-Green", ""); SetCustomColorsSub();
                buttonGreen.OnPress += () =>
                {
                    Main.Config.lightColor = Color.green; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBlue = catColors.AddButton("Blue", "headLight-Color-Blue", "");
                buttonBlue.OnPress += () =>
                {
                    Main.Config.lightColor = Color.blue; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBlack = catColors.AddButton("Black", "headLight-Color-Black", "");
                buttonBlack.OnPress += () =>
                {
                    Main.Config.lightColor = Color.black; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonMagenta = catColors.AddButton("Magenta", "headLight-Color-Magenta", "");
                buttonMagenta.OnPress += () =>
                {
                    Main.Config.lightColor = Color.magenta; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonYellow = catColors.AddButton("Yellow", "headLight-Color-Yellow", "");
                buttonYellow.OnPress += () =>
                {
                    Main.Config.lightColor = Color.yellow; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonCyan = catColors.AddButton("Cyan", "headLight-Color-Cyan", "");
                buttonCyan.OnPress += () =>
                {
                    Main.Config.lightColor = Color.cyan; Main.UpdateLight(); SetCustomColorsSub();
                };
            }
            
            var catCustomColors = subPageColors.AddCategory("Custom Colors");
            //Colors Plus
            {
                var buttonRedPlus = catCustomColors.AddButton("Red +", "headLight-Red-Plus", "");
                buttonRedPlus.OnPress += () =>
                {
                    Main.Config.lightColor.r = Utils.Clamp(Main.Config.lightColor.r + .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonGreenPlus = catCustomColors.AddButton("Green +", "headLight-Green-Plus", "");
                buttonGreenPlus.OnPress += () =>
                {
                    Main.Config.lightColor.g = Utils.Clamp(Main.Config.lightColor.g + .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBluePlus = catCustomColors.AddButton("Blue +", "headLight-Blue-Plus", "");
                buttonBluePlus.OnPress += () =>
                {
                    Main.Config.lightColor.b = Utils.Clamp(Main.Config.lightColor.b + .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonAllPlus = catCustomColors.AddButton("All +", "headLight-White-Plus", "");
                buttonAllPlus.OnPress += () =>
                {
                    Main.Config.lightColor.r = Utils.Clamp(Main.Config.lightColor.r + .1f, 0f, 2f);
                    Main.Config.lightColor.g = Utils.Clamp(Main.Config.lightColor.g + .1f, 0f, 2f);
                    Main.Config.lightColor.b = Utils.Clamp(Main.Config.lightColor.b + .1f, 0f, 2f);
                    Main.UpdateLight(); SetCustomColorsSub();
                };
            }
            //Colors Minus
            {
                var buttonRedMinus = catCustomColors.AddButton("Red -", "headLight-Red-Minus", "");
                buttonRedMinus.OnPress += () =>
                {
                    Main.Config.lightColor.r = Utils.Clamp(Main.Config.lightColor.r - .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonGreenMinus = catCustomColors.AddButton("Green -", "headLight-Green-Minus", "");
                buttonGreenMinus.OnPress += () =>
                {
                    Main.Config.lightColor.g = Utils.Clamp(Main.Config.lightColor.g - .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBlueMinus = catCustomColors.AddButton("Blue -", "headLight-Blue-Minus", "");
                buttonBlueMinus.OnPress += () =>
                {
                    Main.Config.lightColor.b = Utils.Clamp(Main.Config.lightColor.b - .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonAllMinus = catCustomColors.AddButton("All -", "headLight-White-Minus", "");
                buttonAllMinus.OnPress += () =>
                {
                    Main.Config.lightColor.r = Utils.Clamp(Main.Config.lightColor.r - .1f, 0f, 2f);
                    Main.Config.lightColor.g = Utils.Clamp(Main.Config.lightColor.g - .1f, 0f, 2f);
                    Main.Config.lightColor.b = Utils.Clamp(Main.Config.lightColor.b - .1f, 0f, 2f);
                    Main.UpdateLight(); SetCustomColorsSub();
                };
            }
            //Colors Reset
            {
                var buttonRedReset = catCustomColors.AddButton("Red Reset", "headLight-Reset", "");
                buttonRedReset.OnPress += () =>
                {
                    Main.Config.lightColor.r = 1f; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonGreenReset = catCustomColors.AddButton("Green Reset", "headLight-Reset", "");
                buttonGreenReset.OnPress += () =>
                {
                    Main.Config.lightColor.g = 1f; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBlueReset = catCustomColors.AddButton("Blue Reset", "headLight-Reset", "");
                buttonBlueReset.OnPress += () =>
                {
                    Main.Config.lightColor.b = 1f; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonAllReset = catCustomColors.AddButton("All Reset", "headLight-Reset", "");
                buttonAllReset.OnPress += () =>
                {
                    Main.Config.lightColor.r = 1f;
                    Main.Config.lightColor.g = 1f;
                    Main.Config.lightColor.b = 1f;
                    Main.UpdateLight(); SetCustomColorsSub();
                };
            }
        }
    }
}


                   