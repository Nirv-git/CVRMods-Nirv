using MelonLoader;
using UnityEngine;
using UIExpansionKit.API;
using ConsoleColor = System.ConsoleColor;
using System.Linq;


[assembly: MelonInfo(typeof(HeadLightMod.Main), "HeadLightMod", HeadLightMod.Main.versionStr, "Nirvash")]
[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonOptionalDependencies("BTKUILib")]

namespace HeadLightMod
{
    public class Main : MelonMod
    {
        public const string versionStr = "0.5";
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
        public static MelonPreferences_Entry<bool> BTKUILib_en;


        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("HeadLightMod", ConsoleColor.DarkYellow);

            MelonPreferences.CreateCategory("HeadLightMod", "HeadLight Mod");
            BTKUILib_en = MelonPreferences.CreateEntry<bool>("HeadLightMod", "BTKUILib_en", true, "BTKUILib Support (Requires Restart)");

            //var settings = ExpansionKitApi.GetSettingsCategory("HeadLightMod");
            //settings.AddSimpleButton("Head Light Toggle", (() => ToggleLight(!baseObj?.Equals(null) ?? false)));
            //settings.AddSimpleButton("Spot-Point Light", (() =>
            //{
            //    if (Config.lightType != LightType.Point)
            //        Config.lightType = LightType.Point;
            //    else
            //        Config.lightType = LightType.Spot;
            //    UpdateLight();
            //    Logger.Msg("poit/spot");
            //}));
            //settings.AddSimpleButton("Intensity +", (() => { Config.lightIntensity += .1f; UpdateLight(); Logger.Msg(Config.lightIntensity); }));
            //settings.AddSimpleButton("Intensity -", (() => { Config.lightIntensity = Utils.Clamp(Config.lightIntensity - .1f, 0, 1000); UpdateLight(); }));
            //settings.AddSimpleButton("Angle +", () => { Config.lightSpotAngle += 5f; UpdateLight(); });
            //settings.AddSimpleButton("Angle -", () => { Config.lightSpotAngle = Utils.Clamp(Config.lightSpotAngle - 5f, 0, 2000); UpdateLight(); });
            //settings.AddSimpleButton("Range +", () => { Config.lightRange += 1f; UpdateLight(); });
            //settings.AddSimpleButton("Range -", () => { Config.lightRange = Utils.Clamp(Config.lightRange - 1f, 0, 2000); UpdateLight(); });
            //settings.AddSimpleButton("Intensity Reset", () => { Config.lightIntensity = 1f; UpdateLight(); });
            //settings.AddSimpleButton("Angle Reset", () => { Config.lightSpotAngle = 40f; UpdateLight(); });
            //settings.AddSimpleButton("Range Reset", () => { Config.lightRange = 10f; UpdateLight(); });
            //settings.AddSimpleButton("White", () => { Config.lightColor = Color.white; UpdateLight(); });
            //settings.AddSimpleButton("Red", () => { Config.lightColor = Color.red; UpdateLight(); });
            //settings.AddSimpleButton("Blue", () => { Config.lightColor = Color.blue; UpdateLight(); });
            //settings.AddSimpleButton("Green", () => { Config.lightColor = Color.green; UpdateLight(); });
            //settings.AddSimpleButton("Magenta", () => { Config.lightColor = Color.magenta; UpdateLight(); });
            //settings.AddSimpleButton("Yellow", () => { Config.lightColor = Color.yellow; UpdateLight(); });
            //settings.AddSimpleButton("Cyan", () => { Config.lightColor = Color.cyan; UpdateLight(); });

            if (MelonHandler.Mods.Any(m => m.Info.Name == "BTKUILib") && BTKUILib_en.Value)
            {
                CustomBTKUI.InitUi();
            }
            else Logger.Msg("BTKUILib is missing, or setting is toggled off in Mod Settings - Not adding controls to BTKUILib");
        }

        public static void test2()
        {
            Logger.Msg("test2");
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



