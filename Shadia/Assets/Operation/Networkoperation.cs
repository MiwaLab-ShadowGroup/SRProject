using Miwalab.ShadowGroup.Network;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkOperation : Miwalab.ShadowGroup.Operation.AOperationCommand
{

    public List<Object3DOperation> object3DOpe = new List<Object3DOperation>();

   
    public byte[] GetBinary()
    {
        MemoryStream ms = new MemoryStream();

        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(NetworkOperation));

        serializer.Serialize(ms,this);

        ms.Close();

        return ms.ToArray();
    }


    public void SetFromBinary(NetworkOperation nOpe)
    {
        MemoryStream ms = new MemoryStream();

        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(NetworkOperation));

        nOpe = (NetworkOperation)serializer.Deserialize(ms);

        ms.Close();
    }

}
