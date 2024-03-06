using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Util;
using ABI.CCK.Components;
using HarmonyLib;
using ABI_RC.Core.UI;
using System.Threading;

namespace Fix_RotationBlockedAvatar
{
    [HarmonyPatch]
    internal class HarmonyPatches
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ABI_RC.Core.Player.PlayerAvatarMovementData), nameof(PlayerAvatarMovementData.WriteDataToAnimatorLerped))]
        internal static void AfterWriteDataToAnimatorLerped(
                                                              Animator animator,
                                                              Vector3 relativeHipRotation,
                                                              bool isBlocked,
                                                              bool isBlockedAlt,
                                                              PlayerAvatarMovementData previousData,
                                                              float progress,
                                                              PlayerAvatarMovementData __instance
                                                        )
        {
            if (Main.useFix.Value)
            {
                __instance._poseHandler.SetHumanPose(ref __instance._applyPosePast);
                Transform boneTransform = __instance._animator.GetBoneTransform(HumanBodyBones.Hips);
                if (!((UnityEngine.Object)boneTransform != (UnityEngine.Object)null))
                    return;
                boneTransform.position = Vector3.Lerp(previousData.BodyPosition, __instance.BodyPosition, progress);
                
                Quaternion targetRotation = Quaternion.Euler(CVRTools.SlerpEuler(previousData.BodyRotation, __instance.BodyRotation, progress));
                if (isBlocked && !isBlockedAlt)
                {
                    //Convert everything to Quaternion cause they are magic and fix everything
                    Quaternion relativeHipRotationQuat = Quaternion.Euler(relativeHipRotation);
                    Quaternion instanceRelativeHipRotationQuat = Quaternion.Euler(__instance.RelativeHipRotation);
                    // Calculate the rotation offset as a quaternion
                    Quaternion offsetRotation = Quaternion.Inverse(relativeHipRotationQuat) * instanceRelativeHipRotationQuat;
                    // Calculate the angle difference to scale the offset
                    float angleDifference = Quaternion.Angle(relativeHipRotationQuat, instanceRelativeHipRotationQuat);

                    // Determine if scaling is necessary based on the angle difference
                    if (angleDifference > 180f)
                    {
                        // Normalize the difference to a [0, 1] scale
                        float scaleFactor = Mathf.InverseLerp(0, 180, angleDifference);
                        // Scale down the offset based on the angle difference
                        offsetRotation = Quaternion.Slerp(Quaternion.identity, offsetRotation, scaleFactor);
                        // Apply the offset by multiplying the quaternions
                        targetRotation *= Quaternion.Inverse(offsetRotation);
                    }
                    else
                    {
                        targetRotation *= Quaternion.Inverse(offsetRotation);
                    }
                }
                boneTransform.rotation = targetRotation;
            }        
            return;
        }
    }
}
