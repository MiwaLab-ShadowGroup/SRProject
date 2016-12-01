using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using UnityEngine;
using Windows.Kinect;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class LSAhead : AShadowImageProcesser
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
        Scalar color;
        Scalar colorBack;
        float speedCtl;
        int useFrame;
        int preFrame;


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

        //骨格
        List<BodyBasePair> basePairList = new List<BodyBasePair>();



        public LSAhead() : base()
        {
            (ShadowMediaUIHost.GetUI("LSAhead_con_R") as ParameterSlider).ValueChanged += LSAhead_con_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("LSAhead_con_G") as ParameterSlider).ValueChanged += LSAhead_con_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("LSAhead_con_B") as ParameterSlider).ValueChanged += LSAhead_con_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("LSAhead_bgd_R") as ParameterSlider).ValueChanged += LSAhead_bgd_R_ValueChanged;
            (ShadowMediaUIHost.GetUI("LSAhead_bgd_G") as ParameterSlider).ValueChanged += LSAhead_bgd_G_ValueChanged;
            (ShadowMediaUIHost.GetUI("LSAhead_bgd_B") as ParameterSlider).ValueChanged += LSAhead_bgd_B_ValueChanged;
            (ShadowMediaUIHost.GetUI("LSAhead_Rate") as ParameterSlider).ValueChanged += LSAhead_Rate_ValueChanged;
            (ShadowMediaUIHost.GetUI("LSAhead_useFrame") as ParameterSlider).ValueChanged += LSAhead_useFrame_ValueChanged;
            (ShadowMediaUIHost.GetUI("LSAhead_preFrame") as ParameterSlider).ValueChanged += LSAhead_preFrame_ValueChanged;
            (ShadowMediaUIHost.GetUI("LSAhead_UseFade") as ParameterCheckbox).ValueChanged += LSAhead_UseFade_ValueChanged;


            (ShadowMediaUIHost.GetUI("LSAhead_con_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LSAhead_con_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LSAhead_con_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LSAhead_bgd_R") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LSAhead_bgd_G") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LSAhead_bgd_B") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LSAhead_Rate") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LSAhead_useFrame") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LSAhead_preFrame") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("LSAhead_UseFade") as ParameterCheckbox).ValueUpdate();
        }



        private void LSAhead_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void LSAhead_Rate_ValueChanged(object sender, EventArgs e)
        {
            this.sharpness = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void LSAhead_useFrame_ValueChanged(object sender, EventArgs e)
        {
            this.useFrame = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void LSAhead_preFrame_ValueChanged(object sender, EventArgs e)
        {
            this.preFrame = (int)(e as ParameterSlider.ChangedValue).Value;
        }

        private void LSAhead_con_R_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LSAhead_con_G_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LSAhead_con_B_ValueChanged(object sender, EventArgs e)
        {
            this.color.Val0 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LSAhead_bgd_R_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val2 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LSAhead_bgd_G_ValueChanged(object sender, EventArgs e)
        {
            this.colorBack.Val1 = (double)(e as ParameterSlider.ChangedValue).Value;

        }

        private void LSAhead_bgd_B_ValueChanged(object sender, EventArgs e)
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

                            //this.List_Contours.Add(new List<Point>(CvPoints));
                            this.List_NowContsGroup.Add(new NowContsGroup(null, this.CvPoints, new Point((M.M10 / M.M00), (M.M01 / M.M00))));
                        }
                    }

            }
            var _contour = List_Contours.ToArray();
            //Cv2.DrawContours(m_buffer, _contour, -1, color, -1, OpenCvSharp.LineType.Link8);



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

            //------------------------------------------------------------

            //ここから骨格点分類

            //輪郭の重心位置と比較する骨格のSpineBaseの位置リストを作る
            this.basePairList.Clear();
            for (int i = 0; i < BodyData.Length; i++)
            {
                if (BodyData[i].IsTracked)
                {
                    if (this.CheckBodyInSCreen(i, src))
                    {
                        this.basePairList.Add(new BodyBasePair(i, new Point(BodyDataOnDepthImage[i].JointDepth[JointType.SpineBase].position.x, BodyDataOnDepthImage[i].JointDepth[JointType.SpineBase].position.y)) );
                    }
                }
            }
            



            //アクティブな骨格が輪郭の数より少ないとき　　少ない方から調べていく
            if (this.basePairList.Count <= this.Tree_ContsGroup.Count)
            {
                for (int i = 0; i < this.basePairList.Count; ++i)
                {
                    this.dist = 1000;
                    this.minDist = 1000; //とりあえず笑　その場しのぎ
                    this.bufferNum = 0;
                    for (int j = 0; j < this.Tree_ContsGroup.Count; ++j)
                    {

                        dist = Point.Distance(this.basePairList[i].basePoint, this.Tree_ContsGroup[j].contCenter);

                        if (dist < minDist)
                        {
                            bufferNum = j;
                            minDist = dist;
                        }

                        //骨格情報をツリーに入れる
                        for (int k = 0; k < _TrackigBoneList.Count; ++k)
                        {
                            if (this.Tree_ContsGroup[bufferNum].boneStack.ContainsKey(_TrackigBoneList[k]))
                            {
                                //トラッキングしていたらPointをしてなかったらnullを代入
                                if (BodyData[this.basePairList[i].bodyNumber].Joints[_TrackigBoneList[k]].TrackingState == Windows.Kinect.TrackingState.Tracked)
                                {
                                this.Tree_ContsGroup[bufferNum].boneStack[_TrackigBoneList[k]].Insert(0, new Point(BodyDataOnDepthImage[this.basePairList[i].bodyNumber].JointDepth[_TrackigBoneList[k]].position.x, BodyDataOnDepthImage[this.basePairList[i].bodyNumber].JointDepth[_TrackigBoneList[k]].position.y));
                                }
                                else
                                {
                                    this.Tree_ContsGroup[bufferNum].boneStack[_TrackigBoneList[k]].Insert(0, null);
                                }

                                //個数がオーバーしていたら消す
                                if (this.Tree_ContsGroup[bufferNum].boneStack[_TrackigBoneList[k]].Count > this.useFrame) this.Tree_ContsGroup[bufferNum].boneStack[_TrackigBoneList[k]].RemoveAt(this.Tree_ContsGroup[bufferNum].boneStack[_TrackigBoneList[k]].Count-1);


                            }
                            //リストがなかったらとりあえず埋める
                            else
                            {
                                for (int l = 0; l < this.stackNum; ++l)
                                {
                                    this.Tree_ContsGroup[bufferNum].boneStack[_TrackigBoneList[k]].Add(new Point(BodyDataOnDepthImage[this.basePairList[i].bodyNumber].JointDepth[_TrackigBoneList[k]].position.x, BodyDataOnDepthImage[this.basePairList[i].bodyNumber].JointDepth[_TrackigBoneList[k]].position.y));
                                }

                            }
                        }
                    }
                }
            }
            //アクティブな骨格が輪郭の数より多いとき
            else
            {
                for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
                {
                    this.dist = 1000;
                    this.minDist = 1000; //とりあえず笑　その場しのぎ
                    this.bufferNum = 0;
                    for (int j = 0; j < this.basePairList.Count; ++j)
                    {
                        dist = Point.Distance(this.Tree_ContsGroup[i].contCenter, this.basePairList[j].basePoint);
                        if (dist < minDist)
                        {
                            bufferNum = j;
                            minDist = dist;
                        }
                    }


                    //骨格情報をツリーに入れる
                    for (int k = 0; k < _TrackigBoneList.Count; ++k)
                    {
                        if (this.Tree_ContsGroup[i].boneStack.ContainsKey(_TrackigBoneList[k]))
                        {
                            //トラッキングしていたらPointをしてなかったらnullを代入
                            if (BodyData[this.basePairList[bufferNum].bodyNumber].Joints[_TrackigBoneList[k]].TrackingState == Windows.Kinect.TrackingState.Tracked)
                            {
                                this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].Insert(0, new Point(BodyDataOnDepthImage[this.basePairList[bufferNum].bodyNumber].JointDepth[_TrackigBoneList[k]].position.x, BodyDataOnDepthImage[this.basePairList[bufferNum].bodyNumber].JointDepth[_TrackigBoneList[k]].position.y));
                            }
                            else
                            {
                                this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].Insert(0, null);
                            }

                            //個数がオーバーしていたら消す
                            if (this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].Count > this.useFrame) this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].RemoveAt(this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].Count - 1);


                        }
                        //リストがなかったらとりあえず埋める
                        else
                        {
                            for (int l = 0; l < this.stackNum; ++l)
                            {
                                this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].Add(new Point(BodyDataOnDepthImage[this.basePairList[bufferNum].bodyNumber].JointDepth[_TrackigBoneList[k]].position.x, BodyDataOnDepthImage[this.basePairList[bufferNum].bodyNumber].JointDepth[_TrackigBoneList[k]].position.y));
                            }
                        }
                    }
                }
            }

            //骨格情報ツリー完成































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
            Cv2.CvtColor(m_buffer, m_buffer, OpenCvSharp.ColorConversion.BgrToHsv);

            for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
            {
                List<Point>[] m_contour = this.Tree_ContsGroup[i].ToArrey();
                //List<Point>[] m_preContour = SquarePredictCont(this.Tree_ContsGroup[i],5,5);
                List<Point>[] m_preContour = this.Tree_ContsGroup[i].MoveContour(SquarePredictCenterMove(this.Tree_ContsGroup[i], this.useFrame, this.preFrame)).ToArray();
                Cv2.DrawContours(m_buffer, m_contour, -1, this.Tree_ContsGroup[i].color, -1, OpenCvSharp.LineType.Link8);




                //Hをずらす　　　Sを下げて明るくするのはだめ
                Cv2.DrawContours(m_buffer, m_preContour, -1, new Scalar(this.Tree_ContsGroup[i].color.Val1 + 20, this.Tree_ContsGroup[i].color.Val1, this.Tree_ContsGroup[i].color.Val2), -1, OpenCvSharp.LineType.Link8);

                //Cv2.Circle(m_buffer, this.Tree_ContsGroup[i].contsStack[0][0], 3, new Scalar(255, 255, 255));
                //Cv2.Circle(m_buffer, SquarePredict(this.Tree_ContsGroup[i],6,3,0) , 3, new Scalar(120, 240, 240));

            }

            Cv2.CvtColor(m_buffer, m_buffer, OpenCvSharp.ColorConversion.HsvToBgr);













            dst += m_buffer;
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
            public List<Point> contCenterList { get; set; }
            private List<List<Point>> buf { get; set; }

            public Dictionary<JointType, List<Point?>> boneStack { get; set; }


            //新しく作るとき　　　※ストック分の数値は全部ゼロにする
            public StackContsGroup(int? id, List<Point> contList, Point centerPt, int stacklim)
            {
                this.trackingId = id;
                this.contCenter = centerPt;
                this.color = new Scalar(UnityEngine.Random.Range(0, 180), 240, 240);
                this.contsStack = new List<List<Point>>();
                this.contCenterList = new List<Point>();
                this.buf = new List<List<Point>>();
                this.boneStack = new Dictionary<JointType, List<Point?>>();

                for (int a = 0; a < stacklim; ++a)
                {
                    this.contsStack.Add(new List<Point>(contList));
                    this.contCenterList.Add(centerPt);
                    //this.contsStack.Add(contList);
                }



            }

            //新しい値を追加するとき
            public void AddContsGroup(List<Point> contList, Point centerPt, int stacklim)
            {
                this.contCenter = centerPt;
                this.contsStack.Insert(0, new List<Point>(contList));
                if (this.contsStack.Count > stacklim) this.contsStack.RemoveAt(this.contsStack.Count - 1);
                this.contCenterList.Insert(0, centerPt);
                if (this.contCenterList.Count > stacklim) this.contCenterList.RemoveAt(this.contCenterList.Count - 1);
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

            //動かした輪郭の一つのツリーを返す
            public List<List<Point>> MoveContour(Point moveDistance)
            {
                this.buf.Clear();
                List<Point> buflist = new List<Point>();
                if (this.contsStack.Count > 0)
                {
                    for (int a = 0; a < this.contsStack[0].Count; ++a)
                    {
                        buflist.Add(this.contsStack[0][a] + moveDistance);
                    }
                    this.buf.Add(new List<Point>(buflist));

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

       

        public class BodyBasePair
        {
            public int bodyNumber { get; set; }
            public Point basePoint { get; set; }

            public BodyBasePair(int bdNum,  Point basePt)
            {
                this.bodyNumber = bdNum;
                this.basePoint = basePt;
            }
        }



        public Point SquarePredict(StackContsGroup stack, int useFeameNum, int preFrameNum, int predictContNum)
        {
            Point predictPt;

            //List<double> t1 = new List<double>();
            List<double> t2 = new List<double>();
            List<double> t3 = new List<double>();
            List<double> t4 = new List<double>();

            List<double> t0x = new List<double>();
            List<double> t1x = new List<double>();
            List<double> t2x = new List<double>();

            List<double> t0y = new List<double>();
            List<double> t1y = new List<double>();
            List<double> t2y = new List<double>();

            double sumT1 = 0;
            double sumT2 = 0;
            double sumT3 = 0;
            double sumT4 = 0;

            double sumT0X = 0;
            double sumT1X = 0;
            double sumT2X = 0;

            double sumT0Y = 0;
            double sumT1Y = 0;
            double sumT2Y = 0;

            Matrix4x4 TMat = Matrix4x4.identity;
            Matrix4x4 invTMat = Matrix4x4.identity;
            Matrix4x4 XMat = Matrix4x4.zero;
            Matrix4x4 YMat = Matrix4x4.zero;
            Matrix4x4 ansXMat = Matrix4x4.zero;
            Matrix4x4 ansYMat = Matrix4x4.zero;

            //Xから
            //各数値リストの作成
            //計算するフレームはもっと少なくてもいいかも
            for (int i = 0; i < this.t1.Count; ++i)
            {
                t2.Add(Mathf.Pow((float)this.t1[i], 2));
                t3.Add(Mathf.Pow((float)this.t1[i], 3));
                t4.Add(Mathf.Pow((float)this.t1[i], 4));
                if (i < stack.contsStack.Count)
                {
                    t0x.Add(stack.contsStack[i][predictContNum].X);
                    t1x.Add(this.t1[i] * stack.contsStack[i][predictContNum].X);
                    t2x.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contsStack[i][predictContNum].X);

                    t0y.Add(stack.contsStack[i][predictContNum].Y);
                    t1y.Add(this.t1[i] * stack.contsStack[i][predictContNum].Y);
                    t2y.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contsStack[i][predictContNum].Y);
                }
                else
                {
                    t0x.Add(stack.contsStack[stack.contsStack.Count - 1][predictContNum].X);
                    t1x.Add(this.t1[i] * stack.contsStack[stack.contsStack.Count - 1][predictContNum].X);
                    t2x.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contsStack[stack.contsStack.Count - 1][predictContNum].X);

                    t0y.Add(stack.contsStack[stack.contsStack.Count - 1][predictContNum].Y);
                    t1y.Add(this.t1[i] * stack.contsStack[stack.contsStack.Count - 1][predictContNum].Y);
                    t2y.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contsStack[stack.contsStack.Count - 1][predictContNum].Y);
                }
            }

            //各リストの累計を作る
            for (int i = 0; i < useFeameNum; ++i)
            {
                sumT1 += this.t1[i];
                sumT2 += t2[i];
                sumT3 += t3[i];
                sumT4 += t4[i];

                sumT0X += t0x[i];
                sumT1X += t1x[i];
                sumT2X += t2x[i];

                sumT0Y += t0y[i];
                sumT1Y += t1y[i];
                sumT2Y += t2y[i];
            }

            //行列を作る
            TMat.m00 = useFeameNum;
            TMat.m01 = (float)sumT1;
            TMat.m02 = (float)sumT2;
            TMat.m03 = 0;

            TMat.m10 = (float)sumT1;
            TMat.m11 = (float)sumT2;
            TMat.m12 = (float)sumT3;
            TMat.m13 = 0;

            TMat.m20 = (float)sumT2;
            TMat.m21 = (float)sumT3;
            TMat.m22 = (float)sumT4;
            TMat.m23 = 0;

            TMat.m30 = 0;
            TMat.m31 = 0;
            TMat.m32 = 0;
            TMat.m33 = 1; //逆行列が存在するためにはここが１じゃないとだめ

            XMat.m00 = (float)sumT0X;
            XMat.m10 = (float)sumT1X;
            XMat.m20 = (float)sumT2X;

            YMat.m00 = (float)sumT0Y;
            YMat.m10 = (float)sumT1Y;
            YMat.m20 = (float)sumT2Y;

            //行列の計算
            ansXMat = TMat.inverse * XMat;
            ansYMat = TMat.inverse * YMat;

            //座標の計算
            predictPt.X = (int)(ansXMat.m00 + ansXMat.m10 * preFrameNum * 0.016 + ansXMat.m20 * preFrameNum * preFrameNum * 0.016f * 0.016f);
            predictPt.Y = (int)(ansYMat.m00 + ansYMat.m10 * preFrameNum * 0.016 + ansYMat.m20 * preFrameNum * preFrameNum * 0.016f * 0.016f);

            return predictPt;
        }

        //センターポイントの推移を見る
        public Point SquarePredictCenterMove(StackContsGroup stack, int useFeameNum, int preFrameNum)
        {
            Point predictPt;

            //List<double> t1 = new List<double>();
            List<double> t2 = new List<double>();
            List<double> t3 = new List<double>();
            List<double> t4 = new List<double>();

            List<double> t0x = new List<double>();
            List<double> t1x = new List<double>();
            List<double> t2x = new List<double>();

            List<double> t0y = new List<double>();
            List<double> t1y = new List<double>();
            List<double> t2y = new List<double>();

            double sumT1 = 0;
            double sumT2 = 0;
            double sumT3 = 0;
            double sumT4 = 0;

            double sumT0X = 0;
            double sumT1X = 0;
            double sumT2X = 0;

            double sumT0Y = 0;
            double sumT1Y = 0;
            double sumT2Y = 0;

            Matrix4x4 TMat = Matrix4x4.identity;
            Matrix4x4 invTMat = Matrix4x4.identity;
            Matrix4x4 XMat = Matrix4x4.zero;
            Matrix4x4 YMat = Matrix4x4.zero;
            Matrix4x4 ansXMat = Matrix4x4.zero;
            Matrix4x4 ansYMat = Matrix4x4.zero;

            //Xから
            //各数値リストの作成
            //計算するフレームはもっと少なくてもいいかも
            for (int i = 0; i < this.t1.Count; ++i)
            {
                t2.Add(Mathf.Pow((float)this.t1[i], 2));
                t3.Add(Mathf.Pow((float)this.t1[i], 3));
                t4.Add(Mathf.Pow((float)this.t1[i], 4));
                if (i < stack.contsStack.Count)
                {
                    t0x.Add(stack.contCenterList[i].X);
                    t1x.Add(this.t1[i] * stack.contCenterList[i].X);
                    t2x.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contCenterList[i].X);

                    t0y.Add(stack.contCenterList[i].Y);
                    t1y.Add(this.t1[i] * stack.contCenterList[i].Y);
                    t2y.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contCenterList[i].Y);
                }
                else
                {
                    t0x.Add(stack.contCenterList[stack.contCenterList.Count - 1].X);
                    t1x.Add(this.t1[i] * stack.contCenterList[stack.contCenterList.Count - 1].X);
                    t2x.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contCenterList[stack.contCenterList.Count - 1].X);

                    t0y.Add(stack.contCenterList[stack.contCenterList.Count - 1].Y);
                    t1y.Add(this.t1[i] * stack.contCenterList[stack.contCenterList.Count - 1].Y);
                    t2y.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contCenterList[stack.contCenterList.Count - 1].Y);
                }
            }

            //各リストの累計を作る
            for (int i = 0; i < useFeameNum; ++i)
            {
                sumT1 += this.t1[i];
                sumT2 += t2[i];
                sumT3 += t3[i];
                sumT4 += t4[i];

                sumT0X += t0x[i];
                sumT1X += t1x[i];
                sumT2X += t2x[i];

                sumT0Y += t0y[i];
                sumT1Y += t1y[i];
                sumT2Y += t2y[i];
            }

            //行列を作る
            TMat.m00 = useFeameNum;
            TMat.m01 = (float)sumT1;
            TMat.m02 = (float)sumT2;
            TMat.m03 = 0;

            TMat.m10 = (float)sumT1;
            TMat.m11 = (float)sumT2;
            TMat.m12 = (float)sumT3;
            TMat.m13 = 0;

            TMat.m20 = (float)sumT2;
            TMat.m21 = (float)sumT3;
            TMat.m22 = (float)sumT4;
            TMat.m23 = 0;

            TMat.m30 = 0;
            TMat.m31 = 0;
            TMat.m32 = 0;
            TMat.m33 = 1; //逆行列が存在するためにはここが１じゃないとだめ

            XMat.m00 = (float)sumT0X;
            XMat.m10 = (float)sumT1X;
            XMat.m20 = (float)sumT2X;

            YMat.m00 = (float)sumT0Y;
            YMat.m10 = (float)sumT1Y;
            YMat.m20 = (float)sumT2Y;

            //行列の計算
            ansXMat = TMat.inverse * XMat;
            ansYMat = TMat.inverse * YMat;

            //座標の計算
            predictPt.X = (int)(ansXMat.m00 + ansXMat.m10 * preFrameNum * 0.016 + ansXMat.m20 * preFrameNum * preFrameNum * 0.016f * 0.016f);
            predictPt.Y = (int)(ansYMat.m00 + ansYMat.m10 * preFrameNum * 0.016 + ansYMat.m20 * preFrameNum * preFrameNum * 0.016f * 0.016f);

            return predictPt - stack.contCenter;
        }



        public List<Point>[] SquarePredictCont(StackContsGroup stack, int useFeameNum, int preFrameNum)
        {
            List<List<Point>> predictList = new List<List<Point>>();
            List<Point> predictCont = new List<Point>();
            Point predictPt;

            //List<double> t1 = new List<double>();
            List<double> t2 = new List<double>();
            List<double> t3 = new List<double>();
            List<double> t4 = new List<double>();

            List<double> t0x = new List<double>();
            List<double> t1x = new List<double>();
            List<double> t2x = new List<double>();

            List<double> t0y = new List<double>();
            List<double> t1y = new List<double>();
            List<double> t2y = new List<double>();

            double sumT1 = 0;
            double sumT2 = 0;
            double sumT3 = 0;
            double sumT4 = 0;

            double sumT0X = 0;
            double sumT1X = 0;
            double sumT2X = 0;

            double sumT0Y = 0;
            double sumT1Y = 0;
            double sumT2Y = 0;

            Matrix4x4 TMat = Matrix4x4.identity;
            Matrix4x4 XMat = Matrix4x4.zero;
            Matrix4x4 YMat = Matrix4x4.zero;
            Matrix4x4 ansXMat = Matrix4x4.zero;
            Matrix4x4 ansYMat = Matrix4x4.zero;

            //Xから
            //各数値リストの作成
            //計算するフレームはもっと少なくてもいいかも

            
            for (int h = 0; h < stack.contsStack[0].Count; ++h)
            {
                t2.Clear();
                t3.Clear();
                t4.Clear();

                t0x.Clear();
                t1x.Clear();
                t2x.Clear();
                t0y.Clear();

                t1y.Clear();
                t2y.Clear();

                sumT1 = 0;
                sumT2 = 0;
                sumT3 = 0;
                sumT4 = 0;

                sumT0X = 0;
                sumT1X = 0;
                sumT2X = 0;

                sumT0Y = 0;
                sumT1Y = 0;
                sumT2Y = 0;


                for (int i = 0; i < this.t1.Count; ++i)
                {
                    t2.Add(Mathf.Pow((float)this.t1[i], 2));
                    t3.Add(Mathf.Pow((float)this.t1[i], 3));
                    t4.Add(Mathf.Pow((float)this.t1[i], 4));
                    if (i < stack.contsStack.Count)
                    {
                        t0x.Add(stack.contsStack[i][h].X);
                        t1x.Add(this.t1[i] * stack.contsStack[i][h].X);
                        t2x.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contsStack[i][h].X);

                        t0y.Add(stack.contsStack[i][h].Y);
                        t1y.Add(this.t1[i] * stack.contsStack[i][h].Y);
                        t2y.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contsStack[i][h].Y);
                    }
                    else
                    {
                        t0x.Add(stack.contsStack[stack.contsStack.Count - 1][h].X);
                        t1x.Add(this.t1[i] * stack.contsStack[stack.contsStack.Count - 1][h].X);
                        t2x.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contsStack[stack.contsStack.Count - 1][h].X);

                        t0y.Add(stack.contsStack[stack.contsStack.Count - 1][h].Y);
                        t1y.Add(this.t1[i] * stack.contsStack[stack.contsStack.Count - 1][h].Y);
                        t2y.Add(Mathf.Pow((float)this.t1[i], 2) * stack.contsStack[stack.contsStack.Count - 1][h].Y);
                    }
                }

                //各リストの累計を作る
                for (int i = 0; i < useFeameNum; ++i)
                {
                    sumT1 += this.t1[i];
                    sumT2 += t2[i];
                    sumT3 += t3[i];
                    sumT4 += t4[i];

                    sumT0X += t0x[i];
                    sumT1X += t1x[i];
                    sumT2X += t2x[i];

                    sumT0Y += t0y[i];
                    sumT1Y += t1y[i];
                    sumT2Y += t2y[i];
                }

                //行列を作る
                TMat.m00 = useFeameNum;
                TMat.m01 = (float)sumT1;
                TMat.m02 = (float)sumT2;
                TMat.m03 = 0;

                TMat.m10 = (float)sumT1;
                TMat.m11 = (float)sumT2;
                TMat.m12 = (float)sumT3;
                TMat.m13 = 0;

                TMat.m20 = (float)sumT2;
                TMat.m21 = (float)sumT3;
                TMat.m22 = (float)sumT4;
                TMat.m23 = 0;

                TMat.m30 = 0;
                TMat.m31 = 0;
                TMat.m32 = 0;
                TMat.m33 = 1; //逆行列が存在するためにはここが１じゃないとだめ

                XMat.m00 = (float)sumT0X;
                XMat.m10 = (float)sumT1X;
                XMat.m20 = (float)sumT2X;

                YMat.m00 = (float)sumT0Y;
                YMat.m10 = (float)sumT1Y;
                YMat.m20 = (float)sumT2Y;

                //行列の計算
                ansXMat = TMat.inverse * XMat;
                ansYMat = TMat.inverse * YMat;

                //座標の計算
                predictPt.X = (int)(ansXMat.m00 + ansXMat.m10 * preFrameNum * 0.016 + ansXMat.m20 * preFrameNum * preFrameNum * 0.016f * 0.016f);
                predictPt.Y = (int)(ansYMat.m00 + ansYMat.m10 * preFrameNum * 0.016 + ansYMat.m20 * preFrameNum * preFrameNum * 0.016f * 0.016f);

                predictCont.Add(predictPt);
            }
            predictList.Add(predictCont);

            return predictList.ToArray();
        }

        bool CheckBodyInSCreen(int bodyNum, Mat srcMat)
        {
            bool bl = true;
            for (int i = 0; i < this._TrackigBoneList.Count; ++i)
            {
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x > srcMat.Width) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x > srcMat.Width) Debug.Log("OverNum : "  + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x); 
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x < 0) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x < 0 ) Debug.Log("OverNum : " + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x);
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y > srcMat.Height) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y > srcMat.Height) Debug.Log("OverNum : " + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y);
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y < 0) bl = false;
                //if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y < 0) Debug.Log("OverNum : " + this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y);
                if (double.IsInfinity(this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.x))
                {
                    bl = false;
                }
                if (double.IsInfinity(this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y))
                {
                    bl = false;
                }
            }
            return bl;
        }

        //posを計測するボーンのリスト　体が画面内にあるのかどうかを判断するときに使用
        public Dictionary<int, Windows.Kinect.JointType> _TrackigBoneList = new Dictionary<int, Windows.Kinect.JointType>()
        {
            { 0,Windows.Kinect.JointType.AnkleLeft },
            { 1,Windows.Kinect.JointType.KneeLeft},

            { 2,Windows.Kinect.JointType.AnkleRight },
            { 3,Windows.Kinect.JointType.KneeRight  },

            { 4,Windows.Kinect.JointType.WristLeft   },
            { 5,Windows.Kinect.JointType.ElbowLeft    },

            { 6,Windows.Kinect.JointType.WristRight },
            { 7,Windows.Kinect.JointType.ElbowRight },
            { 8,Windows.Kinect.JointType.SpineBase},
            { 9,Windows.Kinect.JointType.Head},
            { 10,Windows.Kinect.JointType.ShoulderLeft},
            { 11,Windows.Kinect.JointType.ShoulderRight},
            { 12,Windows.Kinect.JointType.SpineMid},
            { 13,Windows.Kinect.JointType.Neck},
        };







        public override string ToString()
        {
            return "LSAhead";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.LSAhead;
        }

        public bool IsFirstFrame { get; private set; }
    }




}