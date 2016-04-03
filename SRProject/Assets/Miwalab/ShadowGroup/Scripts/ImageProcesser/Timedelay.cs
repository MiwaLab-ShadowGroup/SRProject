using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Timedelay : AImageProcesser
    {
        private int DelayCounter;

        int count = 0;

        Queue<Mat> queue;


        public Timedelay()
            :base()
        {

            DelayCounter =100;
            //データの登録

            this.queue = new Queue<Mat>();


        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        

        private void Update(ref Mat src, ref Mat dst)
        {
            Mat item = new Mat();
            src.CopyTo(item);


            this.queue.Enqueue(item);
            count++;

            if (count > this.DelayCounter) //この値で遅れ時間を調整(UIで変えられる)
            {
                this.queue.Dequeue().CopyTo(dst);
                
            }

        }
        public override string ToString()
        {
            return "Timedelay";
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.TimeDelay;
        }

        public bool IsFirstFrame { get; private set; }

    }
}
