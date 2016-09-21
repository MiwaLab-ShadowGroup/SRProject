using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class Canny : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 6;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        //Random rand = new Random();
        List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Goast = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;
        DepthBody db;
        float speedCtl;

        List<ColParticle> CvColParticle = new List<ColParticle>();
        List<List<ColParticle>> List_ColParticle = new List<List<ColParticle>>();
        List<List<ColParticle>> List_DelParticle = new List<List<ColParticle>>();
        List<int> delParticleNum = new List<int>();
        List<int> delListNum = new List<int>();

        private Dictionary<int, Scalar> _ColorMap = new Dictionary<int, Scalar>();


        public Canny() : base()
        {
            (ShadowMediaUIHost.GetUI("Canny_con_R") as ParameterSlider).ValueChanged += Canny_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Canny_con_G") as ParameterSlider).ValueChanged += Canny_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Canny_con_B") as ParameterSlider).ValueChanged += Canny_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Canny_bgd_R") as ParameterSlider).ValueChanged += Canny_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Canny_bgd_G") as ParameterSlider).ValueChanged += Canny_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Canny_bgd_B") as ParameterSlider).ValueChanged += Canny_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("Canny_Rate") as ParameterSlider).ValueChanged += Canny_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("Canny_Speed") as ParameterSlider).ValueChanged += Canny_Speed_ValueChanged;
            (ShadowMediaUIHost.GetUI("Canny_UseFade") as ParameterCheckbox).ValueChanged += Canny_UseFade_ValueChanged;

            (ShadowMediaUIHost.GetUI("Canny_CC_Blue") as ParameterButton).Clicked += Canny_CC_Blue_Clicked;
            (ShadowMediaUIHost.GetUI("Canny_CC_Orange") as ParameterButton).Clicked += Canny_CC_Orange_Clicked;
            (ShadowMediaUIHost.GetUI("Canny_CC_Yellow") as ParameterButton).Clicked += Canny_CC_Yellow_Clicked;
            (ShadowMediaUIHost.GetUI("Canny_CC_Pink") as ParameterButton).Clicked += Canny_CC_Pink_Clicked;
            (ShadowMediaUIHost.GetUI("Canny_CC_Green") as ParameterButton).Clicked += Canny_CC_Green_Clicked;


            (ShadowMediaUIHost.GetUI("Canny_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Canny_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Canny_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Canny_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Canny_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Canny_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Canny_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Canny_Speed") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Canny_UseFade") as ParameterCheckbox).ValueUpdate();
        }

        private void Canny_CC_Green_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Canny_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Canny_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Canny_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Canny_CC_Pink_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Canny_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Canny_con_G") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Canny_con_B") as ParameterSlider).m_slider.value = 255;
        }

        private void Canny_CC_Yellow_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Canny_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Canny_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Canny_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Canny_CC_Orange_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Canny_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Canny_con_G") as ParameterSlider).m_slider.value = 125;
            (ShadowMediaUIHost.GetUI("Canny_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void Canny_CC_Blue_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("Canny_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("Canny_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("Canny_con_B") as ParameterSlider).m_slider.value = 255;
        }

        private void Canny_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void Canny_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Canny_Speed_ValueChanged(object sender, EventArgs e)
        {
            this.speedCtl = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Canny_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Canny_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Canny_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Canny_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Canny_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Canny_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        Mat m_buffer;
        bool m_UseFade;
        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            this.delListNum.Clear();



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


            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.Erode(grayimage, grayimage, new Mat(), null, 2);
            Cv2.MedianBlur(grayimage, grayimage, 9);
            //Cv2.Dilate(grayimage, grayimage, new Mat(), null, 1);

            //Cv2.MedianBlur(grayimage, grayimage, 21);


            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,colorBack);
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);

            
            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            
            //とりあえず輪郭のリストを作る
            for (int i = 0; i < contour.Length; i++)
            {

                CvPoints.Clear();
                if (Cv2.ContourArea(contour[i]) > 1000)
                {

                    //for (int j = 0; j < contour[i].Length; j += (int)( contour[i].Length / this.sharpness + 1))
                    for (int j = 0; j < this.sharpness; j ++)
                        {

                            CvPoints.Add(contour[i][ j * contour[i].Length / this.sharpness ]);
                    }

                    this.List_Contours.Add(new List<Point>(CvPoints));

                }

            }
            var _contour = List_Contours.ToArray();
            //Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);


            //輪郭の数とパーティクルリストの数合わせ　消すやつ
            for (int i = 0; i <  this.List_ColParticle.Count - this.List_Contours.Count ; i++)
            {
                this.List_DelParticle.Add(new List<ColParticle>(this.List_ColParticle[this.List_ColParticle.Count - 1] )   );
                this.List_ColParticle.RemoveAt(this.List_ColParticle.Count - 1);
            }

            //消す輪郭の動き
            for (int i = 0; i < this.List_DelParticle.Count; i++)
            {
                this.delParticleNum.Clear();
                this.CvPoints.Clear();
                this.List_Goast.Clear();
                for (int j = 0; j < this.List_DelParticle[i].Count; j++)
                {
                    //合体先の輪郭へ動かす
                    //目的の輪郭がある場合
                    if (this.List_Contours.Count > i)
                    {
                        this.List_DelParticle[i][j].vec = this.List_Contours[i][j] - this.List_DelParticle[i][j].center;
                        this.List_DelParticle[i][j].vel = PointsLength(this.List_Contours[i][j] - this.List_DelParticle[i][j].center);
                        this.List_DelParticle[i][j].Move(speedCtl);

                    }
                    //目的の輪郭がない場合
                    else
                    {
                        //輪郭が一つでもある場合
                        if (this.List_Contours.Count > 0)
                        {
                            this.List_DelParticle[i][j].vec = this.List_Contours[0][j] - this.List_DelParticle[i][j].center;
                            this.List_DelParticle[i][j].vel = PointsLength(this.List_Contours[0][j] - this.List_DelParticle[i][j].center);
                            this.List_DelParticle[i][j].Move(speedCtl);
                        }
                        //輪郭が一つもない場合 動かない
                        else
                        {
                            this.List_DelParticle[i][j].vec = new Point(0,0);
                            this.List_DelParticle[i][j].vel = 0;
                        }
                    }
                    
                    //画面から出ないようにする
                    if (this.List_DelParticle[i][j].center.X < 0) this.List_DelParticle[i][j].center = new Point(0, this.List_DelParticle[i][j].center.Y);
                    if (this.List_DelParticle[i][j].center.X > src.Width ) this.List_DelParticle[i][j].center = new Point(src.Width , this.List_DelParticle[i][j].center.Y);
                    if (this.List_DelParticle[i][j].center.Y < 0) this.List_DelParticle[i][j].center = new Point(this.List_DelParticle[i][j].center.X, 0);
                    if (this.List_DelParticle[i][j].center.Y > src.Height ) this.List_DelParticle[i][j].center = new Point(this.List_DelParticle[i][j].center.X, src.Height);

                    //Cv2.Circle(m_buffer, this.List_ColParticle[i][j].center, 1, new Scalar(255, 255, 255));
                    this.CvPoints.Add(this.List_DelParticle[i][j].center);

                    if (this.List_DelParticle[i][j].vel < 10) this.delParticleNum.Add(j);
                    
                }

                if (this.CvPoints.Count > 3)
                {
                    this.List_Goast.Add(new List<Point>(CvPoints));
                    _contour = this.List_Goast.ToArray();
                    Cv2.FillPoly(m_buffer, _contour, this.List_DelParticle[i][0].color);
                }

                //近づいたパーティクルを消す
                for (int j = 0; j < this.delParticleNum.Count; j++)
                {
                    this.List_DelParticle[i].RemoveAt(delParticleNum[delParticleNum.Count - 1 - j]);
                }
                if (this.List_DelParticle[i].Count < 10) this.delListNum.Add(i);

                


            }
            //数の少なくなったリストを消す
            for (int i = 0; i< this.delListNum.Count; i++)
            {
                this.List_DelParticle.RemoveAt(this.delListNum[delListNum.Count - 1 - i ]);
            }
            


            //輪郭のリストに従ってパーティクルを動かす
            for (int i = 0; i < this.List_Contours.Count; i++)
            {
                
                this.CvColParticle.Clear();
                
                if(i < this.List_ColParticle.Count)
                {
                    CvPoints.Clear();
                    this.List_Goast.Clear();
                    //for (int j = 0; j < this.List_Contours[i].Count; j++)
                    for (int j = 0; j < this.List_ColParticle[i].Count; j++)
                    {
                        this.List_ColParticle[i][j].vec = this.List_Contours[i][j] - this.List_ColParticle[i][j].center;
                        //this.List_ColParticle[i][j].vel = speedCtl * PointsLength(this.List_Contours[i][j] - this.List_ColParticle[i][j].center);
                        this.List_ColParticle[i][j].vel = PointsLength(this.List_Contours[i][j] - this.List_ColParticle[i][j].center);
                        this.List_ColParticle[i][j].Move(speedCtl);

                        if (this.List_ColParticle[i][j].center.X < 0) this.List_ColParticle[i][j].center = new Point(0, this.List_ColParticle[i][j].center.Y);
                        if (this.List_ColParticle[i][j].center.X > src.Width) this.List_ColParticle[i][j].center = new Point(src.Width, this.List_ColParticle[i][j].center.Y);
                        if (this.List_ColParticle[i][j].center.Y < 0) this.List_ColParticle[i][j].center = new Point(this.List_ColParticle[i][j].center.X, 0);
                        if (this.List_ColParticle[i][j].center.Y > src.Height) this.List_ColParticle[i][j].center = new Point(this.List_ColParticle[i][j].center.X, src.Height);

                        CvPoints.Add(this.List_ColParticle[i][j].center);
                        //Cv2.Circle(m_buffer, this.List_ColParticle[i][j].center, 1, new Scalar(255, 255, 255));
                        
                    }
                    this.List_Goast.Add(new List<Point>(CvPoints)  ); 
                    _contour = this.List_Goast.ToArray();
                    Cv2.FillPoly(m_buffer, _contour, this.List_ColParticle[i][0].color);
                }
                else 
                {
                    for (int j = 0; j < this.List_Contours[i].Count; j++)
                    {
                        ColParticle colPtcl = new ColParticle(this.List_Contours[i][j], new Scalar(UnityEngine.Random.Range(150,255), UnityEngine.Random.Range(150, 255), UnityEngine.Random.Range(150, 255) )   );
                        this.CvColParticle.Add(colPtcl);
                    }
                    this.List_ColParticle.Add(new List<ColParticle>(CvColParticle));
                }


            }
          

    
            //Cv2.CvtColor(grayimage, m_buffer, OpenCvSharp.ColorConversion.GrayToBgr);
            //dst += grayimage;
            dst += m_buffer;
            this.List_Contours_Buffer = this.List_Contours;
            //Cv2.CvtColor(dstMat, dst, OpenCvSharp.ColorConversion.BgraToBgr);

        }
        public class ColParticle
        {
            public Scalar color { get; set; }
            public OpenCvSharp.CPlusPlus.Point center { get; set; }
            int radius { get; set; }
            public OpenCvSharp.CPlusPlus.Point vec { get; set; }
            public int maxR { get; set; }
            public float vel { get; set; }
            public int lifeTimeFrame { get; set; }
            int frameCount;

            //public ColParticle(Scalar color, OpenCvSharp.CPlusPlus.Point pt, int rad)
            //{
            //    this.color = color;
            //    this.center = pt;
            //    this.radius = rad;
            //    this.frameCount = 0;
            //}
            public ColParticle( OpenCvSharp.CPlusPlus.Point pt, Scalar col)
            {
                this.center = pt;
                this.vec = new Point (0,0);
                this.vel = 0;
                this.color = col;



            }
            public void Move(float speed)
            {
                //this.center += this.vec * this.vel;
                if (this.vel  < 1) this.vel = 1;
                //this.center += new Point (speed * this.vec.X / this.vel, speed *this.vec.Y / this.vel);
                if (vel > 80)
                {
                    this.center += new Point(speed * 0.1 * this.vec.X, speed * 0.1 * this.vec.Y);
                }
                else
                {
                    this.center += new Point(speed * this.vec.X, speed * this.vec.Y);
                }
            }

            //public bool Life()
            //{
            //    this.Move();
            //    this.frameCount++;
            //    if (this.radius < this.maxR)
            //    {
            //        this.radius++;
            //    }
            //    if (this.frameCount < this.lifeTimeFrame)
            //    {

            //        return true;
            //    }
            //    else
            //    {
            //        return false;
            //    }

            //}


            //public void Draw(ref Mat src)
            //{
            //    src.Circle(this.center, this.radius, this.color, -1);
            //}
        }
   


        private float PointsLength(Point pt1, Point pt2)
        {
            return (float)Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + (Math.Pow(pt1.Y - pt2.Y, 2)));
        }
        private float PointsLength(Point pt1)
        {
            return (float)Math.Sqrt(Math.Pow(pt1.X, 2) + (Math.Pow(pt1.Y, 2)));
        }

        public override string ToString()
        {
            return "Canny";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Canny;
        }

        public bool IsFirstFrame { get; private set; }
    }
    



}
