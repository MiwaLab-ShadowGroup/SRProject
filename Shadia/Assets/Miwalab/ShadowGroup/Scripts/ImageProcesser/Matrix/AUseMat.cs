using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Kinect = Windows.Kinect;


namespace Miwalab.ShadowGroup.ImageProcesser.Matrix
{
    public abstract class AUseMat : AImageProcesser
    {
        
        //全部で10本の骨として考える　handではなくwrist, footではなくankleを使用 spineBaseがkey  /Headがkey
        public Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> _BoneMap = new Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType>()
        {
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

        //posを計測するボーンのリスト
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
        

        public float VecLength(Vec2f vec)
        {
            return (float)Math.Sqrt(Math.Pow(vec.Item0, 2) + (Math.Pow(vec.Item1, 2)));
        }

        public float PointsLength(Point pt1, Point pt2)
        {
            return (float)Math.Sqrt(Math.Pow(pt1.X - pt2.X, 2) + (Math.Pow(pt1.Y - pt2.Y, 2)));
        }
        public float PointsLength(Point pt1)
        {
            return (float)Math.Sqrt(Math.Pow(pt1.X, 2) + (Math.Pow(pt1.Y, 2)));
        }

        public bool InCircleCheck(Point checkPt, Vec2f bone, Vec2f boneCtl, float r)
        {
            bool bl = false;
            if (Math.Sqrt(Math.Pow((checkPt.X - (bone.Item0 + boneCtl.Item0) / 2), 2 + Math.Pow(checkPt.Y - (bone.Item1 + boneCtl.Item1) / 2, 2))) < r)
            {
                bl = true;
            }

            return bl;
        }

        public float LineToPointLength(Vec2f bone, Vec2f bisector, Point pt)
        {

            return (bisector.Item0 * (pt.Y - bone.Item1) - bisector.Item1 * (pt.X - bone.Item0)) / VecLength(bisector);
        }

        public Point JointToDepthPoint(int bodyNum, Windows.Kinect.JointType jt)
        {
            return new Point(BodyDataOnDepthImage[bodyNum].JointDepth[jt].position.x, BodyDataOnDepthImage[bodyNum].JointDepth[jt].position.y);
        }

        public Point ContourRotate(BodyContor bc, double rad)
        {

            float[,] contMatArr = { {bc.center.X },
                                    {bc.center.Y },
                                    {1 } };
            Mat contMat = new Mat(3, 1, MatType.CV_32FC1, contMatArr);

            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bc.bodyNum,_BoneMap[ bc.nearJt1] ).X },
                                     {0,1, -JointToDepthPoint(bc.bodyNum, _BoneMap[ bc.nearJt1]).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1] ).X },
                                       {0,1, JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1]  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);

            //  あと　　←　　先　:　元　　（行列の順番）
            contMat = backOriMat * rotMat * goOriMat * contMat;



            return new Point(contMat.At<float>(0, 0), contMat.At<float>(0, 1));
        }

        public Point ContourRotate(BodyContor bc, double rad, Mat preMat3x3)
        {

            float[,] contMatArr = { {bc.center.X },
                                    {bc.center.Y },
                                    {1 } };
            Mat contMat = new Mat(3, 1, MatType.CV_32FC1, contMatArr);

            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bc.bodyNum,_BoneMap[ bc.nearJt1] ).X },
                                     {0,1, -JointToDepthPoint(bc.bodyNum, _BoneMap[ bc.nearJt1]).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1] ).X },
                                       {0,1, JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1]  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);

            //  あと　　←　　先　:　元　　（行列の順番）
            contMat = preMat3x3 * backOriMat * rotMat * goOriMat * contMat;

            return new Point(contMat.At<float>(0, 0), contMat.At<float>(0, 1));
        }

        public Mat GetRotateMat(BodyContor bc, double rad, Mat preMat3x3)
        {

            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bc.bodyNum,_BoneMap[ bc.nearJt1] ).X },
                                     {0,1, -JointToDepthPoint(bc.bodyNum, _BoneMap[ bc.nearJt1]).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1] ).X },
                                       {0,1, JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1]  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);


            return preMat3x3 * backOriMat * rotMat * goOriMat;
        }

        public Mat GetRotateMat(BodyContor bc, double rad)
        {
            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bc.bodyNum,_BoneMap[ bc.nearJt1] ).X },
                                     {0,1, -JointToDepthPoint(bc.bodyNum, _BoneMap[ bc.nearJt1]).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1] ).X },
                                       {0,1, JointToDepthPoint(bc.bodyNum,  _BoneMap[ bc.nearJt1]  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);


            return backOriMat * rotMat * goOriMat;
        }
        public Mat GetRotateMat(int bodyNum, Kinect.JointType pivotJt, double rad)
        {
            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bodyNum,pivotJt ).X },
                                     {0,1, -JointToDepthPoint(bodyNum, pivotJt).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bodyNum, pivotJt ).X },
                                       {0,1, JointToDepthPoint(bodyNum,  pivotJt  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);


            return backOriMat * rotMat * goOriMat;
        }
        public Mat GetRotateMat(int bodyNum, Kinect.JointType pivotJt, double rad, Mat preMat3x3)
        {
            float[,] goOriMatArr = { {1,0,-JointToDepthPoint(bodyNum,pivotJt ).X },
                                     {0,1, -JointToDepthPoint(bodyNum, pivotJt).Y },
                                     {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1,0,JointToDepthPoint(bodyNum,  pivotJt ).X },
                                       {0,1, JointToDepthPoint(bodyNum,  pivotJt  ).Y },
                                       {0,0,1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);


            float[,] rotMatArr = { { (float)Math.Cos(rad), (float)-Math.Sin(rad),0 },
                                   { (float)Math.Sin(rad), (float)Math.Cos(rad),0  },
                                   {0,0,1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);


            return preMat3x3 * backOriMat * rotMat * goOriMat;
        }

        //ボーンの前情報との比較から変換行列を算出
        /*
        public Mat GetBoneMoveMat(int bodyNum, Kinect.JointType nearJt, Kinect.JointType farJt, DepthBody[] preBody)
        {

            float[,] transMatArr = {{ 1,0, JointToDepthPoint(bodyNum, nearJt).X - preBody[bodyNum].JointDepth[nearJt].position.x },
                                        { 0,1, JointToDepthPoint(bodyNum, nearJt).Y - preBody[bodyNum].JointDepth[nearJt].position.y },
                                        { 0,0,1 }};
            Mat transMat = new Mat(3, 3, MatType.CV_32FC1, transMatArr);

            float scaleRate = PointsLength(JointToDepthPoint(bodyNum, nearJt), JointToDepthPoint(bodyNum, farJt)) /
                              PointsLength(new Point(preBody[bodyNum].JointDepth[nearJt].position.x - preBody[bodyNum].JointDepth[farJt].position.x, preBody[bodyNum].JointDepth[nearJt].position.y - preBody[bodyNum].JointDepth[farJt].position.y));
            float[,] scaleMatArr = { { scaleRate, 0, 0   },
                                         { 0,scaleRate, 0 },
                                         { 0, 0, 1 } };
            Mat scaleMat = new Mat(3, 3, MatType.CV_32FC1, scaleMatArr);


            float[,] goOriMatArr = { {1,0, - preBody[bodyNum].JointDepth[nearJt].position.x },
                                         {0,1, - preBody[bodyNum].JointDepth[nearJt].position.y },
                                         {0,0,1 } };
            Mat goOriMat = new Mat(3, 3, MatType.CV_32FC1, goOriMatArr);

            float[,] backOriMatArr = { {1, 0, preBody[bodyNum].JointDepth[nearJt].position.x  },
                                           {0, 1, preBody[bodyNum].JointDepth[nearJt].position.y  },
                                           {0, 0, 1 } };
            Mat backOriMat = new Mat(3, 3, MatType.CV_32FC1, backOriMatArr);

            //内積から角度を計算
            double boneRad = Math.Acos((double)((JointToDepthPoint(bodyNum, farJt).X - JointToDepthPoint(bodyNum, nearJt).X) * (preBody[bodyNum].JointDepth[farJt].position.x - preBody[bodyNum].JointDepth[nearJt].position.x) +
                                                (JointToDepthPoint(bodyNum, farJt).Y - JointToDepthPoint(bodyNum, nearJt).Y) * (preBody[bodyNum].JointDepth[farJt].position.y - preBody[bodyNum].JointDepth[nearJt].position.y)) /
                                                 PointsLength(JointToDepthPoint(bodyNum, farJt), JointToDepthPoint(bodyNum, nearJt)) * PointsLength(new Point(preBody[bodyNum].JointDepth[farJt].position.x - preBody[bodyNum].JointDepth[nearJt].position.x, preBody[bodyNum].JointDepth[farJt].position.y - preBody[bodyNum].JointDepth[nearJt].position.y)));

            //det = ax * by - ay * bx = - or +  どっち向きかの判断
            if ((JointToDepthPoint(bodyNum, farJt).X - JointToDepthPoint(bodyNum, nearJt).X) * (preBody[bodyNum].JointDepth[farJt].position.y - preBody[bodyNum].JointDepth[nearJt].position.y) -
                (JointToDepthPoint(bodyNum, farJt).Y - JointToDepthPoint(bodyNum, nearJt).Y) * (preBody[bodyNum].JointDepth[farJt].position.x - preBody[bodyNum].JointDepth[nearJt].position.x) < 0)
            {
                boneRad *= -1;   //ちょっとここはやってみてから判断笑
            }

            float[,] rotMatArr = { { (float)Math.Cos(boneRad), (float)-Math.Sin(boneRad),0 },
                                       { (float)Math.Sin(boneRad), (float) Math.Cos(boneRad),0  },
                                       {0, 0, 1 } };
            Mat rotMat = new Mat(3, 3, MatType.CV_32FC1, rotMatArr);

            //  順番 <----  　　完全に一致!　<- 平行移動　<- 元の場所へもどす　<- 回転＋拡大縮小　<- 原点へ移動
            return transMat * backOriMat * rotMat * scaleMat * goOriMat;

        }
        */

        public double GetBoneAccAve(List<List<Dictionary<Kinect.JointType, double>>> accList, int bodyNum, Kinect.JointType jt, int AveNum)
        {
            double Average = 0;
            int count = 0;
            for (int a = 0; a < AveNum; a++)
            {
                if (accList[a].Count > bodyNum)
                {
                    if (accList[a][bodyNum].ContainsKey(jt))
                    {
                        Average += accList[a][bodyNum][jt];
                    }
                    else
                    {
                        count++;
                    }
                }
                else
                {
                    count++;
                }

            }
            if (count == AveNum) count--;
            return Average / (AveNum - count);
        }


        public float GetBaseMoveAve(List<List<float>> baseTree, int bodyNum, int AveNum)
        {
            float Average = 0;
            int count = 0;
            for (int a = 0; a < AveNum; a++)
            {
                if (baseTree[a].Count > bodyNum)
                {
                    Average += baseTree[a][bodyNum];
                }
                else
                {
                    count++;
                }
            }
            if (count == AveNum) count--;
            return Average / (AveNum - count);
        }

        public double GetBoneRadAve(List<Dictionary<Kinect.JointType, double>> boneRadList, Kinect.JointType jt)
        {
            double Average = 0;
            int count = 0;
            if (boneRadList.Count != 0)
            {
                for (int a = 0; a < boneRadList.Count; a++)
                {
                    if (boneRadList[a].ContainsKey(jt))
                    {
                        Average += boneRadList[a][jt];
                    }
                    else
                    {
                        count++;
                    }
                }
                if (count == boneRadList.Count) count--;
                return Average / (boneRadList.Count - count);

            }
            else
            {
                return 0;
            }

        }

        public double GetBoneRad(int bodyNum, Kinect.JointType farJtTop, Kinect.JointType nearJtTop)
        {
            //内積から角度を計算
            double boneRad = Math.Acos(((JointToDepthPoint(bodyNum, farJtTop).X - JointToDepthPoint(bodyNum, _BoneMap[farJtTop]).X) * (JointToDepthPoint(bodyNum, nearJtTop).X - JointToDepthPoint(bodyNum, _BoneMap[nearJtTop]).X) +
                                        (JointToDepthPoint(bodyNum, farJtTop).Y - JointToDepthPoint(bodyNum, _BoneMap[farJtTop]).Y) * (JointToDepthPoint(bodyNum, nearJtTop).Y - JointToDepthPoint(bodyNum, _BoneMap[nearJtTop]).Y)) /
                                          PointsLength(JointToDepthPoint(bodyNum, farJtTop), JointToDepthPoint(bodyNum, _BoneMap[farJtTop])) / PointsLength(JointToDepthPoint(bodyNum, nearJtTop), JointToDepthPoint(bodyNum, _BoneMap[nearJtTop])));

            //det = ax * by - ay * bx = - or +  どっち向きかの判断
            if ((JointToDepthPoint(bodyNum, farJtTop).X - JointToDepthPoint(bodyNum, _BoneMap[farJtTop]).X) * (JointToDepthPoint(bodyNum, nearJtTop).Y - JointToDepthPoint(bodyNum, _BoneMap[nearJtTop]).Y) -
                (JointToDepthPoint(bodyNum, farJtTop).Y - JointToDepthPoint(bodyNum, _BoneMap[farJtTop]).Y) * (JointToDepthPoint(bodyNum, nearJtTop).X - JointToDepthPoint(bodyNum, _BoneMap[nearJtTop]).X) < 0)
            {
                boneRad *= -1;   // これで左曲がりがマイナス　右曲がりがプラス
            }
            return boneRad;
        }

        public double GetBoneRad(int bodyNum, Kinect.JointType spineBase)
        {
            //内積から角度を計算  vec(0,-1 ) との内積を求める

            double baseRad = Math.Acos(-1 * (JointToDepthPoint(bodyNum, _BoneMap[spineBase]).Y - JointToDepthPoint(bodyNum, spineBase).Y) /
                                       PointsLength(JointToDepthPoint(bodyNum, spineBase), JointToDepthPoint(bodyNum, _BoneMap[spineBase])));

            //det = ax * by - ay * bx = - or +  どっち向きかの判断
            //det = 0 * by - (-1)* bx = bx
            if (JointToDepthPoint(bodyNum, spineBase).X - JointToDepthPoint(bodyNum, _BoneMap[spineBase]).X < 0)
            {
                baseRad *= -1;   // これで左曲がりがマイナス　右曲がりがプラス
            }
            return baseRad;
        }

        public bool CheckBodyInSCreen(int bodyNum, Mat srcMat)
        {
            bool bl = true;
            //for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)

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
                    //    Debug.Log("lostBonex : " + _TrackigBoneList[i]);
                    bl = false;
                }
                if (double.IsInfinity(this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].position.y))
                {
                    //    Debug.Log("lostBoney : " + _TrackigBoneList[i]);
                    bl = false;
                }
                /*
                画面内にあるものも消してしまいそう↓
                if (this.BodyDataOnDepthImage[bodyNum].JointDepth[_TrackigBoneList[i]].state == Kinect.TrackingState.NotTracked)
                {
                    Debug.Log("UntrackingBone : " + _TrackigBoneList[i]);
                }
                */
            }

            return bl;
        }


        public abstract class BodyContor   //すべての輪郭点のクラス
        {

            public OpenCvSharp.CPlusPlus.Point center { get; set; }
            public int contourNum { get; set; }
            public int bodyNum { get; set; }
            public Windows.Kinect.JointType nearJt1 { get; set; }
            //Windows.Kinect.JointType nearJt2 { get; set; }

            /*
            public BodyContor(OpenCvSharp.CPlusPlus.Point pt, int contNum, int bdNum, Windows.Kinect.JointType jt1, Windows.Kinect.JointType jt2)
            {
                this.center = pt;
                this.contourNum = contNum;
                this.bodyNum = bdNum;
                this.nearJt1 = jt1;
                this.nearJt2 = jt2;
            }
            */
            public BodyContor(OpenCvSharp.CPlusPlus.Point pt, int contNum, int bdNum, Windows.Kinect.JointType jt1)
            {
                this.center = pt;
                this.contourNum = contNum;
                this.bodyNum = bdNum;
                this.nearJt1 = jt1;
            }
        }

        public class BoneMat   //すべての輪郭点のクラス
        {

            public OpenCvSharp.CPlusPlus.Point keyBonePos { get; set; }
            public OpenCvSharp.CPlusPlus.Point valueBonePos { get; set; }
            public Mat transMat { get; set; }
            public Mat rotMat { get; set; }
            public Mat scaleMat { get; set; }
            public Windows.Kinect.JointType joint { get; set; }

            public BoneMat(OpenCvSharp.CPlusPlus.Point keyPos, OpenCvSharp.CPlusPlus.Point valPos, Windows.Kinect.JointType jt)
            {
                this.keyBonePos = keyPos;
                this.valueBonePos = valPos;
                this.joint = jt;
            }

            public Mat totalMat()
            {
                return new Mat();
            }
        };
    }

}
