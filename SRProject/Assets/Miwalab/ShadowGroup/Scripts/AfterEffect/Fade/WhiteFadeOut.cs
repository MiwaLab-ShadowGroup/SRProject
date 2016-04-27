﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.AfterEffect.Fade
{
    public class WhiteFadeOut : AFadeOut
    {
        public WhiteFadeOut(int FinishCount) : base(FinishCount)
        {
            if (FinishCount == 0)
            {
                FinishCount = 300;
            }
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            //Cv2.CvtColor(src, dst, OpenCvSharp.ColorConversion.BgrToGray);

            //dst = dst + ((double)(this.m_CurrentFrame) * 255 / this.m_FinishFrame);

            //Cv2.CvtColor(dst, dst, OpenCvSharp.ColorConversion.GrayToBgr);

            //dst = (src + 1) * ((double)(this.m_CurrentFrame) / 20);


            int channel = src.Channels();

            unsafe
            {
                byte* srcPtr = src.DataPointer;
                byte* dstPtr = dst.DataPointer;

                for (int i = 0; i < src.Height * src.Width * channel; i += 3)
                {
                    if (srcPtr[i] == 255 && srcPtr[i + 1] == 255 && srcPtr[i + 2] == 255)
                    {
                        dstPtr[i] = 255;
                        dstPtr[i + 1] = 255;
                        dstPtr[i + 2] = 255;

                    }

                    else
                    {
                        if (srcPtr[i] + ((double)(this.m_CurrentFrame) * 255 / this.m_FinishFrame) > 255)
                        {

                            dstPtr[i] = 255;

                        }
                        else
                        {
                            dstPtr[i] = (byte)(srcPtr[i] + ((double)(this.m_CurrentFrame) * 255 / this.m_FinishFrame));

                        }

                        if ((srcPtr[i + 1] + ((double)(this.m_CurrentFrame) * 255 / this.m_FinishFrame)) > 255)
                        {

                            dstPtr[i + 1] = 255;

                        }
                        else
                        {
                            dstPtr[i + 1] = (byte)(srcPtr[i + 1] + ((double)(this.m_CurrentFrame) * 255 / this.m_FinishFrame));

                        }

                        if ((srcPtr[i + 2] + ((double)(this.m_CurrentFrame) * 255 / this.m_FinishFrame)) > 255)
                        {

                            dstPtr[i + 2] = 255;

                        }
                        else
                        {
                            dstPtr[i + 2] = (byte)(srcPtr[i + 2] + ((double)(this.m_CurrentFrame) * 255 / this.m_FinishFrame));

                        }

                    }

                }
            }
            this.UpdateCurrentFrame();
        }
    }
}

