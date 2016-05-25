﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Miwalab.ShadowGroup.ImageProcesser;
using UnityEngine.UI;
using Miwalab.ShadowGroup.Scripts.Sensors;
using Miwalab.ShadowGroup.Scripts.Callibration;
using Miwalab.ShadowGroup.AfterEffect;
using Miwalab.ShadowGroup.Archive;

public class UIHost : MonoBehaviour
{

    #region singleton
    private static Dictionary<string, AParameterUI> m_ParameterUI = new Dictionary<string, AParameterUI>();
    private static float m_lastUpdatedHeight;

    public static Dictionary<string, AParameterUI> GetInstance()
    {
        return m_ParameterUI;
    }

    public static void AddUI(string key, AParameterUI UIObject)
    {
        m_ParameterUI.Add(key, UIObject);
    }
    public static AParameterUI GetUI(string key)
    {
        return m_ParameterUI[key];
    }

    public static void ResetUI()
    {
        foreach (var p in m_ParameterUI)
        {
            GameObject.DestroyImmediate(p.Value.gameObject);
        }
        m_ParameterUI.Clear();

    }
    #endregion

    public ParameterSlider m_slider;
    public ParameterCheckbox m_checkbox;
    public ParameterButton m_button;
    public ParameterText m_text;

    public ASensorImporter m_Sensor;

    public Dropdown ImageProcessingMenu;
    public Dropdown ImportSettingMenu;
    public Dropdown CallibrationSettingMenu;
    public Dropdown AfterEffectSettingMenu;
    public Dropdown ArchiveSettingMenu;
    public ShadowMeshRenderer m_meshrenderer;

    public GameObject SettingPanel;
    public Canvas MainCanvas;

    private List<Dropdown> m_MenuList;
    private Dictionary<string, GameObject> m_PanelDictionary;
    public void Start()
    {
        ResetUI();
        m_MenuList = new List<Dropdown>();
        m_PanelDictionary = new Dictionary<string, GameObject>();

        for (int i = 0; i < (int)ImageProcesserType.Count; ++i)
        {
            this.ImageProcessingMenu.options.Add(new Dropdown.OptionData(((ImageProcesserType)i).ToString()));
        }
        for (int i = 0; i < (int)ImportSettingType.Count; ++i)
        {
            this.ImportSettingMenu.options.Add(new Dropdown.OptionData(((ImportSettingType)i).ToString()));
        }
        for (int i = 0; i < (int)CallibrationSettingType.Count; ++i)
        {
            this.CallibrationSettingMenu.options.Add(new Dropdown.OptionData(((CallibrationSettingType)i).ToString()));
        }
        for (int i = 0; i < (int)AfterEffectSettingType.Count; ++i)
        {
            this.AfterEffectSettingMenu.options.Add(new Dropdown.OptionData(((AfterEffectSettingType)i).ToString()));
        }
        for (int i = 0; i < (int)ArchiveSettingType.Count; ++i)
        {
            this.ArchiveSettingMenu.options.Add(new Dropdown.OptionData(((ArchiveSettingType)i).ToString()));
        }
        m_MenuList.Add(this.ImageProcessingMenu);
        m_MenuList.Add(this.ImportSettingMenu);
        m_MenuList.Add(this.CallibrationSettingMenu);
        m_MenuList.Add(this.AfterEffectSettingMenu);
        m_MenuList.Add(this.ArchiveSettingMenu);

        foreach (var menu in this.m_MenuList)
        {
            foreach (var option in menu.options)
            {
                GameObject item = Instantiate(SettingPanel);
                item.transform.SetParent(MainCanvas.gameObject.transform, false);
                m_PanelDictionary.Add(option.text, item);
                item.SetActive(false);
            }
        }
        this.m_currentImageProcesserSettingPanel = m_PanelDictionary[ImageProcesserType.Normal.ToString()];
        this.m_currentImportSettingPanel = m_PanelDictionary[ImportSettingType.Kinect.ToString()];
        this.m_currentCallibrationSettingPanel = m_PanelDictionary[CallibrationSettingType.CallibrationImport.ToString()];
        this.m_currentAfterEffectSettingPanel = m_PanelDictionary[AfterEffectSettingType.Fade.ToString()];
        this.m_currentArchiveSettingPanel = m_PanelDictionary[ArchiveSettingType.Save.ToString()];


        //UI初期化
        this.CreateUIsImageporcessingNormal(m_PanelDictionary[ImageProcesserType.Normal.ToString()]);
        this.CreateUIsImageporcessingTimeDelay(m_PanelDictionary[ImageProcesserType.TimeDelay.ToString()]);

        this.CreateUIsImageporcessingSpike(m_PanelDictionary[ImageProcesserType.Spike.ToString()]);
        this.CreateUIsImageporcessingPolygon(m_PanelDictionary[ImageProcesserType.Polygon.ToString()]);
        this.CreateUIsImageporcessingZanzou(m_PanelDictionary[ImageProcesserType.DoubleAfterImage.ToString()]);
        this.CreateUIsImageporcessingTamuraSkeleton(m_PanelDictionary[ImageProcesserType.TamuraSkeleton.ToString()]);
        this.CreateUIsImageporcessingParticle(m_PanelDictionary[ImageProcesserType.Particle.ToString()]);


        this.CreateUIsImportKinectv2(m_PanelDictionary[ImportSettingType.Kinect.ToString()]);

        this.CreateUIsCallibrationImport(m_PanelDictionary[CallibrationSettingType.CallibrationImport.ToString()]);
        this.CreateUIsCallibrationExport(m_PanelDictionary[CallibrationSettingType.CallibrationExport.ToString()]);

        this.CreateUIsAffterEffectFade(m_PanelDictionary[AfterEffectSettingType.Fade.ToString()]);

        this.CreateUIsArchiveSave(m_PanelDictionary[ArchiveSettingType.Save.ToString()]);
        this.CreateUIsArchivePlay(m_PanelDictionary[ArchiveSettingType.Play.ToString()]);


        this.m_meshrenderer.SetUpUIs();
        this.m_Sensor.setUpUI();

        this.ImageProcesserSettingPanelSet(false);
        this.ImportSettingPanelSet(false);
        this.CallibrationSettingPanelSet(false);
        this.m_Sensor.AddImageProcesser(new Normal());

    }


