using MelonLoader;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using VRBinding;
using Valve.VR;
using ABI_RC.Systems.Movement;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(FlightBinding.Main), "FlightBinding", FlightBinding.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(FlightBinding.Main.versionStr)]
[assembly: AssemblyFileVersion(FlightBinding.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.DarkBlue)]

namespace FlightBinding
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.1";

        public override void OnInitializeMelon()
        {
            Logger = new MelonLogger.Instance("FlightBinding", ConsoleColor.DarkBlue);

            VRBindingMod.RegisterBinding("FlightBinding", "Toggle Flight", VRBindingMod.Requirement.optional, a =>
            {
                if (a.GetStateDown(SteamVR_Input_Sources.Any) == true)
                {
                    Logger.Msg(ConsoleColor.Blue, "Toggling Flight");
                    BetterBetterCharacterController.Instance.ToggleFlight();
                }
            }); 
        }
    }
}








