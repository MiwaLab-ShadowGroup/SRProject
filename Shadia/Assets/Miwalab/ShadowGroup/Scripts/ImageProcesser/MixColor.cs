using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class MixColor : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 20;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Goast = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;
        float speedCtl;
        int mixRate;

        List<OpenCvSharp.CPlusPlus.Point> contour_Center = new List<Point>();
        List<StackContsGroup> Tree_ContsGroup = new List<StackContsGroup>();
        List<NowContsGroup> List_NowContsGroup = new List<NowContsGroup>();
        int stackNum = 20;


        int numberingId = 0;
        List<int?> activeIdList = new List<int?>();
        float thresholdDist = 200;

        double dist = 0;
        double minDist = 1000; //とりあえず笑　その場しのぎ
        int bufferNum = 0;

        List<int?> protectId = new List<int?>();
        int? bufNum = null;
        bool existNum = false;
        int useContNum;

        //List<double> timeTable = new List<double>();
        List<double> t1 = new List<double>();

        //ここからカラー系
        List<Mat> personalCanvas = new List<Mat>();
        Mat canvas ;
        Mat stayCanvas ;
        Mat cont_mask ;
        Mat m_mask;
        Mat addCanvas ;
        Mat blendCanvas;

        public MixColor() : base()
        {
            (ShadowMediaUIHost.GetUI("MixColor_con_R") as ParameterSlider).ValueChanged += MixColor_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("MixColor_con_G") as ParameterSlider).ValueChanged += MixColor_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("MixColor_con_B") as ParameterSlider).ValueChanged += MixColor_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("MixColor_bgd_R") as ParameterSlider).ValueChanged += MixColor_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("MixColor_bgd_G") as ParameterSlider).ValueChanged += MixColor_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("MixColor_bgd_B") as ParameterSlider).ValueChanged += MixColor_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("MixColor_Rate") as ParameterSlider).ValueChanged += MixColor_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("MixColor_Speed") as ParameterSlider).ValueChanged += MixColor_Speed_ValueChanged;
            (ShadowMediaUIHost.GetUI("MixColor_Mix") as ParameterSlider).ValueChanged += MixColor_Mix_ValueChanged;
            (ShadowMediaUIHost.GetUI("MixColor_UseFade") as ParameterCheckbox).ValueChanged += MixColor_UseFade_ValueChanged;

     

            (ShadowMediaUIHost.GetUI("MixColor_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("MixColor_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("MixColor_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("MixColor_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("MixColor_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("MixColor_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("MixColor_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("MixColor_Speed") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("MixColor_Mix") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("MixColor_UseFade") as ParameterCheckbox).ValueUpdate();
        }

  

        private void MixColor_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void MixColor_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void MixColor_Speed_ValueChanged(object sender, EventArgs e)
        {
            this.speedCtl = (float)(e as ParameterSlider.ChangedValue).Value;
        }

        private void MixColor_Mix_ValueChanged(object sender, EventArgs e)
        {
            this.mixRate = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void MixColor_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void MixColor_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void MixColor_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void MixColor_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void MixColor_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void MixColor_bgd_B_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        Mat m_buffer;
        bool m_UseFade;
        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();
            this.contour_Center.Clear();
            this.List_NowContsGroup.Clear();

            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                canvas = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                cont_mask = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                stayCanvas = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                m_mask = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                addCanvas = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
                blendCanvas = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
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
                cont_mask *= 0;
            }



            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.Erode(grayimage, grayimage, new Mat(), null, 1);
            //Cv2.MedianBlur(grayimage, grayimage, 9);
            //Cv2.Dilate(grayimage, grayimage, new Mat(), null, 1);

            Cv2.GaussianBlur(grayimage, grayimage, new Size(3, 3), 0f);

            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,colorBack);
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);
           

            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);


            //とりあえず輪郭のリストを作る
            for (int i = 0; i < contour.Length; i++)
            {

                this.CvPoints.Clear();
                if (Cv2.ContourArea(contour[i]) > 1000)
                    if (contour[i].Length > this.sharpness)
                    {
                        {
                            //重心検出処理
                            var cont = contour[i].ToArray();

                            var M = Cv2.Moments(cont);
                            this.contour_Center.Add(new Point((M.M10 / M.M00), (M.M01 / M.M00)));


                            //for (int j = 0; j < contour[i].Length; j += (int)( contour[i].Length / this.sharpness + 1))
                            for (int j = 0; j < this.sharpness; j++)
                            {
                                //this.useContNum = j * contour[i].Length / this.sharpness;
                                this.CvPoints.Add(contour[i][j * contour[i].Length / this.sharpness]);
                            }
                            //Debug.Log("cvPoints Num ; " + this.CvPoints.Count);

                            this.List_Contours.Add(new List<Point>(CvPoints));
                            this.List_NowContsGroup.Add(new NowContsGroup(null, this.CvPoints, new Point((M.M10 / M.M00), (M.M01 / M.M00))));
                        }
                    }

            }
            var _contour = List_Contours.ToArray();
            Cv2.DrawContours(cont_mask, _contour, -1, new Scalar(255,255,255), -1, OpenCvSharp.LineType.Link8);



            //現在の輪郭すべてにIDを振る


            this.activeIdList.Clear();


            //Debug.Log("this.Tree_ContsGroup.Count ; " + this.Tree_ContsGroup.Count);
            //前のフレームの輪郭がゼロのときのことを考えていない？
            //ワンフレーム前から輪郭数が減っている時 同じ場合も含む
            if (this.List_NowContsGroup.Count <= this.Tree_ContsGroup.Count)
            {
                for (int i = 0; i < this.List_NowContsGroup.Count; ++i)
                {
                    this.dist = 1000;
                    this.minDist = 1000; //とりあえず笑　その場しのぎ
                    this.bufferNum = 0;
                    for (int j = 0; j < this.Tree_ContsGroup.Count; ++j)
                    {

                        dist = Point.Distance(this.List_NowContsGroup[i].contCenter, this.Tree_ContsGroup[j].contCenter);
                        //Debug.Log("this.List_NowContsGroup[i].contCenter ; " + this.List_NowContsGroup[i].contCenter);
                        //Debug.Log("this.Tree_ContsGroup[j].contCenter ; " + this.Tree_ContsGroup[j].contCenter);

                        if (dist < minDist)
                        {
                            bufferNum = j;
                            minDist = dist;
                        }
                    }
                    //Debug.Log("minDist ; " + this.minDist);

                    //輪郭が離れすぎていたら新しいものとして分類
                    if (this.minDist > this.thresholdDist)
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
                    this.dist = 1000;
                    this.minDist = 1000; //とりあえず笑　その場しのぎ
                    this.bufferNum = 0;
                    for (int j = 0; j < this.List_NowContsGroup.Count; ++j)
                    {
                        dist = Point.Distance(this.Tree_ContsGroup[i].contCenter, this.List_NowContsGroup[j].contCenter);
                        if (dist < minDist)
                        {
                            bufferNum = j;
                            minDist = dist;
                        }
                    }
                    //Debug.Log("minDist ; " + this.minDist);

                    //輪郭が離れすぎていたら新しいものとして分類
                    if (this.minDist > this.thresholdDist)
                    {
                        this.List_NowContsGroup[bufferNum].trackingId = null;
                    }
                    else
                    {
                        this.List_NowContsGroup[bufferNum].trackingId = this.Tree_ContsGroup[i].trackingId;

                    }
                }
            }

            //IDを整理する  //前フレームがゼロの場合もここに直で来る
            for (int i = 0; i < this.List_NowContsGroup.Count; ++i)
            {
                //増えた輪郭に新しいIDを振る
                if (this.List_NowContsGroup[i].trackingId == null)
                {
                    this.numberingId++;
                    this.List_NowContsGroup[i].trackingId = this.numberingId;
                    //Debug.Log("this.List_NowContsGroup[i].trackingId ; " + this.List_NowContsGroup[i].trackingId);

                }

                //重複のチェック
                if (this.activeIdList.Contains(this.List_NowContsGroup[i].trackingId))
                {
                    this.numberingId++;
                    this.List_NowContsGroup[i].trackingId = this.numberingId;
                }
                this.activeIdList.Add(this.List_NowContsGroup[i].trackingId);
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

                        this.Tree_ContsGroup[j].AddContsGroup(this.List_NowContsGroup[i].contsList, this.List_NowContsGroup[i].contCenter, this.stackNum);
                        protectId.Add(this.List_NowContsGroup[i].trackingId);
                    }
                }
                //新しくできたものなら新しいリストを作る
                if (existNum == false)
                {

                    this.Tree_ContsGroup.Add(new StackContsGroup(this.List_NowContsGroup[i].trackingId, this.List_NowContsGroup[i].contsList, this.List_NowContsGroup[i].contCenter, this.stackNum));
                    protectId.Add(this.List_NowContsGroup[i].trackingId);
                }
            }

            //追加がなかったものは削除する
            for (int i = this.Tree_ContsGroup.Count - 1; i >= 0; --i)
            {
                if (!protectId.Contains(this.Tree_ContsGroup[i].trackingId))
                {
                    this.Tree_ContsGroup.RemoveAt(i);
                }
            }
            //輪郭追跡終了


            //ここから最小二乗法

            //タイムテーブルを作成　マイナス方向に作る //最初はとりあえず埋めとく
            if (this.t1.Count == 0)
            {
                for (int i = 0; i < this.stackNum; ++i)
                {
                    this.t1.Add(-Time.deltaTime);
                }
            }
            for (int i = 0; i < this.t1.Count; ++i)
            {
                this.t1[i] -= Time.deltaTime;
            }
            this.t1.Insert(0, 0);
            if (this.t1.Count > this.stackNum) this.t1.RemoveAt(this.t1.Count - 1);













            //描画
            this.personalCanvas.Clear();
            m_mask *= 0;
            addCanvas *= 0;
            blendCanvas *=0;


            for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
            {
                canvas *= 0;
                Cv2.CvtColor(canvas,canvas, OpenCvSharp.ColorConversion.BgrToHsv);

                List<Point>[] m_contour = this.Tree_ContsGroup[i].ToArrey();
                Cv2.DrawContours(canvas, m_contour, -1, this.Tree_ContsGroup[i].color, -1, OpenCvSharp.LineType.Link8);

                Cv2.CvtColor(canvas, canvas, OpenCvSharp.ColorConversion.HsvToBgr);

                Cv2.Dilate(canvas,canvas, new Mat(), null, mixRate);

                this.personalCanvas.Add(canvas);
            }

            //キャンバスごとの処理
            

            //重なった部分が白いマスクを作る
            for (int i = 0; i < this.personalCanvas.Count - 1; ++i)
            {
                for (int j = i + 1; j < this.personalCanvas.Count; ++j)
                {
                    //canvasBuf *= 0;
                    //canvasBuf = this.personalCanvas[i].Mul(this.personalCanvas[j]);
                    //m_mask += canvasBuf;

                    m_mask += this.personalCanvas[i].Mul(this.personalCanvas[j]);
                }
            }
            Cv2.CvtColor(m_mask, m_mask, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.Threshold(m_mask, m_mask, 1, 255, OpenCvSharp.ThresholdType.Binary);
            Cv2.CvtColor(m_mask, m_mask, OpenCvSharp.ColorConversion.GrayToBgr);


            addCanvas *= 0;
            for (int i = 0; i < this.personalCanvas.Count; ++i)
            {
                addCanvas += this.personalCanvas[i];
                Cv2.AddWeighted(blendCanvas, 1, this.personalCanvas[i], 0.5f, 0, blendCanvas);
            }

            addCanvas -= m_mask;
            blendCanvas -= ~m_mask;

            //ここで現在の一ページを完成させる
            //この場合重複部分は平均だが、輪郭はきれいに切っていない。膨張させっぱなし
            addCanvas += blendCanvas;
            Cv2.GaussianBlur(addCanvas,addCanvas, new Size(9, 9), 0f);

            //今までの積み重ねと合成する
            Cv2.AddWeighted(stayCanvas, 1 - speedCtl, addCanvas, speedCtl, 0, stayCanvas);

            m_buffer +=  stayCanvas;
            m_buffer -= ~cont_mask;

            Cv2.GaussianBlur(m_buffer, m_buffer, new Size(5, 5), 0f);


            dst += m_buffer;

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
                this.color = new Scalar(UnityEngine.Random.Range(0, 180), 240, 240);
                this.contsStack = new List<List<Point>>();
                this.buf = new List<List<Point>>();

                for (int a = 0; a < stacklim; ++a)
                {
                    this.contsStack.Add(new List<Point>(contList));
                    //this.contsStack.Add(contList);
                }



            }

            //新しい値を追加するとき
            public void AddContsGroup(List<Point> contList, Point centerPt, int stacklim)
            {
                this.contCenter = centerPt;
                this.contsStack.Insert(0, new List<Point>(contList));
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
                this.contsList = new List<Point>(contList);
                //this.contsList = contList;
            }
        }

      


        public override string ToString()
        {
            return "MixColor";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.MixColor;
        }

        public bool IsFirstFrame { get; private set; }
    }




}
