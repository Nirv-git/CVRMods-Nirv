﻿using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using ABI.CCK.Components;
using ABI_RC.Core.Player;
using ABI_RC.Core.InteractionSystem;
using HarmonyLib;
using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[assembly: MelonInfo(typeof(PortableMirror.Main), "PortableMirrorMod", PortableMirror.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace PortableMirror
{

    public class Main : MelonMod
    {
        public const string versionStr = "2.0.8";
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


        public static MelonPreferences_Entry<float> _base_MirrorScaleX;
        public static MelonPreferences_Entry<float> _base_MirrorScaleY;
        public static MelonPreferences_Entry<float> _base_MirrorDistance;
        public static MelonPreferences_Entry<string> _base_MirrorState;
        public static MelonPreferences_Entry<bool> _base_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _base_enableBase;
        public static MelonPreferences_Entry<bool> _base_PositionOnView;
        public static MelonPreferences_Entry<bool> _base_AnchorToTracking;
        public static MelonPreferences_Entry<string> _base_MirrorKeybind;

        public static MelonPreferences_Entry<bool> QuickMenuOptions;
        public static MelonPreferences_Entry<bool> OpenLastQMpage;
        public static MelonPreferences_Entry<float> TransMirrorTrans;
        public static MelonPreferences_Entry<bool> MirrorsShowInCamera;
        public static MelonPreferences_Entry<float> MirrorDistAdjAmmount;
        public static MelonPreferences_Entry<bool> ActionMenu;
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

        public static MelonPreferences_Entry<float> _ceil_MirrorScaleX;
        public static MelonPreferences_Entry<float> _ceil_MirrorScaleZ;
        public static MelonPreferences_Entry<float> _ceil_MirrorDistance;
        public static MelonPreferences_Entry<string> _ceil_MirrorState;
        public static MelonPreferences_Entry<bool> _ceil_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _ceil_enableCeiling;
        public static MelonPreferences_Entry<bool> _ceil_AnchorToTracking;

        public static MelonPreferences_Entry<float> _micro_MirrorScaleX;
        public static MelonPreferences_Entry<float> _micro_MirrorScaleY;
        public static MelonPreferences_Entry<float> _micro_GrabRange;
        public static MelonPreferences_Entry<string> _micro_MirrorState;
        public static MelonPreferences_Entry<bool> _micro_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _micro_enableMicro;
        public static MelonPreferences_Entry<bool> _micro_AnchorToTracking;
        public static MelonPreferences_Entry<bool> _micro_PositionOnView;

        public static MelonPreferences_Entry<float> _trans_MirrorScaleX;
        public static MelonPreferences_Entry<float> _trans_MirrorScaleY;
        public static MelonPreferences_Entry<float> _trans_MirrorDistance;
        public static MelonPreferences_Entry<string> _trans_MirrorState;
        public static MelonPreferences_Entry<bool> _trans_CanPickupMirror;
        public static MelonPreferences_Entry<bool> _trans_enableTrans;
        public static MelonPreferences_Entry<bool> _trans_AnchorToTracking;
        public static MelonPreferences_Entry<bool> _trans_PositionOnView;

        public static MelonPreferences_Entry<bool> _cal_enable;
        public static MelonPreferences_Entry<float> _cal_MirrorScale;
        public static MelonPreferences_Entry<float> _cal_MirrorDistanceScale;
        public static MelonPreferences_Entry<string> _cal_MirrorState;
        public static MelonPreferences_Entry<bool> _cal_AlwaysInFront;
        public static MelonPreferences_Entry<bool> _cal_DelayMirror;
        public static MelonPreferences_Entry<float> _cal_DelayMirrorTime;

        public static MelonPreferences_Entry<bool> distanceDisable;
        public static MelonPreferences_Entry<float> distanceValue;
        public static MelonPreferences_Entry<float> distanceUpdateInit;



        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("PortableMirrorMod");

            loadAssets();

            MelonPreferences.CreateCategory("PortableMirror", "PortableMirror");
            Spacer2 = MelonPreferences.CreateEntry<bool>("PortableMirror", "Spacer2", false, "-Past this are global settings for all portable mirror types-");
            QMstartMax = MelonPreferences.CreateEntry<bool>("PortableMirror", "QMstartMax", false, "QuickMenu Starts Maximized");
            QMposition = MelonPreferences.CreateEntry<int>("PortableMirror", "QMposition", 0, "QuickMenu Position (0=Right, 1=Top, 2=Left)");
            QMsmaller = MelonPreferences.CreateEntry<bool>("PortableMirror", "QMsmaller", false, "QuickMenu is smaller");
            QMhighlightColor = MelonPreferences.CreateEntry<int>("PortableMirror", "QMhighlightColor", 0, "Enabled color for QuickMenu items (0=Orange, 1=Yellow, 2=Pink)");
            pickupFrame = MelonPreferences.CreateEntry<bool>("PortableMirror", "pickupFrame", false, "Show frame when mirror is pickupable");
            MirrorKeybindEnabled = MelonPreferences.CreateEntry<bool>("PortableMirror", "MirrorKeybindEnabled", false, "Enabled Mirror Keybind");
            usePixelLights = MelonPreferences.CreateEntry<bool>("PortableMirror", "usePixelLights", false, "Use PixelLights for mirrors");
            PickupToHand = MelonPreferences.CreateEntry<bool>("PortableMirror", "PickupToHand", false, "Pickups snap to hand - Global for all mirrors");
            TransMirrorTrans = MelonPreferences.CreateEntry<float>("PortableMirror", "TransMirrorTrans", .4f, "Transparent Mirror transparency - Higher is more transparent - Global for all mirrors");
            fixRenderOrder = MelonPreferences.CreateEntry<bool>("PortableMirror", "fixRenderOrder", false, "Change render order on mirrors to fix overrendering --Don't use--");
            MirrorsShowInCamera = MelonPreferences.CreateEntry<bool>("PortableMirror", "MirrorsShowInCamera", true, "Mirrors show in Cameras - Global for all mirrors --Don't use--");
            MirrorDistAdjAmmount = MelonPreferences.CreateEntry<float>("PortableMirror", "MirrorDistAdjAmmount", .05f, "High Precision Distance Adjustment - Global for all mirrors");
            ColliderDepth = MelonPreferences.CreateEntry<float>("PortableMirror", "ColliderDepth", 0.01f, "Collider Depth - Global for all mirrors");
            

            MelonPreferences.CreateCategory("PortableMirrorBase", "PortableMirror Base");
            _base_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorBase", "MirrorScaleX", 2f, "Mirror Scale X");
            _base_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirrorBase", "MirrorScaleY", 3f, "Mirror Scale Y");
            _base_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorBase", "MirrorDistance", 0f, "Mirror Distance");
            _base_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorBase", "MirrorState", "MirrorFull", "Mirror Type (MirrorFull or MirrorOpt)");
            _base_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorBase", "CanPickupMirror", false, "Can Pickup Mirror");
            _base_PositionOnView = MelonPreferences.CreateEntry<bool>("PortableMirrorBase", "PositionOnView", false, "Position mirror based on view angle");
            _base_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorBase", "AnchorToTracking", false, "Mirror Follows You");

            MelonPreferences.CreateCategory("PortableMirror45", "PortableMirror 45 Degree");
            _45_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirror45", "MirrorScaleX", 5f, "Mirror Scale X");
            _45_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirror45", "MirrorScaleY", 4f, "Mirror Scale Y");
            _45_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirror45", "MirrorDistance", 0f, "Mirror Distance");
            _45_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirror45", "MirrorState", "MirrorFull", "Mirror Type (MirrorFull or MirrorOpt)");
            _45_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirror45", "CanPickupMirror", false, "Can Pickup 45 Mirror");
            _45_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirror45", "AnchorToTracking", false, "Mirror Follows You");

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
            _micro_GrabRange = MelonPreferences.CreateEntry<float>("PortableMirrorMicro", "GrabRange", .1f, "GrabRange");
            _micro_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorMicro", "MirrorState", "MirrorFull", "Mirror Type (MirrorFull or MirrorOpt)");
            _micro_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorMicro", "CanPickupMirror", false, "Can Pickup MirrorMicro");
            _micro_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorMicro", "PositionOnView", false, "Position mirror based on view angle");
            _micro_PositionOnView = MelonPreferences.CreateEntry<bool>("PortableMirrorMicro", "AnchorToTracking", false, "Mirror Follows You");

            MelonPreferences.CreateCategory("PortableMirrorTrans", "PortableMirror Transparent");
            _trans_MirrorScaleX = MelonPreferences.CreateEntry<float>("PortableMirrorTrans", "MirrorScaleX", 5f, "Mirror Scale X");
            _trans_MirrorScaleY = MelonPreferences.CreateEntry<float>("PortableMirrorTrans", "MirrorScaleY", 3f, "Mirror Scale Y");
            _trans_MirrorDistance = MelonPreferences.CreateEntry<float>("PortableMirrorTrans", "MirrorDistance", 0f, "Mirror Distance");
            _trans_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorTrans", "MirrorState", "MirrorTransparent", "Mirror Type - Resets to Transparent on load");
            _trans_MirrorState.Value = "MirrorTransparent"; //Force to Transparent every load
            _trans_CanPickupMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorTrans", "CanPickupMirror", false, "Can Pickup Mirror");
            _trans_PositionOnView = MelonPreferences.CreateEntry<bool>("PortableMirrorTrans", "PositionOnView", false, "Position mirror based on view angle");
            _trans_AnchorToTracking = MelonPreferences.CreateEntry<bool>("PortableMirrorTrans", "AnchorToTracking", false, "Mirror Follows You");

            MelonPreferences.CreateCategory("PortableMirrorCal", "PortableMirror Calibration");
            _cal_enable = MelonPreferences.CreateEntry<bool>("PortableMirrorCal", "MirrorEnable", true, "Enable Mirror when Calibrating");
            _cal_MirrorScale = MelonPreferences.CreateEntry<float>("PortableMirrorCal", "MirrorScale", 1f, "MirrorScale");
            _cal_MirrorDistanceScale = MelonPreferences.CreateEntry<float>("PortableMirrorCal", "MirrorDistanceScale", 1f, "MirrorDistanceScale");
            _cal_MirrorState = MelonPreferences.CreateEntry<string>("PortableMirrorCal", "MirrorState", "MirrorCutoutSolo", "Mirror Type");
            //_cal_AlwaysInFront = MelonPreferences.CreateEntry<bool>("PortableMirrorCal", "AlwaysInFront", false, "Mirror is always infront of where you are looking");
            _cal_DelayMirror = MelonPreferences.CreateEntry<bool>("PortableMirrorCal", "DelayMirror", false, "Delay Mirror Creation for x seconds");
            _cal_DelayMirrorTime = MelonPreferences.CreateEntry<float>("PortableMirrorCal", "DelayMirrorTime", 1f, "Delay Mirror Time");

            MelonPreferences.CreateCategory("PortableMirrorDistDisable", "PortableMirror Distance Disable");
            distanceDisable = MelonPreferences.CreateEntry<bool>("PortableMirrorDistDisable", "distanceDisable", false, "Disable avatars > than a distane from showing in Mirrors");
            distanceValue = MelonPreferences.CreateEntry<float>("PortableMirrorDistDisable", "distanceValue", 3f, "Distance in meteres");
            distanceUpdateInit = MelonPreferences.CreateEntry<float>("PortableMirrorDistDisable", "distanceUpdateInit", .5f, "Update interval");

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

        }
        public override void OnPreferencesSaved()
        {
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
                _mirrorBase.transform.SetParent(null);
                _mirrorBase.transform.localScale = new Vector3(Main._base_MirrorScaleX.Value, Main._base_MirrorScaleY.Value, 1f);
                _mirrorBase.transform.position = new Vector3(_mirrorBase.transform.position.x, _mirrorBase.transform.position.y + ((Main._base_MirrorScaleY.Value - _oldMirrorScaleYBase) / 2), _mirrorBase.transform.position.z  );
                _mirrorBase.transform.position += _mirrorBase.transform.forward * (Main._base_MirrorDistance.Value - _oldMirrorDistance);
                _mirrorBase.GetOrAddComponent<CVRPickupObject>().enabled = Main._base_CanPickupMirror.Value;
                _mirrorBase.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                if (Main._base_MirrorState.Value == "MirrorCutout" || Main._base_MirrorState.Value == "MirrorTransparent" || Main._base_MirrorState.Value == "MirrorCutoutSolo" || Main._base_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                if (Main._base_MirrorState.Value == "MirrorTransparent" || Main._base_MirrorState.Value == "MirrorTransparentSolo") _mirrorBase.transform.Find(Main._base_MirrorState.Value).GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                for (int i = 0; i < _mirrorBase.transform.childCount; i++)
                    _mirrorBase.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirrorBase.transform.Find(Main._base_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                _mirrorBase.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (Main._base_AnchorToTracking.Value) _mirrorBase.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(SetOrder(childMirror.gameObject));
                _mirrorBase.transform.Find("Frame").gameObject.SetActive(_base_CanPickupMirror.Value & pickupFrame.Value);
            }

            _oldMirrorScaleYBase = Main._base_MirrorScaleY.Value;
            _oldMirrorDistance = Main._base_MirrorDistance.Value;

            if (_mirror45 != null && Utils.GetPlayer() != null)
            {
                _mirror45.transform.SetParent(null);
                _mirror45.transform.localScale = new Vector3(Main._45_MirrorScaleX.Value, Main._45_MirrorScaleY.Value, 1f);
                _mirror45.transform.rotation = _mirror45.transform.rotation * Quaternion.AngleAxis(-45, Vector3.left);
                _mirror45.transform.position = new Vector3(_mirror45.transform.position.x, _mirror45.transform.position.y + ((Main._45_MirrorScaleY.Value - _oldMirrorScaleY45)/2.5f), _mirror45.transform.position.z  );
                _mirror45.transform.position += _mirror45.transform.forward * (Main._45_MirrorDistance.Value - _oldMirrorDistance45);
                _mirror45.transform.rotation = _mirror45.transform.rotation * Quaternion.AngleAxis(45, Vector3.left);

                _mirror45.GetOrAddComponent<CVRPickupObject>().enabled = Main._45_CanPickupMirror.Value;
                _mirror45.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;

                if (Main._45_MirrorState.Value == "MirrorCutout" || Main._45_MirrorState.Value == "MirrorTransparent" || Main._45_MirrorState.Value == "MirrorCutoutSolo" || Main._45_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                if (Main._45_MirrorState.Value == "MirrorTransparent" || Main._45_MirrorState.Value == "MirrorTransparentSolo") _mirror45.transform.Find(Main._45_MirrorState.Value).GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                for (int i = 0; i < _mirror45.transform.childCount; i++)
                    _mirror45.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirror45.transform.Find(Main._45_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                _mirror45.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (Main._45_AnchorToTracking.Value) _mirror45.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(SetOrder(childMirror.gameObject));
                _mirror45.transform.Find("Frame").gameObject.SetActive(_45_CanPickupMirror.Value & pickupFrame.Value);
            }
            _oldMirrorScaleY45 = Main._45_MirrorScaleY.Value;
            _oldMirrorDistance45 = Main._45_MirrorDistance.Value;


            if (_mirrorCeiling != null && Utils.GetPlayer() != null)
            {
                _mirrorCeiling.transform.SetParent(null);
                _mirrorCeiling.transform.localScale = new Vector3(Main._ceil_MirrorScaleX.Value, Main._ceil_MirrorScaleZ.Value, 1f);
                _mirrorCeiling.transform.position = new Vector3(_mirrorCeiling.transform.position.x, _mirrorCeiling.transform.position.y + (Main._ceil_MirrorDistance.Value - _oldMirrorDistanceCeiling), _mirrorCeiling.transform.position.z);

                _mirrorCeiling.GetOrAddComponent<CVRPickupObject>().enabled = Main._ceil_CanPickupMirror.Value;
                _mirrorCeiling.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;

                if (Main._ceil_MirrorState.Value == "MirrorCutout" || Main._ceil_MirrorState.Value == "MirrorTransparent" || Main._ceil_MirrorState.Value == "MirrorCutoutSolo" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                if (Main._ceil_MirrorState.Value == "MirrorTransparent" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo") _mirrorCeiling.transform.Find(Main._ceil_MirrorState.Value).GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                for (int i = 0; i < _mirrorCeiling.transform.childCount; i++)
                    _mirrorCeiling.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirrorCeiling.transform.Find(Main._ceil_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                _mirrorCeiling.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (Main._ceil_AnchorToTracking.Value)  _mirrorCeiling.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(SetOrder(childMirror.gameObject));
                _mirrorCeiling.transform.Find("Frame").gameObject.SetActive(_ceil_CanPickupMirror.Value & pickupFrame.Value);
            }
            _oldMirrorDistanceCeiling = Main._ceil_MirrorDistance.Value;


            if (_mirrorMicro != null && Utils.GetPlayer() != null)
            {
                _mirrorMicro.transform.SetParent(null);
                _mirrorMicro.transform.localScale = new Vector3(Main._micro_MirrorScaleX.Value, Main._micro_MirrorScaleY.Value, 1f);
                _mirrorMicro.transform.position = new Vector3(_mirrorMicro.transform.position.x, _mirrorMicro.transform.position.y + ((Main._micro_MirrorScaleY.Value - _oldMirrorScaleYMicro) / 2), _mirrorMicro.transform.position.z);

                _mirrorMicro.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = Main._micro_GrabRange.Value;
                _mirrorMicro.GetOrAddComponent<CVRPickupObject>().enabled = Main._micro_CanPickupMirror.Value;
                _mirrorMicro.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;

                if (Main._micro_MirrorState.Value == "MirrorCutout" || Main._micro_MirrorState.Value == "MirrorTransparent" || Main._micro_MirrorState.Value == "MirrorCutoutSolo" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                if (Main._micro_MirrorState.Value == "MirrorTransparent" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") _mirrorMicro.transform.Find(Main._micro_MirrorState.Value).GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                for (int i = 0; i < _mirrorMicro.transform.childCount; i++)
                    _mirrorMicro.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirrorMicro.transform.Find(Main._micro_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._micro_AnchorToTracking.Value) _mirrorMicro.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(SetOrder(childMirror.gameObject));
            }
            _oldMirrorScaleYMicro = Main._micro_MirrorScaleY.Value;

            if (_mirrorTrans != null && Utils.GetPlayer() != null)
            {
                _mirrorTrans.transform.SetParent(null);
                _mirrorTrans.transform.localScale = new Vector3(Main._trans_MirrorScaleX.Value, Main._trans_MirrorScaleY.Value, 1f);
                _mirrorTrans.transform.position = new Vector3(_mirrorTrans.transform.position.x, _mirrorTrans.transform.position.y + ((Main._trans_MirrorScaleY.Value - _oldMirrorScaleYTrans) / 2), _mirrorTrans.transform.position.z);
                _mirrorTrans.transform.position += _mirrorTrans.transform.forward * (Main._trans_MirrorDistance.Value - _oldMirrorDistanceTrans);

                _mirrorTrans.GetOrAddComponent<CVRPickupObject>().enabled = Main._trans_CanPickupMirror.Value;      
                _mirrorTrans.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;

                if (Main._trans_MirrorState.Value == "MirrorCutout" || Main._trans_MirrorState.Value == "MirrorTransparent" || Main._trans_MirrorState.Value == "MirrorCutoutSolo" || Main._trans_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                if (Main._trans_MirrorState.Value == "MirrorTransparent" || Main._trans_MirrorState.Value == "MirrorTransparentSolo") _mirrorTrans.transform.Find(Main._trans_MirrorState.Value).GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                for (int i = 0; i < _mirrorTrans.transform.childCount; i++)
                    _mirrorTrans.transform.GetChild(i).gameObject.active = false;
                var childMirror = _mirrorTrans.transform.Find(Main._trans_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 10;
                _mirrorTrans.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (Main._trans_AnchorToTracking.Value) _mirrorTrans.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value || usePixelLights.Value) MelonCoroutines.Start(SetOrder(childMirror.gameObject));
                _mirrorTrans.transform.Find("Frame").gameObject.SetActive(_trans_CanPickupMirror.Value & pickupFrame.Value);
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
                            HarmonyInstance.Patch(typeof(PlayerSetup).GetMethod(nameof(PlayerSetup.ClearAvatar)), null, new HarmonyLib.HarmonyMethod(typeof(Main).GetMethod(nameof(OnAvatarClear_Postfix), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)));
                            HarmonyInstance.Patch(typeof(PlayerSetup).GetMethod("SetupAvatarGeneral", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic), null, new HarmonyLib.HarmonyMethod(typeof(Main).GetMethod(nameof(OnSetupAvatarGeneral_Postfix), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)));
                            _calInit = true;
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
                    ToggleMirror();
                }
                if ( Utils.GetKeyDown(KeyCode.Keypad2))
                {
                    ToggleMirror45();
                }
                if ( Utils.GetKeyDown(KeyCode.Keypad3))
                {
                    ToggleMirrorCeiling();
                }
                if ( Utils.GetKeyDown(KeyCode.KeypadMultiply))
                {
                    ToggleMirrorMicro();
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


        public static void ForceMirrorLayer()
        {

            foreach (var mirror in UnityEngine.Object.FindObjectsOfType<CVRMirror>())
            { 
                try
                {
                    mirror.m_ReflectLayers = mirror.m_ReflectLayers.value & ~(1 << 4); //Force all mirrors to not reflect "Water" and set all mirrors to water                                                                                     
                    mirror.gameObject.layer = 4;
                }
                catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
            }
        }
        public static IEnumerator SetOrder(GameObject obj)
        {
            yield return new WaitForSeconds(1f);
            if (!obj?.Equals(null) ?? false)
            {
                if (fixRenderOrder.Value) obj.GetComponentInChildren<Renderer>().material.renderQueue = 5000;
                obj.GetComponentInChildren<CVRMirror>().m_DisablePixelLights = !usePixelLights.Value;
            }

        }

        private static void SetAllMirrorsToIgnoreShader()
        {
            foreach (var mirror in UnityEngine.Object.FindObjectsOfType<CVRMirror>())
            { // https://github.com/knah/VRCMods/blob/master/MirrorResolutionUnlimiter/UiExtensionsAddon.cs
                try
                {
                    //Logger.Msg($"-----");
                    //Logger.Msg($"{vrcMirrorReflection.gameObject.name}");
                    GameObject othermirror = mirror?.gameObject?.transform?.parent?.gameObject; // Question marks are always the answer
                    //Logger.Msg($"othermirror is null:{othermirror is null}, !=base:{othermirror != _mirrorBase}, !=45:{othermirror != _mirror45}, !=Micro:{othermirror != _mirrorCeiling}, !=trans:{othermirror != _mirrorTrans}");
                    if (othermirror is null || (othermirror != _mirrorBase && othermirror != _mirror45 && othermirror != _mirrorCeiling && othermirror != _mirrorMicro && othermirror != _mirrorTrans))
                    {
                        //Logger.Msg($"setting layers");
                        mirror.m_ReflectLayers = mirror.m_ReflectLayers.value & ~reserved4; //Force all mirrors to not reflect "Mirror/TransparentBackground" - Set all mirrors to exclude reserved4                                                                                             
                    }
                }
                catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
            }
        }

        public static void ToggleMirror()
        {
            if (_mirrorBase != null)
            {
                try{ UnityEngine.Object.Destroy(_mirrorBase); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                _mirrorBase = null;
            }
            else
            {
                if (Main._base_MirrorState.Value == "MirrorCutout" || Main._base_MirrorState.Value == "MirrorTransparent" || Main._base_MirrorState.Value == "MirrorCutoutSolo" || Main._base_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();  
                GameObject player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;
                Vector3 pos = player.transform.position;

                pos.y += .5f;
                pos.y += (Main._base_MirrorScaleY.Value - 1)  / 2;

                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.transform.localScale = new Vector3(Main._base_MirrorScaleX.Value, Main._base_MirrorScaleY.Value, 1f);
                mirror.name = "PortableMirror";

                if (Main._base_PositionOnView.Value)
                {
                    mirror.transform.position = cam.transform.position + cam.transform.forward + (cam.transform.forward * Main._base_MirrorDistance.Value);
                    mirror.transform.rotation = cam.transform.rotation;
                }
                else
                {
                    mirror.transform.position = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z); //Set to player height instead of centered on camera
                    mirror.transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical
                    mirror.transform.position = mirror.transform.position + mirror.transform.forward + (mirror.transform.forward * Main._base_MirrorDistance.Value); //Move on distance
                }

                var childMirror = mirror.transform.Find(Main._base_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8; //Default prefab 4:Water - 8:Playerlocal 
                if (Main._base_MirrorState.Value == "MirrorTransparent" || Main._base_MirrorState.Value == "MirrorTransparentSolo") childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._base_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(_base_CanPickupMirror.Value & pickupFrame.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._base_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value) MelonCoroutines.Start(SetOrder(mirror));
                _mirrorBase = mirror;
            }
        }

        public static void ToggleMirror45()
        {
            if (_mirror45 != null)
            {
                try{ UnityEngine.Object.Destroy(_mirror45); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                _mirror45 = null;
            }
            else
            {
                if (Main._45_MirrorState.Value == "MirrorCutout" || Main._45_MirrorState.Value == "MirrorTransparent" || Main._45_MirrorState.Value == "MirrorCutoutSolo" || Main._45_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;
                Vector3 pos = player.transform.position;
                pos.y += .5f;
                pos.y += (Main._45_MirrorScaleY.Value - 1) / 2;

                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.transform.localScale = new Vector3(Main._45_MirrorScaleX.Value, Main._45_MirrorScaleY.Value, 1f);
                mirror.name = "PortableMirror45";

                mirror.transform.position = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z); //Set to player height instead of centered on camera
                mirror.transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z); //Make vertical
                mirror.transform.position = mirror.transform.position + mirror.transform.forward + (mirror.transform.forward * Main._45_MirrorDistance.Value); //Move on distance
                mirror.transform.rotation = mirror.transform.rotation * Quaternion.AngleAxis(45, Vector3.left);  // Sets the transform's current rotation to a new rotation that rotates 30 degrees around the y-axis(Vector3.up)

                var childMirror = mirror.transform.Find(Main._45_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._45_MirrorState.Value == "MirrorTransparent" || Main._45_MirrorState.Value == "MirrorTransparentSolo") childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._45_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(_45_CanPickupMirror.Value & pickupFrame.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._45_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value) MelonCoroutines.Start(SetOrder(mirror));

                _mirror45 = mirror;
            }
        }

        public static void ToggleMirrorCeiling()
        {
            
            if (_mirrorCeiling != null)
            {
                try { UnityEngine.Object.Destroy(_mirrorCeiling); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                _mirrorCeiling = null;
            }
            else
            {
                if (Main._ceil_MirrorState.Value == "MirrorCutout" || Main._ceil_MirrorState.Value == "MirrorTransparent" || Main._ceil_MirrorState.Value == "MirrorCutoutSolo" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;

                Vector3 pos = cam.transform.position + (player.transform.up); // Bases mirror position off of hip, to allow for play space moving 
                //Logger.Msg($"x:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.x}, y:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.y}, z:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.z}");
                pos.y += Main._ceil_MirrorDistance.Value;
                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.transform.position = pos;
                mirror.transform.rotation = Quaternion.Euler(-90f, cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z); 
                //mirror.transform.rotation = Quaternion.AngleAxis(90, Vector3.left);  // Sets the transform's current rotation to a new rotation that rotates 90 degrees around the y-axis(Vector3.up)
                mirror.transform.localScale = new Vector3(Main._ceil_MirrorScaleX.Value, Main._ceil_MirrorScaleZ.Value, 1f);
                mirror.name = "PortableMirrorCeiling";            

                var childMirror = mirror.transform.Find(Main._ceil_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._ceil_MirrorState.Value == "MirrorTransparent" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo") childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value); 
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._ceil_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(_ceil_CanPickupMirror.Value & pickupFrame.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._ceil_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value) MelonCoroutines.Start(SetOrder(mirror));

                _mirrorCeiling = mirror;
            }
        }

        public static void ToggleMirrorMicro()
        {
            if (_mirrorMicro != null)
            {
                try{ UnityEngine.Object.Destroy(_mirrorMicro); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                _mirrorMicro = null;
            }
            else
            {
                if (Main._micro_MirrorState.Value == "MirrorCutout" || Main._micro_MirrorState.Value == "MirrorTransparent" || Main._micro_MirrorState.Value == "MirrorCutoutSolo" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;
                Vector3 pos = cam.transform.position;
                pos.y -= Main._micro_MirrorScaleY.Value / 4;///This will need turning

                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.transform.localScale = new Vector3(Main._micro_MirrorScaleX.Value, Main._micro_MirrorScaleY.Value, 1f);
                mirror.name = "PortableMirrorMicro";

                if (Main._micro_PositionOnView.Value)
                {
                    mirror.transform.position = cam.transform.position + (cam.transform.forward * Main._micro_MirrorScaleY.Value);
                    mirror.transform.rotation = cam.transform.rotation;
                }
                else
                {
                    mirror.transform.position = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z); //Set to player height instead of centered on camera
                    mirror.transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical
                    mirror.transform.position = mirror.transform.position + (mirror.transform.forward * Main._micro_MirrorScaleY.Value); //Move on distance
                }

                var childMirror = mirror.transform.Find(Main._micro_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._micro_MirrorState.Value == "MirrorTransparent" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = Main._micro_GrabRange.Value;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._micro_CanPickupMirror.Value;
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                if (!Main._micro_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value) MelonCoroutines.Start(SetOrder(mirror));

                _mirrorMicro = mirror;
            }
        }

        public static void ToggleMirrorTrans()
        {
            if (_mirrorTrans != null)
            {
                try { UnityEngine.Object.Destroy(_mirrorTrans); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                _mirrorTrans = null;
            }
            else
            {
                if (Main._trans_MirrorState.Value == "MirrorCutout" || Main._trans_MirrorState.Value == "MirrorTransparent" || Main._trans_MirrorState.Value == "MirrorCutoutSolo" || Main._trans_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer();
                var cam = Camera.main.gameObject;
                Vector3 pos = player.transform.position;
                pos.y += .5f;
                pos.y += (Main._trans_MirrorScaleY.Value - 1) / 2;

                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.transform.localScale = new Vector3(Main._trans_MirrorScaleX.Value, Main._trans_MirrorScaleY.Value, 1f);
                mirror.name = "PortableMirrorTrans";

                if (Main._trans_PositionOnView.Value)
                {
                    mirror.transform.position = cam.transform.position + cam.transform.forward + (cam.transform.forward * Main._trans_MirrorDistance.Value);
                    mirror.transform.rotation = cam.transform.rotation;
                }
                else
                {
                    mirror.transform.position = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z); //Set to player height instead of centered on camera
                    mirror.transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical
                    mirror.transform.position = mirror.transform.position + mirror.transform.forward + (mirror.transform.forward * Main._trans_MirrorDistance.Value); //Move on distance
                }

                var childMirror = mirror.transform.Find(Main._trans_MirrorState.Value);
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 10;
                if (Main._trans_MirrorState.Value == "MirrorTransparent" || Main._trans_MirrorState.Value == "MirrorTransparentSolo") childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._trans_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(_trans_CanPickupMirror.Value & pickupFrame.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._trans_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (fixRenderOrder.Value) MelonCoroutines.Start(SetOrder(mirror));

                _mirrorTrans = mirror;
            }
        }

        public static void ToggleMirrorCal(bool state)
        {
            //Logger.Msg("ToggleMirrorCal");

            if (_mirrorCal != null && !state)
            {
                //.Msg("STate 1");
                ResetControllerLayer();
                try { UnityEngine.Object.Destroy(_mirrorCal); } catch (System.Exception ex) { Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                _mirrorCal = null;
            }
            else if (_mirrorCal == null && state)
            {
                //Logger.Msg("STate 2");
                if (Main._cal_MirrorState.Value == "MirrorCutout" || Main._cal_MirrorState.Value == "MirrorTransparent" || Main._cal_MirrorState.Value == "MirrorCutoutSolo" || Main._cal_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer();
                //var avatar = player.transform.Find("[PlayerAvatar]").GetChild(0);
                var cam = Camera.main.gameObject;
                Vector3 pos = player.transform.position;

                float mirrorHeight;
                if (_calHeight == 0f)
                    mirrorHeight = 4f;
                else
                    mirrorHeight = _calHeight * 1.5f;

                pos.y += .5f;
                pos.y += (mirrorHeight - 1) / 2;
                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.transform.localScale = new Vector3(mirrorHeight * 1.5f * .666f * Main._cal_MirrorScale.Value, mirrorHeight * 1.5f * Main._cal_MirrorScale.Value, 1f);
                mirror.name = "PortableMirrorCal";

                mirror.transform.position = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z); //Set to player height instead of centered on camera
                mirror.transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical
                mirror.transform.position += (mirror.transform.forward * mirrorHeight / 3f * Main._cal_MirrorDistanceScale.Value);

                var childMirror = mirror.transform.Find(Main._cal_MirrorState.Value);
                SetControllerLayer(10); //to 10 PlayerNetwork
                childMirror.gameObject.active = true;
                childMirror.gameObject.layer = 4; // Main.MirrorsShowInCamera.Value ? 4 : 10;
                if (Main._cal_MirrorState.Value == "MirrorTransparent" || Main._cal_MirrorState.Value == "MirrorTransparentSolo") childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = false;
                if (fixRenderOrder.Value) MelonCoroutines.Start(SetOrder(mirror));

                _mirrorCal = mirror;

                //if (Main._cal_AlwaysInFront.Value) MelonCoroutines.Start(calMirrorTracking());
                //else 
                //mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                mirror.transform.SetParent(null);
            }
        }

        static void OnAvatarClear_Postfix() => OnCalibrationBegin();
        static void OnSetupAvatarGeneral_Postfix() => OnCalibrationEnd();


        public static void OnCalibrationBegin()
        {
            //Logger.Msg("OnAvatarClear_Postfix");
            try
            {
                //Logger.Msg("CAL STARTED");
                if (_calInit && Main._cal_enable.Value)
                {
                    if (_cal_DelayMirror.Value)
                        calDelayRoutine = MelonCoroutines.Start(DelayCalMirror());
                    else
                        waitForMeasureRoutine = MelonCoroutines.Start(WaitForMeasure());
                }
                //Logger.Msg("CAL START Complete");
            }
            catch (System.Exception ex) { Logger.Error("Error in OnCalibrationBegin:\n" + ex.ToString()); }
        }


        public static void OnCalibrationEnd()
        {
            //Logger.Msg("OnSetupAvatarGeneral_Postfix");
            try
            {
                //Logger.Msg("CAL END");
                if (calDelayRoutine != null) MelonCoroutines.Stop(calDelayRoutine);
                //Logger.Msg("1");
                if (waitForMeasureRoutine != null) MelonCoroutines.Stop(waitForMeasureRoutine);
                //Logger.Msg("2");
                ToggleMirrorCal(false);
                //Logger.Msg("CAL END Complete");
            }
            catch (System.Exception ex) { Logger.Error("Error in OnCalibrationEnd:\n" + ex.ToString()); }
        }

        private static void SetControllerLayer(int layer)
        {
            calObjects.Clear();
            var cons = new string[] {
                "_PLAYERLOCAL/[CameraRigVR]/Controller (left)",
                "_PLAYERLOCAL/[CameraRigVR]/Controller (right)",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker01",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker02",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker03",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker04",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker05",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker06",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker07",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker08",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker09",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker10",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker11",
                "_PLAYERLOCAL/[CameraRigVR]/VRTRacker12"};
            foreach (var c in cons)
            {
                GameObject Con = GameObject.Find(c);
                if (Con?.Equals(null) ?? true)
                    break;
                foreach (var mesh in Con.GetComponentsInChildren<MeshRenderer>(true))
                {
                    if (!Utils.GetPath(mesh.transform).Contains("ControllerUI"))
                    {
                        //Logger.Msg("Setting layer to " + layer + " for " + Utils.GetPath(mesh.transform));
                        calObjects.Add(mesh.gameObject, mesh.gameObject.layer);
                        mesh.gameObject.layer = layer;
                    }
                }
            }
        }

        private static void ResetControllerLayer()
        {
            foreach (var c in calObjects)
            {
                //Logger.Msg("Resetting layer to " + c.Value + " for " + Utils.GetPath(c.Key.transform));
                c.Key.layer = c.Value;
            }
        }

        public static IEnumerator WaitForMeasure()
        {
            //Logger.Msg("WaitForMeasure");
            _calHeight = 0;
            var player = Utils.GetPlayer();
            var abortTime = Time.time + 10f;
            while (Time.time < abortTime && PlayerSetup.Instance._animator is null)
            {
                //Logger.Msg("a n");
                yield return null;
            }
            var anim = PlayerSetup.Instance._animator;
            if (anim is null || !anim.isHuman)
            {
                //Logger.Msg("Null || Not human");
                ToggleMirrorCal(true);
                yield break;
            }
            try
            {
                var height = Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.LeftFoot).position, anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position) +
                                              Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position, anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position) +
                                              Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position, anim.GetBoneTransform(HumanBodyBones.Hips).position) +
                                              Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.Hips).position, anim.GetBoneTransform(HumanBodyBones.Spine).position) +
                                              Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.Spine).position, anim.GetBoneTransform(HumanBodyBones.Chest).position) +
                                              Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.Chest).position, anim.GetBoneTransform(HumanBodyBones.Neck).position) +
                                              Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.Neck).position, anim.GetBoneTransform(HumanBodyBones.Head).position);

                //Logger.Msg("Height is " + height);
                _calHeight = height;
            }
            catch (Exception ex)
            {
                Logger.Error("Error Measuring Height\n" + ex.ToString());
            }

            ToggleMirrorCal(true);
            //Logger.Msg("WaitForMeasure-Done");

        }

        public static IEnumerator DelayCalMirror()
        {
            //Logger.Msg("DelayCalMirror");
            yield return new WaitForSeconds(_cal_DelayMirrorTime.Value);
            waitForMeasureRoutine = MelonCoroutines.Start(WaitForMeasure());
            //Logger.Msg("DelayCalMirror-Done");
        }

        //No head follow yet, so always being in front would never be right
        //public static IEnumerator calMirrorTracking()
        //{
        //    var cam = Camera.main.gameObject;
        //    var player = Utils.GetPlayer();
        //    //var playerAv = player.transform.Find("[PlayerAvatar]");
        //    //var avClone = playerAv.GetChild(0);

        //    while (Main._cal_AlwaysInFront.Value)
        //    {
        //        if (_mirrorCal?.Equals(null) ?? true) yield break;

        //        Vector3 pos = player.transform.position;

        //        float mirrorHeight;
        //        if (_calHeight == 0f)
        //            mirrorHeight = 4f;
        //        else
        //            mirrorHeight = _calHeight * 1.5f;

        //        pos.y += .5f;
        //        pos.y += (mirrorHeight - 1) / 2;
                
        //        Vector3 toPos = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z); //Set to player height instead of centered on camera
        //        toPos += (cam.transform.forward * mirrorHeight / 3 * Main._cal_MirrorDistanceScale.Value);

        //       // _mirrorCal.transform.LookAt(cam.transform);  
        //       // _mirrorCal.transform.rotation *= Quaternion.AngleAxis(180, Vector3.up);
        //        _mirrorCal.transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical

        //        var step = .6f * Time.deltaTime; // calculate distance to move
        //        _mirrorCal.transform.position = Vector3.MoveTowards(_mirrorCal.transform.position, toPos, step);

        //        yield return null;
        //    }
        //}


        private void loadAssets()
        {//https://github.com/ddakebono/BTKSASelfPortrait/blob/master/BTKSASelfPortrait.cs
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PortableMirrorMod.cvrmirrorprefab"))
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
                mirrorPrefab = assetBundle.LoadAsset<GameObject>("MirrorPrefabCVR");
                mirrorPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                mirrorSettingsPrefab = assetBundle.LoadAsset<GameObject>("PortableMirrorSettings");
                mirrorSettingsPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
            else Logger.Error("Bundle was null");
        }

        public static Dictionary<string, Transform> ButtonList = new Dictionary<string, Transform>();

        ///OLD VRC Values
        //PlayerLayer = 1 << 9; // https://github.com/knah/VRCMods/blob/master/MirrorResolutionUnlimiter/UiExtensionsAddon.cs
        //PlayerLocalLayer = 1 << 10; //Mainly just here as a refernce now
        //UiLayer = 1 << 5;
        //UiMenuLayer = 1 << 12;
        //MirrorReflectionLayer = 1 << 18;
        //public static int playerLayer = 1 << 9;
        //public static int reserved2 = 1 << 19;
        public static int reserved4 = 1 << 15;
        //int optMirrorMask = PlayerLayer | MirrorReflectionLayer;
        //int fullMirrorMask = -1 & ~UiLayer & ~UiMenuLayer & ~PlayerLocalLayer & ~reserved2;

        public static AssetBundle assetBundle;
        public static GameObject mirrorPrefab, mirrorSettingsPrefab;
        public static object calDelayRoutine, waitForMeasureRoutine;

        public static GameObject _mirrorBase;

        public static float _oldMirrorDistance;
        public static float _oldMirrorScaleYBase;
        public static KeyCode _mirrorKeybindBase;
        public static int _qmOptionsLastPage = 1;
        public static float _mirrorDistAdj;
        public static bool _mirrorDistHighPrec = false;
        public static bool _AllPickupable = false;
        public static bool _calInit = false;
        public static float _calHeight;
        
        public static GameObject _mirror45;
        public static float _oldMirrorDistance45;
        public static float _oldMirrorScaleY45;

        public static GameObject _mirrorCeiling;
        public static float _oldMirrorDistanceCeiling;

        public static GameObject _mirrorMicro;
        public static float _oldMirrorScaleYMicro;

        public static GameObject _mirrorTrans;
        public static float _oldMirrorDistanceTrans;
        public static float _oldMirrorScaleYTrans;

        public static GameObject _mirrorCal;
        public static Dictionary<GameObject, int> calObjects = new Dictionary<GameObject, int>();
    }
}


