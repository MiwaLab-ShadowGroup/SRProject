using UnityEngine;
using System.Collections;
using OpenCvSharp.CPlusPlus;

[RequireComponent(typeof(RemoteShadowImageManager))]
public class SimpleNetworkTest : MonoBehaviour
{

    public Texture2D texture;

    RemoteShadowImageManager rsim;


    // Use this for initialization
    void Start()
    {
        rsim = this.GetComponent<RemoteShadowImageManager>();

        rsim._SendMat =Mat.ImDecode (texture.GetRawTextureData());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
