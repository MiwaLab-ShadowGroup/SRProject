using Miwalab.ShadowGroup.Network;
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

        public readonly int Row = 10;
        public readonly int Col = 10;

        private Mesh _mesh;
        private Vector3[] _vertices;
        private Vector2[] _uv;
        private int[] _triangles;


        MeshFilter _mF;
        MeshRenderer _mR;
        Material _material;
        Texture2D _mainTexture;

        object syncObject = new object();
        byte[] data;
        bool _isUpdated = false;
        byte[] meshData;

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
            CreateMesh(Row, Col);

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

                if (this._isUpdated)
                {
                    _mesh.vertices = _vertices;
                    _mesh.uv = _uv;
                    _mesh.RecalculateBounds();
                    this._isUpdated = false;
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
                this.data = data;
            }
        }

        public void SetupMesh(byte[] data)
        {
            lock (syncObject)
            {
                UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                dec.Source = data;
                for (int i = 0; i < (Row + 1) * (Col + 1); ++i)
                {
                    Vector3 pos = new Vector3();
                    Vector2 uv = new Vector2();

                    pos.x = dec.get_float();
                    pos.y = dec.get_float();
                    pos.z = dec.get_float();

                    uv.x = dec.get_float();
                    uv.y = dec.get_float();

                    this._vertices[i] = pos;
                    this._uv[i] = uv;
                }

                this._isUpdated = true;
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


        void CreateMesh(int width, int height)
        {
            int localWidth = width + 1;
            int localHeight = height + 1;

            _mesh = new Mesh();
            _mF.mesh = _mesh;

            _vertices = new Vector3[localWidth * localHeight];
            _uv = new Vector2[localWidth * localHeight];
            _triangles = new int[6 * ((localWidth - 1) * (localHeight - 1))];

            int triangleIndex = 0;
            for (int y = 0; y < localHeight; y++)
            {
                for (int x = 0; x < localWidth; x++)
                {
                    int index = (y * localWidth) + x;

                    _vertices[index] = new Vector3(x, -y, 0);
                    _uv[index] = new Vector2(((float)x / (float)localWidth), -((float)y / (float)localHeight));

                    // Skip the last row/col
                    if (x != (localWidth - 1) && y != (localHeight - 1))
                    {
                        int topLeft = index;
                        int topRight = topLeft + 1;
                        int bottomLeft = topLeft + localWidth;
                        int bottomRight = bottomLeft + 1;

                        _triangles[triangleIndex++] = topLeft;
                        _triangles[triangleIndex++] = topRight;
                        _triangles[triangleIndex++] = bottomLeft;
                        _triangles[triangleIndex++] = bottomLeft;
                        _triangles[triangleIndex++] = topRight;
                        _triangles[triangleIndex++] = bottomRight;

                    }
                }
            }

            _mesh.vertices = _vertices;
            _mesh.uv = _uv;
            _mesh.triangles = _triangles;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
    }
}
