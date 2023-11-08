using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core;
using ABI_RC.Core.Player;
using HarmonyLib;
using ABI_RC.Systems.MovementSystem;
using ABI_RC.Core.Util;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(NoHeadShrinkMod.Main), "NoHeadShrinkMod", NoHeadShrinkMod.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(NoHeadShrinkMod.Main.versionStr)]
[assembly: AssemblyFileVersion(NoHeadShrinkMod.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Yellow)]
[assembly: MelonOptionalDependencies("BTKUILib")]

namespace NoHeadShrinkMod
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.5.7";

        public static MelonPreferences_Category cat;
        private const string catagory = "NoHeadShrinkMod";
        public static MelonPreferences_Entry<bool> disableHeadShrink;
        public static MelonPreferences_Entry<bool> unshrinkAtDistance;
        public static MelonPreferences_Entry<float> unshrinkDistance;
        public static MelonPreferences_Entry<bool> scaleDistance;

        public static MelonPreferences_Entry<bool> BTKUILib_en;
        public static MelonPreferences_Entry<bool> useNirvMiscPage;

        public static bool init = false;

        public static Transform Head = null;
        public static float scaleAdj = 1f;

        public static bool alertFlag = false;

        public static System.Object recheckRoutine = null;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("NoHeadShrinkMod", ConsoleColor.DarkYellow);
            cat = MelonPreferences.CreateCategory(catagory, "NoHeadShrinkMod");
            disableHeadShrink = MelonPreferences.CreateEntry(catagory, nameof(disableHeadShrink), false, "Disable head shrink always");
            unshrinkAtDistance = MelonPreferences.CreateEntry(catagory, nameof(unshrinkAtDistance), false, "Unshrink if camera is x distance from head");
            unshrinkDistance = MelonPreferences.CreateEntry(catagory, nameof(unshrinkDistance), .5f, "Distance");
            scaleDistance = MelonPreferences.CreateEntry(catagory, nameof(scaleDistance), true, "Scale distance based on avatar height (Height*Distance)");

            BTKUILib_en = MelonPreferences.CreateEntry(catagory, nameof(BTKUILib_en), true, "BTKUILib Support (Requires Restart)");
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page.");

            scaleDistance.OnValueChanged += UpdateScale;

            if (MelonHandler.Mods.Any(m => m.Info.Name == "BTKUILib") && BTKUILib_en.Value)
            {
                CustomBTKUI.InitUi();
                disableHeadShrink.OnValueChanged += CustomBTKUI.ChangeButtons;
                unshrinkAtDistance.OnValueChanged += CustomBTKUI.ChangeButtons;
                unshrinkDistance.OnValueChanged += CustomBTKUI.ChangeButtons;
                scaleDistance.OnValueChanged += CustomBTKUI.ChangeButtons;
            }
            else Logger.Msg("BTKUILib is missing, or setting is toggled off in Mod Settings - Not adding controls to BTKUILib");

        }

        public static void UpdateScale(bool newValue, bool oldValue)
        {
            if (init)
                FindScale();
        }

        internal static void FindScale()
        {
            //Logger.Msg("OnAnimatorManagerUpdate");
            init = true;
            Head = null;

            if ((!PlayerSetup.Instance._avatar.GetComponent<Animator>()?.avatar?.isHuman ?? true) || PlayerSetup.Instance._avatar.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head) == null)
            {
                Logger.Msg("Animator is null, or is not Humanoid, this avatar will not function with the mod");
                return;
            }

            Animator anim = PlayerSetup.Instance._avatar.GetComponent<Animator>();
            
            if (scaleDistance.Value)
            {
                try
                {
                    var height = Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.LeftFoot).position, anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position) +
                        Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position, anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position) +
                        Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position, anim.GetBoneTransform(HumanBodyBones.Hips).position) +
                        Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.Hips).position, anim.GetBoneTransform(HumanBodyBones.Spine).position) +
                        Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.Spine).position, anim.GetBoneTransform(HumanBodyBones.Chest).position) +
                        Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.Chest).position, anim.GetBoneTransform(HumanBodyBones.Neck).position) +
                        Vector3.Distance(anim.GetBoneTransform(HumanBodyBones.Neck).position, anim.GetBoneTransform(HumanBodyBones.Head).position);
                    Logger.Msg($"Height:{height:F2}, Dist:{unshrinkDistance.Value}, Scaled distance will be {unshrinkDistance.Value * height:F2}");
                    scaleAdj = height;
                }
                catch (Exception ex) { scaleAdj = 1f; Logger.Error("Error Measuring Height defaulting to 1\n" + ex.ToString()); }

                if (recheckRoutine != null) MelonCoroutines.Stop(recheckRoutine);
                recheckRoutine = MelonCoroutines.Start(RecheckScale());
            }
            else
                scaleAdj = 1f;

            Head = anim.GetBoneTransform(HumanBodyBones.Head);
        }

        public static IEnumerator RecheckScale()
        { //Check scale and remeasure avatar if needed
            var lastScale = PlayerSetup.Instance._avatar.transform.localScale;
            while (scaleDistance.Value)
            {
                if (PlayerSetup.Instance?._avatar?.transform.Equals(null) ?? true)
                    yield break;
                var curScale = PlayerSetup.Instance._avatar.transform.localScale;
                if (lastScale != curScale)
                {
                    var newAdj = (curScale.y / lastScale.y) * scaleAdj;
                    Logger.Msg($"Remeasure due to scale changes | Old Height:{scaleAdj:F2}, New:{newAdj:F2} | Dist:{unshrinkDistance.Value}, Old Scaled distance: {unshrinkDistance.Value * scaleAdj:F2}, New: {unshrinkDistance.Value * newAdj:F2}");
                    scaleAdj = newAdj;
                    lastScale = curScale;
                }
                yield return new WaitForSeconds(1.25f);
            }
        }
    }
   

    [HarmonyPatch]
    internal class HarmonyPatches
    {
        // Avatar
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MovementSystem), nameof(MovementSystem.UpdateAnimatorManager))]
        internal static void AfterUpdateAnimatorManager(CVRAnimatorManager manager)
        {
            Main.alertFlag = false;
            Main.FindScale();
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(TransformHiderForMainCamera), nameof(TransformHiderForMainCamera.IsMainCamera))]
        public static bool OnIsMainCamera(ref bool __result)
        {
            if (Main.disableHeadShrink.Value)
            {
                if (!Main.alertFlag)
                {
                    Main.Logger.Msg(ConsoleColor.Yellow, "Head unscaled for this avatar due to toggle");
                    Main.alertFlag = true;
                }
                __result = false;
                return false;
            }
            if (Main.unshrinkAtDistance.Value && Main.Head != null && Vector3.Distance(PlayerSetup.Instance.GetActiveCamera().transform.position, Main.Head.position) > Main.unshrinkDistance.Value * Main.scaleAdj)
            {
                if (!Main.alertFlag) //To reduce support requests of 'I can see my head' Yell at the user that a mod is causing it
                {
                    Main.Logger.Msg(ConsoleColor.Yellow, "Head unscaled for this avatar due to distance");
                    Main.alertFlag = true;
                }
                __result = false;
                return false;
            }
            return true;
        }
    }
}



