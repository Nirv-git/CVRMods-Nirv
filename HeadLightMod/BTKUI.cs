using System.Reflection;
using UnityEngine;
using System.IO;
using BTKUILib;
using System.Collections.Generic;

namespace HeadLightMod
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Settings", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Settings.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "flashLightColors", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.flashLight.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "AngleMinus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.AngleMinus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "AnglePlus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.AnglePlus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "BrightnessHigher", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.BrightnessHigher.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "BrightnessLower", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.BrightnessLower.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "SizeMinus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.SizeMinus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "SizePlus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.SizePlus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Reset.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Red-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Red-Minus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Red-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Red-Plus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Green-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Green-Minus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Green-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Green-Plus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Blue-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Blue-Minus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Blue-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Blue-Plus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "White-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.White-Minus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "White-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.White-Plus.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Color-Blue", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Blue.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Color-Cyan", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Cyan.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Color-Green", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Green.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Color-Magenta", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Magenta.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Color-Red", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Red.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Color-Yellow", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Yellow.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Color-White", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-White.png"));
            QuickMenuAPI.PrepareIcon("HeadLightMod", "Color-Black", Assembly.GetExecutingAssembly().GetManifestResourceStream("HeadLightMod.Icons.Color-Black.png"));
        }

        public static void InitUi()
        {
            loadAssets();

            var cat = QuickMenuAPI.MiscTabPage.AddCategory("Head Light", "HeadLightMod");
            var toggle = cat.AddToggle("Toggle Light", "Enable/disables head mounted light", !Main.baseObj?.Equals(null) ?? false);
            toggle.OnValueUpdated += action =>
            {
                Main.ToggleLight(action);
            };

            var subPageSettings = cat.AddPage("Settings", "Settings", "Settings for light", "HeadLightMod");
            var subPageColors = cat.AddPage("Colors", "flashLightColors", "Colors for light", "HeadLightMod");

            subPageSettings.MenuTitle = "Headlight Settings";
            void SetSettingsSub()
            {
                subPageSettings.MenuSubtitle = $"Intensity:{Utils.NumFormat(Main.Config.lightIntensity)}" +
                    $" Range:{Utils.NumFormat(Main.Config.lightRange)} SpotAngle:{Utils.NumFormat(Main.Config.lightSpotAngle)}" +
                    $" Spot/Point:{(Main.Config.lightType == LightType.Point ? "Point" : "Spot")}";
            }
            subPageColors.MenuTitle = "Headlight Color";
            SetSettingsSub();
            var catSettings1 = subPageSettings.AddCategory("");
            var catSettings2 = subPageSettings.AddCategory("");
            var catSettings3 = subPageSettings.AddCategory("");    
            //Settings
            {
                var buttonIntenPlus = catSettings1.AddButton("Intensity +", "BrightnessHigher", "Brighten");
                buttonIntenPlus.OnPress += () =>
                {
                    Main.Config.lightIntensity += .1f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonIntenMinus = catSettings2.AddButton("Intensity -", "BrightnessLower", "Dim");
                buttonIntenMinus.OnPress += () =>
                {
                    Main.Config.lightIntensity = Utils.Clamp(Main.Config.lightIntensity - .1f, 0, 1000); Main.UpdateLight(); SetSettingsSub();
                };
                var buttonAnglePlus = catSettings1.AddButton("Angle +", "AnglePlus", "Widen Angle");
                buttonAnglePlus.OnPress += () =>
                {
                    Main.Config.lightSpotAngle += 5f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonAngleMinus = catSettings2.AddButton("Angle -", "AngleMinus", "Narrow Angle");
                buttonAngleMinus.OnPress += () =>
                {
                    Main.Config.lightSpotAngle = Utils.Clamp(Main.Config.lightSpotAngle - 5f, 0, 2000); Main.UpdateLight(); SetSettingsSub();
                };
                var buttonRangePlus = catSettings1.AddButton("Range +", "SizePlus", "Increase Range");
                buttonRangePlus.OnPress += () =>
                {
                    Main.Config.lightRange += 1f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonRangeMinus = catSettings2.AddButton("Range -", "SizeMinus", "Lower Range");
                buttonRangeMinus.OnPress += () =>
                {
                    Main.Config.lightRange = Utils.Clamp(Main.Config.lightRange - 1f, 0, 2000); Main.UpdateLight(); SetSettingsSub();
                };
                var buttonResetIntense = catSettings3.AddButton("Reset Brightness", "Reset", "Reset Brightness");
                buttonResetIntense.OnPress += () =>
                {
                    Main.Config.lightIntensity = 1f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonResetAngle = catSettings3.AddButton("Reset Angle", "Reset", "Reset Angle");
                buttonResetAngle.OnPress += () =>
                {
                    Main.Config.lightSpotAngle = 40f; Main.UpdateLight(); SetSettingsSub();
                };
                var buttonResetRange = catSettings3.AddButton("Reset Range", "Reset", "Reset Range");
                buttonResetRange.OnPress += () =>
                {
                    Main.Config.lightRange = 10f; Main.UpdateLight(); SetSettingsSub();
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
                var buttonWhite = catColors.AddButton("White", "Color-White", "");
                buttonWhite.OnPress += () =>
                {
                    Main.Config.lightColor = Color.white; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonRed = catColors.AddButton("Red", "Color-Red", "");
                buttonRed.OnPress += () =>
                {
                    Main.Config.lightColor = Color.red; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonGreen = catColors.AddButton("Green", "Color-Green", ""); SetCustomColorsSub();
                buttonGreen.OnPress += () =>
                {
                    Main.Config.lightColor = Color.green; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBlue = catColors.AddButton("Blue", "Color-Blue", "");
                buttonBlue.OnPress += () =>
                {
                    Main.Config.lightColor = Color.blue; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBlack = catColors.AddButton("Black", "Color-Black", "");
                buttonBlack.OnPress += () =>
                {
                    Main.Config.lightColor = Color.black; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonMagenta = catColors.AddButton("Magenta", "Color-Magenta", "");
                buttonMagenta.OnPress += () =>
                {
                    Main.Config.lightColor = Color.magenta; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonYellow = catColors.AddButton("Yellow", "Color-Yellow", "");
                buttonYellow.OnPress += () =>
                {
                    Main.Config.lightColor = Color.yellow; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonCyan = catColors.AddButton("Cyan", "Color-Cyan", "");
                buttonCyan.OnPress += () =>
                {
                    Main.Config.lightColor = Color.cyan; Main.UpdateLight(); SetCustomColorsSub();
                };
            }
            
            var catCustomColors = subPageColors.AddCategory("Custom Colors");
            //Colors Plus
            {
                var buttonRedPlus = catCustomColors.AddButton("Red +", "Red-Plus", "");
                buttonRedPlus.OnPress += () =>
                {
                    Main.Config.lightColor.r = Utils.Clamp(Main.Config.lightColor.r + .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonGreenPlus = catCustomColors.AddButton("Green +", "Green-Plus", "");
                buttonGreenPlus.OnPress += () =>
                {
                    Main.Config.lightColor.g = Utils.Clamp(Main.Config.lightColor.g + .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBluePlus = catCustomColors.AddButton("Blue +", "Blue-Plus", "");
                buttonBluePlus.OnPress += () =>
                {
                    Main.Config.lightColor.b = Utils.Clamp(Main.Config.lightColor.b + .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonAllPlus = catCustomColors.AddButton("All +", "White-Plus", "");
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
                var buttonRedMinus = catCustomColors.AddButton("Red -", "Red-Minus", "");
                buttonRedMinus.OnPress += () =>
                {
                    Main.Config.lightColor.r = Utils.Clamp(Main.Config.lightColor.r - .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonGreenMinus = catCustomColors.AddButton("Green -", "Green-Minus", "");
                buttonGreenMinus.OnPress += () =>
                {
                    Main.Config.lightColor.g = Utils.Clamp(Main.Config.lightColor.g - .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBlueMinus = catCustomColors.AddButton("Blue -", "Blue-Minus", "");
                buttonBlueMinus.OnPress += () =>
                {
                    Main.Config.lightColor.b = Utils.Clamp(Main.Config.lightColor.b - .1f, 0f, 2f); Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonAllMinus = catCustomColors.AddButton("All -", "White-Minus", "");
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
                var buttonRedReset = catCustomColors.AddButton("Red Reset", "Reset", "");
                buttonRedReset.OnPress += () =>
                {
                    Main.Config.lightColor.r = 1f; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonGreenReset = catCustomColors.AddButton("Green Reset", "Reset", "");
                buttonGreenReset.OnPress += () =>
                {
                    Main.Config.lightColor.g = 1f; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonBlueReset = catCustomColors.AddButton("Blue Reset", "Reset", "");
                buttonBlueReset.OnPress += () =>
                {
                    Main.Config.lightColor.b = 1f; Main.UpdateLight(); SetCustomColorsSub();
                };
                var buttonAllReset = catCustomColors.AddButton("All Reset", "Reset", "");
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


                   