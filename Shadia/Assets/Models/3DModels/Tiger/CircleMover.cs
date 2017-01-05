using System.Collections;
using System.Collections.Generic;
using Miwalab.ShadowGroup.Background;
using UnityEngine;
using System;

public class CircleMover : MonoBehaviour
{
    public GameObject inTargetObj;

    public float radius = 2;
    public float theta;
    public float thetaZero;

    private Vector3 posVec;

    private bool flag = false;
    private int count = 0;
    private float hInput;
    private float vInput;

    public float speedRate = 1.0f;
    public float objScale = 1.0f;

    // Use this for initialization
    void Start()
    {
        this.theta = 0;
        this.posVec.y = this.transform.position.y;

        (ShadowMediaUIHost.GetUI("Tiger_Size") as ParameterSlider).ValueChanged += Particle_Size_ValueChanged;
        (ShadowMediaUIHost.GetUI("Tiger_theta0") as ParameterSlider).ValueChanged += Particle_theta0_ValueChanged;
        (ShadowMediaUIHost.GetUI("Tiger_radius") as ParameterSlider).ValueChanged += Particle_radius_ValueChanged;
        (ShadowMediaUIHost.GetUI("Tiger_spdRate") as ParameterSlider).ValueChanged += Particle_spdRate_ValueChanged;
    }

    private void Particle_Size_ValueChanged(object sender, EventArgs e)
    {
        this.objScale = (e as ParameterSlider.ChangedValue).Value;
    }

    private void Particle_theta0_ValueChanged(object sender, EventArgs e)
    {
        this.thetaZero = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Particle_radius_ValueChanged(object sender, EventArgs e)
    {
        this.radius = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Particle_spdRate_ValueChanged(object sender, EventArgs e)
    {
        this.speedRate = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localScale = new Vector3(this.objScale, this.objScale, this.objScale);

        this.hInput = Input.GetAxis("Horizontal");
        //this.hInput = Mathf.Sign(this.hInput);

        this.vInput = Input.GetAxis("Vertical");
        //this.vInput = Mathf.Sign(this.vInput);

        this.theta += this.hInput / 40 * this.speedRate;


        //移動
        this.posVec.x = this.radius * Mathf.Cos(this.theta + this.thetaZero);
        this.posVec.z = this.radius * Mathf.Sin(this.theta + this.thetaZero);
        this.transform.position = this.posVec;


        //向きの補正　中心に置いたターゲットオブジェクトによって進行方向を見る
        if (this.hInput > 0)
        {
            this.transform.LookAt(this.inTargetObj.transform);
        }
        else if (this.hInput < 0)
        {
            this.transform.LookAt(this.inTargetObj.transform);
            this.transform.Rotate(new Vector3(0, 180, 0));
        }

    }
}
