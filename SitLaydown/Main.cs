using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using ABI.CCK;
using ABI.CCK.Components;
using ABI_RC.Core.InteractionSystem;
using HarmonyLib;


[assembly: MelonInfo(typeof(SitLaydown.Main), "SitLaydown", SitLaydown.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace SitLaydown
{

    public class Main : MelonMod
    {
        public const string versionStr = "0.1";
        public static MelonLogger.Instance Logger;

        public static bool firstload = true;

        public static MelonPreferences_Entry<string> SittingAnim;
        public static MelonPreferences_Entry<int> QMposition;
        public static MelonPreferences_Entry<int> QMhighlightColor;
        public static MelonPreferences_Entry<int> QMtogglePosOffset;
        public static MelonPreferences_Entry<float> DistAdjAmmount;
        public static MelonPreferences_Entry<bool> holdRotataion;

        public static GameObject _baseObj;
        public static System.Object rotRoutine = null;

        public static bool _DistHighPrec = false;
        public static float _DistAdj;
        public static bool rotActive = false;
        public static bool inChair = false;


        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("SitLaydown");

            loadAssets();

            MelonPreferences.CreateCategory("SitLaydown", "SitLaydown");
            SittingAnim = MelonPreferences.CreateEntry("SitLaydown", "SitLaydown", "SitCrossed", "Chair sitting animation");
            DistAdjAmmount = MelonPreferences.CreateEntry<float>("SitLaydown", "DistAdjAmmount", .01f, "High Precision Distance Adjustment");
            holdRotataion = MelonPreferences.CreateEntry<bool>("SitLaydown", "holdRotataion", true, "Hold body in original rotation");
            QMposition = MelonPreferences.CreateEntry<int>("SitLaydown", "QMposition", 0, "QuickMenu Position (0=Right, 1=Top, 2=Left)");
            QMhighlightColor = MelonPreferences.CreateEntry<int>("SitLaydown", "QMhighlightColor", 0, "Enabled color for QuickMenu items (0=Orange, 1=Yellow, 2=Pink)");
            QMtogglePosOffset = MelonPreferences.CreateEntry<int>("SitLaydown", "QMtogglePosOffset", 0, "Position Offset for settings toggle button (int -3 - 3)");
            OnPreferencesSaved();
        }



        public override void OnPreferencesSaved()
        {
            _DistAdj = _DistHighPrec ? DistAdjAmmount.Value : .1f;

            if (QM.settings != null)
            {
                switch (Main.QMposition.Value)
                {
                    case 1: QM.settingsRight.SetActive(false); QM.settingsTop.SetActive(true); QM.settingsLeft.SetActive(false); QM.settingsCanvas = QM.settings.transform.Find("MenuTop/SettingsMenuCanvas").gameObject; break; //Top
                    case 2: QM.settingsRight.SetActive(false); QM.settingsTop.SetActive(false); QM.settingsLeft.SetActive(true); QM.settingsCanvas = QM.settings.transform.Find("MenuLeft/SettingsMenuCanvas").gameObject; break; //Left
                    default: QM.settingsRight.SetActive(true); QM.settingsTop.SetActive(false); QM.settingsLeft.SetActive(false); QM.settingsCanvas = QM.settings.transform.Find("MenuRight/SettingsMenuCanvas").gameObject; break; //Right
                }

                QM.settingsRight.transform.Find("ToggleSettingsCanvas").transform.localPosition = new Vector3((.4f + Main.QMtogglePosOffset.Value * .8f), -.8f, 0f);
                QM.settingsTop.transform.Find("ToggleSettingsCanvas").transform.localPosition = new Vector3((.4f + Main.QMtogglePosOffset.Value * .8f), -.8f, 0f);
                QM.settingsLeft.transform.Find("ToggleSettingsCanvas").transform.localPosition = new Vector3((-.4f + Main.QMtogglePosOffset.Value * .8f), -.8f, 0f);
                QM.ParseSettings();
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)//Without switch this would run 3 times at world load
            {
                case 0: break;
                case 1: break;
                case 2: break;
                default:
                    if (firstload)
                    {
                        HarmonyInstance.Patch(typeof(CVR_MenuManager).GetMethod(nameof(CVR_MenuManager.ToggleQuickMenu)), null, new HarmonyMethod(typeof(Main).GetMethod(nameof(QMtoggle), BindingFlags.NonPublic | BindingFlags.Static)));
                        //Logger.Msg("default" + buildIndex);
                        firstload = false;
                        QM.CreateQuickMenuButton();
                    }
                    QM.ParseSettings();
                    break;
            }
        }

        private static void QMtoggle(bool __0)
        {
            MelonCoroutines.Start(DelayQM(__0));
        }

        public static IEnumerator DelayQM(bool value)
        {
            yield return new WaitForSeconds(.22f);
            QM.settings.SetActive(value);
        }

        public static void ToggleChair(Boolean enableChair)
        {
            //Logger.Msg(enableChair);
            if (!enableChair && _baseObj != null)
            {
                UnityEngine.Object.Destroy(_baseObj);
                _baseObj = null;
                if(rotRoutine!=null) MelonCoroutines.Stop(rotRoutine);
                rotActive = false;
                inChair = false;
            }
            if (enableChair)
            {
                GameObject baseObj = GameObject.Instantiate(chairPrefab);
                GameObject play = Utils.GetPlayer().transform.Find("[PlayerAvatar]").GetChild(0).gameObject;
                baseObj.transform.position = play.transform.position;
                baseObj.transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);//(play.transform.rotation) * Quaternion.AngleAxis(-90, Vector3.up);
                baseObj.GetComponent<CVRInteractable>().actions[0].operations[0].animationVal = GetChairAnim();
                
                MelonCoroutines.Start(TriggerChair());
                 _baseObj = baseObj;
            }
        }

        public static IEnumerator TriggerChair()
        {
            yield return new WaitForSeconds(.1f);
            _baseObj.GetComponent<CVRInteractable>().actions[0].operations[0].gameObjectVal.GetComponent<CVRSeat>().onExitSeat.AddListener(new UnityEngine.Events.UnityAction(() =>
                {
                    ToggleChair(false);
                    QM.ParseSettings();
                    //Logger.Msg("Left chair, toggling off");
                }
                ));
            _baseObj.GetComponent<CVRInteractable>().CustomTrigger();
            
            inChair = true;
            if (holdRotataion.Value) rotRoutine = MelonCoroutines.Start(HoldRotation());
        }

        public static IEnumerator HoldRotation()
        {
            var rot = Utils.GetPlayer().transform.Find("[PlayerAvatar]").GetChild(0).transform.rotation;
            rotActive = true;
            while (_baseObj != null && Main.holdRotataion.Value)
            {
                yield return new WaitForEndOfFrame();
                Utils.GetPlayer().transform.Find("[PlayerAvatar]").GetChild(0).transform.rotation = rot;
            }
            rotActive = false;
        }


        public static void RotateAnim()
        {
            switch (Main.SittingAnim.Value)
            {
                case "BasicSit": Main.SittingAnim.Value = "SitIdle"; break;
                case "SitIdle": Main.SittingAnim.Value = "SitCrossed"; break;
                case "SitCrossed": Main.SittingAnim.Value = "Laydown"; break;
                case "Laydown": Main.SittingAnim.Value = "BasicSit"; break;
                default: Main.SittingAnim.Value = "SitCrossed"; Main.Logger.Msg("Something Broke - Main.RotateAnim - Switch"); break;
            }
        }


        public static AnimationClip GetChairAnim()
        {
            switch (SittingAnim.Value)
            {
                //case "None": return null;
                case "SitIdle": return SitIdle;
                case "SitCrossed": return SitCrossed;
                case "Laydown": return Laydown;
                case "BasicSit": return BasicSit;
                default: Logger.Error("Something Broke - GetChairAnim Switch"); return null;
            }
        }


        private void loadAssets()
        {//https://github.com/ddakebono/BTKSASelfPortrait/blob/master/BTKSASelfPortrait.cs
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.laysit"))
            {
                using (var tempStream = new MemoryStream((int)assetStream.Length))
                {
                    assetStream.CopyTo(tempStream);
                    assetBundle = AssetBundle.LoadFromMemory(tempStream.ToArray(), 0);
                    assetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                }
            }
            if (assetBundle != null)
            {
                settingsPrefab = assetBundle.LoadAsset<GameObject>("SitLaySettings");
                settingsPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                chairPrefab = assetBundle.LoadAsset<GameObject>("ChairPrefab");
                chairPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                SitIdle = assetBundle.LoadAsset<AnimationClip>("Sitting Idle");
                SitIdle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                SitCrossed = assetBundle.LoadAsset<AnimationClip>("Sitting Pose Legs Crossed");
                Laydown = assetBundle.LoadAsset<AnimationClip>("Lay_Down - Feet closer");
                Laydown.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                BasicSit = assetBundle.LoadAsset<AnimationClip>("Sitting Legs Down");
                BasicSit.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
            else Logger.Error("Bundle was null");
        }


        public static AssetBundle assetBundle;
        public static GameObject settingsPrefab;

        public static GameObject chairPrefab;

        public static AnimationClip SitIdle;
        public static AnimationClip SitCrossed;
        public static AnimationClip Laydown;
        public static AnimationClip BasicSit;


    }
}


