using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core;
using ABI.CCK.Components;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.InputManagement;
using System.Diagnostics;


namespace PortableMirror
{

    public class Mirrors
    {
        //PlayerLocal = 1 << 8; 
        //PlayerClone  = 1 << 9; 
        //PlayerNetwork  = 1 << 10; 
        //MirrorReflection  = 1 << 11; 
        //UiLayer = 1 << 5;
        //public static int water = 1 << 4; // Layer Mirror is on
        //public static int notwater = -1 & water;
        //public static int reserved3 = 1 << 14;
        //int optMirrorMask = PlayerNetwork | MirrorReflectionLayer;   //Double check this
        //int fullMirrorMask = -1 & ~UiLayer & ~PlayerLocal & ~reserved3; //Double check this

        public static AssetBundle assetBundle;
        public static GameObject mirrorPrefab, mirrorSettingsPrefab, pickupLine;

        //public static object calDelayRoutine, waitForMeasureRoutine, calDelayRoutineOff;
        //public static bool _calInit = false;
        //public static float _calHeight;
        //public static Dictionary<GameObject, int> calObjects = new Dictionary<GameObject, int>();

        public static bool _baseFollowGazeActive, _45FollowGazeActive, _transFollowGazeActive, _microFollowGazeActive;
        //public static bool _GrabActive, _baseGrabActive, _microGrabActive, _transGrabActive;
        //public static bool _globalHeld = false;


        public static void loadAssets()
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
            else Main.Logger.Error("Bundle was null");
        }

        //public static void ForceMirrorLayer()
        //{
        //    foreach (var mirror in UnityEngine.Object.FindObjectsOfType<CVRMirror>())
        //    {
        //        try
        //        {
        //            mirror.m_ReflectLayers = mirror.m_ReflectLayers.value & ~(1 << 4); //Force all mirrors to not reflect "Water" and set all mirrors to water                                                                                     
        //            mirror.gameObject.layer = 4;
        //        }
        //        catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
        //    }
        //}
        public static IEnumerator SetOrder(GameObject obj)
        {
            yield return new WaitForSeconds(1f);
            if (!obj?.Equals(null) ?? false)
            {
                //if (fixRenderOrder.Value) obj.GetComponentInChildren<Renderer>().material.renderQueue = 5000;
                obj.GetComponentInChildren<CVRMirror>().m_DisablePixelLights = !Main.usePixelLights.Value;
            }

        }

        //public static void SetAllMirrorsToIgnoreShader()
        //{
        //    foreach (var mirror in UnityEngine.Object.FindObjectsOfType<CVRMirror>())
        //    { // https://github.com/knah/VRCMods/blob/master/MirrorResolutionUnlimiter/UiExtensionsAddon.cs
        //        try
        //        {
        //            //Main.Logger.Msg($"-----");
        //            //Main.Logger.Msg($"{mirror.gameObject.name}");
        //            GameObject othermirror = mirror?.gameObject?.transform?.parent?.gameObject; // Question marks are always the answer
        //            //Main.Logger.Msg($"othermirror is null:{othermirror is null}, !=base:{othermirror != Main._mirrorBase}, !=45:{othermirror != Main._mirror45}," +
        //            //    $" !=Micro:{othermirror != Main._mirrorCeiling}, !=trans:{othermirror != Main._mirrorTrans}");
        //            //Main.Logger.Msg($"is child - base:{othermirror?.transform?.IsChildOf(Main._mirrorBase?.transform)}");


        //            if (othermirror is null || (othermirror != Main._mirrorBase && othermirror != Main._mirror45 && othermirror != Main._mirrorCeiling &&
        //                othermirror != Main._mirrorMicro && othermirror != Main._mirrorTrans &&  
        //                (Main._mirrorBase is null || !othermirror.transform.IsChildOf(Main._mirrorBase.transform)) &&
        //                (Main._mirror45 is null || !othermirror.transform.IsChildOf(Main._mirror45.transform)) &&
        //                (Main._mirrorCeiling is null || !othermirror.transform.IsChildOf(Main._mirrorCeiling.transform)) &&
        //                (Main._mirrorMicro is null || !othermirror.transform.IsChildOf(Main._mirrorMicro.transform)) &&
        //                (Main._mirrorTrans is null || !othermirror.transform.IsChildOf(Main._mirrorTrans.transform))  ))
        //            {
        //                //Main.Logger.Msg($"setting layers");
        //                mirror.m_ReflectLayers = mirror.m_ReflectLayers.value & ~reserved3; //Force all mirrors to not reflect "Mirror/TransparentBackground" - Set all mirrors to exclude reserved3                                                                                             
        //            }
        //        }
        //        catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
        //    }
        //}

