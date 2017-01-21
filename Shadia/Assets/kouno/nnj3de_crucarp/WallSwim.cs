using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallSwim : MonoBehaviour
{

    public float objScale = 0.5f;
    public float speed = 0.8f;
    public float cornerSharp = 10f;

    public float ScreenRadius = 2.5f;

    public float degMoveRange = 60;
    public float targetRadian;

    public float thetaZero = 0;

    public bool moveRandomOnCircle = true;
    public bool moveReverse = false;

    private float myRadian = 0;
    private float randomRate = 1.0f;


    private void Start()
    {
        (ShadowMediaUIHost.GetUI("Fish_Size") as ParameterSlider).ValueChanged += Fish_Size_ValueChanged;
        (ShadowMediaUIHost.GetUI("Fish_theta0") as ParameterSlider).ValueChanged += Fish_theta0_ValueChanged;
        (ShadowMediaUIHost.GetUI("Fish_ScrRadius") as ParameterSlider).ValueChanged += Fish_ScrRadius_ValueChanged;
        (ShadowMediaUIHost.GetUI("Fish_speed") as ParameterSlider).ValueChanged += Fish_spdRate_ValueChanged;
        (ShadowMediaUIHost.GetUI("Fish_corner") as ParameterSlider).ValueChanged += Fish_corner_ValueChanged;
        (ShadowMediaUIHost.GetUI("Fish_degMoveRange") as ParameterSlider).ValueChanged += Fish_degMoveRange_ValueChanged;
        (ShadowMediaUIHost.GetUI("Fish_moveRandomOnCircle") as ParameterCheckbox).ValueChanged += Fish_moveRandomOnCircle_ValueChanged;
        (ShadowMediaUIHost.GetUI("Fish_moveReverse") as ParameterCheckbox).ValueChanged += Fish_moveReverse_ValueChanged;

        this.randomRate = UnityEngine.Random.Range(0.9f, 1.1f);

        this.targetRadian = GetRandomRadianOnCylinder();
        this.myRadian = 0;
    }

    private void Fish_Size_ValueChanged(object sender, EventArgs e)
    {
        this.objScale = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Fish_theta0_ValueChanged(object sender, EventArgs e)
    {
        this.thetaZero = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Fish_ScrRadius_ValueChanged(object sender, EventArgs e)
    {
        this.ScreenRadius = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Fish_spdRate_ValueChanged(object sender, EventArgs e)
    {
        this.speed = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Fish_corner_ValueChanged(object sender, EventArgs e)
    {
        this.cornerSharp = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Fish_degMoveRange_ValueChanged(object sender, EventArgs e)
    {
        this.degMoveRange = (float)(e as ParameterSlider.ChangedValue).Value;
    }

    private void Fish_moveRandomOnCircle_ValueChanged(object sender, EventArgs e)
    {
        this.moveRandomOnCircle = (bool)(e as ParameterCheckbox.ChangedValue).Value;
    }

    private void Fish_moveReverse_ValueChanged(object sender, EventArgs e)
    {
        this.moveReverse = (bool)(e as ParameterCheckbox.ChangedValue).Value;
    }


    private void Update()
    {
        //サイズ変更
        this.transform.localScale = new Vector3(this.objScale, this.objScale, this.objScale);

        if (this.moveRandomOnCircle)
        {
            //目標店との距離をチェック
            if (Mathf.Abs(this.myRadian - this.targetRadian) < 0.1f)
            {
                this.targetRadian = GetRandomRadianOnCylinder();
            }

            //自分の角度を変化 半径で割ることで速度の差をなくす
            if (this.myRadian < this.targetRadian)
            {
                this.myRadian += Time.deltaTime * this.speed / this.ScreenRadius * this.randomRate;
            }
            else
            {
                this.myRadian -= Time.deltaTime * this.speed / this.ScreenRadius * this.randomRate;
            }
        }
        else
        {
            if (!moveReverse)
            {
                this.targetRadian = this.degMoveRange * Mathf.PI / 180;
                this.myRadian += Time.deltaTime * this.speed / this.ScreenRadius * this.randomRate;
            }
            else
            {
                this.targetRadian = -this.degMoveRange * Mathf.PI / 180;
                this.myRadian -= Time.deltaTime * this.speed / this.ScreenRadius * this.randomRate;
            }

            if (this.myRadian > this.degMoveRange * Mathf.PI / 180)
            {
                this.myRadian = -this.degMoveRange * Mathf.PI / 180;
                this.transform.Rotate(0,- this.degMoveRange *2,0);
            }
            else if (this.myRadian < -this.degMoveRange * Mathf.PI / 180)
            {
                this.myRadian = this.degMoveRange * Mathf.PI / 180;
                this.transform.Rotate(0, this.degMoveRange * 2, 0);
            }

        }

        //positionを動かす
        this.transform.position = new Vector3(this.ScreenRadius * Mathf.Sin(this.myRadian + this.thetaZero), this.transform.position.y, this.ScreenRadius * Mathf.Cos(this.myRadian + this.thetaZero));


        //向きを変える
        //Quaternion
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(0, this.transform.position.y, 0) - transform.position);

        //LookAt
        //transform.LookAt(new Vector3(0, this.transform.position.y, 0));


        if (this.myRadian < this.targetRadian)
        {
            //this.transform.Rotate(0, rotaterad, 0);
            targetRotation *= Quaternion.AngleAxis(-90f, new Vector3(0, 1f, 0));

        }
        else
        {
            //this.transform.Rotate(0, -90, 0);
            targetRotation *= Quaternion.AngleAxis(90f, new Vector3(0, 1f, 0));
        }

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * cornerSharp);
    }




    public float GetRandomRadianOnCylinder()
    {
        return UnityEngine.Random.Range(-this.degMoveRange * Mathf.PI / 180, this.degMoveRange * Mathf.PI / 180);

    }



}


