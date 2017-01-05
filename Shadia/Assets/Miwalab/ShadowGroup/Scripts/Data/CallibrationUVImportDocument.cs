using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Data
{
    public class CallibrationUVImportDocument : ADocument
    {
        public List<Vector2> ImportUVVector = new List<Vector2>();
        

        public override void CopyFrom(ADocument from)
        {
            ImportUVVector = new List<Vector2>((from as CallibrationUVImportDocument).ImportUVVector);
        }

        public override void Load(string documentName)
        {
            CopyFrom(Serialization.Xml.XmlSerializeHelper.Deserialize<CallibrationUVImportDocument>(documentName));
        }

        public override void Save(string documentName)
        {
            Serialization.Xml.XmlSerializeHelper.Seialize(documentName, this);
        }

        public void SetupField(ShadowMeshRenderer shadowmeshrenderer)
        {
            ImportUVVector.Clear();

            foreach(var p in shadowmeshrenderer.UVDiff)
            {
                this.ImportUVVector.Add(new Vector2(p.x,p.y));

            }

        }

        public void SetValue(ShadowMeshRenderer shadowmeshrenderer)
        {
            shadowmeshrenderer.UVDiff = ImportUVVector.ToArray();
        }
    }
}