        public static IEnumerator FixMirrorLayer(Transform mirrorBase, bool child)
        { //CVR appearently adds layers on init of the mirror, this should? fix that. 

            yield return new WaitForSeconds(.1f);
            if (child)
            {
                mirrorBase.GetComponent<CVRMirror>().m_ReflectLayers = (1 << CVRLayers.PlayerNetwork);
                mirrorBase.GetChild(0).GetComponent<CVRMirror>().m_ReflectLayers = (1 << CVRLayers.PlayerLocal) | (1 << CVRLayers.PlayerClone); 
            }
            else
                mirrorBase.GetComponent<CVRMirror>().m_ReflectLayers = (1 << CVRLayers.PlayerLocal) | (1 << CVRLayers.PlayerClone);
        }


        public static void FixFrame(GameObject mirror, float ScaleX, float ScaleY)
        {
            var FrameCon = mirror.transform.Find("Frame");
            //Top
            FrameCon.transform.Find("Plane").localPosition = new Vector3(0f, (.5f + (0.01f / ScaleY)), 0.01f); 
            FrameCon.transform.Find("Plane").localScale = new Vector3((0.1f + (0.0045f / ScaleX)), 1f, (0.0025f / ScaleY));
            //Bottom
            FrameCon.transform.Find("Plane (1)").localPosition = new Vector3(0f, (-.5f - (0.01f / ScaleY)), 0.01f);
            FrameCon.transform.Find("Plane (1)").localScale = new Vector3((0.1f + (0.0045f / ScaleX)), 1f, (0.0025f / ScaleY));
            //Left
            FrameCon.transform.Find("Plane (2)").localPosition = new Vector3((-.5f - (0.01f / ScaleX)), 0f, 0.01f);
            FrameCon.transform.Find("Plane (2)").localScale = new Vector3((0.0025f / ScaleX), 1f, (0.1f + (0.0045f / ScaleY)));
            //Right
            FrameCon.transform.Find("Plane (3)").localPosition = new Vector3((.5f + (0.01f / ScaleX)), 0f, 0.01f);
            FrameCon.transform.Find("Plane (3)").localScale = new Vector3((0.0025f / ScaleX), 1f, (0.1f + (0.0045f / ScaleY)));
        }

