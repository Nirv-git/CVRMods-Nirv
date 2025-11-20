using System;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.Player;
using ABI_RC.Systems.ChatBox;

namespace VoiceConnectionStatus
{
    public class ChatBox_Integration
    {
        public static void SendConnectionLost()
        {
            ChatBoxAPI.SendMessage("Voice Disconnected", false, true, false);
        }
        public static void SendConnectionRegained()
        {
            ChatBoxAPI.SendMessage("Voice Connected", false, true, false);
        }
    }
}
