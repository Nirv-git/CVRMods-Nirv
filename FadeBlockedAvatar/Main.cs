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
using ABI_RC.Core;
using TMPro;


[assembly: MelonInfo(typeof(FadeBlockedAvatar.Main), "FadeBlockedAvatar", FadeBlockedAvatar.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace FadeBlockedAvatar
{

    public class Main : MelonMod
    {
        public const string versionStr = "0.2";
        public static MelonLogger.Instance Logger;

        public static Main Instance;

        public static MelonPreferences_Entry<bool> enabled;
        public static MelonPreferences_Entry<float> avatarTrans;
        public static MelonPreferences_Entry<bool> useCustomColor;

        public static List<GameObject> blockedAvatars = new List<GameObject>();


        public override void OnApplicationStart()
        {
            Instance = this;
            Logger = new MelonLogger.Instance("FadeBlockedAvatar");

            var cat = MelonPreferences.CreateCategory("FadeBlockedAvatar", "FadeBlockedAvatar");

            enabled = cat.CreateEntry(nameof(enabled), true, "Mod Enabled (Must reload avatars)");
            avatarTrans = cat.CreateEntry(nameof(avatarTrans), .5f, "Transparency (0-1)");
            useCustomColor = cat.CreateEntry(nameof(useCustomColor), true, "Use Custom Color");

            avatarTrans.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { UpdateAvatarColors(); });
            useCustomColor.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { UpdateAvatarColors(); });
            loadAssets();
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
                    blockedAvatars.Clear();
                    break;
            }
        }


        public static void UpdateAvatarColors()
        {
            try
            {
                var transparency = Mathf.Max(Mathf.Min(avatarTrans.Value, 1f), 0f);

                foreach (var playerAv in blockedAvatars)
                {
                    if (!playerAv?.Equals(null) ?? false)
                    {
                        var meshRen = playerAv.GetComponentInChildren<SkinnedMeshRenderer>();
                        //Main.Logger.Msg($"meshRen: {meshRen}");

                        var material = meshRen.material;

                        Color avatarColor = Color.white;
                        if (useCustomColor.Value)
                            avatarColor = Utils.GetColorFromID(playerAv.transform.parent.name);
                        avatarColor = new Color(avatarColor.r, avatarColor.g, avatarColor.b, transparency);
                        material.color = avatarColor;

                        var textColor = new Color(avatarColor.r / 15, avatarColor.g / 15, avatarColor.b / 15, transparency / 2);
                        playerAv.GetComponentInChildren<TextMeshPro>().color = textColor;
                    }
                    else
                        blockedAvatars.Remove(playerAv);
                }
            }
            catch (System.Exception ex) { Main.Logger.Error("Error in UpdateAvatarColors\n" + ex.ToString()); }
   
        
        }
   

        public static void SetupAvatar(PuppetMaster puppet)
        {
            try
            {
                if (!enabled.Value)
                    return;
                //Main.Logger.Msg($"puppet.name:{puppet.transform.name}");
                //Main.Logger.Msg($"puppet._isBlocked:{puppet._isBlocked}");
                if (puppet.IsAvatarBlocked)
                {//do things
                    //Main.Logger.Msg($"was blocked");

                    var playerAv = puppet.transform.Find("[PlayerAvatar]");
                    //Main.Logger.Msg($"playerAv: {playerAv.name}");

                    var meshRen = playerAv.GetComponentInChildren<SkinnedMeshRenderer>();
                    //Main.Logger.Msg($"meshRen: {meshRen}");

                    var material = new Material(fadeAvatar);
                    material.mainTexture = meshRen.material.mainTexture;
                    meshRen.material = material;

                    var transparency = Mathf.Max(Mathf.Min(avatarTrans.Value, 1f), 0f);

                    Color avatarColor = Color.white;
                    if (useCustomColor.Value)
                        avatarColor = Utils.GetColorFromID(puppet.transform.name);
                    avatarColor = new Color(avatarColor.r, avatarColor.g, avatarColor.b, transparency);
                    material.color = avatarColor;

                    var textColor = new Color(avatarColor.r / 15, avatarColor.g / 15, avatarColor.b / 15, transparency / 2);
                    playerAv.GetComponentInChildren<TextMeshPro>().color = textColor;
        
                    if (!blockedAvatars.Contains(playerAv.gameObject))
                        blockedAvatars.Add(playerAv.gameObject);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("Error in SetupAvatar \n" + ex.ToString());
            }
        }

        private void loadAssets()
        {//https://github.com/ddakebono/BTKSASelfPortrait/blob/master/BTKSASelfPortrait.cs
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FadeBlockedAvatar.fadeavatar"))
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
                fadeAvatar = assetBundle.LoadAsset<Material>("Fade_Avatar");
                fadeAvatar.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }
            else Logger.Error("Bundle was null");
        }

        public static AssetBundle assetBundle;
        public static Material fadeAvatar;
    }
}


