using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Miwalab.ShadowGroup.ImageProcesser
{
    public enum ParameterType
    {
        Float,
        Double,
        Int,
        UInt,
        Byte,
        String,
        Bool,
        Other
    }
    public class Parameter
    {
        public Parameter(ParameterType Type, string Name ,Object Value)
        {
            this.Type = Type;
            this.Value = Value;
            this.Name = Name;
        }
        public ParameterType Type { set; get; }
        public Object Value { set; get; }
        public string Name { set; get; }
    }

    public abstract class AImageProcesser
    {
        public abstract void ImageProcess(ref Mat src, ref Mat dst);
        
    }
    public abstract class AShadowImageProcesser : AImageProcesser
    {
        public abstract ImageProcesser.ImageProcesserType getImageProcesserType();

    }
}
