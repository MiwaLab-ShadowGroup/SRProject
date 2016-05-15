using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Thread
{
    public interface IThread
    {
        void Start();
        void Task();
        void Abort();
        bool IsFinished();
    }
}
