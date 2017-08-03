using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Normal : AShadowImageProcesser
    {
        private bool invert;
        private bool NotUseColorChange;
        Scalar color;
        Scalar colorBack;
        int x, y, b_src, g_src, r_src, index_src, index_dst;
        public Normal()
            :base()
        {
            //UIからパラメータ変更通知の追加
            (ShadowMediaUIHost.GetUI("Normal_Invert") as ParameterCheckbox).ValueChanged += Normal_ValueChanged;
            (ShadowMediaUIHost.GetUI("Normal_Invert") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Normal_R") as ParameterSlider).ValueChanged += Normal_R;
            (ShadowMediaUIHost.GetUI("Normal_G") as ParameterSlider).ValueChanged += Normal_G;
            (ShadowMediaUIHost.GetUI("Normal_B") as ParameterSlider).ValueChanged += Normal_B;
            (ShadowMediaUIHost.GetUI("Normal_bgd_R") as ParameterSlider).ValueChanged += Normal_bgd_R;
            (ShadowMediaUIHost.GetUI("Normal_bgd_G") as ParameterSlider).ValueChanged += Normal_bgd_G;
            (ShadowMediaUIHost.GetUI("Normal_bgd_B") as ParameterSlider).ValueChanged += Normal_bgd_B;
            (ShadowMediaUIHost.GetUI("UseColorChange") as ParameterCheckbox).ValueChanged += UseColorChange;
        }

        private void UseColorChange(object sender, EventArgs e)
        {
            this.NotUseColorChange = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Normal_bgd_B(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (float)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Normal_bgd_G(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (float)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Normal_bgd_R(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (float)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Normal_B(object sender, EventArgs e)
        {
            this.color.Val0 = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Normal_G(object sender, EventArgs e)
        {
            this.color.Val1 = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Normal_R(object sender, EventArgs e)
        {
            this.color.Val2= (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Normal_ValueChanged(object sender, EventArgs e)
        {
            invert = (e as ParameterCheckbox.ChangedValue).Value;
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Normal;
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            if (NotUseColorChange)
            {
                unsafe
                {
                    byte* srcPtr = src.DataPointer;
                    byte* dstPtr = dst.DataPointer;

                    this.index_src = 0;

                    for (this.y = 0; this.y < dst.Height; this.y++)
                    {
                        for (this.x = 0; this.x < dst.Width; this.x++)
                        {
                            this.b_src = *(srcPtr + this.index_src * 3 + 0);
                            this.g_src = *(srcPtr + this.index_src * 3 + 1);
                            this.r_src = *(srcPtr + this.index_src * 3 + 2);

                            this.index_dst = this.y * dst.Width + this.x;

                            if (b_src + g_src + r_src != 0) //Kinect画像が背景（黒）じゃないとき
                            {
                                *(dstPtr + index_dst * 3 + 0) = (byte)this.color.Val0;
                                *(dstPtr + index_dst * 3 + 1) = (byte)this.color.Val1;
                                *(dstPtr + index_dst * 3 + 2) = (byte)this.color.Val2;
                            }
                            else
                            {
                                *(dstPtr + index_dst * 3 + 0) = (byte)this.colorBack.Val0;
                                *(dstPtr + index_dst * 3 + 1) = (byte)this.colorBack.Val1;
                                *(dstPtr + index_dst * 3 + 2) = (byte)this.colorBack.Val2;
                            }
                            this.index_src++;
                        }
                    }
                }
            }

            if (invert)
            {
                dst = ~src;
            }
            else
            {
                dst = src;
            }

        }
        public override string ToString()
        {
            return ImageProcesserType.Normal.ToString();
        }

    }
}
