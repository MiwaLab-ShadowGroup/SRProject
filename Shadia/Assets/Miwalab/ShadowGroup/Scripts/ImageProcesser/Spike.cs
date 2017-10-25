using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Spike : AShadowImageProcesser
    {
        #region
        CvGraph grapth;
        CvSize size;
        //int interval;
        Mat thin;
        Mat raster_r;

        /// <summary>
        /// toUI
        /// </summary>
        int interval = 25;
        int length = 8;
        int radius = 2;
        Scalar color;
        Scalar colorBack;

        int count = 0;

        #endregion
        public Spike():base()
        {
            (ShadowMediaUIHost.GetUI("Spike_inval") as ParameterSlider).ValueChanged += Spike_inval_Changed;
            (ShadowMediaUIHost.GetUI("Spike_lngth") as ParameterSlider).ValueChanged += Spike_lngth_Changed;
            (ShadowMediaUIHost.GetUI("Spike_rdius") as ParameterSlider).ValueChanged += Spike_rdius_Changed;
            (ShadowMediaUIHost.GetUI("Spike_con_R") as ParameterSlider).ValueChanged += Spike_con_R_Changed;
            (ShadowMediaUIHost.GetUI("Spike_con_G") as ParameterSlider).ValueChanged += Spike_con_G_Changed;
            (ShadowMediaUIHost.GetUI("Spike_con_B") as ParameterSlider).ValueChanged += Spike_con_B_Changed;
            (ShadowMediaUIHost.GetUI("Spike_bgd_R") as ParameterSlider).ValueChanged += Spike_bgd_R_Changed;
            (ShadowMediaUIHost.GetUI("Spike_bgd_G") as ParameterSlider).ValueChanged += Spike_bgd_G_Changed;
            (ShadowMediaUIHost.GetUI("Spike_bgd_B") as ParameterSlider).ValueChanged += Spike_bgd_B_Changed;

            (ShadowMediaUIHost.GetUI("Spike_CC_Blue") as ParameterButton).Clicked += Spike_CC_Blue_Clicked;
            (ShadowMediaUIHost.GetUI("Spike_CC_Orange") as ParameterButton).Clicked += Spike_CC_Orange_Clicked;
            (ShadowMediaUIHost.GetUI("Spike_CC_Yellow") as ParameterButton).Clicked += Spike_CC_Yellow_Clicked;
            (ShadowMediaUIHost.GetUI("Spike_CC_Pink") as ParameterButton).Clicked += Spike_CC_Pink_Clicked;
            (ShadowMediaUIHost.GetUI("Spike_CC_Green") as ParameterButton).Clicked += Spike_CC_Green_Clicked;


            (ShadowMediaUIHost.GetUI("Spike_inval") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Spike_lngth") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Spike_rdius") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Spike_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Spike_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Spike_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Spike_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Spike_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Spike_bgd_B") as ParameterSlider).ValueUpdate();
            
        }

        private void Spike_CC_Green_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Spike_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Spike_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Spike_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Spike_CC_Pink_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Spike_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Spike_con_G") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Spike_con_B") as ParameterSlider).m_slider.value = 255;
        }

        private void Spike_CC_Yellow_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Spike_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Spike_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Spike_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Spike_CC_Orange_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Spike_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Spike_con_G") as ParameterSlider).m_slider.value = 125;
            (ShadowMediaUIHost.GetUI("Spike_con_B") as ParameterSlider).m_slider.value = 0;

        }

        private void Spike_CC_Blue_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Spike_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Spike_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Spike_con_B") as ParameterSlider).m_slider.value = 255;
        }


        private void Spike_bgd_B_Changed(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Spike_bgd_G_Changed(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;
            
        }

        private void Spike_bgd_R_Changed(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;
            
        }

        private void Spike_con_B_Changed(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;
            
        }

        private void Spike_con_G_Changed(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;
            
        }

        private void Spike_con_R_Changed(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Spike_rdius_Changed(object sender, EventArgs e)
        {
            this.radius = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Spike_lngth_Changed(object sender, EventArgs e)
        {
            this.length = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Spike_inval_Changed(object sender, EventArgs e)
        {
            this.interval = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        private Mat grayimage = new Mat();
        private void Update(ref Mat src, ref Mat dst)
        {


            Mat dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, this.colorBack);

            int imgW = src.Width;
            int imgH = src.Height;
            

            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            

            OpenCvSharp.CPlusPlus.Point[][] contour; 
            HierarchyIndex[] hierarchy;
            CvScalar color = new CvScalar(255, 0, 0);

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            Random rand = new Random();
            double x = 0;
            double y = 0;
            double dir;
            int size = 0;
            OpenCvSharp.CPlusPlus.Point center ;
            for (int i = 0; i < contour.Length; i++)
            {
                if (Cv2.ContourArea(contour[i]) > 100)//面積ContourArea100以上（100以下はノイズとして無視）
                {
                    for (int j = 0; j < contour[i].Length - this.interval; j = j + this.interval)
                    {
                        OpenCvSharp.CPlusPlus.Point vec = contour[i][j + this.interval] - contour[i][j];
                        size = (int)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                        dir = (int)(Math.Atan2(vec.Y, vec.X) * 180);
                        //dir = rand.Next(dir - this.rangeRad, dir + this.rangeRad);
                        x = Math.Cos(dir);
                        y = Math.Sin(dir);


                        for (int k= 0; k < this.length; k++)
                        {
                            center = contour[i][j] + new OpenCvSharp.CPlusPlus.Point(x * k , y * k);
                            dstMat.Circle(center, 1, this.color, this.radius, LineType.Link8, 0);
                        }
                        for (int k = 0; k < this.length; k++)
                        {
                            center = contour[i][j] + new OpenCvSharp.CPlusPlus.Point( - x * k, - y * k);
                            dstMat.Circle(center, 1, this.color, this.radius, LineType.Link8, 0);
                        }

                    }
                }
            }

            dst = dstMat;
            

        }

        public override string ToString()
        {
            return "Vector";
        }
        public bool IsFirstFrame { get; private set; }


        void Vectorize(Mat raster, int interval)
        {
            int mask;
            CvGraphScanner scanner;

            this.size = raster.Size();
            this.interval = interval;

            this.thin = new Mat(this.size, MatType.CV_8UC1);
            this.raster_r = new Mat(this.size, MatType.CV_8UC1);

            //細線化
            
            //反転

            //周囲は白

            //特徴点検出

            //エッジ検出
            

        }

        void Rasterize(Mat img)
        {
            CvGraphScanner scanner;
            int mask;
            CvPoint point;

            scanner = new CvGraphScanner(this.grapth);

        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Spike;
        }
    }
}
