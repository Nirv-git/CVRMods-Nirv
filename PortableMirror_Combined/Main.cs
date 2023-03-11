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
using ABI_RC.Core.Savior;

[assembly: MelonInfo(typeof(PortableMirror.Main), "PortableMirrorMod", PortableMirror.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace PortableMirror
{

    public class Main : MelonMod
    {
        public const string versionStr = "2.1.9";
        public static MelonLogger.Instance Logger;

        public static bool firstload = true;

        public static MelonPreferences_Entry<bool> Spacer1, Spacer2, Spacer3, Spacer4;
        public static MelonPreferences_Entry<bool> SpacerMisc, SpacerMisc2, SpacerMisc3;
        public static MelonPreferences_Entry<bool> SpacerMiscBase, SpacerMisc45, SpacerMiscCeil, SpacerMiscMicro, SpacerMiscTrans;

        public static MelonPreferences_Entry<bool> fixRenderOrder;
        public static MelonPreferences_Entry<bool> usePixelLights;
        public static MelonPreferences_Entry<bool> QMstartMax;
        public static MelonPreferences_Entry<bool> pickupFrame;
        public static MelonPreferences_Entry<int> QMposition;
        public static MelonPreferences_Entry<bool> QMsmaller;
        public static MelonPreferences_Entry<int> QMhighlightColor;
        public static MelonPreferences_Entry<bool> enableGaze;
        public static MelonPreferences_Entry<float> followGazeTime;

        public static MelonPreferences_Entry<bool> followGazeDeadBand_en;
        public static MelonPreferences_Entry<float> followGazeDeadBand;
        public static MelonPreferences_Entry<float> followGazeDeadBandBreakTime;

        public static MelonPreferences_Entry<float> followGazeDeadBandSettle;
        public static MelonPreferences_Entry<float> followGazeDeadBandSeconds;

        public static MelonPreferences_Entry<bool> customGrab_en;
        public static MelonPreferences_Entry<bool> customGrabLine;

        public static MelonPreferences_Entry<float> customGrabSpeed;

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
        //public static MelonPreferences_Entry<bool> amapi_ModsFolder;
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
        public static MelonPreferences_Entry<bool> _trans_MirrorDefState;
        public static MelonPreferences_Entry<bool> _trans_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _trans_enableTrans;
        public static MelonPreferences_Entry<bool> _trans_AnchorToTracking;
        public static MelonPreferences_Entry<bool> _trans_PositionOnView;
        public static MelonPreferences_Entry<bool> _trans_followGaze;

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

            Spacer1 = MelonPreferences.CreateEntry<bool>("PortableMirror", "Spacer1", false, "--These are global settings for all portable mirror types--");///
            fixRenderOrder = MelonPreferences.CreateEntry("PortableMirror", "fixRenderOrder", false, "Change render order on mirrors to fix overrendering --Don't use--", "", true);
            MirrorDistAdjAmmount = MelonPreferences.CreateEntry<float>("PortableMirror", "MirrorDistAdjAmmount", .05f, "High Precision Distance Adjustment Value");
            ColliderDepth = MelonPreferences.CreateEntry<float>("PortableMirror", "ColliderDepth", 0.001f, "Collider Depth");
            pickupFrame = MelonPreferences.CreateEntry<bool>("PortableMirror", "pickupFrame", false, "Show frame when mirror is pickupable");
            enableGaze = MelonPreferences.CreateEntry<bool>("PortableMirror", "enableGaze", false, "Enable 'Follow Gaze' (FG) by clicking Anchor to Tracking button twice");
            followGazeTime = MelonPreferences.CreateEntry<float>("PortableMirror", "followGazeTime", 0.5f, "FG Movement Speed");
            followGazeDeadBand_en = MelonPreferences.CreateEntry<bool>("PortableMirror", "followGazeDeadBand_en", true, "FG DeadBand - Enabled (Base Mirror Only)");
            followGazeDeadBand = MelonPreferences.CreateEntry<float>("PortableMirror", "followGazeDeadBand2", 60f, "FG DeadBand - Break Angle");
            followGazeDeadBandBreakTime = MelonPreferences.CreateEntry<float>("PortableMirror", "followGazeDeadBandBreakTime", 3f, "FG DeadBand - Seconds to wait after break (0 to disable)");
            followGazeDeadBandSettle = MelonPreferences.CreateEntry<float>("PortableMirror", "followGazeDeadBandSettle2", 3f, "FG DeadBand - Settle Angle");
            followGazeDeadBandSeconds = MelonPreferences.CreateEntry<float>("PortableMirror", "followGazeDeadBandSeconds", .5f, "FG DeadBand - Settle Seconds (0 to disable)");

            customGrab_en = MelonPreferences.CreateEntry<bool>("PortableMirror", "customGrab_en", false, "Use custom mirror pickup in VR (Base/Micro/Trans mirrors)");
            customGrabSpeed = MelonPreferences.CreateEntry<float>("PortableMirror", "grabTestSpeed", 5f, "Custom pickup push/pull speed");
            customGrabLine = MelonPreferences.CreateEntry<bool>("PortableMirror", "customGrabLine", true, "Custom pickup line");

            Spacer2 = MelonPreferences.CreateEntry<bool>("PortableMirror", "Spacer2", false, "--These options are on the QM also--");///
            usePixelLights = MelonPreferences.CreateEntry<bool>("PortableMirror", "usePixelLights", false, "Use PixelLights for mirrors");
            PickupToHand = MelonPreferences.CreateEntry<bool>("PortableMirror", "PickupToHand", false, "Pickups snap to hand - Global for all mirrors");
            TransMirrorTrans = MelonPreferences.CreateEntry<float>("PortableMirror", "TransMirrorTrans", .4f, "Transparent Mirror transparency - Higher is more transparent - Global for all mirrors");

            //MirrorsShowInCamera = MelonPreferences.CreateEntry<bool>("PortableMirror", "MirrorsShowInCamera", true, "Mirrors show in Cameras - Global for all mirrors --Don't use--");
            //Spacer3 = MelonPreferences.CreateEntry<bool>("PortableMirror", "Spacer3", false, "--PortableMirror Distance Disable--");///
            //MelonPreferences.CreateCategory("PortableMirrorDistDisable", "PortableMirror Distance Disable");
            distanceDisable = MelonPreferences.CreateEntry<bool>("PortableMirror", "distanceDisable", false, "Disable avatars > than a distance from showing in Mirrors");
            distanceValue = MelonPreferences.CreateEntry<float>("PortableMirror", "distanceValue", 3f, "Disable Distance in meters");
            distanceUpdateInit = MelonPreferences.CreateEntry<float>("PortableMirror", "distanceUpdateInit", .5f, "Disable Update interval");

            //MelonPreferences.CreateCategory("PortableMirrorMisc", "Base-PortableMirror Base");
            MelonPreferences.CreateCategory("PortableMirrorMisc", "PortableMirror Misc Settings");
            SpacerMisc = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "SpacerMisc", false, "--These are all the settings behind the individual mirror types--");///
            SpacerMisc2 = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "SpacerMisc2", false, "--Settings starting with a * are unique to that mirror--");///
            SpacerMisc3 = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "SpacerMisc3", false, "--Settings starting with a ^ are adjustable with the QuickMenu UI--");///

            SpacerMiscBase = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "SpacerMiscBase", false, "--Base Mirror--");
            _base_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Base-MirrorScaleX", 2f, "^Mirror Scale X");
            _base_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Base-MirrorScaleY", 3f, "^Mirror Scale Y");
            _base_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Base-MirrorDistance", 0f, "^Mirror Distance");
            _base_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorMisc", "Base-MirrorState", "MirrorFull", "^Mirror Type (MirrorFull or MirrorOpt)");
            _base_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Base-CanPickupMirror", false, "^Can Pickup Mirror");
            _base_PositionOnView = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Base-PositionOnView", false, "^Position mirror based on view angle");
            _base_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Base-AnchorToTracking", false, "^Mirror Follows You");
            _base_followGaze = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Base-followGaze", false, "^Follow Gaze Enabled");

            //MelonPreferences.CreateCategory("PortableMirror45", "45-PortableMirror 45 Degree");
            SpacerMisc45 = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "SpacerMisc45", false, "--45 Mirror--");
            _45_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "45-MirrorScaleX", 5f, "^Mirror Scale X");
            _45_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "45-MirrorScaleY", 4f, "^Mirror Scale Y");
            _45_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "45-MirrorDistance", 0f, "^Mirror Distance");
            _45_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorMisc", "45-MirrorState", "MirrorFull", "^Mirror Type (MirrorFull or MirrorOpt)");
            _45_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "45-CanPickupMirror", false, "^Can Pickup 45 Mirror");
            _45_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "45-AnchorToTracking", false, "^Mirror Follows You");
            _45_followGaze = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "45-followGaze", false, "^Follow Gaze Enabled");

            //MelonPreferences.CreateCategory("PortableMirrorMisc", "Ceil-PortableMirror Ceiling");
            SpacerMiscCeil = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "SpacerMiscCeil", false, "--Ceiling Mirror--");
            _ceil_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Ceil-MirrorScaleX", 5f, "^Mirror Scale X");
            _ceil_MirrorScaleZ = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Ceil-MirrorScaleZ", 5f, "^Mirror Scale Z");
            _ceil_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Ceil-MirrorDistance", 2, "^Mirror Distance");
            _ceil_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorMisc", "Ceil-MirrorState", "MirrorFull", "^Mirror Type (MirrorFull or MirrorOpt)");
            _ceil_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Ceil-CanPickupMirror", false, "^Can Pickup Ceiling Mirror");
            _ceil_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Ceil-AnchorToTracking", false, "^Mirror Follows You");

            //MelonPreferences.CreateCategory(""PortableMirrorMisc", "Micro-PortableMirror Micro");
            SpacerMiscMicro = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "SpacerMiscMicro", false, "--Micro Mirror--");
            _micro_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Micro-MirrorScaleX", .2f, "^Mirror Scale X");
            _micro_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Micro-MirrorScaleY", .3f, "^Mirror Scale Y");
            _micro_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Micro-MirrorDistance", 0f, "^Mirror Distance");
            _micro_GrabRange = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Micro-GrabRange", .1f, "* GrabRange");
            _micro_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorMisc", "Micro-MirrorState", "MirrorFull", "^Mirror Type (MirrorFull or MirrorOpt)");
            _micro_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Micro-CanPickupMirror", false, "^Can Pickup MirrorMicro");
            _micro_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Micro-PositionOnView", false, "^Position mirror based on view angle");
            _micro_PositionOnView = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Micro-AnchorToTracking", false, "^Mirror Follows You");
            _micro_followGaze = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Micro-followGaze", false, "^Follow Gaze Enabled");

            //MelonPreferences.CreateCategory("PortableMirrorMisc", "Trans-PortableMirror Transparent");
            SpacerMiscTrans = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "SpacerMiscTrans", false, "--Trans Mirror--");
            _trans_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Trans-MirrorScaleX", 5f, "^Mirror Scale X");
            _trans_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Trans-MirrorScaleY", 3f, "^Mirror Scale Y");
            _trans_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorMisc", "Trans-MirrorDistance", 0f, "^Mirror Distance");
            _trans_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorMisc", "Trans-MirrorState", "MirrorTransparent", "^Mirror Type - Resets to below setting on load");
            _trans_MirrorDefState = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Trans-MirrorDefState", true, "* Default type | True-Trans False-Cutout ");
            _trans_MirrorState.Value = _trans_MirrorDefState.Value ? "MirrorTransparent" : "MirrorCutout"; //Force to Transparent every load
            _trans_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Trans-CanPickupMirror", false, "^Can Pickup Mirror");
            _trans_PositionOnView = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Trans-PositionOnView", false, "^Position mirror based on view angle");
            _trans_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Trans-AnchorToTracking", false, "^Mirror Follows You");
            _trans_followGaze = MelonPreferences.CreateEntry<bool>("PortableMirrorMisc", "Trans-followGaze", false, "^Follow Gaze Enabled");  

            _oldMirrorScaleYBase = Main._base_MirrorScaleY.Value;
            _oldMirrorDistance = Main._base_MirrorDistance.Value;
            _oldMirrorScaleY45 = Main._45_MirrorScaleY.Value;
            _oldMirrorDistance45 = Main._45_MirrorDistance.Value;
            _oldMirrorDistanceCeiling = Main._ceil_MirrorDistance.Value;
            _oldMirrorScaleYMicro = Main._micro_MirrorScaleY.Value;
            _oldMirrorDistanceMicro = Main._micro_MirrorDistance.Value;
            _oldMirrorScaleYTrans = Main._trans_MirrorScaleY.Value;
            _oldMirrorDistanceTrans = Main._trans_MirrorDistance.Value;

            MelonCoroutines.Start(WaitForLocalPlayer());
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
                _mirrorBase.GetOrAddComponent<BoxCollider>().enabled = false;
                _mirrorBase.GetOrAddComponent<CVRPickupObject>().enabled = false;
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
                if (Main._base_AnchorToTracking.Value) _mirrorBase.transform.SetParent(GameObject.Find("_PLAYERLOCAL").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                _mirrorBase.transform.Find("Frame").gameObject.SetActive(_base_CanPickupMirror.Value & pickupFrame.Value);
                Mirrors.FixFrame(_mirrorBase, Main._base_MirrorScaleX.Value, Main._base_MirrorScaleY.Value);
                if (Main._base_MirrorState.Value == "MirrorCutoutSolo" || Main._base_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, false));
                if (Main._base_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, true));

                if (MetaPort.Instance.isUsingVr && Main.customGrab_en.Value)
                {
                    if (Main._base_CanPickupMirror.Value && !Mirrors._baseGrabActive) MelonCoroutines.Start(Mirrors.pickupBase());
                }
                else
                {
                    _mirrorBase.GetOrAddComponent<CVRPickupObject>().enabled = Main._base_CanPickupMirror.Value;
                    _mirrorBase.GetOrAddComponent<BoxCollider>().enabled = Main._base_CanPickupMirror.Value;
                }
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
                if (Main._45_AnchorToTracking.Value) _mirror45.transform.SetParent(GameObject.Find("_PLAYERLOCAL").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                _mirror45.transform.Find("Frame").gameObject.SetActive(_45_CanPickupMirror.Value & pickupFrame.Value);
                Mirrors.FixFrame(_mirror45, Main._45_MirrorScaleX.Value, Main._45_MirrorScaleY.Value);
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
                if (Main._ceil_AnchorToTracking.Value)  _mirrorCeiling.transform.SetParent(GameObject.Find("_PLAYERLOCAL").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                _mirrorCeiling.transform.Find("Frame").gameObject.SetActive(_ceil_CanPickupMirror.Value & pickupFrame.Value);
                Mirrors.FixFrame(_mirrorCeiling, Main._ceil_MirrorScaleX.Value, Main._ceil_MirrorScaleZ.Value);
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
                _mirrorMicro.GetOrAddComponent<BoxCollider>().enabled = false;
                _mirrorMicro.GetOrAddComponent<CVRPickupObject>().enabled = false;
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
                if (Main._micro_AnchorToTracking.Value) _mirrorMicro.transform.SetParent(GameObject.Find("_PLAYERLOCAL").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                if (Main._micro_MirrorState.Value == "MirrorCutoutSolo" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, false));
                if (Main._micro_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, true));


                if (MetaPort.Instance.isUsingVr && Main.customGrab_en.Value)
                {
                    if (Main._micro_CanPickupMirror.Value && !Mirrors._microGrabActive) MelonCoroutines.Start(Mirrors.pickupMicro());
                }
                else
                {
                    _mirrorMicro.GetOrAddComponent<CVRPickupObject>().enabled = Main._micro_CanPickupMirror.Value;
                    _mirrorMicro.GetOrAddComponent<BoxCollider>().enabled = Main._micro_CanPickupMirror.Value;
                }
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

                _mirrorTrans.GetOrAddComponent<BoxCollider>().enabled = false;
                _mirrorTrans.GetOrAddComponent<CVRPickupObject>().enabled = false;      
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
                if (Main._trans_AnchorToTracking.Value) _mirrorTrans.transform.SetParent(GameObject.Find("_PLAYERLOCAL").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(Mirrors.SetOrder(childMirror.gameObject));
                _mirrorTrans.transform.Find("Frame").gameObject.SetActive(_trans_CanPickupMirror.Value & pickupFrame.Value);
                Mirrors.FixFrame(_mirrorTrans, Main._trans_MirrorScaleX.Value, Main._trans_MirrorScaleY.Value);
                if (Main._trans_MirrorState.Value == "MirrorCutoutSolo" || Main._trans_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, false));
                if (Main._trans_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(Mirrors.FixMirrorLayer(childMirror, true));


                if (MetaPort.Instance.isUsingVr && Main.customGrab_en.Value)
                {

                    if (Main._trans_CanPickupMirror.Value && !Mirrors._transGrabActive) MelonCoroutines.Start(Mirrors.pickupTrans());
                }
                else
                {

                    _mirrorTrans.GetOrAddComponent<CVRPickupObject>().enabled = Main._trans_CanPickupMirror.Value;
                    _mirrorTrans.GetOrAddComponent<BoxCollider>().enabled = Main._trans_CanPickupMirror.Value;
                }
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

        IEnumerator WaitForLocalPlayer()
        {
            while (PlayerSetup.Instance == null)
                yield return null;
            CVRInputManager.Instance.gameObject.AddComponent<InputSVR>();
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


