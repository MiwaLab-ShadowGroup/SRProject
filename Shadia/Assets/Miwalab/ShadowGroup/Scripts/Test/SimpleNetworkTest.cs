using UnityEngine;
using System.Collections;
using OpenCvSharp.CPlusPlus;
using Miwalab.ShadowGroup.Network;

[RequireComponent(typeof(RemoteShadowImageManager))]
public class SimpleNetworkTest : MonoBehaviour
{

    public Texture2D texture;

    RemoteShadowImageManager rsim;

    private Texture2D texToDraw;

    public Material TargetMaterial;

    // Use this for initialization
    void Start()
    {
        rsim = this.GetComponent<RemoteShadowImageManager>();

        rsim._SendMat =Mat.ImDecode (texture.GetRawTextureData());

        this.texToDraw = new Texture2D(1024,512);
        TargetMaterial.mainTexture = this.texToDraw;
    }

    // Update is called once per frame
    void Update()
    {
   
    }
}
