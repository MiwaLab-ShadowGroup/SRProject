using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;
using UnityEngine;
using System.IO;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class BrightCheck : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            this.Update(ref src, ref dst);
        }


        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        Scalar color;
        Scalar colorBack = new Scalar(0, 0, 0);

        double ellipsePt_B_x;
        double ellipsePt_B_y;

        double ellipsePt_D_x;
        double ellipsePt_D_y;

        double radius_x;
        double radius_y;
        double radius_size;
        double angle;

        float hInput;
        float vInput;

        bool vsbGLD;
        bool vsbTxt;
        float brightTxt;

        public BrightCheck() : base()
        {
            (ShadowMediaUIHost.GetUI("Bright_bright") as ParameterSlider).ValueChanged += BrightCheck_bright_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_pt_B_x") as ParameterSlider).ValueChanged += BrightCheck_pt_B_x_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_pt_B_y") as ParameterSlider).ValueChanged += BrightCheck_pt_B_y_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_pt_D_x") as ParameterSlider).ValueChanged += BrightCheck_pt_D_x_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_pt_D_y") as ParameterSlider).ValueChanged += BrightCheck_pt_D_y_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_rad_x") as ParameterSlider).ValueChanged += BrightCheck_rad_x_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_rad_y") as ParameterSlider).ValueChanged += BrightCheck_rad_y_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_rad_size") as ParameterSlider).ValueChanged += BrightCheck_rad_size_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_angle") as ParameterSlider).ValueChanged += BrightCheck_angle_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_vsbGLD") as ParameterCheckbox).ValueChanged += BrightCheck_vsbGLD_ValueChanged;
            (ShadowMediaUIHost.GetUI("Bright_vsbTxt") as ParameterCheckbox).ValueChanged += BrightCheck_vsbTxt_ValueChanged;

            (ShadowMediaUIHost.GetUI("Bright_bright") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Bright_pt_B_x") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Bright_pt_B_y") as ParameterSlider).ValueUpdate(); 
            (ShadowMediaUIHost.GetUI("Bright_pt_D_y") as ParameterSlider).ValueUpdate(); 
            (ShadowMediaUIHost.GetUI("Bright_pt_D_y") as ParameterSlider).ValueUpdate();                                                                                        
            (ShadowMediaUIHost.GetUI("Bright_rad_x") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Bright_rad_y") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Bright_rad_size") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Bright_angle") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Bright_vsbGLD") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Bright_vsbTxt") as ParameterCheckbox).ValueUpdate();
        }



        private void BrightCheck_vsbGLD_ValueChanged(object sender, EventArgs e)
        {
            this.vsbGLD = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void BrightCheck_vsbTxt_ValueChanged(object sender, EventArgs e)
        {
            this.vsbTxt = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void BrightCheck_bright_ValueChanged(object sender, EventArgs e)
        {
            //this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;
            //this.color.Val1 = this.color.Val0;
            //this.color.Val2 = this.color.Val0;
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_pt_B_x_ValueChanged(object sender, EventArgs e)
        {
            this.ellipsePt_B_x = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_pt_B_y_ValueChanged(object sender, EventArgs e)
        {
            this.ellipsePt_B_y = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_pt_D_x_ValueChanged(object sender, EventArgs e)
        {
            this.ellipsePt_D_x = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_pt_D_y_ValueChanged(object sender, EventArgs e)
        {
            this.ellipsePt_D_y = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_rad_x_ValueChanged(object sender, EventArgs e)
        {
            this.radius_x = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_rad_y_ValueChanged(object sender, EventArgs e)
        {
            this.radius_y = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void BrightCheck_rad_size_ValueChanged(object sender, EventArgs e)
        {
            this.radius_size = (double)(e as ParameterSlider.ChangedValue).Value;
        }


        private void BrightCheck_angle_ValueChanged(object sender, EventArgs e)
        {
            this.angle = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        





        Mat m_buffer;

        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();

            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
            }
            else
            {
                m_buffer *= 0;
            }

            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);

            //キー入力
            this.hInput = Input.GetAxis("Horizontal");
            this.hInput = Math.Sign(this.hInput);

            this.vInput = Input.GetAxis("Vertical");
            this.vInput = Math.Sign(this.vInput  );

          
            this.color.Val0 += this.hInput;
            this.color.Val0 += 2 * this.vInput;

            if(this.color.Val0 > 255)
            {
                this.color.Val0 = 255;
            }
            if (this.color.Val0 < 0)
            {
                this.color.Val0 = 0;
            }

            this.color.Val1 = this.color.Val0;
            this.color.Val2 = this.color.Val0;

         



            Cv2.Ellipse(m_buffer, new Point(this.ellipsePt_B_x, this.ellipsePt_B_y), new Size(this.radius_x * this.radius_size, this.radius_y * this.radius_size), 0, 0, angle, new Scalar(255,255,255), -1);
            Cv2.Ellipse(m_buffer, new Point(this.ellipsePt_D_x, this.ellipsePt_D_y), new Size(this.radius_x * this.radius_size, this.radius_y * this.radius_size), 0, 0, angle, color, -1);


            if (this.vsbGLD)
            {
                Cv2.Circle(m_buffer, src.Width-10, src.Height-10, 10, new Scalar(255, 200, 100),3);
                Cv2.Circle(m_buffer, src.Width-10, 10, 10, new Scalar(255, 200, 100),3);
                Cv2.Circle(m_buffer, 10, src.Height-10, 10, new Scalar(255, 200, 100),3);
                Cv2.Circle(m_buffer, 10, 10, 5, new Scalar(255, 200, 100),3);
                Cv2.Circle(m_buffer, src.Width / 2, 10, 10, new Scalar(255, 200, 100),3);
                Cv2.Circle(m_buffer, src.Width / 2, src.Height-10, 10, new Scalar(255, 200, 100),3);


                Cv2.Circle(m_buffer, new Point(this.ellipsePt_B_x, this.ellipsePt_B_y), 20, new Scalar(255, 200, 100));

            }

            //テキスト表示
            if (this.vsbTxt)
            {
                this.brightTxt = (float)this.color.Val0;
                Cv2.PutText(m_buffer, this.brightTxt.ToString(),new Point(90,400),FontFace.HersheySimplex,1,new Scalar(255,255,255));

            }

            dst += m_buffer;

        }

        public override string ToString()
        {
            return "BrightCheck";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.BrightCheck;
        }

        public bool IsFirstFrame { get; private set; }
    }


}
