using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using ABI_RC.Core.Savior;
using System.Security.Cryptography;
using System.Text;
using ABI_RC.Core.Player;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(PlayerLocator.Main), "PlayerLocator", PlayerLocator.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(PlayerLocator.Main.versionStr)]
[assembly: AssemblyFileVersion(PlayerLocator.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.DarkCyan)]


namespace PlayerLocator
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.7.5";

        public static MelonPreferences_Category cat;
        private const string catagory = "PlayerLocator";
        public static MelonPreferences_Entry<int> lineLifespan;

        public static bool LineKillNow = false;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("PlayerLocator", ConsoleColor.DarkCyan);

            cat = MelonPreferences.CreateCategory(catagory, "PlayerLocator");
            lineLifespan = MelonPreferences.CreateEntry(catagory, nameof(lineLifespan), 7, "How long the line render should last (max 30)");
            CustomBTKUI.InitUi();
        }


        public static void LineObj(GameObject obj, string id)
        {
            try
            {
                var lineObj = SetupLineRender(id);
                lineObj.GetComponent<LineRenderer>().SetPosition(1, lineObj.transform.InverseTransformPoint(obj.transform.position));
                lineObj.SetActive(true);
                MelonCoroutines.Start(LineKill(obj, lineObj));
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error when creating LineObj\n" + ex.ToString()); }
        }

        public static IEnumerator LineKill(GameObject obj, GameObject lineObj)
        {
            var time = Time.time + Mathf.Min(lineLifespan.Value, 30f);
            while (Time.time <= time && (!obj?.Equals(null) ?? false) && !LineKillNow)
            {
                lineObj.GetComponent<LineRenderer>().SetPosition(0, lineObj.transform.position);
                lineObj.GetComponent<LineRenderer>().SetPosition(1, obj.transform.position);
                yield return null;
            }
            lineObj.SetActive(false);
            GameObject.Destroy(lineObj);
            //LineKillNow = false;
        }

        public static IEnumerator LineKillReset()
        {
            var time = Time.time + .5f;
            while (Time.time <= time)
            {
                yield return null;
            }
            LineKillNow = false;
        }

        private static GameObject SetupLineRender(string id)
        {

            GameObject start = MetaPort.Instance.isUsingVr ? PlayerSetup.Instance.vrRayRight.gameObject : Camera.main.gameObject;
            GameObject myLine = new GameObject();
            myLine.name = $"PlayerLocator-Line-{id}";
            myLine.transform.SetParent(start.transform);
            myLine.transform.localPosition = MetaPort.Instance.isUsingVr ? Vector3.zero : new Vector3(0f, -.1f, 0f);
            myLine.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Standard Unlit"));
            lr.useWorldSpace = true;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
            var color = GetColorFromID(id);
            lr.startColor = color;
            lr.endColor = color - new Color(.25f, .25f, .25f);
            lr.startWidth = .0099f;
            lr.endWidth = 0.005f;
            lr.SetPosition(0, myLine.transform.position);
            lr.SetPosition(1, new Vector3(0f, 1f, 0f));
            return myLine;
            
        }

        private static Color GetColorFromID(string id)
        {
            float minBrightness = 0.5f;
            byte[] hashBytes;
            using (MD5 md5Hash = MD5.Create())
            {
                hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(id));
            }
            // Use parts of the hash to generate RGB values.
            float r = hashBytes[0] / 255.0f;
            float g = hashBytes[1] / 255.0f;
            float b = hashBytes[2] / 255.0f;
            // Ensure the brightness is above the minimum threshold.
            float brightness = (r + g + b) / 3f;
            if (brightness < minBrightness)
            {
                // Adjust the color to meet the minimum brightness.
                float delta = (minBrightness - brightness) / 3f;
                r += delta;
                g += delta;
                b += delta;
            }
            return new Color(r, g, b);
        }

    }
}








