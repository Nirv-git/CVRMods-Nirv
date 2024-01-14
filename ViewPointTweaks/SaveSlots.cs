using MelonLoader;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ABI_RC.Core.Savior;

namespace ViewPointTweaks
{
    class SaveSlots
    {
        //GUID,      float, Vector3(Float, Float, Float), Float  , String
        //String    Scale           Pos X    Y     Z       Rot   , AvatarName
        // 0           1                2    3     4         5     6

        public static Dictionary<string, (float, float, float,float, float, string)> AvatarGetSaved()
        {
            MelonPreferences_Entry<string> melonPref = Main.savedAvatarPrefs;
            try
            {
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                if ((!melonPref.Value.Contains(","))) return new Dictionary<string, (float, float, float, float, float, string)>();
                //If no commas, then no entries, so return emtpy dict
                return new Dictionary<string, (float, float, float, float, float, string)>(melonPref.Value.Split(';')
                    .Select(s => s.Split(',')).ToDictionary(p => p[0], p =>
                    (float.Parse(p[1]), float.Parse(p[2]), float.Parse(p[3]), float.Parse(p[4]), float.Parse(p[5]), p[6])));
            }
            catch (System.Exception ex)
            {
                Main.Logger.Error($"Error loading prefs - Resetting to Defaults:\n" + ex.ToString());
                Main.Logger.Msg($"Current saved value before reset: {melonPref.Value}");
                melonPref.Value = "";
            }

            return new Dictionary<string, (float, float, float, float, float, string)>()
            {{ "Error see log file", (1f, 0f, 0f, 0f, 0f, "Error see log file") } };
        }

        public static void AvatarStore(string guid, string avatarname)
        {
            MelonPreferences_Entry<string> melonPref = Main.savedAvatarPrefs;
            try
            {
                //MelonLoader.MelonLogger.Msg("Value: " + melonPref.Value);
                var updated = (Main.currentScaleAdjustment, Main.currentPosAdjustment.x, Main.currentPosAdjustment.y, Main.currentPosAdjustment.z, Main.currentRotAdjustment, avatarname);
                var Dict = AvatarGetSaved();
                Dict[guid] = updated;
                melonPref.Value = string.Join(";", Dict.Select(s => String.Format("{0},{1},{2},{3},{4},{5},{6}", s.Key,
                    s.Value.Item1.ToString("F5").TrimEnd('0'), s.Value.Item2.ToString("F5").TrimEnd('0'), s.Value.Item3.ToString("F5").TrimEnd('0'), s.Value.Item4.ToString("F5").TrimEnd('0'),
                    s.Value.Item5.ToString("F5").TrimEnd('0'), s.Value.Item6  
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
                if (Main.currentScaleAdjustment != 0 || Main.currentPosAdjustment != Vector3.zero || Main.currentRotAdjustment != 0)
                {
                    //Main.Logger.Msg($"Resetting Offsets before load");
                    Main.ResetOffsets();
                    HarmonyPatches.wasReset = false;
                }
                var Dict = AvatarGetSaved();
                Main.ChangeOffsets(Dict[location].Item1, new Vector3(Dict[location].Item2, Dict[location].Item3, Dict[location].Item4), Dict[location].Item5);
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
                melonPref.Value = string.Join(";", Dict.Select(s => String.Format("{0},{1},{2},{3},{4},{5},{6}", s.Key,
                    s.Value.Item1.ToString("F5").TrimEnd('0'), s.Value.Item2.ToString("F5").TrimEnd('0'), s.Value.Item3.ToString("F5").TrimEnd('0'), s.Value.Item4.ToString("F5").TrimEnd('0'),
                    s.Value.Item5.ToString("F5").TrimEnd('0'), s.Value.Item6
                )));
                Main.cat.SaveToFile();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error storing new saved pref\n" + ex.ToString()); }
        }

    }
}
