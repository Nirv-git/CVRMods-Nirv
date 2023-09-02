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
using HighlightPlus;
using UnityEngine.Networking;
using BTKUILib;
using System.IO;
using System.Threading;
using ABI_RC.Core.Networking;



[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(WorldPropListMod.Main), "WorldPropListMod", WorldPropListMod.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(WorldPropListMod.Main.versionStr)]
[assembly: AssemblyFileVersion(WorldPropListMod.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Cyan)]

namespace WorldPropListMod
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.5.14";

        public static MelonPreferences_Category cat;
        private const string catagory = "WorldPropListMod";
        public static MelonPreferences_Entry<bool> useNirvMiscPage;
        public static MelonPreferences_Entry<int> lineLifespan;
        public static MelonPreferences_Entry<int> onPropDetailSelect;
        public static MelonPreferences_Entry<bool> usePropBlockList;
        public static MelonPreferences_Entry<bool> showHUDNotification;
        public static MelonPreferences_Entry<bool> printAPIrequestsToConsole;

        public static Main Instance;
        private Thread _mainThread;
        public Queue<Action> MainThreadQueue = new Queue<Action>();

        public static GameObject lastHighlight, LineRen;
        public static bool init = false;

        public static System.Object lineRenRoutine = null;
        public static bool LineWaitKill = false;
        public static bool LineKillNow = false;

        public static int APIwaitingCount = 0;

        public static Dictionary<string, string> blockedProps = new Dictionary<string, string>(); //GUID, Name
        public static Dictionary<string, (string, string, string, bool, string, string, string, DateTime)> PropNamesCache = new Dictionary<string, (string, string, string, bool, string, string, string, DateTime)>(); //propGUID,(Name,AuthorName,tags, isPub, FileSize, UpdatedAt, DescriptionCacheDate)
        public static Dictionary<string, (string, DateTime)> PlayerNamesCache = new Dictionary<string, (string, DateTime)>(); //playerGUID,(Name,CacheDate)

        public static readonly List<(string, string, string)> BlockedThisSession = new List<(string, string, string)>(); //propGUID,PlayerGUID,Time
        public static readonly List<(string, string, string)> PropsThisSession = new List<(string, string, string)>(); //propGUID,PlayerGUID,Time

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("WorldPropListMod", ConsoleColor.Blue);
            Instance = this;
            _mainThread = Thread.CurrentThread;

            cat = MelonPreferences.CreateCategory(catagory, "WorldPropListMod");
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), false, "BTKUI - Use 'NirvMisc' page instead of custom page. (Restart req)");
            lineLifespan = MelonPreferences.CreateEntry(catagory, nameof(lineLifespan), 7, "How long the line render should last (max 30)");
            onPropDetailSelect = MelonPreferences.CreateEntry(catagory, nameof(onPropDetailSelect), 3, "0-None, 1-Highlight, 2-Line, 3-Both");
            usePropBlockList = MelonPreferences.CreateEntry(catagory, nameof(usePropBlockList), true, "Use prop block list to prevent prop loading");
            showHUDNotification = MelonPreferences.CreateEntry(catagory, nameof(showHUDNotification), true, "Show notification on HUD when a prop is blocked");
            printAPIrequestsToConsole = MelonPreferences.CreateEntry(catagory, nameof(printAPIrequestsToConsole), false, "Prints logging of API requests to console");
            SaveLoad.InitFileListOrLoad();
            BTKUI_Cust.SetupUI();      
        }
        public override void OnPreferencesSaved()
        {
            if(init) SaveLoad.SaveListFiles();
        }

        public override void OnApplicationQuit()
        {
            Main.Logger.Msg("Saving files");
            SaveLoad.SaveListFiles();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)
            {
                case 0: break;
                case 1: break;
                case 2: break;
                default:
                    if(!init)
                    {
                        init = true;
                        //Logger.Msg($"MetaPort.Instance.ownerId {MetaPort.Instance.ownerId} - MetaPort.Instance.username {MetaPort.Instance.username}");
                        PlayerNamesCache[MetaPort.Instance.ownerId] = (AuthManager.username, DateTime.Now);
                        PlayerNamesCache["SYSTEM"] = ("SYSTEM", DateTime.Now); PlayerNamesCache["LocalServer"] = ("LocalServer", DateTime.Now);
                        PropNamesCache["incompatible-content"] = ("Error prop", "CVR", "", false, "", "", "Internal Error Prop for CVR\nID:incompatible-content", DateTime.Now);
                    }
                    break;
            }
        }
        public override void OnUpdate()
        {
            //Fire any queued actions on main thread
            if (MainThreadQueue.Count > 0)
                MainThreadQueue.Dequeue().Invoke();
        }
        public bool IsOnMainThread(Thread thread)
        {
            return thread.Equals(_mainThread);
        }

        public static void HighlightObj(GameObject obj, bool state)
        { //If last object not equal current, unhighlight, or if sent false, unhighlight and halt
            if(!lastHighlight?.Equals(null) ?? false && ((lastHighlight != obj) || !state))
            {
                var lastEffect = lastHighlight.GetComponent<HighlightEffect>();
                if(lastEffect != null)
                    lastEffect.SetHighlighted(false);
                if(!state) return;
            }
            if (obj?.Equals(null) ?? true) return;
            var effect = obj.GetComponent<HighlightEffect>();
            if (effect == null)
                effect = obj.AddComponent<HighlightEffect>();
            effect.ProfileLoad(MetaPort.Instance.worldHighlightProfile);
            effect.Refresh();
            effect.SetHighlighted(true);
            lastHighlight = obj;
        }

        public static void LineObj(GameObject obj)
        {
            try
            {
                SetupLineRender();
                LineRen.GetComponent<LineRenderer>().SetPosition(1, LineRen.transform.InverseTransformPoint(obj.transform.position));
                LineRen.SetActive(true);
                if (LineWaitKill) MelonCoroutines.Stop(lineRenRoutine);
                lineRenRoutine = MelonCoroutines.Start(LineKill(obj));
            }catch (System.Exception ex) { Main.Logger.Error($"Error when creating LineObj\n" + ex.ToString()); }
        }

        public static IEnumerator LineKill(GameObject obj)
        {
            LineWaitKill = true;
            var time = Time.time + lineLifespan.Value;
            while (Time.time <= time && (!obj?.Equals(null) ?? false) && !LineKillNow)
            {
                LineRen.GetComponent<LineRenderer>().SetPosition(0, LineRen.transform.position);
                LineRen.GetComponent<LineRenderer>().SetPosition(1, obj.transform.position);
                yield return null;
            }
            LineRen.SetActive(false);
            LineKillNow = false;
            LineWaitKill = false;
        }

        private static void SetupLineRender()
        {
            if (LineRen?.Equals(null) ?? true) //usePickupLine
            {
                GameObject start = MetaPort.Instance.isUsingVr ? GameObject.Find("_PLAYERLOCAL/[CameraRigVR]/Controller (right)/RayCasterRight") : Camera.main.gameObject;
                GameObject myLine = new GameObject();
                myLine.name = "WolrdPropListMod-Line";
                myLine.transform.SetParent(start.transform);
                myLine.transform.localPosition = MetaPort.Instance.isUsingVr ? Vector3.zero : new Vector3(0f,-.1f,0f);
                myLine.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                myLine.AddComponent<LineRenderer>();
                LineRenderer lr = myLine.GetComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Particles/Standard Unlit"));
                lr.useWorldSpace = true;
                lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lr.receiveShadows = false;
                lr.startColor = new Color(0f, 1f, 1f);
                lr.endColor = new Color(0f, .219f, 1f);
                lr.startWidth = .0099f;
                lr.endWidth = 0.005f;
                lr.SetPosition(0, myLine.transform.position);
                lr.SetPosition(1, new Vector3(0f, 1f, 0f));
                LineRen = myLine;
            }
        }

        public static void CheckPropNames()
        {
            Logger.Msg($"CheckPropNames - Len {CVRSyncHelper.Props.ToArray().Length}");    
            foreach (var propData in CVRSyncHelper.Props.ToArray())
            {
                Logger.Msg($"{propData.Spawnable.guid} - {propData.SpawnedBy}");
                FindPropAPIname(propData.Spawnable.guid);
                FindPlayerAPIname(propData.SpawnedBy);
            }
        }

        internal static async void FindPropAPIname(string guid)
        {//Name, URL, Author
            //Logger.Msg(ConsoleColor.DarkGray, $"FindPropAPIname");
            if (PropNamesCache.ContainsKey(guid)) return;
            PropNamesCache[guid] = ("PendingAPI", "", "", false, "", "", "", DateTime.MinValue);
            (string, string, string, string, bool, string, string, string) propName = (null, null, null, null, false, null, null, null);
            while (APIwaitingCount > 3) { await System.Threading.Tasks.Task.Delay(2000); }
            APIwaitingCount++;
            propName = await ApiRequests.RequestPropDetailsPageTask(guid);
            APIwaitingCount--;
            //Logger.Msg(APIwaitingCount);
            if (propName.Item1 == null) { PropNamesCache.Remove(guid);  return; }
            PropNamesCache[guid] = (propName.Item1, propName.Item3, propName.Item4, propName.Item5, propName.Item6, propName.Item7, propName.Item8, DateTime.Now);
            //PropImageCache[guid] = propName.Item2;
            //Logger.Msg(ConsoleColor.DarkGray, $"propName - {propName}");
            MelonCoroutines.Start(GetPropImage(guid, propName.Item2));
        }

        internal static async void FindPlayerAPIname(string guid)
        {
            //Logger.Msg(ConsoleColor.DarkGray, $"FindPlayerAPIname");
            if (PlayerNamesCache.ContainsKey(guid)) return;
            PlayerNamesCache[guid] = ("PendingAPI", DateTime.MinValue);
            string playerName = null;
            while (APIwaitingCount > 3) { await System.Threading.Tasks.Task.Delay(1500); }
            APIwaitingCount++;
            playerName = await ApiRequests.RequestPlayerDetailsPageTask(guid);
            APIwaitingCount--;
            //Logger.Msg(APIwaitingCount);
            if (playerName == null) { PlayerNamesCache.Remove(guid); return; }
            PlayerNamesCache[guid] = (playerName, DateTime.Now);
            //Logger.Msg(ConsoleColor.DarkGray, $"playerName - {playerName}");
        }

        private static IEnumerator GetPropImage(string guid, string url)
        {
            //Logger.Msg(ConsoleColor.DarkYellow, $"Getting prop image {guid} - {url}");
            var www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                MelonLogger.Error($"Error getting image for prop {guid} - {url}\n" + www.error);
                yield break;
            }

            var texture = DownloadHandlerTexture.GetContent(www);
            using (MemoryStream ms = new MemoryStream(texture.EncodeToPNG()))
            {
                // Create a BinaryReader from the MemoryStream
                using (BinaryReader br = new BinaryReader(ms))
                {
                    // Read the PNG data into a byte array
                    byte[] pngBytes = br.ReadBytes((int)ms.Length);
                    // Create a new MemoryStream from the byte array
                    MemoryStream stream = new MemoryStream(pngBytes);
                    // Use the stream as needed
                    QuickMenuAPI.PrepareIcon("WorldPropList", guid, stream);
                }
            }
        }
    }
}

