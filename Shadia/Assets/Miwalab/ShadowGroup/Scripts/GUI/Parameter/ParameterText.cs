using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ParameterText : AParameterUI
{
    
    public Text m_titleText;
    public Text m_valueText;

    public string Title;
    public string DefaultValue;

    // Use this for initialization
    void Start()
    {
        m_titleText.text = this.Title;
        this.m_valueText.text = DefaultValue;

    }

    public override Rect getSize()
    {
        return new Rect(0, 0, 400, 30);
    }
}
