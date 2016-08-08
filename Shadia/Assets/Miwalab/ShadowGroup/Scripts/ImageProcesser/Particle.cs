using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Particle : AShadowImageProcesser
    {
        #region
        Mat velx = new Mat();
        Mat vely = new Mat();
        Mat status = new Mat();
        Mat error = new Mat();
        Mat grayMat = new Mat();
        Mat erodedMat;
        Mat dstMat;
        Mat preMat;
        Mat diffMat = new Mat();
        List<MyPirticle> List_particle;
        bool flag = false;
        bool IsDamp = false;
        #endregion


        int intervalOfContour = 5;
        Scalar bgColor;
        
        int vel =10;
        int lifeTimeFrame = 30;
        int threthOPFsize = 1500;

        Vec2i red = new Vec2i(100, 255);
        Vec2i green = new Vec2i(0, 100);
        Vec2i blue = new Vec2i(100, 255);

        int particleSizeMax = 5;

        public Particle()
        {
            this.grayMat = new Mat();
            this.List_particle = new List<MyPirticle>();

            (ShadowMediaUIHost.GetUI("Interval_of_Contour") as ParameterSlider).ValueChanged += Interval_of_Contour_ValueChanged;
            (ShadowMediaUIHost.GetUI("Velocity") as ParameterSlider).ValueChanged += Velocity_ValueChanged;
            (ShadowMediaUIHost.GetUI("Lifetime_Frame") as ParameterSlider).ValueChanged += Lifetime_Frame_ValueChanged;
            (ShadowMediaUIHost.GetUI("threthOPFsize") as ParameterSlider).ValueChanged += threthOPFsize_ValueChanged;
            (ShadowMediaUIHost.GetUI("Particle_bgd_R") as ParameterSlider).ValueChanged += Particle_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("Particle_bgd_G") as ParameterSlider).ValueChanged += Particle_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("Particle_bgd_B") as ParameterSlider).ValueChanged += Particle_bgd_B_ValueChanged;


            (ShadowMediaUIHost.GetUI("Interval_of_Contour") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Velocity") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Lifetime_Frame") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("threthOPFsize") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Particle_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Particle_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("Particle_bgd_B") as ParameterSlider).ValueUpdate();

        }

        private void Lifetime_Frame_ValueChanged(object sender, EventArgs e)
        {
            this.lifeTimeFrame = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Particle_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.bgColor.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Particle_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.bgColor.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void Particle_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.bgColor.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void threthOPFsize_ValueChanged(object sender, EventArgs e)
        {
            this.threthOPFsize = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Velocity_ValueChanged(object sender, EventArgs e)
        {
            this.vel = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void Interval_of_Contour_ValueChanged(object sender, EventArgs e)
        {
            this.intervalOfContour = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Particle;
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            this.Update(ref src, ref dst);
        }

        private void Update(ref Mat src, ref Mat dst)
        {
            #region
            Cv2.CvtColor(src, this.grayMat, ColorConversion.BgrToGray);
            int imgW = this.grayMat.Width;
            int imgH = this.grayMat.Height;
            Random random = new Random();
            dst = new Mat(imgH, imgW, MatType.CV_8UC3, this.bgColor);
            if (!this.flag)
            {
                this.preMat = this.grayMat.Clone();
                this.erodedMat = new Mat(imgH, imgW, MatType.CV_8UC1);
                this.velx = new Mat(imgH, imgW, MatType.CV_32FC1);
                this.vely = new Mat(imgH, imgW, MatType.CV_32FC1);
                this.flag = true;
            }
            #endregion

            //膨張縮小処理
            for (int i = 0; i < 5; i++)
            {
                Cv2.Erode(this.grayMat, this.erodedMat, null, null, 1, BorderType.Constant, null);
                Cv2.Dilate(this.grayMat, this.erodedMat, null, null, 1, BorderType.Constant, null);
            }

            Cv2.Erode(this.grayMat, this.erodedMat,null,null,2,BorderType.Constant,null);
            
            //オプティカルフローの計算
            Cv.CalcOpticalFlowLK(this.preMat.ToCvMat(), this.grayMat.ToCvMat(), Cv.Size(15, 15), this.velx.ToCvMat(), this.vely.ToCvMat());

            //輪郭検出
            OpenCvSharp.CPlusPlus.Point[][] contour;
            HierarchyIndex[] hierarchy;
            CvScalar color = new CvScalar(255, 0, 0);
            Cv2.FindContours(this.grayMat, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            //オプティカルフローの最大値取得
            int size = this.SerchMaxVelOfOCF(contour, this.velx, this.vely);
            

            //粒子を飛ばさない時
            //輪郭上に粒子生成 + 輪郭内部を粒子で埋める
            if (!this.IsDamp)
            {
                //オプティカルフロー再計算
                Cv.CalcOpticalFlowLK(this.erodedMat.ToCvMat(), this.grayMat.ToCvMat(), Cv.Size(15, 15), this.velx.ToCvMat(), this.vely.ToCvMat());

                //粒子の作製
                #region
                for (int i = 0; i < contour.Length; i++)
                {
                    if (Cv2.ContourArea(contour[i]) > 1000)
                    {
                        for (int j = 0; j < contour[i].Length; j += intervalOfContour)
                        {
                            //オプティカルフロー取得
                            CvPoint pt = contour[i][j];
                            double dx = Cv.GetReal2D(this.velx.ToCvMat(), pt.Y, pt.X);
                            double dy = Cv.GetReal2D(this.vely.ToCvMat(), pt.Y, pt.X);

                            //粒子の生成
                            int b = random.Next(this.blue.Item0, this.blue.Item1);
                            int g = random.Next(this.green.Item0, this.green.Item1);
                            int r = random.Next(this.red.Item0, this.red.Item1);
                            var mycolor = new Scalar(b, g, r);
                            var p = new MyPirticle(mycolor, pt, 1);
                            p.maxR = this.particleSizeMax;

                            p.vel = 1;
                            p.vec = new CvPoint(-(int)(dx * this.vel), -(int)(dy * this.vel));

                            p.lifeTimeFrame = this.lifeTimeFrame;


                            this.List_particle.Add(p);

                        }
                    }
                }
                #endregion
            }

            //描画と生死判断
            this.List_particle = this.DrawPartcile(this.List_particle, ref dst);

            //粒子を飛ばすかの判断
            #region
            //粒子がないとき（粒子を飛ばし終わった時）もしくは粒子が飛ばない状況の時再判断）
            if (this.List_particle.Count == 0 || !this.IsDamp)
            {          
                if (size > this.threthOPFsize)
                {
                    this.IsDamp = true;
                }
                else
                {
                    this.IsDamp = false;
                }
                //Console.WriteLine(this.IsDamp);
            }
            
            //次フレームで粒子を飛ばさないならば粒子全消去
            if (!this.IsDamp)
            {
                this.List_particle = new List<MyPirticle>();
            }
            #endregion
            //前フレームの画像保持
            this.preMat = this.grayMat.Clone();
        }

        //輪郭上のオプティカルフローのサイズの最大値を返す
        int SerchMaxVelOfOCF(OpenCvSharp.CPlusPlus.Point[][] contour, Mat Xs, Mat Ys) 
        {
            int size = 0;

            for (int i = 0; i < contour.Length; i++)
            {
                
                if (Cv2.ContourArea(contour[i]) > 1000)
                {
                    
                    for (int j = 0; j < contour[i].Length; j += 10)
                    {

                        //オプティカルフロー取得
                        CvPoint pt = contour[i][j];
                        int dx = (int)Cv.GetReal2D(Xs.ToCvMat(), pt.Y, pt.X);
                        int dy = (int)Cv.GetReal2D(Ys.ToCvMat(), pt.Y, pt.X);

                        int localSize = (int)Math.Sqrt(dx * dx + dy * dy);
                        
                        if(size < localSize)
                        {
                            size = localSize;
                        }

                    }
                    
                }
                
            }
            return size;
        }

        //パーティクルの描画と生死判断
        List<MyPirticle> DrawPartcile(List<MyPirticle> list, ref Mat dst)
        {
            List<MyPirticle> local_list = new List<MyPirticle>();
            foreach (var p in list)
            {
                p.Draw(ref dst);

                if (p.Life())
                {
                    local_list.Add(p);
                }
            }
            return local_list;
        }

        public override string ToString()
        {
            return "particle";
        }
        public bool IsFirstFrame { get; private set; }
        
    }

    public class MyPirticle
    {
        Scalar color { get; set; }
        OpenCvSharp.CPlusPlus.Point center { get; set; }
        int radius { get; set; }
        public OpenCvSharp.CPlusPlus.Point vec { get; set; }
        public int maxR { get; set; }
        public int vel { get; set; }
        public int lifeTimeFrame { get; set; }
        int frameCount;

        public MyPirticle(Scalar color , OpenCvSharp.CPlusPlus.Point pt, int rad)
        {
            this.color = color;
            this.center = pt;
            this.radius = rad;
            this.frameCount = 0;
        }
        public void Move()
        {
            this.center += this.vec * this.vel;
        }
 
        public bool Life()
        {
            this.Move();
            this.frameCount++;
            if (this.radius < this.maxR)
            {
                this.radius ++ ;
            }
            if (this.frameCount < this.lifeTimeFrame)
            {
                
                return true;
            }
            else
            {
                return false;
            }

        }


        public void Draw(ref Mat src)
        {
            src.Circle(this.center, this.radius, this.color, -1, LineType.Link8);
        }
    }

}