        public static void ToggleMirror()
        {
            if (Main._mirrorBase != null)
            {
                try { UnityEngine.Object.Destroy(Main._mirrorBase); } catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                Main._mirrorBase = null;
            }
            else
            {
                //if (Main._base_MirrorState.Value == "MirrorCutout" || Main._base_MirrorState.Value == "MirrorTransparent" ||
                //    Main._base_MirrorState.Value == "MirrorCutoutSolo" || Main._base_MirrorState.Value == "MirrorTransparentSolo" ||
                //    Main._base_MirrorState.Value == "MirrorTransCutCombo") SetAllMirrorsToIgnoreShader();
                GameObject player = Utils.GetPlayer().gameObject;

                var cam = Camera.main.gameObject;
                Vector3 pos = PlayerSetup.Instance.GetPlayerPosition();// player.transform.position;

                pos += player.transform.up * .5f;
                pos += player.transform.up * (Main._base_MirrorScaleY.Value - 1) / 2;

                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.layer = CVRLayers.CVRReserved3;
                mirror.transform.localScale = new Vector3(Main._base_MirrorScaleX.Value, Main._base_MirrorScaleY.Value, 0.05f);
                mirror.name = "PortableMirror";

                if (Main._base_PositionOnView.Value)
                {
                    mirror.transform.position = cam.transform.position + cam.transform.forward + (cam.transform.forward * Main._base_MirrorDistance.Value);
                    mirror.transform.rotation = cam.transform.rotation;
                }
                else
                {
                    mirror.transform.position = pos;

                    Vector3 lookDirection = cam.transform.forward;
                    Vector3 projectedLookDirection = Vector3.ProjectOnPlane(lookDirection, player.transform.up);
                    Quaternion rotation = Quaternion.LookRotation(projectedLookDirection, player.transform.up);
                    mirror.transform.rotation = rotation;
                    mirror.transform.position = mirror.transform.position + mirror.transform.forward + (mirror.transform.forward * Main._base_MirrorDistance.Value); //Move on distance
                }

                var childMirror = mirror.transform.Find(Main._base_MirrorState.Value);
                childMirror.gameObject.SetActive(true);
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8; //Default prefab 4:Water - 8:Playerlocal 
                if (Main._base_MirrorState.Value == "MirrorTransparent" || Main._base_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._base_MirrorState.Value == "MirrorTransCutCombo")
                { 
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 300f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = false; // Main._base_CanPickupMirror.Value;
                mirror.GetOrAddComponent<BoxCollider>().enabled = false; // Main._base_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(Main._base_CanPickupMirror.Value & Main.pickupFrame.Value);
                FixFrame(mirror, Main._base_MirrorScaleX.Value, Main._base_MirrorScaleY.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._base_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(PlayerSetup.Instance.transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._base_MirrorState.Value == "MirrorCutoutSolo" || Main._base_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._base_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));
                Main._mirrorBase = mirror;

                if (Main._base_followGaze.Value) MelonCoroutines.Start(followGazeBase());
                if (MetaPort.Instance.isUsingVr && Main.customGrab_en.Value)
                {

                    if (Main._base_CanPickupMirror.Value) customPickupStart("Base", mirror);
                }
                else
                {

                    mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._base_CanPickupMirror.Value;
                    mirror.GetOrAddComponent<BoxCollider>().enabled = Main._base_CanPickupMirror.Value;
                }
            }
        }

        public static void ToggleMirror45()
        {
            if (Main._mirror45 != null)
            {
                try { UnityEngine.Object.Destroy(Main._mirror45); } catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                Main._mirror45 = null;
            }
            else
            {
                //if (Main._45_MirrorState.Value == "MirrorCutout" || Main._45_MirrorState.Value == "MirrorTransparent" ||
                //    Main._45_MirrorState.Value == "MirrorCutoutSolo" || Main._45_MirrorState.Value == "MirrorTransparentSolo" ||
                //    Main._45_MirrorState.Value == "MirrorTransCutCombo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;
                Vector3 pos = player.transform.position;
                pos += player.transform.up * .5f;
                pos += player.transform.up * (Main._45_MirrorScaleY.Value - 1) / 2;

                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.layer = CVRLayers.CVRReserved3;
                mirror.transform.localScale = new Vector3(Main._45_MirrorScaleX.Value, Main._45_MirrorScaleY.Value, 0.05f);
                mirror.name = "PortableMirror45";

                mirror.transform.position = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z); //Set to player height instead of centered on camera
                //mirror.transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z); //Make vertical
                //mirror.transform.rotation = player.transform.rotation;
                Vector3 lookDirection = cam.transform.forward;
                Vector3 projectedLookDirection = Vector3.ProjectOnPlane(lookDirection, player.transform.up);
                Quaternion rotation = Quaternion.LookRotation(projectedLookDirection, player.transform.up);
                mirror.transform.rotation = rotation;
                mirror.transform.position = mirror.transform.position + mirror.transform.forward + (mirror.transform.forward * Main._45_MirrorDistance.Value); //Move on distance
                mirror.transform.rotation = mirror.transform.rotation * Quaternion.AngleAxis(45, Vector3.left);  // Sets the transform's current rotation to a new rotation that rotates 30 degrees around the y-axis(Vector3.up)

                var childMirror = mirror.transform.Find(Main._45_MirrorState.Value);
                childMirror.gameObject.SetActive(true);
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._45_MirrorState.Value == "MirrorTransparent" || Main._45_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._45_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3000f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = false;// Main._45_CanPickupMirror.Value;
                mirror.GetOrAddComponent<BoxCollider>().enabled = false;// Main._45_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(Main._45_CanPickupMirror.Value & Main.pickupFrame.Value);
                FixFrame(mirror, Main._45_MirrorScaleX.Value, Main._45_MirrorScaleY.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._45_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(PlayerSetup.Instance.transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._45_MirrorState.Value == "MirrorCutoutSolo" || Main._45_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._45_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));

                if (Main._45_followGaze.Value) MelonCoroutines.Start(followGaze45());
                Main._mirror45 = mirror;

                if (MetaPort.Instance.isUsingVr && Main.customGrab_en.Value)
                {

                    if (Main._45_CanPickupMirror.Value) customPickupStart("45", mirror);
                }
                else
                {

                    mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._45_CanPickupMirror.Value;
                    mirror.GetOrAddComponent<BoxCollider>().enabled = Main._45_CanPickupMirror.Value;
                }

            }
        }

