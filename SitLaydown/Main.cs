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
using ABI_RC.Core.Savior;
using HarmonyLib;
using ABI_RC.Systems.InputManagement;
using ABI_RC.Core.Player;
using ABI_RC.Systems.GameEventSystem;
using ABI_RC.Systems.Movement;

[assembly: MelonInfo(typeof(SitLaydown.Main), "SitLaydown", SitLaydown.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace SitLaydown
{

    public class Main : MelonMod
    {
        public const string versionStr = "1.7.11";
        public static MelonLogger.Instance Logger;

        public static Main Instance;

        public static MelonPreferences_Entry<string> SittingAnim;
        public static MelonPreferences_Entry<bool> useNirvMiscPage;
        public static MelonPreferences_Entry<float> DistAdjAmmount;
        public static MelonPreferences_Entry<float> joyMoveMult, joyRotMult;
        public static MelonPreferences_Entry<bool> preventLeavingSeat;
        public static MelonPreferences_Entry<bool> autoDisableOffsetAdj;


        public static GameObject _baseObj;
        public static Transform PosOffset = null;
        public static Transform RotOffset = null;

        public static bool _DistHighPrec = false;
        public static float _DistAdj;

        public static bool inChair = false;
        public static float lastHUDnotif = 0;
        public static bool joyMoveActive = false;

        public static Vector3 spawnPos;
        public static bool distFlagActive = false;
        public static bool moveOffsets = false;

        public static Vector3 lastPos;
        public static Quaternion lastRot;
        public static Vector3 lastPosOffset;
        public static Quaternion lastRotOffset;

        public static float lastPosTime = 0;

        public static int SittingAnimIndex = 0;

        public override void OnApplicationStart()
        {
            Instance = this;
            Logger = new MelonLogger.Instance("SitLaydown");

            loadAssets();

            MelonPreferences.CreateCategory("SitLaydown", "SitLaydown");
            useNirvMiscPage = MelonPreferences.CreateEntry("SitLaydown", nameof(useNirvMiscPage), false, "BTKUI - Use 'NirvMisc' page instead of custom page. (Restart req)");
            SittingAnim = MelonPreferences.CreateEntry("SitLaydown", "SitLaydown", "SitCrossed", "Chair sitting animation");
            DistAdjAmmount = MelonPreferences.CreateEntry<float>("SitLaydown", "DistAdjAmmount", .01f, "High Precision Distance Adjustment");
            joyMoveMult = MelonPreferences.CreateEntry<float>("SitLaydown", "joyMoveMult", 5f, "Joystick movement multiplier (1-10)");
            joyRotMult = MelonPreferences.CreateEntry<float>("SitLaydown", "joyRotMult", 5f, "Joystick rotation multiplier (1-10)");
            preventLeavingSeat = MelonPreferences.CreateEntry<bool>("SitLaydown", "preventLeavingSeat", true, "Prevent leaving the seat unless you use the menu or respawn");
            autoDisableOffsetAdj = MelonPreferences.CreateEntry<bool>("SitLaydown", "autoDisableOffsetAdj", true, "Disables adjusting offsets 120 seconds after last adjustment");
            SittingAnim.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { if(inChair) MelonCoroutines.Start(ResetChair()); });  

            SittingAnimIndex = GetAnimIndex();
            OnPreferencesSaved();
            SetupEvents();
            BTKUI_Cust.SetupUI();
        }

        public override void OnPreferencesSaved()
        {
            _DistAdj = _DistHighPrec ? DistAdjAmmount.Value : .1f;
        }


        public static IEnumerator JoyMove()
        {
            //Logger.Msg("Joymove Started");

            distFlagActive = false;
            BTKUI_Cust.SetMovementFlag(false);
            Main.joyMoveActive = true;

            while (_baseObj != null && joyMoveActive)
            {
                if (Mathf.Abs(CVRInputManager.Instance.movementVector.x) > .05f || Mathf.Abs(CVRInputManager.Instance.movementVector.z) > .05f || Mathf.Abs(CVRInputManager.Instance.lookVector.x) > .05f)
                {
                    if (moveOffsets)
                    {
                        //var newPos = PosOffset.localPosition;
                        //newPos += new Vector3((CVRInputManager.Instance.movementVector.z * Time.deltaTime) * Mathf.Clamp(joyMoveMult.Value, 0f, 10f) / 10 / 4, 0f,
                        //    (CVRInputManager.Instance.movementVector.x * Time.deltaTime) * Mathf.Clamp(joyMoveMult.Value, 0f, 10f) / 10 / 4);

                        var newPos = PosOffset.position;
                        newPos += _baseObj.transform.forward * (CVRInputManager.Instance.movementVector.z * Time.deltaTime) * Mathf.Clamp(joyMoveMult.Value, 0f, 10f) / 10 / 4;
                        newPos += _baseObj.transform.right * (CVRInputManager.Instance.movementVector.x * Time.deltaTime) * Mathf.Clamp(joyMoveMult.Value, 0f, 10f) / 10 / 4;
                         
                        if (newPos.magnitude > 5f)
                        {
                            distFlagActive = true;
                            BTKUI_Cust.SetMovementFlag(true);
                        }
                        else if (distFlagActive)
                        {
                            distFlagActive = false;
                            BTKUI_Cust.SetMovementFlag(false);
                        }
                        if (newPos.magnitude < 5.25f)
                        {
                            PosOffset.position = newPos;
                        }

                        RotOffset.transform.RotateAround(RotOffset.transform.position, Vector3.up, CVRInputManager.Instance.lookVector.x * Time.deltaTime * 7.5f * Mathf.Clamp(joyRotMult.Value, 0f, 10f));
                    }
                    else
                    {
                        var newPos = _baseObj.transform.position;
                        newPos += _baseObj.transform.forward * (CVRInputManager.Instance.movementVector.z * Time.deltaTime) * Mathf.Clamp(joyMoveMult.Value, 0f, 10f) / 10;
                        newPos += _baseObj.transform.right * (CVRInputManager.Instance.movementVector.x * Time.deltaTime) * Mathf.Clamp(joyMoveMult.Value, 0f, 10f) / 10;

                        if (Vector3.Distance(newPos, spawnPos) > 5f)
                        {
                            distFlagActive = true;
                            BTKUI_Cust.SetMovementFlag(true);
                        }
                        else if (distFlagActive)
                        {
                            distFlagActive = false;
                            BTKUI_Cust.SetMovementFlag(false);
                        }
                        if (Vector3.Distance(newPos, spawnPos) < 5.25f)
                        {
                            _baseObj.transform.position = newPos;
                        }

                        _baseObj.transform.RotateAround(_baseObj.transform.position, Vector3.up, CVRInputManager.Instance.lookVector.x * Time.deltaTime * 15f * Mathf.Clamp(joyRotMult.Value, 0f, 10f));
                    }
                }
                yield return null;
            }
            BTKUI_Cust.SetMovementFlag(false);
        }

        public static IEnumerator ResetChair()
        {
            ToggleChair(false);
            yield return new WaitForSeconds(.1f);
            ToggleChair(true, true);
        }

        public static IEnumerator WatchChair()
        {
            while(inChair)
            {
                if (_baseObj?.Equals(null) ?? true)
                {
                    inChair = false;
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public static float lastMove;
        public static System.Object watchOff_Rout = null;

        public static IEnumerator WatchMoveOffsets()
        {
            lastMove = Time.time;
            Vector3 WatchOffsetPos = Vector3.zero;
            Quaternion WatchOffsetRot = Quaternion.identity;
            while (Main.moveOffsets && autoDisableOffsetAdj.Value)
            {
                if (PosOffset != null && RotOffset != null && (WatchOffsetPos != PosOffset.position || WatchOffsetRot != RotOffset.rotation))
                {
                    WatchOffsetPos = PosOffset.position;
                    WatchOffsetRot = RotOffset.rotation;
                    lastMove = Time.time;
                }
                if (lastMove + 60 < Time.time)
                { //After 60 seconds, disable
                    Main.moveOffsets = false;
                    BTKUI_Cust.GenerateButtons();
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public static void ToggleChair(bool enableChair, bool reseat = false)
        {
            //Logger.Msg(enableChair);
            if (!enableChair && _baseObj != null && inChair)
            {
                inChair = false;
                lastPos = _baseObj.transform.position;
                lastRot = _baseObj.transform.rotation;
                if (!(_baseObj.transform?.GetChild(0)?.GetChild(0)?.GetChild(0)?.GetChild(0).Equals(null) ?? true))
                {
                    //Logger.Msg("Saving Vectors");
                    lastPosOffset = _baseObj.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).localPosition; //ChairPrefab(Clone)/SittingPosition/VR Sitting Position/VR Sitting Rotation Offset/VR Sitting Position Offset
                    lastRotOffset = _baseObj.transform.GetChild(0).GetChild(0).GetChild(0).rotation; //ChairPrefab(Clone)/SittingPosition/VR Sitting Position/VR Sitting Rotation Offset/
                    lastPosTime = Time.time;
                    //Logger.Msg($"Pos Offsets x:{lastPosOffset.x} y:{lastPosOffset.y} z:{lastPosOffset.z}");
                    //Logger.Msg($"Rot Offsets x:{lastRotOffset.x} y:{lastRotOffset.y} z:{lastRotOffset.z}");
                }
                else
                    lastPosTime = 0f;

                UnityEngine.Object.Destroy(_baseObj);
                _baseObj = null;

                distFlagActive = false;
                BTKUI_Cust.SetMovementFlag(false);
            }
            if (enableChair)
            {
                GameObject baseObj = GameObject.Instantiate(chairPrefab);
                baseObj.name = "ChairPrefab(Mod)-" + MetaPort.Instance.ownerId;
                var playerPos = PlayerSetup.Instance.GetPlayerPosition();
                //var rotTo = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
                
                Vector3 lookDirection = Camera.main.transform.forward;
                Vector3 projectedLookDirection = Vector3.ProjectOnPlane(lookDirection, PlayerSetup.Instance.transform.up);
                Quaternion rotTo = Quaternion.LookRotation(projectedLookDirection, PlayerSetup.Instance.transform.up);

                //Logger.Msg($"playerPos x:{playerPos.x} y:{playerPos.y} z:{playerPos.z}");
                //Logger.Msg($"Original Player Rot {PlayerSetup.Instance.transform.rotation.y}");
                //Logger.Msg($"rotTo {rotTo.y}");

                var reseatValid = (reseat && lastPosTime > Time.time - 5f);
                if (reseatValid)
                {
                    //PlayerSetup.Instance.transform.rotation = lastRot;
                    baseObj.transform.position = lastPos;
                    baseObj.transform.rotation = lastRot;
                }
                else
                {
                    PlayerSetup.Instance.transform.rotation = rotTo;
                    baseObj.transform.position = playerPos;
                    baseObj.transform.rotation = rotTo;
                }
                spawnPos = baseObj.transform.position;

                //Logger.Msg($"After Player Rot {PlayerSetup.Instance.transform.rotation.y}");

                baseObj.GetComponent<CVRInteractable>().actions[0].operations[0].animationVal = GetChairAnim();
                
                MelonCoroutines.Start(TriggerChair(reseatValid));
                 _baseObj = baseObj;
            }
        }

        public static IEnumerator TriggerChair(bool reseat = false)
        {
            yield return new WaitForSeconds(.1f);
            _baseObj.GetComponent<CVRInteractable>().actions[0].operations[0].gameObjectVal.GetComponent<CVRSeat>().onExitSeat.AddListener(new UnityEngine.Events.UnityAction(() =>
                {
                    ToggleChair(false);
                    BTKUI_Cust.SetMovementFlag(false);
                    //Logger.Msg("Left chair, toggling off");
                }
                ));
            _baseObj.GetComponent<CVRInteractable>().CustomTrigger();

            inChair = true;
            yield return new WaitForSeconds(.15f);

            BetterBetterCharacterController.Instance.prone = false;
            BetterBetterCharacterController.Instance.crouching = false;

            PosOffset = _baseObj.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0); //ChairPrefab(Clone)/SittingPosition/VR Sitting Position/VR Sitting Rotation Offset/VR Sitting Position Offset
            RotOffset = _baseObj.transform.GetChild(0).GetChild(0).GetChild(0); //ChairPrefab(Clone)/SittingPosition/VR Sitting Position/VR Sitting Rotation Offset/
            //Logger.Msg($"Before adj Pos Offsets x:{PosOffset.localPosition.x} y:{PosOffset.localPosition.y} z:{PosOffset.localPosition.z}");
            //Logger.Msg($"Before adj Rot Offsets x:{RotOffset.rotation.x} y:{RotOffset.rotation.y} z:{RotOffset.rotation.z}");

            if (reseat)
            {
                //Logger.Msg("Restoring offsets");
                PosOffset.localPosition = lastPosOffset; //ChairPrefab(Clone)/SittingPosition/VR Sitting Position/VR Sitting Rotation Offset/VR Sitting Position Offset
                RotOffset.rotation = lastRotOffset; //ChairPrefab(Clone)/SittingPosition/VR Sitting Position/VR Sitting Rotation Offset/
                //Logger.Msg($"After adj Pos Offsets x:{PosOffset.localPosition.x} y:{PosOffset.localPosition.y} z:{PosOffset.localPosition.z}");
                //Logger.Msg($"After adj Rot Offsets x:{RotOffset.rotation.x} y:{RotOffset.rotation.y} z:{RotOffset.rotation.z}");
            } 
            else
            {
                lastPosOffset = PosOffset.localPosition;
                lastRotOffset = RotOffset.rotation;
            }

            if (Main.joyMoveActive) MelonCoroutines.Start(Main.JoyMove());

        }


        public static int GetAnimIndex()
        {
            switch (Main.SittingAnim.Value)
            {
                case "Laydown": return 0; break;
                case "SitIdle": return 1; break;
                case "SitCrossed": return 2; break;
                case "BasicSit": return 3; break;
                default: return 0; Main.Logger.Msg("Something Broke - Main.GetAnimIndex - Switch"); break;
            }
        }

        public static void SetAnimIndex(int index)
        {
            switch (index)
            {
                case 0: Main.SittingAnim.Value = "Laydown"; break;
                case 1: Main.SittingAnim.Value = "SitIdle"; break;
                case 2: Main.SittingAnim.Value = "SitCrossed"; break;
                case 3: Main.SittingAnim.Value = "BasicSit"; break;
                default: Main.SittingAnim.Value = "Laydown"; Main.Logger.Msg("Something Broke - Main.SetAnimIndex - Switch"); break;
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

        public static void SetupEvents()
        {

            CVRGameEventSystem.World.OnLoad.AddListener((message) =>
            {
                try
                {
                    if (Main.inChair)
                    {
                        inChair = false;
                        Main.ToggleChair(false);
                        //Main.Logger.Msg(ConsoleColor.Magenta, "Left chair due to OnLoad");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within CVRGameEventSystem.World.OnLoad!");
                    Logger.Error(e);
                }
            });

            CVRGameEventSystem.World.OnUnload.AddListener((message) =>
            {
                try
                {
                    if (Main.inChair)
                    {
                        inChair = false;
                        Main.ToggleChair(false);
                        //Main.Logger.Msg(ConsoleColor.Magenta, "Left chair due to OnUnload");
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within CVRGameEventSystem.World.OnUnload!");
                    Logger.Error(e);
                }
            });

            //CVRGameEventSystem.Instance.OnConnected.AddListener((message) =>
            //{
            //    try
            //    {
            //        if (Main.inChair)
            //        {
            //            inChair = false;
            //            Main.ToggleChair(false);
            //            Main.Logger.Msg(ConsoleColor.Magenta, "Left chair due to OnConnected");
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Logger.Error("An error occured within !");
            //        Logger.Error(e);
            //    }
            //});

            //CVRGameEventSystem.Instance.OnConnectionLost.AddListener((message) =>
            //{
            //    try
            //    {
            //        if (Main.inChair)
            //        {
            //            inChair = false;
            //            Main.ToggleChair(false);
            //            Main.Logger.Msg(ConsoleColor.Magenta, "Left chair due to OnConnectionLost");
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Logger.Error("An error occured within OnConnectionLost!");
            //        Logger.Error(e);
            //    }
            //});

            //CVRGameEventSystem.Instance.OnConnectionRecovered.AddListener((message) =>
            //{
            //    try
            //    {
            //        if (Main.inChair)
            //        {
            //            inChair = false;
            //            Main.ToggleChair(false);
            //            Main.Logger.Msg(ConsoleColor.Magenta, "Left chair due to OnReocvered");
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Logger.Error("An error occured within OnConnectionRecovered!");
            //        Logger.Error(e);
            //    }
            //});

            //CVRGameEventSystem.Instance.OnDisconnected.AddListener((message) =>
            //{
            //    try
            //    {
            //        if (Main.inChair)
            //        {
            //            inChair = false;
            //            Main.ToggleChair(false);
            //            Main.Logger.Msg(ConsoleColor.Magenta, "Left chair due to onDisconnect");
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Logger.Error("An error occured within OnDisconnected!");
            //        Logger.Error(e);
            //    }
            //});

        }

    }
}


