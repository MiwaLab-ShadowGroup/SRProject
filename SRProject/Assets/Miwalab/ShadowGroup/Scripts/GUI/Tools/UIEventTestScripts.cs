using UnityEngine;
using System.Collections;
using System;

public class UIEventTestScripts : MonoBehaviour {
    ParameterSlider m_Slider;
	// Use this for initialization
	void Start () {
        m_Slider = UIHost.GetUI("MySlider_ValueChanged") as ParameterSlider;
        m_Slider.ValueChanged += UIEventTestScripts_myevent;
    }

    private void UIEventTestScripts_myevent(object sender, EventArgs e)
    {
        Debug.Log((e as ParameterSlider.ChangedValue).Value.ToString() );
    }

    // Update is called once per frame
    void Update () {

    }
}
