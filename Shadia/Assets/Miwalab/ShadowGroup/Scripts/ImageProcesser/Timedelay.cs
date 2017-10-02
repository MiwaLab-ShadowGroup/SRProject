using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Timedelay : AShadowImageProcesser
    {
        private int DelayCounter;
        private bool invert;
        private bool flip;
        int count = 0;
        Scalar color;
        Scalar colorBack;
        int x, y, b_src, g_src, r_src, index_src, index_dst;

        Queue<Mat> queue;


        public Timedelay()
            :base()
        {

            DelayCounter =100;
            //データの登録

            this.queue = new Queue<Mat>();
            (ShadowMediaUIHost.GetUI("TimeDelay_DelayTime") as ParameterSlider).ValueChanged += DelayTime_ValueChanged;

            (ShadowMediaUIHost.GetUI("TimeDelay_DelayTime") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("TimeDelay_Invert") as ParameterCheckbox).ValueChanged += TimeDelay_Invert;
            (ShadowMediaUIHost.GetUI("TimeDelay_Flip") as ParameterCheckbox).ValueChanged += TimeDelay_Flip;
            (ShadowMediaUIHost.GetUI("TimeDelay_R") as ParameterSlider).ValueChanged += Timedelay_R;
            (ShadowMediaUIHost.GetUI("TimeDelay_G") as ParameterSlider).ValueChanged += TimeDelay_G;
            (ShadowMediaUIHost.GetUI("TimeDelay_B") as ParameterSlider).ValueChanged += TimeDelay_B;
            (ShadowMediaUIHost.GetUI("TimeDelay_bgd_R") as ParameterSlider).ValueChanged += TimeDelay_bgd_R;
            (ShadowMediaUIHost.GetUI("TimeDelay_bgd_G") as ParameterSlider).ValueChanged += TimeDelay_bgd_G;
            (ShadowMediaUIHost.GetUI("TimeDelay_bgd_B") as ParameterSlider).ValueChanged += TimeDelay_bgd_B;
        }

        private void TimeDelay_Flip(object sender, EventArgs e)
        {
            this.flip = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void TimeDelay_bgd_B(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void TimeDelay_bgd_G(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void TimeDelay_bgd_R(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void TimeDelay_B(object sender, EventArgs e)
        {
            this.color.Val0 = (float)(e as ParameterSlider.ChangedValue).Value;

        }

        private void TimeDelay_G(object sender, EventArgs e)
        {
            this.color.Val1 = (float)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Timedelay_R(object sender, EventArgs e)
        {
            this.color.Val2 = (float)(e as ParameterSlider.ChangedValue).Value;

        }

        private void TimeDelay_Invert(object sender, EventArgs e)
        {
            this.invert = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void DelayTime_ValueChanged(object sender, EventArgs e)
        {
            DelayCounter = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        

        private void Update(ref Mat src, ref Mat dst)
        {
            Mat item = new Mat();
            src.CopyTo(item);


            this.queue.Enqueue(item);
            

            if (this.queue.Count > this.DelayCounter) //この値で遅れ時間を調整(UIで変えられる)
            {
                this.queue.Dequeue().CopyTo(dst);
                
            }

            if(this.queue.Count > this.DelayCounter)
            {
                this.queue.Dequeue();
            }

            //色変えtest
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

            //左右反転
            if (flip)
            {
                Cv2.Flip(src, dst, OpenCvSharp.FlipMode.Y);
            }
            else
            {
                dst = src;
            }

            //色反転
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
            return "Timedelay";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.TimeDelay;
        }

        public bool IsFirstFrame { get; private set; }

    }
}