        public static void ToggleMirrorCeiling()
        {

            if (Main._mirrorCeiling != null)
            {
                try { UnityEngine.Object.Destroy(Main._mirrorCeiling); } catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                Main._mirrorCeiling = null;
            }
            else
            {
                //if (Main._ceil_MirrorState.Value == "MirrorCutout" || Main._ceil_MirrorState.Value == "MirrorTransparent" ||
                //    Main._ceil_MirrorState.Value == "MirrorCutoutSolo" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo" ||
                //    Main._ceil_MirrorState.Value == "MirrorTransCutCombo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;

                Vector3 pos = cam.transform.position + (player.transform.up); // Bases mirror position off of hip, to allow for play space moving 
                //Main.Logger.Msg($"x:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.x}, y:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.y}, z:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.z}");
                pos += player.transform.up * Main._ceil_MirrorDistance.Value;
                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.layer = CVRLayers.CVRReserved3;
                mirror.transform.position = pos;

                Vector3 lookDirection = player.transform.up;
                Vector3 projectedLookDirection = Vector3.ProjectOnPlane(lookDirection, player.transform.right);
                Quaternion rotation = Quaternion.LookRotation(projectedLookDirection, player.transform.right);
                mirror.transform.rotation = rotation;

                mirror.transform.localScale = new Vector3(Main._ceil_MirrorScaleX.Value, Main._ceil_MirrorScaleZ.Value, 0.05f);
                mirror.name = "PortableMirrorCeiling";

                var childMirror = mirror.transform.Find(Main._ceil_MirrorState.Value);
                childMirror.gameObject.SetActive(true);
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._ceil_MirrorState.Value == "MirrorTransparent" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._ceil_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3000f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = false; //Main._ceil_CanPickupMirror.Value;
                mirror.GetOrAddComponent<BoxCollider>().enabled = false; //Main._ceil_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(Main._ceil_CanPickupMirror.Value & Main.pickupFrame.Value);
                FixFrame(mirror, Main._ceil_MirrorScaleX.Value, Main._ceil_MirrorScaleZ.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._ceil_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(PlayerSetup.Instance.transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._ceil_MirrorState.Value == "MirrorCutoutSolo" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._ceil_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));
                Main._mirrorCeiling = mirror;

                if (MetaPort.Instance.isUsingVr && Main.customGrab_en.Value)
                {

                    if (Main._ceil_CanPickupMirror.Value) customPickupStart("Ceiling", mirror);
                }
                else
                {

                    mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._ceil_CanPickupMirror.Value;
                    mirror.GetOrAddComponent<BoxCollider>().enabled = Main._ceil_CanPickupMirror.Value;
                }
            }
        }

        public static void ToggleMirrorMicro()
        {
            if (Main._mirrorMicro != null)
            {
                try { UnityEngine.Object.Destroy(Main._mirrorMicro); } catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                Main._mirrorMicro = null;
            }
            else
            {
                //if (Main._micro_MirrorState.Value == "MirrorCutout" || Main._micro_MirrorState.Value == "MirrorTransparent" ||
                //    Main._micro_MirrorState.Value == "MirrorCutoutSolo" || Main._micro_MirrorState.Value == "MirrorTransparentSolo" ||
                //    Main._micro_MirrorState.Value == "MirrorTransCutCombo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;
                Vector3 pos = PlayerSetup.Instance.GetPlayerPosition();// player.transform.position;
                //pos.y += Main._micro_MirrorScaleY.Value / 4;///This will need turning

                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.layer = CVRLayers.CVRReserved3;
                mirror.transform.localScale = new Vector3(Main._micro_MirrorScaleX.Value, Main._micro_MirrorScaleY.Value, 0.05f);
                mirror.name = "PortableMirrorMicro";

                if (Main._micro_PositionOnView.Value)
                {
                    mirror.transform.rotation = cam.transform.rotation;
                    mirror.transform.position = cam.transform.position + (cam.transform.forward * Main._micro_MirrorScaleY.Value)
                        + (mirror.transform.forward * Main._micro_MirrorDistance.Value);
                }
                else
                {
                    mirror.transform.position = cam.transform.position;
                    Vector3 lookDirection = cam.transform.forward;
                    Vector3 projectedLookDirection = Vector3.ProjectOnPlane(lookDirection, player.transform.up);
                    Quaternion rotation = Quaternion.LookRotation(projectedLookDirection, player.transform.up);
                    mirror.transform.rotation = rotation;
                    mirror.transform.position = mirror.transform.position + (mirror.transform.forward * Main._micro_MirrorScaleY.Value)
                        + (mirror.transform.forward * Main._micro_MirrorDistance.Value); //Move on distance
                    mirror.transform.position -= player.transform.up * Main._micro_MirrorScaleY.Value / 4;
                    //mirror.transform.position = mirror.transform.position - (mirror.transform.up * Main._micro_MirrorScaleY.Value/PlayerSetup.Instance.GetAvatarHeight() * .75f);//Move mirror down based on avatar height
                }

                var childMirror = mirror.transform.Find(Main._micro_MirrorState.Value);
                childMirror.gameObject.SetActive(true);
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._micro_MirrorState.Value == "MirrorTransparent" || Main._micro_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._micro_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = Main._micro_GrabRange.Value;  /////change?
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = false;// Main._micro_CanPickupMirror.Value;
                mirror.GetOrAddComponent<BoxCollider>().enabled = false;//  Main._micro_CanPickupMirror.Value;
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                if (!Main._micro_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(PlayerSetup.Instance.transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._micro_MirrorState.Value == "MirrorCutoutSolo" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._micro_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));

                Main._mirrorMicro = mirror;
                if (Main._micro_followGaze.Value) MelonCoroutines.Start(followGazeMicro());
                if (MetaPort.Instance.isUsingVr && Main.customGrab_en.Value)
                {

                    if (Main._micro_CanPickupMirror.Value) customPickupStart("Micro", mirror);
                }
                else
                {

                    mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._micro_CanPickupMirror.Value;
                    mirror.GetOrAddComponent<BoxCollider>().enabled = Main._micro_CanPickupMirror.Value;
                }
            }
        }

