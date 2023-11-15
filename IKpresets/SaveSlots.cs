using MelonLoader;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ABI_RC.Core.Savior;


namespace IKpresetsMod
{
    class SaveSlots
    {

        //IKPitchYawShoulders IKPlantFeet IKHipPinned IKStraightenNeck IKHipShifting IKPreStraightenSpine IKSpineRelaxIterations IKMaxSpineAngleFwd IKMaxSpineAngleBack IKMaxNeckAngleFwd IKMaxNeckAngleBack IKNeckPriority IKStraightSpineAngle IKStraightSpinePower IKTrackingSmoothing GeneralEnableRunningAnimationFullBody GeneralPlayerHeight
        //      bool             bool         bool         bool             bool             bool                  int                  int               int                  int               int               int              int                int                float                   bool                                int
        //       1                 2          3            4                  5              6                      7                    8                  9                   10                11                12               13                 14                  15                     16                                  17
        //       true             false        true        false             true          false                10                      30                  30                   30               15               2                 15                  2                  5                       false                              0
        public static Dictionary<int, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int)>GetSaved()
        {
            MelonPreferences_Entry<string> melonPref = Main.savedPrefs;
            try
            {
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                return new Dictionary<int, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int)>(melonPref.Value.Split(';')
                    .Select(s => s.Split(',')).ToDictionary(p => int.Parse(p[0]), p =>
                    (Boolean.Parse(p[1]), Boolean.Parse(p[2]), Boolean.Parse(p[3]), Boolean.Parse(p[4]), Boolean.Parse(p[5]), Boolean.Parse(p[6]),
                    int.Parse(p[7]), int.Parse(p[8]), int.Parse(p[9]), int.Parse(p[10]), int.Parse(p[11]), int.Parse(p[12]), int.Parse(p[13]), int.Parse(p[14]), float.Parse(p[15]),
                     Boolean.Parse(p[16]), int.Parse(p[17]))));
            }
            catch (System.Exception ex) { 
                Main.Logger.Error($"Error loading prefs - Resetting to Defaults:\n" + ex.ToString());
                Main.Logger.Msg($"Current saved value before reset: {melonPref.Value}");
                melonPref.Value = "1,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;2,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;3,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;4,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;5,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;6,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;7,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;8,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;9,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;10,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;11,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;12,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;13,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;14,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;15,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;16,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0";
            }

