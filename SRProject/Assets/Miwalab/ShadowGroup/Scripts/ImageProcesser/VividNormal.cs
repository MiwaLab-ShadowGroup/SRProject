using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class VividNormal : AImageProcesser
    {
        private bool invert;
        public VividNormal()
            : base()
        {
            //UIからパラメータ変更通知の追加
            (UIHost.GetUI("Normal_Invert") as ParameterCheckbox).ValueChanged += Normal_ValueChanged;
        }

        private void Normal_ValueChanged(object sender, EventArgs e)
        {
            invert = (e as ParameterCheckbox.ChangedValue).Value;
        }

        public override ImageProcesserType getImageProcesserType()
        {
            return ImageProcesserType.Normal;
        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            if (invert)
            {
                dst = ~src;
            }
            else
            {
                dst = src;
            }

            dst = dst.MedianBlur(3);
        }
        public override string ToString()
        {
            return ImageProcesserType.Normal.ToString();
        }
    }
}
