using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Miwalab.ShadowGroup.ImageProcesser;
using UnityEngine.UI;
using Miwalab.ShadowGroup.Scripts.Sensors;
using Miwalab.ShadowGroup.Scripts.Callibration;

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

    public ASensorImporter m_Sensor;

    public Dropdown ImageProcessingMenu;
    public Dropdown ImportSettingMenu;
    public Dropdown CallibrationSettingMenu;
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
        m_MenuList.Add(this.ImageProcessingMenu);
        m_MenuList.Add(this.ImportSettingMenu);
        m_MenuList.Add(this.CallibrationSettingMenu);

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


        //UI初期化
        this.CreateUIsImageporcessingNormal(m_PanelDictionary[ImageProcesserType.Normal.ToString()]);
        this.CreateUIsImageporcessingTimeDelay(m_PanelDictionary[ImageProcesserType.TimeDelay.ToString()]);

        this.CreateUIsImageporcessingSpike(m_PanelDictionary[ImageProcesserType.Spike.ToString()]);
        this.CreateUIsImageporcessingPolygon(m_PanelDictionary[ImageProcesserType.Polygon.ToString()]);
        this.CreateUIsImageporcessingZanzou(m_PanelDictionary[ImageProcesserType.DoubleAfterImage.ToString()]);


        this.CreateUIsImportKinectv2(m_PanelDictionary[ImportSettingType.Kinect.ToString()]);

        this.CreateUIsCallibrationImport(m_PanelDictionary[CallibrationSettingType.CallibrationImport.ToString()]);
        this.CreateUIsCallibrationExport(m_PanelDictionary[CallibrationSettingType.CallibrationExport.ToString()]);


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
        this.m_Sensor.RemoveAllImageProcesser();

        switch (type)
        {
            case ImageProcesserType.Normal:
                //一回作って使いまわす
                this.m_Sensor.AddImageProcesser(new Normal());
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Normal.ToString()];
                break;
            case ImageProcesserType.CellAutomaton:
                break;
            case ImageProcesserType.Polygon:
                this.m_Sensor.AddImageProcesser(new polygon());
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Polygon.ToString()];
                break;
            case ImageProcesserType.DoubleAfterImage:
                this.m_Sensor.AddImageProcesser(new Zanzou());
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.DoubleAfterImage.ToString()];
                break;
            case ImageProcesserType.TimeDelay:
                this.m_Sensor.AddImageProcesser(new Timedelay());
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.TimeDelay.ToString()];
                break;
            case ImageProcesserType.TamuraSkeleton:
                this.m_Sensor.AddImageProcesser(new TamuraSkelton());
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.TamuraSkeleton.ToString()];
                break;
            case ImageProcesserType.Spike:
                this.m_Sensor.AddImageProcesser(new Spike());
                this.m_currentImageProcesserSettingPanel = this.m_PanelDictionary[ImageProcesserType.Spike.ToString()];
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

    }

    private void CreateUIsImageporcessingPolygon(GameObject parent)
    {
        m_lastUpdatedHeight = 0;
        AddFloatUI(parent, "Polygon_con_R", 255, 0, 100);
        AddFloatUI(parent, "Polygon_con_G", 255, 0, 0);
        AddFloatUI(parent, "Polygon_con_B", 255, 0, 0);
        AddFloatUI(parent, "Polygon_bgd_R", 255, 0, 0);
        AddFloatUI(parent, "Polygon_bgd_G", 255, 0, 0);
        AddFloatUI(parent, "Polygon_bgd_B", 255, 0, 0);

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
    #endregion
}
