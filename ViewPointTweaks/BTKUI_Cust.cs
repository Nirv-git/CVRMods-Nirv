using System.Reflection;
using UnityEngine;
using System;
using System.IO;
using BTKUILib;
using BTKUILib.UIObjects;
using System.Collections.Generic;
using ABI_RC.Core.Savior;
using ABI_RC.Core.InteractionSystem;
using MelonLoader;
using System.Linq;
using Semver;

namespace ViewPointTweaks
{
    public class CustomBTKUI
    {
        public static void loadAssets()
        {
            QuickMenuAPI.PrepareIcon(ModName, "NirvMisc", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.NirvMisc.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Wider", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Wider.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Wide", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Wide.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Narrower", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Narrower.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Narrow", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Narrow.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Back", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Back.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Down", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Back.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Left", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Left.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Forward", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Forward.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Right", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Right.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Rotate-Left", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Rotate-Left.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Rotate-Right", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Rotate-Right.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Up", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Up.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Reset", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Reset.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Delete", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Delete.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Save", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Save.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-Load", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.Load.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-ViewPointPos", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.ViewPointPos.png"));
            QuickMenuAPI.PrepareIcon(ModName, "vrcadj-FloppyDisk", Assembly.GetExecutingAssembly().GetManifestResourceStream("ViewPointTweaks.Icons.FloppyDisk.png"));
        }

        public static string ModName = "NirvBTKUI";
        private static MethodInfo _btkGetCreatePageAdapter;

        public static Category mainCat;
        public static Page scaleAdjPage, moveAdjPage, pageAvatarSaveLoad;
        public static Dictionary<GameObject, Vector3> wingDict = new Dictionary<GameObject, Vector3>();

        public static bool movePrec = true;

        public static void InitUi()
        {
            if (MelonMod.RegisteredMelons.Any(x => x.Info.Name.Equals("BTKUILib") && x.Info.SemanticVersion.CompareByPrecedence(new SemVersion(1, 9)) > 0))
            {
                //We're working with UILib 2.0.0, let's reflect the get create page function
                _btkGetCreatePageAdapter = typeof(Page).GetMethod("GetOrCreatePage", BindingFlags.Public | BindingFlags.Static);
                Main.Logger.Msg($"BTKUILib 2.0.0 detected, attempting to grab GetOrCreatePage function: {_btkGetCreatePageAdapter != null}");
            }
            if (!Main.useNirvMiscPage.Value)
            {
                ModName = "viewPointTweakMod";
            }

            loadAssets();

            Category cat = null;
            if (Main.useNirvMiscPage.Value)
            {
                //var page = new Page("NirvMisc", "Nirv Misc Page", true, "NirvMisc");
                Page page = null;
                if (_btkGetCreatePageAdapter != null)
                    page = (Page)_btkGetCreatePageAdapter.Invoke(null, new object[] { ModName, "Nirv Misc Page", true, "NirvMisc", null, false });
                else
                    page = new Page(ModName, "Nirv Misc Page", true, "NirvMisc");
                page.MenuTitle = "Nirv Misc Page";
                page.MenuSubtitle = "Misc page for mods by Nirv, can disable this in MelonPrefs for the individual mods";
                cat = page.AddCategory("Viewpoint Pos Adj (VR Only)");
            }
            else
            {
                cat = QuickMenuAPI.MiscTabPage.AddCategory("Viewpoint Adjust (VR Only)", ModName);
            }
            mainCat = cat;

            moveAdjPage = new Page(ModName, "Position Adjust", false);
            QuickMenuAPI.AddRootPage(moveAdjPage);
            mainCat.AddButton($"Viewpoint Position Adj", "vrcadj-ViewPointPos", $"Move the Viewpoint").OnPress += () =>
            {
                PosAdj();
            };

            scaleAdjPage = new Page(ModName, "Scale Adjust", false);
            QuickMenuAPI.AddRootPage(scaleAdjPage);
            mainCat.AddButton($"Viewpoint Scale Adj", "vrcadj-Narrow", $"Scale the Viewpoint").OnPress += () =>
            {
                ScaleAdj();
            };

            pageAvatarSaveLoad = new Page(ModName, "Saved Avatar Slots", false);
            QuickMenuAPI.AddRootPage(pageAvatarSaveLoad);
            mainCat.AddButton($"Save/Load Slots", "vrcadj-FloppyDisk", $"Save or Load from settings").OnPress += () =>
            {


                AvatarSaveLoad();
            };

            mainCat.AddButton($"Reset All", "vrcadj-Reset", $"Resets all offsets").OnPress += () =>
            {
                QuickMenuAPI.ShowConfirm("Reset all Offsets?", "Resets the position, rotation and scale offsets", () => { Main.ResetOffsets(); }, () => { }, "Yes", "No");
            };
        }




