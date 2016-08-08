﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.AfterEffect.Fade
{
    public class BlackFadeOut : AFadeOut
    {
        public BlackFadeOut(int FinishCount)
            :base(FinishCount)
        {
            if (FinishCount == 0)
            {
                FinishCount = 300;
            }
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            dst = src * ((double)(this.m_FinishFrame - this.m_CurrentFrame )/ this.m_FinishFrame);
            this.UpdateCurrentFrame();

        }
    }
}