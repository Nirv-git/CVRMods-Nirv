using MelonLoader;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ABI_RC.Core.Savior;
using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;


namespace IKpresetsMod
{
    class SaveSlots
    {
        public static void LoadConfigSlots()
        {
            if (!Directory.Exists(Main.config_path)) Directory.CreateDirectory(Main.config_path);
            var filePath = Main.config_path + "/" + Main.config_Slots_file;
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                if (content != "")
                {
                    Main.config_Slots = JsonConvert.DeserializeObject<Main.AvatarConfig>(content);
                    return;
                }
            }
            Main.config_Slots = new Main.AvatarConfig();
            Main.config_Slots.Settings = new Dictionary<string, Main.AvatarConfig.AvatarSettings>();
            for (int i = 1; i <= 16; i++)
            {
                Main.config_Slots.Settings.Add(i.ToString(), new Main.AvatarConfig.AvatarSettings());
            }
            SaveConfigSlots();
        }

        public static void SaveConfigSlots()
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(Main.config_Slots, Formatting.Indented);
                File.WriteAllText(Main.config_path + '/' + Main.config_Slots_file, jsonContent);
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error saving Slots Config\n" + ex.ToString()); }
        }

        public static void StoreSlot(string location)
        {
            try
            {
                var updated = new Main.AvatarConfig.AvatarSettings
                {
                    IKPitchYawShoulders = MetaPort.Instance.settings.GetSettingsBool("IKPitchYawShoulders"),
                    IKPlantFeet = MetaPort.Instance.settings.GetSettingsBool("IKPlantFeet"),
                    IKHipPinned = MetaPort.Instance.settings.GetSettingsBool("IKHipPinned"),
                    IKStraightenNeck = MetaPort.Instance.settings.GetSettingsBool("IKStraightenNeck"),
                    IKHipShifting = MetaPort.Instance.settings.GetSettingsBool("IKHipShifting"),
                    IKPreStraightenSpine = MetaPort.Instance.settings.GetSettingsBool("IKPreStraightenSpine"),
                    IKSpineRelaxIterations = MetaPort.Instance.settings.GetSettingInt("IKSpineRelaxIterations"),
                    IKMaxSpineAngleFwd = MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleFwd"),
                    IKMaxSpineAngleBack = MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleBack"),
                    IKMaxNeckAngleFwd = MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleFwd"),
                    IKMaxNeckAngleBack = MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleBack"),
                    IKNeckPriority = MetaPort.Instance.settings.GetSettingInt("IKNeckPriority"),
                    IKStraightSpineAngle = MetaPort.Instance.settings.GetSettingInt("IKStraightSpineAngle"),
                    IKStraightSpinePower = MetaPort.Instance.settings.GetSettingInt("IKStraightSpinePower"),
                    IKTrackingSmoothing = MetaPort.Instance.settings.GetSettingsFloat("IKTrackingSmoothing"),
                    GeneralEnableRunningAnimationFullBody = MetaPort.Instance.settings.GetSettingsBool("GeneralEnableRunningAnimationFullBody"),
                    GeneralPlayerHeight = MetaPort.Instance.settings.GetSettingInt("GeneralPlayerHeight"),
                    IKCalibrationMode = MetaPort.Instance.settings.GetSettingInt("IKCalibrationMode"),
                };

                Main.config_Slots.Settings[location] = updated;
                SaveConfigSlots();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error storing new saved config for Slot {location}\n" + ex.ToString()); }
        }


        public static void LoadSlot(string location)
        {
            try
            {
                var settings = Main.config_Slots.Settings[location];

                MetaPort.Instance.settings.SetSettingsBool("IKPitchYawShoulders", settings.IKPitchYawShoulders);
                MetaPort.Instance.settings.SetSettingsBool("IKPlantFeet", settings.IKPlantFeet);
                MetaPort.Instance.settings.SetSettingsBool("IKHipPinned", settings.IKHipPinned);
                MetaPort.Instance.settings.SetSettingsBool("IKStraightenNeck", settings.IKStraightenNeck);
                MetaPort.Instance.settings.SetSettingsBool("IKHipShifting", settings.IKHipShifting);
                MetaPort.Instance.settings.SetSettingsBool("IKPreStraightenSpine", settings.IKPreStraightenSpine);
                MetaPort.Instance.settings.SetSettingsInt("IKSpineRelaxIterations", settings.IKSpineRelaxIterations);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxSpineAngleFwd", settings.IKMaxSpineAngleFwd);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxSpineAngleBack", settings.IKMaxSpineAngleBack);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxNeckAngleFwd", settings.IKMaxNeckAngleFwd);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxNeckAngleBack", settings.IKMaxNeckAngleBack);
                MetaPort.Instance.settings.SetSettingsInt("IKNeckPriority", settings.IKNeckPriority);
                MetaPort.Instance.settings.SetSettingsInt("IKStraightSpineAngle", settings.IKStraightSpineAngle);
                MetaPort.Instance.settings.SetSettingsInt("IKStraightSpinePower", settings.IKStraightSpinePower);
                MetaPort.Instance.settings.SetSettingsFloat("IKTrackingSmoothing", settings.IKTrackingSmoothing);
                MetaPort.Instance.settings.SetSettingsBool("GeneralEnableRunningAnimationFullBody", settings.GeneralEnableRunningAnimationFullBody);
                if (settings.GeneralPlayerHeight != -1) MetaPort.Instance.settings.SetSettingsInt("GeneralPlayerHeight", settings.GeneralPlayerHeight);
                if (settings.IKCalibrationMode != -1) MetaPort.Instance.settings.SetSettingsInt("IKCalibrationMode", settings.IKCalibrationMode);
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading prefs from slot {location}\n" + ex.ToString());
            }
        }


        //Per Avatar
        public static void LoadConfigAvatars()
        {
            if (!Directory.Exists(Main.config_path)) Directory.CreateDirectory(Main.config_path);
            var filePath = Main.config_path + '/' + Main.config_Avatars_file;
            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                if (content != "")
                {
                    Main.config_Avatars = JsonConvert.DeserializeObject<Main.AvatarConfig>(content);
                    return;
                }
            }

            Main.config_Avatars = new Main.AvatarConfig();
            Main.config_Avatars.Settings = new Dictionary<string, Main.AvatarConfig.AvatarSettings>();

            SaveConfigAvatars();
        }

        public static void SaveConfigAvatars()
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(Main.config_Avatars, Formatting.Indented);
                File.WriteAllText(Main.config_path + '/' + Main.config_Avatars_file, jsonContent);
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error saving Avatars Config\n" + ex.ToString()); }
        }

        public static void StoreAvatars(string guid, string name)
        {
            try
            {
                var updated = new Main.AvatarConfig.AvatarSettings
                {
                    IKPitchYawShoulders = MetaPort.Instance.settings.GetSettingsBool("IKPitchYawShoulders"),
                    IKPlantFeet = MetaPort.Instance.settings.GetSettingsBool("IKPlantFeet"),
                    IKHipPinned = MetaPort.Instance.settings.GetSettingsBool("IKHipPinned"),
                    IKStraightenNeck = MetaPort.Instance.settings.GetSettingsBool("IKStraightenNeck"),
                    IKHipShifting = MetaPort.Instance.settings.GetSettingsBool("IKHipShifting"),
                    IKPreStraightenSpine = MetaPort.Instance.settings.GetSettingsBool("IKPreStraightenSpine"),
                    IKSpineRelaxIterations = MetaPort.Instance.settings.GetSettingInt("IKSpineRelaxIterations"),
                    IKMaxSpineAngleFwd = MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleFwd"),
                    IKMaxSpineAngleBack = MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleBack"),
                    IKMaxNeckAngleFwd = MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleFwd"),
                    IKMaxNeckAngleBack = MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleBack"),
                    IKNeckPriority = MetaPort.Instance.settings.GetSettingInt("IKNeckPriority"),
                    IKStraightSpineAngle = MetaPort.Instance.settings.GetSettingInt("IKStraightSpineAngle"),
                    IKStraightSpinePower = MetaPort.Instance.settings.GetSettingInt("IKStraightSpinePower"),
                    IKTrackingSmoothing = MetaPort.Instance.settings.GetSettingsFloat("IKTrackingSmoothing"),
                    GeneralEnableRunningAnimationFullBody = MetaPort.Instance.settings.GetSettingsBool("GeneralEnableRunningAnimationFullBody"),
                    GeneralPlayerHeight = MetaPort.Instance.settings.GetSettingInt("GeneralPlayerHeight"),
                    IKCalibrationMode = MetaPort.Instance.settings.GetSettingInt("IKCalibrationMode"),
                    AvatarName = name
                };

                Main.config_Avatars.Settings[Main.avatarGUID] = updated;
                SaveConfigAvatars();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error storing new saved config for Avatar {name} {guid}\n" + ex.ToString()); }
        }

        public static void LoadAvatars(string guid)
        {
            try
            {
                if (!Main.config_Avatars.Settings.ContainsKey(guid))
                    return;

                var settings = Main.config_Avatars.Settings[guid];
                MetaPort.Instance.settings.SetSettingsBool("IKPitchYawShoulders", settings.IKPitchYawShoulders);
                MetaPort.Instance.settings.SetSettingsBool("IKPlantFeet", settings.IKPlantFeet);
                MetaPort.Instance.settings.SetSettingsBool("IKHipPinned", settings.IKHipPinned);
                MetaPort.Instance.settings.SetSettingsBool("IKStraightenNeck", settings.IKStraightenNeck);
                MetaPort.Instance.settings.SetSettingsBool("IKHipShifting", settings.IKHipShifting);
                MetaPort.Instance.settings.SetSettingsBool("IKPreStraightenSpine", settings.IKPreStraightenSpine);
                MetaPort.Instance.settings.SetSettingsInt("IKSpineRelaxIterations", settings.IKSpineRelaxIterations);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxSpineAngleFwd", settings.IKMaxSpineAngleFwd);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxSpineAngleBack", settings.IKMaxSpineAngleBack);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxNeckAngleFwd", settings.IKMaxNeckAngleFwd);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxNeckAngleBack", settings.IKMaxNeckAngleBack);
                MetaPort.Instance.settings.SetSettingsInt("IKNeckPriority", settings.IKNeckPriority);
                MetaPort.Instance.settings.SetSettingsInt("IKStraightSpineAngle", settings.IKStraightSpineAngle);
                MetaPort.Instance.settings.SetSettingsInt("IKStraightSpinePower", settings.IKStraightSpinePower);
                MetaPort.Instance.settings.SetSettingsFloat("IKTrackingSmoothing", settings.IKTrackingSmoothing);
                MetaPort.Instance.settings.SetSettingsBool("GeneralEnableRunningAnimationFullBody", settings.GeneralEnableRunningAnimationFullBody);
                if (settings.GeneralPlayerHeight != -1) MetaPort.Instance.settings.SetSettingsInt("GeneralPlayerHeight", settings.GeneralPlayerHeight);
                if (settings.IKCalibrationMode != -1) MetaPort.Instance.settings.SetSettingsInt("IKCalibrationMode", settings.IKCalibrationMode);
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading prefs from avatar {guid}\n" + ex.ToString());
            }
        }

        public static void AvatarSlotDelete(string guid)
        {
            try
            {
                if (!Main.config_Avatars.Settings.ContainsKey(guid))
                    return;
                Main.config_Avatars.Settings.Remove(guid);
                SaveConfigAvatars();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error removing Avatar {guid}\n" + ex.ToString()); }
        }




        public static void MigrateOldDataSlots()
        {
            // Load saved settings and slot names
            Dictionary<int, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool)> savedSettings = GetSaved();
            Dictionary<int, string> savedSlotNames = GetSavedSlotNames();

            // Clear existing settings in Main.config_Slots
            Main.config_Slots.Settings.Clear();

            // Migrate each slot
            foreach (var kvp in savedSettings)
            {
                int location = kvp.Key;

                // Check if the location is valid in savedSlotNames dictionary
                if (savedSlotNames.TryGetValue(location, out string slotName))
                {
                    // Create a new AvatarSettings instance
                    Main.AvatarConfig.AvatarSettings newSettings = new Main.AvatarConfig.AvatarSettings
                    {
                        IKPitchYawShoulders = kvp.Value.Item1,
                        IKPlantFeet = kvp.Value.Item2,
                        IKHipPinned = kvp.Value.Item3,
                        IKStraightenNeck = kvp.Value.Item4,
                        IKHipShifting = kvp.Value.Item5,
                        IKPreStraightenSpine = kvp.Value.Item6,
                        IKSpineRelaxIterations = kvp.Value.Item7,
                        IKMaxSpineAngleFwd = kvp.Value.Item8,
                        IKMaxSpineAngleBack = kvp.Value.Item9,
                        IKMaxNeckAngleFwd = kvp.Value.Item10,
                        IKMaxNeckAngleBack = kvp.Value.Item11,
                        IKNeckPriority = kvp.Value.Item12,
                        IKStraightSpineAngle = kvp.Value.Item13,
                        IKStraightSpinePower = kvp.Value.Item14,
                        IKTrackingSmoothing = kvp.Value.Item15,
                        GeneralEnableRunningAnimationFullBody = kvp.Value.Item16,
                        SlotName = slotName
                    };

                    // Add the new settings to Main.config_Slots
                    Main.config_Slots.Settings[location.ToString()] = newSettings;
                }
                else
                {
                    Main.Logger.Error($"Slot name not found for location {location}. Skipping migration for this slot.");
                }
            }

            // Save the migrated settings to file
            SaveConfigSlots();
            Main.savedPrefs.Value = "Migrated__" + Main.savedPrefs.Value;
            Main.savedPrefNames.Value = "Migrated__" + Main.savedPrefNames.Value;
        }


        public static void MigrateOldDataAvatars()
        {
            // Load saved avatar settings
            Dictionary<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, string)> savedSettings = AvatarGetSaved();

            // Clear existing settings in Main.config_Avatars
            Main.config_Avatars.Settings.Clear();

            // Migrate each avatar
            foreach (var kvp in savedSettings)
            {
                var location = kvp.Key;

                // Create a new AvatarSettings instance
                Main.AvatarConfig.AvatarSettings newSettings = new Main.AvatarConfig.AvatarSettings
                {
                    IKPitchYawShoulders = kvp.Value.Item1,
                    IKPlantFeet = kvp.Value.Item2,
                    IKHipPinned = kvp.Value.Item3,
                    IKStraightenNeck = kvp.Value.Item4,
                    IKHipShifting = kvp.Value.Item5,
                    IKPreStraightenSpine = kvp.Value.Item6,
                    IKSpineRelaxIterations = kvp.Value.Item7,
                    IKMaxSpineAngleFwd = kvp.Value.Item8,
                    IKMaxSpineAngleBack = kvp.Value.Item9,
                    IKMaxNeckAngleFwd = kvp.Value.Item10,
                    IKMaxNeckAngleBack = kvp.Value.Item11,
                    IKNeckPriority = kvp.Value.Item12,
                    IKStraightSpineAngle = kvp.Value.Item13,
                    IKStraightSpinePower = kvp.Value.Item14,
                    IKTrackingSmoothing = kvp.Value.Item15,
                    GeneralEnableRunningAnimationFullBody = kvp.Value.Item16,
                    AvatarName = kvp.Value.Item17
                };

                // Add the new settings to Main.config_Slots
                Main.config_Avatars.Settings[location.ToString()] = newSettings;
            }

            // Save the migrated settings to file
            SaveConfigAvatars();
            Main.savedAvatarPrefs.Value = "Migrated__" + Main.savedAvatarPrefs.Value;
        }



        //IKPitchYawShoulders IKPlantFeet IKHipPinned IKStraightenNeck IKHipShifting IKPreStraightenSpine IKSpineRelaxIterations IKMaxSpineAngleFwd IKMaxSpineAngleBack IKMaxNeckAngleFwd IKMaxNeckAngleBack IKNeckPriority IKStraightSpineAngle IKStraightSpinePower IKTrackingSmoothing GeneralEnableRunningAnimationFullBody GeneralPlayerHeight
        //      bool             bool         bool         bool             bool             bool                  int                  int               int                  int               int               int              int                int                float                   bool                                int
        //       1                 2          3            4                  5              6                      7                    8                  9                   10                11                12               13                 14                  15                     16                                  17
        //       true             false        true        false             true          false                10                      30                  30                   30               15               2                 15                  2                  5                       false                              0
        public static Dictionary<int, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool)> GetSaved()
        {
            MelonPreferences_Entry<string> melonPref = Main.savedPrefs;
            try
            {
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                return new Dictionary<int, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool)>(melonPref.Value.Split(';')
                    .Select(s => s.Split(',')).ToDictionary(p => int.Parse(p[0]), p =>
                    (Boolean.Parse(p[1]), Boolean.Parse(p[2]), Boolean.Parse(p[3]), Boolean.Parse(p[4]), Boolean.Parse(p[5]), Boolean.Parse(p[6]),
                    int.Parse(p[7]), int.Parse(p[8]), int.Parse(p[9]), int.Parse(p[10]), int.Parse(p[11]), int.Parse(p[12]), int.Parse(p[13]), int.Parse(p[14]), float.Parse(p[15]),
                     Boolean.Parse(p[16]))));
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading prefs - Resetting to Defaults:\n" + ex.ToString());
                Main.Logger.Msg($"Current saved value before reset: {melonPref.Value}");
                melonPref.Value = "1,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;2,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;3,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;4,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;5,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;6,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;7,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;8,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;9,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;10,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;11,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;12,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;13,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;14,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;15,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false;16,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false";
            }

            return new Dictionary<int, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool)>()
            {{ 1, (true, false, true, false, true, false, 10, 30, 30, 30, 15, 2, 15, 2, 5, false) } };
        }


        public static Dictionary<int, string> GetSavedSlotNames()
        {
            MelonPreferences_Entry<string> melonPref = Main.savedPrefNames;
            try
            {
                //Main.Logger.Msg("Value: " + melonPref.Value);
                return new Dictionary<int, string>(melonPref.Value.Split(';').Select(s => s.Split(',')).ToDictionary(p => int.Parse(p[0]), p => p[1]));
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error loading slot names - Resetting to Defaults:\n" + ex.ToString()); melonPref.Value = "1,N/A;2,N/A;3,N/A;4,N/A;5,N/A;6,N/A;7,N/A;8,N/A;9,N/A;10,N/A;11,N/A;12,N/A;13,N/A;14,N/A;15,N/A;16,N/A"; }
            return new Dictionary<int, string>() { { 1, "Error see log file" } };

        }

     
        public static Dictionary<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, string)> AvatarGetSaved()
        {
            MelonPreferences_Entry<string> melonPref = Main.savedAvatarPrefs;
            try
            {
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                if ((!melonPref.Value.Contains(","))) return new Dictionary<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, string)>();
                //If no commas, then no entries, so return emtpy dict
                return new Dictionary<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, string)>(melonPref.Value.Split(';')
                    .Select(s => s.Split(',')).ToDictionary(p => p[0], p =>
                    (Boolean.Parse(p[1]), Boolean.Parse(p[2]), Boolean.Parse(p[3]), Boolean.Parse(p[4]), Boolean.Parse(p[5]), Boolean.Parse(p[6]),
                    int.Parse(p[7]), int.Parse(p[8]), int.Parse(p[9]), int.Parse(p[10]), int.Parse(p[11]), int.Parse(p[12]), int.Parse(p[13]), int.Parse(p[14]), float.Parse(p[15]),
                     Boolean.Parse(p[16]), p[17])));
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading prefs - Resetting to Defaults:\n" + ex.ToString());
                Main.Logger.Msg($"Current saved value before reset: {melonPref.Value}");
                melonPref.Value = "";
            }

            return new Dictionary<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, string)>()
            {{ "Error see log file", (true, false, true, false, true, false, 10, 30, 30, 30, 15, 2, 15, 2, 5, false, "Error see log file") } };
        }
    }
}
