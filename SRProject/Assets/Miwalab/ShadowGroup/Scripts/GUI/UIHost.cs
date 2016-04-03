using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Miwalab.ShadowGroup.ImageProcesser;
using UnityEngine.UI;
using Miwalab.ShadowGroup.Scripts.Sensors;

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
        m_MenuList.Add(this.ImageProcessingMenu);
        m_MenuList.Add(this.ImportSettingMenu);

        foreach(var menu in this.m_MenuList)
        {
            foreach(var option in menu.options)
            {
                GameObject item = Instantiate(SettingPanel);
                item.transform.SetParent(MainCanvas.gameObject.transform, false);
                m_PanelDictionary.Add(option.text, item);
                item.SetActive(false);
            }
        }
        this.m_currentImageProcesserSettingPanel = m_PanelDictionary[ImageProcesserType.Normal.ToString()];
        this.m_currentImportSettingPanel = m_PanelDictionary[ImportSettingType.Kinect.ToString()];

        //UI初期化
        this.CreateUIsImageporcessingNormal(m_PanelDictionary[ImageProcesserType.Normal.ToString()]);
        this.CreateUIsImportKinectv2(m_PanelDictionary[ImportSettingType.Kinect.ToString()]);

        this.m_Sensor.setUpUI();

        this.ImageProcesserSettingPanelSet(false);
        this.ImportSettingPanelSet(false);
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
                break;
            case ImageProcesserType.CellAutomaton:
                break;
            case ImageProcesserType.Polygon:
                this.m_Sensor.AddImageProcesser(new polygon());
                break;
            case ImageProcesserType.DoubleAfterImage:
                this.m_Sensor.AddImageProcesser(new Zanzou());
                break;
        }
        
    }

    #region createUIMethods
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
        //AddFloatUI(parent, "Color_R", 255, 0, 0);
        //AddFloatUI(parent, "Color_G", 255, 0, 0);
        //AddFloatUI(parent, "Color_B", 255, 0, 0);
        AddBooleanUI(parent, "Normal_Invert", false);
    }
    #endregion

    #region parts
    private void AddFloatUI(GameObject parent,string ParameterName,float max,float min, float @default)
    {
        var slider = Instantiate<ParameterSlider>(m_slider);
        slider.Title =ParameterName;
        slider.Max = max;
        slider.Min = min;
        slider.DefaultValue = @default;
        slider.gameObject.transform.SetParent(parent.transform, false);
        var recttransform = slider.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0,-m_lastUpdatedHeight);

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
    #endregion
}
