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
using ABI_RC.Core.InteractionSystem;


namespace PortableMirror
{

    public class Mirrors
    {
        //PlayerLocal = 1 << 8; 
        //PlayerClone  = 1 << 9; 
        //PlayerNetwork  = 1 << 10; 
        //MirrorReflection  = 1 << 11; 
        //UiLayer = 1 << 5;
        public static int reserved4 = 1 << 15;
        //int optMirrorMask = PlayerNetwork | MirrorReflectionLayer;   //Double check this
        //int fullMirrorMask = -1 & ~UiLayer & ~PlayerLocal & ~reserved4; //Double check this

        public static AssetBundle assetBundle;
        public static GameObject mirrorPrefab, mirrorSettingsPrefab;

        public static object calDelayRoutine, waitForMeasureRoutine, calDelayRoutineOff;
        public static bool _calInit = false;
        public static float _calHeight;
        public static Dictionary<GameObject, int> calObjects = new Dictionary<GameObject, int>();

        public static bool _baseFollowGazeActive, _45FollowGazeActive, _transFollowGazeActive, _microFollowGazeActive;

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

        public static void ForceMirrorLayer()
        {
            foreach (var mirror in UnityEngine.Object.FindObjectsOfType<CVRMirror>())
            {
                try
                {
                    mirror.m_ReflectLayers = mirror.m_ReflectLayers.value & ~(1 << 4); //Force all mirrors to not reflect "Water" and set all mirrors to water                                                                                     
                    mirror.gameObject.layer = 4;
                }
                catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
            }
        }
        public static IEnumerator SetOrder(GameObject obj)
        {
            yield return new WaitForSeconds(1f);
            if (!obj?.Equals(null) ?? false)
            {
                //if (fixRenderOrder.Value) obj.GetComponentInChildren<Renderer>().material.renderQueue = 5000;
                obj.GetComponentInChildren<CVRMirror>().m_DisablePixelLights = !Main.usePixelLights.Value;
            }

        }

        public static void SetAllMirrorsToIgnoreShader()
        {
            foreach (var mirror in UnityEngine.Object.FindObjectsOfType<CVRMirror>())
            { // https://github.com/knah/VRCMods/blob/master/MirrorResolutionUnlimiter/UiExtensionsAddon.cs
                try
                {
                    //Main.Logger.Msg($"-----");
                    //Main.Logger.Msg($"{vrcMirrorReflection.gameObject.name}");
                    GameObject othermirror = mirror?.gameObject?.transform?.parent?.gameObject; // Question marks are always the answer
                    //Main.Logger.Msg($"othermirror is null:{othermirror is null}, !=base:{othermirror != _mirrorBase}, !=45:{othermirror != _mirror45}, !=Micro:{othermirror != _mirrorCeiling}, !=trans:{othermirror != _mirrorTrans}");
                    if (othermirror is null || (othermirror != Main._mirrorBase && othermirror != Main._mirror45 && othermirror != Main._mirrorCeiling &&
                        othermirror != Main._mirrorMicro && othermirror != Main._mirrorTrans))
                    {
                        //Main.Logger.Msg($"setting layers");
                        mirror.m_ReflectLayers = mirror.m_ReflectLayers.value & ~reserved4; //Force all mirrors to not reflect "Mirror/TransparentBackground" - Set all mirrors to exclude reserved4                                                                                             
                    }
                }
                catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
            }
        }

