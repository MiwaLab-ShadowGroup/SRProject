using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenCvSharp.CPlusPlus;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public class Normal : AShadowImageProcesser
    {
        private bool invert;
        public Normal()
            :base()
        {
            //UIからパラメータ変更通知の追加
            (UIHost.GetUI("Normal_Invert") as ParameterCheckbox).ValueChanged += Normal_ValueChanged;
            (UIHost.GetUI("Normal_Invert") as ParameterCheckbox).ValueUpdate();
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

        }
        public override string ToString()
        {
            return ImageProcesserType.Normal.ToString();
        }
    }
}
