using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using ABI_RC.Core.Savior;

namespace PortableMirror
{
    internal class QM
    {
        public static GameObject settings, settingsCanvas;
        public static GameObject settingsRight, settingsTop, settingsLeft;

        public static void CreateQuickMenuButton()
        {
            settings = GameObject.Instantiate(Mirrors.mirrorSettingsPrefab);
            settings.SetActive(false);

            settings.transform.SetParent(GameObject.Find("Cohtml/QuickMenu").transform, false);
            settings.transform.localScale = new Vector3(0.01f, 0.01f, 0.05f);
            settings.transform.localPosition = new Vector3(0f, 0f, 0f);

            settingsRight = settings.transform.Find("MenuRight").gameObject;
            settingsLeft = settings.transform.Find("MenuLeft").gameObject;
            settingsTop = settings.transform.Find("MenuTop").gameObject;




            if (Main.QMsmaller.Value)
            {//smoll
                QM.settingsRight.transform.localPosition = new Vector3(63f, 0f, -0.05f);  //Right
                QM.settingsTop.transform.localPosition = new Vector3(-30f, 77f, -0.05f); //Top
                QM.settingsLeft.transform.localPosition = new Vector3(-66f, 0f, -0.05f); //Left

                QM.settingsRight.transform.localScale = new Vector3(10f, 10f, 20f);
                QM.settingsTop.transform.localScale = new Vector3(10f, 10f, 20f);
                QM.settingsLeft.transform.localScale = new Vector3(10f, 10f, 20f);
            }
            else
            {//big
                QM.settingsRight.transform.localPosition = new Vector3(77f, 0f, -0.05f);
                QM.settingsTop.transform.localPosition = new Vector3(-42f, 105f, -0.05f);
                QM.settingsLeft.transform.localPosition = new Vector3(-77f, 0f, -0.05f);

                QM.settingsRight.transform.localScale = new Vector3(20f, 20f, 20f);
                QM.settingsTop.transform.localScale = new Vector3(20f, 20f, 20f);
                QM.settingsLeft.transform.localScale = new Vector3(20f, 20f, 20f);
            }



            var list = new GameObject[] { settingsRight, settingsLeft, settingsTop };
            foreach (var item in list)
            {

                settingsCanvas = item.transform.Find("SettingsMenuCanvas").gameObject;
                settingsCanvas.SetActive(Main.QMstartMax.Value);


                item.transform.Find("ToggleSettingsCanvas/ToggleSettings").GetComponent<Button>().onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
                {
                    var setCanvas = item.transform.Find("SettingsMenuCanvas").gameObject;
                    if(!setCanvas.activeSelf)
                        ParseSettings();
                    setCanvas.SetActive(!setCanvas.activeSelf);
                }
                ));