        public static IEnumerator FixMirrorLayer(Transform mirrorBase, bool child)
        { //CVR appearently adds layers on init of the mirror, this should? fix that. 

            yield return new WaitForSeconds(.1f);
            if (child)
            {
                mirrorBase.GetComponent<CVRMirror>().m_ReflectLayers = 33792; //Layers 10 (PlayerNetwork) & 15 (layer for shader)
                mirrorBase.GetChild(0).GetComponent<CVRMirror>().m_ReflectLayers = 33024; //Layers 8 (Playerlocal) & 15 (layer for shader)
            }
            else
                mirrorBase.GetComponent<CVRMirror>().m_ReflectLayers = 33024; //Layers 8 (Playerlocal) & 15 (layer for shader)
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
                if (Main._base_MirrorState.Value == "MirrorCutout" || Main._base_MirrorState.Value == "MirrorTransparent" || Main._base_MirrorState.Value == "MirrorCutoutSolo" || Main._base_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                GameObject player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;
                Vector3 pos = player.transform.position;

                pos.y += .5f;
                pos.y += (Main._base_MirrorScaleY.Value - 1) / 2;

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
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8; //Default prefab 4:Water - 8:Playerlocal 
                if (Main._base_MirrorState.Value == "MirrorTransparent" || Main._base_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._base_MirrorState.Value == "MirrorTransCutCombo")
                { 
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._base_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(Main._base_CanPickupMirror.Value & Main.pickupFrame.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._base_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._base_MirrorState.Value == "MirrorCutoutSolo" || Main._base_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._base_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));
                if (Main._base_followGaze.Value) MelonCoroutines.Start(followGazeBase());

                Main._mirrorBase = mirror;
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
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._45_MirrorState.Value == "MirrorTransparent" || Main._45_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._45_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._45_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(Main._45_CanPickupMirror.Value & Main.pickupFrame.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._45_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._45_MirrorState.Value == "MirrorCutoutSolo" || Main._45_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._45_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));
                if (Main._45_followGaze.Value) MelonCoroutines.Start(followGaze45());

                Main._mirror45 = mirror;
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
                if (Main._ceil_MirrorState.Value == "MirrorCutout" || Main._ceil_MirrorState.Value == "MirrorTransparent" || Main._ceil_MirrorState.Value == "MirrorCutoutSolo" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;

                Vector3 pos = cam.transform.position + (player.transform.up); // Bases mirror position off of hip, to allow for play space moving 
                //Main.Logger.Msg($"x:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.x}, y:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.y}, z:{GameObject.Find(player.gameObject.name + "/AnimationController/HeadAndHandIK/HipTarget").transform.position.z}");
                pos.y += Main._ceil_MirrorDistance.Value;
                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.transform.position = pos;
                mirror.transform.rotation = Quaternion.Euler(-90f, cam.transform.rotation.eulerAngles.y, cam.transform.rotation.eulerAngles.z);
                //mirror.transform.rotation = Quaternion.AngleAxis(90, Vector3.left);  // Sets the transform's current rotation to a new rotation that rotates 90 degrees around the y-axis(Vector3.up)
                mirror.transform.localScale = new Vector3(Main._ceil_MirrorScaleX.Value, Main._ceil_MirrorScaleZ.Value, 1f);
                mirror.name = "PortableMirrorCeiling";

                var childMirror = mirror.transform.Find(Main._ceil_MirrorState.Value);
                childMirror.gameObject.active = true;
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._ceil_MirrorState.Value == "MirrorTransparent" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._ceil_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._ceil_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(Main._ceil_CanPickupMirror.Value & Main.pickupFrame.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._ceil_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._ceil_MirrorState.Value == "MirrorCutoutSolo" || Main._ceil_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._ceil_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));
                Main._mirrorCeiling = mirror;
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
                if (Main._micro_MirrorState.Value == "MirrorCutout" || Main._micro_MirrorState.Value == "MirrorTransparent" || Main._micro_MirrorState.Value == "MirrorCutoutSolo" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") SetAllMirrorsToIgnoreShader();
                var player = Utils.GetPlayer().gameObject;
                var cam = Camera.main.gameObject;
                Vector3 pos = cam.transform.position;
                //pos.y += Main._micro_MirrorScaleY.Value / 4;///This will need turning

                GameObject mirror = GameObject.Instantiate(mirrorPrefab);
                mirror.transform.localScale = new Vector3(Main._micro_MirrorScaleX.Value, Main._micro_MirrorScaleY.Value, 1f);
                mirror.name = "PortableMirrorMicro";

                if (Main._micro_PositionOnView.Value)
                {
                    mirror.transform.rotation = cam.transform.rotation;
                    mirror.transform.position = cam.transform.position + (cam.transform.forward * Main._micro_MirrorScaleY.Value)
                        + (mirror.transform.forward * Main._micro_MirrorDistance.Value);
                }
                else
                {
                    mirror.transform.position = new Vector3(cam.transform.position.x, pos.y, cam.transform.position.z); //Set to player height instead of centered on camera
                    mirror.transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f); //Make vertical
                    mirror.transform.position = mirror.transform.position + (mirror.transform.forward * Main._micro_MirrorScaleY.Value)
                        + (mirror.transform.forward * Main._micro_MirrorDistance.Value); //Move on distance
                }

                var childMirror = mirror.transform.Find(Main._micro_MirrorState.Value);
                childMirror.gameObject.active = true;
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 8;
                if (Main._micro_MirrorState.Value == "MirrorTransparent" || Main._micro_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._micro_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = Main._micro_GrabRange.Value;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._micro_CanPickupMirror.Value;
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                if (!Main._micro_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._micro_MirrorState.Value == "MirrorCutoutSolo" || Main._micro_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._micro_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));
                if (Main._micro_followGaze.Value) MelonCoroutines.Start(followGazeMicro());

                Main._mirrorMicro = mirror;
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
                //childMirror.gameObject.layer = Main.MirrorsShowInCamera.Value ? 4 : 10;
                if (Main._trans_MirrorState.Value == "MirrorTransparent" || Main._trans_MirrorState.Value == "MirrorTransparentSolo" ||
                    Main._trans_MirrorState.Value == "MirrorTransCutCombo")
                {
                    childMirror.GetComponent<Renderer>().material.SetFloat("_Transparency", Main.TransMirrorTrans.Value);
                    childMirror.GetComponent<Renderer>().material.renderQueue = 3000;
                }
                mirror.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3f;
                mirror.GetOrAddComponent<CVRPickupObject>().enabled = Main._trans_CanPickupMirror.Value;
                mirror.transform.Find("Frame").gameObject.SetActive(Main._trans_CanPickupMirror.Value & Main.pickupFrame.Value);
                //mirror.GetOrAddComponent<CVRPickupObject>().allowManipulationWhenEquipped = false;
                mirror.GetOrAddComponent<CVRPickupObject>().gripType = Main.PickupToHand.Value ? CVRPickupObject.GripType.Origin : CVRPickupObject.GripType.Free;
                mirror.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, Main.ColliderDepth.Value);
                if (!Main._trans_AnchorToTracking.Value) mirror.transform.SetParent(null);
                else mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                if (Main._trans_MirrorState.Value == "MirrorCutoutSolo" || Main._trans_MirrorState.Value == "MirrorTransparentSolo") MelonCoroutines.Start(FixMirrorLayer(childMirror, false));
                if (Main._trans_MirrorState.Value == "MirrorTransCutCombo") MelonCoroutines.Start(FixMirrorLayer(childMirror, true));
                if (Main._trans_followGaze.Value) MelonCoroutines.Start(followGazeTrans());

