using ABI_RC.Core;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using ABI_RC.Systems.ChatBox;
using ABI_RC.Systems.ChatBox.Messages;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

[assembly: MelonGame(null, "ChilloutVR")]
[assembly: MelonInfo(typeof(ChatBox_History.Main), "ChatBox_History", ChatBox_History.Main.versionStr, "Nirvash")]
[assembly: AssemblyVersion(ChatBox_History.Main.versionStr)]
[assembly: AssemblyFileVersion(ChatBox_History.Main.versionStr)]
[assembly: MelonColor(ConsoleColor.Green)]

namespace ChatBox_History
{
    public class Main : MelonMod
    {
        public static MelonLogger.Instance Logger;
        public const string versionStr = "0.0.1";

        public static MelonPreferences_Category cat;
        private const string catagory = "ChatBox_History";
        public static MelonPreferences_Entry<bool> useNirvMiscPage;

                                    //Message, Username, Time, Distance
        public static List<(ChatBoxAPI.ChatBoxMessage, string, string, string)> chatboxMessages = new List<(ChatBoxAPI.ChatBoxMessage, string, string, string)>() ;

        public static Dictionary<string, string> UsernameCache = new Dictionary<string, string>();

        public override void OnApplicationStart()
        {
            Logger = new MelonLogger.Instance("ChatBox_History", ConsoleColor.Green);

            cat = MelonPreferences.CreateCategory(catagory, "ChatBox_History");
            useNirvMiscPage = MelonPreferences.CreateEntry(catagory, nameof(useNirvMiscPage), false, "BTKUI - Use 'NirvMisc' page instead of custom page. (Restart req)");
            CustomUI.InitUi();

            ChatBoxAPI.OnMessageReceived += HandleMessage;
        }

        public static void HandleMessage(ChatBoxAPI.ChatBoxMessage msg)
        {
            try
            {
                //Logger.Msg("---");
                //Logger.Msg(msg.Source);
                //Logger.Msg(msg.SenderGuid);
                //Logger.Msg(msg.Message);
                //Logger.Msg(msg.TriggerNotification);
                //Logger.Msg(msg.DisplayOnChatBox);
                //Logger.Msg(msg.DisplayOnHistory);
                //Logger.Msg(msg.ModName);
                //Logger.Msg("---");

                if (msg.DisplayOnHistory)
                    chatboxMessages.Add((msg, GetUsername(msg.SenderGuid), DateTime.Now.ToString("HH:mm:ss"), GetDistance(msg.SenderGuid).ToString("F1"))); //"yyyy-MM-dd HH:mm:ss" |  AM/PM "yyyy-MM-dd hh:mm:ss tt"
            }
            catch (Exception e) {Logger.Error("Error in HandleMessage - " + e.ToString());}
        }

        public static float GetDistance(string senderGuid)
        {
            var obj = GameObject.Find($"{senderGuid}/[PlayerAvatar]")?.transform?.GetChild(0)?.gameObject;
            if (obj == null)
                return -1f;
            return Vector3.Distance(PlayerSetup.Instance.AvatarObject.transform.position, obj.transform.position);
        }

        public static string GetUsername(string senderGuid)
        {
            if (UsernameCache.ContainsKey(senderGuid))
                return UsernameCache[senderGuid];

            string username = GameObject.Find($"{senderGuid}/[NamePlate]/Canvas/Content/TMP:Username")?.GetComponent<TMP_Text>()?.text;
            if (username == null)
                return "N/A - Unknown";
            return username;
        }

    }
   
}



