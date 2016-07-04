using Miwalab.ShadowGroup.ImageProcesser.Particle2D;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class ShadowParticle2D : AShadowImageProcesser
    {
        public List<Particle2D.AParticle2D> m_particleList = new List<Particle2D.AParticle2D>();
        public ShadowParticle2D()
            : base()
        {
            for (int i = 0; i < 1000; ++i)
            {
                var particle = new CircleParticle();
                particle.Size = 5;
                particle.Color = new Scalar(255,255,255);
                particle.Position = new UnityEngine.Vector2(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100));
                this.m_particleList.Add(particle);
            }
        }


        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Particle2D;
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
                var size = src.Size();
            for (int i =0; i < this.m_particleList.Count; ++i)
            {
                this.m_particleList[i].AddForce(new UnityEngine.Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)));
                this.m_particleList[i].Update();
                this.m_particleList[i].DeadCheck(size.Width, size.Height);
                this.m_particleList[i].Revirth(size.Width, size.Height);
                this.m_particleList[i].DrawShape(ref dst);
            }
        } 
        public override string ToString()
        {
            return ImageProcesserType.Normal.ToString();
        }
    }
}