                //Base
                settingsCanvas.transform.Find("Base-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Mirrors.ToggleMirror();
                    ParseSettings();
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
                    if(Main._base_MirrorState.Value != "MirrorCutout")
                        Main._base_MirrorState.Value = "MirrorCutout";
                    else
                        Main._base_MirrorState.Value = "MirrorCutoutSolo";
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Base-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    switch(Main._base_MirrorState.Value)
                    {
                        case "MirrorTransparent" : Main._base_MirrorState.Value = "MirrorTransparentSolo"; break;
                        case "MirrorTransparentSolo": Main._base_MirrorState.Value = "MirrorTransCutCombo"; break;
                        case "MirrorTransCutCombo": Main._base_MirrorState.Value = "MirrorTransparent"; break;
                        default : Main._base_MirrorState.Value = "MirrorTransparent"; break;
                    }

                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Base-MinusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._base_MirrorDistance.Value -= Main._mirrorDistAdj;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Base-PlusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._base_MirrorDistance.Value += Main._mirrorDistAdj;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Base-MinusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._base_MirrorScaleX.Value > .25 && Main._base_MirrorScaleY.Value > .25)
                    {
                        Main._base_MirrorScaleX.Value -= .25f;
                        Main._base_MirrorScaleY.Value -= .25f;
                        Main main = new Main(); main.OnPreferencesSaved();
                        ParseSettings();
                    }

                }
                ));
                settingsCanvas.transform.Find("Base-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._base_MirrorScaleX.Value += .25f;
                    Main._base_MirrorScaleY.Value += .25f;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Base-Grab").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._base_CanPickupMirror.Value = !Main._base_CanPickupMirror.Value;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Base-Grab-Cust").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._base_CanPickupMirror.Value = !Main._base_CanPickupMirror.Value;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Base-ToTracking").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (!Main._base_AnchorToTracking.Value)
                        Main._base_AnchorToTracking.Value = true;
                    else
                    {
                        if (!Main._base_followGaze.Value && Main.enableGaze.Value)
                            Main._base_followGaze.Value = true;
                        else
                        {
                            Main._base_AnchorToTracking.Value = false;
                            Main._base_followGaze.Value = false;
                        }
                    }
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                //45
                settingsCanvas.transform.Find("45-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Mirrors.ToggleMirror45();
                    ParseSettings();
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
                    if (Main._45_MirrorState.Value != "MirrorCutout")
                        Main._45_MirrorState.Value = "MirrorCutout";
                    else
                        Main._45_MirrorState.Value = "MirrorCutoutSolo";
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("45-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    switch (Main._45_MirrorState.Value)
                    {
                        case "MirrorTransparent": Main._45_MirrorState.Value = "MirrorTransparentSolo"; break;
                        case "MirrorTransparentSolo": Main._45_MirrorState.Value = "MirrorTransCutCombo"; break;
                        case "MirrorTransCutCombo": Main._45_MirrorState.Value = "MirrorTransparent"; break;
                        default: Main._45_MirrorState.Value = "MirrorTransparent"; break;
                    }
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("45-MinusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._45_MirrorDistance.Value -= Main._mirrorDistAdj;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("45-PlusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._45_MirrorDistance.Value += Main._mirrorDistAdj;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("45-MinusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._45_MirrorScaleX.Value > .25 && Main._45_MirrorScaleY.Value > .25)
                    {
                        Main._45_MirrorScaleX.Value -= .25f;
                        Main._45_MirrorScaleY.Value -= .25f;
                        Main main = new Main(); main.OnPreferencesSaved();
                        ParseSettings();
                    }

                }
                ));
                settingsCanvas.transform.Find("45-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._45_MirrorScaleX.Value += .25f;
                    Main._45_MirrorScaleY.Value += .25f;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
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
                    if (!Main._45_AnchorToTracking.Value)
                        Main._45_AnchorToTracking.Value = true;
                    else
                    {
                        if (!Main._45_followGaze.Value && Main.enableGaze.Value)
                            Main._45_followGaze.Value = true;
                        else
                        {
                            Main._45_AnchorToTracking.Value = false;
                            Main._45_followGaze.Value = false;
                        }
                    }
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                //ceil
                settingsCanvas.transform.Find("Ceil-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Mirrors.ToggleMirrorCeiling();
                    ParseSettings();
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
                    if(Main._ceil_MirrorState.Value != "MirrorCutout")
                        Main._ceil_MirrorState.Value = "MirrorCutout";
                    else
                        Main._ceil_MirrorState.Value = "MirrorCutoutSolo";
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    switch (Main._ceil_MirrorState.Value)
                    {
                        case "MirrorTransparent": Main._ceil_MirrorState.Value = "MirrorTransparentSolo"; break;
                        case "MirrorTransparentSolo": Main._ceil_MirrorState.Value = "MirrorTransCutCombo"; break;
                        case "MirrorTransCutCombo": Main._ceil_MirrorState.Value = "MirrorTransparent"; break;
                        default: Main._ceil_MirrorState.Value = "MirrorTransparent"; break;
                    }
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Ceil-MinusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._ceil_MirrorDistance.Value -= Main._mirrorDistAdj;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Ceil-PlusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._ceil_MirrorDistance.Value += Main._mirrorDistAdj;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Ceil-MinusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._ceil_MirrorScaleX.Value > .25 && Main._ceil_MirrorScaleZ.Value > .25)
                    {
                        Main._ceil_MirrorScaleX.Value -= .25f;
                        Main._ceil_MirrorScaleZ.Value -= .25f;
                        Main main = new Main(); main.OnPreferencesSaved();
                        ParseSettings();
                    }

                }
                ));
                settingsCanvas.transform.Find("Ceil-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._ceil_MirrorScaleX.Value += .25f;
                    Main._ceil_MirrorScaleZ.Value += .25f;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
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
                    Mirrors.ToggleMirrorMicro();
                    ParseSettings();
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
                    if(Main._micro_MirrorState.Value != "MirrorCutout")
                        Main._micro_MirrorState.Value = "MirrorCutout";
                    else
                        Main._micro_MirrorState.Value = "MirrorCutoutSolo";
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                 ));
                settingsCanvas.transform.Find("Micro-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    switch (Main._micro_MirrorState.Value)
                    {
                        case "MirrorTransparent": Main._micro_MirrorState.Value = "MirrorTransparentSolo"; break;
                        case "MirrorTransparentSolo": Main._micro_MirrorState.Value = "MirrorTransCutCombo"; break;
                        case "MirrorTransCutCombo": Main._micro_MirrorState.Value = "MirrorTransparent"; break;
                        default: Main._micro_MirrorState.Value = "MirrorTransparent"; break;
                    }
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Micro-MinusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._micro_MirrorDistance.Value -= Main._mirrorDistAdj / 4;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Micro-PlusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._micro_MirrorDistance.Value += Main._mirrorDistAdj / 4;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();ParseSettings();
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
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Micro-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._micro_MirrorScaleX.Value += .01f;
                    Main._micro_MirrorScaleY.Value += .01f;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Micro-Grab").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._micro_CanPickupMirror.Value = !Main._micro_CanPickupMirror.Value;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Micro-Grab-Cust").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._micro_CanPickupMirror.Value = !Main._micro_CanPickupMirror.Value;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Micro-ToTracking").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (!Main._micro_AnchorToTracking.Value)
                        Main._micro_AnchorToTracking.Value = true;
                    else
                    {
                        if (!Main._micro_followGaze.Value && Main.enableGaze.Value)
                            Main._micro_followGaze.Value = true;
                        else
                        {
                            Main._micro_AnchorToTracking.Value = false;
                            Main._micro_followGaze.Value = false;
                        }
                    }
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                //trans
                settingsCanvas.transform.Find("Trans-Toggle").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Mirrors.ToggleMirrorTrans();
                    ParseSettings();
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
                    if(Main._trans_MirrorState.Value != "MirrorCutout")
                        Main._trans_MirrorState.Value = "MirrorCutout";
                    else
                        Main._trans_MirrorState.Value = "MirrorCutoutSolo";
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Trans-Trans").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    switch (Main._trans_MirrorState.Value)
                    {
                        case "MirrorTransparent": Main._trans_MirrorState.Value = "MirrorTransparentSolo"; break;
                        case "MirrorTransparentSolo": Main._trans_MirrorState.Value = "MirrorTransCutCombo"; break;
                        case "MirrorTransCutCombo": Main._trans_MirrorState.Value = "MirrorTransparent"; break;
                        default: Main._trans_MirrorState.Value = "MirrorTransparent"; break;
                    }
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Trans-MinusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._trans_MirrorDistance.Value -= Main._mirrorDistAdj;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Trans-PlusDist").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._trans_MirrorDistance.Value += Main._mirrorDistAdj;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Trans-MinusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (Main._trans_MirrorScaleX.Value > .25 && Main._trans_MirrorScaleY.Value > .25)
                    {
                        Main._trans_MirrorScaleX.Value -= .25f;
                        Main._trans_MirrorScaleY.Value -= .25f;
                        Main main = new Main(); main.OnPreferencesSaved();
                        ParseSettings();
                    }

                }
                ));
                settingsCanvas.transform.Find("Trans-PlusSize").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._trans_MirrorScaleX.Value += .25f;
                    Main._trans_MirrorScaleY.Value += .25f;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Trans-Grab").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._trans_CanPickupMirror.Value = !Main._trans_CanPickupMirror.Value;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Trans-Grab-Cust").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._trans_CanPickupMirror.Value = !Main._trans_CanPickupMirror.Value;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Trans-ToTracking").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    if (!Main._trans_AnchorToTracking.Value)
                        Main._trans_AnchorToTracking.Value = true;
                    else
                    {
                        if (!Main._trans_followGaze.Value && Main.enableGaze.Value)
                            Main._trans_followGaze.Value = true;
                        else
                        {
                            Main._trans_AnchorToTracking.Value = false;
                            Main._trans_followGaze.Value = false;
                        }
                    }
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

                //settingsCanvas.transform.Find("Sett-MirrorsShowInCamera").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                //{
                //    Main.MirrorsShowInCamera.Value = !Main.MirrorsShowInCamera.Value;
                //    Main main = new Main(); main.OnPreferencesSaved();
                //    ParseSettings();
                //}
                //));

                settingsCanvas.transform.Find("Sett-PositionOnView").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    bool temp = !Main._base_PositionOnView.Value;
                    Main._base_PositionOnView.Value = temp;
                    Main._micro_PositionOnView.Value = temp;
                    Main._trans_PositionOnView.Value = temp;
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
                    Mirrors.ForceMirrorLayer();
                }
                ));
                settingsCanvas.transform.Find("Sett-HighPrecisionDistanceAdjustment").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main._mirrorDistHighPrec = !Main._mirrorDistHighPrec;
                    Main main = new Main(); main.OnPreferencesSaved();
                    ParseSettings();
                }
                ));
                settingsCanvas.transform.Find("Sett-DistDisable").GetComponent<Button>().onClick.AddListener(new UnityAction(() =>
                {
                    Main.distanceDisable.Value = !Main.distanceDisable.Value;
                    Main main = new Main(); main.OnPreferencesSaved();
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

            //MirrorEn
            if(Main._mirrorBase != null)
                settingsCanvas.transform.Find("Base-Toggle").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Base-Toggle").GetComponent<Image>().color = custColor2;
            
            if (Main._mirror45 != null)
                settingsCanvas.transform.Find("45-Toggle").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("45-Toggle").GetComponent<Image>().color = custColor2;
            
            if (Main._mirrorCeiling != null)
                settingsCanvas.transform.Find("Ceil-Toggle").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Ceil-Toggle").GetComponent<Image>().color = custColor2;
            
            if (Main._mirrorMicro != null)
                settingsCanvas.transform.Find("Micro-Toggle").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Micro-Toggle").GetComponent<Image>().color = custColor2;

            if (Main._mirrorTrans != null)
                settingsCanvas.transform.Find("Trans-Toggle").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Trans-Toggle").GetComponent<Image>().color = custColor2;


            //State
            switch (Main._base_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Base-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutoutSolo":
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-CutSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("Base-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparentSolo":
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Base-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("Base-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransCutCombo":
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Base-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransCombo").gameObject.SetActive(true);
                    break;
                default:
                    settingsCanvas.transform.Find("Base-Full").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Base-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Base-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Base-TransCombo").gameObject.SetActive(false);
                    break; 
            }

            switch (Main._45_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("45-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutoutSolo":
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-CutSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("45-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparentSolo":
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("45-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("45-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransCutCombo":
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("45-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransCombo").gameObject.SetActive(true);
                    break;
                default:
                    settingsCanvas.transform.Find("45-Full").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("45-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("45-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("45-TransCombo").gameObject.SetActive(false);
                    break;
            }
            switch (Main._ceil_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Ceil-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutoutSolo":
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-CutSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("Ceil-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparentSolo":
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Ceil-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("Ceil-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransCutCombo":
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Ceil-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransCombo").gameObject.SetActive(true);
                    break;
                default:
                    settingsCanvas.transform.Find("Ceil-Full").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Ceil-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Ceil-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Ceil-TransCombo").gameObject.SetActive(false);
                    break;
            }
            switch (Main._micro_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Micro-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutoutSolo":
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-CutSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("Micro-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparentSolo":
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Micro-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("Micro-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransCutCombo":
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Micro-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransCombo").gameObject.SetActive(true);
                    break;
                default:
                    settingsCanvas.transform.Find("Micro-Full").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Micro-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Micro-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Micro-TransCombo").gameObject.SetActive(false);
                    break;
            }
            switch (Main._trans_MirrorState.Value)
            {
                case "MirrorOpt":
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutout":
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparent":
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Trans-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorCutoutSolo":
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-CutSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("Trans-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransparentSolo":
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Trans-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransSolo").gameObject.SetActive(true);
                    settingsCanvas.transform.Find("Trans-TransCombo").gameObject.SetActive(false);
                    break;
                case "MirrorTransCutCombo":
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Trans-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransCombo").gameObject.SetActive(true);
                    break;
                default:
                    settingsCanvas.transform.Find("Trans-Full").GetComponent<Image>().color = custColor1;
                    settingsCanvas.transform.Find("Trans-Opt").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Cut").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-Trans").GetComponent<Image>().color = custColor2;
                    settingsCanvas.transform.Find("Trans-CutSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransSolo").gameObject.SetActive(false);
                    settingsCanvas.transform.Find("Trans-TransCombo").gameObject.SetActive(false);
                    break;
            }






            //Pickup
            if (Main._base_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Base-Grab").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Base-Grab").GetComponent<Image>().color = custColor2;

            if (Main._45_CanPickupMirror.Value)
                settingsCanvas.transform.Find("45-Grab").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("45-Grab").GetComponent<Image>().color = custColor2;
            if (Main._ceil_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Ceil-Grab").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Ceil-Grab").GetComponent<Image>().color = custColor2;
            if (Main._micro_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Micro-Grab").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Micro-Grab").GetComponent<Image>().color = custColor2;
            if (Main._trans_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Trans-Grab").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Trans-Grab").GetComponent<Image>().color = custColor2;
            //Grab Custom
            if (Main._base_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Base-Grab-Cust").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Base-Grab-Cust").GetComponent<Image>().color = custColor2;
            if (Main._micro_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Micro-Grab-Cust").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Micro-Grab-Cust").GetComponent<Image>().color = custColor2;
            if (Main._trans_CanPickupMirror.Value)
                settingsCanvas.transform.Find("Trans-Grab-Cust").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Trans-Grab-Cust").GetComponent<Image>().color = custColor2;
            //
            bool custGrab = MetaPort.Instance.isUsingVr && Main._base_followGaze.Value;
            settingsCanvas.transform.Find("Base-Grab").gameObject.SetActive(!custGrab);
            settingsCanvas.transform.Find("Base-Grab-Cust").gameObject.SetActive(custGrab);
            settingsCanvas.transform.Find("Micro-Grab").gameObject.SetActive(!custGrab);
            settingsCanvas.transform.Find("Micro-Grab-Cust").gameObject.SetActive(custGrab);
            //settingsCanvas.transform.Find("Trans-Grab").gameObject.SetActive(!custGrab);
            //settingsCanvas.transform.Find("Trans-Grab-Cust").gameObject.SetActive(custGrab);

            //ToTracking
            if (Main._base_AnchorToTracking.Value)
                settingsCanvas.transform.Find("Base-ToTracking").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Base-ToTracking").GetComponent<Image>().color = custColor2;

            if (Main._45_AnchorToTracking.Value)
                settingsCanvas.transform.Find("45-ToTracking").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("45-ToTracking").GetComponent<Image>().color = custColor2;
            if (Main._ceil_AnchorToTracking.Value)
                settingsCanvas.transform.Find("Ceil-ToTracking").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Ceil-ToTracking").GetComponent<Image>().color = custColor2;
            if (Main._micro_AnchorToTracking.Value)
                settingsCanvas.transform.Find("Micro-ToTracking").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Micro-ToTracking").GetComponent<Image>().color = custColor2;
            if (Main._trans_AnchorToTracking.Value)
                settingsCanvas.transform.Find("Trans-ToTracking").GetComponent<Image>().color = custColor1;
            else
                settingsCanvas.transform.Find("Trans-ToTracking").GetComponent<Image>().color = custColor2;

            //Gaze
            settingsCanvas.transform.Find("Base-Gaze").gameObject.SetActive(Main._base_followGaze.Value);
            settingsCanvas.transform.Find("45-Gaze").gameObject.SetActive(Main._45_followGaze.Value);
            settingsCanvas.transform.Find("Micro-Gaze").gameObject.SetActive(Main._micro_followGaze.Value);
            settingsCanvas.transform.Find("Trans-Gaze").gameObject.SetActive(Main._trans_followGaze.Value);



            //Dist
            settingsCanvas.transform.Find("Base-Dist").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._base_MirrorDistance.Value);
            settingsCanvas.transform.Find("45-Dist").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._45_MirrorDistance.Value);
            settingsCanvas.transform.Find("Ceil-Dist").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._ceil_MirrorDistance.Value);
            settingsCanvas.transform.Find("Micro-Dist").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._micro_MirrorDistance.Value);
            settingsCanvas.transform.Find("Trans-Dist").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._trans_MirrorDistance.Value);

            //Size
            settingsCanvas.transform.Find("Base-Size").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._base_MirrorScaleX.Value) + "x" +
                Utils.RoundFloatToString(Main._base_MirrorScaleY.Value);
            settingsCanvas.transform.Find("45-Size").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._45_MirrorScaleX.Value) + "x" +
                Utils.RoundFloatToString(Main._45_MirrorScaleY.Value);
            settingsCanvas.transform.Find("Ceil-Size").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._ceil_MirrorScaleX.Value) + "x" +
                Utils.RoundFloatToString(Main._ceil_MirrorScaleZ.Value);
            settingsCanvas.transform.Find("Micro-Size").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._micro_MirrorScaleX.Value) + "x" +
                Utils.RoundFloatToString(Main._micro_MirrorScaleY.Value);
            settingsCanvas.transform.Find("Trans-Size").GetComponent<TextMeshProUGUI>().text = Utils.RoundFloatToString(Main._trans_MirrorScaleX.Value) + "x" +
                Utils.RoundFloatToString(Main._trans_MirrorScaleY.Value);

            //Mics Settings
            if (Main.PickupToHand.Value)
                settingsCanvas.transform.Find("Sett-PickupToHand/Text").GetComponent<TextMeshProUGUI>().color = custColor1;
            else
                settingsCanvas.transform.Find("Sett-PickupToHand/Text").GetComponent<TextMeshProUGUI>().color = custColor2;

            if (Main.usePixelLights.Value)
                settingsCanvas.transform.Find("Sett-usePixelLights/Text").GetComponent<TextMeshProUGUI>().color = custColor1;
            else
                settingsCanvas.transform.Find("Sett-usePixelLights/Text").GetComponent<TextMeshProUGUI>().color = custColor2;

            //if (Main.MirrorsShowInCamera.Value)
            //    settingsCanvas.transform.Find("Sett-MirrorsShowInCamera/Text").GetComponent<TextMeshProUGUI>().color = custColor1;
            //else
            //    settingsCanvas.transform.Find("Sett-MirrorsShowInCamera/Text").GetComponent<TextMeshProUGUI>().color = custColor2;
;
            if (Main._base_PositionOnView.Value)
                settingsCanvas.transform.Find("Sett-PositionOnView/Text").GetComponent<TextMeshProUGUI>().color = custColor1;
            else
                settingsCanvas.transform.Find("Sett-PositionOnView/Text").GetComponent<TextMeshProUGUI>().color = custColor2;

            if (Main._mirrorDistHighPrec)
                settingsCanvas.transform.Find("Sett-HighPrecisionDistanceAdjustment/Text").GetComponent<TextMeshProUGUI>().color = custColor1;
            else
                settingsCanvas.transform.Find("Sett-HighPrecisionDistanceAdjustment/Text").GetComponent<TextMeshProUGUI>().color = custColor2;

            if (Main.distanceDisable.Value)
                settingsCanvas.transform.Find("Sett-DistDisable/Text").GetComponent<TextMeshProUGUI>().color = custColor1;
            else
                settingsCanvas.transform.Find("Sett-DistDisable/Text").GetComponent<TextMeshProUGUI>().color = custColor2;


            //if (Main.fixRenderOrder.Value)
            //    settingsCanvas.transform.Find("Sett-fixRenderOrder/Text").GetComponent<TextMeshProUGUI>().color = custColor1;
            //else
            //    settingsCanvas.transform.Find("Sett-fixRenderOrder/Text").GetComponent<TextMeshProUGUI>().color = custColor2;

        }



    }
}
