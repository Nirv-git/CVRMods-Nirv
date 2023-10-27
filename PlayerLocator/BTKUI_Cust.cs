using System.Reflection;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using BTKUILib;
using BTKUILib.UIObjects;
using MelonLoader;

namespace PlayerLocator
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("PlayerLocator", "playLoc-RemoveLine", Assembly.GetExecutingAssembly().GetManifestResourceStream("PlayerLocator.Icons.Remove Line.png"));
            QuickMenuAPI.PrepareIcon("PlayerLocator", "playLoc-SeveralUsers", Assembly.GetExecutingAssembly().GetManifestResourceStream("PlayerLocator.Icons.SeveralUsers.png"));
            QuickMenuAPI.PrepareIcon("PlayerLocator", "playLoc-Single Player", Assembly.GetExecutingAssembly().GetManifestResourceStream("PlayerLocator.Icons.Single Player.png"));
        }


        public static void InitUi()
        {
            loadAssets();
            Category cat = QuickMenuAPI.PlayerSelectPage.AddCategory("Player Locator", "PlayerLocator");

            cat.AddButton($"Create line to user", "playLoc-Single Player", "Creates a line towards this user<p>From your right hand in VR").OnPress += () =>
            {
                try
                {
                    var obj = GameObject.Find($"{QuickMenuAPI.SelectedPlayerID}/[PlayerAvatar]")?.transform?.GetChild(0)?.gameObject;
                    if (obj == null)
                    {
                        QuickMenuAPI.ShowAlertToast("An error occurred", 2);
                        return;
                    }
                    Main.LineObj(obj, QuickMenuAPI.SelectedPlayerID);

                    var cam = Camera.main.transform;
                    var dist = Vector3.Distance(obj.transform.position, cam.position);
                    QuickMenuAPI.ShowAlertToast($"Player at X:{obj.transform.position.x:F1} Y:{obj.transform.position.y:F1} Z:{obj.transform.position.z:F1}, Your at X:{cam.position.x:F1} Y:{cam.position.y:F1} Z:{cam.position.z:F1}<p>{dist:F1}m from you", 5);
                }
                catch (System.Exception ex) { Main.Logger.Error($"Error creating line to one user\n" + ex.ToString()); }
            };

            cat.AddButton($"Create line to ALL users", "playLoc-SeveralUsers", "Creates a line towards ALL users in the instance<p>From your right hand in VR").OnPress += () =>
            {
                try
                {
                    foreach (var avatar in GameObject.FindObjectsOfType<ABI.CCK.Components.CVRAvatar>(true))
                    {
                        var obj = avatar.gameObject;
                        var scene = obj.scene.name;
                        if (obj == null || scene == "DontDestroyOnLoad" || scene == "HideAndDontSave")
                        {
                            continue;
                        }
                        Main.LineObj(obj, obj.transform.parent.parent.name);
                    }
                }
                catch (System.Exception ex) { Main.Logger.Error($"Error creating line to all users\n" + ex.ToString()); }
            };

            cat.AddButton($"Remove Lines Now", "playLoc-RemoveLine", "Remove all lines").OnPress += () =>
            {
                try
                {
                    Main.LineKillNow = true;
                    MelonCoroutines.Start(Main.LineKillReset());
                }
                catch (System.Exception ex) { Main.Logger.Error($"Error removing lines\n" + ex.ToString()); }
            };

        }

    }

}