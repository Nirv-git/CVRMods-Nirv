using MelonLoader;
using UnityEngine;
using ConsoleColor = System.ConsoleColor;

[assembly: MelonInfo(typeof(HeadLightMod.Main), "HeadLightMod", HeadLightMod.Main.versionStr, "Nirvash")]
[assembly: MelonGame(null, "ChilloutVR")]

namespace HeadLightMod
{
    public class Main : MelonMod
    {
        public const string versionStr = "0.7.1";
        public static MelonLogger.Instance Logger;
        public static MelonPreferences_Category cat;
        private const string catagory = "HeadLightMod";
        public static MelonPreferences_Entry<bool> useNirvMiscPage;
        public static MelonPreferences_Entry<LightType> lightType;
        public static MelonPreferences_Entry<float> lightRange;
        public static MelonPreferences_Entry<float> lightSpotAngle;
        public static MelonPreferences_Entry<Color> lightColor;
        public static MelonPreferences_Entry<float> lightIntensity;


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

            cat = MelonPreferences.CreateCategory(catagory, "HeadLightMod Settings");
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page. (Restart req)");
            lightType = MelonPreferences.CreateEntry(catagory, nameof(lightType), LightType.Spot, "Light Type", "", true);
            lightRange = MelonPreferences.CreateEntry(catagory, nameof(lightRange), 10f, "light Range", "", true);
            lightSpotAngle = MelonPreferences.CreateEntry(catagory, nameof(lightSpotAngle), 40f, "light Spot Angle", "", true);
            lightColor = MelonPreferences.CreateEntry(catagory, nameof(lightColor), Color.white, "light Color", "", true);
            lightIntensity = MelonPreferences.CreateEntry(catagory, nameof(lightIntensity), 1f, "light Intensity", "", true);

            Config.lightType = lightType.Value;
            Config.lightRange = lightRange.Value;
            Config.lightSpotAngle = lightSpotAngle.Value;
            Config.lightColor = lightColor.Value;
            Config.lightIntensity = lightIntensity.Value;

            CustomBTKUI.InitUi();
        }

        public static void UpdatePrefs() 
        {
            lightType.Value = Config.lightType;
            lightRange.Value = Config.lightRange;
            lightSpotAngle.Value = Config.lightSpotAngle;
            lightColor.Value = Config.lightColor;
            lightIntensity.Value = Config.lightIntensity;
        }


        public static void ToggleLight(bool state)
        {
            UpdatePrefs();
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
            UpdatePrefs();
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



