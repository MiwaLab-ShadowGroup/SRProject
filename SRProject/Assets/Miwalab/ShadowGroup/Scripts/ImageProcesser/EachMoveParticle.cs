using Miwalab.ShadowGroup.ImageProcesser.Particle2D;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class EachMoveParticle : AShadowImageProcesser
    {
        public List<Particle2D.AParticle2D> m_particleList = new List<Particle2D.AParticle2D>();


        public float MaxSize = 2;
        public float MinSize = 0;
        private float ParticleNum = 1000;


        public EachMoveParticle()
            : base()
        {

            (GUI.BackgroundMediaUIHost.GetUI("EMP_Num_Init") as ParameterSlider).ValueChanged += BackRenderCamera_PV_Num_Init_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("EMP_Size_Max") as ParameterSlider).ValueChanged += BackRenderCamera_PV_Size_Max_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("EMP_Size_Min") as ParameterSlider).ValueChanged += BackRenderCamera_PV_Size_Min_ValueChanged;

            (GUI.BackgroundMediaUIHost.GetUI("EMP_Num_Init") as ParameterSlider).ValueUpdate();
            System.Random rand = new System.Random();
            for (int i = 0; i < 2000; ++i)
            {
                var particle = new SerializedCircleParticle();

                particle.SetRundomId(rand);

                particle.Size = 5;
                particle.Color = new Scalar(255, 255, 255);
                particle.Position = new UnityEngine.Vector2(UnityEngine.Random.Range(0, 512), UnityEngine.Random.Range(0, 424));
                this.m_particleList.Add(particle);
            }
        }

        private void BackRenderCamera_PV_Size_Min_ValueChanged(object sender, EventArgs e)
        {
            MinSize = (e as ParameterSlider.ChangedValue).Value;
        }

        private void BackRenderCamera_PV_Size_Max_ValueChanged(object sender, EventArgs e)
        {
            MaxSize = (e as ParameterSlider.ChangedValue).Value;
        }

        private void BackRenderCamera_PV_Num_Init_ValueChanged(object sender, EventArgs e)
        {
            ParticleNum = (e as ParameterSlider.ChangedValue).Value;
        }



        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.ParticleVector;
        }
        Mat m_dst;
        Vector2 m_currentCenter;
        Vector2 m_pastCenter;

        Mat m_buffer;
        private Mat grayimage = new Mat();
        private Mat dstMat = new Mat();
        // Mat dstMat = new Mat()
        List<OpenCvSharp.CPlusPlus.Point[]> List_Contours = new List<Point[]>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours_Buffer = new List<List<Point>>();


        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.List_Contours.Clear();

            if (m_buffer == null)
            {
                m_buffer = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, new Scalar(0, 0, 0));
            }
            else
            {
                m_buffer *= 0.9;
            }


            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            //dstMat = new Mat(dst.Height, dst.Width, MatType.CV_8UC4,colorBack);

            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            for (int i = 0; i < contour.Length; ++i)
            {
                if (Cv2.ContourArea(contour[i]) > 1000)
                {
                    this.List_Contours.Add(contour[i]);
                }
            }


            var size = src.Size();

            float CenterPositionX = 0;
            float CenterPositionY = 0;
            int counter = 0;

            m_dst = new Mat(size, MatType.CV_8UC3, new Scalar(0, 0, 0));
            unsafe
            {
                byte* data = src.DataPointer;
                for (int i = 0; i < this.m_particleList.Count; ++i)
                {
                    if (this.m_pastCenter.x == 0 && this.m_pastCenter.y == 0)
                    {

                        this.m_pastCenter.x = this.m_currentCenter.x;
                        this.m_pastCenter.y = this.m_currentCenter.y;
                    }

                    var particle = m_particleList[i] as SerializedCircleParticle;
                    var hid = particle.FId % this.List_Contours.Count;
                    var pid = particle.FId % this.List_Contours[hid].Length;




                    var targetpos = this.List_Contours[hid][pid];

                    this.m_particleList[i].AddForce(new UnityEngine.Vector2((targetpos.X - particle.Position.x) / 1000, (targetpos.Y - particle.Position.y) / 1000));
                    this.m_particleList[i].AddForce(this.m_particleList[i].Vellocity * -0.01f);

                    //this.m_particleList[i].Vellocity = new Vector2((targetpos.X - this.m_particleList[i].Position.x) / 100, (targetpos.Y - this.m_particleList[i].Position.y)/100);

                    this.m_particleList[i].Update();
                    //this.m_particleList[i].CutOffVellocity(MaxVellocity);
                    //this.m_particleList[i].DeadCheck(size.Width, size.Height);
                    //this.m_particleList[i].Revirth(size.Width, size.Height);
                    int index = ((int)this.m_particleList[i].Position.x + size.Width * (int)this.m_particleList[i].Position.y) * 3;
                    if (index > size.Height * size.Width * 3 - 1 || index < 0)
                    {
                        continue;
                    }

                    if (data[index] > 100)
                    {
                        this.m_particleList[i].Size = MaxSize;
                        CenterPositionX += this.m_particleList[i].Position.x;
                        CenterPositionY += this.m_particleList[i].Position.y;
                        ++counter;
                    }
                    else
                    {
                        this.m_particleList[i].Size = MinSize;
                    }

                    this.m_particleList[i].DrawShape(ref m_dst);
                }
            }

            Cv2.DrawContours(m_dst, this.List_Contours.ToArray(), -1, new Scalar(255, 255, 255), 1);
            m_dst.CopyTo(dst);



            this.m_pastCenter.x = this.m_currentCenter.x;
            this.m_pastCenter.y = this.m_currentCenter.y;
            this.m_currentCenter.x = CenterPositionX / counter;
            this.m_currentCenter.y = CenterPositionY / counter;
        }
        public override string ToString()
        {
            return ImageProcesserType.Particle2D.ToString();
        }
    }
}
