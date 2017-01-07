using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageProcessingSelector : MonoBehaviour {

    public ShadowMediaUIHost _host;

    bool _usePtsImageProcesser;
    int _selectedIndex;


    // Use this for initialization
    void Start () {
        (ShadowMediaUIHost.GetUI("IPS_ImgChange") as ParameterDropdown).ValueChanged += IPS_ImgChange_ValueChanged;
        (ShadowMediaUIHost.GetUI("IPS_UsePtsImageProcesser") as ParameterCheckbox).ValueChanged += IPS_UsePtsImageProcesser_ValueChanged;
        _selectedIndex = 1;
        _usePtsImageProcesser = false;
    }


    private void IPS_UsePtsImageProcesser_ValueChanged(object sender, EventArgs e)
    {
        _usePtsImageProcesser = (e as ParameterCheckbox.ChangedValue).Value;
        _host.ChangeImageProcessingTo(_selectedIndex, _usePtsImageProcesser);

    }

    private void IPS_ImgChange_ValueChanged(object sender, EventArgs e)
    {
        _selectedIndex = (e as ParameterDropdown.ChangedValue).Value;
        _host.ChangeImageProcessingTo(_selectedIndex,_usePtsImageProcesser);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
