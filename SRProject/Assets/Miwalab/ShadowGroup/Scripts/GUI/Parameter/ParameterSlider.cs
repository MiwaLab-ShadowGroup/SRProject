using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public class ParameterSlider : AParameterUI{


    public Slider m_slider;
    public Text m_titleText;
    public Text m_valueText;

    public string Title;
    public float Max = 1;
    public float Min = 0;
    public float DefaultValue = 0;


    public class ChangedValue : EventArgs
    {
        public ChangedValue(float value)
        {
            this.Value = value;
        }
        public float Value { set; get; }
    }


    public event EventHandler ValueChanged;

    public ParameterSlider()
        :base()
    {
    }

    public void OnValueChanged(float value)
    {
        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, new ChangedValue(value));
        }
        this.m_valueText.text = value.ToString("0.00");
    }

    public void ValueUpdate()
    {
        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, new ChangedValue(m_slider.value));
        }
    }

    // Use this for initialization
    void Start()
    {
        m_titleText.text = this.Title;
        m_slider.maxValue = this.Max;
        m_slider.minValue = this.Min;
        m_slider.value = this.DefaultValue;
        this.m_valueText.text = m_slider.value.ToString("0.00");
    }
    public override Rect getSize()
    {
        return new Rect(0, 0, 400, 30);
    }

}
