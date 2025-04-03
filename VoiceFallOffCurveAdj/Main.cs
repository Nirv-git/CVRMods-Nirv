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
using ABI_RC.Systems.Communications;
using ABI_RC.Systems.Communications.Audio;
using ABI_RC.Systems.Communications.Audio.Components;
using ABI.CCK.Components;
using ABI_RC.Core;
using ABI_RC.Core.Savior;
using UnityEngine.Events;
using UIExpansionKit.API;


[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(VoiceFallOffCurveAdj.Main), "VoiceFallOffCurveAdj", VoiceFallOffCurveAdj.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(VoiceFallOffCurveAdj.Main.versionStr)]
[assembly: AssemblyFileVersion(VoiceFallOffCurveAdj.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.DarkCyan)]

namespace VoiceFallOffCurveAdj
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.8";

        public static MelonPreferences_Category cat;
        private const string catagory = "VoiceFallOffCurveAdj";


        //public static MelonPreferences_Entry<bool> reworkFocus;
        public static MelonPreferences_Entry<bool> changeVolumeCurve;
        public static MelonPreferences_Entry<float> volumeCurveMidPos;
        public static MelonPreferences_Entry<float> volumeCurveMidValue;
        public static MelonPreferences_Entry<float> volumeCurveMid2Pos;
        public static MelonPreferences_Entry<float> volumeCurveMid2Value;
        public static MelonPreferences_Entry<float> volumeCurveMid3Pos;
        public static MelonPreferences_Entry<float> volumeCurveMid3Value;
        public static MelonPreferences_Entry<bool> info;

        //public static MelonPreferences_Entry<bool> debugParticipantPipeline;



        public static Stopwatch sw = new Stopwatch();

        public static AnimationCurve origVolumeCurve;
        public static AnimationCurve newVolumeCurve;

        public static bool _changeVolumeCurve = false;
        public static bool firstRun = true;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("VoiceFallOffCurveAdj", ConsoleColor.DarkCyan);

            cat = MelonPreferences.CreateCategory(catagory, "VoiceFallOffCurveAdj");

            changeVolumeCurve = MelonPreferences.CreateEntry(catagory, nameof(changeVolumeCurve), true, "Change Volume Curve");
            volumeCurveMidPos = MelonPreferences.CreateEntry(catagory, nameof(volumeCurveMidPos), .15f, "Curve Point 1 Position (0.15-0.95");
            volumeCurveMidValue = MelonPreferences.CreateEntry(catagory, nameof(volumeCurveMidValue), .5f, "Curve Point 1 Value (0-1)");
            volumeCurveMid2Pos = MelonPreferences.CreateEntry(catagory, nameof(volumeCurveMid2Pos), .75f, "Curve Point 2 Position (0.15-0.95)");
            volumeCurveMid2Value = MelonPreferences.CreateEntry(catagory, nameof(volumeCurveMid2Value), .05f, "Curve Point 2 Value (0-1)");
            volumeCurveMid3Pos = MelonPreferences.CreateEntry(catagory, nameof(volumeCurveMid3Pos), .95f, "Curve Point 3 Position (0.15-0.95)");
            volumeCurveMid3Value = MelonPreferences.CreateEntry(catagory, nameof(volumeCurveMid3Value), .025f, "Curve Point 3 Value (0-1)");
            info = MelonPreferences.CreateEntry(catagory, nameof(info), false, "Position/Value for later points are clamped to be later/lower then the previous");

            //debugParticipantPipeline = MelonPreferences.CreateEntry(catagory, nameof(debugParticipantPipeline), false, "Debug log spam - ParticipantPipeline";

            changeVolumeCurve.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { UpdateCurve(true); });
            volumeCurveMidPos.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { UpdateCurve(false); CompareCurves(); });
            volumeCurveMidValue.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { UpdateCurve(false); CompareCurves(); });
            volumeCurveMid2Pos.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { UpdateCurve(false); CompareCurves(); });
            volumeCurveMid2Value.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { UpdateCurve(false); CompareCurves(); });
            volumeCurveMid3Pos.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { UpdateCurve(false); CompareCurves(); });
            volumeCurveMid3Value.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { UpdateCurve(false); CompareCurves(); });

            var settings = ExpansionKitApi.GetSettingsCategory("VoiceFallOffCurveAdj");
            settings.AddSimpleButton("Print Current Curve to Console", () => DrawCurveInConsole(GetCurrentCurve(), ConsoleColor.White));
            settings.AddSimpleButton("Dump Curve Values (Logspam)", () => PrintCurveValues(GetCurrentCurve(), 0.01f));
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)//Without switch this would run 3 times at world load
            {
                case 0: break;
                case 1: break;
                case 2: break;
                default:
                    if (firstRun)
                    {
                        firstRun = false;
                        MetaPort.Instance.settings.settingIntChanged.AddListener(new UnityAction<string, int>(OnIntSettingChanged));
                        CreateVolumeCurve(0, 0); //Update Stock Curve
                        GenerateCustomCurve();
                        _changeVolumeCurve = changeVolumeCurve.Value;  
                    }
                    break;
            }
        }

        public static void GenerateCustomCurve()
        {
            float middlePos = Mathf.Min(Mathf.Max(volumeCurveMidPos.Value, 0.15f), .95f); 
            float middleValue = Mathf.Min(Mathf.Max(volumeCurveMidValue.Value, 0f), 1f);
            float middle2Pos = Mathf.Min(Mathf.Max(volumeCurveMid2Pos.Value, middlePos + .05f), .95f);
            float middle2Value = Mathf.Min(Mathf.Max(volumeCurveMid2Value.Value, 0f), middleValue - .05f);
            float middle3Pos = Mathf.Min(Mathf.Max(volumeCurveMid2Pos.Value, middle2Pos + .05f), .95f);
            float middle3Value = Mathf.Min(Mathf.Max(volumeCurveMid2Value.Value, 0f), middle2Value - .05f);
            float maxDist = MetaPort.Instance.settings.GetSettingsInt("AudioPlayerVoiceAttenuationDistanceFalloff", 8) - 1; //STOCK CURVE CUTS OFF LAST 

            Keyframe kf1 = new Keyframe(0, 1, 0, 0);
            Keyframe kf2 = new Keyframe(0.1f, 1, 0, 0);
            Keyframe kf3 = new Keyframe(maxDist * middlePos, middleValue, 0, 0);
            Keyframe kf4 = new Keyframe(maxDist * middle2Pos, middle2Value, 0, 0);
            Keyframe kf5 = new Keyframe(maxDist * middle3Pos, middle3Value, 0, 0);
            Keyframe kf6 = new Keyframe(maxDist, 0, 0, 0);
            newVolumeCurve = new AnimationCurve(kf1, kf2, kf3, kf4, kf5, kf6);

            // Smooth the tangents
            //newVolumeCurve.SmoothTangents(0, 0);  // Index of keyframe, weight of smoothing
            //newVolumeCurve.SmoothTangents(1, 0);
            //newVolumeCurve.SmoothTangents(2, 0);
            //newVolumeCurve.SmoothTangents(3, 0);
            //newVolumeCurve.SmoothTangents(4, 0);
            //newVolumeCurve.SmoothTangents(5, 0);

            // Smooth the tangents
            for (int i = 0; i < newVolumeCurve.length; i++)
            {
                newVolumeCurve.SmoothTangents(i, 0);
            }

            
            //DrawCurveInConsole(newVolumeCurve, ConsoleColor.DarkGray); //Comment out
            AdjustCurveTangents(ref newVolumeCurve, 0.01f, 0, 1);
            //DrawCurveInConsole(newVolumeCurve, ConsoleColor.Gray); //Comment out
            AdjustCurveTangentsToPreventDips(ref newVolumeCurve, 0.01f);
            //DrawCurveInConsole(newVolumeCurve, ConsoleColor.White); //Comment out
        }



        // Method to adjust the curve until all points are within the specified range
        public static void AdjustCurveTangents(ref AnimationCurve curve, float resolution, float min, float max)
        {
            int loopdetect = 0;
            bool isWithinRange;
            do
            {
                isWithinRange = true;
                for (float t = curve.keys[0].time; t <= curve.keys[curve.length - 1].time; t += resolution)
                {
                    loopdetect++;
                    if (loopdetect > 20010) {
                        Logger.Warning($"!!! AdjustCurveTangents got stuck in a loop !!! {loopdetect}");
                        return;
                    }
                    float value = curve.Evaluate(t);
                    if (value < min || value > max)
                    {
                        float derivative = curve.Derivative(t);
                        if (loopdetect > 20000) Logger.Msg(ConsoleColor.Cyan, $"time:{t} derivative:{derivative}");
                        AdjustTangentsAtTime(ref curve, t, derivative);
                        isWithinRange = false;
                        break;
                    }
                }
            } while (!isWithinRange);
        }
        public static void AdjustTangentsAtTime(ref AnimationCurve curve, float time, float derivative)
        {
            
            for (int i = 1; i < curve.keys.Length; i++)
            {
                if (curve.keys[i].time >= time)
                {
                    Keyframe prevKey = curve.keys[i - 1];
                    Keyframe thisKey = curve.keys[i];

                    if (derivative > 0) // Ascending
                    {
                        prevKey.outTangent *= 0.5f;
                        thisKey.inTangent *= 0.5f;
                    }
                    else if (derivative < 0) // Descending
                    {
                        prevKey.outTangent *= 0.5f;
                        thisKey.inTangent *= 0.5f;
                    }

                    curve.MoveKey(i - 1, prevKey);
                    curve.MoveKey(i, thisKey);
                    break;
                }
            }
            curve = new AnimationCurve(curve.keys); // Rebuild the curve to ensure updates
        }

        // Method to adjust tangents to ensure that the curve does not dip below the next keyframe
        public static void AdjustCurveTangentsToPreventDips(ref AnimationCurve curve, float resolution)
        {
            for (int i = 0; i < curve.keys.Length - 1; i++)
            {
                float startTime = curve.keys[i].time;
                float endTime = curve.keys[i + 1].time;
                float nextKeyValue = curve.keys[i + 1].value;

                for (float t = startTime; t <= endTime; t += resolution)
                {
                    float value = curve.Evaluate(t);
                    if (value < nextKeyValue)
                    {
                        // Adjust the tangents to avoid the dip
                        Keyframe prevKey = curve.keys[i];
                        Keyframe nextKey = curve.keys[i + 1];

                        // Reduce tangents to flatten the curve between these points
                        prevKey.outTangent *= 0.5f;
                        nextKey.inTangent *= 0.5f;

                        curve.MoveKey(i, prevKey);
                        curve.MoveKey(i + 1, nextKey);

                        // Rebuild the curve after each adjustment
                        curve = new AnimationCurve(curve.keys);

                        // Restart the check for this segment after adjusting
                        t = startTime;
                    }
                }
            }
        }

        private void OnIntSettingChanged(string name, int value)
        {
            if (!(name == "AudioPlayerVoiceAttenuationDistanceFalloff"))
                return;
            UpdateCurve(false);
        }

        public static void UpdateCurve(bool print)
        {
            var currentCurve = GetCurrentCurve();
            if (print) Main.Logger.Msg(ConsoleColor.Yellow, $"Changing curve to {(changeVolumeCurve.Value ? "Custom Curve" : "Original Curve")}");
            if (print) Main.Logger.Msg(ConsoleColor.Yellow, $"Before change:");
            if (print) DrawCurveInConsole(currentCurve, ConsoleColor.DarkYellow);

            //Update curves
            CreateVolumeCurve(0, 0); 
            GenerateCustomCurve();
            _changeVolumeCurve = changeVolumeCurve.Value;
            foreach (var comms in GameObject.FindObjectsOfType<Comms_ParticipantPipeline>())
            {
                //Main.Logger.Msg($"Changing for {comms.gameObject.name}");
                comms.CreateVolumeCurve(0, MetaPort.Instance.settings.GetSettingsInt("AudioPlayerVoiceAttenuationDistanceFalloff", 8));
            }

            currentCurve = GetCurrentCurve();
            if (print) Main.Logger.Msg(ConsoleColor.Yellow, $"After change:");
            if (print) DrawCurveInConsole(currentCurve, ConsoleColor.DarkYellow);
        }


        public static AnimationCurve GetCurrentCurve()
        {
            if (_changeVolumeCurve)
            {
                return newVolumeCurve;
            }
            else
                return origVolumeCurve;
        }

        //Stock Curve Logic
        public static void CreateVolumeCurve(int minDistance, int maxDistance)
        {
            maxDistance = MetaPort.Instance.settings.GetSettingsInt("AudioPlayerVoiceAttenuationDistanceFalloff", 8);
            Keyframe[] keyframeArray = new Keyframe[maxDistance - minDistance];
            for (int index = 0; index < maxDistance - minDistance; ++index)
                keyframeArray[index] = new Keyframe((float)(index + minDistance), 1f - Mathf.Log((float)(index + 1), (float)maxDistance));
            origVolumeCurve = new AnimationCurve(keyframeArray);
        }

        public static void CompareCurves()
        {
            Main.Logger.Msg(ConsoleColor.Blue, $"===============================================================");

            Main.Logger.Msg(ConsoleColor.Cyan, $"Stock Curve:");
            DrawCurveInConsole(origVolumeCurve, ConsoleColor.DarkCyan);
            
            Main.Logger.Msg(ConsoleColor.Cyan, $"Custom Curve:");
            DrawCurveInConsole(newVolumeCurve, ConsoleColor.DarkCyan);
        }

        public static void DrawCurveInConsole(AnimationCurve curve, ConsoleColor color, float resolution = 0.01f, int height = 20, int width = 96)
        {
            if (curve == null)
            {
                Main.Logger.Warning("AnimationCurve is null!");
                return;
            }

            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            float tStart = curve.keys[0].time;
            float tEnd = curve.keys[curve.length - 1].time;

            // Calculate scaling factors for width
            float step = (tEnd - tStart) / width;

            // Find min and max values to scale the graph properly
            for (float t = tStart; t <= tEnd; t += resolution)
            {
                float value = curve.Evaluate(t);
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }

            string outLine = "Curve:\n";
            // Generate each line of the graph
            for (int i = 0; i <= height; i++)
            {
                float graphLine = minValue + (maxValue - minValue) * (height - i) / height;
                string line = string.Format("{0:F2} |", graphLine).PadRight(8); // Pad for alignment of graph
                for (float t = tStart; t <= tEnd; t += step)
                {
                    float value = curve.Evaluate(t);
                    if (Mathf.Abs(value - graphLine) < (maxValue - minValue) / height * 0.5f)
                        line += "*";
                    else
                        line += " ";
                }
                outLine += line + "\n"; 
                //Main.Logger.Msg(line);
            }

            string bottomKey = new string(' ', 8); // Offset for the vertical scale

            

            string valueKey = new string(' ', 8); // Offset for the vertical scale
            for (int j = 0; j <= width; j++)
            {
                if (j % (width / 8) == 0)  // Only mark each 1/8 increment
                {
                    bottomKey += "|";
                    float timeAtPosition = tStart + (tEnd - tStart) * j / width;
                    valueKey += string.Format("{0:F2}m", timeAtPosition).PadRight((width / 8));
                }
                else
                {
                    bottomKey += " ";
                }
            }

            var maxDist = MetaPort.Instance.settings.GetSettingsInt("AudioPlayerVoiceAttenuationDistanceFalloff", 8) - 1; //STOCK CURVE CUTS OFF LAST 
            string valueKeyDist = new string(' ', 8); // Offset for the vertical scale
            for (int j = 0; j <= width; j++)
            {
                if (j % (width / 8) == 0)  // Only mark each 1/8 increment
                {
                    float timeAtPosition = tStart + (tEnd - tStart) * j / width;
                    valueKeyDist += string.Format("{0:F2}", timeAtPosition / maxDist).PadRight((width / 8));
                }
            }
            
            outLine += bottomKey + "\n";
            outLine += valueKeyDist + "\n";
            outLine += valueKey + "\n";

            //var maxDist = MetaPort.Instance.settings.GetSettingInt("AudioPlayerVoiceAttenuationDistanceFalloff", 8);
            //string valueKeyDist = new string(' ', 8); // Offset for the vertical scale
            //for (int j = 0; j <= width; j++)
            //{
            //    if (j % (width / 8) == 0)  // Only mark each 1/8 increment
            //    {
            //        float timeAtPosition = tStart + (tEnd - tStart) * j / width;
            //        valueKeyDist += string.Format("{0:F2}m", timeAtPosition * maxDist).PadRight((width / 8));
            //    }
            //}
            //outLine += valueKeyDist + "\n";

            outLine += new string(' ', 36) + $"Time range: {tStart:F2} to {tEnd:F2}, Value range: {minValue:F2} to {maxValue:F2}" + "\n";
            foreach (Keyframe kf in curve.keys)
            {
                outLine += $"T:{kf.time:F2}V:{kf.value:F2}In:{kf.inTangent:F2}Out:{kf.outTangent:F2}" + " | ";
            }
            Main.Logger.Msg(color, outLine);
        }

        public static void PrintCurveValues(AnimationCurve curve, float resolution)
        {
            if (curve == null)
            {
                Main.Logger.Warning("AnimationCurve is null!");
                return;
            }
            Main.Logger.Msg(ConsoleColor.Yellow, "Curve Values:");
            for (float t = curve.keys[0].time; t <= curve.keys[curve.length - 1].time; t += resolution)
            {
                float value = curve.Evaluate(t);
                Main.Logger.Msg($"Time: {t:F2} Value {value:F2}");
            }
            // Ensure the last value is printed if the loop ends due to precision issues
            float lastTime = curve.keys[curve.length - 1].time;
            if (lastTime % resolution != 0)
            {
                float lastValue = curve.Evaluate(lastTime);
                Main.Logger.Msg($"Time: {lastTime:F2} Value {lastValue:F2}");
            }
        }
    }

    [HarmonyPatch]
    internal class HarmonyPatches
    {


        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Comms_ParticipantPipeline), nameof(Comms_ParticipantPipeline.LateUpdate))]
        //internal static void LateUpdate(Comms_ParticipantPipeline __instance)
        //{
        //    if (!Main.debugParticipantPipeline.Value)
        //        return;

        //    Vector3 remotePosition = __instance._puppetMaster.GetVoiceWorldPosition();
        //    Quaternion remoteRotation =  __instance._puppetMaster.GetVoiceWorldRotation();
        //    Transform transform = PlayerSetup.Instance.GetActiveCamera().transform;
        //    Vector3 camPosition = transform.position;
        //    Quaternion camRotation = transform.rotation;
        //    var distance = Vector3.Distance(camPosition, remotePosition);

        //    Vector3 speakerPosition = remotePosition;
        //    Vector3 speakerForward = remoteRotation * Vector3.forward;
        //    Vector3 listenerPosition = camPosition;
        //    Vector3 listenerForward = camRotation * Vector3.forward;
        //    Vector3 listenerRight = camRotation * Vector3.right;

        //    var speakerAngle = Vector3.Angle(speakerForward, listenerPosition - speakerPosition);
        //    var listenerAngle = Vector3.Angle(listenerForward, speakerPosition - listenerPosition);

        //    Main.Logger.Msg($"dist: {distance:F2} distAttn: {__instance._distanceAttenuation:F2} focusVol: {__instance._focusVolume:F2} _mastVol: {__instance._masterVolume:F2} playerVol: {__instance._playerVolume:F2} selfModVol: {__instance._selfModerationVolume:F2} voiceBoost: {__instance._voiceBoost:F2} listenerAngle: {listenerAngle:F2} speakerAngle: {speakerAngle:F2}");
        //}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Comms_ParticipantPipeline), nameof(Comms_ParticipantPipeline.CreateVolumeCurve))]
        internal static bool CreateVolumeCurve(Comms_ParticipantPipeline __instance)
        {
            if (Main._changeVolumeCurve)
            {
                //Main.Logger.Msg($"Applying custom curve for {__instance.gameObject.name}");
                __instance._volumeCurve = Main.newVolumeCurve;
                __instance.transform.GetChild(0).GetComponent<AudioSource>().SetCustomCurve(AudioSourceCurveType.CustomRolloff, __instance._volumeCurve);
                return false;
            }
            else
            {
                //Main.Logger.Msg($"Applying stock curve for {__instance.gameObject.name}");
                __instance._volumeCurve = Main.origVolumeCurve;
                __instance.transform.GetChild(0).GetComponent<AudioSource>().SetCustomCurve(AudioSourceCurveType.CustomRolloff, __instance._volumeCurve);
            }
            return true;
        }
    }
}