        public static void ToggleMirrorTrans()
        {
            if (Main._mirrorTrans != null)
            {
                try { UnityEngine.Object.Destroy(Main._mirrorTrans); } catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                Main._mirrorTrans = null;
            }
            else
            {
                //if (Main._trans_MirrorState.Value == "MirrorCutout" || Main._trans_MirrorState.Value == "MirrorTransparent" ||
                //    Main._trans_MirrorState.Value == "MirrorCutoutSolo" || Main._trans_MirrorState.Value == "MirrorTransparentSolo" ||
                //    Main._trans_MirrorState.Value == "MirrorTransCutCombo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer();
                var cam = Camera.main.gameObject;
                Vector3 pos = PlayerSetup.Instance.GetPlayerPosition();// player.transform.position;
                pos += player.transform.up * .5f;
                pos += player.transform.up * (Main._trans_MirrorScaleY.Value - 1) / 2;

                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.layer = CVRLayers.CVRReserved3;
                mirror.transform.localScale = new Vector3(Main._trans_MirrorScaleX.Value, Main._trans_MirrorScaleY.Value, 0.05f);
                mirror.name = "PortableMirrorTrans";

                if (Main._trans_PositionOnView.Value)
                {
                    mirror.transform.position = cam.transform.position + cam.transform.forward + (cam.transform.forward * Main._trans_MirrorDistance.Value);
                    mirror.transform.rotation = cam.transform.rotation;
                }
                else
                {
                    mirror.transform.position = pos;
                    Vector3 lookDirection = cam.transform.forward;
                    Vector3 projectedLookDirection = Vector3.ProjectOnPlane(lookDirection, player.transform.up);
                    Quaternion rotation = Quaternion.LookRotation(projectedLookDirection, player.transform.up);
                    mirror.transform.rotation = rotation;
                    mirror.transform.position = mirror.transform.position + mirror.transform.forward + (mirror.transform.forward * Main._trans_MirrorDistance.Value); //Move on distance
                }

                var childMirror = mirror.transform.Find(Main._trans_MirrorState.Value);
                childMirror.gameObject.SetActive(true);
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 10;
                if (Main._trans_MirrorState.Value == "MirrorTransparent" || Main._trans_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._trans_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 300f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = false;
                mirror.GetOrAddComponent<BoxCollider>().enabled = false;
                mirror.transform.Find("Frame").gameObject.SetActive(Main._trans_CanPickupMirror.Value & Main.pickupFrame.Value);
                FixFrame(mirror, Main._trans_MirrorScaleX.Value, Main._trans_MirrorScaleY.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._trans_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(PlayerSetup.Instance.transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._trans_MirrorState.Value == "MirrorCutoutSolo" || Main._trans_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._trans_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));

                if (Main._trans_followGaze.Value) MelonCoroutines.Start(followGazeTrans());
                Main._mirrorTrans = mirror;

                if (MetaPort.Instance.isUsingVr && Main.customGrab_en.Value)
                {

                    if (Main._trans_CanPickupMirror.Value) customPickupStart("Trans", mirror);
                }
                else
                {

                    mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._trans_CanPickupMirror.Value;
                    mirror.GetOrAddComponent<BoxCollider>().enabled = Main._trans_CanPickupMirror.Value;
                }
            }
        }


        private static void SetupLineRender()
        {
            if (pickupLine?.Equals(null) ?? true) //usePickupLine
            {
                GameObject myLine = new GameObject();
                myLine.name = "PortMirrorPickupLine";
                myLine.transform.SetParent(PlayerSetup.Instance.vrRayRight.transform);
                myLine.transform.localPosition = Vector3.zero;
                myLine.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                myLine.AddComponent<LineRenderer>();
                myLine.layer = CVRLayers.UIInternal;
                LineRenderer lr = myLine.GetComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Particles/Standard Unlit"));
                lr.useWorldSpace = false;
                lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lr.receiveShadows = false;
                lr.startColor = new Color(1f, 0f, 1f, .75f);
                lr.endColor = new Color(.58f, .219f, 1f, .5f);
                lr.startWidth = .005f;
                lr.endWidth = 0.001f;
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, new Vector3(0f, 1f, 0f));

                pickupLine = myLine;
            }
        }

        //Could I have made this all just one Coroutine that handled all pickups, yes! But that is later me's issue -- 2023-02-24
        //Later me, is going to fix that, I swear -- 2023-03-10
        //I did it! -- 2023-11-02
        public static Dictionary<string, GameObject> customGrabs = new Dictionary<string, GameObject>();
        public static bool pickupObjsRunning = false;
        public static System.Object pickupObjs_Rout = null;

