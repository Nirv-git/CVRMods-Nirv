//This is free and unencumbered software released into the public domain.
//
//Anyone is free to copy, modify, publish, use, compile, sell, or
//distribute this software, either in source code form or as a compiled
//binary, for any purpose, commercial or non-commercial, and by any
//means.
//
//In jurisdictions that recognize copyright laws, the author or authors
//of this software dedicate any and all copyright interest in the
//software to the public domain. We make this dedication for the benefit
//of the public at large and to the detriment of our heirs and
//successors. We intend this dedication to be an overt act of
//relinquishment in perpetuity of all present and future rights to this
//software under copyright law.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.
//
//For more information, please refer to <https://unlicense.org>

using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using ABI_RC.Core.Player;
using HarmonyLib;
using ABI_RC.Systems.Audio;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(VoiceBalanceModeRework.Main), "VoiceBalanceModeRework", VoiceBalanceModeRework.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(VoiceBalanceModeRework.Main.versionStr)]
[assembly: AssemblyFileVersion(VoiceBalanceModeRework.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.DarkCyan)]

namespace VoiceBalanceModeRework
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.1";

        public static MelonPreferences_Category cat;
        private const string catagory = "VoiceBalanceModeRework";
        public static MelonPreferences_Entry<bool> modEnabled;
        public static MelonPreferences_Entry<bool> nearbyFocus;
        public static MelonPreferences_Entry<bool> leftFocus;
        public static MelonPreferences_Entry<bool> rightFocus;
        public static MelonPreferences_Entry<int> earFocusAngle;
        public static MelonPreferences_Entry<int> minVol;
        public static MelonPreferences_Entry<bool> debug;
        public static MelonPreferences_Entry<bool> debugStats;
        public static MelonPreferences_Entry<bool> debugTime;

        public static Stopwatch sw = new Stopwatch();

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("VoiceBalanceModeRework", ConsoleColor.DarkCyan);

            cat = MelonPreferences.CreateCategory(catagory, "VoiceBalanceModeRework");
            modEnabled = MelonPreferences.CreateEntry(catagory, nameof(modEnabled), true, "Mod Enabled");
            nearbyFocus = MelonPreferences.CreateEntry(catagory, nameof(nearbyFocus), true, "nearbyFocus");
            leftFocus = MelonPreferences.CreateEntry(catagory, nameof(leftFocus), false, "leftFocus");
            rightFocus = MelonPreferences.CreateEntry(catagory, nameof(rightFocus), false, "rightFocus");
            earFocusAngle = MelonPreferences.CreateEntry(catagory, nameof(earFocusAngle), 60, "earFocusAngle");
            minVol = MelonPreferences.CreateEntry(catagory, nameof(minVol), 30, "Min Volume");
            debug = MelonPreferences.CreateEntry(catagory, nameof(debug), false, "Debug log spam");
            debugStats = MelonPreferences.CreateEntry(catagory, nameof(debugStats), false, "Debug profiling");
            debugTime = MelonPreferences.CreateEntry(catagory, nameof(debugTime), false, "Debug Procesing Time");
        }
    }

    [HarmonyPatch]
    internal class HarmonyPatches
    {
        private static int count = 0;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CVRFocusAttenuation), nameof(CVRFocusAttenuation.Apply))]
        internal static bool OnApply(ref float __result,
                    CVRFocusAttenuation.eFocusMode mode, bool leftEarFocus, bool rightEarFocus, float multiplierMin, float multiplierMax, float volume,
                    Vector3 speakerPosition, Vector3 speakerForward, Vector3 listenerPosition, Vector3 listenerForward, Vector3 listenerRight)
        { 
            if (!Main.modEnabled.Value)
                return true;

            Main.sw.Start();

            if (Main.debug.Value) Main.Logger.Msg($"mode: {mode} multiplierMin: {multiplierMin} multiplierMax: {multiplierMax} volume: {volume} Left:{leftEarFocus} Right:{rightEarFocus}");

            if (mode == CVRFocusAttenuation.eFocusMode.None)
            {
                __result = volume;
                return false;
            }

            var listenerAngle = Vector3.Angle(listenerForward, speakerPosition - listenerPosition);
            if (Main.debug.Value) Main.Logger.Msg("listenerAngle: " + listenerAngle);
            var speakerAngle = Vector3.Angle(speakerForward, listenerPosition -  speakerPosition);
            if (Main.debug.Value) Main.Logger.Msg("speakerAngle: " + speakerAngle);

            //https://www.dpamicrophones.com/mic-university/facts-about-speech-intelligibility
            //linear fit {45,1},{90,.8},{120,.66},{150,.5}
            //1.21944 - 0.0047352 x
            var speakerVolume = 0f;
            if (speakerAngle <= 45f)
                speakerVolume = 1f;
            else if (speakerAngle >= 150f)
                speakerVolume = .5f;
            else
                speakerVolume = 1.21944f - (0.0047352f * speakerAngle);
            if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.DarkCyan, "speakerVolume: " + speakerVolume);


            var listenerVolume = 0f;
            switch (mode)
            {
                case CVRFocusAttenuation.eFocusMode.Balanced:
                    //quadratic fit {40,1},{90,.75},{180,.5}
                    //0.000015873 x^2 - 0.00706349 x + 1.25714
                    if (listenerAngle <= 40f)
                        listenerVolume = 1f;
                    else
                        listenerVolume = (0.000015873f * Mathf.Pow(listenerAngle, 2)) - (0.00706349f * listenerAngle) + 1.25714f;
                    break;
                case CVRFocusAttenuation.eFocusMode.Forward:
                    //quadratic fit {35,1},{90,.4},{140,.25},{180,.2}
                    //0.0000527502 x^2 - 0.0166711 x + 1.50843
                    if (listenerAngle <= 35f)
                        listenerVolume = 1f;
                    else if (listenerAngle >= 160f)
                        listenerVolume = .19f;
                    else
                        listenerVolume = (0.0000527502f * Mathf.Pow(listenerAngle, 2f)) - (0.0166711f * listenerAngle) + 1.50843f;
                    break;
                case CVRFocusAttenuation.eFocusMode.Backward:
                    //quadratic fit {0,.25},{45,.33},{90,.66},{135,1}
                    //0.0000320988 x^2 + 0.0014 x + 0.238
                    if (listenerAngle >= 135f)
                        listenerVolume = 1f;
                    else
                        listenerVolume = (0.0000320988f * Mathf.Pow(listenerAngle, 2)) + (0.0014f * listenerAngle) + 0.238f;
                    break;
            }
            if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.Cyan, "listenerVolume: " + listenerVolume);
            

            var earAngle = Main.earFocusAngle.Value;
            var speakerEarAngle = Vector3.Angle(listenerRight, listenerPosition - speakerPosition); //Left = 0 | Right = 180 
            var earFocusVol = 0f;
            if (Main.leftFocus.Value)//leftEarFocus) Current broke due to game settings bug
            {
                if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.Green, "speakerEarAngleLeft: " + speakerEarAngle);       
                var speakerEarAngleLeft = Mathf.Clamp(speakerEarAngle, 0f, earAngle); // Ensure the value is within range
                var speakerEarLeftVolume = 1f - speakerEarAngleLeft / earAngle; // Normalize the value to the range 0 to 1 with 0 being 1
                if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.Green, "speakerEarLeftVolume: " + speakerEarLeftVolume);
                earFocusVol += speakerEarLeftVolume;
            }
            if (Main.rightFocus.Value)//rightEarFocus)
            {
                var speakerEarAngleRight = Math.Abs(Mathf.Clamp(speakerEarAngle, (180f - earAngle), 180f) - 180);
                if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.DarkGreen, "speakerEarAngleRight: " + speakerEarAngleRight);
                var speakerEarRightVolume = 1f - speakerEarAngleRight / earAngle;
                if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.DarkGreen, "speakerEarRightVolume: " + speakerEarRightVolume);
                earFocusVol += speakerEarRightVolume;
            }


            // Distanced based audio boost
            var distBoost = 0F;
            if (Main.nearbyFocus.Value)
            {
                var height = PlayerSetup.Instance.GetAvatarHeight();
                var dist = Vector3.Distance(speakerPosition, listenerPosition);
                if (dist < height) 
                {
                    var scale = dist / height; 
                    //input, output
                    //quadratic fit {1,0},{.6,.35},{.2,1}
                    //0.9375 x^2 - 2.375 x + 1.4375
                    if (scale < 0.2f)
                        distBoost = 1f;
                    else
                        distBoost = Mathf.Clamp(((0.9375f * Mathf.Pow(scale, 2)) - (2.375f * scale) + 1.4375f), 0f, 1f);
                    if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.DarkMagenta, $"distBoost: {distBoost:F2}| height: {height:F2} dist{dist:F2} scale{scale:F2}");
                }
            }


            var endVolume = (speakerVolume * listenerVolume);
            if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.Yellow, "endVolume-Raw: " + endVolume);

            var endVolumeAdj = endVolume + .75f * earFocusVol;
            if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.DarkYellow, "endVolume-Ear: " + endVolumeAdj);

            endVolumeAdj += .75f * distBoost;
            if (Main.debug.Value) Main.Logger.Msg(ConsoleColor.Yellow, "endVolume-Dist: " + endVolumeAdj);

            endVolumeAdj = Mathf.Clamp(endVolumeAdj, Math.Max(multiplierMin, Main.minVol.Value/100f), multiplierMax);

            __result = volume * endVolumeAdj;

            if (Main.debugStats.Value) Main.Logger.Msg($"listenerAngle: {listenerAngle:F0} Volume: {__result:F2}");
            //if (Main.debugStats.Value) Main.Logger.Msg($"speakerAngle: {speakerAngle:F0} Volume: {speakerVolume:F2}");

            Main.sw.Stop();
            if(Main.debugTime.Value)
            {
                count++;
                if (count >= 10000)
                {
                    var timeMS = Main.sw.ElapsedMilliseconds;
                    Main.Logger.Msg($"Count: {count} timeMS: {timeMS} Avg: {timeMS/count}");
                    count = 0;
                    Main.sw.Reset();
                }
            }
            
            return false;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(CVRFocusAttenuation), nameof(CVRFocusAttenuation.Apply))]
        internal static void OnApplyPost(ref float __result,
                    CVRFocusAttenuation.eFocusMode mode, bool leftEarFocus, bool rightEarFocus, float multiplierMin, float multiplierMax, float volume,
                    Vector3 speakerPosition, Vector3 speakerForward, Vector3 listenerPosition, Vector3 listenerForward, Vector3 listenerRight)
        {
            if (Main.modEnabled.Value)
                return;

            if (Main.debug.Value) Main.Logger.Msg($"Volume: {__result}");

            var listenerAngle = Vector3.Angle(listenerForward, speakerPosition - listenerPosition);
            if (Main.debugStats.Value) Main.Logger.Msg($"listenerAngle: {listenerAngle:F0} Volume: {__result:F2}");

        }  
    }
}








