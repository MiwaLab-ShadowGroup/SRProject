using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Data
{
    [Serializable]
    public class CallibImportDocument : ACallibDataDocument
    {
        [SerializeField]
        private List<Vector3> m_PointList;

        public CallibImportDocument()
        {
            m_PointList = new List<Vector3>();

        }

        public override void SetPoints(IEnumerable<Vector3> points)
        {
            this.m_PointList.Clear();

            foreach (var point in points)
            {
                this.m_PointList.Add(point);
            }
        }
        public override void CopyFrom(ADocument from)
        {
            var CallibData = from as CallibImportDocument;

            m_PointList = CallibData.m_PointList;
        }

        public override void Save(string documentName)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter(documentName);
            sw.WriteLine(JsonUtility.ToJson(this));
        }
        public override void Load(string documentName)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(documentName);
            this.CopyFrom(JsonUtility.FromJson<CallibImportDocument>(sr.ReadLine()));

        }
    }
}
