using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Xml.Serialization;

namespace Miwalab.ShadowGroup.Data
{
    /// <summary>
    /// すべてのパラメータ情報を保存するクラス
    /// </summary>
    [Serializable]
    public class UIParameterDocument : ADocument
    {
        
        public List<KeyAndValue<string, Single>> parameterSingle = new List<KeyAndValue<string, float>>();
        public List<KeyAndValue<string, Boolean>> parameterBoolean = new List<KeyAndValue<string, bool>>();
        public List<KeyAndValue<string, String>> parameterString = new List<KeyAndValue<string, string>>();



        public override void CopyFrom(ADocument from)
        {
            parameterSingle = new List<KeyAndValue<string, Single>>((from as UIParameterDocument).parameterSingle);
            parameterBoolean = new List<KeyAndValue<string, Boolean>>((from as UIParameterDocument).parameterBoolean);
            parameterString = new List<KeyAndValue<string, String>>((from as UIParameterDocument).parameterString);
        }

        public override void Load(string documentName)
        {
            CopyFrom(Serialization.Xml.XmlSerializeHelper.Deserialize<UIParameterDocument>(documentName));
        }

        public override void Save(string documentName)
        {
            Serialization.Xml.XmlSerializeHelper.Seialize(documentName, this);
        }


        public void SetupField(Dictionary<string, AParameterUI> parameterUI)
        {
            this.parameterBoolean.Clear();
            this.parameterSingle.Clear();
            this.parameterString.Clear();

            foreach(var p in parameterUI)
            {
                switch (p.Value.GetParameterType())
                {
                    case AParameterUI.ParameterType.Single:
                        this.parameterSingle.Add(new KeyAndValue<string, Single>(p.Key,(Single)p.Value.GetValue()));
                        break;
                    case AParameterUI.ParameterType.Boolean:
                        this.parameterBoolean.Add(new KeyAndValue<string, Boolean>(p.Key,(Boolean)p.Value.GetValue()));
                        break;
                    case AParameterUI.ParameterType.String:
                        this.parameterString.Add(new KeyAndValue<string, String>(p.Key,(String)p.Value.GetValue()));
                        break;
                }
                    
            }

        }

        public void SetParameter(Dictionary<string, AParameterUI> parameterUI)
        {
            foreach (var p in parameterUI)
            {
                switch (p.Value.GetParameterType())
                {
                    case AParameterUI.ParameterType.Single:
                        p.Value.SetValue(this.parameterSingle.Find(k => k.Key == p.Key).Value);
                        break;
                    case AParameterUI.ParameterType.Boolean:
                        p.Value.SetValue(this.parameterBoolean.Find(k => k.Key == p.Key).Value);
                        break;
                    case AParameterUI.ParameterType.String:
                        p.Value.SetValue(this.parameterString.Find(k => k.Key == p.Key).Value);
                        break;
                }

            }

        }


        [Serializable]
        public struct KeyAndValue<TKey, TValue>
        {
            public TKey Key;
            public TValue Value;

            public KeyAndValue(KeyValuePair<TKey, TValue> pair)
            {
                Key = pair.Key;
                Value = pair.Value;
            }
            public KeyAndValue(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
