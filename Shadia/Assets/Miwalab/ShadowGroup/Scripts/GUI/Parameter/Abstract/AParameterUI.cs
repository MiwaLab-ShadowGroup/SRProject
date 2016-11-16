using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public abstract class AParameterUI : MonoBehaviour {
    
	public AParameterUI()
    {
    }

    public virtual ParameterType GetParameterType()
    {
        return ParameterType.Other;
    }
    public virtual object GetValue()
    {
        return new object();
    }

    public virtual void SetValue(object value)
    {

    }

    public abstract Rect getSize();

    public enum ParameterType
    {
        Single,
        Boolean,
        String,
        Enum,
        Other
    }
}
