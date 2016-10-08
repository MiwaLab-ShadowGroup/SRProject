using Miwalab.ShadowGroup.ImageProcesser.Particle2D;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Windows.Kinect;

namespace Miwalab.ShadowGroup.ImageProcesser
{
 
    public class ParticleVector : AShadowImageProcesser
    {
        public List<Particle2D.TaggedCircleParticle> m_particleList = new List<Particle2D.TaggedCircleParticle>();


        public float MaxSize = 2;
        public float MinSize = 0;
        private float ParticleNum = 1000;


        public ParticleVector()
            : base()
        {

            (GUI.BackgroundMediaUIHost.GetUI("PV_Num_Init") as ParameterSlider).ValueChanged += BackRenderCamera_PV_Num_Init_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("PV_Size_Max") as ParameterSlider).ValueChanged += BackRenderCamera_PV_Size_Max_ValueChanged;
            (GUI.BackgroundMediaUIHost.GetUI("PV_Size_Min") as ParameterSlider).ValueChanged += BackRenderCamera_PV_Size_Min_ValueChanged;

            (GUI.BackgroundMediaUIHost.GetUI("PV_Num_Init") as ParameterSlider).ValueUpdate();
            (ShadowMediaUIHost.GetUI("PV_Reset") as ParameterButton).Clicked += ParticleVector_Clicked;

            for (int i = 0; i < 2000; ++i)
            {
                var particle = new TaggedCircleParticle();
                particle.Size = 5;
                particle.Color = new Scalar(255, 255, 255);
                particle.Position = new UnityEngine.Vector2(UnityEngine.Random.Range(0, 512), UnityEngine.Random.Range(0, 424));
                this.m_particleList.Add(particle);
            }

            this.ChangeHumanCount += ParticleVector_ChangeHumanCount;

        }

        private void ParticleVector_ChangeHumanCount(int count)
        {
            this.ResetCircles();
        }

        private void ParticleVector_Clicked(object sender, EventArgs e)
        {
            this.ResetCircles();
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

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            var size = src.Size();
            m_dst = new Mat(size, MatType.CV_8UC3, new Scalar(0, 0, 0));
            Vector3 vell = new Vector2(0, 0);
           

            unsafe
            {
                byte* data = src.DataPointer;
                for (int i = 0; i < this.m_particleList.Count; ++i)
                {
                    //this.m_particleList[i].AddForce(new UnityEngine.Vector2(UnityEngine.Random.Range(-0.1f, 0.1f) + (this.m_currentCenter.x-this.m_pastCenter.x)/10, UnityEngine.Random.Range(-0.1f, 0.1f) + (this.m_currentCenter.y - this.m_pastCenter.y) / 10));

                    if (this.m_particleList[i].Setupped)
                    {
                        var p = this.m_particleList[i];
                        vell = this.BodyDataOnDepthImage[p.id].JointDepth[p.jointType].vellocity_upperCorrect;
                    }
                    else
                    {
                        vell.Set(0, 0, 0);
                    }

                    this.m_particleList[i].AddForce(vell);
                    this.m_particleList[i].AddForce(this.m_particleList[i].Vellocity * -0.01f);
                    this.m_particleList[i].Update();
                    //this.m_particleList[i].CutOffVellocity(MaxVellocity);
                    this.m_particleList[i].DeadCheck(size.Width, size.Height);
                    this.m_particleList[i].Revirth(size.Width, size.Height);
                    int index = ((int)this.m_particleList[i].Position.x + size.Width * (int)this.m_particleList[i].Position.y) * 3;
                    if (index > size.Height * size.Width * 3 - 1 || index < 0)
                    {
                        continue;
                    }
                    if (data[index] > 100)
                    {
                        this.m_particleList[i].Size = MaxSize;
                    }
                    else
                    {
                        this.m_particleList[i].Size = MinSize;
                    }
                    this.m_particleList[i].DrawShape(ref m_dst);
                }
            }


            m_dst.CopyTo(dst);
        }

        private void ResetCircles()
        {
            for (int i = 0; i < this.m_particleList.Count; ++i)
            {
                this.m_particleList[i].AutoReset(TaggedCircleParticle.ResetType.Simbolic, this.bodyIdList);
            }
        }

        public override string ToString()
        {
            return ImageProcesserType.Particle2D.ToString();
        }
    }
}
