using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Miwalab.ShadowGroup.ImageProcesser;
using UnityEngine.UI;

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
        m_lastUpdatedHeight = 0;
        foreach (var p in m_ParameterUI)
        {
            GameObject.DestroyImmediate(p.Value.gameObject);
        }
        m_ParameterUI.Clear();

    }
    #endregion

    public RectTransform DynamicUIpanel;
    public ParameterSlider m_slider;
    public ParameterCheckbox m_checkbox;


    public Dropdown ImageProcessingMenu;

    public void Start()
    {
        for (int i = 0; i < (int)ImageProcesserType.Count; ++i)
        {
            this.ImageProcessingMenu.options.Add(new Dropdown.OptionData(((ImageProcesserType)i).ToString()));
        }

    }

    public void ChangeOption(int number)
    {
        ResetUI();
        ImageProcesserType type = (ImageProcesserType)number;
        switch (type)
        {
            case ImageProcesserType.Normal:
                this.CreateUIsOfNormal(type);
                break;
            case ImageProcesserType.CellAutomaton:
                break;
            case ImageProcesserType.Polygon:
                break;
            case ImageProcesserType.DoubleAfterImage:
                break;
        }
        
    }

    #region createUIMethods
    private void CreateUIsOfNormal(ImageProcesserType type)
    {
        
        AddFloatUI(type, "Color_R", 255, 0, 0);
        AddFloatUI(type, "Color_G", 255, 0, 0);
        AddFloatUI(type, "Color_B", 255, 0, 0);
        AddBooleanUI(type, "Invert", false);
    }
    #endregion

    #region parts
    private void AddFloatUI(ImageProcesserType type,string ParameterName,float max,float min, float @default)
    {
        var slider = Instantiate<ParameterSlider>(m_slider);
        slider.Title = type.ToString()+ "_"+ ParameterName;
        slider.Max = max;
        slider.Min = min;
        slider.DefaultValue = @default;
        slider.gameObject.transform.SetParent(DynamicUIpanel, false);
        var recttransform = slider.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0,-m_lastUpdatedHeight);
        
        m_lastUpdatedHeight += slider.getSize().height;

    }
    private void AddBooleanUI(ImageProcesserType type, string ParameterName, bool @default)
    {
        var checkBox = Instantiate<ParameterCheckbox>(m_checkbox);
        checkBox.Title = type.ToString() + "_" + ParameterName;
        checkBox.DefaultValue = @default;
        checkBox.gameObject.transform.SetParent(DynamicUIpanel, false);
        var recttransform = checkBox.gameObject.transform as RectTransform;
        recttransform.anchoredPosition = new Vector2(0, -m_lastUpdatedHeight);

        m_lastUpdatedHeight += checkBox.getSize().height;

    }
    #endregion

    #region Panel OnOff
    private GameObject ImageProcesserSettingPanel;
    public void ImageProcesserSettingPanelSet(bool value)
    {
        if (ImageProcesserSettingPanel == null)
        {
            ImageProcesserSettingPanel = GameObject.Find("ImageProcesserSettingPanel");
        }
        ImageProcesserSettingPanel.SetActive(value);

    }
    private GameObject ImportSettingPanel;
    public void ImportSettingPanelSet(bool value)
    {
        if (ImportSettingPanel == null)
        {
            ImportSettingPanel = GameObject.Find("ImportSettingPanel");
        }
        ImportSettingPanel.SetActive(value);

    }
    #endregion
}