        public static void customPickupStart(string name, GameObject mirror)
        { 
            SetupLineRender();
            if (!customGrabs.ContainsKey(name)) 
                customGrabs.Add(name, mirror);
            else
                customGrabs[name] = mirror;
            if (!pickupObjsRunning)
            {
                if (pickupObjs_Rout != null) MelonCoroutines.Stop(pickupObjs_Rout);
                pickupObjs_Rout = MelonCoroutines.Start(pickupObjs());
            }
        }

        public static IEnumerator pickupObjs()
        {
            pickupObjsRunning = true;
            var rightCon = PlayerSetup.Instance.vrRayRight.gameObject;

            var held = false;
            (string, GameObject) lastHeld = ("", null);

            while (customGrabs.Count > 0)
            {
                try
                {
                    if (!held ? CVRInputManager.Instance.interactRightValue > .5f : CVRInputManager.Instance.gripRightValue > Main.customGrab_gripRightValue.Value && CVRInputManager.Instance.interactRightValue > .5f)
                    {//Allow laser to object while holding trigger without grabbing it

                        var hitFound = false;
                        RaycastHit hit = new RaycastHit();
                        Ray ray = new Ray(rightCon.transform.position, rightCon.transform.forward);
                        (string, GameObject, float) nearest = ("", null, 1000f);

                        foreach (var mirror in customGrabs)
                        { //Find the nearest mirror to the raycast

                            if (mirror.Value?.Equals(null) ?? true)
                            {
                                //Main.Logger.Msg($"Removing mirror {mirror.Key}");
                                customGrabs.Remove(mirror.Key);
                                //Main.Logger.Msg($"Removed");
                                break;
                            }

                            var col = mirror.Value.GetComponent<BoxCollider>(); col.enabled = true;
                            if (col.Raycast(ray, out hit, 1000f))
                            {
                                if (hit.distance < nearest.Item3)
                                {
                                    nearest.Item1 = mirror.Key; nearest.Item2 = mirror.Value; nearest.Item3 = hit.distance;
                                }
                                hitFound = true;
                            }
                            col.enabled = false;
                        }

                        if (hitFound || held)
                        {
                            var dist = nearest.Item3;

                            if (held || (CVRInputManager.Instance.gripRightValue > Main.customGrab_gripRightValue.Value && CVRInputManager.Instance.interactRightValue > .5f && !(nearest.Item2?.Equals(null) ?? true)))
                            {//Grab or continue grabbing
                                if (!held)
                                {
                                    dist = nearest.Item3;
                                    nearest.Item2.transform.SetParent(rightCon.transform, true);
                                    lastHeld.Item1 = nearest.Item1; lastHeld.Item2 = nearest.Item2;
                                    held = true;
                                }
                                else
                                {//Joystick Forward/Back

                                    Vector3 direction = rightCon.transform.forward;
                                    var tempPos = nearest.Item2.transform.position + direction * (InputSVR.GetVRLookVector().y * Time.deltaTime) * Mathf.Clamp(Main.customGrabSpeed.Value, 0f, 10f);
                                    var moveDistance = Vector3.Distance(tempPos, nearest.Item2.transform.position);
                                    var inputY = InputSVR.GetVRLookVector().y;

                                    dist = Vector3.Distance(rightCon.transform.position, nearest.Item2.transform.position);
                                    var col = nearest.Item2.GetComponent<BoxCollider>(); col.enabled = true;
                                    if (col.Raycast(ray, out hit, 1000f))
                                    {//Get the distance for ray, can't just use Gameobject distance as that will be short/long cause it is to the transform center
                                        dist = hit.distance;
                                    }
                                    col.enabled = false;

                                    //Make sure if never goes past the hand
                                    if (!(dist - moveDistance <= 0f && inputY < 0F))
                                        lastHeld.Item2.transform.position += direction * Mathf.Clamp(hit.distance / 2, 0, 2f) * (InputSVR.GetVRLookVector().y * Time.deltaTime) * Mathf.Clamp(Main.customGrabSpeed.Value, 0f, 10f);
                                }
                            }

                            pickupLine.SetActive(Main.customGrabLine.Value);
                            pickupLine.GetComponent<LineRenderer>().SetPosition(1, new Vector3(0f, dist, 0f));
                        }
                        else
                        {
                            pickupLine.SetActive(false);
                        }
                    }
                    else
                    {
                        pickupLine.SetActive(false);
                        if (held && !(lastHeld.Item2?.Equals(null) ?? true))
                        {
                            bool toTracking = false;
                            switch (lastHeld.Item1)
                            {
                                case "Base": toTracking = Main._base_AnchorToTracking.Value; break;
                                case "45": toTracking = Main._45_AnchorToTracking.Value; break;
                                case "Ceiling": toTracking = Main._ceil_AnchorToTracking.Value; break;
                                case "Micro": toTracking = Main._micro_AnchorToTracking.Value; break;
                                case "Trans": toTracking = Main._trans_AnchorToTracking.Value; break;
                                default: toTracking = false; break;
                            }

                            if (toTracking) lastHeld.Item2.transform.SetParent(PlayerSetup.Instance.transform, true);
                            else lastHeld.Item2.transform.SetParent(null);
                        }
                        held = false;
                    }
                }
                catch (System.Exception ex) { Main.Logger.Error("Error in pickupObjs - Aborting:\n" +  ex.ToString()); pickupObjsRunning = false; yield break; }
                yield return null;        
            }
            pickupLine.SetActive(false);
            pickupObjsRunning = false;
        }

        
        public static IEnumerator followGazeBase()
        {
            bool waitToSettle = false;
            float settleSeconds = 0f;
            float breakSeconds = 0f;
            //Main.Logger.Msg($"EnterFollowGaze");
            var cam = Camera.main.gameObject;
            var player = Utils.GetPlayer();
            var mirror = Main._mirrorBase;
            Vector3 velocity = Vector3.zero;
            Vector3 velocityRot = Vector3.zero;
            _baseFollowGazeActive = true;
            while (Main._base_followGaze.Value)
            {
                if (mirror?.Equals(null) ?? true) { _baseFollowGazeActive = false; yield break; }

                var pos = player.transform.position;
                pos.y += .5f;
                pos.y += (Main._base_MirrorScaleY.Value - 1) / 2;
                Quaternion tempRot; Vector3 tempPos;
                if (Main._base_PositionOnView.Value)
                {
                    tempRot = cam.transform.rotation;
                    tempPos = cam.transform.position + cam.transform.forward + (cam.transform.forward * Main._base_MirrorDistance.Value);
                }
                else
                {
                    tempRot = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical
                    tempPos = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z) +
                          tempRot * Vector3.forward * (1f + Main._base_MirrorDistance.Value); //Set to player height instead of centered on camera, then move in forward direction of camera
                }
                if (Main.followGazeDeadBand_en.Value)
                {
                    var angleDif = Quaternion.Angle(mirror.transform.rotation, tempRot);
                    if (angleDif > Main.followGazeDeadBand.Value && !waitToSettle)
                    { //Is the player looking far enough away from mirror? If so check how long, or just instantly try to resettle the mirror
                        if (Main.followGazeDeadBandBreakTime.Value > 0)
                        {
                            if (breakSeconds == 0)
                                breakSeconds = Time.time + Main.followGazeDeadBandBreakTime.Value;
                            else
                            {
                                if (breakSeconds < Time.time) { breakSeconds = 0; waitToSettle = true; }
                            }
                        }
                        else
                            waitToSettle = true;
                    }
                    else //If no longer looking away, reset timer
                        breakSeconds = 0;
                    if (waitToSettle)
                    {
                        mirror.transform.position = Vector3.SmoothDamp(mirror.transform.position, tempPos, ref velocity, Main.followGazeTime.Value);
                        mirror.transform.rotation = Utils.SmoothDampQuaternion(mirror.transform.rotation, tempRot, ref velocityRot, Main.followGazeTime.Value);
                        if (angleDif < Main.followGazeDeadBandSettle.Value)
                        {
                            if (Main.followGazeDeadBandSeconds.Value > 0)
                            {
                                if (settleSeconds == 0)
                                    settleSeconds = Time.time + Main.followGazeDeadBandSeconds.Value;
                                else
                                {
                                    if (settleSeconds < Time.time) { settleSeconds = 0; waitToSettle = false; }
                                }
                            }
                            else
                                waitToSettle = false;
                        }
                        else
                            settleSeconds = 0f;
                    }
                    
                }
                else
                {
                    mirror.transform.position = Vector3.SmoothDamp(mirror.transform.position, tempPos, ref velocity, Main.followGazeTime.Value);
                    mirror.transform.rotation = Utils.SmoothDampQuaternion(mirror.transform.rotation, tempRot, ref velocityRot, Main.followGazeTime.Value);
                }
                yield return null;
            }
            _baseFollowGazeActive = false;
            //Main.Logger.Msg($"ExitFollowGaze");
        }

        public static IEnumerator followGaze45()
        {
            //Main.Logger.Msg($"EnterFollowGaze");
            var cam = Camera.main.gameObject;
            var player = Utils.GetPlayer();
            var mirror = Main._mirror45;
            Vector3 velocity = Vector3.zero;
            Vector3 velocityRot = Vector3.zero;
            _45FollowGazeActive = true;
            while (Main._45_followGaze.Value)
            {
                if (mirror?.Equals(null) ?? true) { _45FollowGazeActive = false; yield break; }

                var pos = player.transform.position;
                pos.y += .5f;
                pos.y += (Main._45_MirrorScaleY.Value - 1) / 2;

                Quaternion tempRot; Vector3 tempPos;

                tempRot = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical
                tempPos = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z)
                    + tempRot * Vector3.forward * (1f + Main._45_MirrorDistance.Value); //Set to player height instead of centered on camera, then move in forward direction of camera
                tempRot = tempRot * Quaternion.AngleAxis(45, Vector3.left);  // Sets the transform's current rotation to a new rotation that rotates 30 degrees around the y-axis(Vector3.up)


                mirror.transform.position = Vector3.SmoothDamp(mirror.transform.position, tempPos, ref velocity, Main.followGazeTime.Value);
                mirror.transform.rotation = Utils.SmoothDampQuaternion(mirror.transform.rotation, tempRot, ref velocityRot, Main.followGazeTime.Value);

                yield return null;
            }
            _45FollowGazeActive = false;
            //Main.Logger.Msg($"ExitFollowGaze");
        }

        public static IEnumerator followGazeMicro()
        {
            //Main.Logger.Msg($"EnterFollowGaze");
            var cam = Camera.main.gameObject;
            var mirror = Main._mirrorMicro;
            Vector3 velocity = Vector3.zero;
            Vector3 velocityRot = Vector3.zero;
            _microFollowGazeActive = true;
            while (Main._micro_followGaze.Value)
            {
                if (mirror?.Equals(null) ?? true) { _microFollowGazeActive = false; yield break; }

                var pos = cam.transform.position;

                Quaternion tempRot; Vector3 tempPos;

                if (Main._micro_PositionOnView.Value)
                {
                    tempRot = cam.transform.rotation;
                    tempPos = cam.transform.position + (cam.transform.forward * Main._micro_MirrorScaleY.Value)
                        + (mirror.transform.forward * Main._micro_MirrorDistance.Value);
                }
                else
                {
                    tempRot = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical
                    tempPos = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z) +
                          tempRot * Vector3.forward * (Main._micro_MirrorScaleY.Value + Main._micro_MirrorDistance.Value); //Set to player height instead of centered on camera, then move in forward direction of camera
                }

                mirror.transform.position = Vector3.SmoothDamp(mirror.transform.position, tempPos, ref velocity, Main.followGazeTime.Value);
                mirror.transform.rotation = Utils.SmoothDampQuaternion(mirror.transform.rotation, tempRot, ref velocityRot, Main.followGazeTime.Value);

                yield return null;
            }
            _microFollowGazeActive = false;
            //Main.Logger.Msg($"ExitFollowGaze");
        }

        public static IEnumerator followGazeTrans()
        {
            //Main.Logger.Msg($"EnterFollowGaze");
            var cam = Camera.main.gameObject;
            var player = Utils.GetPlayer();
            var mirror = Main._mirrorTrans;
            Vector3 velocity = Vector3.zero;
            Vector3 velocityRot = Vector3.zero;
            _transFollowGazeActive = true;
            while (Main._trans_followGaze.Value)
            {
                if (mirror?.Equals(null) ?? true) { _transFollowGazeActive = false; yield break; }

                var pos = player.transform.position;
                pos.y += .5f;
                pos.y += (Main._trans_MirrorScaleY.Value - 1) / 2;

                Quaternion tempRot; Vector3 tempPos;

                if (Main._trans_PositionOnView.Value)
                {
                    tempRot = cam.transform.rotation;
                    tempPos = cam.transform.position + cam.transform.forward + (cam.transform.forward * Main._trans_MirrorDistance.Value);
                }
                else
                {
                    tempRot = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical
                    tempPos = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z) +
                          tempRot * Vector3.forward * (1f + Main._trans_MirrorDistance.Value); //Set to player height instead of centered on camera, then move in forward direction of camera
                }

                mirror.transform.position = Vector3.SmoothDamp(mirror.transform.position, tempPos, ref velocity, Main.followGazeTime.Value);
                mirror.transform.rotation = Utils.SmoothDampQuaternion(mirror.transform.rotation, tempRot, ref velocityRot, Main.followGazeTime.Value);

                //_mirrorCal.transform.rotation = tempRot;
                //var step = Main.followGazeSpeed.Value * Time.deltaTime; // calculate distance to move
                //mirror.transform.position = Vector3.MoveTowards(mirror.transform.position, tempPos, step);
                //mirror.transform.rotation = Quaternion.RotateTowards(mirror.transform.rotation, tempRot, step * 40);

                yield return null;
            }
            _transFollowGazeActive = false;
            //Main.Logger.Msg($"ExitFollowGaze");
        }

    }
}
