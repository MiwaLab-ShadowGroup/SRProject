using Miwalab.ShadowGroup.Network;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkOperation : Miwalab.ShadowGroup.Operation.AOperationCommand
{

    public List<Object3DOperation> object3DOpe = new List<Object3DOperation>();

    MemoryStream ms;

    public byte[] data;
   
    public void ChangeToBinary()
    {
        ms = new MemoryStream();

        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(NetworkOperation));

        serializer.Serialize(ms,this);

        data = ms.ToArray();
    }

}