        private static void PosAdj()
        {
            var page = moveAdjPage;
            page.ClearChildren();

            if (!MetaPort.Instance.isUsingVr)
            {
                page.AddCategory("!! Mod only functions in VR !!", true, false);
            }

            var titleCat = moveAdjPage.AddCategory("temp");
            setText(); //can we just use this?
            var dist = movePrec ? 0.001f : 0.01f;
            var rotValue = movePrec ? 0.2f : 2f;

            void setText()
            {
                var isVRHead = Main.VRheadPoint != null;
                titleCat.CategoryName = $"Pos: X:{(isVRHead ? Main.VRheadPoint.localPosition.x.ToString("F3").TrimEnd('0') : "N/A")} Y:{(isVRHead ? Main.VRheadPoint.localPosition.y.ToString("F3").TrimEnd('0') : "N/A")}" +
                    $" Z:{(isVRHead ? Main.VRheadPoint.localPosition.z.ToString("F3").TrimEnd('0') : "N/A")}<p>" +
                    $"Offset: X:{(isVRHead ? Main.currentPosAdjustment.x.ToString("F3").TrimEnd('0') : "N/A")} Y:{(isVRHead ? Main.currentPosAdjustment.y.ToString("F3").TrimEnd('0') : "N/A")}" +
                    $" Z:{(isVRHead ? Main.currentPosAdjustment.z.ToString("F3").TrimEnd('0') : "N/A")}<p>Rot: {(isVRHead ? Main.currentRotAdjustment.ToString("F3").TrimEnd('0') : "N/A")}";

            }

            {
                var catRow = moveAdjPage.AddCategory("");
                catRow.AddButton($"Up", "vrcadj-Up", $"Adjusts camera Up by {dist}").OnPress += () =>
                {
                    Main.ChangeOffsets(0, new Vector3(0, -dist, 0));
                    setText();
                };
                catRow.AddButton($"Forward", "vrcadj-Forward", $"Adjusts camera Forward by {dist}").OnPress += () =>
                {
                    Main.ChangeOffsets(0, new Vector3(0, 0, -dist));

                    setText();
                };
                catRow.AddButton($"Rotate Up", "vrcadj-Rotate-Right", $"Adjusts camera rotation Up by {rotValue}").OnPress += () =>
                {
                    Main.ChangeOffsets(0, Vector3.zero, -rotValue);
                    setText();
                };
                catRow.AddToggle("Higher Precision", $"Toggles between moving with steps of {0.01f} and {0.05f}", movePrec).OnValueUpdated += action =>
                {
                    movePrec = action;
                    PosAdj();
                };
            }
            //
            {
                var catRow = moveAdjPage.AddCategory("");
                catRow.AddButton($"Left", "vrcadj-Left", $"Adjusts camera Left by {dist}").OnPress += () =>
                {
                    Main.ChangeOffsets(0, new Vector3(dist, 0, 0));

                    setText();
                };
                catRow.AddButton($"", "null", $"x").OnPress += () =>
                {

                    setText();
                };
                catRow.AddButton($"Right", "vrcadj-Right", $"Adjusts camera Right by {dist}").OnPress += () =>
                {
                    Main.ChangeOffsets(0, new Vector3(-dist, 0, 0));

                    setText();
                };

                catRow.AddButton($"Reset Position", "vrcadj-Reset", $"Resets Position").OnPress += () =>
                {
                    Main.ChangeOffsets(0, new Vector3(-Main.currentPosAdjustment.x, -Main.currentPosAdjustment.y, -Main.currentPosAdjustment.z));
                    setText();
                };

            }
            //
            {
                var catRow = moveAdjPage.AddCategory("");
                catRow.AddButton($"Down", "vrcadj-Down", $"Adjusts camera Down by {dist}").OnPress += () =>
                {
                    Main.ChangeOffsets(0, new Vector3(0, dist, 0));

                    setText();
                };
                catRow.AddButton($"Backwards", "vrcadj-Back", $"Adjusts camera Backwards by {dist}").OnPress += () =>
                {
                    Main.ChangeOffsets(0, new Vector3(0, 0, dist));

                    setText();
                };
                catRow.AddButton($"Rotate Down", "vrcadj-Rotate-Left", $"Adjusts camera rotation Down by {rotValue}").OnPress += () =>
                {
                    Main.ChangeOffsets(0, Vector3.zero, rotValue);
                    setText();
                };
                catRow.AddButton($"Reset Rotation", "vrcadj-Reset", $"Resets Rotation").OnPress += () =>
                {
                    Main.ChangeOffsets(0, Vector3.zero, -Main.currentRotAdjustment);
                    setText();
                };
            }

            page.OpenPage();
        }


