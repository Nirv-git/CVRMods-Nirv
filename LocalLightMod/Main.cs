using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using ABI.CCK.Components;


[assembly: MelonInfo(typeof(LocalLightMod.Main), "LocalLightMod", LocalLightMod.Main.versionStr, "Nirvash")]
[assembly: MelonGame(null, "ChilloutVR")]
[assembly: AssemblyVersion(LocalLightMod.Main.versionStr)]
[assembly: AssemblyFileVersion(LocalLightMod.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Cyan)]


namespace LocalLightMod
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.7.2";

        public static class Config
        {
            static public string name = "DefaultName";
            static public float width = .05f;
            static public float legnth = .1f;
            static public float height = .05f;
            static public bool pickupOrient = false;
            static public bool pickupable = true;
            static public LightType lightType = LightType.Point;
            static public float lightRange = 10;
            static public float lightSpotAngle = 30;
            static public Color lightColor = Color.white;
            static public float lightIntensity = 1;
            static public float lightBounceIntensity = 1;//Remove
            static public LightShadows lightShadows = LightShadows.None;
            static public float lightShadowStr = 1;
            static public int lightShadowCustRes = 2048;
            static public bool hideMeshRender = false;
            static public int cullingMask = -1;
        }

        public static List<GameObject> lightList = new List<GameObject>();
        public static Dictionary<GameObject, Texture2D> textDic = new Dictionary<GameObject, Texture2D>();
        public static GameObject activeLight;

        private const string catagory = "LocalLightMod";
        public static MelonPreferences_Category cat;

        public static MelonPreferences_Entry<bool> useNirvMiscPage;
        public static MelonPreferences_Entry<bool> loadDefaults;
        public static MelonPreferences_Entry<bool> textureLights;
        public static MelonPreferences_Entry<bool> updateActiveWithChange;

        public static MelonPreferences_Entry<string> savedColors;
        public static MelonPreferences_Entry<string> savedPrefs;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("LocalLightMod");

            cat = MelonPreferences.CreateCategory(catagory, "Local Light Mod");

            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), true, "BTKUI - Use 'NirvMisc' page instead of default 'Misc' page. (Restart req)");
            loadDefaults = MelonPreferences.CreateEntry(catagory, nameof(loadDefaults), false, "Load Slot 1 as Default");
            textureLights = MelonPreferences.CreateEntry(catagory, nameof(textureLights), true, "Texture Lights with Name");
            updateActiveWithChange = MelonPreferences.CreateEntry(catagory, nameof(updateActiveWithChange), false, "Update active light with every change");
            savedPrefs = MelonPreferences.CreateEntry(catagory, nameof(savedPrefs), "1,False,True,Point,10.,30.,1,1,1,1.,1.,Hard,1.,N/A,False,-1;2,False,True,Point,10.,30.,1,1,1,1.,1.,Hard,1.,N/A,False,-1;3,False,True,Point,10.,30.,1,1,1,1.,1.,Hard,1.,N/A,False,-1;4,False,True,Point,10.,30.,1,1,1,1.,1.,Hard,1.,N/A,False,-1;5,False,True,Point,10.,30.,1,1,1,1.,1.,Hard,1.,N/A,False,-1", "saved perfs", "", true);
            savedColors = MelonPreferences.CreateEntry(catagory, nameof(savedColors), "1,0.0,0.0,0.0;2,0.0,0.0,0.0;3,0.0,0.0,0.0;4,0.0,0.0,0.0;5,0.0,0.0,0.0;6,0.0,0.0,0.0", "saved colors", "", true);

            BTKUI_Cust.SetupUI();
            if (loadDefaults.Value) SaveSlots.LoadDefaultSlot();
        }


        public static void CreateLight()
        {
            var liName = Config.name;
            liName += Utils.RandomString(2);

            GameObject cam = Camera.main.gameObject;
            Vector3 pos = cam.transform.position + (cam.transform.forward * .25f); // Gets position of Head 
            GameObject _light = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _light.transform.position = pos;
            _light.transform.rotation = cam.transform.rotation;
            _light.name = liName;
            _light.transform.localScale = new Vector3((Config.width), (Config.height), Config.legnth);
            _light.GetOrAddComponent<BoxCollider>().size = new Vector3(1, 1, 1);
            _light.GetOrAddComponent<BoxCollider>().isTrigger = true;
            _light.GetOrAddComponent<MeshRenderer>().enabled = false;
            if (textureLights.Value)
            {
                var tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, ImageGen.ImageToPNG(ImageGen.DrawText(liName)));
                textDic.Add(_light, tex);
                _light.GetOrAddComponent<MeshRenderer>().material.SetTexture("_MainTex", tex);
            }
            else _light.GetOrAddComponent<MeshRenderer>().material.SetColor("_Color", Color.black);

            _light.GetOrAddComponent<CVRPickupObject>().maximumGrabDistance = 3f;
            _light.GetOrAddComponent<CVRPickupObject>().enabled = Config.pickupable;
            _light.GetOrAddComponent<CVRPickupObject>().gripType = !Config.pickupOrient ? CVRPickupObject.GripType.Free : CVRPickupObject.GripType.Origin;
            _light.GetOrAddComponent<Rigidbody>().useGravity = false;
            _light.GetOrAddComponent<Rigidbody>().isKinematic = true;
            _light.GetOrAddComponent<MeshRenderer>().enabled = !Config.hideMeshRender;

            _light.GetOrAddComponent<Light>().type = Config.lightType; // LightType.Point LightType.Directional LightType.Spot;
            _light.GetOrAddComponent<Light>().range = Config.lightRange; //Spot|Point
            _light.GetOrAddComponent<Light>().spotAngle = Config.lightSpotAngle; //Spot
            _light.GetOrAddComponent<Light>().color = Config.lightColor;
            _light.GetOrAddComponent<Light>().intensity = Config.lightIntensity;
            _light.GetOrAddComponent<Light>().shadows = Config.lightShadows;
            _light.GetOrAddComponent<Light>().shadowStrength = Config.lightShadowStr;
            _light.GetOrAddComponent<Light>().shadowCustomResolution = Config.lightShadowCustRes;
            _light.GetOrAddComponent<Light>().boundingSphereOverride = new Vector4(0, 0, 0, 500);
            _light.GetOrAddComponent<Light>().useBoundingSphereOverride = true;
            _light.GetOrAddComponent<Light>().cullingMask = Config.cullingMask;
            _light.GetOrAddComponent<Light>().flare = null;
            _light.GetOrAddComponent<Light>().renderMode = LightRenderMode.ForcePixel;

            lightList.Add(_light);
            activeLight = _light;
        }

        public static void UpdateLight(GameObject selLight)
        {
            if (!selLight?.Equals(null) ?? false)
            {
                var _light = selLight;
                _light.GetOrAddComponent<MeshRenderer>().enabled = !Config.hideMeshRender;
                _light.GetOrAddComponent<CVRPickupObject>().enabled = Config.pickupable;
                _light.GetOrAddComponent<CVRPickupObject>().gripType = !Config.pickupOrient ? CVRPickupObject.GripType.Free : CVRPickupObject.GripType.Origin;
                _light.GetOrAddComponent<Light>().type = Config.lightType; // LightType.Point LightType.Directional LightType.Spot;
                _light.GetOrAddComponent<Light>().range = Config.lightRange; //Spot|Point
                _light.GetOrAddComponent<Light>().spotAngle = Config.lightSpotAngle; //Spot
                _light.GetOrAddComponent<Light>().color = Config.lightColor;
                _light.GetOrAddComponent<Light>().intensity = Config.lightIntensity;
                _light.GetOrAddComponent<Light>().shadows = Config.lightShadows;
                _light.GetOrAddComponent<Light>().shadowStrength = Config.lightShadowStr;
                _light.GetOrAddComponent<Light>().shadowCustomResolution = Config.lightShadowCustRes;
                _light.GetOrAddComponent<Light>().cullingMask = Config.cullingMask;
            }
        }

        public static void LoadLightSettings(GameObject selLight)
        {
            if (!selLight?.Equals(null) ?? false)
            {
                var _light = selLight;
                Config.hideMeshRender = !_light.GetOrAddComponent<MeshRenderer>().enabled;
                Config.pickupOrient = (_light.GetOrAddComponent<CVRPickupObject>().gripType == CVRPickupObject.GripType.Free);
                Config.pickupable = _light.GetOrAddComponent<CVRPickupObject>().enabled;
                Config.lightType = _light.GetOrAddComponent<Light>().type;
                Config.lightRange = _light.GetOrAddComponent<Light>().range;
                Config.lightSpotAngle = _light.GetOrAddComponent<Light>().spotAngle;
                Config.lightColor = _light.GetOrAddComponent<Light>().color;
                Config.lightIntensity = _light.GetOrAddComponent<Light>().intensity;
                Config.lightShadows = _light.GetOrAddComponent<Light>().shadows;
                Config.lightShadowStr = _light.GetOrAddComponent<Light>().shadowStrength;
                Config.lightShadowCustRes = _light.GetOrAddComponent<Light>().shadowCustomResolution;
                Config.cullingMask = _light.GetOrAddComponent<Light>().cullingMask;

            }
        }

        public static string LightDetailsString(GameObject selLight)
        {
            if (!selLight?.Equals(null) ?? false)
            {
                var _light = selLight;
                return $"{_light.name} {_light.GetOrAddComponent<Light>().type} R:{_light.GetOrAddComponent<Light>().color.r}G:{_light.GetOrAddComponent<Light>().color.g}" +
                    $"B:{_light.GetOrAddComponent<Light>().color.b} Inten:{Utils.NumFormat(_light.GetOrAddComponent<Light>().intensity)} " +
                    $"Range:{Utils.NumFormat(_light.GetOrAddComponent<Light>().range)}";
            }
            return "Null";
        }

        public static void CleanupVisObjects()
        {
            foreach (var obj in lightList)
            {
                CleanupOneObject(obj, false);
            }
            lightList.Clear();
        }

        public static void CleanupOneObject(GameObject obj, bool clearAsGo = true)
        {
            if (!obj?.Equals(null) ?? false)
            {
                //Logger.Msg("Removing Object");
                if (clearAsGo) lightList.Remove(obj);
                UnityEngine.Object.Destroy(obj);
                if (textDic.TryGetValue(obj, out Texture2D tex))
                {
                    //Logger.Msg("Removing Texture");
                    textDic.Remove(obj);
                    if (!tex?.Equals(null) ?? false)
                        UnityEngine.Object.Destroy(tex);
                }
            }
        }
    }
}



