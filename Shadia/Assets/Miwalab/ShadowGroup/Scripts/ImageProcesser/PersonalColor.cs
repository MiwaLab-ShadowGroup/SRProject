using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    class PersonalColor : AShadowImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        private Mat grayimage = new Mat();
        List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        Scalar color;
        Scalar colorBack;
        int minArea;

        List<OpenCvSharp.CPlusPlus.Point> contour_Center = new List<Point>();
        List<StackContsGroup> Tree_ContsGroup = new List<StackContsGroup>();
        List<NowContsGroup> List_NowContsGroup = new List<NowContsGroup>();
        int stackNum = 5;


        int numberingId = 0;
        List<int?> activeIdList = new List<int?>();
        float thresholdDist = 200;

        double dist = 0;
        double minDist = 1000; //とりあえず笑　その場しのぎ
        int bufferNum = 0;

        List<int?> protectId = new List<int?>();
        bool existNum = false;

        public PersonalColor() : base()
        {
            (ShadowMediaUIHost.GetUI("PersonalColor_UseFade") as ParameterCheckbox).ValueChanged += PersonalColor_UseFade_ValueChanged;
            (ShadowMediaUIHost.GetUI("PersonalColor_minArea") as ParameterSlider).ValueChanged += PersonalColor_minArea_ValueChanged;

            (ShadowMediaUIHost.GetUI("PersonalColor_UseFade") as ParameterCheckbox).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PersonalColor_minArea") as ParameterSlider).ValueUpdate();
        }



        private void PersonalColor_UseFade_ValueChanged(object sender, EventArgs e)
        {
            this.m_UseFade = (bool)(e as ParameterCheckbox.ChangedValue).Value;
        }

      
        private void PersonalColor_minArea_ValueChanged(object sender, EventArgs e)
        {
            this.minArea = (int)(e as ParameterSlider.ChangedValue).Value;

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

            Cv2.GaussianBlur(grayimage, grayimage, new Size(3, 3), 0f);

            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, colorBack);


            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);


            //とりあえず輪郭のリストを作る
            for (int i = 0; i < contour.Length; i++)
            {
                this.CvPoints.Clear();
                if (Cv2.ContourArea(contour[i]) > this.minArea)

                {
                    //重心検出処理
                    var cont = contour[i].ToArray();
                    var M = Cv2.Moments(cont);
                    this.contour_Center.Add(new Point((M.M10 / M.M00), (M.M01 / M.M00)));
                    //輪郭点のリスト作成
                    for (int j = 0; j < contour[i].Length; j++)
                    {
                        CvPoints.Add(contour[i][j]);

                    }
                    this.List_NowContsGroup.Add(new NowContsGroup(null, this.CvPoints, new Point((M.M10 / M.M00), (M.M01 / M.M00))));
                }

            }

            //------------------------------------------------
            //現在の輪郭すべてにIDを振る
            this.activeIdList.Clear();

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

                        if (dist < minDist)
                        {
                            bufferNum = j;
                            minDist = dist;
                        }
                    }

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
                    this.minDist = 1000; //とりあえず
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

            //-----------------------------------------------
           
            //描画
            Cv2.CvtColor(m_buffer, m_buffer, OpenCvSharp.ColorConversion.BgrToHsv);

            for (int i = 0; i < this.Tree_ContsGroup.Count; ++i)
            {
                List<Point>[] m_contour = this.Tree_ContsGroup[i].ToArrey();
                Cv2.DrawContours(m_buffer, m_contour, -1, this.Tree_ContsGroup[i].color, -1, OpenCvSharp.LineType.Link8);

            }
            Cv2.CvtColor(m_buffer, m_buffer, OpenCvSharp.ColorConversion.HsvToBgr);


            //ブラーで滑らかにぼかす
            Cv2.GaussianBlur(m_buffer, m_buffer, new Size(3, 3), 0f);

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
            public List<Point> contCenterList { get; set; }
            private List<List<Point>> buf { get; set; }


            //新しく作るとき　　　※ストック分の数値は全部ゼロにする
            public StackContsGroup(int? id, List<Point> contList, Point centerPt, int stacklim)
            {
                this.trackingId = id;
                this.contCenter = centerPt;
                this.color = new Scalar(UnityEngine.Random.Range(0, 180), UnityEngine.Random.Range(180, 240), UnityEngine.Random.Range(180, 240));
                this.contsStack = new List<List<Point>>();
                this.contCenterList = new List<Point>();
                this.buf = new List<List<Point>>();

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


        public override string ToString()
        {
            return "PersonalColor";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.PersonalColor;
        }

        public bool IsFirstFrame { get; private set; }
    }




}
