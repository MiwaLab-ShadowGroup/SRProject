using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class ParameterButton : AParameterUI
{

    public Button m_button;
    public Text m_titleText;
    public Text m_valueText;

    public string Title;
    public int m_ClickCount;

    public event EventHandler Clicked;

    public void OnClicked()
    {
        if (this.Clicked != null)
        {
            this.Clicked(this, new EventArgs());
        }
        m_ClickCount++;
        this.m_valueText.text = m_ClickCount.ToString();
    }

    // Use this for initialization
    void Start()
    {
        m_titleText.text = this.Title;
        this.m_valueText.text = m_ClickCount.ToString();

    }

    public override Rect getSize()
    {
        return new Rect(0, 0, 400, 30);
    }

    public override ParameterType GetParameterType()
    {
        return ParameterType.Other;
    }
}
