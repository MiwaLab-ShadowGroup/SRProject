using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Data
{
    /// <summary>
    /// ドキュメントの基底クラス
    /// </summary>
    public abstract class ADocument
    {
        public abstract void CopyFrom(ADocument from);
        public abstract void Save(string documentName);
        public abstract void Load(string documentName);
    }
}
