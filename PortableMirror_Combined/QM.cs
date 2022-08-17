using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace PortableMirror
{
    internal class QM
    {
        public static GameObject settings, settingsCanvas;

        public static void CreateQuickMenuButton()
        {
            settings = GameObject.Instantiate(Main.mirrorSettingsPrefab);
            settings.SetActive(false);

            settings.transform.SetParent(GameObject.Find("Cohtml/QuickMenu").transform, false);
            switch (Main.QMposition.Value)
            {
                case 1: if (Main.QMsmaller.Value) QM.settings.transform.localPosition = new Vector3(-0.35f, 0.55f, 0f); else QM.settings.transform.localPosition = new Vector3(-0.3f, 0.55f, 0f); break; //Top
                case 2: if (Main.QMsmaller.Value) QM.settings.transform.localPosition = new Vector3(-0.85f, -0.3f, 0f); else QM.settings.transform.localPosition = new Vector3(-1.1f, -0.45f, 0f); break; //Left
                default: if (Main.QMsmaller.Value) QM.settings.transform.localPosition = new Vector3(0.5f, -0.3f, 0f); else QM.settings.transform.localPosition = new Vector3(0.5f, -0.45f, 0f); break; //Right
            }
            if (Main.QMsmaller.Value)
                settings.transform.localScale = new Vector3(0.007f, 0.007f, 0.01f);
            else
                settings.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            settingsCanvas = settings.transform.Find("SettingsMenuCanvas").gameObject;
            settingsCanvas.SetActive(Main.QMstartMax.Value);

            settings.transform.Find("ToggleSettingsCanvas/ToggleSettings").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() => 
            {
                settingsCanvas.SetActive(!settingsCanvas.activeSelf);
            }
            ));

            //Base
            settingsCanvas.transform.Find("Base-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main.ToggleMirror();
            }
            ));


            settingsCanvas.transform.Find("Base-Full").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_MirrorState.Value = "MirrorFull";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Base-Opt").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_MirrorState.Value = "MirrorOpt";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Base-Cut").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_MirrorState.Value = "MirrorCutout";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Base-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_MirrorState.Value = "MirrorTransparent";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Base-MinusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_MirrorDistance.Value -= Main._mirrorDistAdj;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Base-PlusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_MirrorDistance.Value += Main._mirrorDistAdj;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Base-MinusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                if (Main._base_MirrorScaleX.Value > .25 && Main._base_MirrorScaleY.Value > .25)
                {
                    Main._base_MirrorScaleX.Value -= .25f;
                    Main._base_MirrorScaleY.Value -= .25f;
                    Main main = new Main(); main.OnPreferencesSaved();
                }

            }
            ));
            settingsCanvas.transform.Find("Base-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_MirrorScaleX.Value += .25f;
                Main._base_MirrorScaleY.Value += .25f;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Base-Grab").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_CanPickupMirror.Value = !Main._base_CanPickupMirror.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Base-ToTracking").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_AnchorToTracking.Value = !Main._base_AnchorToTracking.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            //45
            settingsCanvas.transform.Find("45-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main.ToggleMirror45();
            }
            ));
            settingsCanvas.transform.Find("45-Full").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._45_MirrorState.Value = "MirrorFull";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("45-Opt").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._45_MirrorState.Value = "MirrorOpt";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("45-Cut").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._45_MirrorState.Value = "MirrorCutout";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("45-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._45_MirrorState.Value = "MirrorTransparent";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("45-MinusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._45_MirrorDistance.Value -= Main._mirrorDistAdj;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("45-PlusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._45_MirrorDistance.Value += Main._mirrorDistAdj;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("45-MinusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                if (Main._45_MirrorScaleX.Value > .25 && Main._45_MirrorScaleY.Value > .25)
                {
                    Main._45_MirrorScaleX.Value -= .25f;
                    Main._45_MirrorScaleY.Value -= .25f;
                    Main main = new Main(); main.OnPreferencesSaved();
                }

            }
            ));
            settingsCanvas.transform.Find("45-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._45_MirrorScaleX.Value += .25f;
                Main._45_MirrorScaleY.Value += .25f;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("45-Grab").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._45_CanPickupMirror.Value = !Main._45_CanPickupMirror.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("45-ToTracking").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._45_AnchorToTracking.Value = !Main._45_AnchorToTracking.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            //ceil
            settingsCanvas.transform.Find("Ceil-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main.ToggleMirrorCeiling();
            }
            ));
            settingsCanvas.transform.Find("Ceil-Full").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._ceil_MirrorState.Value = "MirrorFull";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._ceil_MirrorState.Value = "MirrorOpt";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._ceil_MirrorState.Value = "MirrorCutout";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._ceil_MirrorState.Value = "MirrorTransparent";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Ceil-MinusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._ceil_MirrorDistance.Value -= Main._mirrorDistAdj;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Ceil-PlusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._ceil_MirrorDistance.Value += Main._mirrorDistAdj;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Ceil-MinusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                if (Main._ceil_MirrorScaleX.Value > .25 && Main._ceil_MirrorScaleZ.Value > .25)
                {
                    Main._ceil_MirrorScaleX.Value -= .25f;
                    Main._ceil_MirrorScaleZ.Value -= .25f;
                    Main main = new Main(); main.OnPreferencesSaved();
                }

            }
            ));
            settingsCanvas.transform.Find("Ceil-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._ceil_MirrorScaleX.Value += .25f;
                Main._ceil_MirrorScaleZ.Value += .25f;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Ceil-Grab").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._ceil_CanPickupMirror.Value = !Main._ceil_CanPickupMirror.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Ceil-ToTracking").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._ceil_AnchorToTracking.Value = !Main._ceil_AnchorToTracking.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            //micro
            settingsCanvas.transform.Find("Micro-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main.ToggleMirrorMicro();
            }
            ));
            settingsCanvas.transform.Find("Micro-Full").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._micro_MirrorState.Value = "MirrorFull";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Micro-Opt").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._micro_MirrorState.Value = "MirrorOpt";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Micro-Cut").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._micro_MirrorState.Value = "MirrorCutout";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
             ));
            settingsCanvas.transform.Find("Micro-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._micro_MirrorState.Value = "MirrorTransparent";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Micro-MinusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                if (Main._micro_MirrorScaleX.Value > .02 && Main._micro_MirrorScaleY.Value > .02)
                {
                    Main._micro_MirrorScaleX.Value -= .01f;
                    Main._micro_MirrorScaleY.Value -= .01f;
                }
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Micro-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._micro_MirrorScaleX.Value += .01f;
                Main._micro_MirrorScaleY.Value += .01f;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Micro-Grab").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._micro_CanPickupMirror.Value = !Main._micro_CanPickupMirror.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Micro-ToTracking").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._micro_AnchorToTracking.Value = !Main._micro_AnchorToTracking.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            //trans
            settingsCanvas.transform.Find("Trans-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main.ToggleMirrorTrans();
            }
            ));


            settingsCanvas.transform.Find("Trans-Full").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._trans_MirrorState.Value = "MirrorFull";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Trans-Opt").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._trans_MirrorState.Value = "MirrorOpt";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Trans-Cut").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._trans_MirrorState.Value = "MirrorCutout";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Trans-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._trans_MirrorState.Value = "MirrorTransparent";
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Trans-MinusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._trans_MirrorDistance.Value -= Main._mirrorDistAdj;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Trans-PlusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._trans_MirrorDistance.Value += Main._mirrorDistAdj;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Trans-MinusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                if (Main._trans_MirrorScaleX.Value > .25 && Main._trans_MirrorScaleY.Value > .25)
                {
                    Main._trans_MirrorScaleX.Value -= .25f;
                    Main._trans_MirrorScaleY.Value -= .25f;
                    Main main = new Main(); main.OnPreferencesSaved();
                }

            }
            ));
            settingsCanvas.transform.Find("Trans-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._trans_MirrorScaleX.Value += .25f;
                Main._trans_MirrorScaleY.Value += .25f;
                Main main = new Main(); main.OnPreferencesSaved();
            }
            ));
            settingsCanvas.transform.Find("Trans-Grab").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._trans_CanPickupMirror.Value = !Main._trans_CanPickupMirror.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));
            settingsCanvas.transform.Find("Trans-ToTracking").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._trans_AnchorToTracking.Value = !Main._trans_AnchorToTracking.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));



            ///Mics Settings
            
            settingsCanvas.transform.Find("TransMinus").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                if (Main.TransMirrorTrans.Value >= .1f)
                    Main.TransMirrorTrans.Value -= .1f;
                else
                    Main.TransMirrorTrans.Value = 0f;
                Main main = new Main(); main.OnPreferencesSaved();
            }
           ));
            settingsCanvas.transform.Find("TransPlus").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                if (Main.TransMirrorTrans.Value < 1f)
                    Main.TransMirrorTrans.Value += .1f;
                else
                    Main.TransMirrorTrans.Value = 1f;
                Main main = new Main(); main.OnPreferencesSaved();
            }
           ));

            settingsCanvas.transform.Find("Sett-PickupToHand").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main.PickupToHand.Value = !Main.PickupToHand.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));

            settingsCanvas.transform.Find("Sett-usePixelLights").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main.usePixelLights.Value = !Main.usePixelLights.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            )); 

            settingsCanvas.transform.Find("Sett-MirrorsShowInCamera").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main.MirrorsShowInCamera.Value = !Main.MirrorsShowInCamera.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));

            settingsCanvas.transform.Find("Sett-PositionOnView").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._base_PositionOnView.Value = !Main._base_PositionOnView.Value;
                Main._micro_PositionOnView.Value = !Main._micro_PositionOnView.Value;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));

            //settingsCanvas.transform.Find("Sett-fixRenderOrder").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            //{
            //    Main.fixRenderOrder.Value = !Main.fixRenderOrder.Value;
            //    Main main = new Main(); main.OnPreferencesSaved();
            //    ParseSettings();
            //}
            //));

            settingsCanvas.transform.Find("Sett-forceMirrors").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main.ForceMirrorLayer();
            }
            ));
            settingsCanvas.transform.Find("Sett-HighPrecisionDistanceAdjustment").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
            {
                Main._mirrorDistHighPrec = !Main._mirrorDistHighPrec;
                Main main = new Main(); main.OnPreferencesSaved();
                ParseSettings();
            }
            ));

           


        }



        public static void ParseSettings()
        {
            var custColor = new Color(.18f, .18f, .18f);
            //State
            switch (Main._base_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor; 
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor; 
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = Color.white; 
                    break;
                default:
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor;
                    break; 
            }

            switch (Main._45_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor;
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor;
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = Color.white;
                    break;
                default:
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor;
                    break;
            }
            switch (Main._ceil_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor;
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor;
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = Color.white;
                    break;
                default:
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor;
                    break;
            }
            switch (Main._micro_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor;
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor;
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = Color.white;
                    break;
                default:
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor;
                    break;
            }
            switch (Main._trans_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor;
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor;
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = Color.white;
                    break;
                default:
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = Color.white;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor;
                    break;
            }






            //Pickup
            if (Main._base_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Base-Grab").GetComponent<Image>().color = Color.white;
            else
                settingsCanvas.transform.Find("Base-Grab").GetComponent<Image>().color = Color.gray;

            if (Main._45_CanPickupMirror.Value)
                settingsCanvas.transform.Find("45-Grab").GetComponent<Image>().color = Color.white;
            else
                settingsCanvas.transform.Find("45-Grab").GetComponent<Image>().color = Color.gray;
            if (Main._ceil_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Ceil-Grab").GetComponent<Image>().color = Color.white;
            else
                settingsCanvas.transform.Find("Ceil-Grab").GetComponent<Image>().color = Color.gray;
            if (Main._micro_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Micro-Grab").GetComponent<Image>().color = Color.white;
            else
                settingsCanvas.transform.Find("Micro-Grab").GetComponent<Image>().color = Color.gray;
    
            //ToTracking
            if (Main._base_AnchorToTracking.Value)
                settingsCanvas.transform.Find("Base-ToTracking").GetComponent<Image>().color = Color.white;
            else
                settingsCanvas.transform.Find("Base-ToTracking").GetComponent<Image>().color = Color.gray;

            if (Main._45_AnchorToTracking.Value)
                settingsCanvas.transform.Find("45-ToTracking").GetComponent<Image>().color = Color.white;
            else
                settingsCanvas.transform.Find("45-ToTracking").GetComponent<Image>().color = Color.gray;
            if (Main._ceil_AnchorToTracking.Value)
                settingsCanvas.transform.Find("Ceil-ToTracking").GetComponent<Image>().color = Color.white;
            else
                settingsCanvas.transform.Find("Ceil-ToTracking").GetComponent<Image>().color = Color.gray;
            if (Main._micro_AnchorToTracking.Value)
                settingsCanvas.transform.Find("Micro-ToTracking").GetComponent<Image>().color = Color.white;
            else
                settingsCanvas.transform.Find("Micro-ToTracking").GetComponent<Image>().color = Color.gray;


            //Mics Settings
            if (Main.PickupToHand.Value)
                settingsCanvas.transform.Find("Sett-PickupToHand/Text").GetComponent<TextMeshProUGUI>().color = Color.white;
            else
                settingsCanvas.transform.Find("Sett-PickupToHand/Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

            if (Main.usePixelLights.Value)
                settingsCanvas.transform.Find("Sett-usePixelLights/Text").GetComponent<TextMeshProUGUI>().color = Color.white;
            else
                settingsCanvas.transform.Find("Sett-usePixelLights/Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

            if (Main.MirrorsShowInCamera.Value)
                settingsCanvas.transform.Find("Sett-MirrorsShowInCamera/Text").GetComponent<TextMeshProUGUI>().color = Color.white;
            else
                settingsCanvas.transform.Find("Sett-MirrorsShowInCamera/Text").GetComponent<TextMeshProUGUI>().color = Color.gray;
;
            if (Main._base_PositionOnView.Value)
                settingsCanvas.transform.Find("Sett-PositionOnView/Text").GetComponent<TextMeshProUGUI>().color = Color.white;
            else
                settingsCanvas.transform.Find("Sett-PositionOnView/Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

            if (Main._mirrorDistHighPrec)
                settingsCanvas.transform.Find("Sett-HighPrecisionDistanceAdjustment/Text").GetComponent<TextMeshProUGUI>().color = Color.white;
            else
                settingsCanvas.transform.Find("Sett-HighPrecisionDistanceAdjustment/Text").GetComponent<TextMeshProUGUI>().color = Color.gray;

            //if (Main.fixRenderOrder.Value)
            //    settingsCanvas.transform.Find("Sett-fixRenderOrder/Text").GetComponent<TextMeshProUGUI>().color = Color.white;
            //else
            //    settingsCanvas.transform.Find("Sett-fixRenderOrder/Text").GetComponent<TextMeshProUGUI>().color = custColor;

        }

       

    }
}
