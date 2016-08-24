using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser.Particle2D
{
    public class CircleParticle : AParticle2D
    {

        public override void DrawShape(ref Mat mat)
        {
            if (this.Alive)
            {
                if (Size >= 0)
                {
                    point.X = (int)Position.x;
                    point.Y = (int)Position.y;
                    OpenCvSharp.CPlusPlus.Cv2.Circle(mat, point, (int)this.Size, Color, -1);
                }
            }
        }
    }
}
