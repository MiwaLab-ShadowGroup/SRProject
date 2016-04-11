using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using Miwalab.ShadowGroup.ImageProcesser;
using System.Collections.Generic;

public abstract class ASensorImporter : MonoBehaviour
{
    public abstract Mat getCvMat();
    public abstract MatType getMatType();
    public abstract void setUpUI();
    public void AddImageProcesser(AImageProcesser ImageProcesser)
    {
        this.m_ImagerProcesserList.Add(ImageProcesser);
    }
    public void RemoveImageProcesser(int num)
    {
        this.m_ImagerProcesserList.RemoveAt(num);
    }
    public void RemoveAllImageProcesser()
    {
        this.m_ImagerProcesserList.Clear();
    }

    protected List<AImageProcesser> m_ImagerProcesserList { set; get; }
}

