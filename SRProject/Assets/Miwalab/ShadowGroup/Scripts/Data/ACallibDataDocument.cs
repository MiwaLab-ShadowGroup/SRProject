using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Data
{
    public abstract class ACallibDataDocument : ADocument
    {
        public abstract void SetFloatValues(IEnumerable<float> points);
        public abstract IEnumerable<float> GetFloatValues();
    }
}
