using Miwalab.ShadowGroup.ImageProcesser.Particle2D;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class FlowParticlesShadow : AShadowImageProcesser
    {
        public List<Particle2D.AParticle2D> m_particleList = new List<Particle2D.AParticle2D>();


        public float MaxSize = 2;
        public float MinSize = 0;
        private float ParticleNum = 1000;

        public FlowParticlesShadow()
            : base()
        {

            (GUI.BackgroundMediaUIHost.GetUI("FP_Num_Init") as ParameterSlider).ValueChanged += FP_Num_Init_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("FP_Size_Max") as ParameterSlider).ValueChanged += FP_Size_Max_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("FP_Size_Min") as ParameterSlider).ValueChanged += FP_Size_Min_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("FP_Max_V") as ParameterSlider).ValueChanged += FP_Max_V_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("FP_ColorUse") as ParameterCheckbox).ValueChanged += FP_ColorUse_ValueChanged;

            (GUI.BackgroundMediaUIHost.GetUI("FP_Force_X") as ParameterSlider).ValueChanged += FP_Force_X_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("FP_Force_Y") as ParameterSlider).ValueChanged += FP_Force_Y_ValueChanged;



            (GUI.BackgroundMediaUIHost.GetUI("FP_Num_Init") as ParameterSlider).ValueUpdate();
            System.Random rand = new System.Random();
            for (int i = 0; i < ParticleNum; ++i)
            {
                var particle = new SerializedCircleParticle();

                particle.SetRundomId(rand);

                particle.Size = 5;
                particle.Color = new Scalar(255, 255, 255);
                particle.Position = new UnityEngine.Vector2(UnityEngine.Random.Range(0, 512), UnityEngine.Random.Range(0, 424));
                this.m_particleList.Add(particle);
            }
        }

        private void FP_Force_Y_ValueChanged(object sender, EventArgs e)
        {
            m_FlowDirection_Y = (e as ParameterSlider.ChangedValue).Value;
        }

        private void FP_Force_X_ValueChanged(object sender, EventArgs e)
        {
            m_FlowDirection_X = (e as ParameterSlider.ChangedValue).Value;
        }

        private void FP_ColorUse_ValueChanged(object sender, EventArgs e)
        {
            UseColor = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void FP_Max_V_ValueChanged(object sender, EventArgs e)
        {
            MaxVellocity = (e as ParameterSlider.ChangedValue).Value;
        }

        private void FP_Size_Min_ValueChanged(object sender, EventArgs e)
        {
            MinSize = (e as ParameterSlider.ChangedValue).Value;
        }

        private void FP_Size_Max_ValueChanged(object sender, EventArgs e)
        {
            MaxSize = (e as ParameterSlider.ChangedValue).Value;
        }

        private void FP_Num_Init_ValueChanged(object sender, EventArgs e)
        {
            ParticleNum = (e as ParameterSlider.ChangedValue).Value;
        }
        
        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.ParticleVector;
        }
        Mat m_dst;

        static readonly int HUMAN_NUMBER = 16;
        Vector2[] m_currentCenter = new Vector2[HUMAN_NUMBER];
        Vector2[] m_pastCenter = new Vector2[HUMAN_NUMBER];
        float[] m_bufferCenterX = new float[HUMAN_NUMBER];
        float[] m_bufferCenterY = new float[HUMAN_NUMBER];
        int[] m_counter = new int[HUMAN_NUMBER];

        public float MaxVellocity = 1f;
        private Mat m_indexMat;
        Mat grayimage = new Mat();

        public float VellocityK { get; private set; }
        public bool UseColor { get; private set; }
        public float m_FlowDirection_X;
        public float m_FlowDirection_Y;


        public int ColorCounter_R = 170;
        public int ColorCounter_G = 170;
        public int ColorCounter_B = 0;

        public bool ColorCounter_R_Return = false;
        public bool ColorCounter_G_Return = true;
        public bool ColorCounter_B_Return = true;


        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            var size = src.Size();

            for (int i = 0; i < HUMAN_NUMBER; ++i)
            {
                this.m_bufferCenterX[i] = 0;
                this.m_bufferCenterY[i] = 0;
                this.m_counter[i] = 0;
            }


            m_dst = new Mat(size, MatType.CV_8UC3, new Scalar(0, 0, 0));



            #region Contour
            if (m_indexMat == null)
            {
                m_indexMat = new Mat(size, MatType.CV_8UC1, new Scalar(0));
            }
            m_indexMat *= 0;



            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);


            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);
            int MaxHumanNum = 0;

            for (int i = 0; i < contour.Length; i++)
            {

                if (Cv2.ContourArea(contour[i]) > 1000)
                {
                    m_currentCenter[MaxHumanNum] = this.GetCenter(contour[i]);
                    Cv2.DrawContours(m_indexMat, new Point[][] { contour[i] }, -1, new Scalar(MaxHumanNum + 1), -1, OpenCvSharp.LineType.Link8);

                    ++MaxHumanNum;

                }

            }
            #endregion
            unsafe
            {
                byte* data = src.DataPointer;
                byte* indexData = m_indexMat.DataPointer;
                int currentNumber;
                for (int i = 0; i < this.m_particleList.Count; ++i)
                {


                    this.m_particleList[i].AddForce(new UnityEngine.Vector2(m_FlowDirection_X,m_FlowDirection_Y));
                    //this.m_particleList[i].GraduallyChangeColorTo(Scalar.Black, 0.005f);
                    this.m_particleList[i].Update();
                    this.m_particleList[i].CutOffVellocity(MaxVellocity);
                    this.m_particleList[i].DeadCheck(size.Width, size.Height);
                    this.m_particleList[i].Revirth(size.Width, size.Height);
                    this.m_particleList[i].DeadCheck(size.Width, size.Height);
                    this.m_particleList[i].RevirthRandom(size.Width, size.Height);

                    int index = ((int)this.m_particleList[i].Position.x + size.Width * (int)this.m_particleList[i].Position.y) * 3;
                    if (index > size.Height * size.Width * 3 - 1 || index < 0)
                    {
                        continue;
                    }

                    currentNumber = indexData[index / 3];

                    if (data[index] > 100)
                    {
                        if (currentNumber > 0)
                        {
                            this.m_particleList[i].Size = MaxSize;
                            

                            if (UseColor)
                            {
                                this.m_particleList[i].GraduallyChangeColorTo(new Scalar(ColorCounter_B, ColorCounter_G, ColorCounter_R), 1);

                            }
                            ++m_counter[currentNumber - 1];
                        }
                    }
                    else
                    {
                        this.m_particleList[i].Size = MinSize;
                    }



                    this.m_particleList[i].DrawShape(ref m_dst);


                }

            }
            m_dst.CopyTo(dst);


            for (int i = 0; i < HUMAN_NUMBER; ++i)
            {
                this.m_pastCenter[i].x = this.m_currentCenter[i].x;
                this.m_pastCenter[i].y = this.m_currentCenter[i].y;
            }


            if(ColorCounter_R > 255)
            {
                ColorCounter_R_Return = false;
            }
            if (ColorCounter_G > 255)
            {
                ColorCounter_G_Return = false;
            }
            if (ColorCounter_B > 255)
            {
                ColorCounter_B_Return = false;
            }

            if (ColorCounter_R < 1)
            {
                ColorCounter_R_Return = true;
            }
            if (ColorCounter_G < 1)
            {
                ColorCounter_G_Return = true;
            }
            if (ColorCounter_B < 1)
            {
                ColorCounter_B_Return = true;
            }


            ColorCounter_R = ColorCounter_R_Return ? ColorCounter_R + 1 : ColorCounter_R - 1;
            ColorCounter_G = ColorCounter_G_Return ? ColorCounter_G + 1 : ColorCounter_G - 1;
            ColorCounter_B = ColorCounter_B_Return ? ColorCounter_B + 1 : ColorCounter_B - 1;


        }

        private Vector2 GetCenter(Point[] point)
        {
            float x = 0;
            float y = 0;
            for (int i = 0; i < point.Length; ++i)
            {
                x += point[i].X;
                y += point[i].Y;
            }
            x /= point.Length;
            y /= point.Length;
            return new Vector2(x, y);
        }

        public override string ToString()
        {
            return ImageProcesserType.Particle2D.ToString();
        }

    }
}
