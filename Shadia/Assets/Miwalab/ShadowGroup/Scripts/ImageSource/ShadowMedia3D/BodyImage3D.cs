using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using OpenCvSharp.CPlusPlus;

public class BodyImage3D : MonoBehaviour
{
    MeshFilter _Mf;
    SkinnedMeshRenderer _Mr;
    Mesh _mesh;
    Vector3[] _Vertices;
    int[] _Triangles;

    private int _Height;
    private int _Width;

    private int _ActualWidth;

    public readonly int Division = 2;

    public Material material;

    // Use this for initialization
    void Start()
    {
        this.InitializeClasses();
        this.SetupTriangles();
        _mesh.vertices = _Vertices;
        _mesh.triangles = _Triangles;
        this._Mr.material = material;
        this._Mf.sharedMesh = _mesh;
        this._Mr.sharedMesh = _mesh;
    }

    private void SetupTriangles()
    {
        int i = 0;
        int k = 0;
        for (int h = 0; h < _Height - 1; ++h)
        {
            for (int w = 0; w < _Width - 1; ++w)
            {
                k = h * _Width + w;
                this._Triangles[i] = k;
                this._Triangles[i + 1] = k + 1;
                this._Triangles[i + 2] = k + _Width;

                this._Triangles[i + 3] = k + 1;
                this._Triangles[i + 4] = k + _Width + 1;
                this._Triangles[i + 5] = k + _Width;

                i += 6;
            }
        }
    }

    private void InitializeClasses()
    {
        _mesh = new Mesh();
        _Mf = this.gameObject.AddComponent<MeshFilter>();
        _Mr = this.gameObject.AddComponent<SkinnedMeshRenderer>();
        _Mr.updateWhenOffscreen = true;
        _Width = (512 / Division);
        _Height = (424 / Division);
        _ActualWidth = 512;
        _Vertices = new Vector3[_Height * _Width];
        _Triangles = new int[(_Width - 1) * (_Height - 1) * 2 * 3];

    }

    public void SetupVertices(Windows.Kinect.CameraSpacePoint[] positions)
    {
        int i1;
        int i2;
        for (int y = 0; y < this._Height; ++y)
        {
            for (int x = 0; x < this._Width; ++x)
            {
                i1 = x + _Width * y;
                i2 = x * Division + _ActualWidth * y * Division;
                _Vertices[i1].x = positions[i2].X;
                _Vertices[i1].y = positions[i2].Y;
                _Vertices[i1].z = positions[i2].Z;
            }
        }
        _mesh.vertices = _Vertices;
    }
    private void CopyMemory(IntPtr dst, IntPtr src, int size)
    {
        byte[] temp = new byte[size];
        Marshal.Copy(src, temp, 0, size);
        Marshal.Copy(temp, 0, dst, size);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
