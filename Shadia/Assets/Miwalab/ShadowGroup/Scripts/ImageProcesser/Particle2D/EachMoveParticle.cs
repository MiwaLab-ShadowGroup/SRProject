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
        private float Response = 20f;
        private bool UseOwnMove = true;

        public EachMoveParticle()
            : base()
        {

            (GUI.BackgroundMediaUIHost.GetUI("EMP_Num_Init") as ParameterSlider).ValueChanged += BackRenderCamera_PV_Num_Init_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("EMP_Size_Max") as ParameterSlider).ValueChanged += BackRenderCamera_PV_Size_Max_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("EMP_Size_Min") as ParameterSlider).ValueChanged += BackRenderCamera_PV_Size_Min_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("EMP_Max_V") as ParameterSlider).ValueChanged += EMP_Max_V_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("EMP_Response") as ParameterSlider).ValueChanged += EMP_Response_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("EMP_VK") as ParameterSlider).ValueChanged += EMP_VK_ValueChanged;

            (GUI.BackgroundMediaUIHost.GetUI("EMP_UseOwnMove") as ParameterCheckbox).ValueChanged += EMP_UseOwnMove_ValueChanged;

            (GUI.BackgroundMediaUIHost.GetUI("EMP_ColorUse") as ParameterCheckbox).ValueChanged += EMP_ColorUse_ValueChanged;



            (GUI.BackgroundMediaUIHost.GetUI("EMP_Num_Init") as ParameterSlider).ValueUpdate();
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

        private void EMP_ColorUse_ValueChanged(object sender, EventArgs e)
        {
            UseColor = (e as ParameterCheckbox.ChangedValue).Value; ;
        }

        private void EMP_VK_ValueChanged(object sender, EventArgs e)
        {
            VellocityK = (e as ParameterSlider.ChangedValue).Value;

        }

        private void EMP_UseOwnMove_ValueChanged(object sender, EventArgs e)
        {
            UseOwnMove = (e as ParameterCheckbox.ChangedValue).Value;
        }

        private void EMP_Response_ValueChanged(object sender, EventArgs e)
        {
            Response = (e as ParameterSlider.ChangedValue).Value;
        }

        private void EMP_Max_V_ValueChanged(object sender, EventArgs e)
        {
            MaxVellocity = (e as ParameterSlider.ChangedValue).Value;
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

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            var size = src.Size();

            //for (int i = 0; i < HUMAN_NUMBER; ++i)
            //{
            //    this.m_bufferCenterX[i] = 0;
            //    this.m_bufferCenterY[i] = 0;
            //    this.m_counter[i] = 0;
            //}


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


                    //this.m_particleList[i].AddForce(new UnityEngine.Vector2(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f)));
                    //this.m_particleList[i].AddForce(new UnityEngine.Vector2((this.m_currentCenter.x - this.m_pastCenter.x) / 10, (this.m_currentCenter.y - this.m_pastCenter.y) / 10));
                    this.m_particleList[i].AddForce(this.m_particleList[i].Vellocity * -0.01f);
                    //this.m_particleList[i].GraduallyChangeColorTo(Scalar.White, 0.02f);
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
                            //m_bufferCenterX[currentNumber] += this.m_particleList[i].Position.x;
                            //m_bufferCenterY[currentNumber] += this.m_particleList[i].Position.y;

                            if (UseOwnMove)
                            {
                                this.m_particleList[i].AddForce((this.m_currentCenter[currentNumber - 1] - this.m_pastCenter[currentNumber - 1]) / Response);
                            }
                            this.m_particleList[i].AddForce(this.m_particleList[i].Vellocity * this.VellocityK);

                            if (UseColor)
                            {
                                this.m_particleList[i].GraduallyChangeColorTo(Scalar.Blue, 0.05f);

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
                //this.m_currentCenter[i].x = m_bufferCenterX[i] / m_counter[i];
                //this.m_currentCenter[i].y = m_bufferCenterY[i] / m_counter[i];
            }
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
