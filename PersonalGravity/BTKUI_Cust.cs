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
using ABI.CCK.Components;

namespace PersonalGravity
{
    class BTKUI_Cust
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Gravity", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Gravity.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Cords", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Cords.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Cords-Off", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Cords-Off.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Horizon", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Horizon.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-HorizonDown", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.HorizonDown.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-X-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.X-Minus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-X-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.X-Plus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Y-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Y-Minus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Y-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Y-Plus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Z-Minus", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Z-Minus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Z-Plus", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Z-Plus.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-GazeRayCast", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.GazeRayCast.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-HandRayCast", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.HandRayCast.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Keypad", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Keypad.png"));
            QuickMenuAPI.PrepareIcon(ModName, "personalGrav-Settings", Assembly.GetExecutingAssembly().GetManifestResourceStream("PersonalGravity.Icons.Settings.png"));
        }

        public static string ModName = "NirvBTKUI";

        public static Page pageRoot;
        public static Page settingsPage;
        public static string modPageID, nirvMiscPageID;
        public static BTKUILib.UIObjects.Components.Button NirvToggle;

        public static void SetupUI()
        {
            if (!Main.useNirvMiscPage.Value)
            {
                ModName = "personalGrav";
            }

            loadAssets();
            QuickMenuAPI.OnOpenedPage += OnPageOpen;
            QuickMenuAPI.OnBackAction += OnPageBack;
            
            Page page;
            if (Main.useNirvMiscPage.Value)
            {
                Page pageNirv = Page.GetOrCreatePage(ModName, "Nirv Misc Page", true, "NirvMisc", null, false);
                nirvMiscPageID = pageNirv.ElementID;
                pageNirv.MenuTitle = "Nirv Misc Page";
                pageNirv.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                var catNirv = pageNirv.AddCategory("Personal Gravity");
                page = new Page(ModName, "Personal Gravity", false);
                QuickMenuAPI.AddRootPage(page);

                var toggleStr = Main._baseObj != null ? "personalGrav-Cords" : "personalGrav-Cords-Off";
                NirvToggle = catNirv.AddButton("Toggle", toggleStr, $"desc");
                NirvToggle.OnPress += () =>
                {
                    Main.ToggleGrav();
                    updateNirvToggle();
                };
                catNirv.AddButton("Open Personal Gravity Settings", "personalGrav-Gravity", "Opens the menu for Personal Gravity").OnPress += () =>
                {
                    page.OpenPage();
                };
                catNirv.AddButton("Surface Normal of Raycast", "personalGrav-GazeRayCast", $"Desktop: Set Gravity based on the surface hit from your current gaze<p>VR: Set Gravity based on the surface hit from your right hand raycast").OnPress += () =>
                {
                    Main.FindCamRayCast();
                };
                catNirv.AddButton("Y- (Normal)", "personalGrav-Y-Minus", $"Set Gravity to the World Y- Direction").OnPress += () =>
                {
                    Main.gravDirection = -Vector3.up;
                    Main.SetRotation();
                };
            }
            else
            {
                page = new Page(ModName, "PersonalGravity", true, "personalGrav-Gravity");
            }
            modPageID = page.ElementID;
            pageRoot = page;

            GenerateButtons();
        }

        public static void GenerateButtons()
        {
            try
            {
                if (pageRoot == null) return;
                if (pageRoot.IsGenerated) pageRoot.ClearChildren();

                var page = pageRoot;
                page.MenuTitle = "Personal Gravity";
                page.MenuSubtitle = "Control your own gravity!";

                var cat = page.AddCategory(makeCatName(), true, false);
               
                void setCatName()
                {
                    cat.CategoryName = makeCatName();
                }
                string makeCatName()
                {
                    return $"(X:{Main.gravDirection.x:F2}, Y:{Main.gravDirection.y:F2}, Z:{Main.gravDirection.z:F2})";
                }

                var toggleStr = Main._baseObj != null ? "personalGrav-Cords" : "personalGrav-Cords-Off";
                cat.AddButton("Toggle", toggleStr, $"Enable/Disables local Gravity Zone").OnPress += () =>
                {
                    Main.ToggleGrav();
                    GenerateButtons();
                };

                if (!MetaPort.Instance?.isUsingVr ?? false)
                {//Desktop
                    cat.AddButton("Surface Normal of Gaze Hit", "personalGrav-GazeRayCast", $"Set Gravity Down to the surface normal of a hit from a Raycast out from your current gaze direction").OnPress += () =>
                    {
                        Main.FindCamRayCast();
                        setCatName();
                    };
                }
                else
                {//VR
                    cat.AddButton("Surface Normal of Raycast", "personalGrav-HandRayCast", $"Set Gravity Down to the surface normal of a hit from a Raycast out from your Right Hand. Click the trigger to select current position. Pressing both grips will cancel.").OnPress += () =>
                    {
                        Main.FindCamRayCast();
                        setCatName();
                    };
                }
                cat.AddButton("Current Gaze Direction", "personalGrav-Horizon", $"Set Gravity Down to the direction of your current camera gaze").OnPress += () =>
                {
                    var direction = Camera.main.transform.forward;
                    if (Main.snapAngles.Value)
                        direction = Utils.RoundToNearest5(direction);
                    Main.gravDirection = direction;
                    Main.SetRotation();
                    setCatName();
                };
                cat.AddButton("Current Gaze Down", "personalGrav-HorizonDown", $"Set Gravity Down to the down vector from your current camera gaze").OnPress += () =>
                {
                    var direction = -Camera.main.transform.up;
                    if (Main.snapAngles.Value)
                        direction = Utils.RoundToNearest5(direction);
                    Main.gravDirection = direction;
                    Main.SetRotation();
                    setCatName();
                };
                //
                {
                    var catControls = page.AddCategory("", false, false);
                    catControls.AddButton("Y+", "personalGrav-Y-Plus", $"Set Gravity to the World Y+ Direction").OnPress += () =>
                    {
                        Main.gravDirection = Vector3.up;
                        Main.SetRotation();
                        setCatName();
                    };
                    catControls.AddButton("Y- (Normal)", "personalGrav-Y-Minus", $"Set Gravity to the World Y- Direction").OnPress += () =>
                    {
                        Main.gravDirection = -Vector3.up;
                        Main.SetRotation();
                        setCatName();
                    };
                    catControls.AddToggle("Align to Gravity", $"Align player rotation with Gravity Zone", Main.alignPlayer.Value).OnValueUpdated += action => {
                        Main.alignPlayer.Value = action;
                        Main.SetAlign();
                    };

                    {///////////////////////// Settings Menu
                        if (settingsPage?.IsGenerated ?? false) settingsPage.ClearChildren();
                        settingsPage = catControls.AddPage("Personal Gravity Settings", "personalGrav-Settings", "Settings for the mod", ModName);
                        var settingsCat1 = settingsPage.AddCategory("Gravity Zone Options", true, false);

                        var effectText = (CVRWorld.Instance?.allowSpawnables ?? true) ? "" : "(Forced_On)";
                        settingsCat1.AddToggle($"Only Effects Players {effectText}", $"True: Only players are effected by Gravity<p>False: Objects are also effected in worlds that allow Props", Main.effectOnlyPlayer.Value).OnValueUpdated += action =>
                        {
                            Main.effectOnlyPlayer.Value = action;
                            Main.SetEffect();
                            if (!action && (!CVRWorld.Instance?.allowSpawnables ?? false))
                                QuickMenuAPI.ShowAlertToast("Gravity effecting objects is only supported in worlds that allow Props.<p>This world does not.", 5);
                        };
                        settingsCat1.AddToggle("Mix Type Override", $"True: Gravity Mix Type is Override<p>False: Gravity Mix Type is Additive", Main.mixOverride.Value).OnValueUpdated += action =>
                        {
                            Main.mixOverride.Value = action;
                            Main.SetMix();
                        };
                        settingsCat1.AddButton($"Gravity Priority", "personalGrav-Keypad", "Change Gravity Priority<p>Default:99999999").OnPress += () =>
                        {
                            QuickMenuAPI.OpenNumberInput("Gravity Priority", Main.gravPriority.Value, (action) =>
                            {
                                Main.gravPriority.Value = (int)action;
                                Main.SetPriority();
                            });
                        };
                        //
                        var settingsCat2 = settingsPage.AddCategory("Mod Options", true, false);
                        settingsCat2.AddToggle("Auto Toggle with Raycast", $"Will enable the Gravity Zone if it is off when using the raycast option", Main.autoToggle.Value).OnValueUpdated += action =>
                        {
                            Main.autoToggle.Value = action;
                        };
                        settingsCat2.AddToggle("Snap to Angle", $"Snaps to nearest 20 degrees for the Gaze Direction and Down options", Main.snapAngles.Value).OnValueUpdated += action =>
                        {
                            Main.snapAngles.Value = action;
                        };
                    }
                }
                //
                {
                    var catControls = page.AddCategory("", false, false);
                    catControls.AddButton("Z+", "personalGrav-Z-Plus", $"Set Gravity to the World Z+ Direction").OnPress += () =>
                    {
                        Main.gravDirection = Vector3.forward;
                        Main.SetRotation();
                        setCatName();
                    };
                    catControls.AddButton("Z-", "personalGrav-Z-Minus", $"Set Gravity to the World Z- Direction").OnPress += () =>
                    {
                        Main.gravDirection = -Vector3.forward;
                        Main.SetRotation();
                        setCatName();
                    };
                    catControls.AddButton("X+", "personalGrav-X-Plus", $"Set Gravity to the World X+ Direction").OnPress += () =>
                    {
                        Main.gravDirection = Vector3.right;
                        Main.SetRotation();
                        setCatName();
                    };
                    catControls.AddButton("X-", "personalGrav-X-Minus", $"Set Gravity to the World X- Direction").OnPress += () =>
                    {
                        Main.gravDirection = -Vector3.right;
                        Main.SetRotation();
                        setCatName();
                    };
                }
                //
                {
                    var catControls = page.AddCategory("", false, false);
                    catControls.AddSlider("Gravity Value", "The strength of gravity, default 9.81", Main.gravStr, 0f, 20f, 2, 9.81f, true).OnValueUpdated += action =>
                    {
                        Main.gravStr = action;
                        Main.SetStrength();
                    };
                }
            }
            catch (System.Exception ex) { Main.Logger.Error($"Error when creating menu\n" + ex.ToString()); }
        }

        public static string lastQMPage = "";
        public static void OnPageOpen(string targetPage, string lastPage)
        {
            lastQMPage = targetPage;
            if (lastQMPage == modPageID)
                GenerateButtons();
            else if (lastQMPage == nirvMiscPageID)
                updateNirvToggle();
        }
        public static void OnPageBack(string targetPage, string lastPage)
        {
            lastQMPage = targetPage;
            if(lastQMPage == modPageID)
                GenerateButtons();
            else if (lastQMPage == nirvMiscPageID)
                updateNirvToggle();
        }
        public static void QMtoggle(bool __0)
        {
            if (__0)
            {
                if (lastQMPage == modPageID)
                    GenerateButtons();
                else if (lastQMPage == nirvMiscPageID)
                    updateNirvToggle();
            }     
        }

        public static void updateNirvToggle()
        {
            if (NirvToggle != null)
            {
                var toggleStr = Main._baseObj != null ? "personalGrav-Cords" : "personalGrav-Cords-Off";
                NirvToggle.ButtonIcon = toggleStr;
            }
        }
    }
}