    public void ChangeImageProcessingOptionTo(int number)
    {
        ImageProcesserType type = (ImageProcesserType)number;
        switch (type)
        {
            case ImageProcesserType.Normal:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(),m_Sensor, new Normal()));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Normal.ToString()];
                break;
            case ImageProcesserType.VividNormal:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(),m_Sensor, new VividNormal()));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.VividNormal.ToString()];
                break;
            case ImageProcesserType.CellAutomaton:
                break;
            case ImageProcesserType.Polygon:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new Polygon()));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Polygon.ToString()];
                break;
            case ImageProcesserType.DoubleAfterImage:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(),m_Sensor, new Zanzou()));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.DoubleAfterImage.ToString()];
                break;
            case ImageProcesserType.TimeDelay:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(),m_Sensor, new Timedelay()));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.TimeDelay.ToString()];
                break;
            case ImageProcesserType.TamuraSkeleton:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(),m_Sensor, new TamuraSkelton()));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.TamuraSkeleton.ToString()];
                break;
            case ImageProcesserType.Spike:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(),m_Sensor, new Spike()));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Spike.ToString()];
                break;
            case ImageProcesserType.Particle:
                this.m_Sensor.AddAfterEffect(new FadeTransition(this.m_Sensor.GetAffterEffectList(), m_Sensor, new Miwalab.ShadowGroup.ImageProcesser.Particle()));
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Particle.ToString()];
                break;
        }
        this.SwitchOffOtherPanelsExceptOf(this.m_currentImageProcesserSettingPanel);

    }
    public void ChangeImportSettingOptionTo(int number)
    {
        ImportSettingType type = (ImportSettingType)number;

        switch (type)
        {
            case ImportSettingType.Kinect:
                //一回作って使いまわす
                this.m_currentImportSettingPanel = this.m_PanelDictionary[ImportSettingType.Kinect.ToString()];
                this.SwitchOffOtherPanelsExceptOf(this.m_currentImportSettingPanel);
                break;
            case ImportSettingType.Camera:
                this.m_currentImportSettingPanel = this.m_PanelDictionary[ImportSettingType.Camera.ToString()];
                this.SwitchOffOtherPanelsExceptOf(this.m_currentImportSettingPanel);
                break;

        }

    }

    public void ChangeCallibrationSettingOptionTo(int number)
    {
        CallibrationSettingType type = (CallibrationSettingType)number;

        switch (type)
        {
            case CallibrationSettingType.CallibrationImport:
                //一回作って使いまわす
                this.m_currentCallibrationSettingPanel = this.m_PanelDictionary[CallibrationSettingType.CallibrationImport.ToString()];
                this.SwitchOffOtherPanelsExceptOf(this.m_currentCallibrationSettingPanel);
                break;
            case CallibrationSettingType.CallibrationExport:
                this.m_currentCallibrationSettingPanel = this.m_PanelDictionary[CallibrationSettingType.CallibrationExport.ToString()];
                this.SwitchOffOtherPanelsExceptOf(this.m_currentCallibrationSettingPanel);
                break;

        }

    }

    public void ChangeFadeSettingOptionTo(int number)
    {
        AfterEffectSettingType type = (AfterEffectSettingType)number;

        switch (type)
        {
            case AfterEffectSettingType.Fade:
                //一回作って使いまわす
                this.m_currentCallibrationSettingPanel = this.m_PanelDictionary[AfterEffectSettingType.Fade.ToString()];
                this.SwitchOffOtherPanelsExceptOf(this.m_currentAfterEffectSettingPanel);
                break;
            
        }

    }
    public void ChangeArchiveSettingOptionTo(int number)
    {
        ArchiveSettingType type = (ArchiveSettingType)number;

        switch (type)
        {
            case ArchiveSettingType.Save:
                //一回作って使いまわす
                this.m_currentArchiveSettingPanel = this.m_PanelDictionary[ArchiveSettingType.Save.ToString()];
                this.SwitchOffOtherPanelsExceptOf(this.m_currentArchiveSettingPanel);
                break;

            case ArchiveSettingType.Play:
                //一回作って使いまわす
                this.m_currentArchiveSettingPanel = this.m_PanelDictionary[ArchiveSettingType.Play.ToString()];
                this.SwitchOffOtherPanelsExceptOf(this.m_currentArchiveSettingPanel);
                break;
        }

    }

    private void SwitchOffOtherPanelsExceptOf(GameObject currentPanel)
    {
        foreach (var panel in m_PanelDictionary)
        {
            panel.Value.SetActive(false);
        }
        currentPanel.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    #region createUIMethods


    private void CreateUIsArchiveSave(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddButtonUI(parent, "ChooseSaveFile");
        //AddTextUI(parent, "SaveFolderPath");
        
        AddButtonUI(parent, "SaveStart");
        AddButtonUI(parent, "SaveStop");

    }
    private void CreateUIsArchivePlay(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddButtonUI(parent, "ChooseFile");
        AddButtonUI(parent, "PlayStart");
        AddBooleanUI(parent, "Pause", false);

    }

    private void CreateUIsImageporcessingParticle(GameObject parent)
    {
        m_lastUpdatedHeight = 0;

        AddFloatUI(parent, "Interval_of_Contour", 100, 1, 5);
        AddFloatUI(parent, "Velocity", 100, 0, 10);
        AddFloatUI(parent, "Lifetime_Frame", 100, 1, 30);
        AddFloatUI(parent, "threthOPFsize", 5000, 0, 1500);
        AddFloatUI(parent, "Particle_bgd_R", 255, 0, 255);
        AddFloatUI(parent, "Particle_bgd_G", 255, 0, 255);
        AddFloatUI(parent, "Particle_bgd_B", 255, 0, 255);

    }

    private void CreateUIsAffterEffectFade(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Frame_of_FadeIn", 1000, 1, 100);
        AddFloatUI(parent, "Frame_of_FadeOut", 1000, 1, 100);
        AddBooleanUI(parent, "White_Fade", false);
    }

    private void CreateUIsImageporcessingTamuraSkeleton(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddBooleanUI(parent, "TamuraSkeleton_Invert", false);
    }

    private void CreateUIsImageporcessingZanzou(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Zanzou_ins_R", 255, 0, 255);
        AddFloatUI(parent, "Zanzou_ins_G", 255, 0, 255);
        AddFloatUI(parent, "Zanzou_ins_B", 255, 0, 255);
        AddFloatUI(parent, "Zanzou_out_R", 255, 0, 0);
        AddFloatUI(parent, "Zanzou_out_G", 255, 0, 255);
        AddFloatUI(parent, "Zanzou_out_B", 255, 0, 0);
        AddFloatUI(parent, "Zanzou_in_tm", 1, 0, 0.2f);
        AddFloatUI(parent, "Zanzou_ou_tm", 10, 0, 10);
        AddFloatUI(parent, "Zanzou_param", 1000, 0, 230);
        AddBooleanUI(parent, "Zanzou_Invert", false);
    }

    private void CreateUIsImageporcessingPolygon(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Polygon_con_R", 255, 0, 0);
        AddFloatUI(parent, "Polygon_con_G", 255, 0, 0);
        AddFloatUI(parent, "Polygon_con_B", 255, 0, 200);
        AddFloatUI(parent, "Polygon_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "Polygon_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "Polygon_bgd_B", 255, 0, 0);
        AddFloatUI(parent, "Polygon_Rate", 40, 1, 6);
        AddBooleanUI(parent, "Polygon_UseFade", true);

    }

    private void CreateUIsImageporcessingSpike(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Spike_inval", 100, 1, 10);
        AddFloatUI(parent, "Spike_lngth", 50, 1, 5);
        AddFloatUI(parent, "Spike_rdius", 5, 1, 1);
        AddFloatUI(parent, "Spike_con_R", 255, 0, 27);
        AddFloatUI(parent, "Spike_con_G", 255, 0, 206);
        AddFloatUI(parent, "Spike_con_B", 255, 0, 135);
        AddFloatUI(parent, "Spike_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "Spike_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "Spike_bgd_B", 255, 0, 0);
    }

    private void CreateUIsImportKinectv2(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Kinect_x_min", 0, -5, -5);
        AddFloatUI(parent, "Kinect_x_max", 5, 0, 5);
        AddFloatUI(parent, "Kinect_y_min", 0, -2, -1);
        AddFloatUI(parent, "Kinect_y_max", 5, 0, 2);
        AddFloatUI(parent, "Kinect_z_min", 8, 0, 1);
        AddFloatUI(parent, "Kinect_z_max", 8, 0, 8);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Kinect_pos_x", 15, -15, 0);
        AddFloatUI(parent, "Kinect_pos_y", 5, 0, 1);
        AddFloatUI(parent, "Kinect_pos_z", 10, 0, 8);
        m_lastUpdatedHeight += 10;
        AddFloatUI(parent, "Kinect_Cut_y", 2, -2, 0);
        AddFloatUI(parent, "Kinect_Cut_diff", 0.1f, 0.005f, 0.02f);
        AddBooleanUI(parent, "Archive",false);

    }
    /// <summary>
    /// normal shadow
    /// </summary>
    /// <param name="parent"></param>
    private void CreateUIsImageporcessingNormal(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddBooleanUI(parent, "Normal_Invert", false);
    }
    private void CreateUIsImageporcessingTimeDelay(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "TimeDelay_DelayTime", 1000, 0, 0);
    }

    private void CreateUIsCallibrationImport(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Clb_I_TL_X", 2, -1, 0);
        AddFloatUI(parent, "Clb_I_TL_Y", 2, -1, 0);
        AddFloatUI(parent, "Clb_I_BL_X", 2, -1, 0);
        AddFloatUI(parent, "Clb_I_BL_Y", 2, -1, 1);
        AddFloatUI(parent, "Clb_I_BR_X", 2, -1, 1);
        AddFloatUI(parent, "Clb_I_BR_Y", 2, -1, 1);
        AddFloatUI(parent, "Clb_I_TR_X", 2, -1, 1);
        AddFloatUI(parent, "Clb_I_TR_Y", 2, -1, 0);
    }

    private void CreateUIsCallibrationExport(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Clb_E_TL_X", 2000, -1000, 0);
        AddFloatUI(parent, "Clb_E_TL_Y", 2000, -1000, Screen.height);
        AddFloatUI(parent, "Clb_E_BL_X", 2000, -1000, 0);
        AddFloatUI(parent, "Clb_E_BL_Y", 2000, -1000, 0);
        AddFloatUI(parent, "Clb_E_BR_X", 2000, -1000, Screen.width);
        AddFloatUI(parent, "Clb_E_BR_Y", 2000, -1000, 0);
        AddFloatUI(parent, "Clb_E_TR_X", 2000, -1000, Screen.width);
        AddFloatUI(parent, "Clb_E_TR_Y", 2000, -1000, Screen.height);

        AddBooleanUI(parent, "Clb_E_Vsbl", true);
    }

    #endregion

    #region parts
    private void AddFloatUI(GameObject parent, string ParameterName, float max, float min, float @default)
    {
        var slider = Instantiate<ParameterSlider>(m_slider);
        slider.Title = ParameterName;
        slider.Max = max;
        slider.Min = min;
        slider.DefaultValue = @default;
        slider.gameObject.transform.SetParent(parent.transform, false);
        var recttransform = slider.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        AddUI(ParameterName, slider);

        m_lastUpdatedHeight += slider.getSize().height;

    }
    private void AddBooleanUI(GameObject parent, string ParameterName, bool @default)
    {
        var checkBox = Instantiate<ParameterCheckbox>(m_checkbox);
        checkBox.Title = ParameterName;
        checkBox.DefaultValue = @default;
        checkBox.gameObject.transform.SetParent(parent.transform, false);
        var recttransform = checkBox.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        AddUI(ParameterName, checkBox);

        m_lastUpdatedHeight += checkBox.getSize().height;

    }
    private void AddButtonUI(GameObject parent, string ParameterName)
    {
        var button = Instantiate<ParameterButton>(m_button);
        button.Title = ParameterName;
        button.gameObject.transform.SetParent(parent.transform, false);
        var recttransform = button.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        AddUI(ParameterName, button);

        m_lastUpdatedHeight += button.getSize().height;

    }

    private void AddTextUI(GameObject parent, string ParameterName)
    {
        var Text = Instantiate<ParameterText>(m_text);
        Text.Title = ParameterName;
        Text.gameObject.transform.SetParent(parent.transform, false);
        var recttransform = Text.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        AddUI(ParameterName, Text);

        m_lastUpdatedHeight += Text.getSize().height;

    }

    #endregion

    #region Panel OnOff
    private GameObject m_currentImageProcesserSettingPanel;
    public void ImageProcesserSettingPanelSet(bool value)
    {
        m_currentImageProcesserSettingPanel.SetActive(value);

    }
    private GameObject m_currentImportSettingPanel;
    public void ImportSettingPanelSet(bool value)
    {
        m_currentImportSettingPanel.SetActive(value);
    }
    private GameObject m_currentCallibrationSettingPanel;
    public void CallibrationSettingPanelSet(bool value)
    {
        m_currentCallibrationSettingPanel.SetActive(value);
    }
    private GameObject m_currentAfterEffectSettingPanel;
    public void AfetrEffectSettingPanelSet(bool value)
    {
        m_currentAfterEffectSettingPanel.SetActive(value);
    }
    private GameObject m_currentArchiveSettingPanel;
    public void ArchiveSettingPanelSet(bool value)
    {
        m_currentArchiveSettingPanel.SetActive(value);
    }


    #endregion
}
