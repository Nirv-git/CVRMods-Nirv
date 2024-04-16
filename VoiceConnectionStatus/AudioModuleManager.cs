using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI_RC.Core.AudioEffects;
using System.Reflection;
using UnityEngine;
using System.IO;

namespace VoiceConnectionStatus
{
    internal class AudioModuleManager
    { //Thanks to NotAKid for this part https://github.com/NotAKidOnSteam/NAK_CVR_Mods/tree/main/MuteSFX

        #region SFX Strings
        public const string sfx_disconnect = "VoiceConnectionStatus_sfx_Disconnected";
        public const string sfx_connect = "VoiceConnectionStatus_sfx_Connected";
        #endregion

        #region Public Methods
        public static void SetupDefaultAudioClips()
        {
            string path = Application.streamingAssetsPath + "/Cohtml/UIResources/GameUI/mods/VoiceConnectionStatus/audio/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Main.Logger.Msg("Created VoiceConnectionStatus/audio directory!");
            }

            string[] clipNames = { "sfx_Disconnected.wav", "sfx_Connected.wav" };
            foreach (string clipName in clipNames)
            {
                string clipPath = Path.Combine(path, clipName);

                if (File.Exists(clipPath))
                    continue;

                byte[] clipData;
                string resourceName = "VoiceConnectionStatus.SFX." + clipName;

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream == null) continue;
                    clipData = new byte[stream.Length];
                    stream.Read(clipData, 0, clipData.Length);
                }

                using (FileStream fileStream = new FileStream(clipPath, FileMode.CreateNew))
                    fileStream.Write(clipData, 0, clipData.Length);

                Main.Logger.Msg("Placed missing sfx in audio folder: " + clipName);
            }
        }

        public static void PlayAudioModule(string module) => InterfaceAudio.PlayModule(module);
        #endregion

    }
}
