using Miwalab.ShadowGroup.Network;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Particle3D : AShadowImageProcesser
    {
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<Point>>();
        DepthBody db;

        Mat m_buffer;
        bool m_UseFade;


        

        particle3DShadow particle3DShadow;

        float[] _xList;
        float[] _yList;
        float[] _xListPrev;
        float[] _yListPrev;
        float[] _diffx;
        float[] _diffy;

        public Particle3D()
            : base()
        {

            var uihost = GameObject.FindWithTag("uihost").GetComponent<ShadowMediaUIHost>();
            uihost.SetActive3DObjects((int)Background.BackgroundType.Particle3DShadow);


            //UIからパラメータ変更通知の追加
            particle3DShadow = GameObject.FindWithTag("particle3D").GetComponent<particle3DShadow>();


            _xList = new float[particle3DShadow.MaxHumanNum];
            _yList = new float[particle3DShadow.MaxHumanNum];
            _xListPrev = new float[particle3DShadow.MaxHumanNum];
            _yListPrev = new float[particle3DShadow.MaxHumanNum];
            _diffx = new float[particle3DShadow.MaxHumanNum];
            _diffy = new float[particle3DShadow.MaxHumanNum];
        }


        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Particle3D;
        }

        private void Update(ref Mat src, ref Mat dst)
        {
            this.List_Contours.Clear();

            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, Scalar.Black);
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

            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER _enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();

            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Cv2.MedianBlur(grayimage, grayimage, 21);
            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,colorBack);
            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();
            int humanCount = 0;

            for (int i = 0; i < contour.Length; i++)
            {

                CvPoints.Clear();
                if (Cv2.ContourArea(contour[i]) > 1000)
                {
                    humanCount++;
                    if (humanCount >= _xList.Length) continue;
                    _xList[humanCount] = 0;
                    _yList[humanCount] = 0;
                    this.List_Contours.Add(new List<Point>(contour[i]));
                    foreach(var con in contour[i])
                    {
                        _xList[humanCount] += con.X;
                        _yList[humanCount] += con.Y;
                    }
                    _xList[humanCount] /= contour[i].Length;
                    _yList[humanCount] /= contour[i].Length;

                    _diffx[humanCount] = (_xList[humanCount] - _xListPrev[humanCount])/1000f;
                    _diffy[humanCount] = (_yList[humanCount] - _yListPrev[humanCount])/1000f;
                    
                    _xListPrev[humanCount] = _xList[humanCount];
                    _yListPrev[humanCount] = _yList[humanCount];
                }

            }
            var _contour = List_Contours.ToArray();
            Cv2.DrawContours(dst, _contour, -1, Scalar.Aqua);

            particle3DShadow.SendHumanVelocity(humanCount, _diffx, _diffy);
            dst = src;

        }

        public override string ToString()
        {
            return this.getImageProcesserType().ToString();
        }
    }
}
