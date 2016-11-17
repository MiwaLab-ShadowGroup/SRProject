using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class LeastSquare : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 100;
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

 
        List<int> delParticleNum = new List<int>();
        List<int> delListNum = new List<int>();
        private Dictionary<int, Scalar> _ColorMap = new Dictionary<int, Scalar>();



        List<OpenCvSharp.CPlusPlus.Point> contour_Center = new List<Point>();
        List<StackContsGroup> Tree_ContsGroup = new List<StackContsGroup>();
        List<NowContsGroup> List_NowContsGroup = new List<NowContsGroup>();
        int stackNum = 20;


        int numberingId= 0;
        List<int?> activeIdList = new List<int?>();
        float thresholdDist = 100;

        double dist = 0;
        double minDist = 1000; //とりあえず笑　その場しのぎ
        int bufferNum = 0;

        List<int?> protectId = new List<int?>();
        int? bufNum = null;
        bool existNum = false;
        int useContNum;



        public LeastSquare() : base()
        {
            (ShadowMediaUIHost.GetUI("LeastSquare_con_R") as ParameterSlider).ValueChanged += LeastSquare_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_G") as ParameterSlider).ValueChanged += LeastSquare_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_B") as ParameterSlider).ValueChanged += LeastSquare_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("LeastSquare_bgd_R") as ParameterSlider).ValueChanged += LeastSquare_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("LeastSquare_bgd_G") as ParameterSlider).ValueChanged += LeastSquare_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("LeastSquare_bgd_B") as ParameterSlider).ValueChanged += LeastSquare_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("LeastSquare_Rate") as ParameterSlider).ValueChanged += LeastSquare_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("LeastSquare_Speed") as ParameterSlider).ValueChanged += LeastSquare_Speed_ValueChanged;
            (ShadowMediaUIHost.GetUI("LeastSquare_UseFade") as ParameterCheckbox).ValueChanged += LeastSquare_UseFade_ValueChanged;

            (ShadowMediaUIHost.GetUI("LeastSquare_CC_Blue") as ParameterButton).Clicked += LeastSquare_CC_Blue_Clicked;
            (ShadowMediaUIHost.GetUI("LeastSquare_CC_Orange") as ParameterButton).Clicked += LeastSquare_CC_Orange_Clicked;
            (ShadowMediaUIHost.GetUI("LeastSquare_CC_Yellow") as ParameterButton).Clicked += LeastSquare_CC_Yellow_Clicked;
            (ShadowMediaUIHost.GetUI("LeastSquare_CC_Pink") as ParameterButton).Clicked += LeastSquare_CC_Pink_Clicked;
            (ShadowMediaUIHost.GetUI("LeastSquare_CC_Green") as ParameterButton).Clicked += LeastSquare_CC_Green_Clicked;


            (ShadowMediaUIHost.GetUI("LeastSquare_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LeastSquare_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LeastSquare_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LeastSquare_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LeastSquare_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LeastSquare_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LeastSquare_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LeastSquare_Speed") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LeastSquare_UseFade") as ParameterCheckbox).ValueUpdate();
        }

        private void LeastSquare_CC_Green_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("LeastSquare_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void LeastSquare_CC_Pink_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("LeastSquare_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_G") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_B") as ParameterSlider).m_slider.value = 255;
        }

        private void LeastSquare_CC_Yellow_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("LeastSquare_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void LeastSquare_CC_Orange_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("LeastSquare_con_R") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_G") as ParameterSlider).m_slider.value = 125;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_B") as ParameterSlider).m_slider.value = 0;
        }

        private void LeastSquare_CC_Blue_Clicked(object sender, EventArgs e)
        {
            (ShadowMediaUIHost.GetUI("LeastSquare_con_R") as ParameterSlider).m_slider.value = 0;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_G") as ParameterSlider).m_slider.value = 255;
            (ShadowMediaUIHost.GetUI("LeastSquare_con_B") as ParameterSlider).m_slider.value = 255;
        }

        private void LeastSquare_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void LeastSquare_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void LeastSquare_Speed_ValueChanged(object sender, EventArgs e)
        {
            this.speedCtl = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void LeastSquare_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LeastSquare_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LeastSquare_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LeastSquare_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LeastSquare_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LeastSquare_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        Mat m_buffer;
        bool m_UseFade;
        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            this.delListNum.Clear();
            this.contour_Center.Clear();
            this.List_NowContsGroup.Clear();

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
            //Cv2.Erode(grayimage, grayimage, new Mat(), null, 2);
            //Cv2.MedianBlur(grayimage, grayimage, 9);
            //Cv2.Dilate(grayimage, grayimage, new Mat(), null, 1);

            Cv2.GaussianBlur(grayimage,grayimage, new Size(3, 3), 0f);

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
                    if (contour[i].Length > this.sharpness) {
                        {
                            //重心検出処理
                            var cont = contour[i].ToArray();

                            var M = Cv2.Moments(cont);
                            this.contour_Center.Add(new Point((M.M10 / M.M00), (M.M01 / M.M00)));


                            //for (int j = 0; j < contour[i].Length; j += (int)( contour[i].Length / this.sharpness + 1))
                            for (int j = 0; j < this.sharpness; j++)
                            {
                                //this.useContNum = j * contour[i].Length / this.sharpness;
                                CvPoints.Add(contour[i][j * contour[i].Length / this.sharpness]);
                            }

                            //this.List_Contours.Add(new List<Point>(CvPoints));
                            this.List_NowContsGroup.Add(new NowContsGroup(null, CvPoints, new Point((M.M10 / M.M00), (M.M01 / M.M00))));
                        }
                }

            }
            var _contour = List_Contours.ToArray();
            //Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);



            //現在の輪郭すべてにIDを振る

            this.dist = 0;
            this.minDist = 1000; //とりあえず笑　その場しのぎ
            this.bufferNum = 0;
            this.activeIdList.Clear();


            //前のフレームの輪郭がゼロのときのことを考えていない？
            //ワンフレーム前から輪郭数が減っている時 同じ場合も含む
            if (this.List_NowContsGroup.Count <= this.Tree_ContsGroup.Count)
            {
                for (int i = 0; i < this.List_NowContsGroup.Count; ++i)
                {
                    for (int j = 0; j < this.Tree_ContsGroup.Count; ++j)
                    {
                        dist = Point.Distance(this.List_NowContsGroup[i].contCenter, this.Tree_ContsGroup[j].contCenter);
                        if (dist < minDist)
                        {
                            bufferNum = j;
                            minDist = dist;
                        }
                    }
                    //輪郭が離れすぎていたら新しいものとして分類
                    if (dist > this.thresholdDist)
                    {
                        this.List_NowContsGroup[i].trackingId = null;
                    }
                    else
                    {
                        this.List_NowContsGroup[i].trackingId = this.Tree_ContsGroup[bufferNum].trackingId;
                    }

                }
            }
            //ワンフレーム前から輪郭数が増えている時
            else
            {
                for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
                {
                    for (int j = 0; i < this.List_NowContsGroup.Count; ++j)
                    {
                        dist = Point.Distance( this.Tree_ContsGroup[i].contCenter, this.List_NowContsGroup[j].contCenter);
                        if (dist < minDist)
                        {
                            bufferNum = j;
                            minDist = dist;
                        }
                    }

                    //輪郭が離れすぎていたら新しいものとして分類
                    if (dist > this.thresholdDist)
                    {
                        this.List_NowContsGroup[bufferNum].trackingId = null;
                    }
                    else
                    {
                        this.List_NowContsGroup[bufferNum].trackingId = this.Tree_ContsGroup[i].trackingId;

                    }
                }
            }

            //IDを整理する
            for (int i = 0; i < this.List_NowContsGroup.Count; ++i)
            {
                //増えた輪郭に新しいIDを振る
                if (this.List_NowContsGroup[i].trackingId == null)
                {
                    this.numberingId++;
                    this.List_NowContsGroup[i].trackingId = this.numberingId;
                }

                //重複のチェック
                if (this.activeIdList.Contains(this.List_NowContsGroup[i].trackingId))
                {
                    this.numberingId++;
                    this.List_NowContsGroup[i].trackingId = this.numberingId;
                }
                this.activeIdList.Add( this.List_NowContsGroup[i].trackingId);
            }
            //現在の輪郭のID振りは終了
        



            //現在の輪郭リストを過去の輪郭ツリーに代入する
            protectId.Clear();

            for (int i = 0; i < this.List_NowContsGroup.Count; ++i)
            {
                this.existNum = false;

                //おなじIDがあれば追加する
                for (int j = 0; j < this.Tree_ContsGroup.Count; ++j)
                {
                    if (this.List_NowContsGroup[i].trackingId == this.Tree_ContsGroup[j].trackingId)
                    {
                        existNum = true;

                        this.Tree_ContsGroup[j].AddContsGroup(this.List_NowContsGroup[i].contsList,this.List_NowContsGroup[i].contCenter,this.stackNum);
                        protectId.Add(this.List_NowContsGroup[i].trackingId);
                    }
                }
                //新しくできたものなら新しいリストを作る
                if (existNum == false)
                {
                    Debug.Log("this.List_NowContsGroup[i].trackingId ; " + this.List_NowContsGroup[i].trackingId);
                    Debug.Log("this.List_NowContsGroup[i].contsList.count ; " + this.List_NowContsGroup[i].contsList.Count);
                    this.Tree_ContsGroup.Add(new StackContsGroup(this.List_NowContsGroup[i].trackingId, this.List_NowContsGroup[i].contsList, this.List_NowContsGroup[i].contCenter,this.stackNum) );
                    protectId.Add(this.List_NowContsGroup[i].trackingId);
                }
            }

            //追加がなかったものは削除する
            for (int i = this.Tree_ContsGroup.Count -1; i >= 0; ++i)
            {
                if (!protectId.Contains(this.Tree_ContsGroup[i].trackingId))
                {
                    this.Tree_ContsGroup.RemoveAt(i);
                }
            }

            //描画
            Cv2.CvtColor(m_buffer,m_buffer, OpenCvSharp.ColorConversion.BgrToHsv);

            for (int i = 0; i < this.Tree_ContsGroup.Count ; ++i)
            {
                List<Point>[] m_contour = this.Tree_ContsGroup[i].ToArrey();
                Cv2.DrawContours(m_buffer, m_contour, -1, this.Tree_ContsGroup[i].color, -1, OpenCvSharp.LineType.Link8);

            }

            Cv2.CvtColor(m_buffer, m_buffer, OpenCvSharp.ColorConversion.HsvToBgr);





            dst += m_buffer;
            this.List_Contours_Buffer = this.List_Contours;
            //Cv2.CvtColor(dstMat, dst, OpenCvSharp.ColorConversion.BgraToBgr);

        }













        public class StackContsGroup
        {
            public int? trackingId { get; set; }
            public Scalar color { get; set; }
            int radius { get; set; }
            public OpenCvSharp.CPlusPlus.Point vec { get; set; }
            public float vel { get; set; }
            int frameCount { get; set; }

            public List<List<Point>> contsStack { get; set; }
            public Point contCenter { get; set; }
            private List<List<Point>> buf { get; set; }

            //新しく作るとき　　　※ストック分の数値は全部ゼロにする
            public StackContsGroup(int? id, List<Point> contList, Point centerPt, int stacklim)
            {
                this.trackingId = id;
                this.contCenter = centerPt;
                this.color = new Scalar(UnityEngine.Random.Range(0,180),240,240  );

                for (int a = 0; a < stacklim; ++a)
                {
                this.contsStack.Add(new List<Point>(contList));
                //this.contsStack.Add(contList);
                }
            }

            //新しい値を追加するとき
            public void AddContsGroup(List<Point> contList, Point centerPt,int stacklim)
            {
                this.contCenter = centerPt;
                this.contsStack.Insert(0, new List<Point>( contList));
                if (this.contsStack.Count > stacklim) this.contsStack.RemoveAt(this.contsStack.Count - 1);
            }


            //arreyで返す
            public List<Point>[] ToArrey()
            {
                this.buf.Clear();
                if (this.contsStack.Count > 0)
                {
                this.buf.Add(new List<Point>(this.contsStack[0]));

                return buf.ToArray();

                }

                else
                {
                    return null;
                }
            }

            //１つの輪郭のツリーで返す
            public List<List<Point>> thisContour()
            {
                this.buf.Clear();
                if (this.contsStack.Count > 0)
                {
                    this.buf.Add(new List<Point>(this.contsStack[0]));

                    return buf;
                }

                else
                {
                    return null;
                }
            }
            #region  没

            //public void Move(float speed)
            //{
            //    //this.center += this.vec * this.vel;
            //    if (this.vel < 1) this.vel = 1;
            //    //this.center += new Point (speed * this.vec.X / this.vel, speed *this.vec.Y / this.vel);
            //    if (vel > 80)
            //    {
            //        this.center += new Point(speed * 0.1 * this.vec.X, speed * 0.1 * this.vec.Y);
            //    }
            //    else
            //    {
            //        this.center += new Point(speed * this.vec.X, speed * this.vec.Y);
            //    }
            //}

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

            #endregion
        }

        public class NowContsGroup
        {
            public int? trackingId { get; set; }
            public List<Point> contsList { get; set; }
            public Point contCenter { get; set; }

            public NowContsGroup(int? id, List<Point> contList, Point centerPt)
            {
                this.contCenter = centerPt;
                this.trackingId = id;
                this.contsList = contList;
            }
        }




        public override string ToString()
        {
            return "LeastSquare";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.LeastSquare;
        }

        public bool IsFirstFrame { get; private set; }
    }




}
