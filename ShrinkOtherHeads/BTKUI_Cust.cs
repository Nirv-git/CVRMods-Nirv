using System.Reflection;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using BTKUILib;
using BTKUILib.UIObjects;
using MelonLoader;
using ABI.CCK.Components;

namespace ShrinkOtherHeads
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("ShrinkOtherHeads", "shrinkother-Head", Assembly.GetExecutingAssembly().GetManifestResourceStream("ShrinkOtherHeads.Icons.Head.png"));
            QuickMenuAPI.PrepareIcon("ShrinkOtherHeads", "shrinkother-HeadShrink", Assembly.GetExecutingAssembly().GetManifestResourceStream("ShrinkOtherHeads.Icons.HeadShrink.png"));
        }

        public static Dictionary<string, Vector3> HeadScales = new Dictionary<string, Vector3>();


        public static void InitUi()
        {
            loadAssets();
            Category cat = QuickMenuAPI.PlayerSelectPage.AddCategory("Shrink Other Heads", "ShrinkOtherHeads");

            cat.AddButton($"Shrink Head", "shrinkother-HeadShrink", "Shrinks Player's Head to 0").OnPress += () =>
            {
                try
                {
                    var obj = GameObject.Find($"{QuickMenuAPI.SelectedPlayerID}/[PlayerAvatar]")?.transform?.GetChild(0)?.gameObject;
                    if (obj == null)
                    {
                        QuickMenuAPI.ShowAlertToast("Error - Player not found", 2);
                        return;
                    }
                    var anim = obj.GetComponent<Animator>();
                    var id = obj.GetComponent<CVRAssetInfo>()?.objectId;
                    if (anim == null || !anim.isHuman || id == null)
                    {
                        QuickMenuAPI.ShowAlertToast("Error - No Animator or not Human or ID Null", 2);
                        return;
                    }
                    var trans = anim.GetBoneTransform(HumanBodyBones.Head).transform;
                    if (trans == null)
                    {
                        QuickMenuAPI.ShowAlertToast("Error - No head Transform", 2);
                        return;
                    }
                    if(trans.localScale != Vector3.zero)
                    {
                        if (!HeadScales.ContainsKey(id))
                            HeadScales.Add(id, trans.localScale);

                        trans.localScale = Vector3.zero;
                        QuickMenuAPI.ShowAlertToast($"Player's head has been shrunk", 2);
                    }
                }
                catch (System.Exception ex) { Main.Logger.Error($"Error shrinking head\n" + ex.ToString()); }
            };

            cat.AddButton($"Unshrink Head", "shrinkother-Head", "Reset's Player's Head Scale").OnPress += () =>
            {
                try
                {
                    var obj = GameObject.Find($"{QuickMenuAPI.SelectedPlayerID}/[PlayerAvatar]")?.transform?.GetChild(0)?.gameObject;
                    if (obj == null)
                    {
                        QuickMenuAPI.ShowAlertToast("Error - Player not found", 2);
                        return;
                    }
                    var anim = obj.GetComponent<Animator>();
                    var id = obj.GetComponent<CVRAssetInfo>()?.objectId;
                    if (anim == null || !anim.isHuman || id == null)
                    {
                        QuickMenuAPI.ShowAlertToast("Error - No Animator or not Human or ID Null", 2);
                        return;
                    }
                    var trans = anim.GetBoneTransform(HumanBodyBones.Head).transform;
                    if (trans == null)
                    {
                        QuickMenuAPI.ShowAlertToast("Error - No head Transform", 2);
                        return;
                    }
                    if (trans.localScale == Vector3.zero)
                    {
                        if (!HeadScales.ContainsKey(id))
                        {
                            QuickMenuAPI.ShowAlertToast("Error - No Previous Scale Saved", 2);
                            return;
                        }
                        trans.localScale = HeadScales[id];
                        HeadScales.Remove(id);
                        QuickMenuAPI.ShowAlertToast($"Player's head has been unshrunk", 2);
                    }
                }
                catch (System.Exception ex) { Main.Logger.Error($"Error shrinking head\n" + ex.ToString()); }
            };

        }

    }

}