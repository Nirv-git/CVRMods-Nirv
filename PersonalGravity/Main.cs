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
using ABI_RC.Core.Player;
using ABI_RC.Systems.InputManagement;
using ABI_RC.Systems.GameEventSystem;
using ABI_RC.Systems.Gravity;
using ABI_RC.Core;
using BTKUILib;

[assembly: MelonInfo(typeof(PersonalGravity.Main), "PersonalGravity", PersonalGravity.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace PersonalGravity
{

    public class Main : MelonMod
    {
        public const string versionStr = "0.7.8";
        public static MelonLogger.Instance Logger;

        public static Main Instance;

        public static MelonPreferences_Entry<bool> useNirvMiscPage;
        public static MelonPreferences_Entry<bool> snapAngles;
        public static MelonPreferences_Entry<bool> autoToggle;
        public static MelonPreferences_Entry<bool> alignPlayer;
        public static MelonPreferences_Entry<bool> mixOverride;
        public static MelonPreferences_Entry<bool> effectOnlyPlayer;
        public static MelonPreferences_Entry<int> gravPriority;

        public static Vector3 gravDirection = Vector3.down;
        public static float gravStr = 9.81f;
        public static bool worldMovementModAllowed = false;

        public static GameObject _baseObj;
        public static System.Object handRayCast_Rout = null;
        public static GameObject handRayLine;
        public static int layermask = -1 & ~(1 << CVRLayers.PlayerClone) & ~(1 << CVRLayers.PlayerLocal) & ~(1 << CVRLayers.PlayerNetwork) & ~(1 << CVRLayers.CVRReserved3) & ~(1 << CVRLayers.CameraOnly) & ~(1 << CVRLayers.UI) & ~(1 << CVRLayers.UIInternal);


        public override void OnApplicationStart()
        {
            Instance = this;
            Logger = new MelonLogger.Instance("PersonalGravity");

            var cat = MelonPreferences.CreateCategory("PersonalGravity", "PersonalGravity");
            useNirvMiscPage = cat.CreateEntry(nameof(useNirvMiscPage), false, "BTKUI - Use 'NirvMisc' page instead of custom page. (Restart req)");
            snapAngles = cat.CreateEntry(nameof(snapAngles), true, "snapAngles", "", true);
            autoToggle = cat.CreateEntry(nameof(autoToggle), true, "autoToggle", "", true);
            alignPlayer = cat.CreateEntry(nameof(alignPlayer), true, "alignPlayer", "", true);
            mixOverride = cat.CreateEntry(nameof(mixOverride), true, "mixOverride", "True-Override | False-Additive", true);
            effectOnlyPlayer = cat.CreateEntry(nameof(effectOnlyPlayer), false, "effectOnlyPlayer", "True-Only Player | False-Player+Objects", true);
            gravPriority = cat.CreateEntry(nameof(gravPriority), 99999999, "Gravity Zone Priority (Default:99999999)", "", true);

            loadAssets();
            SetupEvents();
            BTKUI_Cust.SetupUI();
        }

        public override void OnPreferencesSaved()
        {
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)//Without switch this would run 3 times at world load
            {
                case 0: break;
                case 1: break;
                case 2: break;
                default:
                    worldMovementModAllowed = CVRWorld.Instance.allowFlying || CVRWorld.Instance.allowSpawnables;
                    break;
            }
        }


        public static void FindCamRayCast()
        {
            if (!MetaPort.Instance.isUsingVr)
            {//Desktop
                RaycastHit hit = new RaycastHit();
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                var layermask = -1 & ~(1 << CVRLayers.PlayerClone) & ~(1 << CVRLayers.PlayerLocal) & ~(1 << CVRLayers.PlayerNetwork) & ~(1 << CVRLayers.CVRReserved3) & ~(1 << CVRLayers.CameraOnly) & ~(1 << CVRLayers.UI) & ~(1 << CVRLayers.UIInternal);
                if (Physics.Raycast(ray, out hit, 500f, layermask))
                {
                    gravDirection = -hit.normal;
                    SetRotation();
                    if (autoToggle.Value && (_baseObj?.Equals(null) ?? true))
                        ToggleGrav();
                    //Logger.Msg($"dist:{hit.distance} x:{hit.point.x} y:{hit.point.y} z:{hit.point.z} norm_x:{hit.normal.x} norm_y:{hit.normal.y} norm_z:{hit.normal.z}");
                    //Logger.Msg($"collider.name:{hit.collider.name}");
                    //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //cube.transform.position = hit.point;
                    //cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
            else
            {//VR Hand Ray  
                if (handRayCast_Rout != null) MelonCoroutines.Stop(handRayCast_Rout);
                handRayCast_Rout = MelonCoroutines.Start(handRayCast());
            }
        }

        private static void SetupLineRender()
        {
            if (handRayLine?.Equals(null) ?? true) //usePickupLine
            {
                GameObject myLine = new GameObject();
                myLine.name = "PersonalGravityRayCastLine";
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
                lr.startColor = new Color(.25f, 0f, 1f, .75f);
                lr.endColor = new Color(.58f, .219f, 1f, .5f);
                lr.startWidth = .005f;
                lr.endWidth = 0.001f;
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, new Vector3(0f, 1f, 0f));
                handRayLine = myLine;
            }
        }

        public static IEnumerator handRayCast()
        {
            SetupLineRender();
            var startTime = Time.time;
            var rightCon = PlayerSetup.Instance.vrRayRight.gameObject;
            handRayLine.SetActive(true);
            var line = handRayLine.GetComponent<LineRenderer>();
            while (startTime + 30f > Time.time)
            {
                try
                {
                    if (CVRInputManager.Instance.gripLeftDown && CVRInputManager.Instance.gripRightDown)
                    {
                        handRayLine.SetActive(false);
                        break;
                    }

                    RaycastHit hit = new RaycastHit();
                    Ray ray = new Ray(rightCon.transform.position, rightCon.transform.forward);
                    if (Physics.Raycast(ray, out hit, 500f, layermask))
                    {
                        line.SetPosition(1, new Vector3(0f, hit.distance, 0f));
                        line.material.color = Color.white;
                        if (CVRInputManager.Instance.interactRightDown)
                        {
                            gravDirection = -hit.normal;
                            SetRotation();
                            if (autoToggle.Value && (_baseObj?.Equals(null) ?? true))
                                ToggleGrav();
                            handRayLine.SetActive(false);
                            break;
                        }
                    }
                    else
                    {
                        line.SetPosition(1, new Vector3(0f, 1f, 0f));
                        line.material.color = Color.grey;
                    }
                }
                catch (System.Exception ex) { Main.Logger.Error("Error in handRayCast - Aborting:\n" + ex.ToString()); yield break; }
                yield return null;
            }
            handRayLine.SetActive(false);
        }

        

        public static void ToggleGrav(bool enable = true)
        {
            if (!_baseObj?.Equals(null) ?? false)
            {
                UnityEngine.Object.Destroy(_baseObj);
                _baseObj = null;
            }
            else if (enable)
            {
                if (!worldMovementModAllowed)
                {
                    QuickMenuAPI.ShowAlertToast("Can not enabled personal gravity - World does not allow Flight or Props", 5);
                    return; 
                }
                GameObject baseObj = GameObject.Instantiate(gravPrefab);
                _baseObj = baseObj;
                _baseObj.transform.position = PlayerSetup.Instance.GetPlayerPosition();
                var zone = _baseObj.GetComponent<GravityZone>();
                zone.priority = gravPriority.Value;
                zone.strength = -gravStr;
                zone.playerGravityAlignmentMode = alignPlayer.Value ? GravitySystem.PlayerAlignmentMode.Auto : GravitySystem.PlayerAlignmentMode.Disabled;
                zone.gravityEffect = GetEffect();
                zone.gravityMix = mixOverride.Value ? GravityZone.GravityMix.Override : GravityZone.GravityMix.Additive;
                SetRotation();
            }
        }

        public static void SetRotation()
        {
            if (_baseObj != null)
            {
                _baseObj.transform.rotation = Quaternion.LookRotation(gravDirection);
            }
        }

        public static void SetStrength()
        {
            if (_baseObj != null)
            {
                _baseObj.GetComponent<GravityZone>().strength = -gravStr;
            }
        }

        public static void SetPriority()
        {
            if (_baseObj != null)
            {
                _baseObj.GetComponent<GravityZone>().priority = gravPriority.Value;
            }
        }

        public static void SetAlign()
        {
            if (_baseObj != null)
            {
                _baseObj.GetComponent<GravityZone>().playerGravityAlignmentMode = alignPlayer.Value ? GravitySystem.PlayerAlignmentMode.Auto : GravitySystem.PlayerAlignmentMode.Disabled;
            }
        }

        public static void SetEffect()
        {
            if (_baseObj != null)
            {
                _baseObj.GetComponent<GravityZone>().gravityEffect = GetEffect();
            }
        }

        public static GravityZone.GravityEffect GetEffect()
        {
            if (!CVRWorld.Instance.allowSpawnables)
                return GravityZone.GravityEffect.Players;
            if (effectOnlyPlayer.Value)
                return GravityZone.GravityEffect.Players;
            return GravityZone.GravityEffect.Players | GravityZone.GravityEffect.Objects;
        }

        public static void SetMix()
        {
            if (_baseObj != null)
            {
                _baseObj.GetComponent<GravityZone>().gravityMix = mixOverride.Value ? GravityZone.GravityMix.Override : GravityZone.GravityMix.Additive;
            }
        }

        private void loadAssets()
        {//https://github.com/ddakebono/BTKSASelfPortrait/blob/master/BTKSASelfPortrait.cs
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.personalgravity"))
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
                gravPrefab = assetBundle.LoadAsset<GameObject>("Personal_Gravity");
                gravPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
            else Logger.Error("Bundle was null");
        }

        public static AssetBundle assetBundle;
        public static GameObject gravPrefab;

        public static void SetupEvents()
        {

            CVRGameEventSystem.World.OnUnload.AddListener((message) =>
            {
                try
                {
                    Main.ToggleGrav(false);
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within CVRGameEventSystem.World.OnUnload!");
                    Logger.Error(e);
                }
            });
        }
    }
}


