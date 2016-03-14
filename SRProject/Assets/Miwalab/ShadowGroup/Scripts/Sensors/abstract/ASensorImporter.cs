using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

public abstract class ASensorImporter : MonoBehaviour
{
    public abstract Mat getCvMat();
}

