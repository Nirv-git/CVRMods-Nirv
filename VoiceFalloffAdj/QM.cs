using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using MelonLoader;

namespace VoiceFalloffAdj
{
    internal class QM
    {
        public static GameObject settings, settingsCanvas;
        public static GameObject settingsRight, settingsTop, settingsLeft;

        public static void CreateQuickMenuButton()
        {
            settings = GameObject.Instantiate(Main.voiceSettingsPrefab);
            settings.SetActive(false);

            settings.transform.SetParent(GameObject.Find("Cohtml/QuickMenu").transform, false);
            settings.transform.localScale = new Vector3(0.01f, 0.01f, 0.05f);
            settings.transform.localPosition = new Vector3(0f, 0f, 0f);

            settingsRight = settings.transform.Find("MenuRight").gameObject;
            settingsLeft = settings.transform.Find("MenuLeft").gameObject;
            settingsTop = settings.transform.Find("MenuTop").gameObject;


            QM.settingsRight.transform.localPosition = new Vector3(78f, -31f, -0.05f);
            QM.settingsTop.transform.localPosition = new Vector3(-17f, 75.3f, -0.05f);
            QM.settingsLeft.transform.localPosition = new Vector3(-75f, -30.6f, -0.05f);


            var list = new GameObject[] { settingsRight, settingsLeft, settingsTop };
            foreach (var item in list)
            {

                settingsCanvas = item.transform.Find("SettingsMenuCanvas").gameObject;
                settingsCanvas.SetActive(false);

                item.transform.Find("ToggleSettingsCanvas/ToggleSettings").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
                {
                    var setCanvas = item.transform.Find("SettingsMenuCanvas").gameObject;
                    if (!setCanvas.activeSelf)
                        ParseSettings();
                    setCanvas.SetActive(!setCanvas.activeSelf);
                }
                ));

                //Base
                settingsCanvas.transform.Find("EnableAutoSet").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main.adjustVoiceMax.Value = !Main.adjustVoiceMax.Value;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                var slider = settingsCanvas.transform.Find("VoiceDistance").GetComponent<Slider>();
                slider.maxValue = 14.5f;
                slider.minValue = 1f;
                slider.value = Main.voiceMax.Value;
                slider.onValueChanged.AddListener(new UnityAction<float>((val) =>
                {
                    Main.voiceMax.Value = val;
                    //Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                    if(Main.sliderCoroutine != null) MelonCoroutines.Stop(Main.sliderCoroutine); //TLDR want to make sure it only tries to update every so often, likely a better way to do this though
                    Main.sliderCoroutine = MelonCoroutines.Start(Main.DelaySlider());
                }
                ));

            }

            switch (Main.QMposition.Value)
            {
                case 1: QM.settingsRight.SetActive(false); QM.settingsTop.SetActive(true); QM.settingsLeft.SetActive(false); QM.settingsCanvas = QM.settings.transform.Find("MenuTop/SettingsMenuCanvas").gameObject; break; //Top
                case 2: QM.settingsRight.SetActive(false); QM.settingsTop.SetActive(false); QM.settingsLeft.SetActive(true); QM.settingsCanvas = QM.settings.transform.Find("MenuLeft/SettingsMenuCanvas").gameObject; break; //Left
                default: QM.settingsRight.SetActive(true); QM.settingsTop.SetActive(false); QM.settingsLeft.SetActive(false); QM.settingsCanvas = QM.settings.transform.Find("MenuRight/SettingsMenuCanvas").gameObject; break; //Right
            }
        }


        public static void ParseSettings()
        {
            var custColor1 = Color.yellow;
            switch (Main.QMhighlightColor.Value)
            {
                case 1: custColor1 = new Color(1f, 1f, 0f); break;
                case 2: custColor1 = new Color(1f, 0f, 1f); break;
                default: custColor1 = new Color(1f, .6f, 0f); break;
            }
            var custColor2 = Color.white;//new Color(.18f, .18f, .18f);

            if (Main.adjustVoiceMax.Value)
                settingsCanvas.transform.Find("EnableAutoSet").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("EnableAutoSet").GetComponent<Image>().color = custColor2;

            settingsCanvas.transform.Find("MetersValue").GetComponent<TextMeshProUGUI>().text = Main.voiceMax.Value.ToString("F1");      
        }
    }
}
