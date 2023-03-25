using System;
using MelonLoader;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using BTKUILib;
using BTKUILib.UIObjects;
using ABI_RC.Core.Savior;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using ABI_RC.Core.Player;
using ABI_RC.Core;
using ABI_RC.Core.Util;


namespace WorldPropListMod
{
    class BTKUI_Cust
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon("NirvMisc", "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "Delete", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.Delete.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.Reset.png")); 
            QuickMenuAPI.PrepareIcon("WorldPropList", "Select", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.Select.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "LineHighlight", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.LineHighlight.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "LineProp", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.LineProp.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "WorldProps", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.WorldProps.png"));
            //QuickMenuAPI.PrepareIcon("WorldPropList", "", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons..png"));
        }

        public static Page pagePropList, pagePropSingle;
        public static string pagePropName = "World Prop List - All Props"; //If changed you must update PropDetailMenuBack()
        public static string pagePropSingleName = "World Prop List - Prop Detail";

        private static FieldInfo _uiInstance = typeof(QMUIElement).Assembly.GetType("BTKUILib.UserInterface").GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static);
        private static MethodInfo _registerRootPage = typeof(QMUIElement).Assembly.GetType("BTKUILib.UserInterface").GetMethod("RegisterRootPage", BindingFlags.NonPublic | BindingFlags.Instance);
        public static void HackRegisterRoot(Page element)
        {
            _registerRootPage.Invoke(_uiInstance.GetValue(null), new object[] { element });
        }

        public static void SetupUI()
        {
            loadAssets();
            QuickMenuAPI.OnBackAction += PropDetailMenuBack;

            pagePropList = new Page("WorldPropList", pagePropName, false);
            HackRegisterRoot(pagePropList);
            pagePropSingle = new Page("WorldPropList", pagePropSingleName, false);
            HackRegisterRoot(pagePropSingle);

            Category cat = null;
            if (Main.useNirvMiscPage.Value)
            {
                var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                page.MenuTitle = "Nirv Misc Page";
                page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                cat = page.AddCategory("World Prop List", "WorldPropList");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("World Prop List", "WorldPropList");
            }

            cat.AddButton("Props in World", "WorldProps", "Lists all props in the world with options to locate and delete").OnPress += () =>
            {
                PropMenu();
            };
        }

        public static void PropMenu()
        {
            try
            {
                //Clear any past highlights
                Main.HighlightObj(null, false);
                if (Main.LineWaitKill) Main.LineKillNow = true;

                var page = pagePropList;
                page.ClearChildren();

                page.MenuTitle = "Prop List";
                void SetCustomSub()
                {
                    page.MenuSubtitle = $"All the props in the current world, listed by distance from player";
                }
                SetCustomSub();

                var cat1 = page.AddCategory("");
                var cat2 = page.AddCategory("Props");

                cat1.AddButton("Delete All Props", "Delete", "Delete all props in the world").OnPress += () =>
                {
                    CVRSyncHelper.DeleteAllProps();
                };
                cat1.AddButton("Refresh", "Reset", "Refresh this page").OnPress += () =>
                {
                    PropMenu();
                };
                //cat1.AddButton("CheckPropNames()", "Reset", "").OnPress += () =>
                //{
                //    Main.CheckPropNames();
                //};
                string SelectString() {
                    switch (Main.onPropDetailSelect.Value)
                    {
                        case 0: return "None"; break;
                        case 1: return "Highlight"; break;
                        case 2: return "Line"; break;
                        case 3: return "Both"; break;
                        default: return $"Error:{Main.onPropDetailSelect.Value}"; break;
                    }
                }
                var selectButt = cat1.AddButton($"OnSelect: {SelectString()}", "LineHighlight", $"Action to do on selecting a prop | None/Highlight/Line/Both | Currently: {SelectString()}");
                selectButt.OnPress += () =>
                {
                    if (Main.onPropDetailSelect.Value <= 2)
                        Main.onPropDetailSelect.Value++;
                    else
                        Main.onPropDetailSelect.Value = 0;
                    selectButt.ButtonText = $"OnSelect: {SelectString()}";
                };

                var propList = new Dictionary<CVRSyncHelper.PropData, (string, string, float)>();
                foreach (var propData in CVRSyncHelper.Props.ToArray())
                {
                    if (propData?.Equals(null) ?? true) { Main.Logger.Msg($"propData null"); continue; }
                    if (propData?.Spawnable?.Equals(null) ?? true) { Main.Logger.Msg($"Spawnable null"); continue; }
                    if (propData.Spawnable.guid == null) { Main.Logger.Msg($"Spawnable.guid null"); continue; }
                    if (propData.SpawnedBy == null ) { Main.Logger.Msg($"SpawnedBy null"); continue; }

                    var location = new Vector3(propData.PositionX, propData.PositionY, propData.PositionZ);
                    var dist = Math.Abs(Vector3.Distance(location, Camera.main.transform.position));
                    string name = Main.PropNamesCache.TryGetValue(propData.Spawnable.guid, out var propName) ? propName : "Error: PropNameNotFound";
                    string player = Main.PlayerNamesCache.TryGetValue(propData.SpawnedBy, out var playerName) ? playerName : "Error: PlayerNameNotFound";

                    var data = (name, player, dist);
                    propList[propData] = data;
                }

                foreach (var propItem in propList.OrderBy(pair => pair.Value.Item3))
                {
                    string label = $"{propItem.Value.Item1}, by: {propItem.Value.Item2}, Distance: {Utils.NumFormat(propItem.Value.Item3)}";
                    cat2.AddButton(propItem.Value.Item1, propItem.Key.Spawnable.guid, label).OnPress += () =>
                    {
                        PropDetailMenu(propItem.Key);
                    };
                }
                page.OpenPage();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error when creating prop menu\n" + ex.ToString()); }
        }

        public static void PropDetailMenuBack(string targetPage, string lastPage)
        {
            Main.Logger.Msg($"targetPage {targetPage}, lastPage {lastPage}");
            if (targetPage == "btkUI-WorldPropList-WorldPropListAllProps" && lastPage == "btkUI-WorldPropList-WorldPropListPropDetail")
            { //This was originally going to reload the PropMenu page when going to it, however doing it this way would result the back chain getting broken
                Main.HighlightObj(null, false);
                if (Main.LineWaitKill) Main.LineKillNow = true;
                //PropMenu();
            }
        }
        
        public static void PropDetailMenu(CVRSyncHelper.PropData propData)
        {
            try {

                switch (Main.onPropDetailSelect.Value)
                {
                    case 0: break; //None
                    case 1: Main.HighlightObj(propData.Spawnable.gameObject, true); break; //Highlight
                    case 2: Main.LineObj(propData.Spawnable.gameObject); break; //Line
                    case 3: Main.HighlightObj(propData.Spawnable.gameObject, true); Main.LineObj(propData.Spawnable.gameObject); break; //Both
                    default: break;
                }

                var page = pagePropSingle;
                page.ClearChildren();

                page.MenuTitle = "Prop Detail Page";
                void SetCustomSub()
                {
                    page.MenuSubtitle = $"Details about a specific prop";
                }
                SetCustomSub();

                var cat1 = page.AddCategory("");
                var cat2 = page.AddCategory("");

                cat1.AddButton("", propData.Spawnable.guid, $"Just an icon, pressing this does nothing");
                cat1.AddToggle("Highlight", "Highlight Prop", (Main.onPropDetailSelect.Value == 1 || Main.onPropDetailSelect.Value == 3)).OnValueUpdated += action =>
                {
                    Main.HighlightObj(propData.Spawnable.gameObject, action);
                };
                cat1.AddButton("Show Line to Prop", "LineProp", $"Line from your right hand to the prop, stays active for {Main.lineLifespan.Value}s").OnPress += () =>
                {
                    Main.LineObj(propData.Spawnable.gameObject);
                };

                cat2.AddButton("Delete Prop", "Delete", "Delete this prop").OnPress += () =>
                {
                    if (propData.SpawnedBy != "SYSTEM" && propData.SpawnedBy != "LocalServer")
                    {
                        if (propData.Spawnable == null)
                            propData.Recycle();
                        else
                            propData.Spawnable.Delete();
                    }
                    PropMenu();
                };

                string name = Main.PropNamesCache.TryGetValue(propData.Spawnable.guid, out var propName) ? propName : "Error: PropNameNotFound";
                string player = Main.PlayerNamesCache.TryGetValue(propData.SpawnedBy, out var playerName) ? playerName : "Error: PlayerNameNotFound";
                var location = new Vector3(propData.PositionX, propData.PositionY, propData.PositionZ);
                var dist = Utils.NumFormat(Math.Abs(Vector3.Distance(location, Camera.main.transform.position)));

                var catName = page.AddCategory($"Name: {name}");
                var catSpawnBy = page.AddCategory($"Spawned by: {player}");
                var catDist = page.AddCategory($"Prop is {dist} meters away");
                var catPos = page.AddCategory($"X:{Utils.NumFormat(propData.PositionX)} Y:{Utils.NumFormat(propData.PositionY)} Z:{Utils.NumFormat(propData.PositionZ)}");

                page.OpenPage();
            }
            catch (System.Exception ex) { 
                Main.Logger.Error($"Error when creating prop detail menu\n" + ex.ToString()); 
                PropMenu(); //Reload PropMenu page, likely trying to open stuff for a deleted prop
            }
        }
    }
}


