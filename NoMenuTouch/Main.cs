using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using ABI_RC.Core.InteractionSystem;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(NoMenuTouch.Main), "NoMenuTouch", NoMenuTouch.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(NoMenuTouch.Main.versionStr)]
[assembly: AssemblyFileVersion(NoMenuTouch.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Yellow)]

namespace NoMenuTouch
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.2";

        public static MelonPreferences_Category cat;
        private const string catagory = "NoMenuTouch";
        public static MelonPreferences_Entry<bool> disableMenuTouch;

        private static bool touchDisable = true;

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("NoMenuTouch", ConsoleColor.Magenta);

            cat = MelonPreferences.CreateCategory(catagory, "NoMenuTouch");
            disableMenuTouch = MelonPreferences.CreateEntry(catagory, nameof(disableMenuTouch), true, "Disables Menu Touch");
            touchDisable = disableMenuTouch.Value;
            disableMenuTouch.OnValueChanged += (oldValue, newValue) =>
            {
                touchDisable = newValue;
                Logger.Msg("Changed [touchDisable] to " + newValue);
            };

            Logger.Msg("Starting transpiler");
            HarmonyInstance.Patch(typeof(ControllerRay).GetMethod(nameof(ControllerRay.HandleMenuUIInteraction), BindingFlags.Instance | BindingFlags.NonPublic),
                transpiler: new HarmonyMethod(typeof(Main).GetMethod(nameof(InputTranspiler), BindingFlags.Static | BindingFlags.NonPublic)));
            Logger.Msg("Executed transpiler for HandleMenuUIInteraction.");

        }


        private static IEnumerable<CodeInstruction> InputTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        { // https://gist.github.com/JavidPack/454477b67db8b017cb101371a8c49a1c 
            var code = new List<CodeInstruction>(instructions);
            
            int insertionIndex = -1;
            Label skipTouch = il.DefineLabel();
            FieldInfo instanceField = AccessTools.Field(typeof(ABI_RC.Core.UI.CohtmlViewInputHandler), "Instance");
            var instanceField_count = 0;

            for (int i = 0; i < code.Count - 1; i++) // -1 since we will be checking i + 1
            {

                if (code[i].opcode == OpCodes.Stloc_S && code[i].operand is LocalBuilder local && local.LocalIndex == 4)
                {
                    insertionIndex = i + 1; //Inserts after 'float num = 0.15f * PlayerSetup.Instance.GetPlaySpaceScale();' (This is matching the store instruction, hence the +1)
                    //Logger.Msg("Index - " + insertionIndex);
                }

                if (code[i].LoadsField(instanceField))
                {
                    instanceField_count++;
                    //Logger.Msg("Field_count - " + instanceField_count);
                    if(instanceField_count == 4)
                    {
                        code[i].labels.Add(skipTouch);
                    }
                }
            }

            if (instanceField_count == 5 && insertionIndex == 56)
            {
                var instructionsToInsert = new List<CodeInstruction>();
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Main), nameof(Main.touchDisable))));
                instructionsToInsert.Add(new CodeInstruction(OpCodes.Brtrue, skipTouch));

                code.InsertRange(insertionIndex, instructionsToInsert); 
            }
            else
                Logger.Error($"Field Count is incorrect or Index is wrong! Not patching code. Fields:{instanceField_count} Index:{insertionIndex}\nPlease contact mod author!!!");

            return code;
        }

    }
}



