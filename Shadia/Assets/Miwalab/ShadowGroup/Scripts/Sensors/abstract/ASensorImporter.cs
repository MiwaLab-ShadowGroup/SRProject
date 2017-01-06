using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using Miwalab.ShadowGroup.ImageProcesser;
using System.Collections.Generic;
using Miwalab.ShadowGroup.AfterEffect;

public abstract class ASensorImporter : MonoBehaviour
{
    public abstract Mat getCvMat();
    public abstract MatType getMatType();
    public abstract void setUpUI();
    public void AddImageProcesser(AShadowImageProcesser ImageProcesser)
    {
        this.m_ImagerProcesserList.Add(ImageProcesser);
    }
    public void AddImageProcessers(List<AShadowImageProcesser> ImageProcesser)
    {
        this.m_ImagerProcesserList.AddRange(ImageProcesser);
    }
    public void RemoveImageProcesser(int num)
    {
        this.m_ImagerProcesserList.RemoveAt(num);
    }
    public void RemoveAllImageProcesser()
    {
        this.m_ImagerProcesserList.Clear();
    }

    public void AddAfterEffect(AAfterEffect AfterEffect)
    {
        this.m_AfterEffectList.Add(AfterEffect);
    }

    public List<AAfterEffect> GetAffterEffectList()
    {
        return this.m_AfterEffectList;
    }
    public void RemoveAfterEffect(int num)
    {
        this.m_AfterEffectList.RemoveAt(num);
    }
    public void RemoveAllAfterEffect()
    {
        this.m_AfterEffectList.Clear();
    }

    protected List<AShadowImageProcesser> m_ImagerProcesserList { set; get; }
    protected List< AAfterEffect> m_AfterEffectList { set; get; }
}

