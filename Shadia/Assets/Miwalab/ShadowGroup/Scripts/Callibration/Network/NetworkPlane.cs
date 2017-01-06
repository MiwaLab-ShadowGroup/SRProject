using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Callibration.Network
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class NetworkPlane : MonoBehaviour
    {
        public string MyTag;
        public Shader _DoubleSideShader;
        public MatAttacher _matAttacher;


        MeshFilter _mF;
        MeshRenderer _mR;
        Material _material;
        Texture2D _mainTexture;

        object syncObject = new object();
        byte[] data;

        public void Initialzie(MatAttacher matAttacher, Shader doubleSideShader)
        {
            _matAttacher = matAttacher;
            _DoubleSideShader = doubleSideShader;

            Debug.Assert(_DoubleSideShader != null);
            Debug.Assert(_matAttacher != null);


            _mainTexture = new Texture2D((int)_matAttacher.textureSize.x, (int)_matAttacher.textureSize.y);
            _mF = GetComponent<MeshFilter>();
            _mR = GetComponent<MeshRenderer>();
            _material = new Material(_DoubleSideShader);
            _mR.sharedMaterial = _material;
            _material.mainTexture = this._mainTexture;

            (ShadowMediaUIHost.GetUI("core_network_blend") as ParameterDropdown).ValueChanged += ImageSorceBlender_ValueChanged;


        }
        public void Update()
        {
            lock (syncObject)
            {
                if (data != null)
                {
                    _mainTexture.LoadImage(data);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">".PNG"形式</param>
        public void SetupTexture(byte[] data)
        {
            lock (syncObject)
            {
                if (this.data == null)
                {
                    this.data = new byte[data.Length];
                }
                data.CopyTo(this.data, 0);
            }
        }




        private void ImageSorceBlender_ValueChanged(object sender, EventArgs e)
        {
            setBlendMode((e as ParameterDropdown.ChangedValue).Value);
        }


        public enum BlendMode : int
        {
            //デフォルト
            Normal = 0,
            Additive = 1,
            SoftAdditive = 2,
            Substract = 3,
            Multiply = 4
        }

        public void setBlendMode(int num)
        {
            setBlendModeSettings((BlendMode)num);
        }

        private void setBlendModeSettings(BlendMode mode)
        {
            switch (mode)
            {
                case BlendMode.Normal:
                    _material.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.One);
                    _material.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                case BlendMode.Additive:
                    _material.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.One);
                    _material.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case BlendMode.SoftAdditive:
                    _material.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                    _material.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case BlendMode.Substract:
                    _material.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.One);
                    _material.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                    break;
                case BlendMode.Multiply:
                    _material.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    _material.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                default:
                    break;
            }
        }
    }
}
