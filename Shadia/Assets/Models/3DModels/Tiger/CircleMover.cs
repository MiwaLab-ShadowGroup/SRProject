using System.Collections;
using System.Collections.Generic;
using Miwalab.ShadowGroup.Background;
using UnityEngine;
using System;
using Miwalab.ShadowGroup.Network;

public class CircleMover : MonoBehaviour
{
    //public GameObject inTargetObj;

    public float radius = 2;
    public float theta;
    public float thetaZero;

    private Vector3 posVec;

    private float hInput;
    private float vInput;

    public float speedRate = 1.0f;
    public float objScale = 1.0f;

    public bool operation = false;
    public bool moveReverse = false;


    // Use this for initialization
    void Start()
    {
        this.theta = 0;
        this.posVec.y = this.transform.position.y;

        (ShadowMediaUIHost.GetUI("Tiger_Size") as ParameterSlider).ValueChanged += Tiger_Size_ValueChanged;
        (ShadowMediaUIHost.GetUI("Tiger_theta0") as ParameterSlider).ValueChanged += Tiger_theta0_ValueChanged;
        (ShadowMediaUIHost.GetUI("Tiger_radius") as ParameterSlider).ValueChanged += Tiger_radius_ValueChanged;
        (ShadowMediaUIHost.GetUI("Tiger_spdRate") as ParameterSlider).ValueChanged += Tiger_spdRate_ValueChanged;
        (ShadowMediaUIHost.GetUI("Tiger_Operate") as ParameterCheckbox).ValueChanged += Tiger_Operate_ValueChanged;
        (ShadowMediaUIHost.GetUI("Tiger_moveReverse") as ParameterCheckbox).ValueChanged += Tiger_moveReverse_ValueChanged;
    }

    private void Tiger_Size_ValueChanged(object sender, EventArgs e)
    {
        this.objScale = (e as ParameterSlider.ChangedValue).Value;
    }

    private void Tiger_theta0_ValueChanged(object sender, EventArgs e)
    {
        this.thetaZero = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Tiger_radius_ValueChanged(object sender, EventArgs e)
    {
        this.radius = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Tiger_spdRate_ValueChanged(object sender, EventArgs e)
    {
        this.speedRate = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Tiger_Operate_ValueChanged(object sender, EventArgs e)
    {
        this.operation = (bool)(e as ParameterCheckbox.ChangedValue).Value;
    }

    private void Tiger_moveReverse_ValueChanged(object sender, EventArgs e)
    {
        this.moveReverse = (bool)(e as ParameterCheckbox.ChangedValue).Value;
    }

    // Update is called once per frame
    void Update()
    {
        //大きさの変更
        this.transform.localScale = new Vector3(this.objScale, this.objScale, this.objScale);

        //操作するとき
        if (this.operation)
        {
           

            this.hInput = Input.GetAxis("Horizontal");
            //this.hInput = Mathf.Sign(this.hInput);

            this.vInput = Input.GetAxis("Vertical");
            //this.vInput = Mathf.Sign(this.vInput);

            this.theta += this.hInput / 40 * this.speedRate;
        }
        //操作しないとき　ぐるぐる回る
        else
        {
           
            if (!this.moveReverse)
            {
                this.theta += this.speedRate / 40;
                this.hInput = 1;
            }
            else
            {
                this.theta -= this.speedRate / 40;
                this.hInput = -1;
            }
        }

        //移動
        this.posVec.x = this.radius * Mathf.Cos(this.theta + this.thetaZero);
        this.posVec.z = this.radius * Mathf.Sin(this.theta + this.thetaZero);
        this.transform.position = this.posVec;


        //向きの補正　中心に置いたターゲットオブジェクトによって進行方向を見る
        if (this.hInput > 0)
        {
            //this.transform.LookAt(this.inTargetObj.transform);
            this.transform.LookAt(new Vector3(0, this.transform.position.y, 0));
        }
        else if (this.hInput < 0)
        {
            //this.transform.LookAt(this.inTargetObj.transform);
            this.transform.LookAt(new Vector3(0, this.transform.position.y, 0));
            this.transform.Rotate(new Vector3(0, 180, 0));
        }
    }
}
