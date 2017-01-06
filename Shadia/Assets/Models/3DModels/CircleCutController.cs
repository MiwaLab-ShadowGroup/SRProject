using System.Collections;
using System.Collections.Generic;
using Miwalab.ShadowGroup.Background;
using UnityEngine;
using System;

public class CircleCutController : MonoBehaviour
{
    public GameObject CircleBlind;
    public float radius = 1f;
    private bool useCut;

    // Use this for initialization
    void Start()
    {
        (ShadowMediaUIHost.GetUI("CircleCut_radius") as ParameterSlider).ValueChanged += CircleCut_radius_ValueChanged;

        (ShadowMediaUIHost.GetUI("CircleCut_active") as ParameterCheckbox).ValueChanged += CircleCut_active_ValueChanged;
    }


    private void CircleCut_radius_ValueChanged(object sender, EventArgs e)
    {
        this.radius = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void CircleCut_active_ValueChanged(object sender, EventArgs e)
    {
        this.useCut = (bool)(e as ParameterCheckbox.ChangedValue).Value;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = new Vector3(this.radius, this.transform.localScale.y, this.radius);

        if (this.useCut)
        {
            this.CircleBlind.SetActive(true);
        }
        else
        {
            this.CircleBlind.SetActive(false);
        }
    }
}


