using UnityEngine;
using System.Collections;
using OpenCvSharp.CPlusPlus;

[RequireComponent(typeof(Camera))]
public class CameraMatAttacher : MonoBehaviour
{
    public RenderTexture _RenderTexture;
    Texture2D _Texture;
    // Use this for initialization
    void Start()
    {
        
        _Texture = new Texture2D(512, 424, TextureFormat.RGB24, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attach(ref Mat mat)
    {
        RenderTexture.active = _RenderTexture;
        //don't forget that you need to specify rendertexture before you call readpixels
        //otherwise it will read screen pixels.
        _Texture.ReadPixels(new UnityEngine.Rect(0, 0, _RenderTexture.width, _RenderTexture.height), 0, 0);
        unsafe
        {
            var srcData = _Texture.GetRawTextureData();
            byte* dstData = (byte*)mat.Data;
            for (int i = 0; i < 512 * 424 * 3; ++i)
            {
                dstData[i] = srcData[512 * 424 * 3 - i-1];
            }
        }
        RenderTexture.active = null;
    }
}