            return new Dictionary<int, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int)>()
            {{ 1, (true, false, true, false, true, false, 10, 30, 30, 30, 15, 2, 15, 2, 5, false, 0) } };
        }

        public static void Store(int location)
        {
            MelonPreferences_Entry<string> melonPref = Main.savedPrefs;
            try
            {
                var updated = (MetaPort.Instance.settings.GetSettingsBool("IKPitchYawShoulders"), MetaPort.Instance.settings.GetSettingsBool("IKPlantFeet"), MetaPort.Instance.settings.GetSettingsBool("IKHipPinned"),
                    MetaPort.Instance.settings.GetSettingsBool("IKStraightenNeck"), MetaPort.Instance.settings.GetSettingsBool("IKHipShifting"), MetaPort.Instance.settings.GetSettingsBool("IKPreStraightenSpine"),
                    MetaPort.Instance.settings.GetSettingInt("IKSpineRelaxIterations"), MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleFwd"), MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleBack"),
                    MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleFwd"), MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleBack"), MetaPort.Instance.settings.GetSettingInt("IKNeckPriority"),
                    MetaPort.Instance.settings.GetSettingInt("IKStraightSpineAngle"), MetaPort.Instance.settings.GetSettingInt("IKStraightSpinePower"),
                    MetaPort.Instance.settings.GetSettingsFloat("IKTrackingSmoothing"), MetaPort.Instance.settings.GetSettingsBool("GeneralEnableRunningAnimationFullBody"), MetaPort.Instance.settings.GetSettingInt("GeneralPlayerHeight"));
                var Dict = GetSaved();
                Dict[location] = updated;
                melonPref.Value = string.Join(";", Dict.Select(s => String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}", s.Key,
                    s.Value.Item1, s.Value.Item2, s.Value.Item3, s.Value.Item4, s.Value.Item5, s.Value.Item6,
                    s.Value.Item7, s.Value.Item8, s.Value.Item9, s.Value.Item10, s.Value.Item11, s.Value.Item12, s.Value.Item13, s.Value.Item14, 
                    s.Value.Item15.ToString("F5").TrimEnd('0'), s.Value.Item16, s.Value.Item17
                )));
                Main.cat.SaveToFile();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error storing new saved pref\n" + ex.ToString()); }
        }
        
        public static void LoadSlot(int location)
        {
            try
            {
                var Dict = GetSaved();
                MetaPort.Instance.settings.SetSettingsBool("IKPitchYawShoulders", Dict[location].Item1);
                MetaPort.Instance.settings.SetSettingsBool("IKPlantFeet", Dict[location].Item2);
                MetaPort.Instance.settings.SetSettingsBool("IKHipPinned", Dict[location].Item3);
                MetaPort.Instance.settings.SetSettingsBool("IKStraightenNeck", Dict[location].Item4);
                MetaPort.Instance.settings.SetSettingsBool("IKHipShifting", Dict[location].Item5);
                MetaPort.Instance.settings.SetSettingsBool("IKPreStraightenSpine", Dict[location].Item6);
                MetaPort.Instance.settings.SetSettingsInt("IKSpineRelaxIterations", Dict[location].Item7);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxSpineAngleFwd", Dict[location].Item8);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxSpineAngleBack", Dict[location].Item9);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxNeckAngleFwd", Dict[location].Item10);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxNeckAngleBack", Dict[location].Item11);
                MetaPort.Instance.settings.SetSettingsInt("IKNeckPriority", Dict[location].Item12);
                MetaPort.Instance.settings.SetSettingsInt("IKStraightSpineAngle", Dict[location].Item13);
                MetaPort.Instance.settings.SetSettingsInt("IKStraightSpinePower", Dict[location].Item14);
                MetaPort.Instance.settings.SetSettingsFloat("IKTrackingSmoothing", Dict[location].Item15);
                MetaPort.Instance.settings.SetSettingsBool("GeneralEnableRunningAnimationFullBody", Dict[location].Item16);
                if (Dict[location].Item17 != 0) MetaPort.Instance.settings.SetSettingsInt("GeneralPlayerHeight", Dict[location].Item17);

            }
            catch (System.Exception ex) { Main.Logger.Error($"Error loading prefs from slot {location}\n" + ex.ToString()); }
        }

        //Slot names
        //"1,Slot 1;2,Slot 2;3,Slot 3;4,Slot 4;5,Slot 5;6,Slot 6"
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

        public static void StoreSlotNames(int location, string updated)
        {
            MelonPreferences_Entry<string> melonPref = Main.savedPrefNames;
            try
            {
                var Dict = GetSavedSlotNames();
                Dict[location] = updated;
                melonPref.Value = string.Join(";", Dict.Select(s => String.Format("{0},{1}", s.Key, s.Value)));
                Main.cat.SaveToFile();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error storing new saved slot names - \n" + ex.ToString()); }
        }

        public static void MigrateData()
        {
            { //Slots
            MelonPreferences_Entry<string> melonPref = Main.savedPrefs;

                switch (melonPref.Value.Split(';')[0].Split(',').Length)
                {
                    case 17:
                        Main.Logger.Msg($"MigrateData - 17 Elements Found - Migrating Data"); Main.Logger.Msg($"Current saved value before migration: {melonPref.Value}");
                        string newData = "";
                        int set = 1;
                        int end = melonPref.Value.Split(';').Length;
                        foreach (var s in melonPref.Value.Split(';'))
                        {
                            int element = 0;
                            foreach (var e in s.Split(','))
                            {
                                switch (element)
                                {
                                    case 17:
                                        if (set == end)
                                        { newData += e + ",0"; break; }
                                        else
                                        { newData += e + ",0;"; break; }
                                    default: newData += e + ","; break;
                                }
                                element++;
                            }
                            set++;
                        }
                        melonPref.Value = newData;
                        break; 
                    case 18:
                        Main.Logger.Msg($"MigrateData - 18 Elements Found - Current version");
                        break;

                    default:
                        Main.Logger.Msg($"MigrateData Default case, data is corrupt - Resetting to default"); Main.Logger.Msg($"Current saved value before reset: {melonPref.Value}");
                        melonPref.Value = "1,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;2,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;3,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;4,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;5,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;6,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;7,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;8,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;9,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;10,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;11,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;12,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;13,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;14,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;15,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0;16,True,False,True,False,True,False,10,30,30,30,15,2,15,2,5.,false,0";
                        break;
                }  
            }
            {//Per Avatar
                MelonPreferences_Entry<string> melonPref = Main.savedAvatarPrefs;

                switch (melonPref.Value.Split(';')[0].Split(',').Length)
                {
                    case 18:
                        Main.Logger.Msg($"MigrateData AvatarSlots - 18 Elements Found - Migrating Data"); Main.Logger.Msg($"Current saved value before migration: {melonPref.Value}");
                        string newData = "";
                        int set = 1;
                        int end = melonPref.Value.Split(';').Length;
                        foreach (var s in melonPref.Value.Split(';'))
                        {
                            int element = 0;
                            foreach (var e in s.Split(','))
                            {
                                switch (element)
                                {
                                    case 18: //Add new data before AvatarName
                                        if (set == end)
                                        { newData += "0," + e; break; }
                                        else
                                        { newData += "0," + e + ";"; break; }
                                    default: newData += e + ","; break;
                                }
                                element++;
                            }
                            set++;
                        }
                        melonPref.Value = newData;
                        break;
                    case 19:
                        Main.Logger.Msg($"MigrateData AvatarSlots - 19 Elements Found - Current version");
                        break;

                    default:
                        Main.Logger.Msg($"MigrateData AvatarSlots Default case, data is corrupt!!!"); Main.Logger.Msg($"Current saved value: {melonPref.Value}");
                        break;
                }

            }
        }

        /// Yea, I could optimize this down and collapse GetSaved/AvatarGetSaved into one, ect...

        //IKPitchYawShoulders IKPlantFeet IKHipPinned IKStraightenNeck IKHipShifting IKPreStraightenSpine IKSpineRelaxIterations IKMaxSpineAngleFwd IKMaxSpineAngleBack IKMaxNeckAngleFwd IKMaxNeckAngleBack IKNeckPriority IKStraightSpineAngle IKStraightSpinePower IKTrackingSmoothing GeneralEnableRunningAnimationFullBody  GeneralPlayerHeight AvatarName
        //      bool             bool         bool         bool             bool             bool                  int                  int               int                  int               int               int              int                int                float                   bool                                   int           string   
        //       1                 2          3            4                  5              6                      7                    8                  9                   10                11                12               13                 14                  15                     16                                     17             18(was 17)      
        //       true             false        true        false             true          false                10                      30                  30                   30               15               2                 15                  2                  5                       false                                 0             ""      

        public static Dictionary<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int, string)> AvatarGetSaved()
        {
            MelonPreferences_Entry<string> melonPref = Main.savedAvatarPrefs;
            try
            {
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                if ((!melonPref.Value.Contains(","))) return new Dictionary<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int, string)>();
                //If no commas, then no entries, so return emtpy dict
                return new Dictionary<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int, string)>(melonPref.Value.Split(';')
                    .Select(s => s.Split(',')).ToDictionary(p => p[0], p =>
                    (Boolean.Parse(p[1]), Boolean.Parse(p[2]), Boolean.Parse(p[3]), Boolean.Parse(p[4]), Boolean.Parse(p[5]), Boolean.Parse(p[6]),
                    int.Parse(p[7]), int.Parse(p[8]), int.Parse(p[9]), int.Parse(p[10]), int.Parse(p[11]), int.Parse(p[12]), int.Parse(p[13]), int.Parse(p[14]), float.Parse(p[15]),
                     Boolean.Parse(p[16]), int.Parse(p[17]), p[19] )));
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading prefs - Resetting to Defaults:\n" + ex.ToString());
                Main.Logger.Msg($"Current saved value before reset: {melonPref.Value}");
                melonPref.Value = "";
            }

            return new Dictionary<string, (bool, bool, bool, bool, bool, bool, int, int, int, int, int, int, int, int, float, bool, int, string)>()
            {{ "Error see log file", (true, false, true, false, true, false, 10, 30, 30, 30, 15, 2, 15, 2, 5, false, 0, "Error see log file") } };
        }

        public static void AvatarStore(string guid, string avatarname)
        {
            MelonPreferences_Entry<string> melonPref = Main.savedAvatarPrefs;
            try
            {
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                var updated = (MetaPort.Instance.settings.GetSettingsBool("IKPitchYawShoulders"), MetaPort.Instance.settings.GetSettingsBool("IKPlantFeet"), MetaPort.Instance.settings.GetSettingsBool("IKHipPinned"),
                    MetaPort.Instance.settings.GetSettingsBool("IKStraightenNeck"), MetaPort.Instance.settings.GetSettingsBool("IKHipShifting"), MetaPort.Instance.settings.GetSettingsBool("IKPreStraightenSpine"),
                    MetaPort.Instance.settings.GetSettingInt("IKSpineRelaxIterations"), MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleFwd"), MetaPort.Instance.settings.GetSettingInt("IKMaxSpineAngleBack"),
                    MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleFwd"), MetaPort.Instance.settings.GetSettingInt("IKMaxNeckAngleBack"), MetaPort.Instance.settings.GetSettingInt("IKNeckPriority"),
                    MetaPort.Instance.settings.GetSettingInt("IKStraightSpineAngle"), MetaPort.Instance.settings.GetSettingInt("IKStraightSpinePower"),
                    MetaPort.Instance.settings.GetSettingsFloat("IKTrackingSmoothing"), MetaPort.Instance.settings.GetSettingsBool("GeneralEnableRunningAnimationFullBody"), MetaPort.Instance.settings.GetSettingInt("GeneralPlayerHeight"),
                avatarname);
                var Dict = AvatarGetSaved();
                Dict[guid] = updated;
                melonPref.Value = string.Join(";", Dict.Select(s => String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18}", s.Key,
                    s.Value.Item1, s.Value.Item2, s.Value.Item3, s.Value.Item4, s.Value.Item5, s.Value.Item6,
                    s.Value.Item7, s.Value.Item8, s.Value.Item9, s.Value.Item10, s.Value.Item11, s.Value.Item12, s.Value.Item13, s.Value.Item14,
                    s.Value.Item15.ToString("F5").TrimEnd('0'), s.Value.Item16, s.Value.Item17, s.Value.Item18
                )));
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                Main.cat.SaveToFile();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error storing new saved pref\n" + ex.ToString()); }
        }

        public static void AvatarLoadSlot(string location)
        {
            try
            {
                var Dict = AvatarGetSaved();
                MetaPort.Instance.settings.SetSettingsBool("IKPitchYawShoulders", Dict[location].Item1);
                MetaPort.Instance.settings.SetSettingsBool("IKPlantFeet", Dict[location].Item2);
                MetaPort.Instance.settings.SetSettingsBool("IKHipPinned", Dict[location].Item3);
                MetaPort.Instance.settings.SetSettingsBool("IKStraightenNeck", Dict[location].Item4);
                MetaPort.Instance.settings.SetSettingsBool("IKHipShifting", Dict[location].Item5);
                MetaPort.Instance.settings.SetSettingsBool("IKPreStraightenSpine", Dict[location].Item6);
                MetaPort.Instance.settings.SetSettingsInt("IKSpineRelaxIterations", Dict[location].Item7);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxSpineAngleFwd", Dict[location].Item8);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxSpineAngleBack", Dict[location].Item9);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxNeckAngleFwd", Dict[location].Item10);
                MetaPort.Instance.settings.SetSettingsInt("IKMaxNeckAngleBack", Dict[location].Item11);
                MetaPort.Instance.settings.SetSettingsInt("IKNeckPriority", Dict[location].Item12);
                MetaPort.Instance.settings.SetSettingsInt("IKStraightSpineAngle", Dict[location].Item13);
                MetaPort.Instance.settings.SetSettingsInt("IKStraightSpinePower", Dict[location].Item14);
                MetaPort.Instance.settings.SetSettingsFloat("IKTrackingSmoothing", Dict[location].Item15);
                MetaPort.Instance.settings.SetSettingsBool("GeneralEnableRunningAnimationFullBody", Dict[location].Item16);
                if (Dict[location].Item17 != 0) MetaPort.Instance.settings.SetSettingsInt("GeneralPlayerHeight", Dict[location].Item17);
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error loading prefs from slot {location}\n" + ex.ToString()); }
        }

        public static void AvatarSlotDelete(string guid)
        {
            MelonPreferences_Entry<string> melonPref = Main.savedAvatarPrefs;
            try
            {
                
                var Dict = AvatarGetSaved();
                Dict.Remove(guid);
                melonPref.Value = string.Join(";", Dict.Select(s => String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}.{18}", s.Key,
                    s.Value.Item1, s.Value.Item2, s.Value.Item3, s.Value.Item4, s.Value.Item5, s.Value.Item6,
                    s.Value.Item7, s.Value.Item8, s.Value.Item9, s.Value.Item10, s.Value.Item11, s.Value.Item12, s.Value.Item13, s.Value.Item14,
                    s.Value.Item15.ToString("F5").TrimEnd('0'), s.Value.Item16, s.Value.Item17, s.Value.Item18
                )));
                Main.cat.SaveToFile();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error storing new saved pref\n" + ex.ToString()); }
        }

        


















    }
}
