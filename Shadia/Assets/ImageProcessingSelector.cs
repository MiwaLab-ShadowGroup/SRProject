using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProcessingSelector : MonoBehaviour {

    public ShadowMediaUIHost _host;

    bool _usePtsImageProcesser;
    bool _useDoubleAfter;

    int _selectedIndex;


    // Use this for initialization
    void Start () {
        (ShadowMediaUIHost.GetUI("IPS_ImgChange") as ParameterDropdown).ValueChanged += IPS_ImgChange_ValueChanged;
        (ShadowMediaUIHost.GetUI("IPS_UsePtsImageProcesser") as ParameterCheckbox).ValueChanged += IPS_UsePtsImageProcesser_ValueChanged;
        (ShadowMediaUIHost.GetUI("IPS_double_after") as ParameterCheckbox).ValueChanged += IPS_doubleAfter_ValueChanged;

        _selectedIndex = 1;
        _usePtsImageProcesser = false;
    }

    private void IPS_doubleAfter_ValueChanged(object sender, EventArgs e)
    {
        _useDoubleAfter = (e as ParameterCheckbox.ChangedValue).Value;
        _host.ChangeImageProcessingTo(_selectedIndex, _usePtsImageProcesser, _useDoubleAfter);
    }

    private void IPS_UsePtsImageProcesser_ValueChanged(object sender, EventArgs e)
    {
        _usePtsImageProcesser = (e as ParameterCheckbox.ChangedValue).Value;
        _host.ChangeImageProcessingTo(_selectedIndex, _usePtsImageProcesser, _useDoubleAfter);

    }

    private void IPS_ImgChange_ValueChanged(object sender, EventArgs e)
    {
        _selectedIndex = (e as ParameterDropdown.ChangedValue).Value;
        _host.ChangeImageProcessingTo(_selectedIndex,_usePtsImageProcesser, _useDoubleAfter);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
