using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class ParameterDropDown<T> : AParameterUI where T : struct
{

    public Text m_titleText;
    public Dropdown m_valueText;

    public string Title;

    private int _length;

    public class ChangedValue : EventArgs
    {
        public ChangedValue(T value)
        {
            this.Value = value;
        }
        public T Value { set; get; }
    }


    public event EventHandler ValueChanged;

    public void OnValueChanged(int value)
    {
        if (this.ValueChanged != null)
        {
            this.ValueChanged(this, new ChangedValue((T)(object)value));
        }

        this.m_valueText.value = value;
    }
    

    // Use this for initialization
    void Start()
    {
        _length = Enum.GetNames(typeof(T)).Length;
        List<string> optionList = new List<string>();
        for(int i =0; i < _length; ++i)
        {
            optionList.Add(((T)(object)i).ToString());
        }
        m_valueText.AddOptions(optionList);
        
        m_titleText.text = this.Title;

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
