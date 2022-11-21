using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using MelonLoader;

namespace SitLaydown
{
    internal class QM
    {
        public static GameObject settings, settingsCanvas;
        public static GameObject settingsRight, settingsTop, settingsLeft;

        public static void CreateQuickMenuButton()
        {
            settings = GameObject.Instantiate(Main.settingsPrefab);
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

            QM.settingsRight.transform.Find("ToggleSettingsCanvas").transform.localPosition = new Vector3((.4f + Main.QMtogglePosOffset.Value * .8f), -.8f, 0f);
            QM.settingsTop.transform.Find("ToggleSettingsCanvas").transform.localPosition = new Vector3((.4f + Main.QMtogglePosOffset.Value * .8f), -.8f, 0f);
            QM.settingsLeft.transform.Find("ToggleSettingsCanvas").transform.localPosition = new Vector3((-.4f + Main.QMtogglePosOffset.Value * -.8f), -.8f, 0f);

            var list = new GameObject[] { settingsRight, settingsLeft, settingsTop };
            foreach (var item in list)
            {
                settingsCanvas = item.transform.Find("SettingsMenuCanvas").gameObject;
                settingsCanvas.SetActive(false);

                item.transform.Find("ToggleSettingsCanvas/ToggleSettings").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
                {
                    var setCanvas = item.transform.Find("SettingsMenuCanvas").gameObject;
                    setCanvas.SetActive(!setCanvas.activeSelf);
                    ParseSettings();
                }
                ));

                //Base
                settingsCanvas.transform.Find("EnableLaySit").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main.ToggleChair(Main._baseObj == null);
                    ParseSettings();
                }
                ));

                //Anims
                settingsCanvas.transform.Find("Anim-Lay").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main.SittingAnim.Value = "Laydown";
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Anim-SitIdle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main.SittingAnim.Value = "SitIdle";
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Anim-SitCrossed").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main.SittingAnim.Value = "SitCrossed";
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Anim-BasicLegsDown").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main.SittingAnim.Value = "BasicSit";
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));


                //Movement
                settingsCanvas.transform.Find("Adj-Up").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null) Main._baseObj.transform.position += Main._baseObj.transform.up * Main._DistAdj;
                }
                ));
                settingsCanvas.transform.Find("Adj-Down").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null) Main._baseObj.transform.position -= Main._baseObj.transform.up * Main._DistAdj;
                }
                ));
                settingsCanvas.transform.Find("Adj-UpDouble").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null) Main._baseObj.transform.position += Main._baseObj.transform.up * Main._DistAdj * 4;
                }
                ));
                settingsCanvas.transform.Find("Adj-DownDouble").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null) Main._baseObj.transform.position -= Main._baseObj.transform.up * Main._DistAdj * 4;
                }
                ));
                settingsCanvas.transform.Find("Adj-Forward").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null) Main._baseObj.transform.position += Main._baseObj.transform.forward * Main._DistAdj;
                }
                ));
                settingsCanvas.transform.Find("Adj-Backward").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null) Main._baseObj.transform.position -= Main._baseObj.transform.forward * Main._DistAdj;
                }
                ));
                settingsCanvas.transform.Find("Adj-Right").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null) Main._baseObj.transform.position += Main._baseObj.transform.right * Main._DistAdj;
                }
                ));
                settingsCanvas.transform.Find("Adj-Left").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null) Main._baseObj.transform.position -= Main._baseObj.transform.right * Main._DistAdj;
                }
                ));
                settingsCanvas.transform.Find("Adj-RotRight").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null) Main._baseObj.transform.rotation *= Quaternion.AngleAxis(Main._DistHighPrec ? 1f : 5f, Vector3.up);
                }
                ));
                settingsCanvas.transform.Find("Adj-RotLeft").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._baseObj != null)  Main._baseObj.transform.rotation *= Quaternion.AngleAxis(Main._DistHighPrec ? -1f : -5f, Vector3.up);
                }
                ));

                //Misc
                settingsCanvas.transform.Find("Adj-Prec").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._DistHighPrec = !Main._DistHighPrec;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Rot-Lock").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main.holdRotataion.Value = !Main.holdRotataion.Value;
                    ParseSettings();
                    if(!Main.rotActive & Main.inChair) Main.rotRoutine = MelonCoroutines.Start(Main.HoldRotation());
                }
                ));
                settingsCanvas.transform.Find("Joy-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main.joyMoveActive) Main.joyMoveActive = false;
                    else if (Main.inChair)
                        MelonCoroutines.Start(Main.JoyMove());
                    ParseSettings();
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

            if (Main._baseObj != null)
                settingsCanvas.transform.Find("EnableLaySit").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("EnableLaySit").GetComponent<Image>().color = custColor2;

            if (Main._DistHighPrec)
                settingsCanvas.transform.Find("Adj-Prec").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Adj-Prec").GetComponent<Image>().color = custColor2;

            if (Main.holdRotataion.Value)
                settingsCanvas.transform.Find("Rot-Lock").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Rot-Lock").GetComponent<Image>().color = custColor2;

            if (Main.joyMoveActive)
                settingsCanvas.transform.Find("Joy-Toggle").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Joy-Toggle").GetComponent<Image>().color = custColor2;

            settingsCanvas.transform.Find("Anim-Lay").GetComponent<Image>().color = custColor2;
            settingsCanvas.transform.Find("Anim-SitIdle").GetComponent<Image>().color = custColor2;
            settingsCanvas.transform.Find("Anim-SitCrossed").GetComponent<Image>().color = custColor2;
            settingsCanvas.transform.Find("Anim-BasicLegsDown").GetComponent<Image>().color = custColor2;
            switch (Main.SittingAnim.Value)
            {
                case "SitIdle": settingsCanvas.transform.Find("Anim-SitIdle").GetComponent<Image>().color = custColor1; break;
                case "SitCrossed": settingsCanvas.transform.Find("Anim-SitCrossed").GetComponent<Image>().color = custColor1; break;
                case "Laydown": settingsCanvas.transform.Find("Anim-Lay").GetComponent<Image>().color = custColor1; break;
                case "BasicSit": settingsCanvas.transform.Find("Anim-BasicLegsDown").GetComponent<Image>().color = custColor1; break;
                default: break;
            }

        }
    }
}
