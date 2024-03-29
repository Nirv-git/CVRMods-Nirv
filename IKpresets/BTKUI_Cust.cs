﻿using System;
using MelonLoader;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using BTKUILib;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Objects;
using ABI_RC.Core.Savior;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using ABI_RC.Core.Player;
using ABI_RC.Core;
using Semver;

namespace IKpresetsMod
{
    class BTKUI_Cust
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-ConfigIK", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.ConfigIK.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-IKSaveLoad", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.IKSaveLoad.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-Load", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Load.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Reset.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-Save", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Save.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-Select", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Select.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-White-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.White-Minus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-White-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.White-Plus.png")); 
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-Recal", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Recal.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-Delete", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Delete.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-IKSaveLoadSlots", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.IKSaveLoadSlots.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-Calibration", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Calibration.png"));
            QuickMenuAPI.PrepareIcon(ModName, "ikPre-Edit", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Edit.png"));
        }

        public static string ModName = "NirvBTKUI";
        private static MethodInfo _btkGetCreatePageAdapter;

        public static Page pageEditSettings, pageSaveLoad, pageSetName, pageEditSlotName, pageAvatarSaveLoad;
        public static BTKUILib.UIObjects.Category pageEditSlotName_Text;
       

        public static void SaveChanges()
        {
            MetaPort.Instance.SaveGameConfig();
            ViewManager.Instance.gameMenuView.View.TriggerEvent("CVRAppActionLoadSettings");
        }


        public static void SetupUI()
        {
            if (MelonMod.RegisteredMelons.Any(x => x.Info.Name.Equals("BTKUILib") && x.Info.SemanticVersion.CompareByPrecedence(new SemVersion(1, 9)) > 0))
            {
                //We're working with UILib 2.0.0, let's reflect the get create page function
                _btkGetCreatePageAdapter = typeof(Page).GetMethod("GetOrCreatePage", BindingFlags.Public | BindingFlags.Static);
                Main.Logger.Msg($"BTKUILib 2.0.0 detected, attempting to grab GetOrCreatePage function: {_btkGetCreatePageAdapter != null}");
            }
            if (!Main.useNirvMiscPage.Value)
            {
                ModName = "ikPresetsMod";
            }

            loadAssets();
            pageEditSettings = new Page(ModName, "IK Presets - Editing", false);
            QuickMenuAPI.AddRootPage(pageEditSettings);
            pageSaveLoad = new Page(ModName, "IK Presets - Save/Load", false);
            QuickMenuAPI.AddRootPage(pageSaveLoad);
            pageSetName = new Page(ModName, "IK Presets - Set Name", false);
            QuickMenuAPI.AddRootPage(pageSetName);
            pageEditSlotName = new Page(ModName, "IK Presets - Edit slot names", false);
            QuickMenuAPI.AddRootPage(pageEditSlotName);
            pageAvatarSaveLoad = new Page(ModName, "IK Presets - Avatar Slots", false);
            QuickMenuAPI.AddRootPage(pageAvatarSaveLoad);
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
                cat = page.AddCategory("IK Presets");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("IK Presets", ModName);
            }
            cat.AddButton("Edit current values", "ikPre-ConfigIK", "Edit current IK settings. Same options as in the big menu").OnPress += () =>
            {
                EditSettings();
            };
            cat.AddButton("Per Avatar Presets", "ikPre-IKSaveLoad", "Presets for specific avatars").OnPress += () =>
            {
                AvatarSaveLoad();
            };
            cat.AddButton("Save/Load Slots", "ikPre-IKSaveLoadSlots", "Save and load preset slots").OnPress += () =>
            {
                SaveLoad(true);
            };
            cat.AddButton("ReCalibrate", "ikPre-Recal", "ReCalibrate Avatar").OnPress += () =>
            {
                PlayerSetup.Instance.ReCalibrateAvatar();
            };

            
        }

        public static void EditSettings()
        {
            var page = pageEditSettings;
            page.ClearChildren();
            page.MenuTitle = "Edit Settings";
            page.MenuSubtitle = "";

            var toggles = page.AddCategory("Toggles (Default Value)");
            //Toggles
            {
                {
                    string setting = "IKPitchYawShoulders";
                    string desc = "Pitch-Yaw Shoulders - changes how shoulder angles are computed in 3/4-point tracking. Enabling it usually provides better-looking results";
                    toggles.AddToggle("Pitch Yaw Shoulders (True)", desc, MetaPort.Instance.settings.GetSettingsBool(setting)).OnValueUpdated += action =>
                    {
                        MetaPort.Instance.settings.SetSettingsBool(setting, action);
                        SaveChanges();
                    };
                }
                {
                    string setting = "IKPlantFeet";
                    string desc = "Feet stick to ground - uncheck if you want your feet (and the rest of your avatar) to be unable to leave the ground";
                    toggles.AddToggle("Plant Feet (False)", desc, MetaPort.Instance.settings.GetSettingsBool(setting)).OnValueUpdated += action =>
                    {
                        MetaPort.Instance.settings.SetSettingsBool(setting, action);
                        SaveChanges();
                    };
                }
                {
                    string setting = "IKHipPinned";
                    string desc = "Enforce hip rotation match - if enabled, avatar's hip rotation will exactly match tracker's rotation. Otherwise, IK may rotate the hip to bend the spine more.";
                    toggles.AddToggle("Hip Pinned (True)", desc, MetaPort.Instance.settings.GetSettingsBool(setting)).OnValueUpdated += action =>
                    {
                        MetaPort.Instance.settings.SetSettingsBool(setting, action);
                        SaveChanges();
                    };
                }
                {
                    string setting = "IKStraightenNeck";
                    string desc = "Straighten neck - this does something cursed to the neck. No further description can be provided.";
                    toggles.AddToggle("Straighten Neck (False)", desc, MetaPort.Instance.settings.GetSettingsBool(setting)).OnValueUpdated += action =>
                    {
                        MetaPort.Instance.settings.SetSettingsBool(setting, action);
                        SaveChanges();
                    };
                }
                {
                    string setting = "IKHipShifting";
                    string desc = "Shift hip pivot (support inverted hip) The hip will be rotated around the midpoint of two leg bones (where the hip bone should be normally). This greatly improves IK on avatars with the inverted hip rig hack.";
                    toggles.AddToggle("Hip Shifting (True)", desc, MetaPort.Instance.settings.GetSettingsBool(setting)).OnValueUpdated += action =>
                    {
                        MetaPort.Instance.settings.SetSettingsBool(setting, action);
                        SaveChanges();
                    };
                }
                {
                    string setting = "IKPreStraightenSpine";
                    string desc = "Pre-straighten spine (improve IK stability) - if enabled, you avatar's spine will be forcefully straightened before solving it. This reduces flippiness/jitter on avatars that have spine bent backwards by default.";
                    toggles.AddToggle("Pre-Straighten Spine (False)", desc, MetaPort.Instance.settings.GetSettingsBool(setting)).OnValueUpdated += action =>
                    {
                        MetaPort.Instance.settings.SetSettingsBool(setting, action);
                        SaveChanges();
                    };
                }
                {
                    string setting = "GeneralEnableRunningAnimationFullBody";
                    string desc = "Enables running animations while using Fully body tracking";
                    toggles.AddToggle("Running Animation in FullBody (False)", desc, MetaPort.Instance.settings.GetSettingsBool(setting)).OnValueUpdated += action =>
                    {
                        MetaPort.Instance.settings.SetSettingsBool(setting, action);
                        SaveChanges();
                    };
                }

                {
                    var calMode = toggles.AddButton("Calibration Mode", "ikPre-Calibration", "Select what calibration mode should be used"); ////0 InPlace, 1 FollowHead, 2FullyFollowHead
                    var multiSel_calMode = new MultiSelection("Calibration Mode", new string[] { "In Place", "Follow Head", "Fully FollowHead" }, MetaPort.Instance.settings.GetSettingInt("IKCalibrationMode"));
                    multiSel_calMode.OnOptionUpdated += value => MetaPort.Instance.settings.SetSettingsInt("IKCalibrationMode", value);
                    SaveChanges();
                    calMode.OnPress += () => BTKUILib.QuickMenuAPI.OpenMultiSelect(multiSel_calMode);

                }
            }


            //Int/Float
            {
                //IKSpineRelaxIterations
                {
                    string setting = "IKSpineRelaxIterations";
                    string desc = "Spine Relax Iterations - How much work will be done on bending the spine. Below 5 is not recommended, 10 will provide about 1mm precision for hip positioning, 25 is the maximum sensible value.";
                    int value = MetaPort.Instance.settings.GetSettingInt(setting);

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Spine Relax Iterations: {value}"; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 5 : 1), 0, 25)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 10", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 10); UpdateSet(); SaveChanges();
                        //MetaPort.Instance.settings.SetSetting(setting, 10.ToString(), true);
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value + (fast ? 5 : 1), 0, 25)); UpdateSet(); SaveChanges();
                        //MetaPort.Instance.settings.SetSetting(setting, 13.ToString(), false);
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }

                //IKMaxSpineAngleFwd
                {
                    string setting = "IKMaxSpineAngleFwd";
                    string desc = "Max bend angles - how much spine/neck can be bent forward/back. If your spine bends too much to your taste or looks cursed on your specific avatar, reduce these angles (min recommended value is 1 though)";
                    int value = MetaPort.Instance.settings.GetSettingInt(setting);

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Max Spine Angle Fwd: {value}"; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 30", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 30); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value + (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }

                //IKMaxSpineAngleFwd
                {
                    string setting = "IKMaxSpineAngleBack";
                    string desc = "Max bend angles - how much spine/neck can be bent forward/back. If your spine bends too much to your taste or looks cursed on your specific avatar, reduce these angles (min recommended value is 1 though)";
                    int value = MetaPort.Instance.settings.GetSettingInt(setting);

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Max Spine Angle Back: {value}"; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 30", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 30); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value + (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }
                //IKMaxNeckAngleFwd
                {
                    string setting = "IKMaxNeckAngleFwd";
                    string desc = "Max bend angles - how much spine/neck can be bent forward/back. If your spine bends too much to your taste or looks cursed on your specific avatar, reduce these angles (min recommended value is 1 though)";
                    int value = MetaPort.Instance.settings.GetSettingInt(setting);

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Max Neck Angle Fwd: {value}"; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 30", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 30); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value + (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }

                //IKMaxNeckAngleBack
                {
                    string setting = "IKMaxNeckAngleBack";
                    string desc = "Max bend angles - how much spine/neck can be bent forward/back. If your spine bends too much to your taste or looks cursed on your specific avatar, reduce these angles (min recommended value is 1 though)";
                    int value = MetaPort.Instance.settings.GetSettingInt(setting);

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Max Neck Angle Back: {value}"; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 15", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 15); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value + (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }
                //IKNeckPriority
                {
                    string setting = "IKNeckPriority";
                    string desc = "Neck bend priority - neck will bend this much faster than the spine. This is intended to handle the fact that people move their neck way more than their spine, so IK should start off by bending it, not spine.";
                    int value = MetaPort.Instance.settings.GetSettingInt(setting);

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Neck Priority: {value}"; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 2 : 1), 1, 10)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 2", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 2); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value + (fast ? 2 : 1), 1, 10)); UpdateSet(); SaveChanges();
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }

                //IKStraightSpineAngle
                {
                    string setting = "IKStraightSpineAngle";
                    string desc = "Straight spine angle - withing this angle from perfectly straight, the spine will be considered almost straight and Max bend angles will be reduced.";
                    int value = MetaPort.Instance.settings.GetSettingInt(setting);

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Straight Spine Angle: {value}"; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 15", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 15); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value + (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }
                //IKStraightSpinePower
                {
                    string setting = "IKStraightSpinePower";
                    string desc = "Straight spine power - controls the curve with which the spine transitions from straight to bend within the straight angle.";
                    int value = MetaPort.Instance.settings.GetSettingInt(setting);

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Straight Spine Power: {value}"; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 2 : 1), 1, 10)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 2", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 2); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value + (fast ? 2 : 1), 1, 10)); UpdateSet(); SaveChanges();
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }
                //IKTrackingSmoothing ----- Float
                {
                    string setting = "IKTrackingSmoothing";
                    string desc = "Smoothening applied to trackers to reduce jitter. High values will delay tracking";
                    float value = MetaPort.Instance.settings.GetSettingsFloat(setting);

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Tracking Smoothing: {value}"; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingsFloat(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsFloat(setting, Utils.Clamp(value - (fast ? 5f : 1f), 0f, 50f)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 5", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsFloat(setting, 5); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsFloat(setting, Utils.Clamp(value + (fast ? 5f : 1f), 0f, 50f)); UpdateSet(); SaveChanges();
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }

                {
                    string setting = "GeneralPlayerHeight";
                    string desc = "Player Height as set in General Settings";
                    int value = MetaPort.Instance.settings.GetSettingInt(setting);

                    var cat = page.AddCategory(catText());
                    string catText() {
                        //https://stackoverflow.com/a/22590456
                        int totalInches = (int)(value / 2.54); // This will take a floor function of Centimetres/2.54
                        int Feet = (totalInches - totalInches % 12) / 12; // This will make it divisible by 12
                        int inches = totalInches % 12; // This will give you the remainder after you divide by 12
                        return $"Player Height: {value}cm - {Feet}\' {inches}\""; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "ikPre-White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 5 : 1), 1, 305)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton($"Reset to {value}", "ikPre-Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, value); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "ikPre-White-Plus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value + (fast ? 5 : 1), 1, 305)); UpdateSet(); SaveChanges();
                    };
                    cat.AddToggle("Quick Adj", "Faster adjust", fast).OnValueUpdated += action =>
                    {
                        fast = action;
                    };
                }
            }
           
            page.OpenPage();
        }

        private static void SaveLoad(bool open)
        {//type T|F - Pos|Rot
            var page = pageSaveLoad;
            page.ClearChildren();
            page.MenuTitle = "Save/Load IK Presets";
            page.MenuSubtitle = Text();

            var catMain = page.AddCategory("");
            catMain.AddButton("Edit Slot Names", "ikPre-Edit", "Edit Slot Names").OnPress += () =>
            {
                EditSlotNames();
            };
            catMain.AddButton("Refresh", "ikPre-Reset", "Refresh Names").OnPress += () =>
            {
                SaveLoad(true);
            };
            try
            {
                //var slotNames = Main.config_Slots.Settings;
                foreach (var slot in Main.config_Slots.Settings)
                {
                    string label = $"Slot: {slot.Key} - {slot.Value.SlotName}";
                    var cat = page.AddCategory(label, true, false);

                    var desc = $"PitchYaw:{Utils.CompactTF(slot.Value.IKPitchYawShoulders)}_HipPin:{Utils.CompactTF(slot.Value.IKHipPinned)}_" +
                       $"StrNeck:{Utils.CompactTF(slot.Value.IKStraightenNeck)}_HipShift:{Utils.CompactTF(slot.Value.IKHipShifting)}_StrSpine:{Utils.CompactTF(slot.Value.IKPreStraightenSpine)}_" +
                       $"RelxIter:{slot.Value.IKSpineRelaxIterations} SpineFwd:{slot.Value.IKMaxSpineAngleFwd}Bck:{slot.Value.IKMaxSpineAngleBack}_" +
                       $"NeckFwd:{slot.Value.IKMaxNeckAngleFwd}Bck:{slot.Value.IKMaxNeckAngleBack}_NeckPri:{slot.Value.IKNeckPriority}_" +
                       $"StrSpine:{slot.Value.IKStraightSpineAngle}Pow:{slot.Value.IKStraightSpinePower}Height:{slot.Value.GeneralPlayerHeight}";
                    cat.AddButton("Load", "ikPre-Load", desc).OnPress += () =>
                    {
                        if (slot.Value.SlotName == "N/A")
                        {
                            QuickMenuAPI.ShowConfirm("Loading from default slot name.", "This slot has no name set, are you sure you want to load from it?", () =>
                            {
                                SaveSlots.LoadSlot(slot.Key);
                                UpdateText();
                                SaveChanges();
                            }, () => { }, "Yes", "No");
                        }
                        else
                        {
                            SaveSlots.LoadSlot(slot.Key);
                            UpdateText();
                            SaveChanges();
                        }
                    };
                    cat.AddButton("Save", "ikPre-Save", "Save current IK settings to this slot").OnPress += () =>
                    {
                        SaveSlots.StoreSlot(slot.Key);
                        SaveLoad(true);
                    };
                }
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading slots\n" + ex.ToString());
                page.AddCategory("Error loading slots", true, false);
            }
            void UpdateText()
            {
                page.MenuSubtitle = Text();
            }
            string Text()
            {
                return $"PchYaw:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKPitchYawShoulders"))}_HipPin:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKHipPinned"))}_" +
                    $"StrNeck:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKStraightenNeck"))}_HipShft:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKHipShifting"))}_StrSpine:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKPreStraightenSpine"))}_" +
                    $"RelxIter:{MetaPort.Instance.settings.GetSettingInt("IKSpineRelaxIterations")}_SpineFwd:{MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleFwd")} Bck:{MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleBack")}_" +
                    $"NeckFwd:{MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleFwd")}Bck:{MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleBack")}_NeckPri:{MetaPort.Instance.settings.GetSettingInt("IKNeckPriority")}_" +
                    $"StrSpine:{MetaPort.Instance.settings.GetSettingInt("IKStraightSpineAngle")}Pow:{MetaPort.Instance.settings.GetSettingInt("IKStraightSpinePower")}_Hgt:{MetaPort.Instance.settings.GetSettingInt("GeneralPlayerHeight")}";// +
                    //$"Smooth:{MetaPort.Instance.settings.GetSettingsFloat("IKTrackingSmoothing")}";
            }
            if (open) page.OpenPage(); 
        }


        private static void EditSlotNames()
        {
            var page = pageEditSlotName;
            page.ClearChildren();
            page.MenuTitle = "Edit Slot Names";
            page.MenuSubtitle = "Used for changing the names of the slots";

            try
            {
                foreach (var slot in Main.config_Slots.Settings)
                {
                    string label = $"Slot: {slot.Key}\n{slot.Value.SlotName}";
                    var cat = page.AddCategory(label, true, false);

                    cat.AddButton("Edit String", "ikPre-Edit", "Edit String").OnPress += () =>
                    {
                        QuickMenuAPI.OpenKeyboard(slot.Value.SlotName, (action) => { slot.Value.SlotName = action; SaveSlots.SaveConfigSlots(); EditSlotNames(); SaveLoad(false); CVR_MenuManager.Instance.ToggleQuickMenu(true); });
                    };
                }
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading names\n" + ex.ToString());
                page.AddCategory("Error loading names", true, false);
            }
            page.OpenPage();
        }

       
        private static void AvatarSaveLoad()
        {
            var page = pageAvatarSaveLoad;
            page.ClearChildren();
            page.MenuTitle = "Avatar IK Presets";
            page.MenuSubtitle = Text();

            var catMain = page.AddCategory("");
            catMain.AddButton("Save this avatar config", "ikPre-Save", "Save current IK settings").OnPress += () =>
            {
                //Main.Logger.Msg($"{Main.avatarGUID} - {MetaPort.Instance.currentAvatarGuid} - {Main.avatarName}");
                SaveSlots.StoreAvatars(MetaPort.Instance.currentAvatarGuid, Main.avatarName);
                AvatarSaveLoad();
            };
        
            catMain.AddToggle("Auto load", "Auto load matching IK settings on avatar change", Main.autoLoadAvatarPresets.Value).OnValueUpdated += action =>
            {
                Main.autoLoadAvatarPresets.Value = action;
            };

            catMain.AddButton("Save Default config", "ikPre-Save", "Save current IK settings into a Default profile").OnPress += () =>
            {
                QuickMenuAPI.ShowConfirm("Save Default config", "Save current IK settings into a Default profile?", () =>
                {
                    SaveSlots.StoreAvatars("Default", "Default IK Settings");
                    AvatarSaveLoad();
                }, () => { }, "Yes", "No");  
            };
            catMain.AddToggle("Auto load Default", "Auto load default IK settings if avatar doesn't have saved profile<p>This and 'Auto load' need to be enabled and you must save a default profile", Main.autoLoadAvatarDefault.Value).OnValueUpdated += action =>
            {
                Main.autoLoadAvatarDefault.Value = action;
            };

            //catMain.AddButton("Refresh", "ikPre-Reset", "Refresh").OnPress += () =>
            //{
            //    AvatarSaveLoad();
            //};

            try
            {
                var currentID = MetaPort.Instance.currentAvatarGuid;
                if (Main.config_Avatars.Settings.ContainsKey(currentID))
                {
                    SlotLine(true, new System.Collections.Generic.KeyValuePair<string, Main.AvatarConfig.AvatarSettings>(currentID, Main.config_Avatars.Settings[currentID]));
                }
                else if (Main.config_Avatars.Settings.ContainsKey("Default"))
                {
                    SlotLine(true, new System.Collections.Generic.KeyValuePair<string, Main.AvatarConfig.AvatarSettings>("Default", Main.config_Avatars.Settings["Default"]));
                }       

                foreach (var slot in Main.config_Avatars.Settings.Reverse())
                {
                    if (slot.Key == currentID || slot.Key == "Default")
                        continue; //Don't repeat the current one if it matches
                    SlotLine(false, slot);
                }
                void SlotLine(bool current, System.Collections.Generic.KeyValuePair<string, Main.AvatarConfig.AvatarSettings>
                    slot)
                {
                    string label = $"{(current ? "*Current avatar* " : "")}{slot.Value.AvatarName} - {slot.Key}";
                    var cat = page.AddCategory(label, true, false);

                    var desc = $"PchYaw:{Utils.CompactTF(slot.Value.IKPitchYawShoulders)}_HipPin:{Utils.CompactTF(slot.Value.IKHipPinned)}_" +
                       $"StrNeck:{Utils.CompactTF(slot.Value.IKStraightenNeck)}_HipShft:{Utils.CompactTF(slot.Value.IKHipShifting)}_StrSpine:{Utils.CompactTF(slot.Value.IKPreStraightenSpine)}_" +
                       $"RelxIter:{slot.Value.IKSpineRelaxIterations}_SpFwd:{slot.Value.IKMaxSpineAngleFwd} Bck:{slot.Value.IKMaxSpineAngleBack}_" +
                       $"NeckFwd:{slot.Value.IKMaxNeckAngleFwd}Bck:{slot.Value.IKMaxNeckAngleBack}_NeckPri:{slot.Value.IKNeckPriority}_" +
                       $"StrSpine:{slot.Value.IKStraightSpineAngle}Pow:{slot.Value.IKStraightSpinePower}_Hgt:{slot.Value.GeneralPlayerHeight}";
                    cat.AddButton("Load", "ikPre-Load", desc).OnPress += () =>
                    {
                        SaveSlots.LoadAvatars(slot.Key);
                        SaveChanges();
                        UpdateText();
                    };
                    cat.AddButton("Delete", "ikPre-Delete", "Remove this slot").OnPress += () =>
                    {
                        SaveSlots.AvatarSlotDelete(slot.Key);
                        AvatarSaveLoad();
                    };
                }
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading avatar slots\n" + ex.ToString());
                page.AddCategory("Error loading avatar slots", true, false);
            }
            void UpdateText()
            {
                page.MenuSubtitle = Text();
            }
            string Text()
            {
                return $"PchYaw:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKPitchYawShoulders"))}_HipPin:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKHipPinned"))}_" +
                    $"StrNeck:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKStraightenNeck"))}_HipShft:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKHipShifting"))}_StrSpine:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKPreStraightenSpine"))}_" +
                    $"RelxIter:{MetaPort.Instance.settings.GetSettingInt("IKSpineRelaxIterations")}_SpineFwd:{MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleFwd")} Bck:{MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleBack")}_" +
                    $"NeckFwd:{MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleFwd")}Bck:{MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleBack")}_NeckPri:{MetaPort.Instance.settings.GetSettingInt("IKNeckPriority")}_" +
                    $"StrSpine:{MetaPort.Instance.settings.GetSettingInt("IKStraightSpineAngle")}Pow:{MetaPort.Instance.settings.GetSettingInt("IKStraightSpinePower")}_Hgt:{MetaPort.Instance.settings.GetSettingInt("GeneralPlayerHeight")}";// +
                                                                                                                                                                         //$"Smooth:{MetaPort.Instance.settings.GetSettingsFloat("IKTrackingSmoothing")}";
            }
            page.OpenPage();
        }
    }
}

