
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class ChangeColor : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        private Mat grayimage = new Mat();
        Scalar color;
        Scalar colorBack = new Scalar(0, 0, 0);
        bool m_UseFade;

        public ChangeColor() : base()
        {
            (ShadowMediaUIHost.GetUI("ChangeColor_con_R") as ParameterSlider).ValueChanged += ChangeColor_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("ChangeColor_con_G") as ParameterSlider).ValueChanged += ChangeColor_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("ChangeColor_con_B") as ParameterSlider).ValueChanged += ChangeColor_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("ChangeColor_UseFade") as ParameterCheckbox).ValueChanged += ChangeColor_UseFade_ValueChanged;


            (ShadowMediaUIHost.GetUI("ChangeColor_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("ChangeColor_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("ChangeColor_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("ChangeColor_UseFade") as ParameterCheckbox).ValueUpdate();
        }

        private void ChangeColor_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void ChangeColor_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void ChangeColor_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void ChangeColor_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }



        Mat m_buffer;
        Mat m_colorBuffer;
        private void Update(ref Mat src, ref Mat dst)
        {
            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);

            }
            else
            {
                if (this.m_UseFade)
                {
                    m_buffer *= 0.9;
                }
                else
                {
                    m_buffer *= 0;
                }

            }
            m_colorBuffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, color);

            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
            
            m_buffer += src.Mul(this.m_colorBuffer);

            dst += m_buffer;

            //dst += m_buffer;


        }

        public override string ToString()
        {
            return "ChangeColor";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.ChangeColor;
        }

        public bool IsFirstFrame { get; private set; }
    }


}
