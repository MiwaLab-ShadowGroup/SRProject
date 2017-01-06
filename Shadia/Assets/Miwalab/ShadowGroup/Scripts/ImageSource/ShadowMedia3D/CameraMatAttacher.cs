using UnityEngine;
using System.Collections;
using OpenCvSharp.CPlusPlus;
using System.Runtime.InteropServices;

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
        _Texture.Apply();
        var srcData = _Texture.GetRawTextureData();
        Marshal.Copy(srcData, 0, mat.Data, 512 * 424 * 3);
        mat = mat.CvtColor(OpenCvSharp.ColorConversion.RgbToBgr);
        Cv2.Flip(mat, mat, OpenCvSharp.FlipMode.XY);
        RenderTexture.active = null;
    }
}
