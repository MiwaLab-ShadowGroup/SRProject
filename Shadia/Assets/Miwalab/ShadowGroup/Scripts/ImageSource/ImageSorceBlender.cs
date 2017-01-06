using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace Miwalab.ShadowGroup.ImageSource
{

    [RequireComponent(typeof(MeshRenderer))]
    public class ImageSorceBlender : MonoBehaviour
    {

        private MeshRenderer _renderer;
        private Material _targetMaterial;

        public MatAttacher _matattacher { set; get; }

        #region unity functions
        public void Start()
        {
            //first attach
            _renderer = GetComponent<MeshRenderer>();
            _renderer.sharedMaterial = _renderer.material;
            _targetMaterial = _renderer.sharedMaterial;

            (ShadowMediaUIHost.GetUI("core_blending_mode") as ParameterDropdown).ValueChanged += ImageSorceBlender_ValueChanged;

        }

        private void ImageSorceBlender_ValueChanged(object sender, EventArgs e)
        {
            setBlendMode((e as ParameterDropdown.ChangedValue).Value);
            this._matattacher.setMaterial(this._targetMaterial);
        }

        public void Update()
        {

        }

        #endregion

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
                    _targetMaterial.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.Zero);
                    _targetMaterial.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case BlendMode.Additive:
                    _targetMaterial.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.One);
                    _targetMaterial.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case BlendMode.SoftAdditive:
                    _targetMaterial.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                    _targetMaterial.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.One);
                    break;
                case BlendMode.Substract:
                    _targetMaterial.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.One);
                    _targetMaterial.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                    break;
                case BlendMode.Multiply:
                    _targetMaterial.SetInt("_BlendModeSrc", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    _targetMaterial.SetInt("_BlendModeDst", (int)UnityEngine.Rendering.BlendMode.Zero);
                    break;
                default:
                    break;
            }
        }
        
    }
}
