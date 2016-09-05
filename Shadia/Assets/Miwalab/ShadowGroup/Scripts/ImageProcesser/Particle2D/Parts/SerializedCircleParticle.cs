using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.ImageProcesser.Particle2D
{
    public class SerializedCircleParticle : CircleParticle
    {
        public int FId { set; get; }

        public void SetRundomId(Random rand)
        {
            FId = rand.Next(0,10000);
        }
    }
}
