using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkController : MonoBehaviour {

    private Animator _animator;
    public float hInput;
    private bool hit;
    private bool sound;

    // Use this for initialization
    void Start () {
        this._animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        //animator
        this.hInput = Mathf.Abs( Input.GetAxis("Horizontal"));

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
}
