using System;
using MelonLoader;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using BTKUILib;
using BTKUILib.UIObjects;
using ABI_RC.Core.Savior;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using ABI_RC.Core.Player;
using ABI_RC.Core;

namespace IKpresetsMod
{
    class BTKUI_Cust
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("NirvMisc", "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "ConfigIK", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.ConfigIK.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "IKSaveLoad", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.IKSaveLoad.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "Load", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Load.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Reset.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "Save", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Save.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "Select", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Select.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "White-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.White-Minus.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "White-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.White-Plus.png")); 
            QuickMenuAPI.PrepareIcon("IKpresets", "Recal", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Recal.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "Delete", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.Delete.png"));
            QuickMenuAPI.PrepareIcon("IKpresets", "IKSaveLoadSlots", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons.IKSaveLoadSlots.png"));
            //QuickMenuAPI.PrepareIcon("IKpresets", "", Assembly.GetExecutingAssembly().GetManifestResourceStream("IKpresetsMod.Icons..png"));
        }

        public static Page pageEditSettings, pageSaveLoad, pageSetName, pageEditSlotName, pageAvatarSaveLoad;
        public static BTKUILib.UIObjects.Category pageEditSlotName_Text;
        
        private static FieldInfo _uiInstance = typeof(QMUIElement).Assembly.GetType("BTKUILib.UserInterface").GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static);
        private static MethodInfo _registerRootPage = typeof(QMUIElement).Assembly.GetType("BTKUILib.UserInterface").GetMethod("RegisterRootPage", BindingFlags.NonPublic | BindingFlags.Instance);
        public static void HackRegisterRoot(Page element)
        {
            _registerRootPage.Invoke(_uiInstance.GetValue(null), new object[] { element });
        }

        public static void SaveChanges()
        {
            MetaPort.Instance.SaveGameConfig();
            ViewManager.Instance.gameMenuView.View.TriggerEvent("CVRAppActionLoadSettings");
        }


        public static void SetupUI()
        {
            loadAssets();
            pageEditSettings = new Page("IKpresets", "IK Presets - Editing", false);
            HackRegisterRoot(pageEditSettings);
            pageSaveLoad = new Page("IKpresets", "IK Presets - Save/Load", false);
            HackRegisterRoot(pageSaveLoad);
            pageSetName = new Page("IKpresets", "IK Presets - Set Name", false);
            HackRegisterRoot(pageSetName);
            pageEditSlotName = new Page("IKpresets", "IK Presets - Edit slot names", false);
            HackRegisterRoot(pageEditSlotName);
            pageAvatarSaveLoad = new Page("IKpresets", "IK Presets - Avatar Slots", false);
            HackRegisterRoot(pageAvatarSaveLoad);
            Category cat = null;
            if (Main.useNirvMiscPage.Value)
            {
                var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                page.MenuTitle = "Nirv Misc Page";
                page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                cat = page.AddCategory("IK Presets", "IKpresets");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("IK Presets", "IKpresets");
            }
            cat.AddButton("Edit current values", "ConfigIK", "Edit current IK settings. Same options as in the big menu").OnPress += () =>
            {
                EditSettings();
            };
            cat.AddButton("Per Avatar Presets", "IKSaveLoad", "Presets for specific avatars").OnPress += () =>
            {
                AvatarSaveLoad();
            };
            cat.AddButton("Save/Load Slots", "IKSaveLoadSlots", "Save and load preset slots").OnPress += () =>
            {
                SaveLoad();
            };
            cat.AddButton("ReCalibrate", "Recal", "ReCalibrate Avatar").OnPress += () =>
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
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 5 : 1), 0, 25)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 10", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 10); UpdateSet(); SaveChanges();
                        //MetaPort.Instance.settings.SetSetting(setting, 10.ToString(), true);
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 30", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 30); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 30", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 30); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 30", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 30); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 15", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 15); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 2 : 1), 1, 10)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 2", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 2); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 10 : 1), -60, 60)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 15", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 15); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 2 : 1), 1, 10)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 2", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, 2); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsFloat(setting, Utils.Clamp(value - (fast ? 5f : 1f), 0f, 50f)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Reset to 5", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsFloat(setting, 5); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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

                    //https://stackoverflow.com/a/22590456
                        int totalInches = (int)(value / 2.54); // This will take a floor function of Centimetres/2.54
                        int Feet = (totalInches - totalInches % 12) / 12; // This will make it divisible by 12
                        int inches = totalInches % 12; // This will give you the remainder after you divide by 12
                    

                    var cat = page.AddCategory(catText());
                    string catText() { return $"Player Height: {value}cm - {Feet}\' {inches}\""; }
                    void UpdateSet()
                    {
                        value = MetaPort.Instance.settings.GetSettingInt(setting);
                        cat.CategoryName = catText();
                    }

                    var fast = false;
                    cat.AddButton("Decrease", "White-Minus", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, Utils.Clamp(value - (fast ? 5 : 1), 1, 305)); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton($"Reset to {value}", "Reset", desc).OnPress += () =>
                    {
                        MetaPort.Instance.settings.SetSettingsInt(setting, value); UpdateSet(); SaveChanges();
                    };
                    cat.AddButton("Increase", "White-Plus", desc).OnPress += () =>
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

        private static void SaveLoad()
        {//type T|F - Pos|Rot
            var page = pageSaveLoad;
            page.ClearChildren();
            page.MenuTitle = "Save/Load IK Presets";
            page.MenuSubtitle = Text();

            var catMain = page.AddCategory("");
            catMain.AddButton("Edit Slot Names", "blank", "Edit Slot Names").OnPress += () =>
            {
                EditSlotNames();
            };
            catMain.AddButton("Refresh", "Reset", "Refresh Names").OnPress += () =>
            {
                SaveLoad();
            };
            try
            {
                var slotNames = SaveSlots.GetSavedSlotNames();
                foreach (System.Collections.Generic.KeyValuePair<int, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int)>
                    slot in SaveSlots.GetSaved())
                {
                    string label = $"Slot: {slot.Key} - {slotNames[slot.Key]}";
                    var cat = page.AddCategory(label);

                    var desc = $"PitchYaw:{Utils.CompactTF(slot.Value.Item1)}_HipPin:{Utils.CompactTF(slot.Value.Item3)}_" +
                            $"StrNeck:{Utils.CompactTF(slot.Value.Item4)}_HipShift:{Utils.CompactTF(slot.Value.Item5)}_StrSpine:{Utils.CompactTF(slot.Value.Item6)}_" +
                            $"RelxIter:{slot.Value.Item7} SpineFwd:{slot.Value.Item8}Bck:{slot.Value.Item9}_" +
                            $"NeckFwd:{slot.Value.Item10}Bck:{slot.Value.Item11}_NeckPri:{slot.Value.Item12}_" +
                            $"StrSpine:{slot.Value.Item13}Pow:{slot.Value.Item14}Height:{slot.Value.Item17}";
                    cat.AddButton("Load", "Load", desc).OnPress += () =>
                    {
                        if (slotNames[slot.Key] == "N/A")
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
                    cat.AddButton("Save", "Save", "Save current IK settings to this slot").OnPress += () =>
                    {
                        SaveSlots.Store(slot.Key);
                        SaveLoad();
                    };
                }
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading slots\n" + ex.ToString());
                page.AddCategory("Error loading slots");
            }
            void UpdateText()
            {
                page.MenuSubtitle = Text();
            }
            string Text()
            {
                return $"PitchYaw:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKPitchYawShoulders"))}_HipPin:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKHipPinned"))}_" +
                    $"StrNeck:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKStraightenNeck"))}_HipShift:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKHipShifting"))}_StrSpine:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKPreStraightenSpine"))}_" +
                    $"RelxIter:{MetaPort.Instance.settings.GetSettingInt("IKSpineRelaxIterations")} SpineFwd:{MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleFwd")}Bck:{MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleBack")}_" +
                    $"NeckFwd:{MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleFwd")}Bck:{MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleBack")}_NeckPri:{MetaPort.Instance.settings.GetSettingInt("IKNeckPriority")}_" +
                    $"StrSpine:{MetaPort.Instance.settings.GetSettingInt("IKStraightSpineAngle")}Pow:{MetaPort.Instance.settings.GetSettingInt("IKStraightSpinePower")}Height{MetaPort.Instance.settings.GetSettingInt("GeneralPlayerHeight")}";// +
                    //$"Smooth:{MetaPort.Instance.settings.GetSettingsFloat("IKTrackingSmoothing")}";
            }
            page.OpenPage(); 
        }


        private static void EditSlotNames()
        {
            var page = pageEditSlotName;
            page.ClearChildren();
            page.MenuTitle = "Edit Slot Names";
            page.MenuSubtitle = "Used for changing the names of the slots";

            pageEditSlotName_Text = page.AddCategory($"Current string: {Main.tempString}");
            pageEditSlotName_Text.AddButton("Edit String", "blank", "Edit String").OnPress += () =>
            {
                SetName();
            };

            try
            {
                var slotNames = SaveSlots.GetSavedSlotNames();
                foreach (System.Collections.Generic.KeyValuePair<int, string> slot in slotNames)
                {
                    string label = $"Slot: {slot.Key}\n{slot.Value}";
                    var cat = page.AddCategory(label);

                    cat.AddButton("Load String", "blank", "Load String from slot into current string").OnPress += () =>
                    {
                        Main.tempString = slot.Value;
                        EditSlotNames();
                    };
                    cat.AddButton("Set String", "blank", "Set String into this slot name").OnPress += () =>
                    {
                        SaveSlots.StoreSlotNames(slot.Key, Main.tempString);
                        EditSlotNames();
                    };
                }
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading names\n" + ex.ToString());
                page.AddCategory("Error loading names");
            }
            page.OpenPage();
        }

        public static void SetName()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz-_ ".ToCharArray();
            bool cas = true;

            var page = pageSetName;
            page.ClearChildren();

            page.MenuTitle = "Editing the current string";
            void SetCustomSub()
            {
                page.MenuSubtitle = $"Current name: {Main.tempString}";
            }
            SetCustomSub();

            var cat1 = page.AddCategory("");
            var cat2 = page.AddCategory("");

            cat1.AddButton("BackSpace", "blank", "").OnPress += () =>
            {
                if (Main.tempString.Length > 0) Main.tempString = Main.tempString.Remove(Main.tempString.Length - 1, 1); SetCustomSub();
            };
            cat1.AddButton("Clear", "blank", "").OnPress += () =>
            {
                Main.tempString = ""; SetCustomSub();
            };
            cat1.AddToggle("Upper Case", "Toggle Upper/Lower Case", cas).OnValueUpdated += action =>
            {
                cas = action;
            };

            foreach (char c in chars)
            {
                var s = c.ToString();
                cat2.AddButton(s.ToUpper(), "blank", $"{s.ToUpper()}/{s.ToLower()}").OnPress += () =>
                {
                    Main.tempString += cas ? s.ToUpper() : s.ToLower(); SetCustomSub();
                    pageEditSlotName_Text.CategoryName = $"Current string: {Main.tempString}";
                };
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
            catMain.AddButton("Save this avatar config", "Save", "Save current IK settings").OnPress += () =>
            {
                //Main.Logger.Msg($"{Main.avatarGUID} - {MetaPort.Instance.currentAvatarGuid} - {Main.avatarName}");
                SaveSlots.AvatarStore(MetaPort.Instance.currentAvatarGuid, Main.avatarName);
                AvatarSaveLoad();
            };
        
            catMain.AddToggle("Auto load", "Auto load matching IK settings on avatar change", Main.autoLoadAvatarPresets.Value).OnValueUpdated += action =>
            {
                Main.autoLoadAvatarPresets.Value = action;
            };
            //catMain.AddButton("Refresh", "Reset", "Refresh").OnPress += () =>
            //{
            //    AvatarSaveLoad();
            //};


            try
            {
                var currentID = MetaPort.Instance.currentAvatarGuid;
                if (SaveSlots.AvatarGetSaved().ContainsKey(currentID))
                {
                    SlotLine(true, new System.Collections.Generic.KeyValuePair<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int, string)>(
                        currentID, SaveSlots.AvatarGetSaved()[currentID]));
                }

                foreach (System.Collections.Generic.KeyValuePair<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int, string)>
                    slot in SaveSlots.AvatarGetSaved().Reverse())
                {
                    if (slot.Key == currentID)
                        continue; //Don't repeat the current one if it matches
                    SlotLine(false, slot);
                }
                void SlotLine(bool current, System.Collections.Generic.KeyValuePair<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int, string)>
                    slot)
                {
                    string label = $"{(current ? "*Current avatar* " : "")}{slot.Value.Item17} - {slot.Key}";
                    var cat = page.AddCategory(label);

                    var desc = $"PitchYaw:{Utils.CompactTF(slot.Value.Item1)}_HipPin:{Utils.CompactTF(slot.Value.Item3)}_" +
                            $"StrNeck:{Utils.CompactTF(slot.Value.Item4)}_HipShift:{Utils.CompactTF(slot.Value.Item5)}_StrSpine:{Utils.CompactTF(slot.Value.Item6)}_" +
                            $"RelxIter:{slot.Value.Item7} SpineFwd:{slot.Value.Item8}Bck:{slot.Value.Item9}_" +
                            $"NeckFwd:{slot.Value.Item10}Bck:{slot.Value.Item11}_NeckPri:{slot.Value.Item12}_" +
                            $"StrSpine:{slot.Value.Item13}Pow:{slot.Value.Item14}Height{slot.Value.Item17}";
                    cat.AddButton("Load", "Load", desc).OnPress += () =>
                    {
                        SaveSlots.AvatarLoadSlot(slot.Key);
                        SaveChanges();
                        UpdateText();
                    };
                    cat.AddButton("Delete", "Delete", "Remove this slot").OnPress += () =>
                    {
                        SaveSlots.AvatarSlotDelete(slot.Key);
                        AvatarSaveLoad();
                    };
                }
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading avatar slots\n" + ex.ToString());
                page.AddCategory("Error loading avatar slots");
            }
            void UpdateText()
            {
                page.MenuSubtitle = Text();
            }
            string Text()
            {
                return $"PitchYaw:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKPitchYawShoulders"))}_HipPin:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKHipPinned"))}_" +
                    $"StrNeck:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKStraightenNeck"))}_HipShift:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKHipShifting"))}_StrSpine:{Utils.CompactTF(MetaPort.Instance.settings.GetSettingsBool("IKPreStraightenSpine"))}_" +
                    $"RelxIter:{MetaPort.Instance.settings.GetSettingInt("IKSpineRelaxIterations")} SpineFwd:{MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleFwd")}Bck:{MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleBack")}_" +
                    $"NeckFwd:{MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleFwd")}Bck:{MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleBack")}_NeckPri:{MetaPort.Instance.settings.GetSettingInt("IKNeckPriority")}_" +
                    $"StrSpine:{MetaPort.Instance.settings.GetSettingInt("IKStraightSpineAngle")}Pow:{MetaPort.Instance.settings.GetSettingInt("IKStraightSpinePower")}Height{MetaPort.Instance.settings.GetSettingInt("GeneralPlayerHeight")}";// +
                                                                                                                                                                         //$"Smooth:{MetaPort.Instance.settings.GetSettingsFloat("IKTrackingSmoothing")}";
            }
            page.OpenPage();
        }
    }
}

