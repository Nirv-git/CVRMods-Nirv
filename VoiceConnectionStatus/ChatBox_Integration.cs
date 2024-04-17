using System;
using MelonLoader;
using UnityEngine;
using ABI_RC.Core.Player;

namespace VoiceConnectionStatus
{
    public class ChatBox_Integration
    {
        public static void SendConnectionLost()
        {
            Kafe.ChatBox.API.SendMessage("Voice Disconnected", false, true, false);
        }
        public static void SendConnectionRegained()
        {
            Kafe.ChatBox.API.SendMessage("Voice Connected", false, true, false);
        }
    }
}
