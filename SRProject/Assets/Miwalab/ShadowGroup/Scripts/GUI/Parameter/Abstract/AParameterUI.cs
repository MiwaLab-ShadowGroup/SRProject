using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class AParameterUI : MonoBehaviour {
    
	public AParameterUI()
    {
    }

    public abstract Rect getSize();
}
