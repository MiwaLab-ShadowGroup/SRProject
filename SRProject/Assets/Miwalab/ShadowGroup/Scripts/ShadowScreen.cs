using UnityEngine;
using System.Collections;

public class ShadowScreen : MonoBehaviour {

    //public
    public Vector3 topLeft;
    public Vector3 bottomLeft;
    public Vector3 bottomRight;
    public Vector3 topRight;

    public int Row = 4;
    public int Col = 4;

    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;

    // Use this for initialization
    void Start()
    {
        //メッシュを作る
        this.CreateMesh(this.Col, this.Row);
        //this.RefreshData();
    }

    // Update is called once per frame
    void Update()
    {
        //頂点変更

        //頂点に変更があったらメッシュ再構築
        this.RefreshData();
    }

    void CreateMesh(int width, int height)
    {
        int localWidth = width + 1;
        int localHeight = height + 1;

        _Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _Mesh;

        _Vertices = new Vector3[localWidth * localHeight];
        _UV = new Vector2[localWidth * localHeight];
        _Triangles = new int[6 * ((localWidth - 1) * (localHeight - 1))];

        int triangleIndex = 0;
        for (int y = 0; y < localHeight; y++)
        {
            for (int x = 0; x < localWidth; x++)
            {
                int index = (y * localWidth) + x;

                _Vertices[index] = new Vector3(x, -y, 0);
                _UV[index] = new Vector2(((float)x / (float)localWidth), -((float)y / (float)localHeight));

                // Skip the last row/col
                if (x != (localWidth - 1) && y != (localHeight - 1))
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + localWidth;
                    int bottomRight = bottomLeft + 1;

                    _Triangles[triangleIndex++] = topLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();
    }

    private void RefreshData()
    {
        int width = this.Col + 1;
        int height = this.Row + 1;

        Vector3 downVec_R = (this.bottomRight - this.topRight) / (this.Row);
        Vector3 downVec_L = (this.bottomLeft - this.topLeft) / (this.Row);

       // Debug.Log(downVec_R);
       // Debug.Log(downVec_L);

        Vector3 downVec_L_e = downVec_L / downVec_L.magnitude;

        for (int y = 0; y < height; y++)
        {
            Vector3 rightVec = ((this.topRight + downVec_R * y) - (this.topLeft + downVec_L * y)) / (this.Col);
            Vector3 rightVec_e = rightVec / rightVec.magnitude;

          //  Debug.Log(rightVec);

            for (int x = 0; x < width; x++)
            {
                //各頂点の座標算出
                int index = y * width + x;

                Vector3 pos = new Vector3();
                pos = this.topLeft + downVec_L * y + rightVec * x;
                _Vertices[index] = pos;

                //  Debug.Log(index.ToString() + ":" + pos);
                //_UV[index] = pos;// downVec_L_e * y + rightVec_e * x;

            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();
    }
}
