using System;
using MelonLoader;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using BTKUILib;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Objects;
using ABI_RC.Core.Savior;
using ABI_RC.Core.InteractionSystem;
using cohtml;
using Semver;
using ABI_RC.Systems.Movement;

namespace SitLaydown
{
    class BTKUI_Cust
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Back", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Back.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Down", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Down.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-DownDown", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.DownDown.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Forward", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Forward.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Left", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Left.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Right", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Right.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Rotate-Left", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Rotate-Left.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Rotate-Right", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Rotate-Right.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-SitLayIcon", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.SitLayIcon.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Up", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Up.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-UpUp", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.UpUp.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Chair", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Chair.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Reset.png"));
            QuickMenuAPI.PrepareIcon(ModName, "sitlay-Enter", Assembly.GetExecutingAssembly().GetManifestResourceStream("SitLaydownMod.Icons.Enter.png"));
        }

         public static string ModName = "NirvBTKUI";
        private static MethodInfo _btkGetCreatePageAdapter;

        public static Page pageRoot;
        public static Category movementCat;
        public static string modPageID;

        public static void SetupUI()
        {
            if (MelonMod.RegisteredMelons.Any(x => x.Info.Name.Equals("BTKUILib") && x.Info.SemanticVersion.CompareByPrecedence(new SemVersion(1, 9)) > 0))
            {
                //We're working with UILib 2.0.0, let's reflect the get create page function
                _btkGetCreatePageAdapter = typeof(Page).GetMethod("GetOrCreatePage", BindingFlags.Public | BindingFlags.Static);
                Main.Logger.Msg($"BTKUILib 2.0.0 detected, attempting to grab GetOrCreatePage function: {_btkGetCreatePageAdapter != null}");
            }
            if (!Main.useNirvMiscPage.Value)
            {
                ModName = "sitlayMod";
            }

            loadAssets();
            QuickMenuAPI.OnOpenedPage += OnPageOpen;
            QuickMenuAPI.OnBackAction += OnPageBack;
            
            Page page;
            if (Main.useNirvMiscPage.Value)
            {
                //var pageNirv = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                Page pageNirv = null;
                if (_btkGetCreatePageAdapter != null)
                    pageNirv = (Page)_btkGetCreatePageAdapter.Invoke(null, new object[] { ModName, "Nirv Misc Page", true, "NirvMisc", null, false });
                else
                    pageNirv = new Page(ModName, "Nirv Misc Page", true, "NirvMisc");

                pageNirv.MenuTitle = "Nirv Misc Page";
                pageNirv.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";

                var catNirv = pageNirv.AddCategory("SitLaydown");
                page = new Page(ModName, "SitLaydown", false);
                QuickMenuAPI.AddRootPage(page);
                catNirv.AddButton("Open SitLaydown", "sitlay-SitLayIcon", "Opens the menu for SitLaydown").OnPress += () =>
                {
                    page.OpenPage();
                };
                catNirv.AddButton("Toggle Sit", "sitlay-Enter", "Sit with selected Animation").OnPress += () => {
                    Main.ToggleChair(Main._baseObj == null);
                };
            }
            else
            {
                page = new Page(ModName, "SitLaydown", true, "sitlay-SitLayIcon");
            }
            modPageID = page.ElementID;
            GenerateButtons();

            pageRoot = page;
        }

        public static void GenerateButtons()
        {
            try
            {
                if (pageRoot == null) return;
                if (pageRoot.IsGenerated) pageRoot.ClearChildren();

                var page = pageRoot;

                page.MenuTitle = "SitLaydown";
                page.MenuSubtitle = "Places your avatar in a sitting animation with a selection of poses";
                var cat = page.AddCategory("");

                cat.AddButton("Toggle Sit", "sitlay-Enter", "Sit with selected Animation").OnPress += () => {
                    Main.ToggleChair(Main._baseObj == null);
                };

                var animSelect = cat.AddButton("Select Animation", "sitlay-Chair", "Select an Animation");
                var multiSel_Anim = new MultiSelection("Animation", new string[] { "Laydown", "Legs Forward", "Legs Crossed", "Legs Down" }, Main.GetAnimIndex());
                multiSel_Anim.OnOptionUpdated += value => Main.SetAnimIndex(value);
                animSelect.OnPress += () => BTKUILib.QuickMenuAPI.OpenMultiSelect(multiSel_Anim);

                cat.AddToggle("Joystick Control", "Move seat position with Joystick/Keyboard", Main.joyMoveActive).OnValueUpdated += action => {
                    if (Main.joyMoveActive) Main.joyMoveActive = false;
                    else if (Main.inChair)
                        MelonCoroutines.Start(Main.JoyMove());
                };

                if (MetaPort.Instance.isUsingVr)
                {
                    cat.AddToggle("Adjust Offsets\n(VR)", $"Adjust VR player position offset{(Main.autoDisableOffsetAdj.Value ? "\nAuto disables in 120 sec after last movement" : "")}", Main.moveOffsets).OnValueUpdated += action =>
                    {
                        Main.moveOffsets = action;
                        if (Main.autoDisableOffsetAdj.Value)
                        {
                            if (Main.watchOff_Rout != null) MelonCoroutines.Stop(Main.watchOff_Rout);
                            MelonCoroutines.Start(Main.WatchMoveOffsets());
                        }
                    };
                }
                else
                    Main.moveOffsets = false;



                {
                    var catControls = page.AddCategory("tempToKeepAlive", true, false);
                    catControls.CategoryName = "";
                    movementCat = catControls;
                    SetMovementFlag(Main.distFlagActive);

                    catControls.AddButton("Up", "sitlay-Up", $"Move up by: {Main._DistAdj}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                movePos(Main._baseObj.transform.position + Main._baseObj.transform.up * Main._DistAdj);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                movePosOffset(Main.PosOffset.position + Main._baseObj.transform.up * Main._DistAdj / 2);
                        }
                    };

                    catControls.AddButton("Forward", "sitlay-Forward", $"Move forward by: {Main._DistAdj}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                movePos(Main._baseObj.transform.position + Main._baseObj.transform.forward * Main._DistAdj);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                movePosOffset(Main.PosOffset.position + Main._baseObj.transform.forward * Main._DistAdj / 2);
                        }
                    };

                    catControls.AddButton("Up Large", "sitlay-UpUp", $"Move up by: {Main._DistAdj * 4}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                movePos(Main._baseObj.transform.position + Main._baseObj.transform.up * Main._DistAdj * 4);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                movePosOffset(Main.PosOffset.position + Main._baseObj.transform.up * Main._DistAdj * 4 / 2);
                        }
                    };

                    catControls.AddButton("Rotate Right", "sitlay-Rotate-Right", $"Rotate right by: {(Main._DistHighPrec ? 1f : 5f)}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                Main._baseObj.transform.rotation *= Quaternion.AngleAxis(Main._DistHighPrec ? 1f : 5f, Vector3.up);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                Main.RotOffset.transform.RotateAround(Main.RotOffset.transform.position, Vector3.up, Main._DistHighPrec ? 1f : 5f);
                        }
                    };
                }
                {
                    var catControls = page.AddCategory("");

                    catControls.AddButton("Left", "sitlay-Left", $"Move left by: {Main._DistAdj}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                movePos(Main._baseObj.transform.position - Main._baseObj.transform.right * Main._DistAdj);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                movePosOffset(Main.PosOffset.position - Main._baseObj.transform.right * Main._DistAdj / 2);
                        }
                    };

                    catControls.AddButton("Reset", "sitlay-Reset", "Reset to starting position").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            QuickMenuAPI.ShowConfirm("Reset position?", $"Resets your {(Main.moveOffsets ? "VR Player Position and Rotation Offsets" : "Chair world position")}", () => 
                            {
                                if (!Main.moveOffsets)
                                    Main._baseObj.transform.position = Main.spawnPos;
                                else if (Main.PosOffset != null && Main.RotOffset != null)
                                {
                                    Main.PosOffset.position = Main.lastPosOffset;
                                    Main.RotOffset.rotation = Main.lastRotOffset;
                                }
                            }, () => { }, "Yes", "No");              
                        }
                    };

                    catControls.AddButton("Right", "sitlay-Right", $"Move right by: {Main._DistAdj}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                movePos(Main._baseObj.transform.position + Main._baseObj.transform.right * Main._DistAdj);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                movePosOffset(Main.PosOffset.position + Main._baseObj.transform.right * Main._DistAdj / 2);
                        }
                    };

                    catControls.AddButton("Rotate Left", "sitlay-Rotate-Left", $"Rotate left by: {(Main._DistHighPrec ? 1f : 5f)}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                Main._baseObj.transform.rotation *= Quaternion.AngleAxis(Main._DistHighPrec ? -1f : -5f, Vector3.up);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                Main.RotOffset.transform.RotateAround(Main.RotOffset.transform.position, Vector3.up, Main._DistHighPrec ? -1f : -5f);

                        }
                    };
                }

                {
                    var catControls = page.AddCategory("");

                    catControls.AddButton("Down", "sitlay-Down", $"Move down by: {Main._DistAdj}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                movePos(Main._baseObj.transform.position - Main._baseObj.transform.up * Main._DistAdj);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                movePosOffset(Main.PosOffset.position - Main._baseObj.transform.up * Main._DistAdj / 2);
                        }
                    };

                    catControls.AddButton("Backwards", "sitlay-Back", $"Move backwards by: {Main._DistAdj}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                movePos(Main._baseObj.transform.position - Main._baseObj.transform.forward * Main._DistAdj);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                movePosOffset(Main.PosOffset.position - Main._baseObj.transform.forward * Main._DistAdj / 2);
                        }
                    };

                    catControls.AddButton("Down Large", "sitlay-DownDown", $"Move down by: {Main._DistAdj * 4}").OnPress += () =>
                    {
                        if (Main._baseObj != null)
                        {
                            if (!Main.moveOffsets)
                                movePos(Main._baseObj.transform.position - Main._baseObj.transform.up * Main._DistAdj * 4);
                            else if (Main.PosOffset != null && Main.RotOffset != null)
                                movePosOffset(Main.PosOffset.position - Main._baseObj.transform.up * Main._DistAdj * 4 / 2);
                        }
                    };

                    catControls.AddToggle("Higher Precision", $"Toggles between moving with steps of {Main.DistAdjAmmount.Value} and {0.1f}", Main._DistHighPrec).OnValueUpdated += action => {
                        Main._DistHighPrec = !Main._DistHighPrec;
                        Main.Instance.OnPreferencesSaved();
                        GenerateButtons();
                    };
                }

                void movePos(Vector3 newPos)
                {
                    if (Vector3.Distance(newPos, Main.spawnPos) > 5f)
                    {
                        Main.distFlagActive = true;
                        SetMovementFlag(true);
                    }
                    else if (Main.distFlagActive)
                    {
                        Main.distFlagActive = false;
                        SetMovementFlag(false);
                    }
                    if (Vector3.Distance(newPos, Main.spawnPos) < 5.25f)
                    {
                        Main._baseObj.transform.position = newPos;
                    }
                }

                void movePosOffset(Vector3 newPos)
                {

                    if (Vector3.Distance(newPos, Main.spawnPos) > 5f)
                    {
                        Main.distFlagActive = true;
                        SetMovementFlag(true);
                    }
                    else if (Main.distFlagActive)
                    {
                        Main.distFlagActive = false;
                        SetMovementFlag(false);
                    }
                    if (Vector3.Distance(newPos, Main.spawnPos) < 5.25f)
                    {
                        Main.PosOffset.position = newPos;
                    }
                }
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error when creating menu\n" + ex.ToString()); }
        }

        public static void SetMovementFlag(bool flag)
        {
            if (movementCat == null)
                return;

            if (flag)
                movementCat.CategoryName = "!! You have moved too far from the starting location !!";
            else
                movementCat.CategoryName = "";

        }

        public static string lastQMPage = "";
        public static void OnPageOpen(string targetPage, string lastPage)
        {
            lastQMPage = targetPage;
            if (lastQMPage == modPageID)
                GenerateButtons();
        }
        public static void OnPageBack(string targetPage, string lastPage)
        {
            lastQMPage = targetPage;
            if(lastQMPage == modPageID)
                GenerateButtons();
        }
        public static void QMtoggle(bool __0)
        {
            if (__0 && lastQMPage == modPageID)
            {
                GenerateButtons();
            }
        }


    }
}