                Main._mirrorTrans = mirror;
            }
        }

        public static void ToggleMirrorCal(bool state)
        {
            //Main.Logger.Msg("ToggleMirrorCal");

            if (Main._mirrorCal != null && !state)
            {
                //.Msg("STate 1");
                ResetControllerLayer();
                try { UnityEngine.Object.Destroy(Main._mirrorCal); } catch (System.Exception ex) { Main.Logger.Msg(ConsoleColor.DarkRed, ex.ToString()); }
                Main._mirrorCal = null;

                if (Main._cal_hideOthers.Value)
                {
                    if (!Main._mirrorBase?.Equals(null) ?? false) Main._mirrorBase.SetActive(true);
                    if (!Main._mirror45?.Equals(null) ?? false) Main._mirror45.SetActive(true);
                    if (!Main._mirrorCeiling?.Equals(null) ?? false) Main._mirrorCeiling.SetActive(true);
                    if (!Main._mirrorMicro?.Equals(null) ?? false) Main._mirrorMicro.SetActive(true);
                    if (!Main._mirrorTrans?.Equals(null) ?? false) Main._mirrorTrans.SetActive(true);
                }
            }
            else if (Main._mirrorCal == null && state)
            {
                if (Main._cal_hideOthers.Value)
                {
                    if (!Main._mirrorBase?.Equals(null) ?? false) Main._mirrorBase.SetActive(false);
                    if (!Main._mirror45?.Equals(null) ?? false) Main._mirror45.SetActive(false);
                    if (!Main._mirrorCeiling?.Equals(null) ?? false) Main._mirrorCeiling.SetActive(false);
                    if (!Main._mirrorMicro?.Equals(null) ?? false) Main._mirrorMicro.SetActive(false);
                    if (!Main._mirrorTrans?.Equals(null) ?? false) Main._mirrorTrans.SetActive(false);
                }

                //Main.Logger.Msg("STate 2");
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
                if (Main.fixRenderOrder.Value || Main.usePixelLights.Value) MelonCoroutines.Start(SetOrder(mirror));
                mirror.transform.SetParent(GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]").transform, true);
                mirror.transform.SetParent(null); //Put cal mirror in DontDestroy Scene

                Main._mirrorCal = mirror;
            }
        }


        //Calibration mirror -- vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv

        public static void OnCalibrationBegin()
        {
            //Main.Logger.Msg("OnAvatarClear_Postfix");
            try
            {
                //Main.Logger.Msg("CAL STARTED");
                if (_calInit && Main._cal_enable.Value)
                {
                    if (calDelayRoutineOff != null) MelonCoroutines.Stop(calDelayRoutineOff);
                    ToggleMirrorCal(false);

                    if (Main._cal_DelayMirror.Value)
                        calDelayRoutine = MelonCoroutines.Start(DelayCalMirror());
                    else
                        waitForMeasureRoutine = MelonCoroutines.Start(WaitForMeasure());
                }
                //Main.Logger.Msg("CAL START Complete");
            }
            catch (System.Exception ex) { Main.Logger.Error("Error in OnCalibrationBegin:\n" + ex.ToString()); }
        }


        public static void OnCalibrationEnd()
        {
            //Main.Logger.Msg("OnSetupAvatarGeneral_Postfix");
            try
            {
                //Main.Logger.Msg("CAL END");
                if (calDelayRoutine != null) MelonCoroutines.Stop(calDelayRoutine);
                //Main.Logger.Msg("1");
                if (waitForMeasureRoutine != null) MelonCoroutines.Stop(waitForMeasureRoutine);
                //Main.Logger.Msg("2");
                if (Main._cal_DelayOff.Value)
                    calDelayRoutineOff = MelonCoroutines.Start(DelayCalMirrorOff());
                else
                    ToggleMirrorCal(false);
                //Main.Logger.Msg("CAL END Complete");
            }
            catch (System.Exception ex) { Main.Logger.Error("Error in OnCalibrationEnd:\n" + ex.ToString()); }
        }

        public static IEnumerator DelayCalMirror()
        {
            //Main.Logger.Msg("DelayCalMirror");
            yield return new WaitForSeconds(Main._cal_DelayMirrorTime.Value);
            waitForMeasureRoutine = MelonCoroutines.Start(WaitForMeasure());
            //Main.Logger.Msg("DelayCalMirror-Done");
        }

        public static IEnumerator DelayCalMirrorOff()
        {
            yield return new WaitForSeconds(Main._cal_DelayOffTime.Value);
            ToggleMirrorCal(false);
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
                        //Main.Logger.Msg("Setting layer to " + layer + " for " + Utils.GetPath(mesh.transform));
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
                //Main.Logger.Msg("Resetting layer to " + c.Value + " for " + Utils.GetPath(c.Key.transform));
                c.Key.layer = c.Value;
            }
        }

        public static IEnumerator WaitForMeasure()
        {
            //Main.Logger.Msg("WaitForMeasure");
            _calHeight = 0;
            var player = Utils.GetPlayer();
            var abortTime = Time.time + 10f;
            while (Time.time < abortTime && PlayerSetup.Instance._animator is null)
            {
                //Main.Logger.Msg("a n");
                yield return null;
            }
            var anim = PlayerSetup.Instance._animator;
            if (anim is null || !anim.isHuman)
            {
                //Main.Logger.Msg("Null || Not human");
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

                //Main.Logger.Msg("Height is " + height);
                _calHeight = height;
            }
            catch (Exception ex)
            {
                Main.Logger.Error("Error Measuring Height\n" + ex.ToString());
            }

            ToggleMirrorCal(true);
            //Main.Logger.Msg("WaitForMeasure-Done");

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

        //Calibration mirror -- ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

        public static IEnumerator followGazeBase()
        {
            Main.Logger.Msg($"EnterFollowGaze");
            var cam = Camera.main.gameObject;
            var player = Utils.GetPlayer();
            var mirror = Main._mirrorBase;
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
                //_mirrorCal.transform.rotation = tempRot;
                var step = Main.followGazeSpeed.Value * Time.deltaTime; // calculate distance to move
                mirror.transform.position = Vector3.MoveTowards(mirror.transform.position, tempPos, step);
                mirror.transform.rotation = Quaternion.RotateTowards(mirror.transform.rotation, tempRot, step * 40);

                yield return null;
            }
            _baseFollowGazeActive = false;
            Main.Logger.Msg($"ExitFollowGaze");
        }

        public static IEnumerator followGaze45()
        {
            Main.Logger.Msg($"EnterFollowGaze");
            var cam = Camera.main.gameObject;
            var player = Utils.GetPlayer();
            var mirror = Main._mirror45;
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

                var step = Main.followGazeSpeed.Value * Time.deltaTime; // calculate distance to move
                mirror.transform.position = Vector3.MoveTowards(mirror.transform.position, tempPos, step);
                mirror.transform.rotation = Quaternion.RotateTowards(mirror.transform.rotation, tempRot, step * 40);

                yield return null;
            }
            _45FollowGazeActive = false;
            Main.Logger.Msg($"ExitFollowGaze");
        }

        public static IEnumerator followGazeMicro()
        {
            Main.Logger.Msg($"EnterFollowGaze");
            var cam = Camera.main.gameObject;
            var mirror = Main._mirrorMicro;
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

                var step = Main.followGazeSpeed.Value * Time.deltaTime; // calculate distance to move
                mirror.transform.position = Vector3.MoveTowards(mirror.transform.position, tempPos, step);
                mirror.transform.rotation = Quaternion.RotateTowards(mirror.transform.rotation, tempRot, step * 40);

                yield return null;
            }
            _microFollowGazeActive = false;
            Main.Logger.Msg($"ExitFollowGaze");
        }

        public static IEnumerator followGazeTrans()
        {
            Main.Logger.Msg($"EnterFollowGaze");
            var cam = Camera.main.gameObject;
            var player = Utils.GetPlayer();
            var mirror = Main._mirrorTrans;
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
                //_mirrorCal.transform.rotation = tempRot;
                var step = Main.followGazeSpeed.Value * Time.deltaTime; // calculate distance to move
                mirror.transform.position = Vector3.MoveTowards(mirror.transform.position, tempPos, step);
                mirror.transform.rotation = Quaternion.RotateTowards(mirror.transform.rotation, tempRot, step * 40);

                yield return null;
            }
            _transFollowGazeActive = false;
            Main.Logger.Msg($"ExitFollowGaze");
        }

    }
}


