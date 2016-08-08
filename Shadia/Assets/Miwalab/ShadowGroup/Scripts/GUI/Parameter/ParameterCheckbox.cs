using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ParameterCheckbox : AParameterUI {

    public Toggle m_toggle;
    public Text m_titleText;
    public Text m_valueText;

    public string Title;
    public bool DefaultValue = false;


    public class ChangedValue : EventArgs
    {
        public ChangedValue(bool value)
        {
            this.Value = value;
        }
        public bool Value { set; get; }
    }


    public event EventHandler ValueChanged;

    public void OnValueChanged(bool value)
    {
        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, new ChangedValue(value));
        }
        this.m_valueText.text = value.ToString();
    }

    public void ValueUpdate()
    {
        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, new ChangedValue(m_toggle.isOn));
        }
    }

    // Use this for initialization
    void Start()
    {
        m_titleText.text = this.Title;
        m_toggle.isOn = this.DefaultValue;
        this.m_valueText.text = m_toggle.isOn.ToString();

    }

    public override Rect getSize()
    {
        return new Rect(0, 0, 400, 30);
    }
}
