using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Data
{
    [Serializable]
    public class CallibExportDocument : ACallibDataDocument
    {
        [SerializeField]
        private List<float> m_floatList;

        public CallibExportDocument()
        {
            m_floatList= new List<float>();

        }

        public override void SetFloatValues(IEnumerable<float> values)
        {
            this.m_floatList.Clear();

            foreach (var point in values)
            {
                this.m_floatList.Add(point);
            }
        }

        public override IEnumerable<float> GetFloatValues()
        {
            return this.m_floatList;
        }


        public override void CopyFrom(ADocument from)
        {
            var CallibData = from as CallibExportDocument;

            m_floatList = CallibData.m_floatList;
        }

        public override void Save(string documentName)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(documentName);
            sw.WriteLine(JsonUtility.ToJson(this,true));
            sw.Close();

        }
        public override void Load(string documentName)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(documentName);
            this.CopyFrom(JsonUtility.FromJson<CallibExportDocument>(sr.ReadToEnd()));
            sr.Close();

        }
    }
}
