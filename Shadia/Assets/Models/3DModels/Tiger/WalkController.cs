using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WalkController : MonoBehaviour {

    private Animator _animator;
    public float hInput;
    private bool hit;
    private bool sound;

    public bool operation = false;

    // Use this for initialization
    void Start () {
        this._animator = GetComponent<Animator>();

        (ShadowMediaUIHost.GetUI("Tiger_Operate") as ParameterCheckbox).ValueChanged += Tiger_Operate_ValueChanged;
    }

    private void Tiger_Operate_ValueChanged(object sender, EventArgs e)
    {
        this.operation = (bool)(e as ParameterCheckbox.ChangedValue).Value;
    }

  
    // Update is called once per frame
    void Update () {

        if (this.operation)
        {
            //animator
            this.hInput = Mathf.Abs(Input.GetAxis("Horizontal"));

            if (Input.GetAxis("Vertical") > 0)
            {
                this.hit = true;
            }
            if (Input.GetAxis("Vertical") < 0)
            {
                this.sound = true;
            }


            this._animator.SetFloat("Speed", this.hInput);
            this._animator.SetBool("Hit", this.hit);
            this._animator.SetBool("Sound", this.sound);

            this.hit = false;
            this.sound = false;
        }
        else
        {
            this._animator.SetFloat("Speed", 1.0f);
        }
     
    }
}
