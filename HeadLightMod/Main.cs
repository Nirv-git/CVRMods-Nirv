using MelonLoader;
using UnityEngine;
using ConsoleColor = System.ConsoleColor;

[assembly: MelonInfo(typeof(HeadLightMod.Main), "HeadLightMod", HeadLightMod.Main.versionStr, "Nirvash")]
[assembly: MelonGame(null, "ChilloutVR")]

namespace HeadLightMod
{
    public class Main : MelonMod
    {
        public const string versionStr = "0.6";
        public static MelonLogger.Instance Logger;
        public static class Config
        {
            static public LightType lightType = LightType.Spot;
            static public float lightRange = 10;
            static public float lightSpotAngle = 40;
            static public Color lightColor = Color.white;
            static public float lightIntensity = 1;
        }
        public static Light baseObj;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("HeadLightMod", ConsoleColor.DarkYellow);
            CustomBTKUI.InitUi();
        }

        public static void ToggleLight(bool state)
        {
            GameObject cam = Camera.main.gameObject;
            if (cam is null) return;

            if ((!baseObj?.Equals(null) ?? false) && !state) //If light isn't null and state is false, destroy
            {
                UnityEngine.Object.Destroy(baseObj);
                baseObj = null;
            }
            else
            {
                var _light = cam.AddComponent<Light>();
                _light.type = Config.lightType;
                _light.range = Config.lightRange; //Spot|Point
                _light.spotAngle = Config.lightSpotAngle; //Spot
                _light.color = Config.lightColor;
                _light.intensity = Config.lightIntensity;
                _light.shadows = LightShadows.None;
                _light.boundingSphereOverride = new Vector4(0, 0, 0, 4);
                _light.renderMode = LightRenderMode.ForcePixel;
                _light.useBoundingSphereOverride = true;
                baseObj = _light;
            }        
        }

        public static void UpdateLight()
        {
            if (!baseObj?.Equals(null) ?? false)
            {
                baseObj.type = Config.lightType;
                baseObj.range = Config.lightRange; //Spot|Point
                baseObj.spotAngle = Config.lightSpotAngle; //Spot
                baseObj.color = Config.lightColor;
                baseObj.intensity = Config.lightIntensity;
            }
        }
    }
}



