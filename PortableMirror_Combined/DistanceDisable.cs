using System;
using System.Collections.Generic;
using System.Collections;
using MelonLoader;
using UnityEngine;
using ABI.CCK.Components;

namespace PortableMirror
{
    internal class DistanceDisable
    {
        private static int nearLayer = 10; 
        private static int farLayer = 4;

        public static object distDisableRoutine = null;
        public static bool distActive = false;
        public static bool distWasActive = false;


        public static IEnumerator DistDisableUpdate()
        {
            //Main.Logger.Msg($"start dist rout");
            distActive = true;
            distWasActive = true;
            var self = GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]");
            //Main.Logger.Msg($"player {self.gameObject.name}");
            while (Main.distanceDisable.Value)
            {
                var avatars = GameObject.FindObjectsOfType<CVRAvatar>();
                //Main.Logger.Msg($"got {avatars.Length}");
              
                for (int i = 0; i < avatars.Length; i++)
                {
                    try
                    {
                        var avatar = avatars[i];
                        if (avatar.transform.IsChildOf(self.transform))
                            continue;

                        int setToLayer = 0;
                        //Main.Logger.Msg($"dist {Vector3.Distance(self.transform.position, avatar.transform.position)}");
                        if (Mathf.Abs(Vector3.Distance(self.transform.position, avatar.transform.position)) > Main.distanceValue.Value)
                            setToLayer = farLayer;
                        else
                            setToLayer = nearLayer;
                        //Main.Logger.Msg($"setto {setToLayer}");
                        if (avatar.gameObject.layer != setToLayer)
                        {
                            //Main.Logger.Msg($"{avatar.gameObject.name} to {setToLayer}");
                            avatar.gameObject.layer = setToLayer;
                            var childMeshs = avatar.gameObject.GetComponentsInChildren<MeshRenderer>();
                            for (int meshI = 0; meshI < childMeshs.Length; meshI++)
                            {
                                childMeshs[meshI].gameObject.layer = setToLayer;
                            }
                            var childSkinnedMeshs = avatar.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                            for (int meshSkinI = 0; meshSkinI < childSkinnedMeshs.Length; meshSkinI++)
                            {
                                childSkinnedMeshs[meshSkinI].gameObject.layer = setToLayer;
                            }
                        }
                    }
                    catch (System.Exception ex) { Main.Logger.Msg($"Error for {i} DistDisableUpdate\n" + ex.ToString()); }
                }
                yield return new WaitForSeconds(Main.distanceUpdateInit.Value);
            }
            distActive = false;
        }

        public static void ReturnAllPlayersToNormal()
        {
            if (distActive)
                MelonCoroutines.Stop(distDisableRoutine);
            distActive = false;

            var self = GameObject.Find("_PLAYERLOCAL/[PlayerAvatar]");
            var avatars = GameObject.FindObjectsOfType<CVRAvatar>();
            for (int i = 0; i < avatars.Length; i++)
            {
                try
                {
                    var avatar = avatars[i];
                    if (avatar.transform.IsChildOf(self.transform))
                        continue;

                    int setToLayer = nearLayer;
                        
                    if (avatar.gameObject.layer != setToLayer)
                    {
                        avatar.gameObject.layer = setToLayer;
                        var childMeshs = avatar.gameObject.GetComponentsInChildren<MeshRenderer>();
                        for (int meshI = 0; meshI < childMeshs.Length; meshI++)
                        {
                            childMeshs[meshI].gameObject.layer = setToLayer;
                        }
                        var childSkinnedMeshs = avatar.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                        for (int meshSkinI = 0; meshSkinI < childSkinnedMeshs.Length; meshSkinI++)
                        {
                            childSkinnedMeshs[meshSkinI].gameObject.layer = setToLayer;
                        }
                    }
                }
                catch (System.Exception ex) { Main.Logger.Msg($"Error for {i} ReturnAllPlayersToNormal\n" + ex.ToString()); }
            }
        }
    }
}
