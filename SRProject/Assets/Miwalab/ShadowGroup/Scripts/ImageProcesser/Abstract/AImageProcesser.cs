using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

public abstract class AImageProcesser : MonoBehaviour
{
    public abstract void Processing(Mat srcMat, ref Mat dstMat);
    public abstract string Name();
 
}