        private static void ScaleAdj()
        {
            var page = scaleAdjPage;
            page.ClearChildren();

            if (!MetaPort.Instance.isUsingVr)
            {
                page.AddCategory("!! Mod only functions in VR !!", true, false);
            }

            var titleCat = scaleAdjPage.AddCategory("temp");
            setText();
            void setText()
            {
                titleCat.CategoryName = $"Scale: {((Main.VRcam != null) ? Main.VRcam.localScale.z.ToString("F3").TrimEnd('0') : "N/A")}";
            }

            {
                var valueList = new Dictionary<float, string>{
                    { -.1f,"vrcadj-Narrower" },
                    { -.01f,"vrcadj-Narrow" },
                    { .01f,"vrcadj-Wide" },
                    { .1f,"vrcadj-Wider" }
                };

                var catRow = scaleAdjPage.AddCategory("");
                foreach (var item in valueList)
                {
                    catRow.AddButton($"{item.Key}", item.Value, $"Adjusts camera scale by {item.Key}").OnPress += () =>
                    {
                        Main.ChangeOffsets(item.Key, Vector3.zero);
                        setText();
                    };
                }

                catRow.AddButton($"Reset Scale", "vrcadj-Reset", $"Resets Scale").OnPress += () =>
                {
                    Main.ChangeOffsets(-Main.currentScaleAdjustment, Vector3.zero);
                    setText();
                };
            }

            page.OpenPage();
        }


        private static void AvatarSaveLoad()
        {
            var page = pageAvatarSaveLoad;
            page.ClearChildren();

            var catMain = page.AddCategory("temp", true, false);
            UpdateText();
            void UpdateText()
            {
                var isVRHead = Main.VRheadPoint != null;
                var isPosOff = Main.currentPosAdjustment != null;
                catMain.CategoryName = "Current Settings:<p>" + $"Pos: X:{(isVRHead ? Main.VRheadPoint.localPosition.x.ToString("F3").TrimEnd('0') : "N/A")} Y:{(isVRHead ? Main.VRheadPoint.localPosition.y.ToString("F3").TrimEnd('0') : "N/A")}" +
                    $" Z:{(isVRHead ? Main.VRheadPoint.localPosition.z.ToString("F3").TrimEnd('0') : "N/A")}<p>" +
                    $"Offset: X:{(isPosOff ? Main.currentPosAdjustment.x.ToString("F3").TrimEnd('0') : "N/A")} Y:{(isPosOff ? Main.currentPosAdjustment.y.ToString("F3").TrimEnd('0') : "N/A")}" +
                    $" Z:{(isPosOff ? Main.currentPosAdjustment.z.ToString("F3").TrimEnd('0') : "N/A")}<p>Rot: {Main.currentRotAdjustment.ToString("F3").TrimEnd('0')} Scale: {(Main.currentScaleAdjustment.ToString("F3").TrimEnd('0'))}";
            }

            catMain.AddButton("Save this avatar config", "vrcadj-Save", "Save current settings").OnPress += () =>
            {
                SaveSlots.AvatarStore(MetaPort.Instance.currentAvatarGuid, Main.avatarName);
                AvatarSaveLoad();
            };

            catMain.AddToggle("Auto load", "Auto load matching settings on avatar change", Main.autoLoadAvatarPresets.Value).OnValueUpdated += action =>
            {
                Main.autoLoadAvatarPresets.Value = action;
            };

            var currentID = MetaPort.Instance.currentAvatarGuid;
            if (SaveSlots.AvatarGetSaved().ContainsKey(currentID))
            {
                SlotLine(true, new System.Collections.Generic.KeyValuePair<string, (float, float, float, float, float, string)>(
                    currentID, SaveSlots.AvatarGetSaved()[currentID]));
            }

            foreach (System.Collections.Generic.KeyValuePair<string, (float, float, float, float, float, string)>
                slot in SaveSlots.AvatarGetSaved().Reverse())
            {
                if (slot.Key == currentID)
                    continue; //Don't repeat the current one if it matches
                SlotLine(false, slot);
            }

            void SlotLine(bool current, System.Collections.Generic.KeyValuePair<string, (float, float, float, float, float, string)> slot)
            {
                string label = $"{(current ? "*Current avatar* " : "")}{slot.Value.Item6} - {slot.Key}";
                var cat = page.AddCategory(label, true, false);

                var desc = $"Offsets: X:{slot.Value.Item2.ToString("F3").TrimEnd('0')} Y:{slot.Value.Item3.ToString("F3").TrimEnd('0')}" +
                    $"Z:{slot.Value.Item4.ToString("F3").TrimEnd('0')}<p>Rot: {slot.Value.Item5.ToString("F3").TrimEnd('0')} Scale: {slot.Value.Item1.ToString("F3").TrimEnd('0')} ";
                cat.AddButton("Load", "vrcadj-Load", desc).OnPress += () =>
                {
                    SaveSlots.AvatarLoadSlot(slot.Key);
                    UpdateText();
                };
                cat.AddButton("Delete", "vrcadj-Delete", "Remove this slot").OnPress += () =>
                {
                    SaveSlots.AvatarSlotDelete(slot.Key);
                    AvatarSaveLoad();
                };
            }

            page.OpenPage();
        }
    }
}