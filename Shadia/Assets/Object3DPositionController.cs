using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Remote3DModelManager))]
public class Object3DPositionController : MonoBehaviour {

    Remote3DModelManager r3DMM;

    public UnityChanControlScriptWithRgidBody unitychan;
    

	// Use this for initialization
	void Start () {
        r3DMM = GetComponent<Remote3DModelManager>();

        (ShadowMediaUIHost.GetUI("3D_CLB_X") as ParameterSlider).ValueChanged += R3DMM_CLB_X;
        (ShadowMediaUIHost.GetUI("3D_CLB_Y") as ParameterSlider).ValueChanged += R3DMM_CLB_Y;
        (ShadowMediaUIHost.GetUI("3D_CLB_Z") as ParameterSlider).ValueChanged += R3DMM_CLB_Z;
        (ShadowMediaUIHost.GetUI("3D_CLB_R") as ParameterSlider).ValueChanged += R3DMM_CLB_R;
    }

    private void R3DMM_CLB_R(object sender, EventArgs e)
    {
        float value = (e as ParameterSlider.ChangedValue).Value;
        this.transform.rotation = Quaternion.Euler(0, value, 0);
    }

    private void R3DMM_CLB_Z(object sender, EventArgs e)
    {
        float value = (e as ParameterSlider.ChangedValue).Value;
        var pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, pos.y, value);

    }

    private void R3DMM_CLB_Y(object sender, EventArgs e)
    {
        float value = (e as ParameterSlider.ChangedValue).Value;
        var pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, value, pos.z);
    }

    private void R3DMM_CLB_X(object sender, EventArgs e)
    {
        float value = (e as ParameterSlider.ChangedValue).Value;
        var pos = this.transform.position;
        this.transform.position = new Vector3(value, pos.y, pos.z);
    }

    // Update is called once per frame
    void Update () {
        var ope = r3DMM.GetReceiveed3DOpe();
        if (ope == null)
        {
            return;
        }
        this.setupParameters(ope);
	}

    private void setupParameters(Object3DOperation ope)
    {
        unitychan._H = ope._H;
        unitychan._V = ope._V;
    }
}
