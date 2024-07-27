using System;
using System.Linq;
using System.Collections;
using System.Reflection;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using ABI.CCK;
using ABI.CCK.Components;
using ABI_RC.Core;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Core.Networking;
using System.Text;
using ABI_RC.Systems.GameEventSystem;
using DarkRift;

[assembly: MelonInfo(typeof(VoiceConnectionStatus.Main), "VoiceConnectionStatus", VoiceConnectionStatus.Main.versionStr, "Nirvash")] 
[assembly: MelonGame(null, "ChilloutVR")]

namespace VoiceConnectionStatus
{
    public class Main : MelonMod
    {
        public const string versionStr = "0.1.4";
        public static MelonLogger.Instance Logger;
        public static Main Instance;

        public static MelonPreferences_Entry<bool> setAvatarParam;
        public static MelonPreferences_Entry<bool> playSound;
        public static MelonPreferences_Entry<bool> sendChatBoxDisconnect;
        public static MelonPreferences_Entry<bool> sendChatBoxConnect;
        public static MelonPreferences_Entry<bool> logToFile;

        public static StringBuilder sb;
        public static string ExtraLogPath;
        public static bool chatbox_En = false;
        public static float joinTime = 0f;
        public static object waitReconnect_Rout = null;
        public static bool currentVoiceState = false;
        public static object waitAvatarChange_Rout = null;

        public override void OnApplicationStart()
        {
            Instance = this;
            Logger = new MelonLogger.Instance("VoiceConnectionStatus", ConsoleColor.Magenta);

            var cat = MelonPreferences.CreateCategory("VoiceConnectionStatus", "VoiceConnectionStatus");
            setAvatarParam = cat.CreateEntry(nameof(setAvatarParam), true, "Set Parameter 'VoiceConnectionStatus' on Avatar");
            playSound = cat.CreateEntry(nameof(playSound), true, "Play sound locally");
            sendChatBoxDisconnect = cat.CreateEntry(nameof(sendChatBoxDisconnect), true, "Send Chatbox Disconnect Message");
            sendChatBoxConnect = cat.CreateEntry(nameof(sendChatBoxConnect), true, "Send Chatbox Connect Message");
            logToFile = cat.CreateEntry(nameof(logToFile), false, "Log events to File", "", true);

            logToFile.OnEntryValueChangedUntyped.Subscribe((oldValue, newValue) => { InitDebugLog(); });

            // Check for ChatBox
            if (RegisteredMelons.FirstOrDefault(m => m.Info.Name == "ChatBox") != null)
            {
                Logger.Msg("Chatbox found!");
                chatbox_En = true;
            }

            AudioModuleManager.SetupDefaultAudioClips();
            InitDebugLog();
            SetupEvents();
        }

        //Set param after avatar change -- TODO

        public override void OnPreferencesSaved()
        {
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            switch (buildIndex)//Without switch this would run 3 times at world load
            {
                case 0: break;
                case 1: break;
                case 2: break;
                default:
                    joinTime = Time.time;
                    LogLine($"World Loaded at:{joinTime} Scene:{sceneName} ID:{CVRWorld.Instance.AssetInfo.objectId}");
                    break;
            }
        }

        public static System.Collections.IEnumerator VerifyReconnect()
        {
            yield return new WaitForSeconds(.5f);
            HandleConnection(true);
        }

        public static void HandleConnection(bool state)
        {
            currentVoiceState = state;
            ConsoleColor textColor = state ? ConsoleColor.Green : ConsoleColor.Red;
            if (Main.waitAvatarChange_Rout != null) MelonCoroutines.Stop(Main.waitAvatarChange_Rout);

            if (Main.playSound.Value)
            {
                AudioModuleManager.PlayAudioModule(state ? AudioModuleManager.sfx_connect : AudioModuleManager.sfx_disconnect);
            }
            if (Main.setAvatarParam.Value)
            {
                Main.Logger.Msg(textColor, $"Setting Parameter 'VoiceConnectionStatus' to: {state}");
                PlayerSetup.Instance.animatorManager.SetParameter("VoiceConnectionStatus", state);
            }
            if (Main.chatbox_En && (Time.time > Main.joinTime + 5f) && 
                (NetworkManager.Instance != null && NetworkManager.Instance.GameNetwork.ConnectionState == ConnectionState.Connected))
            {
                if (Main.sendChatBoxDisconnect.Value && !state)
                {
                    Main.Logger.Msg(textColor, $"Sending Chatbox - Connection Lost");
                    ChatBox_Integration.SendConnectionLost();
                }
                else if (Main.sendChatBoxConnect.Value && state)
                {
                    Main.Logger.Msg(textColor, $"Sending Chatbox - Connection Regained");
                    ChatBox_Integration.SendConnectionRegained();
                }
            }
        }

        public static void OnSetupAvatarGeneral()
        {
            if (Main.setAvatarParam.Value)
            {
                var state = currentVoiceState;
                ConsoleColor textColor = state ? ConsoleColor.Green : ConsoleColor.Red;
                Main.Logger.Msg(textColor, $"Avatar changed, setting Parameter 'VoiceConnectionStatus' to: {state}");
                PlayerSetup.Instance.animatorManager.SetParameter("VoiceConnectionStatus", state);
                waitAvatarChange_Rout = MelonCoroutines.Start(Main.WaitAvatarChange());
            }
        }
        public static System.Collections.IEnumerator WaitAvatarChange()
        {
            yield return new WaitForSeconds(1f);
            PlayerSetup.Instance.animatorManager.SetParameter("VoiceConnectionStatus", currentVoiceState);
            yield return new WaitForSeconds(2f);
            PlayerSetup.Instance.animatorManager.SetParameter("VoiceConnectionStatus", currentVoiceState);
        }


