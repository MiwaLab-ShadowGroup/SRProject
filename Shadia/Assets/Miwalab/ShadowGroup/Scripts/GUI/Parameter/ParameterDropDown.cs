using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class ParameterDropdown : AParameterUI 
{

    public Text m_titleText;
    public Dropdown m_Dropdown;

    public string Title;

    private int _length;

    public class ChangedValue : EventArgs
    {
        public ChangedValue(int value)
        {
            this.Value = value;
        }
        public int Value { set; get; }
    }


    public event EventHandler ValueChanged;

    public void OnValueChanged(int value)
    {
        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, new ChangedValue(value));
        }

        this.m_Dropdown.value = value;
    }

    public void ValueUpdate()
    {
        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, new ChangedValue(m_Dropdown.value));
        }
    }

    public void initialize<T>(T _default)
    {
        _length = Enum.GetNames(typeof(T)).Length;
        List<string> optionList = new List<string>();
        int index=0;
        for (int i = 0; i < _length; ++i)
        {
            optionList.Add(((T)(object)i).ToString());
            if(((T)(object)i).ToString() ==  _default.ToString())
            {
               index = i;
            }
        }
        m_Dropdown.AddOptions(optionList);
        m_Dropdown.value = index;
        m_titleText.text = this.Title;
    }

    // Use this for initialization
    void Start()
    {
        

    }

    public override Rect getSize()
    {
        return new Rect(0, 0, 400, 30);
    }

    public override ParameterType GetParameterType()
    {
        return ParameterType.Enum;
    }
}
