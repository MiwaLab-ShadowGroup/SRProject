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
        int stackNum = 50;


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

        //リストがないときに代入するとりあえずのリスト
        List<Point?> bonePt_buf = new List<Point?>();
        List<float?> boneRad_buf = new List<float?>();

        bool useCubeCurve;
        float degbuf;



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
            (ShadowMediaUIHost.GetUI("LSAhead_useCubeCurve") as ParameterCheckbox).ValueChanged += LSAhead_useCubeCurve_ValueChanged;


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
            (ShadowMediaUIHost.GetUI("LSAhead_useCubeCurve") as ParameterCheckbox).ValueUpdate();
        }



        private void LSAhead_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

        private void LSAhead_useCubeCurve_ValueChanged(object sender, EventArgs e)
        {
            this.useCubeCurve = (bool)(e as ParameterCheckbox.ChangedValue).Value;
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

            //ここから骨格点同期

            //輪郭の重心位置と比較する骨格のSpineBaseの位置リストを作る
            this.basePairList.Clear();
            for (int i = 0; i < BodyData.Length; i++)
            {
                if (BodyData[i].IsTracked)
                {
                    if (this.CheckBodyInSCreen(i, src))
                    {
                        this.basePairList.Add(new BodyBasePair(i, new Point(BodyDataOnDepthImage[i].JointDepth[JointType.SpineBase].position.x, BodyDataOnDepthImage[i].JointDepth[JointType.SpineBase].position.y)));
                    }
                }
            }


            //アクティブな骨格が輪郭の数より少ないとき　　少ない方から調べていく
            this.protectId.Clear();

            if (this.basePairList.Count <= this.Tree_ContsGroup.Count)
            {
                if (this.basePairList.Count != 0)
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
                        }
                        //余りものを検出するためのリスト作り
                        this.protectId.Add(this.Tree_ContsGroup[bufferNum].trackingId);

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
                                if (this.Tree_ContsGroup[bufferNum].boneStack[_TrackigBoneList[k]].Count > this.stackNum) this.Tree_ContsGroup[bufferNum].boneStack[_TrackigBoneList[k]].RemoveAt(this.Tree_ContsGroup[bufferNum].boneStack[_TrackigBoneList[k]].Count - 1);


                            }
                            //リストがなかったらとりあえず埋める　勝手にnullになるだろうという期待
                            else
                            {
                                this.bonePt_buf.Clear();
                                for (int l = 0; l < this.stackNum; ++l)
                                {
                                    this.bonePt_buf.Add(new Point(BodyDataOnDepthImage[this.basePairList[i].bodyNumber].JointDepth[_TrackigBoneList[k]].position.x, BodyDataOnDepthImage[this.basePairList[i].bodyNumber].JointDepth[_TrackigBoneList[k]].position.y));
                                }
                                this.Tree_ContsGroup[bufferNum].boneStack.Add(_TrackigBoneList[k], new List<Point?>(this.bonePt_buf));


                            }

                        }
                    }
                    //追加がなかったものにもnullリストをぶっこむ
                    for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
                    {
                        if (!protectId.Contains(this.Tree_ContsGroup[i].trackingId))
                        {
                            for (int k = 0; k < _TrackigBoneList.Count; ++k)
                            {
                                if (this.Tree_ContsGroup[i].boneStack.ContainsKey(_TrackigBoneList[k]))
                                {

                                    this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].Insert(0, null);

                                    //個数がオーバーしていたら消す
                                    if (this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].Count > this.stackNum) this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].RemoveAt(this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[k]].Count - 1);

                                }
                                //リストがなかったらとりあえず埋める　　勝手にnullになるだろうという期待
                                else
                                {
                                    this.bonePt_buf.Clear();
                                    for (int l = 0; l < this.stackNum; ++l)
                                    {
                                        this.bonePt_buf.Add(null);
                                    }
                                    this.Tree_ContsGroup[i].boneStack.Add(_TrackigBoneList[k], new List<Point?>(this.bonePt_buf));
                                }

                            }
                        }
                    }


                }
                //アクティブな骨格一つもないとき
                else
                {
                    for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
                    {
                        //骨格情報をツリーに入れる
                        for (int j = 0; j < _TrackigBoneList.Count; ++j)
                        {
                            if (this.Tree_ContsGroup[i].boneStack.ContainsKey(_TrackigBoneList[j]))
                            {
                                this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]].Insert(0, null);

                                //個数がオーバーしていたら消す
                                if (this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]].Count > this.stackNum) this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]].RemoveAt(this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]].Count - 1);


                            }
                            //リストがなかったらとりあえず埋める
                            else
                            {
                                this.bonePt_buf.Clear();
                                for (int l = 0; l < this.stackNum; ++l)
                                {
                                    this.bonePt_buf.Add(null);
                                }
                                this.Tree_ContsGroup[i].boneStack.Add(_TrackigBoneList[j], new List<Point?>(this.bonePt_buf));
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
                            //トラッキングしていたらPointを,してなかったらnullを代入
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
                            this.bonePt_buf.Clear();
                            for (int l = 0; l < this.stackNum; ++l)
                            {
                                this.bonePt_buf.Add(new Point(BodyDataOnDepthImage[this.basePairList[bufferNum].bodyNumber].JointDepth[_TrackigBoneList[k]].position.x, BodyDataOnDepthImage[this.basePairList[bufferNum].bodyNumber].JointDepth[_TrackigBoneList[k]].position.y));
                            }
                            this.Tree_ContsGroup[i].boneStack.Add(_TrackigBoneList[k], new List<Point?>(this.bonePt_buf));
                        }
                    }
                }
            }

            //骨格情報ツリー完成
            //---------------------------------------------------
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


            //------------------------------------------------------
            //各関節ごとの角度を算出

            for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
            {
                this.Tree_ContsGroup[i].moveMats.Clear();
                //全部入っているor全部入っていない　なのでチェックするのは何でもいい　とりあえずエルボー
                if (this.Tree_ContsGroup[i].boneRadstack.ContainsKey(JointType.ElbowLeft))
                {
                    //SpineBase
                    this.Tree_ContsGroup[i].boneRadstack[JointType.SpineBase].Insert(0, GetBaseBoneRad(this.Tree_ContsGroup[i].boneStack));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.SpineBase].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.SpineBase].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.SpineBase].Count - 1);

                    this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.SpineBase], this.useFrame, this.preFrame);
                    //Debug.Log("this.Tree_ContsGroup[i].boneRadstack[JointType.SpineBase] ; " + this.Tree_ContsGroup[i].boneRadstack[JointType.SpineBase][0]);
                    //Debug.Log("degbuf ; " + this.degbuf);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.SpineBase, GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType.SpineBase, this.degbuf, Matrix4x4.identity));

                    //ShoulderLeft
                    this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderLeft].Insert(0, GetBoneRad(this.Tree_ContsGroup[i].boneStack, JointType.ElbowLeft, JointType.SpineMid));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderLeft].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderLeft].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderLeft].Count - 1);

                    this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderLeft], this.useFrame, this.preFrame);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.ShoulderLeft, GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType.ShoulderLeft, this.degbuf, this.Tree_ContsGroup[i].moveMats[JointType.SpineBase]));

                    //ElbowLeft
                    this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowLeft].Insert(0, GetBoneRad(this.Tree_ContsGroup[i].boneStack, JointType.WristLeft, JointType.ElbowLeft));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowLeft].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowLeft].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowLeft].Count - 1);

                    this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowLeft], this.useFrame, this.preFrame);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.ElbowLeft, GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType.ElbowLeft, this.degbuf, this.Tree_ContsGroup[i].moveMats[JointType.ShoulderLeft]));


                    //ShoulderRight
                    this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderRight].Insert(0, GetBoneRad(this.Tree_ContsGroup[i].boneStack, JointType.ElbowRight, JointType.SpineMid));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderRight].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderRight].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderRight].Count - 1);

                    this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.ShoulderRight], this.useFrame, this.preFrame);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.ShoulderRight, GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType.ShoulderRight, this.degbuf, this.Tree_ContsGroup[i].moveMats[JointType.SpineBase]));


                    //ElbowRight
                    this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowRight].Insert(0, GetBoneRad(this.Tree_ContsGroup[i].boneStack, JointType.WristRight, JointType.ElbowRight));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowRight].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowRight].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowRight].Count - 1);

                    this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.ElbowRight], this.useFrame, this.preFrame);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.ElbowRight, GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType.ElbowRight, this.degbuf, this.Tree_ContsGroup[i].moveMats[JointType.ShoulderRight]));

                    //Neck
                    this.Tree_ContsGroup[i].boneRadstack[JointType.Neck].Insert(0, GetBoneRad(this.Tree_ContsGroup[i].boneStack, JointType.Head, JointType.SpineMid));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.Neck].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.Neck].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.Neck].Count - 1);

                    this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.Neck], this.useFrame, this.preFrame);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.Neck, GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType.Neck, this.degbuf, this.Tree_ContsGroup[i].moveMats[JointType.SpineBase]));

                    //HipLeft
                    this.Tree_ContsGroup[i].boneRadstack[JointType.HipLeft].Insert(0, GetBoneRad(this.Tree_ContsGroup[i].boneStack, JointType.KneeLeft, JointType.SpineBase));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.HipLeft].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.HipLeft].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.HipLeft].Count - 1);

                    //this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.], this.useFrame, this.preFrame);
                    //this.Tree_ContsGroup[i].moveMats.Add(JointType., GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType., this.degbuf, this.Tree_ContsGroup[i].moveMats[JointType.]));
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.HipLeft, Matrix4x4.identity);

                    //KneeKeft
                    this.Tree_ContsGroup[i].boneRadstack[JointType.KneeLeft].Insert(0, GetBoneRad(this.Tree_ContsGroup[i].boneStack, JointType.AnkleLeft, JointType.KneeLeft));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.KneeLeft].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.KneeLeft].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.KneeLeft].Count - 1);

                    //this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.], this.useFrame, this.preFrame);
                    //this.Tree_ContsGroup[i].moveMats.Add(JointType., GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType., this.degbuf, this.Tree_ContsGroup[i].moveMats[JointType.]));
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.KneeLeft, Matrix4x4.identity);

                    //HipRight
                    this.Tree_ContsGroup[i].boneRadstack[JointType.HipRight].Insert(0, GetBoneRad(this.Tree_ContsGroup[i].boneStack, JointType.KneeRight, JointType.SpineBase));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.HipRight].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.HipRight].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.HipRight].Count - 1);

                    //this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.], this.useFrame, this.preFrame);
                    //this.Tree_ContsGroup[i].moveMats.Add(JointType., GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType., this.degbuf, this.Tree_ContsGroup[i].moveMats[JointType.]));
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.HipRight, Matrix4x4.identity);

                    //KneeRight
                    this.Tree_ContsGroup[i].boneRadstack[JointType.KneeRight].Insert(0, GetBoneRad(this.Tree_ContsGroup[i].boneStack, JointType.AnkleRight, JointType.KneeRight));
                    if (this.Tree_ContsGroup[i].boneRadstack[JointType.KneeRight].Count > stackNum) this.Tree_ContsGroup[i].boneRadstack[JointType.KneeRight].RemoveAt(this.Tree_ContsGroup[i].boneRadstack[JointType.KneeRight].Count - 1);

                    //this.degbuf = LSRadPredict(this.Tree_ContsGroup[i].boneRadstack[JointType.], this.useFrame, this.preFrame);
                    //this.Tree_ContsGroup[i].moveMats.Add(JointType., GetMoveBoneMat(this.Tree_ContsGroup[i].boneStack, JointType., this.degbuf, this.Tree_ContsGroup[i].moveMats[JointType.]));
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.KneeRight, Matrix4x4.identity);
                }
                else
                {
                    //空ならすべてにnullのリストを入れる
                    this.boneRad_buf.Clear();
                    for (int l = 0; l < this.stackNum; ++l)
                    {
                        this.boneRad_buf.Add(null);
                    }
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.ElbowLeft, new List<float?>(this.boneRad_buf));
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.ShoulderLeft, new List<float?>(this.boneRad_buf));
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.ElbowRight, new List<float?>(this.boneRad_buf));
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.ShoulderRight, new List<float?>(this.boneRad_buf));
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.Neck, new List<float?>(this.boneRad_buf));
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.KneeLeft, new List<float?>(this.boneRad_buf));
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.HipLeft, new List<float?>(this.boneRad_buf));
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.KneeRight, new List<float?>(this.boneRad_buf));
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.HipRight, new List<float?>(this.boneRad_buf));
                    this.Tree_ContsGroup[i].boneRadstack.Add(JointType.SpineBase, new List<float?>(this.boneRad_buf));

                    this.Tree_ContsGroup[i].moveMats.Add(JointType.ElbowLeft,    Matrix4x4.identity);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.ShoulderLeft, Matrix4x4.identity);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.ElbowRight,   Matrix4x4.identity);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.ShoulderRight,Matrix4x4.identity);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.Neck,         Matrix4x4.identity);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.KneeLeft,     Matrix4x4.identity);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.HipLeft,      Matrix4x4.identity);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.KneeRight,    Matrix4x4.identity);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.HipRight,     Matrix4x4.identity);
                    this.Tree_ContsGroup[i].moveMats.Add(JointType.SpineBase,    Matrix4x4.identity);


                }
            }
            //-------------------------------------------------------
            
            //骨格点を先行させたリストを作る
            for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
            {
                //先行骨格点のリストを更新
                this.Tree_ContsGroup[i].preBonePoints.Clear();

                for (int j = 0; j < _TrackigBoneList.Count; ++j)
                {
                    /*
                    if (!this.useCubeCurve)
                    {
                        //二次曲線フィッティング
                        this.Tree_ContsGroup[i].preBonePoints.Add(_TrackigBoneList[j], LeastSquarePredict(this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]], this.useFrame, this.preFrame));
                    }
                    else
                    {
                        //三次曲線フィッティング
                        this.Tree_ContsGroup[i].preBonePoints.Add(_TrackigBoneList[j], LeastSquareCubePredict(this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]], this.useFrame, this.preFrame));
                    }
                    */

                  

                }

                //角度で回転させたもの


                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.WristLeft, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.WristLeft][0], this.Tree_ContsGroup[i].moveMats[JointType.ElbowLeft]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.ElbowLeft, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.ElbowLeft][0], this.Tree_ContsGroup[i].moveMats[JointType.ShoulderLeft]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.WristRight, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.WristRight][0], this.Tree_ContsGroup[i].moveMats[JointType.ElbowRight]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.ElbowRight, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.ElbowRight][0], this.Tree_ContsGroup[i].moveMats[JointType.ShoulderRight]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.SpineBase, this.Tree_ContsGroup[i].boneStack[JointType.SpineBase][0]);
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.Head, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.Head][0], this.Tree_ContsGroup[i].moveMats[JointType.Neck]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.AnkleLeft, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.AnkleLeft][0], this.Tree_ContsGroup[i].moveMats[JointType.KneeLeft]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.KneeLeft, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.KneeLeft][0], this.Tree_ContsGroup[i].moveMats[JointType.HipLeft]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.AnkleRight, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.AnkleRight][0], this.Tree_ContsGroup[i].moveMats[JointType.KneeRight]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.KneeRight, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.KneeRight][0], this.Tree_ContsGroup[i].moveMats[JointType.HipRight]));

                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.ShoulderLeft, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.ShoulderLeft][0], this.Tree_ContsGroup[i].moveMats[JointType.SpineBase]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.ShoulderRight, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.ShoulderRight][0], this.Tree_ContsGroup[i].moveMats[JointType.SpineBase]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.SpineMid, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.SpineMid][0], this.Tree_ContsGroup[i].moveMats[JointType.SpineBase]));
                this.Tree_ContsGroup[i].preBonePoints.Add(JointType.Neck, MoveBonePoint(this.Tree_ContsGroup[i].boneStack[JointType.Neck][0], this.Tree_ContsGroup[i].moveMats[JointType.SpineBase]));

            }

            //描画
            Cv2.CvtColor(m_buffer, m_buffer, OpenCvSharp.ColorConversion.BgrToHsv);

            for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
            {
                /*
                List<Point>[] m_contour = this.Tree_ContsGroup[i].ToArrey();
                //List<Point>[] m_preContour = SquarePredictCont(this.Tree_ContsGroup[i],5,5);
                List<Point>[] m_preContour = this.Tree_ContsGroup[i].MoveContour(SquarePredictCenterMove(this.Tree_ContsGroup[i], this.useFrame, this.preFrame)).ToArray();

                Cv2.DrawContours(m_buffer, m_contour, -1, this.Tree_ContsGroup[i].color, -1, OpenCvSharp.LineType.Link8);
                //Hをずらす　　　Sを下げて明るくするのは違いが分かりずらい
                Cv2.DrawContours(m_buffer, m_preContour, -1, new Scalar(this.Tree_ContsGroup[i].color.Val0 + 20, this.Tree_ContsGroup[i].color.Val1, this.Tree_ContsGroup[i].color.Val2), -1, OpenCvSharp.LineType.Link8);
                */

                for (int j = 0; j < _TrackigBoneList.Count; ++j)
                {
                    if (this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]][0] != null)
                    {
                        Cv2.Circle(m_buffer, (Point)this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]][0], 3, this.Tree_ContsGroup[i].color);

                    }
                    if (this.Tree_ContsGroup[i].preBonePoints[_TrackigBoneList[j]] != null)
                    {
                        Cv2.Circle(m_buffer, (Point)this.Tree_ContsGroup[i].preBonePoints[_TrackigBoneList[j]], 3, new Scalar(this.Tree_ContsGroup[i].color.Val0 + 40, this.Tree_ContsGroup[i].color.Val1, this.Tree_ContsGroup[i].color.Val2));

                    }

                }


                //骨格点の描画

                for (int j = 0; j < _TrackigBoneList.Count; ++j)
                {
                    if (_BoneConectMap.ContainsKey(_TrackigBoneList[j]))
                    {
                        if (this.Tree_ContsGroup[i].boneStack.ContainsKey(_TrackigBoneList[j]))
                        {
                            if (this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]][0] != null && this.Tree_ContsGroup[i].boneStack[_BoneConectMap[_TrackigBoneList[j]]][0] != null)
                            {
                                Cv2.Line(m_buffer, (Point)this.Tree_ContsGroup[i].boneStack[_TrackigBoneList[j]][0], (Point)this.Tree_ContsGroup[i].boneStack[_BoneConectMap[_TrackigBoneList[j]]][0], this.Tree_ContsGroup[i].color);
                            }
                            if (this.Tree_ContsGroup[i].preBonePoints[_TrackigBoneList[j]] != null && this.Tree_ContsGroup[i].preBonePoints[_BoneConectMap[_TrackigBoneList[j]]] != null)
                            {
                                Cv2.Line(m_buffer, (Point)this.Tree_ContsGroup[i].preBonePoints[_TrackigBoneList[j]], (Point)this.Tree_ContsGroup[i].preBonePoints[_BoneConectMap[_TrackigBoneList[j]]], new Scalar(this.Tree_ContsGroup[i].color.Val0 + 40, this.Tree_ContsGroup[i].color.Val1, this.Tree_ContsGroup[i].color.Val2));
                            }
                        }
                    }
                }
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
            public Dictionary<JointType, Matrix4x4> moveMats { get; set; }

            //現在のボーンの位置
            public Dictionary<JointType, List<Point?>> boneStack { get; set; }
            //先行させたボーンの位置
            public Dictionary<JointType, Point?> preBonePoints { get; set; }

            //現在のボーンの角度
            public Dictionary<JointType, List<float?>> boneRadstack { get; set; }

            //新しく作るとき　　　※ストック分の数値は全部ゼロにする
            public StackContsGroup(int? id, List<Point> contList, Point centerPt, int stacklim)
            {
                this.trackingId = id;
                this.contCenter = centerPt;
                this.color = new Scalar(UnityEngine.Random.Range(90, 270), 240, 240);
                this.contsStack = new List<List<Point>>();
                this.contCenterList = new List<Point>();
                this.buf = new List<List<Point>>();
                this.boneStack = new Dictionary<JointType, List<Point?>>();
                this.boneRadstack = new Dictionary<JointType, List<float?>>();
                this.preBonePoints = new Dictionary<JointType, Point?>();
                this.moveMats = new Dictionary<JointType, Matrix4x4>();


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

            public BodyBasePair(int bdNum, Point basePt)
            {
                this.bodyNumber = bdNum;
                this.basePoint = basePt;
            }
        }
        
        //関節の角度を180°表記で算出する
        public float? GetBoneRad(Dictionary<JointType,List<Point?>> bonePointList, JointType farJtTop, JointType nearJtTop)
        {
            if (bonePointList[farJtTop][0] != null && bonePointList[_BoneMap[farJtTop]][0] != null && bonePointList[nearJtTop][0] != null && bonePointList[_BoneMap[nearJtTop]][0] != null )
            {
                Vector3 farBoneVec  = new Vector3(bonePointList[farJtTop][0].Value.X - bonePointList[_BoneMap[farJtTop]][0].Value.X, bonePointList[farJtTop][0].Value.Y - bonePointList[_BoneMap[farJtTop]][0].Value.Y);
                Vector3 nearBoneVec = new Vector3(bonePointList[nearJtTop][0].Value.X - bonePointList[_BoneMap[nearJtTop]][0].Value.X, bonePointList[nearJtTop][0].Value.Y - bonePointList[_BoneMap[nearJtTop]][0].Value.Y);
           
                return Mathf.Sign(Vector3.Cross(farBoneVec, nearBoneVec).z) * Vector3.Angle(nearBoneVec, farBoneVec);
            }
            else
            {
                return null;
            }
        }

        //spinebaseの傾きを算出
        public float? GetBaseBoneRad(Dictionary<JointType, List<Point?>> bonePointList)
        {
            if (bonePointList[JointType.SpineBase][0] != null && bonePointList[_BoneMap[JointType.SpineBase]][0] != null )
            {
                Vector3 farBoneVec = new Vector3(bonePointList[JointType.SpineBase][0].Value.X - bonePointList[_BoneMap[JointType.SpineBase]][0].Value.X, bonePointList[JointType.SpineBase][0].Value.Y - bonePointList[_BoneMap[JointType.SpineBase]][0].Value.Y);
                Vector3 nearBoneVec = new Vector3(0,1,0);

                return Mathf.Sign(Vector3.Cross(farBoneVec, nearBoneVec).z) * Vector3.Angle(nearBoneVec, farBoneVec);
            }
            else
            {
                return null;
            }
        }

        //骨格点変換
        public Point? MoveBonePoint(Dictionary<JointType, List<Point?>> bonePointList, Point? movePt,  JointType pivotJt, float deg, Matrix4x4 preMat)
        {
            if(movePt == null)
            {
                return null;
            }
            else
            {
                Matrix4x4 ptMat = Matrix4x4.zero;
                ptMat.m00 = (float)movePt.Value.X;
                ptMat.m01 = (float)movePt.Value.Y;
                ptMat.m02 = 1;

                Matrix4x4 goOriMat = Matrix4x4.identity;
                goOriMat.m02 = -(float)bonePointList[pivotJt][0].Value.X;
                goOriMat.m12 = -(float)bonePointList[pivotJt][0].Value.Y;

                Matrix4x4 backOriMat = Matrix4x4.identity;
                backOriMat.m02 = (float)bonePointList[pivotJt][0].Value.X;
                backOriMat.m12 = (float)bonePointList[pivotJt][0].Value.Y;

                Matrix4x4 rotMat = Matrix4x4.identity;
                rotMat.m00 = (float) Math.Cos(deg * Mathf.Deg2Rad);
                rotMat.m01 = (float)-Math.Sin(deg * Mathf.Deg2Rad);
                rotMat.m10 = (float) Math.Sin(deg * Mathf.Deg2Rad);
                rotMat.m11 = (float) Math.Cos(deg * Mathf.Deg2Rad);

                Matrix4x4 ansMat = preMat * backOriMat * rotMat * goOriMat * ptMat;

                return new Point(ansMat.m00, ansMat.m01);
            }
        }

        //変換行列作成
        public Matrix4x4 GetMoveBoneMat(Dictionary<JointType, List<Point?>> bonePointList,  JointType pivotJt, float deg, Matrix4x4 preMat)
        {
            if (bonePointList[pivotJt][0] == null)
            {
                return Matrix4x4.identity;
            }
            else
            {
                Matrix4x4 goOriMat = Matrix4x4.identity;
                goOriMat.m02 = -(float)bonePointList[pivotJt][0].Value.X;
                goOriMat.m12 = -(float)bonePointList[pivotJt][0].Value.Y;
                goOriMat.m33 = 0;

                Matrix4x4 backOriMat = Matrix4x4.identity;
                backOriMat.m02 = (float)bonePointList[pivotJt][0].Value.X;
                backOriMat.m12 = (float)bonePointList[pivotJt][0].Value.Y;
                backOriMat.m33 = 0;

                Matrix4x4 rotMat = Matrix4x4.identity;
                rotMat.m00 = (float)Math.Cos(deg * Mathf.Deg2Rad);
                rotMat.m01 = (float)-Math.Sin(deg * Mathf.Deg2Rad);
                rotMat.m10 = (float)Math.Sin(deg * Mathf.Deg2Rad);
                rotMat.m11 = (float)Math.Cos(deg * Mathf.Deg2Rad);
                rotMat.m33 = 0;


                //float[,] goOriMatArr = { {1,0, -bonePointList[pivotJt][0].Value.X},
                //                         {0,1, -bonePointList[pivotJt][0].Value.Y},
                //                         {0,0,1 } };
                //Mat goOriMat2 = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

                //float[,] backOriMatArr ={ {1,0, bonePointList[pivotJt][0].Value.X},
                //                          {0,1, bonePointList[pivotJt][0].Value.Y},
                //                          {0,0,1 } };
                //Mat backOriMat2 = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


                //float[,] rotMatArr = { { (float)Math.Cos(deg * Mathf.Deg2Rad), (float)-Math.Sin(deg * Mathf.Deg2Rad),0 },
                //                     { (float)Math.Sin(deg * Mathf.Deg2Rad), (float)Math.Cos(deg * Mathf.Deg2Rad),0  },
                //                     {0,0,1 } };
                //Mat rotMat2 = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);

                //float[,] preMat3x3Arr = { {preMat.m00,preMat.m01, -bonePointList[pivotJt][0].Value.X},
                //                         {0,1, -bonePointList[pivotJt][0].Value.Y},
                //                         {0,0,1 } };
                //Mat preMat3x3 = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);


                ////  あと　　←　　先　:　元　　（行列の順番）
                //contMat = preMat3x3 * backOriMat * rotMat * goOriMat;

                return preMat * backOriMat * rotMat * goOriMat ;

            }




        }

        //骨格点変換
        public Point? MoveBonePoint( Point? movePt, Matrix4x4 preMat)
        {
            if (movePt == null)
            {
                return null;
            }
            else
            {
                Matrix4x4 ptMat = Matrix4x4.zero;
                ptMat.m00 = (float)movePt.Value.X;
                ptMat.m10 = (float)movePt.Value.Y;
                ptMat.m20 = 1;

               

                Matrix4x4 ansMat = preMat * ptMat;

                return new Point(ansMat.m00, ansMat.m10);
            }
        }

        //最小二乗法推測アルゴリズム　　nullがあったら即終了スタイル
        public Point? LeastSquarePredict(List<Point?> bonePointList, int useFeameNum, int preFrameNum)
        {
            Point predictPt;
            Point bufpt;

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
                //トラッキングできていないものが混じっていたら即終了
                if (bonePointList[i] == null)
                {
                    //if (bonePointList[0] == null)
                    //{
                    //    return new Point(0, 0);
                    //}
                    //else
                    //{
                    //    return (Point)bonePointList[0];
                    //}
                    return bonePointList[0];

                }


                t2.Add(Mathf.Pow((float)this.t1[i], 2));
                t3.Add(Mathf.Pow((float)this.t1[i], 3));
                t4.Add(Mathf.Pow((float)this.t1[i], 4));
                if (i < bonePointList.Count)
                {
                    bufpt = (Point)bonePointList[i];


                    t0x.Add(bufpt.X);
                    t1x.Add(this.t1[i] * bufpt.X);
                    t2x.Add(Mathf.Pow((float)this.t1[i], 2) * bufpt.X);

                    t0y.Add(bufpt.Y);
                    t1y.Add(this.t1[i] * bufpt.Y);
                    t2y.Add(Mathf.Pow((float)this.t1[i], 2) * bufpt.Y);
                }
                else
                {
                    bufpt = (Point)bonePointList[bonePointList.Count - 1];

                    t0x.Add(bufpt.X);
                    t1x.Add(this.t1[i] * bufpt.X);
                    t2x.Add(Mathf.Pow((float)this.t1[i], 2) * bufpt.X);

                    t0y.Add(bufpt.Y);
                    t1y.Add(this.t1[i] * bufpt.Y);
                    t2y.Add(Mathf.Pow((float)this.t1[i], 2) * bufpt.Y);
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

        //最小二乗法推測アルゴリズム　　nullがあったら即終了スタイル 角度バージョン
        //現在との差を出す
        public float LSRadPredict(List<float?> boneRadList, int useFeameNum, int preFrameNum)
        {
            double bufd;

            //List<double> t1 = new List<double>();
            List<double> t2 = new List<double>();
            List<double> t3 = new List<double>();
            List<double> t4 = new List<double>();
            List<double> t0d = new List<double>();
            List<double> t1d = new List<double>();
            List<double> t2d = new List<double>();


            double sumT1 = 0;
            double sumT2 = 0;
            double sumT3 = 0;
            double sumT4 = 0;

            double sumT0D = 0;
            double sumT1D = 0;
            double sumT2D = 0;


            Matrix4x4 TMat = Matrix4x4.identity;
            Matrix4x4 invTMat = Matrix4x4.identity;
            Matrix4x4 DMat = Matrix4x4.zero;
            Matrix4x4 ansDMat = Matrix4x4.zero;

            //Xから
            //各数値リストの作成
            //計算するフレームはもっと少なくてもいいかも
            for (int i = 0; i < this.t1.Count; ++i)
            {
                //トラッキングできていないものが混じっていたら即終了
                if (boneRadList[i] == null)
                {
                    return 0;
                }

                t2.Add(Mathf.Pow((float)this.t1[i], 2));
                t3.Add(Mathf.Pow((float)this.t1[i], 3));
                t4.Add(Mathf.Pow((float)this.t1[i], 4));
                if (i < boneRadList.Count)
                {
                    bufd = (double)boneRadList[i];

                    t0d.Add(bufd);
                    t1d.Add(this.t1[i] * bufd);
                    t2d.Add(Mathf.Pow((float)this.t1[i], 2) * bufd);
                }
                else
                {
                    bufd = (double)boneRadList[boneRadList.Count - 1];

                    t0d.Add(bufd);
                    t1d.Add(this.t1[i] * bufd);
                    t2d.Add(Mathf.Pow((float)this.t1[i], 2) * bufd);
                }
            }

            //各リストの累計を作る
            for (int i = 0; i < useFeameNum; ++i)
            {
                sumT1 += this.t1[i];
                sumT2 += t2[i];
                sumT3 += t3[i];
                sumT4 += t4[i];

                sumT0D += t0d[i];
                sumT1D += t1d[i];
                sumT2D += t2d[i];
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

            DMat.m00 = (float)sumT0D;
            DMat.m10 = (float)sumT1D;
            DMat.m20 = (float)sumT2D;

            //行列の計算
            ansDMat = TMat.inverse * DMat;

            //座標の計算
            return (float) (ansDMat.m00 + ansDMat.m10 * preFrameNum * 0.016 + ansDMat.m20 * preFrameNum * preFrameNum * 0.016f * 0.016f - boneRadList[0]);
        }

        //三次曲線フィッティング
        public Point? LeastSquareCubePredict(List<Point?> bonePointList, int useFeameNum, int preFrameNum)
        {
            Point predictPt;
            Point bufpt;

            //List<double> t1 = new List<double>();
            List<double> t2 = new List<double>();
            List<double> t3 = new List<double>();
            List<double> t4 = new List<double>();
            List<double> t5 = new List<double>();
            List<double> t6 = new List<double>();

            List<double> t0x = new List<double>();
            List<double> t1x = new List<double>();
            List<double> t2x = new List<double>();
            List<double> t3x = new List<double>();

            List<double> t0y = new List<double>();
            List<double> t1y = new List<double>();
            List<double> t2y = new List<double>();
            List<double> t3y = new List<double>();

            double sumT1 = 0;
            double sumT2 = 0;
            double sumT3 = 0;
            double sumT4 = 0;
            double sumT5 = 0;
            double sumT6 = 0;

            double sumT0X = 0;
            double sumT1X = 0;
            double sumT2X = 0;
            double sumT3X = 0;

            double sumT0Y = 0;
            double sumT1Y = 0;
            double sumT2Y = 0;
            double sumT3Y = 0;

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
                //トラッキングできていないものが混じっていたら即終了
                if (bonePointList[i] == null)
                {
                    //if (bonePointList[0] == null)
                    //{
                    //    return new Point(0, 0);
                    //}
                    //else
                    //{
                    //    return (Point)bonePointList[0];
                    //}
                    return bonePointList[0];
                }


                t2.Add(Mathf.Pow((float)this.t1[i], 2));
                t3.Add(Mathf.Pow((float)this.t1[i], 3));
                t4.Add(Mathf.Pow((float)this.t1[i], 4));
                t5.Add(Mathf.Pow((float)this.t1[i], 5));
                t6.Add(Mathf.Pow((float)this.t1[i], 6));
                if (i < bonePointList.Count)
                {
                    bufpt = (Point)bonePointList[i];

                    t0x.Add(bufpt.X);
                    t1x.Add(this.t1[i] * bufpt.X);
                    t2x.Add(Mathf.Pow((float)this.t1[i], 2) * bufpt.X);
                    t3x.Add(Mathf.Pow((float)this.t1[i], 3) * bufpt.X);

                    t0y.Add(bufpt.Y);
                    t1y.Add(this.t1[i] * bufpt.Y);
                    t2y.Add(Mathf.Pow((float)this.t1[i], 2) * bufpt.Y);
                    t3y.Add(Mathf.Pow((float)this.t1[i], 3) * bufpt.Y);
                }
                else
                {
                    bufpt = (Point)bonePointList[bonePointList.Count - 1];

                    t0x.Add(bufpt.X);
                    t1x.Add(this.t1[i] * bufpt.X);
                    t2x.Add(Mathf.Pow((float)this.t1[i], 2) * bufpt.X);
                    t3x.Add(Mathf.Pow((float)this.t1[i], 3) * bufpt.X);

                    t0y.Add(bufpt.Y);
                    t1y.Add(this.t1[i] * bufpt.Y);
                    t2y.Add(Mathf.Pow((float)this.t1[i], 2) * bufpt.Y);
                    t3y.Add(Mathf.Pow((float)this.t1[i], 3) * bufpt.Y);
                }
            }

            //各リストの累計を作る
            for (int i = 0; i < useFeameNum; ++i)
            {
                sumT1 += this.t1[i];
                sumT2 += t2[i];
                sumT3 += t3[i];
                sumT4 += t4[i];
                sumT5 += t5[i];
                sumT6 += t6[i];

                sumT0X += t0x[i];
                sumT1X += t1x[i];
                sumT2X += t2x[i];
                sumT3X += t3x[i];

                sumT0Y += t0y[i];
                sumT1Y += t1y[i];
                sumT2Y += t2y[i];
                sumT3Y += t3y[i];
            }

            //行列を作る
            TMat.m00 = useFeameNum;
            TMat.m01 = (float)sumT1;
            TMat.m02 = (float)sumT2;
            TMat.m03 = (float)sumT3;

            TMat.m10 = (float)sumT1;
            TMat.m11 = (float)sumT2;
            TMat.m12 = (float)sumT3;
            TMat.m13 = (float)sumT4;

            TMat.m20 = (float)sumT2;
            TMat.m21 = (float)sumT3;
            TMat.m22 = (float)sumT4;
            TMat.m23 = (float)sumT5;

            TMat.m30 = (float)sumT3;
            TMat.m31 = (float)sumT4;
            TMat.m32 = (float)sumT5;
            TMat.m33 = (float)sumT6;

            XMat.m00 = (float)sumT0X;
            XMat.m10 = (float)sumT1X;
            XMat.m20 = (float)sumT2X;
            XMat.m30 = (float)sumT3X;

            YMat.m00 = (float)sumT0Y;
            YMat.m10 = (float)sumT1Y;
            YMat.m20 = (float)sumT2Y;
            YMat.m30 = (float)sumT3Y;

            //行列の計算
            ansXMat = TMat.inverse * XMat;
            ansYMat = TMat.inverse * YMat;

            //座標の計算
            predictPt.X = (int)(ansXMat.m00 + ansXMat.m10 * preFrameNum * 0.016 + ansXMat.m20 * preFrameNum * preFrameNum * 0.016f * 0.016f + ansXMat.m30 * preFrameNum * preFrameNum * preFrameNum * 0.016f * 0.016f * 0.016f);
            predictPt.Y = (int)(ansYMat.m00 + ansYMat.m10 * preFrameNum * 0.016 + ansYMat.m20 * preFrameNum * preFrameNum * 0.016f * 0.016f + ansXMat.m30 * preFrameNum * preFrameNum * preFrameNum * 0.016f * 0.016f * 0.016f);

            return predictPt;
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

        //骨を作る際に使用
        private Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> _BoneConectMap = new Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType>()
    {
        //{ Windows.Kinect.JointType.FootLeft, Windows.Kinect.JointType.AnkleLeft },
        { Windows.Kinect.JointType.AnkleLeft, Windows.Kinect.JointType.KneeLeft },
        { Windows.Kinect.JointType.KneeLeft, Windows.Kinect.JointType.SpineBase },

        //{ Windows.Kinect.JointType.FootRight, Windows.Kinect.JointType.AnkleRight },
        { Windows.Kinect.JointType.AnkleRight, Windows.Kinect.JointType.KneeRight },
        { Windows.Kinect.JointType.KneeRight,Windows.Kinect.JointType.SpineBase },

        //{ Windows.Kinect.JointType.HandTipLeft, Windows.Kinect.JointType.HandLeft },
        //{ Windows.Kinect.JointType.ThumbLeft, Windows.Kinect.JointType.HandLeft },
        //{ Windows.Kinect.JointType.HandLeft, Windows.Kinect.JointType.WristLeft },
        { Windows.Kinect.JointType.WristLeft, Windows.Kinect.JointType.ElbowLeft },
        { Windows.Kinect.JointType.ElbowLeft, Windows.Kinect.JointType.ShoulderLeft },
        { Windows.Kinect.JointType.ShoulderLeft, Windows.Kinect.JointType.Neck },

        //{ Windows.Kinect.JointType.HandTipRight, Windows.Kinect.JointType.HandRight },
        //{ Windows.Kinect.JointType.ThumbRight, Windows.Kinect.JointType.HandRight },
        //{ Windows.Kinect.JointType.HandRight, Windows.Kinect.JointType.WristRight },
        { Windows.Kinect.JointType.WristRight, Windows.Kinect.JointType.ElbowRight },
        { Windows.Kinect.JointType.ElbowRight, Windows.Kinect.JointType.ShoulderRight },
        { Windows.Kinect.JointType.ShoulderRight, Windows.Kinect.JointType.Neck },

        { Windows.Kinect.JointType.SpineBase, Windows.Kinect.JointType.SpineMid },
        { Windows.Kinect.JointType.SpineMid, Windows.Kinect.JointType.Neck },
        { Windows.Kinect.JointType.Neck, Windows.Kinect.JointType.Head },
    };

        //全部で10本の骨として考える　handではなくwrist, footではなくankleを使用 spineBaseがkey  /Headがkey
        public Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> _BoneMap = new Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType>()
        {
            //例
            //Windows.Kinect.JointType.farJtTop, Windows.Kinect.JointType.nearJtTop },
            { Windows.Kinect.JointType.AnkleLeft, Windows.Kinect.JointType.KneeLeft },
            { Windows.Kinect.JointType.KneeLeft,  Windows.Kinect.JointType.SpineBase},

            { Windows.Kinect.JointType.AnkleRight,Windows.Kinect.JointType.KneeRight },
            { Windows.Kinect.JointType.KneeRight, Windows.Kinect.JointType.SpineBase },

            { Windows.Kinect.JointType.WristLeft, Windows.Kinect.JointType.ElbowLeft },
            { Windows.Kinect.JointType.ElbowLeft, Windows.Kinect.JointType.ShoulderLeft },

            { Windows.Kinect.JointType.WristRight, Windows.Kinect.JointType.ElbowRight },
            { Windows.Kinect.JointType.ElbowRight, Windows.Kinect.JointType.ShoulderRight },
            { Windows.Kinect.JointType.SpineBase, Windows.Kinect.JointType.SpineMid },
            { Windows.Kinect.JointType.Head, Windows.Kinect.JointType.Neck },

            { Windows.Kinect.JointType.SpineMid, Windows.Kinect.JointType.SpineBase },
        };

        //全部で10本の骨として考える　handではなくwrist, footではなくankleを使用 spineBaseがkey  /Headがkey
        public Dictionary<int, Windows.Kinect.JointType> _BoneKeyTable = new Dictionary<int, Windows.Kinect.JointType>()
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