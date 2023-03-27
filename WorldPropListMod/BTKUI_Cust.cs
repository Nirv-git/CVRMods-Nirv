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
            //QuickMenuAPI.PrepareIcon("NirvMisc", "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "Delete", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.Delete.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.Reset.png")); 
            QuickMenuAPI.PrepareIcon("WorldPropList", "Select", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.Select.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "LineHighlight", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.LineHighlight.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "LineProp", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.LineProp.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "WorldProps", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.WorldProps.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "Block", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.Block.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "BlockTrash", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.BlockTrash.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "Unblock", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.Unblock.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "UnblockAll", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.UnblockAll.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "Props", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.Props.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "BlockList", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.BlockList.png"));
            QuickMenuAPI.PrepareIcon("WorldPropList", "ResetList", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons.ResetList.png"));
            //QuickMenuAPI.PrepareIcon("WorldPropList", "", Assembly.GetExecutingAssembly().GetManifestResourceStream("WorldPropListMod.Icons..png"));
        }

        public static Page pagePropList, pagePropSingle, pagePropBlocks, pagePropBlockHistory;

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

            pagePropList = new Page("WorldPropList", "World Prop List - All Props", false);
            HackRegisterRoot(pagePropList);
            pagePropSingle = new Page("WorldPropList", "World Prop List - Prop Detail", false);
            HackRegisterRoot(pagePropSingle);
            pagePropBlocks = new Page("WorldPropList", "World Prop List - Blocked Props", false);
            HackRegisterRoot(pagePropBlocks);
            pagePropBlockHistory = new Page("WorldPropList", "World Prop List - Blocked Props History", false);
            HackRegisterRoot(pagePropBlockHistory);

            //Category cat = null;
            //if (Main.useNirvMiscPage.Value)
            //{
            //    var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
            //    page.MenuTitle = "Nirv Misc Page";
            //    page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
            //    cat = page.AddCategory("World Prop List", "WorldPropList");
            //}
            //else
            //{
            //    cat = QuickMenuAPI.MiscTabPage.AddCategory("World Prop List", "WorldPropList");
            //}

            var page = new Page("WorldPropList", "World Prop List", true, "Props");
            page.MenuTitle = "World Prop List";
            page.MenuSubtitle = "Lists all props in the world with options to locate and delete";
            var cat = page.AddCategory("");

            cat.AddButton("Props in World", "WorldProps", "Lists all props in the world with options to locate and delete").OnPress += () =>
            {
                PropMenu(true);
            };
            cat.AddButton("Blocked Props List", "Block", "Lists all props that you have blocked").OnPress += () =>
            {
                PropBlockMenu();
            };
            cat.AddButton("Blocked Props this Session", "BlockList", "Lists all props that have been blocked from spawning this session").OnPress += () =>
            {
                PropBlockHistoryMenu();
            };
            cat.AddButton("Delete All Props", "Delete", "Delete all props in the world").OnPress += () =>
            {
                QuickMenuAPI.ShowConfirm("Delete all props?", "This will delete all props in the world", () => { CVRSyncHelper.DeleteAllProps(); }, () => { }, "Yes", "No");
            };
        }

        public static void PropMenu(bool openPage)
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
                    QuickMenuAPI.ShowConfirm("Delete all props?", "This will delete all props in the world", () => { CVRSyncHelper.DeleteAllProps(); PropMenu(true); }, () => { }, "Yes", "No");         
                };
                cat1.AddButton("Refresh", "Reset", "Refresh this page").OnPress += () =>
                {
                    PropMenu(true);
                };
                //cat1.AddButton("CheckPropNames()", "Reset", "").OnPress += () =>
                //{
                //    Main.CheckPropNames();
                //};
                string SelectString() {
                    switch (Main.onPropDetailSelect.Value)
                    {
                        case 0: return "None";
                        case 1: return "Highlight";
                        case 2: return "Line";
                        case 3: return "Both";
                        default: return $"Error:{Main.onPropDetailSelect.Value}";
                    }
                }
                var selectButt = cat1.AddButton($"--OnSelect-- {SelectString()}", "LineHighlight", $"Action to do on selecting a prop | None/Highlight/Line/Both<p>Currently: {SelectString()}");
                selectButt.OnPress += () =>
                {
                    if (Main.onPropDetailSelect.Value <= 2)
                        Main.onPropDetailSelect.Value++;
                    else
                        Main.onPropDetailSelect.Value = 0;
                    selectButt.ButtonText = $"--OnSelect-- {SelectString()}";
                };

                var propList = new Dictionary<CVRSyncHelper.PropData, (string, string, float)>();
                foreach (var propData in CVRSyncHelper.Props.ToArray())
                {
                    if (propData?.Equals(null) ?? true) { continue; } // Main.Logger.Msg($"propData null"); continue; }
                    if (propData?.Spawnable?.Equals(null) ?? true) { continue; } // Main.Logger.Msg($"Spawnable null"); continue; }
                    if (propData.Spawnable.guid == null) { continue; } // Main.Logger.Msg($"Spawnable.guid null"); continue; }
                    if (propData.SpawnedBy == null ) { continue; } //Main.Logger.Msg($"SpawnedBy null"); continue; }

                    var location = new Vector3(propData.PositionX, propData.PositionY, propData.PositionZ);
                    var dist = Math.Abs(Vector3.Distance(location, Camera.main.transform.position));
                    string name = Main.PropNamesCache.TryGetValue(propData.Spawnable.guid, out var propName) ? propName : "Error: PropNameNotFound";
                    string player = Main.PlayerNamesCache.TryGetValue(propData.SpawnedBy, out var playerName) ? playerName : "Error: PlayerNameNotFound";

                    var data = (name, player, dist);
                    propList[propData] = data;
                }

                foreach (var propItem in propList.OrderBy(pair => pair.Value.Item3))
                {
                    string label = $"{propItem.Value.Item1}, by: {propItem.Value.Item2}<p>Distance: {Utils.NumFormat(propItem.Value.Item3)}";
                    cat2.AddButton(propItem.Value.Item1, propItem.Key.Spawnable.guid, label).OnPress += () =>
                    {
                        if (!propItem.Key?.Spawnable?.gameObject.Equals(null) ?? false) PropDetailMenu(propItem.Key);
                        else PropMenu(true); //Reload PropMenu page, likely trying to open stuff for a deleted prop
                    };
                }

                if(!MetaPort.Instance.worldAllowProps)
                {
                    page.AddCategory("");
                    page.AddCategory("Props are not allowed in this world");
                }

                if(openPage) page.OpenPage();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error when creating prop menu\n" + ex.ToString()); }
        }

        public static void PropDetailMenuBack(string targetPage, string lastPage)
        {
            //Main.Logger.Msg($"targetPage {targetPage}, lastPage {lastPage}");
            if (targetPage == pagePropList.ElementID && lastPage == pagePropSingle.ElementID)
            { 
                Main.HighlightObj(null, false);
                if (Main.LineWaitKill) Main.LineKillNow = true;
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

                var guid = propData.Spawnable.guid;
                string name = Main.PropNamesCache.TryGetValue(guid, out var propName) ? propName : "Error: PropNameNotFound";
                string player = Main.PlayerNamesCache.TryGetValue(propData.SpawnedBy, out var playerName) ? playerName : "Error: PlayerNameNotFound";
                var location = new Vector3(propData.PositionX, propData.PositionY, propData.PositionZ);
                var dist = Utils.NumFormat(Math.Abs(Vector3.Distance(location, Camera.main.transform.position)));

                cat1.AddButton("", guid, $"Just an icon, pressing this does nothing");
                cat1.AddToggle("Highlight", "Highlight Prop", (Main.onPropDetailSelect.Value == 1 || Main.onPropDetailSelect.Value == 3)).OnValueUpdated += action =>
                {
                    if (!propData?.Spawnable?.gameObject.Equals(null) ?? false) Main.HighlightObj(propData.Spawnable.gameObject, action);
                    else { PropMenu(false);  page.ClearChildren(); page.AddCategory("Prop was deleted"); }
                };
                cat1.AddButton("Show Line to Prop", "LineProp", $"Line from your right hand to the prop, stays active for {Main.lineLifespan.Value}s").OnPress += () =>
                {
                    if(!propData?.Spawnable?.gameObject.Equals(null) ?? false) Main.LineObj(propData.Spawnable.gameObject);
                    else { PropMenu(false); page.ClearChildren(); page.AddCategory("Prop was deleted"); }
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
                    PropMenu(false);
                    page.ClearChildren();
                    page.AddCategory("Prop was deleted");
                };

                cat2.AddButton("Block Prop and Delete", "BlockTrash", "Add this prop to the block list and Delete it").OnPress += () =>
                {
                    QuickMenuAPI.ShowConfirm("Confirm Block", "Are you sure you want to block and delete this prop?", () => {
                        if (propData.SpawnedBy != "SYSTEM" && propData.SpawnedBy != "LocalServer")
                        {
                            if (!SaveLoad.blockedProps.ContainsKey(guid))
                            {
                                SaveLoad.blockedProps.Add(guid, name);
                                SaveLoad.SaveListFiles();
                            }
                            if (propData.Spawnable == null)
                                propData.Recycle();
                            else
                                propData.Spawnable.Delete();

                            PropMenu(false);
                            page.ClearChildren();
                            page.AddCategory("Prop was deleted");
                        }
                    }, () => { }, "Yes", "No");
                };            

                var catName = page.AddCategory($"Name: {name}");
                var catSpawnBy = page.AddCategory($"Spawned by: {player}");
                var catDist = page.AddCategory($"Prop is {dist} meters away");
                var catPos = page.AddCategory($"X:{Utils.NumFormat(propData.PositionX)} Y:{Utils.NumFormat(propData.PositionY)} Z:{Utils.NumFormat(propData.PositionZ)}");
                if (SaveLoad.blockedProps.ContainsKey(guid))
                {
                    page.AddCategory("");
                    page.AddCategory("");
                    page.AddCategory("");
                    page.AddCategory("PROP IS IN BLOCK LIST");
                }
                page.OpenPage();
            }
            catch (System.Exception ex) { 
                Main.Logger.Error($"Error when creating prop detail menu\n" + ex.ToString()); 
                PropMenu(true); //Reload PropMenu page, likely trying to open stuff for a deleted prop
            }
        }

        public static void PropBlockMenu()
        {
            try
            {
                var page = pagePropBlocks;
                page.ClearChildren();

                page.MenuTitle = "Blocked Prop List";
                page.MenuSubtitle = $"List of all blocked props. Clicking each entry will unblock";

                var cat1 = page.AddCategory("");
                var cat2 = page.AddCategory("Props");

                cat1.AddButton("Unblock all Props", "UnblockAll", "Clear the block list").OnPress += () =>
                {
                    QuickMenuAPI.ShowConfirm("Unblock all Props", "Are you sure you want to remove all props from the block list?", () => 
                    {
                        SaveLoad.blockedProps = new Dictionary<string, string>();
                        SaveLoad.SaveListFiles();
                        PropBlockMenu(); 
                    }, () => { }, "Yes", "No");
                };

                if (SaveLoad.blockedProps.Count > 0)
                {
                    foreach (var blockedProp in SaveLoad.blockedProps.Reverse())
                    {
                        cat2.AddButton(blockedProp.Value, "Unblock", $"Unblock prop: {blockedProp.Value}<p>GUID:{blockedProp.Key}").OnPress += () =>
                        {
                            SaveLoad.blockedProps.Remove(blockedProp.Key);
                            SaveLoad.SaveListFiles();
                            PropBlockMenu();
                        };
                    }
                }
                else
                {
                    page.AddCategory("");
                    page.AddCategory("");
                    page.AddCategory("");
                    page.AddCategory("Prop blocklist is empty");
                }
                page.OpenPage();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error when creating prop block menu\n" + ex.ToString()); }
        }

        public static void PropBlockHistoryMenu()
        {
            try
            {
                var page = pagePropBlockHistory;
                page.ClearChildren();

                page.MenuTitle = "Blocked Prop History";
                page.MenuSubtitle = $"List of props that have been blocked from spawning this session";

                var cat1 = page.AddCategory("");
                cat1.AddButton("Clear History", "ResetList", "Clear the history").OnPress += () =>
                {
                    Main.BlockedThisSession.Clear();
                    PropBlockHistoryMenu();
                };
                cat1.AddButton("Refresh", "Reset", "Refresh this page").OnPress += () =>
                {
                    PropBlockHistoryMenu();
                };

                if (Main.BlockedThisSession.Count > 0)
                {
                    foreach (var blockedProp in Main.BlockedThisSession.Reverse<(string, string, string)>())
                    {
                        var playerName = Main.PlayerNamesCache.TryGetValue(blockedProp.Item2, out string name) ? name : blockedProp.Item2;
                        page.AddCategory($"{blockedProp.Item1}");
                        page.AddCategory($"Spawned by:{playerName}, At:{blockedProp.Item3}");
                        page.AddCategory("");
                    }
                }
                else
                {
                    page.AddCategory("");
                    page.AddCategory("");
                    page.AddCategory("");
                    page.AddCategory("List is empty");
                }
                page.OpenPage();
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error when creating prop block history menu\n" + ex.ToString()); }
        }

    }
}