        //Toooooo much logging!
        public static void SetupEvents()
        {

            CVRGameEventSystem.World.OnLoad.AddListener((message) =>
            {
                try
                {
                    LogLine($"World OnLoad: {message} ");
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within CVRGameEventSystem.World.OnLoad!");
                    Logger.Error(e);
                }
            });

            CVRGameEventSystem.World.OnUnload.AddListener((message) =>
            {
                try
                {
                    LogLine($"World Unload: {message} ");
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within CVRGameEventSystem.World.OnUnload!");
                    Logger.Error(e);
                }
            });

            CVRGameEventSystem.Player.OnJoin.AddListener((message) =>
            {
                try
                {
                    LogLine($"Player OnJoin: {message.userName} - avtrId:{message.avtrId}, accountVerified: {message.accountVerified} avatarBlocked: {message.avatarBlocked} " +
                        $"ownerId:{message.ownerId} profileImageUrl:{message.profileImageUrl} userClanTag:{message.userClanTag} userRank:{message.userRank} userStaffTag:{message.userStaffTag}");
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within CVRGameEventSystem.Player.OnJoin!");
                    Logger.Error(e);
                }
            });

            CVRGameEventSystem.Player.OnLeave.AddListener((message) =>
            {
                try
                {
                    LogLine($"Player OnLeave: {message.userName} - avtrId:{message.avtrId}, accountVerified: {message.accountVerified} avatarBlocked: {message.avatarBlocked} " +
                        $"ownerId:{message.ownerId} profileImageUrl:{message.profileImageUrl} userClanTag:{message.userClanTag} userRank:{message.userRank} userStaffTag:{message.userStaffTag}");
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within CVRGameEventSystem.Player.OnLeave!");
                    Logger.Error(e);
                }
            });

            CVRGameEventSystem.Spawnable.OnLoad.AddListener((message, message2) =>
            {
                try
                {
                    LogLine($"Spawnable OnLoad: {message}, {message2}");
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within CVRGameEventSystem.Spawnable.OnLoad!");
                    Logger.Error(e);
                }
            });

            CVRGameEventSystem.Instance.OnConnected.AddListener((message) =>
            {
                try
                {
                    LogLine($"Instance OnConnected: {message}");
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within !");
                    Logger.Error(e);
                }
            });
            CVRGameEventSystem.Instance.OnConnectionLost.AddListener((message) =>
            {
                try
                {
                    LogLine($"Instance OnConnectionLost: {message}");
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within OnConnectionLost!");
                    Logger.Error(e);
                }
            });
            CVRGameEventSystem.Instance.OnConnectionRecovered.AddListener((message) =>
            {
                try
                {
                    LogLine($"Instance OnConnectionRecovered: {message}");
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within OnConnectionRecovered!");
                    Logger.Error(e);
                }
            });
            CVRGameEventSystem.Instance.OnDisconnected.AddListener((message) =>
            {
                try
                {
                    LogLine($"Instance OnDisconnected: {message}");
                }
                catch (Exception e)
                {
                    Logger.Error("An error occured within OnDisconnected!");
                    Logger.Error(e);
                }
            });

        }

        //Bunch of oooooold code I pulled from an ooooooold mod
        public static void LogLine(string text)
        {
            if (!logToFile.Value) return;
            sb.Append(DateTime.Now.ToString("'['HH':'mm':'ss.fff'] '") + text + Environment.NewLine);
        }
        public static void InitDebugLog()
        {
            if (!logToFile.Value) return;
            string basePath = "UserData/VoiceConnectionStatus";
            if (ExtraLogPath is null)
            {
                ExtraLogPath = basePath + "/" + DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss") + ".log";
                Logger.Msg(ConsoleColor.Yellow, $"DebugLog is enabled - This will write a separate log file to '{basePath}'");

            }
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            if (!File.Exists(ExtraLogPath))
            {
                File.AppendAllText(ExtraLogPath, "CVR VoiceConnectionStatus Logger - " + DateTime.Now.ToString() + Environment.NewLine);
            }
            sb = new StringBuilder(100000);
            MelonCoroutines.Start(ManageDebugBuffer());
        }
        static System.Collections.IEnumerator ManageDebugBuffer()
        {
            var nextUpdate = Time.time;
            while (logToFile.Value)
            {
                if (((sb.Length > 100000 && nextUpdate - 15f < Time.time) || nextUpdate < Time.time) && sb.Length > 0)
                {
                    WriteToFile(sb.ToString());
                    sb.Clear();
                    nextUpdate = Time.time + 60f;
                }
                yield return new WaitForSeconds(5f);
            }
            ExtraLogPath = null;
            sb.Clear();
            Logger.Msg(ConsoleColor.Gray, "End Debug Log");
        }
        public async static void WriteToFile(string text)
        {
            try
            {
                using var stream = new FileStream(ExtraLogPath, FileMode.Append, FileAccess.Write, FileShare.Write, 4096, useAsync: true);
                {
                    var bytes = Encoding.UTF8.GetBytes(text);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
            }
            catch (System.Exception ex) { Logger.Error($"Failed to save Log" + ex.ToString()); }
        }
    }
}


