using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

public class ImageProcesser : MonoBehaviour {

    public GameObject ImageSouce;
    public int Index;
    Mat srcImage;
    Mat dstImage;
    AImageProcesser[] List_ImageProcessors;

	// Use this for initialization
	void Start ()
    {
        this.srcImage = this.ImageSouce.GetComponent<ASensorImporter>().srcMat;

        this.List_ImageProcessors = this.GetComponents<AImageProcesser>();
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.List_ImageProcessors[this.Index].Processing(this.srcImage, ref this.dstImage);
	
	}
}
