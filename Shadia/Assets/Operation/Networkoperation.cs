using Miwalab.ShadowGroup.Network;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkOperation : Miwalab.ShadowGroup.Operation.AOperationCommand
{

    public List<Object3DOperation> _object3DOpe = new List<Object3DOperation>();

    private void CopyFrom(NetworkOperation from)
    {
        _object3DOpe.Clear();
        _object3DOpe = new List<Object3DOperation>(from._object3DOpe);
    }

    public byte[] GetBinary()
    {
        MemoryStream ms = new MemoryStream();

        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(NetworkOperation));

        serializer.Serialize(ms, this);
        byte[] data = ms.ToArray();
        ms.Close();

        return data;
    }


    public void SetFromBinary(byte[] data)
    {
        MemoryStream ms = new MemoryStream();
        ms.Write(data, 0, data.Length);
        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(NetworkOperation));

        this.CopyFrom((NetworkOperation)serializer.Deserialize(ms));

        ms.Close();
    }

    public void clear()
    {
        this._object3DOpe.Clear();
    }

    public void AddObject3DOpe(Object3DOperation object3DOpe)
    {
        this._object3DOpe.Add(object3DOpe);
    }

    public Object3DOperation getObject3DOpe(int index)
    {
        if (this._object3DOpe.Count > index)
        {

            return this._object3DOpe[index];
        }
        return null;
    }
}
