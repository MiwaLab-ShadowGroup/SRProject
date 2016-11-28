using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ShadowMeshRenderer : MonoBehaviour
{

    public int CallibNumber;

    //public
    [SerializeField, Space(15)]
    public Vector3 topLeft;
    public Vector3 bottomLeft;
    public Vector3 bottomRight;
    public Vector3 topRight;

    public Camera ProjectionCamera;

    public Vector3 src_topLeft;
    public Vector3 src_bottomLeft;
    public Vector3 src_bottomRight;
    public Vector3 src_topRight;

    public Vector3 topLeftofViewPort;
    public Vector3 bottomLeftofViewPort;
    public Vector3 bottomRightofViewPort;
    public Vector3 topRightofViewPort;

    public Vector3 debug_topLeft;
    public Vector3 debug_bottomLeft;
    public Vector3 debug_bottomRight;
    public Vector3 debug_topRight;

    public Vector3 Inpt_topLeft;
    public Vector3 Inpt_bottomLeft;
    public Vector3 Inpt_bottomRight;
    public Vector3 Inpt_topRight;

    public Vector3 Expt_topLeft;
    public Vector3 Expt_bottomLeft;
    public Vector3 Expt_bottomRight;
    public Vector3 Expt_topRight;

    private float dbgPlaneWidth;
    private float dbgPlaneHeight;

    public int Row = 10;
    public int Col = 10;

    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;


    public GameObject PointObjectDst;
    public GameObject PointObjectSrc;
    public Camera CameraSrc;
    private List<GameObject> PointObjectList;

    public GameObject debugPlane;

    // Use this for initialization
    public void SetUpUIs()
    {
        if (CallibNumber % 2 == 1)
        {
            (ShadowMediaUIHost.GetUI("clb_camera_mode" + 1) as ParameterDropdown).ValueChanged += ShadowMeshRenderer_cameramode_ValueChanged;
        }
        else
        {
            (ShadowMediaUIHost.GetUI("clb_camera_mode" + 2) as ParameterDropdown).ValueChanged += ShadowMeshRenderer_cameramode_ValueChanged;
        }



        //メッシュを作る
        this.CreateMesh(this.Col, this.Row);
        //this.RefreshData();

        (ShadowMediaUIHost.GetUI("Clb_I_TL_X" + CallibNumber) as ParameterSlider).ValueChanged += Clb_I_TL_XChanged;
        (ShadowMediaUIHost.GetUI("Clb_I_TL_Y" + CallibNumber) as ParameterSlider).ValueChanged += Clb_I_TL_YChanged;
        (ShadowMediaUIHost.GetUI("Clb_I_BL_X" + CallibNumber) as ParameterSlider).ValueChanged += Clb_I_BL_XChanged;
        (ShadowMediaUIHost.GetUI("Clb_I_BL_Y" + CallibNumber) as ParameterSlider).ValueChanged += Clb_I_BL_YChanged;
        (ShadowMediaUIHost.GetUI("Clb_I_BR_X" + CallibNumber) as ParameterSlider).ValueChanged += Clb_I_BR_XChanged;
        (ShadowMediaUIHost.GetUI("Clb_I_BR_Y" + CallibNumber) as ParameterSlider).ValueChanged += Clb_I_BR_YChanged;
        (ShadowMediaUIHost.GetUI("Clb_I_TR_X" + CallibNumber) as ParameterSlider).ValueChanged += Clb_I_TR_XChanged;
        (ShadowMediaUIHost.GetUI("Clb_I_TR_Y" + CallibNumber) as ParameterSlider).ValueChanged += Clb_I_TR_YChanged;


        (ShadowMediaUIHost.GetUI("Clb_I_Save" + CallibNumber) as ParameterButton).Clicked += Clb_I_Save_Clicked;
        (ShadowMediaUIHost.GetUI("Clb_I_Load" + CallibNumber) as ParameterButton).Clicked += Clb_I_Load_Clicked;



        (ShadowMediaUIHost.GetUI("Clb_E_TL_X" + CallibNumber) as ParameterSlider).ValueChanged += Clb_E_TL_XChanged;
        (ShadowMediaUIHost.GetUI("Clb_E_TL_Y" + CallibNumber) as ParameterSlider).ValueChanged += Clb_E_TL_YChanged;
        (ShadowMediaUIHost.GetUI("Clb_E_BL_X" + CallibNumber) as ParameterSlider).ValueChanged += Clb_E_BL_XChanged;
        (ShadowMediaUIHost.GetUI("Clb_E_BL_Y" + CallibNumber) as ParameterSlider).ValueChanged += Clb_E_BL_YChanged;
        (ShadowMediaUIHost.GetUI("Clb_E_BR_X" + CallibNumber) as ParameterSlider).ValueChanged += Clb_E_BR_XChanged;
        (ShadowMediaUIHost.GetUI("Clb_E_BR_Y" + CallibNumber) as ParameterSlider).ValueChanged += Clb_E_BR_YChanged;
        (ShadowMediaUIHost.GetUI("Clb_E_TR_X" + CallibNumber) as ParameterSlider).ValueChanged += Clb_E_TR_XChanged;
        (ShadowMediaUIHost.GetUI("Clb_E_TR_Y" + CallibNumber) as ParameterSlider).ValueChanged += Clb_E_TR_YChanged;
        (ShadowMediaUIHost.GetUI("Clb_E_Vsbl" + CallibNumber) as ParameterCheckbox).ValueChanged += Clb_E_VsblChanged;


        (ShadowMediaUIHost.GetUI("Clb_E_Save" + CallibNumber) as ParameterButton).Clicked += Clb_E_Save_Clicked;
        (ShadowMediaUIHost.GetUI("Clb_E_Load" + CallibNumber) as ParameterButton).Clicked += Clb_E_Load_Clicked;

        //pointobject
        this.PointObjectList = new List<GameObject>();
        for (int i = 0; i < 4; ++i)
        {
            var item = Instantiate(PointObjectDst);
            item.transform.SetParent(this.gameObject.transform);
            PointObjectList.Add(item);
        }

        for (int i = 0; i < 4; ++i)
        {
            var item = Instantiate(PointObjectSrc);
            item.transform.SetParent(this.gameObject.transform);
            PointObjectList.Add(item);
        }

        (ShadowMediaUIHost.GetUI("Clb_I_TL_X" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_I_TL_Y" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_I_BL_X" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_I_BL_Y" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_I_BR_X" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_I_BR_Y" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_I_TR_X" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_I_TR_Y" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_E_TL_X" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_E_TL_Y" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_E_BL_X" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_E_BL_Y" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_E_BR_X" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_E_BR_Y" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_E_TR_X" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_E_TR_Y" + CallibNumber) as ParameterSlider).ValueUpdate();
        (ShadowMediaUIHost.GetUI("Clb_E_Vsbl" + CallibNumber) as ParameterCheckbox).ValueUpdate();
    }

    private void ShadowMeshRenderer_cameramode_ValueChanged(object sender, EventArgs e)
    {
        switch ((Miwalab.ShadowGroup.Scripts.Callibration.ProjectionCameraMode)(e as ParameterDropdown.ChangedValue).Value)
        {
            case Miwalab.ShadowGroup.Scripts.Callibration.ProjectionCameraMode.Orthographic:
                ProjectionCamera.orthographic = true;
                break;
            case Miwalab.ShadowGroup.Scripts.Callibration.ProjectionCameraMode.Perthpective:
                ProjectionCamera.orthographic = false;
                break;
        }



    }

    private void Clb_E_Load_Clicked(object sender, EventArgs e)
    {
        string str = "";
        OpenFileDialog.OpenFileDialog.Read(ref str);
        if (str == "")
        {
            return;
        }
        Miwalab.ShadowGroup.Data.CallibExportDocument CED = new Miwalab.ShadowGroup.Data.CallibExportDocument();
        CED.Load(str);
        var values = CED.GetFloatValues() as List<float>;

        (ShadowMediaUIHost.GetUI("Clb_E_TL_X" + CallibNumber) as ParameterSlider).m_slider.value = values[0];
        (ShadowMediaUIHost.GetUI("Clb_E_TL_Y" + CallibNumber) as ParameterSlider).m_slider.value = values[1];
        (ShadowMediaUIHost.GetUI("Clb_E_BL_X" + CallibNumber) as ParameterSlider).m_slider.value = values[2];
        (ShadowMediaUIHost.GetUI("Clb_E_BL_Y" + CallibNumber) as ParameterSlider).m_slider.value = values[3];
        (ShadowMediaUIHost.GetUI("Clb_E_BR_X" + CallibNumber) as ParameterSlider).m_slider.value = values[4];
        (ShadowMediaUIHost.GetUI("Clb_E_BR_Y" + CallibNumber) as ParameterSlider).m_slider.value = values[5];
        (ShadowMediaUIHost.GetUI("Clb_E_TR_X" + CallibNumber) as ParameterSlider).m_slider.value = values[6];
        (ShadowMediaUIHost.GetUI("Clb_E_TR_Y" + CallibNumber) as ParameterSlider).m_slider.value = values[7];

    }

    private void Clb_I_Load_Clicked(object sender, EventArgs e)
    {
        string str = "";
        OpenFileDialog.OpenFileDialog.Read(ref str);
        if (str == "")
        {
            return;
        }

        Miwalab.ShadowGroup.Data.CallibImportDocument CID = new Miwalab.ShadowGroup.Data.CallibImportDocument();
        CID.Load(str);
        var values = CID.GetFloatValues() as List<float>;

        (ShadowMediaUIHost.GetUI("Clb_I_TL_X" + CallibNumber) as ParameterSlider).m_slider.value = values[0];
        (ShadowMediaUIHost.GetUI("Clb_I_TL_Y" + CallibNumber) as ParameterSlider).m_slider.value = values[1];
        (ShadowMediaUIHost.GetUI("Clb_I_BL_X" + CallibNumber) as ParameterSlider).m_slider.value = values[2];
        (ShadowMediaUIHost.GetUI("Clb_I_BL_Y" + CallibNumber) as ParameterSlider).m_slider.value = values[3];
        (ShadowMediaUIHost.GetUI("Clb_I_BR_X" + CallibNumber) as ParameterSlider).m_slider.value = values[4];
        (ShadowMediaUIHost.GetUI("Clb_I_BR_Y" + CallibNumber) as ParameterSlider).m_slider.value = values[5];
        (ShadowMediaUIHost.GetUI("Clb_I_TR_X" + CallibNumber) as ParameterSlider).m_slider.value = values[6];
        (ShadowMediaUIHost.GetUI("Clb_I_TR_Y" + CallibNumber) as ParameterSlider).m_slider.value = values[7];
    }

    private void Clb_E_Save_Clicked(object sender, EventArgs e)
    {
        string str = "";
        OpenFileDialog.OpenFileDialog.Save(ref str);
        if (str == "")
        {
            return;
        }

        Regex reg = new Regex(@"\.CED$");
        if (!reg.IsMatch(str))
        {
            str += ".CED";
        }

        float[] data = new float[8];

        data[0] = (ShadowMediaUIHost.GetUI("Clb_E_TL_X" + CallibNumber) as ParameterSlider).m_slider.value;
        data[1] = (ShadowMediaUIHost.GetUI("Clb_E_TL_Y" + CallibNumber) as ParameterSlider).m_slider.value;
        data[2] = (ShadowMediaUIHost.GetUI("Clb_E_BL_X" + CallibNumber) as ParameterSlider).m_slider.value;
        data[3] = (ShadowMediaUIHost.GetUI("Clb_E_BL_Y" + CallibNumber) as ParameterSlider).m_slider.value;
        data[4] = (ShadowMediaUIHost.GetUI("Clb_E_BR_X" + CallibNumber) as ParameterSlider).m_slider.value;
        data[5] = (ShadowMediaUIHost.GetUI("Clb_E_BR_Y" + CallibNumber) as ParameterSlider).m_slider.value;
        data[6] = (ShadowMediaUIHost.GetUI("Clb_E_TR_X" + CallibNumber) as ParameterSlider).m_slider.value;
        data[7] = (ShadowMediaUIHost.GetUI("Clb_E_TR_Y" + CallibNumber) as ParameterSlider).m_slider.value;

        Miwalab.ShadowGroup.Data.CallibExportDocument CED = new Miwalab.ShadowGroup.Data.CallibExportDocument();
        CED.SetFloatValues(
            new float[] {
                data[0],data[1],
                data[2],data[3],
                data[4],data[5],
                data[6],data[7],
            });
        CED.Save(str);

    }

    private void Clb_I_Save_Clicked(object sender, EventArgs e)
    {
        string str = "";
        OpenFileDialog.OpenFileDialog.Save(ref str);
        if (str == "")
        {
            return;
        }
        Regex reg = new Regex(@"\.CID$");
        if (!reg.IsMatch(str))
        {
            str += ".CID";
        }

        float[] data = new float[8];

        data[0] = (ShadowMediaUIHost.GetUI("Clb_I_TL_X" + CallibNumber) as ParameterSlider).m_slider.value;
        data[1] = (ShadowMediaUIHost.GetUI("Clb_I_TL_Y" + CallibNumber) as ParameterSlider).m_slider.value;
        data[2] = (ShadowMediaUIHost.GetUI("Clb_I_BL_X" + CallibNumber) as ParameterSlider).m_slider.value;
        data[3] = (ShadowMediaUIHost.GetUI("Clb_I_BL_Y" + CallibNumber) as ParameterSlider).m_slider.value;
        data[4] = (ShadowMediaUIHost.GetUI("Clb_I_BR_X" + CallibNumber) as ParameterSlider).m_slider.value;
        data[5] = (ShadowMediaUIHost.GetUI("Clb_I_BR_Y" + CallibNumber) as ParameterSlider).m_slider.value;
        data[6] = (ShadowMediaUIHost.GetUI("Clb_I_TR_X" + CallibNumber) as ParameterSlider).m_slider.value;
        data[7] = (ShadowMediaUIHost.GetUI("Clb_I_TR_Y" + CallibNumber) as ParameterSlider).m_slider.value;

        Miwalab.ShadowGroup.Data.CallibImportDocument CID = new Miwalab.ShadowGroup.Data.CallibImportDocument();
        CID.SetFloatValues(
            new float[] {
                data[0],data[1],
                data[2],data[3],
                data[4],data[5],
                data[6],data[7],
            });
        CID.Save(str);
    }

    private void Clb_E_VsblChanged(object sender, EventArgs e)
    {
        var visible = (e as ParameterCheckbox.ChangedValue).Value;
        for (int i = 0; i < 8; ++i)
        {
            this.PointObjectList[i].gameObject.SetActive(visible);
        }
    }

    private void Clb_E_TR_YChanged(object sender, EventArgs e)
    {
        this.topRightofViewPort.y = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_E_BR_YChanged(object sender, EventArgs e)
    {
        this.bottomRightofViewPort.y = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_E_TR_XChanged(object sender, EventArgs e)
    {
        this.topRightofViewPort.x = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();

    }

    private void Clb_E_BR_XChanged(object sender, EventArgs e)
    {
        this.bottomRightofViewPort.x = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_E_BL_YChanged(object sender, EventArgs e)
    {
        this.bottomLeftofViewPort.y = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_E_BL_XChanged(object sender, EventArgs e)
    {
        this.bottomLeftofViewPort.x = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_E_TL_XChanged(object sender, EventArgs e)
    {
        this.topLeftofViewPort.x = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_E_TL_YChanged(object sender, EventArgs e)
    {
        this.topLeftofViewPort.y = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_I_TR_YChanged(object sender, EventArgs e)
    {
        this.src_topRight.y = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_I_TR_XChanged(object sender, EventArgs e)
    {
        this.src_topRight.x = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_I_BR_YChanged(object sender, EventArgs e)
    {
        this.src_bottomRight.y = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();

    }

    private void Clb_I_BR_XChanged(object sender, EventArgs e)
    {
        this.src_bottomRight.x = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_I_TL_XChanged(object sender, EventArgs e)
    {
        this.src_topLeft.x = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_I_TL_YChanged(object sender, EventArgs e)
    {
        this.src_topLeft.y = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();

    }

    private void Clb_I_BL_XChanged(object sender, EventArgs e)
    {
        this.src_bottomLeft.x = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void Clb_I_BL_YChanged(object sender, EventArgs e)
    {
        this.src_bottomLeft.y = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    // Update is called once per frame
    void UpdatePos()
    {
        //頂点変更
        topLeft = ProjectionCamera.ScreenToWorldPoint(topLeftofViewPort);
        bottomLeft = ProjectionCamera.ScreenToWorldPoint(bottomLeftofViewPort);
        bottomRight = ProjectionCamera.ScreenToWorldPoint(bottomRightofViewPort);
        topRight = ProjectionCamera.ScreenToWorldPoint(topRightofViewPort);

        //エクスポート側　こっちはあってる
        this.PointObjectList[0].transform.position = topLeft;
        this.PointObjectList[1].transform.position = bottomLeft;
        this.PointObjectList[2].transform.position = bottomRight;
        this.PointObjectList[3].transform.position = topRight;

        //インポート側

        this.dbgPlaneWidth = this.debugPlane.transform.lossyScale.x * 10;
        this.dbgPlaneHeight = this.debugPlane.transform.lossyScale.y * 10;

        this.Inpt_topLeft.x = this.dbgPlaneWidth / 2 * (src_topLeft.x * -2 + 1);
        this.Inpt_topLeft.y = this.dbgPlaneHeight / 2 * (src_topLeft.y * 2 - 1);
        this.Inpt_topLeft.z = this.debugPlane.transform.position.z;
        this.Inpt_bottomLeft.x = this.dbgPlaneWidth / 2 * (src_bottomLeft.x * -2 + 1);
        this.Inpt_bottomLeft.y = this.dbgPlaneHeight / 2 * (src_bottomLeft.y * 2 - 1);
        this.Inpt_bottomLeft.z = this.debugPlane.transform.position.z;
        this.Inpt_bottomRight.x = this.dbgPlaneWidth / 2 * (src_bottomRight.x * -2 + 1);
        this.Inpt_bottomRight.y = this.dbgPlaneHeight / 2 * (src_bottomRight.y * 2 - 1);
        this.Inpt_bottomRight.z = this.debugPlane.transform.position.z; ;
        this.Inpt_topRight.x = this.dbgPlaneWidth / 2 * (src_topRight.x * -2 + 1);
        this.Inpt_topRight.y = this.dbgPlaneHeight / 2 * (src_topRight.y * 2 - 1);
        this.Inpt_topRight.z = this.debugPlane.transform.position.z;


        this.PointObjectList[4].transform.position = this.Inpt_topLeft;
        this.PointObjectList[5].transform.position = this.Inpt_bottomLeft;
        this.PointObjectList[6].transform.position = this.Inpt_bottomRight;
        this.PointObjectList[7].transform.position = this.Inpt_topRight;



        //頂点に変更があったらメッシュ再構築
        this.RefreshData();
    }

    private Vector3 ConvertUVPointToScreenPoint(Vector3 point)
    {
        return new Vector3(point.x * Screen.width, point.y * Screen.height, 1);
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
        _Mesh.RecalculateBounds();
    }

    private void RefreshData()
    {
        int width = this.Col + 1;
        int height = this.Row + 1;




        Vector3 downVec_R = (this.bottomRight - this.topRight) / (this.Row);
        Vector3 downVec_L = (this.bottomLeft - this.topLeft) / (this.Row);

        Vector3 UV_downVec_R = (this.src_bottomRight - this.src_topRight);
        Vector3 UV_downVec_L = (this.src_bottomLeft - this.src_topLeft);

        //Debug.Log(downVec_R);
        // \Debug.Log(downVec_L);

        //Vector3 downVec_L_e = downVec_L / downVec_L.magnitude;

        for (int y = 0; y < height; y++)
        {
            Vector3 rightVec = ((this.topRight + downVec_R * y) - (this.topLeft + downVec_L * y)) / (this.Col);
            //Vector3 rightVec_e = rightVec / rightVec.magnitude;

            Vector3 UV_rightVec = ((this.src_topRight + UV_downVec_R * y / this.Row) - (this.src_topLeft + UV_downVec_L * y / this.Row));

            //  Debug.Log(rightVec);

            for (int x = 0; x < width; x++)
            {
                //各頂点の座標算出
                int index = y * width + x;

                Vector3 pos = new Vector3();
                pos = this.topLeft + downVec_L * y + rightVec * x;
                _Vertices[index] = pos;

                //Debug.Log(index.ToString() + ":" + pos);
                //_UV[index] = downVec_L_e * y / this.Row + rightVec_e * x / this.Col; //this.topLeft + downVec_L_e * y + rightVec_e * x;
                //_UV[index] = new Vector2(((float)x / (float)width), -((float)y / (float)height)) ;
                _UV[index] = (this.src_topLeft + UV_downVec_L * y / this.Col + UV_rightVec * x / this.Row);


            }
        }



        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();
        _Mesh.RecalculateBounds();
    }
}
