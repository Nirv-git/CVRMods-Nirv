using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using ABI.CCK.Components;
using ABI_RC.Core.Player;
using ABI_RC.Systems.IK.SubSystems;
using ABI_RC.Core.InteractionSystem;
using HarmonyLib;

[assembly: MelonInfo(typeof(PortableMirror.Main), "PortableMirrorMod", PortableMirror.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace PortableMirror
{

    public class Main : MelonMod
    {
        public const string versionStr = "2.1.6";
        public static MelonLogger.Instance Logger;

        public static bool firstload = true;

        public static MelonPreferences_Entry<bool> MirrorKeybindEnabled;
        public static MelonPreferences_Entry<bool> Spacer1;
        public static MelonPreferences_Entry<bool> Spacer2;
        public static MelonPreferences_Entry<bool> fixRenderOrder;
        public static MelonPreferences_Entry<bool> usePixelLights;
        public static MelonPreferences_Entry<bool> QMstartMax;
        public static MelonPreferences_Entry<bool> pickupFrame;
        public static MelonPreferences_Entry<int> QMposition;
        public static MelonPreferences_Entry<bool> QMsmaller;
        public static MelonPreferences_Entry<int> QMhighlightColor;
        public static MelonPreferences_Entry<bool> enableGaze;
        //public static MelonPreferences_Entry<float> followGazeSpeed;
        public static MelonPreferences_Entry<float> followGazeTime;
        public static MelonPreferences_Entry<float> followGazeDeadBand;
        public static MelonPreferences_Entry<float> followGazeDeadBandSettle;

        public static MelonPreferences_Entry<bool> grabTest;
        public static MelonPreferences_Entry<float> grabTestSpeed;



        public static MelonPreferences_Entry<bool> ActionMenu;


        public static MelonPreferences_Entry<float> _base_MirrorScaleX;
        public static MelonPreferences_Entry<float> _base_MirrorScaleY;
        public static MelonPreferences_Entry<float> _base_MirrorDistance;
        public static MelonPreferences_Entry<string> _base_MirrorState;
        public static MelonPreferences_Entry<bool> _base_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _base_enableBase;
        public static MelonPreferences_Entry<bool> _base_PositionOnView;
        public static MelonPreferences_Entry<bool> _base_AnchorToTracking;
        public static MelonPreferences_Entry<bool> _base_followGaze;

        public static MelonPreferences_Entry<bool> QuickMenuOptions;
        public static MelonPreferences_Entry<bool> OpenLastQMpage;
        public static MelonPreferences_Entry<float> TransMirrorTrans;
        //public static MelonPreferences_Entry<bool> MirrorsShowInCamera;
        public static MelonPreferences_Entry<float> MirrorDistAdjAmmount;
        public static MelonPreferences_Entry<bool> amapi_ModsFolder;
        public static MelonPreferences_Entry<float> ColliderDepth;
        public static MelonPreferences_Entry<bool> PickupToHand;

        public static MelonPreferences_Entry<float> _45_MirrorScaleX;
        public static MelonPreferences_Entry<float> _45_MirrorScaleY;
        public static MelonPreferences_Entry<float> _45_MirrorDistance;
        public static MelonPreferences_Entry<string> _45_MirrorState;
        public static MelonPreferences_Entry<bool> _45_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _45_enable45;
        public static MelonPreferences_Entry<bool> _45_AnchorToTracking;
        public static MelonPreferences_Entry<bool> _45_followGaze;

        public static MelonPreferences_Entry<float> _ceil_MirrorScaleX;
        public static MelonPreferences_Entry<float> _ceil_MirrorScaleZ;
        public static MelonPreferences_Entry<float> _ceil_MirrorDistance;
        public static MelonPreferences_Entry<string> _ceil_MirrorState;
        public static MelonPreferences_Entry<bool> _ceil_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _ceil_enableCeiling;
        public static MelonPreferences_Entry<bool> _ceil_AnchorToTracking;

        public static MelonPreferences_Entry<float> _micro_MirrorScaleX;
        public static MelonPreferences_Entry<float> _micro_MirrorScaleY;
        public static MelonPreferences_Entry<float> _micro_MirrorDistance;
        public static MelonPreferences_Entry<float> _micro_GrabRange;
        public static MelonPreferences_Entry<string> _micro_MirrorState;
        public static MelonPreferences_Entry<bool> _micro_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _micro_enableMicro;
        public static MelonPreferences_Entry<bool> _micro_AnchorToTracking;
        public static MelonPreferences_Entry<bool> _micro_PositionOnView;
        public static MelonPreferences_Entry<bool> _micro_followGaze;


        public static MelonPreferences_Entry<float> _trans_MirrorScaleX;
        public static MelonPreferences_Entry<float> _trans_MirrorScaleY;
        public static MelonPreferences_Entry<float> _trans_MirrorDistance;
        public static MelonPreferences_Entry<string> _trans_MirrorState;
        public static MelonPreferences_Entry<bool> _trans_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _trans_enableTrans;
        public static MelonPreferences_Entry<bool> _trans_AnchorToTracking;
        public static MelonPreferences_Entry<bool> _trans_PositionOnView;
        public static MelonPreferences_Entry<bool> _trans_followGaze;

        public static MelonPreferences_Entry<bool> _cal_enable;
        public static MelonPreferences_Entry<float> _cal_MirrorScale;
        public static MelonPreferences_Entry<float> _cal_MirrorDistanceScale;
        public static MelonPreferences_Entry<string> _cal_MirrorState;
        public static MelonPreferences_Entry<bool> _cal_AlwaysInFront;
        public static MelonPreferences_Entry<bool> _cal_DelayMirror;
        public static MelonPreferences_Entry<float> _cal_DelayMirrorTime;
        public static MelonPreferences_Entry<bool> _cal_DelayOff;
        public static MelonPreferences_Entry<float> _cal_DelayOffTime;
        public static MelonPreferences_Entry<bool> _cal_hideOthers;

        public static MelonPreferences_Entry<bool> distanceDisable;
        public static MelonPreferences_Entry<float> distanceValue;
        public static MelonPreferences_Entry<float> distanceUpdateInit;



        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("PortableMirrorMod");

            Mirrors.loadAssets();

            MelonPreferences.CreateCategory("PortableMirror", "PortableMirror");
            QMstartMax = MelonPreferences.CreateEntry<bool>("PortableMirror", "QMstartMax", false, "QuickMenu (QM) Starts Maximized");
            QMposition = MelonPreferences.CreateEntry<int>("PortableMirror", "QMposition", 0, "QM Position (0=Right, 1=Top, 2=Left)");
            QMsmaller = MelonPreferences.CreateEntry<bool>("PortableMirror", "QMsmaller", false, "QM is smaller");
            QMhighlightColor = MelonPreferences.CreateEntry<int>("PortableMirror", "QMhighlightColor", 0, "Enabled color for QM (0=Orange, 1=Yellow, 2=Pink)");
            //ActionMenu = MelonPreferences.CreateEntry<bool>("PortableMirror", "ActionMenu", true, "Enable Controls on Action Menu (Requires Restart)");

            MirrorKeybindEnabled = MelonPreferences.CreateEntry<bool>("PortableMirror", "MirrorKeybindEnabled", false, "Enabled Mirror Keybind (See ReadMe)");
            Spacer1 = MelonPreferences.CreateEntry<bool>("PortableMirror", "Spacer1", false, "-These are global settings for all portable mirror types-");///
            fixRenderOrder = MelonPreferences.CreateEntry("PortableMirror", "fixRenderOrder", false, "Change render order on mirrors to fix overrendering --Don't use--", "", true);
            MirrorDistAdjAmmount = MelonPreferences.CreateEntry<float>("PortableMirror", "MirrorDistAdjAmmount", .05f, "High Precision Distance Adjustment");
            ColliderDepth = MelonPreferences.CreateEntry<float>("PortableMirror", "ColliderDepth", 0.01f, "Collider Depth");
            pickupFrame = MelonPreferences.CreateEntry<bool>("PortableMirror", "pickupFrame", false, "Show frame when mirror is pickupable");
            enableGaze = MelonPreferences.CreateEntry<bool>("PortableMirror", "enableGaze", true, "Enable 'Follow Gaze' by clicking Anchor to Tracking button twice");
            //followGazeSpeed = MelonPreferences.CreateEntry<float>("PortableMirror", "followGazeSpeed", .6f, "Follow Gaze Speed");
            followGazeTime = MelonPreferences.CreateEntry<float>("PortableMirror", "followGazeTime", 0.5f, "Follow Gaze Time");
            followGazeDeadBand = MelonPreferences.CreateEntry<float>("PortableMirror", "followGazeDeadBand", 5f, "Follow Gaze DeadBand");
            followGazeDeadBandSettle = MelonPreferences.CreateEntry<float>("PortableMirror", "followGazeDeadBandSettle", 1f, "Follow Gaze DeadBand Settle");
            grabTest = MelonPreferences.CreateEntry<bool>("PortableMirror", "grabTest", false, "grabTest");
            grabTestSpeed = MelonPreferences.CreateEntry<float>("PortableMirror", "grabTestSpeed", .5f, "grabTestSpeed");

            Spacer2 = MelonPreferences.CreateEntry<bool>("PortableMirror", "Spacer2", false, "-These options are on the QM also-");///
            usePixelLights = MelonPreferences.CreateEntry<bool>("PortableMirror", "usePixelLights", false, "Use PixelLights for mirrors");
            PickupToHand = MelonPreferences.CreateEntry<bool>("PortableMirror", "PickupToHand", false, "Pickups snap to hand - Global for all mirrors");
            TransMirrorTrans = MelonPreferences.CreateEntry<float>("PortableMirror", "TransMirrorTrans", .4f, "Transparent Mirror transparency - Higher is more transparent - Global for all mirrors");

            //MirrorsShowInCamera = MelonPreferences.CreateEntry<bool>("PortableMirror", "MirrorsShowInCamera", true, "Mirrors show in Cameras - Global for all mirrors --Don't use--");

            MelonPreferences.CreateCategory("PortableMirrorDistDisable", "PortableMirror Distance Disable");
            distanceDisable = MelonPreferences.CreateEntry<bool>("PortableMirrorDistDisable", "distanceDisable", false, "Disable avatars > than a distane from showing in Mirrors");
            distanceValue = MelonPreferences.CreateEntry<float>("PortableMirrorDistDisable", "distanceValue", 3f, "Distance in meteres");
            distanceUpdateInit = MelonPreferences.CreateEntry<float>("PortableMirrorDistDisable", "distanceUpdateInit", .5f, "Update interval");

            MelonPreferences.CreateCategory("PortableMirrorBase", "PortableMirror Base");
            _base_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorBase", "MirrorScaleX", 2f, "Mirror Scale X");
            _base_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirrorBase", "MirrorScaleY", 3f, "Mirror Scale Y");
            _base_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorBase", "MirrorDistance", 0f, "Mirror Distance");
            _base_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorBase", "MirrorState", "MirrorFull", "Mirror Type (MirrorFull or MirrorOpt)");
            _base_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorBase", "CanPickupMirror", false, "Can Pickup Mirror");
            _base_PositionOnView = MelonPreferences.CreateEntry<bool>("PortableMirrorBase", "PositionOnView", false, "Position mirror based on view angle");
            _base_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorBase", "AnchorToTracking", false, "Mirror Follows You");
            _base_followGaze = MelonPreferences.CreateEntry<bool>("PortableMirrorBase", "followGaze", false, "Follow Gaze Enabled");

            MelonPreferences.CreateCategory("PortableMirror45", "PortableMirror 45 Degree");
            _45_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirror45", "MirrorScaleX", 5f, "Mirror Scale X");
            _45_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirror45", "MirrorScaleY", 4f, "Mirror Scale Y");
            _45_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirror45", "MirrorDistance", 0f, "Mirror Distance");
            _45_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirror45", "MirrorState", "MirrorFull", "Mirror Type (MirrorFull or MirrorOpt)");
            _45_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirror45", "CanPickupMirror", false, "Can Pickup 45 Mirror");
            _45_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirror45", "AnchorToTracking", false, "Mirror Follows You");
            _45_followGaze = MelonPreferences.CreateEntry<bool>("PortableMirror45", "followGaze", false, "Follow Gaze Enabled");

            MelonPreferences.CreateCategory("PortableMirrorCeiling", "PortableMirror Ceiling");
            _ceil_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorCeiling", "MirrorScaleX", 5f, "Mirror Scale X");
            _ceil_MirrorScaleZ = MelonPreferences.CreateEntry<float>("PortableMirrorCeiling", "MirrorScaleZ", 5f, "Mirror Scale Z");
            _ceil_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorCeiling", "MirrorDistance", 2, "Mirror Distance");
            _ceil_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorCeiling", "MirrorState", "MirrorFull", "Mirror Type (MirrorFull or MirrorOpt)");
            _ceil_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorCeiling", "CanPickupMirror", false, "Can Pickup Ceiling Mirror");
            _ceil_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorCeiling", "AnchorToTracking", false, "Mirror Follows You");

            MelonPreferences.CreateCategory("PortableMirrorMicro", "PortableMirror Micro");
            _micro_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorMicro", "MirrorScaleX", .05f, "Mirror Scale X");
            _micro_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirrorMicro", "MirrorScaleY", .1f, "Mirror Scale Y");
            _micro_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorMicro", "MirrorDistance", 0f, "Mirror Distance");
            _micro_GrabRange = MelonPreferences.CreateEntry<float>("PortableMirrorMicro", "GrabRange", .1f, "GrabRange");
            _micro_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorMicro", "MirrorState", "MirrorFull", "Mirror Type (MirrorFull or MirrorOpt)");
            _micro_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorMicro", "CanPickupMirror", false, "Can Pickup MirrorMicro");
            _micro_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorMicro", "PositionOnView", false, "Position mirror based on view angle");
            _micro_PositionOnView = MelonPreferences.CreateEntry<bool>("PortableMirrorMicro", "AnchorToTracking", false, "Mirror Follows You");
            _micro_followGaze = MelonPreferences.CreateEntry<bool>("PortableMirrorMicro", "followGaze", false, "Follow Gaze Enabled");

            MelonPreferences.CreateCategory("PortableMirrorTrans", "PortableMirror Transparent");
            _trans_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorTrans", "MirrorScaleX", 5f, "Mirror Scale X");
            _trans_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirrorTrans", "MirrorScaleY", 3f, "Mirror Scale Y");
            _trans_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorTrans", "MirrorDistance", 0f, "Mirror Distance");
            _trans_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorTrans", "MirrorState", "MirrorTransparent", "Mirror Type - Resets to Transparent on load");
            _trans_MirrorState.Value = "MirrorTransparent"; //Force to Transparent every load
            _trans_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorTrans", "CanPickupMirror", false, "Can Pickup Mirror");
            _trans_PositionOnView = MelonPreferences.CreateEntry<bool>("PortableMirrorTrans", "PositionOnView", false, "Position mirror based on view angle");
            _trans_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorTrans", "AnchorToTracking", false, "Mirror Follows You");
            _trans_followGaze = MelonPreferences.CreateEntry<bool>("PortableMirrorTrans", "followGaze", false, "Follow Gaze Enabled");

            MelonPreferences.CreateCategory("PortableMirrorCal", "PortableMirror Calibration");
            _cal_enable = MelonPreferences.CreateEntry<bool>("PortableMirrorCal", "MirrorEnable", true, "Enable Mirror when Calibrating");
            _cal_MirrorScale = MelonPreferences.CreateEntry<float>("PortableMirrorCal", "MirrorScale", 1f, "MirrorScale");
            _cal_MirrorDistanceScale = MelonPreferences.CreateEntry<float>("PortableMirrorCal", "MirrorDistanceScale", 1f, "MirrorDistanceScale");
            _cal_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorCal", "MirrorState", "MirrorCutoutSolo", "Mirror Type");
            //_cal_AlwaysInFront = MelonPreferences.CreateEntry<bool>("PortableMirrorCal", "AlwaysInFront", false, "Mirror is always infront of where you are looking");
            _cal_DelayMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorCal", "DelayMirror2", true, "Delay Mirror Creation for x seconds");
            _cal_DelayMirrorTime = MelonPreferences.CreateEntry<float>("PortableMirrorCal", "DelayMirrorTime", 1f, "Delay Mirror Time");
            _cal_hideOthers = MelonPreferences.CreateEntry<bool>("PortableMirrorCal", "hideOthers", true, "Hide other mirrors while calibrating");
            _cal_DelayOff = MelonPreferences.CreateEntry<bool>("PortableMirrorCal", "DelayOff", false, "Delay Mirror Deletion for x seconds");
            _cal_DelayOffTime = MelonPreferences.CreateEntry<float>("PortableMirrorCal", "DelayOffTime", 5f, "Delay Mirror Deletion Time");

            _oldMirrorScaleYBase = Main._base_MirrorScaleY.Value;
            _oldMirrorDistance = Main._base_MirrorDistance.Value;
            _oldMirrorScaleY45 = Main._45_MirrorScaleY.Value;
            _oldMirrorDistance45 = Main._45_MirrorDistance.Value;
            _oldMirrorDistanceCeiling = Main._ceil_MirrorDistance.Value;
            _oldMirrorScaleYMicro = Main._micro_MirrorScaleY.Value;
            _oldMirrorDistanceMicro = Main._micro_MirrorDistance.Value;
            _oldMirrorScaleYTrans = Main._trans_MirrorScaleY.Value;
            _oldMirrorDistanceTrans = Main._trans_MirrorDistance.Value;

            if (MirrorKeybindEnabled.Value)
            { //God help you
                Logger.Msg($"[Num1] -> Toggle portable mirror" +
                    $"\n[Num2] -> Toggle 45 mirror" +
                    $"\n[Num3] -> Toggle ceiling mirror" +
                    $"\n[Num*] -> Toggle micro mirror" +
                    $"\n[Num4] -> Portable Mirror Distance -" +
                    $"\n[Num7] -> Portable Mirror Distance +" +
                    $"\n[Num8] -> 45 Mirror Distance -" +
                    $"\n[Num/] -> 45 Mirror Distance +" +
                    $"\n[Num6] -> Ceiling Mirror Distance -" +
                    $"\n[Num9] -> Ceiling Mirror Distance +" +
                    $"\n[NumEnter] -> Toggle Pickup on All" +
                    $"\n[Num+] -> Mirror Size + on All" +
                    $"\n[Num-] -> Mirror Size - on All" +
                    $"\n[Num.] -> Toggle position on view for base and micro mirrors");
            }

            //if (MelonHandler.Mods.Any(m => m.Info.Name == "ActionMenu") && ActionMenu.Value)
            //{
            //    CustomActionMenu.InitUi();
            //}
            //else Logger.Msg("ActionMenu is missing, or setting is toggled off in Mod Settings - Not adding controls to ActionMenu");

        }
        public override void OnPreferencesSaved()
        {
            if (firstload) return; //Other mods, stop calling on saved for all mods OnAppInit! 
            if (QM.settings != null)
            {
                switch (Main.QMposition.Value)
                {
                    case 1: QM.settingsRight.SetActive(false); QM.settingsTop.SetActive(true); QM.settingsLeft.SetActive(false); QM.settingsCanvas = QM.settings.transform.Find("MenuTop/SettingsMenuCanvas").gameObject; break; //Top
                    case 2: QM.settingsRight.SetActive(false); QM.settingsTop.SetActive(false); QM.settingsLeft.SetActive(true); QM.settingsCanvas = QM.settings.transform.Find("MenuLeft/SettingsMenuCanvas").gameObject; break; //Left
                    default: QM.settingsRight.SetActive(true); QM.settingsTop.SetActive(false); QM.settingsLeft.SetActive(false); QM.settingsCanvas = QM.settings.transform.Find("MenuRight/SettingsMenuCanvas").gameObject; break; //Right
                }

                if (Main.QMsmaller.Value)
                {//smoll
                    QM.settingsRight.transform.localPosition = new Vector3(63f, 0f, -0.05f);  //Right
                    QM.settingsTop.transform.localPosition = new Vector3(-30f, 77f, -0.05f); //Top
                    QM.settingsLeft.transform.localPosition = new Vector3(-66f, 0f, -0.05f); //Left

                    QM.settingsRight.transform.localScale = new Vector3(10f, 10f, 20f);
                    QM.settingsTop.transform.localScale = new Vector3(10f, 10f, 20f);
                    QM.settingsLeft.transform.localScale = new Vector3(10f, 10f, 20f);
                }
                else
                {//big
                    QM.settingsRight.transform.localPosition = new Vector3(77f, 0f, -0.05f);
                    QM.settingsTop.transform.localPosition = new Vector3(-42f, 105f, -0.05f);
                    QM.settingsLeft.transform.localPosition = new Vector3(-77f, 0f, -0.05f);

                    QM.settingsRight.transform.localScale = new Vector3(20f, 20f, 20f);
                    QM.settingsTop.transform.localScale = new Vector3(20f, 20f, 20f);
                    QM.settingsLeft.transform.localScale = new Vector3(20f, 20f, 20f);
                }
            }


            if (Main.distanceDisable.Value)
            {
                if (!DistanceDisable.distActive)
                    DistanceDisable.distDisableRoutine = MelonCoroutines.Start(DistanceDisable.DistDisableUpdate());
            }
            else if (DistanceDisable.distWasActive)
            {
                DistanceDisable.distWasActive = false;
                DistanceDisable.ReturnAllPlayersToNormal();
            }


            _mirrorDistAdj = _mirrorDistHighPrec ? MirrorDistAdjAmmount.Value : .25f;
            if (_mirrorBase != null && Utils.GetPlayer() != null)
            {
                if (_base_followGaze.Value)
                    if (!Mirrors._baseFollowGazeActive)
                        MelonCoroutines.Start(Mirrors.followGazeBase());

                _mirrorBase.transform.SetParent(null);
                _mirrorBase.transform.localScale = new Vector3(Main._base_MirrorScaleX.Value, Main._base_MirrorScaleY.Value, 0.05f);
                _mirrorBase.transform.position = new Vector3(_mirrorBase.transform.position.x, _mirrorBase.transform.position.y + ((Main._base_MirrorScaleY.Value - _oldMirrorScaleYBase) / 2), _mirrorBase.transform.position.z  );
                _mirrorBase.transform.position += _mirrorBase.transform.forward * (Main._base_MirrorDistance.Value - _oldMirrorDistance);
                _mirrorBase.GetOrAddComponent<BoxCollider>().enabled = Main._base_CanPickupMirror.Value;
                _mirrorBase.GetOrAddComponent<CVRPickupObject>().enabled = Main._base_CanPickupMirror.Value;
                _mirrorBase.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                if (Main._base_MirrorState.Value == "MirrorCutout" || Main._base_MirrorState.Value == "MirrorTransparent" || Main._base_MirrorState.Value == "MirrorCutoutSolo" || Main._base_MirrorState.Value == "MirrorTransparentSolo") Mirrors.SetAllMirrorsToIgnoreShader();
                for (int i = 0; i < _mirrorBase.transform.childCount; i++)
                    _mirrorBase.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirrorBase.transform.Find(Main._base_MirrorState.Value);
                childMirror.gameObject.active = true;
                if (Main._base_MirrorState.Value == "MirrorTransparent" || Main._base_MirrorState.Value == "MirrorTransparentSolo" ||
                   Main._base_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                _mirrorBase.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (Main._base_AnchorToTracking.Value) _mirrorBase.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                _mirrorBase.transform.Find("Frame").gameObject.SetActive(_base_CanPickupMirror.Value & pickupFrame.Value);
                if (Main._base_MirrorState.Value == "MirrorCutoutSolo" || Main._base_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, false));
                if (Main._base_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, true));
            }

            _oldMirrorScaleYBase = Main._base_MirrorScaleY.Value;
            _oldMirrorDistance = Main._base_MirrorDistance.Value;

            if (_mirror45 != null && Utils.GetPlayer() != null)
            {
                if (_45_followGaze.Value)
                    if (!Mirrors._45FollowGazeActive)
                        MelonCoroutines.Start(Mirrors.followGaze45());

                _mirror45.transform.SetParent(null);
                _mirror45.transform.localScale = new Vector3(Main._45_MirrorScaleX.Value, Main._45_MirrorScaleY.Value, 0.05f);
                _mirror45.transform.rotation = _mirror45.transform.rotation * Quaternion.AngleAxis(-45, Vector3.left);
                _mirror45.transform.position = new Vector3(_mirror45.transform.position.x, _mirror45.transform.position.y + ((Main._45_MirrorScaleY.Value - _oldMirrorScaleY45)/2.5f), _mirror45.transform.position.z  );
                _mirror45.transform.position += _mirror45.transform.forward * (Main._45_MirrorDistance.Value - _oldMirrorDistance45);
                _mirror45.transform.rotation = _mirror45.transform.rotation * Quaternion.AngleAxis(45, Vector3.left);

                _mirror45.GetOrAddComponent<BoxCollider>().enabled = Main._45_CanPickupMirror.Value;
                _mirror45.GetOrAddComponent<CVRPickupObject>().enabled = Main._45_CanPickupMirror.Value;
                _mirror45.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;

                if (Main._45_MirrorState.Value == "MirrorCutout" || Main._45_MirrorState.Value == "MirrorTransparent" || Main._45_MirrorState.Value == "MirrorCutoutSolo" || Main._45_MirrorState.Value == "MirrorTransparentSolo") Mirrors.SetAllMirrorsToIgnoreShader();
                for (int i = 0; i < _mirror45.transform.childCount; i++)
                    _mirror45.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirror45.transform.Find(Main._45_MirrorState.Value);
                childMirror.gameObject.active = true;
                if (Main._45_MirrorState.Value == "MirrorTransparent" || Main._45_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._45_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                _mirror45.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (Main._45_AnchorToTracking.Value) _mirror45.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                _mirror45.transform.Find("Frame").gameObject.SetActive(_45_CanPickupMirror.Value & pickupFrame.Value);
                if (Main._45_MirrorState.Value == "MirrorCutoutSolo" || Main._45_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, false));
                if (Main._45_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, true));

            }
            _oldMirrorScaleY45 = Main._45_MirrorScaleY.Value;
            _oldMirrorDistance45 = Main._45_MirrorDistance.Value;


            if (_mirrorCeiling != null && Utils.GetPlayer() != null)
            {
                _mirrorCeiling.transform.SetParent(null);
                _mirrorCeiling.transform.localScale = new Vector3(Main._ceil_MirrorScaleX.Value, Main._ceil_MirrorScaleZ.Value, 0.05f);
                _mirrorCeiling.transform.position = new Vector3(_mirrorCeiling.transform.position.x, _mirrorCeiling.transform.position.y + (Main._ceil_MirrorDistance.Value - _oldMirrorDistanceCeiling), _mirrorCeiling.transform.position.z);

                _mirrorCeiling.GetOrAddComponent<BoxCollider>().enabled = Main._ceil_CanPickupMirror.Value;
                _mirrorCeiling.GetOrAddComponent<CVRPickupObject>().enabled = Main._ceil_CanPickupMirror.Value;
                _mirrorCeiling.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;

                if (Main._ceil_MirrorState.Value == "MirrorCutout" || Main._ceil_MirrorState.Value == "MirrorTransparent" || Main._ceil_MirrorState.Value == "MirrorCutoutSolo" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo") Mirrors.SetAllMirrorsToIgnoreShader();
                for (int i = 0; i < _mirrorCeiling.transform.childCount; i++)
                    _mirrorCeiling.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirrorCeiling.transform.Find(Main._ceil_MirrorState.Value);
                childMirror.gameObject.active = true;
                if (Main._ceil_MirrorState.Value == "MirrorTransparent" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._ceil_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                _mirrorCeiling.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (Main._ceil_AnchorToTracking.Value)  _mirrorCeiling.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                _mirrorCeiling.transform.Find("Frame").gameObject.SetActive(_ceil_CanPickupMirror.Value & pickupFrame.Value);
                if (Main._ceil_MirrorState.Value == "MirrorCutoutSolo" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, false));
                if (Main._ceil_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, true));

            }
            _oldMirrorDistanceCeiling = Main._ceil_MirrorDistance.Value;


            if (_mirrorMicro != null && Utils.GetPlayer() != null)
            {
                if (_micro_followGaze.Value)
                    if (!Mirrors._microFollowGazeActive)
                        MelonCoroutines.Start(Mirrors.followGazeMicro());

                _mirrorMicro.transform.SetParent(null);
                _mirrorMicro.transform.localScale = new Vector3(Main._micro_MirrorScaleX.Value, Main._micro_MirrorScaleY.Value, 0.05f);
                _mirrorMicro.transform.position = new Vector3(_mirrorMicro.transform.position.x, _mirrorMicro.transform.position.y + ((Main._micro_MirrorScaleY.Value - _oldMirrorScaleYMicro) / 2), _mirrorMicro.transform.position.z);
                _mirrorMicro.transform.position += _mirrorMicro.transform.forward * (Main._micro_MirrorDistance.Value - _oldMirrorDistanceMicro);


                _mirrorMicro.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = Main._micro_GrabRange.Value;
                _mirrorMicro.GetOrAddComponent<BoxCollider>().enabled = Main._micro_CanPickupMirror.Value;
                _mirrorMicro.GetOrAddComponent<CVRPickupObject>().enabled = Main._micro_CanPickupMirror.Value;
                _mirrorMicro.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;

                if (Main._micro_MirrorState.Value == "MirrorCutout" || Main._micro_MirrorState.Value == "MirrorTransparent" || Main._micro_MirrorState.Value == "MirrorCutoutSolo" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") Mirrors.SetAllMirrorsToIgnoreShader();
                for (int i = 0; i < _mirrorMicro.transform.childCount; i++)
                    _mirrorMicro.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirrorMicro.transform.Find(Main._micro_MirrorState.Value);
                childMirror.gameObject.active = true;
                if (Main._micro_MirrorState.Value == "MirrorTransparent" || Main._micro_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._micro_MirrorState.Value == "MirrorTransCutCombo") 
                { 
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._micro_AnchorToTracking.Value) _mirrorMicro.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                if (Main._micro_MirrorState.Value == "MirrorCutoutSolo" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, false));
                if (Main._micro_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, true));

            }
            _oldMirrorScaleYMicro = Main._micro_MirrorScaleY.Value;
            _oldMirrorDistanceMicro = Main._micro_MirrorDistance.Value;


            if (_mirrorTrans != null && Utils.GetPlayer() != null)
            {
                if (_trans_followGaze.Value)
                    if (!Mirrors._transFollowGazeActive)
                        MelonCoroutines.Start(Mirrors.followGazeTrans());

                _mirrorTrans.transform.SetParent(null);
                _mirrorTrans.transform.localScale = new Vector3(Main._trans_MirrorScaleX.Value, Main._trans_MirrorScaleY.Value, 0.05f);
                _mirrorTrans.transform.position = new Vector3(_mirrorTrans.transform.position.x, _mirrorTrans.transform.position.y + ((Main._trans_MirrorScaleY.Value - _oldMirrorScaleYTrans) / 2), _mirrorTrans.transform.position.z);
                _mirrorTrans.transform.position += _mirrorTrans.transform.forward * (Main._trans_MirrorDistance.Value - _oldMirrorDistanceTrans);

                _mirrorTrans.GetOrAddComponent<BoxCollider>().enabled = Main._trans_CanPickupMirror.Value;
                _mirrorTrans.GetOrAddComponent<CVRPickupObject>().enabled = Main._trans_CanPickupMirror.Value;      
                _mirrorTrans.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;

                if (Main._trans_MirrorState.Value == "MirrorCutout" || Main._trans_MirrorState.Value == "MirrorTransparent" || Main._trans_MirrorState.Value == "MirrorCutoutSolo" || Main._trans_MirrorState.Value == "MirrorTransparentSolo") Mirrors.SetAllMirrorsToIgnoreShader();
                for (int i = 0; i < _mirrorTrans.transform.childCount; i++)
                    _mirrorTrans.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirrorTrans.transform.Find(Main._trans_MirrorState.Value);
                childMirror.gameObject.active = true;
                if (Main._trans_MirrorState.Value == "MirrorTransparent" || Main._trans_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._trans_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 10;
                _mirrorTrans.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (Main._trans_AnchorToTracking.Value) _mirrorTrans.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                _mirrorTrans.transform.Find("Frame").gameObject.SetActive(_trans_CanPickupMirror.Value & pickupFrame.Value);
                if (Main._trans_MirrorState.Value == "MirrorCutoutSolo" || Main._trans_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, false));
                if (Main._trans_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, true));

            }
            _oldMirrorScaleYTrans = Main._trans_MirrorScaleY.Value;
            _oldMirrorDistanceTrans = Main._trans_MirrorDistance.Value;
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
                        OnPreferencesSaved();
                        HarmonyInstance.Patch(typeof(CVR_MenuManager).GetMethod(nameof(CVR_MenuManager.ToggleQuickMenu)), null, new HarmonyMethod(typeof(Main).GetMethod(nameof(QMtoggle), BindingFlags.NonPublic | BindingFlags.Static)));
                        try
                        {//<3 SDraw https://github.com/SDraw/ml_mods_cvr/blob/0ebafccb33d2d2cf8ac2c0c3133a5fd2ace00378/ml_fpt/Main.cs#L37
                            //Begin
                            HarmonyInstance.Patch(typeof(BodySystem).GetMethod(nameof(BodySystem.StartCalibration)), null,
                                new HarmonyLib.HarmonyMethod(typeof(Main).GetMethod(nameof(OnStartCalibration_Postfix), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)));
                            //End
                            HarmonyInstance.Patch(typeof(BodySystem).GetMethod(nameof(BodySystem.Calibrate)), null,
                                new HarmonyLib.HarmonyMethod(typeof(Main).GetMethod(nameof(OnCalibrateAvatar_Postfix), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)));
                            Mirrors._calInit = true;
                        }
                        catch (System.Exception ex) { Main.Logger.Error($"Error for calibration patches\n" + ex.ToString()); }

                        firstload = false;
                        //Logger.Msg("default" + buildIndex);
                        QM.CreateQuickMenuButton();
                        QM.ParseSettings();
                    }
                    if (_mirrorBase != null)
                    { try { UnityEngine.Object.Destroy(_mirrorBase); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); } _mirrorBase = null; }
                    if (_mirror45 != null)
                    { try { UnityEngine.Object.Destroy(_mirror45); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); } _mirror45 = null; }
                    if (_mirrorCeiling != null)
                    { try { UnityEngine.Object.Destroy(_mirrorCeiling); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); } _mirrorCeiling = null; }
                    if (_mirrorMicro != null)
                    { try { UnityEngine.Object.Destroy(_mirrorMicro); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); } _mirrorMicro = null; }
                    if (_mirrorTrans != null)
                    { try { UnityEngine.Object.Destroy(_mirrorTrans); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); } _mirrorTrans = null; }
                    QM.ParseSettings();
                    break;
            }
        }

        static void OnStartCalibration_Postfix() => Mirrors.OnCalibrationBegin();
        static void OnCalibrateAvatar_Postfix() => Mirrors.OnCalibrationEnd();

        private static void QMtoggle(bool __0)
        {
            MelonCoroutines.Start(DelayQM(__0));
        }


        public static IEnumerator DelayQM(bool value)
        {
            yield return new WaitForSeconds(.22f);
            QM.settings.SetActive(value);
        }


        public override void OnUpdate()
        {
            if (Main.MirrorKeybindEnabled.Value)
            {//God help you
                // Toggle portable mirror
                if ( Utils.GetKeyDown(KeyCode.Keypad1))
                {
                    Mirrors.ToggleMirror();
                }
                if ( Utils.GetKeyDown(KeyCode.Keypad2))
                {
                    Mirrors.ToggleMirror45();
                }
                if ( Utils.GetKeyDown(KeyCode.Keypad3))
                {
                    Mirrors.ToggleMirrorCeiling();
                }
                if ( Utils.GetKeyDown(KeyCode.KeypadMultiply))
                {
                    Mirrors.ToggleMirrorMicro();
                }
                if ( Utils.GetKeyDown(KeyCode.KeypadEnter))
                {
                    var en = !_base_CanPickupMirror.Value;
                    Main._base_CanPickupMirror.Value = en;
                    Main._45_CanPickupMirror.Value = en;
                    Main._ceil_CanPickupMirror.Value = en;
                    Main._micro_CanPickupMirror.Value = en;
                    OnPreferencesSaved();
                }
                if ( Utils.GetKeyDown(KeyCode.Keypad4))
                {
                    Main._base_MirrorDistance.Value -= Main._mirrorDistAdj;
                    OnPreferencesSaved();
                }
                if ( Utils.GetKeyDown(KeyCode.Keypad7))
                {
                    Main._base_MirrorDistance.Value += Main._mirrorDistAdj;
                    OnPreferencesSaved();
                }
                if ( Utils.GetKeyDown(KeyCode.Keypad8))
                {
                    Main._45_MirrorDistance.Value -= Main._mirrorDistAdj;
                    OnPreferencesSaved();
                }
                if ( Utils.GetKeyDown(KeyCode.KeypadDivide))
                {
                    Main._45_MirrorDistance.Value += Main._mirrorDistAdj;
                    OnPreferencesSaved();
                }
                if ( Utils.GetKeyDown(KeyCode.Keypad6))
                {
                    Main._ceil_MirrorDistance.Value -= Main._mirrorDistAdj;
                    OnPreferencesSaved();
                }
                if ( Utils.GetKeyDown(KeyCode.Keypad9))
                {
                    Main._ceil_MirrorDistance.Value += Main._mirrorDistAdj;
                    OnPreferencesSaved();
                }

                //All Bigger/Smaller
                if ( Utils.GetKeyDown(KeyCode.KeypadPlus))
                {
                    Main._base_MirrorScaleX.Value += .25f;
                    Main._base_MirrorScaleY.Value += .25f;
                    Main._45_MirrorScaleX.Value += .25f;
                    Main._45_MirrorScaleY.Value += .25f;
                    Main._ceil_MirrorScaleX.Value += .25f;
                    Main._ceil_MirrorScaleZ.Value += .25f;
                    Main._micro_MirrorScaleX.Value += .01f;
                    Main._micro_MirrorScaleY.Value += .01f;
                    OnPreferencesSaved();
                }
                if ( Utils.GetKeyDown(KeyCode.KeypadMinus))
                {

                    if (Main._base_MirrorScaleX.Value > .25 && Main._base_MirrorScaleY.Value > .25)
                    {
                        Main._base_MirrorScaleX.Value -= .25f;
                        Main._base_MirrorScaleY.Value -= .25f;
                    }
                    if (Main._45_MirrorScaleX.Value > .25 && Main._45_MirrorScaleY.Value > .25)
                    {
                        Main._45_MirrorScaleX.Value -= .25f;
                        Main._45_MirrorScaleY.Value -= .25f;
                    }
                    if (Main._ceil_MirrorScaleX.Value > .25 && Main._ceil_MirrorScaleZ.Value > .25)
                    {
                        Main._ceil_MirrorScaleX.Value -= .25f;
                        Main._ceil_MirrorScaleZ.Value -= .25f;
                    }

                    if (Main._micro_MirrorScaleX.Value > .02 && Main._micro_MirrorScaleY.Value > .02)
                    {
                        Main._micro_MirrorScaleX.Value -= .01f;
                        Main._micro_MirrorScaleY.Value -= .01f;
                    }
                    OnPreferencesSaved();
                }

                if ( Utils.GetKeyDown(KeyCode.KeypadPeriod))
                {

                    _base_PositionOnView.Value = !_base_PositionOnView.Value;
                    _micro_PositionOnView.Value = !_micro_PositionOnView.Value;
                    OnPreferencesSaved();
                }
            }
        }
        public static float _mirrorDistAdj;
        public static bool _mirrorDistHighPrec = false;
        public static bool _AllPickupable = false;


        public static GameObject _mirrorBase;
        public static float _oldMirrorDistance;
        public static float _oldMirrorScaleYBase;

        public static GameObject _mirror45;
        public static float _oldMirrorDistance45;
        public static float _oldMirrorScaleY45;

        public static GameObject _mirrorCeiling;
        public static float _oldMirrorDistanceCeiling;

        public static GameObject _mirrorMicro;
        public static float _oldMirrorDistanceMicro;
        public static float _oldMirrorScaleYMicro;

        public static GameObject _mirrorTrans;
        public static float _oldMirrorDistanceTrans;
        public static float _oldMirrorScaleYTrans;

        public static GameObject _mirrorCal;
    }
}


