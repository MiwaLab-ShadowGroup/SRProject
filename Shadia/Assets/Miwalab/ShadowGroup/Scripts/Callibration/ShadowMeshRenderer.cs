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

    public Vector2 src_topLeft;
    public Vector2 src_bottomLeft;
    public Vector2 src_bottomRight;
    public Vector2 src_topRight;

    public Vector3 topLeftofViewPort;
    public Vector3 bottomLeftofViewPort;
    public Vector3 bottomRightofViewPort;
    public Vector3 topRightofViewPort;

    public Vector3 Inpt_topLeft;
    public Vector3 Inpt_bottomLeft;
    public Vector3 Inpt_bottomRight;
    public Vector3 Inpt_topRight;


    public float dbgPlaneWidth;
    public float dbgPlaneHeight;

    public int Row = 10;
    public int Col = 10;

    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;

    public GameObject PointObjectDst;
    public GameObject PointObjectSrc;
    public Camera CameraSrc;
    private List<GameObject> PointObjectDstList;
    private List<GameObject> PointObjectSrcList;

    public GameObject debugPlane;

    public enum PlaneMode
    {
        Plane,
        Cylinder,
        PinePlane,
    }
    public enum ImportMode
    {
        Quadrangle,
        Curve,
    }

    PlaneMode _PlaneMode;
    ImportMode _ImportMode;

    float _Radius;
    float _Height;
    float _InnerRadius;

    float _AngleStart;
    float _AngleFinish;

    float _CurveRTop;
    bool _UseUpperTop;
    float _CurveRBtm;
    bool _UseUpperBtm;



    // Use this for initialization
    public void SetUpUIs()
    {
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

        (ShadowMediaUIHost.GetUI("clb_i_import_mode" + CallibNumber) as ParameterDropdown).ValueChanged += clb_i_import_mode_changed;

        (ShadowMediaUIHost.GetUI("clb_i_r_top" + CallibNumber) as ParameterSlider).ValueChanged += clb_i_r_topChanged;
        (ShadowMediaUIHost.GetUI("clb_i_use_up_top" + CallibNumber) as ParameterCheckbox).ValueChanged += clb_i_use_up_topChanged;
        (ShadowMediaUIHost.GetUI("clb_i_r_btm" + CallibNumber) as ParameterSlider).ValueChanged += clb_i_r_btmChanged;
        (ShadowMediaUIHost.GetUI("clb_i_use_up_btm" + CallibNumber) as ParameterCheckbox).ValueChanged += clb_i_use_up_btmChanged;



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

        (ShadowMediaUIHost.GetUI("clb_e_pos_x" + CallibNumber) as ParameterSlider).ValueChanged += clb_e_pos_xChanged;
        (ShadowMediaUIHost.GetUI("clb_e_pos_y" + CallibNumber) as ParameterSlider).ValueChanged += clb_e_pos_yChanged;
        (ShadowMediaUIHost.GetUI("clb_e_pos_z" + CallibNumber) as ParameterSlider).ValueChanged += clb_e_pos_zChanged;

        (ShadowMediaUIHost.GetUI("clb_e_radius" + CallibNumber) as ParameterSlider).ValueChanged += clb_e_radiusChanged;
        (ShadowMediaUIHost.GetUI("clb_e_height" + CallibNumber) as ParameterSlider).ValueChanged += clb_e_heightChanged;
        (ShadowMediaUIHost.GetUI("clb_e_startAngle" + CallibNumber) as ParameterSlider).ValueChanged += clb_e_startAngleChanged;
        (ShadowMediaUIHost.GetUI("clb_e_finishAngle" + CallibNumber) as ParameterSlider).ValueChanged += clb_e_finishAngleChanged;

        (ShadowMediaUIHost.GetUI("clb_e_in_rad" + CallibNumber) as ParameterSlider).ValueChanged += clb_e_in_radChanged;


        (ShadowMediaUIHost.GetUI("Clb_E_Save" + CallibNumber) as ParameterButton).Clicked += Clb_E_Save_Clicked;
        (ShadowMediaUIHost.GetUI("Clb_E_Load" + CallibNumber) as ParameterButton).Clicked += Clb_E_Load_Clicked;

        (ShadowMediaUIHost.GetUI("clb_e_mode" + CallibNumber) as ParameterDropdown).ValueChanged += clb_e_mode_changed;

        //pointobject
        this.PointObjectDstList = new List<GameObject>();
        this.PointObjectSrcList = new List<GameObject>();
        for (int i = 0; i < (Row + 1) * (Col + 1); ++i)
        {
            var item = Instantiate(PointObjectDst);
            item.transform.SetParent(this.gameObject.transform);
            PointObjectDstList.Add(item);
        }

        for (int i = 0; i < (Row + 1) * (Col + 1); ++i)
        {
            var item = Instantiate(PointObjectSrc);
            item.transform.SetParent(debugPlane.transform);
            PointObjectSrcList.Add(item);
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

    private void clb_i_use_up_btmChanged(object sender, EventArgs e)
    {
        this._UseUpperBtm = (e as ParameterCheckbox.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_i_r_btmChanged(object sender, EventArgs e)
    {
        this._CurveRBtm = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_i_use_up_topChanged(object sender, EventArgs e)
    {
        this._UseUpperTop = (e as ParameterCheckbox.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_i_r_topChanged(object sender, EventArgs e)
    {
        this._CurveRTop = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_e_in_radChanged(object sender, EventArgs e)
    {
        this._InnerRadius = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_e_heightChanged(object sender, EventArgs e)
    {
        this._Height = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_e_finishAngleChanged(object sender, EventArgs e)
    {
        this._AngleFinish = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_e_startAngleChanged(object sender, EventArgs e)
    {
        this._AngleStart = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_e_radiusChanged(object sender, EventArgs e)
    {
        this._Radius = (e as ParameterSlider.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_e_pos_zChanged(object sender, EventArgs e)
    {
        var pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, pos.y, (e as ParameterSlider.ChangedValue).Value);
        UpdatePos();
    }

    private void clb_e_pos_yChanged(object sender, EventArgs e)
    {
        var pos = this.transform.position;
        this.transform.position = new Vector3(pos.x, (e as ParameterSlider.ChangedValue).Value, pos.z);
        UpdatePos();
    }

    private void clb_e_pos_xChanged(object sender, EventArgs e)
    {
        var pos = this.transform.position;
        this.transform.position = new Vector3((e as ParameterSlider.ChangedValue).Value, pos.y, pos.z);
        UpdatePos();
    }

    private void clb_i_import_mode_changed(object sender, EventArgs e)
    {
        _ImportMode = (ImportMode)(e as ParameterDropdown.ChangedValue).Value;
        UpdatePos();
    }

    private void clb_e_mode_changed(object sender, EventArgs e)
    {
        _PlaneMode = (PlaneMode)(e as ParameterDropdown.ChangedValue).Value;
        UpdatePos();
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
        for (int i = 0; i < (Row + 1) * (Col + 1); ++i)
        {
            this.PointObjectSrcList[i].gameObject.SetActive(visible);
            this.PointObjectDstList[i].gameObject.SetActive(visible);
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
        topLeft.z = 0;
        bottomLeft.z = 0;
        bottomRight.z = 0;
        topRight.z = 0;


        //インポート側
        this.dbgPlaneWidth = this.debugPlane.transform.lossyScale.x * 10;
        this.dbgPlaneHeight = this.debugPlane.transform.lossyScale.y * 10;


        //頂点に変更があったらメッシュ再構築
        this.RefreshData();
    }

    private Vector3 getInptPosition(Vector3 src)
    {
        Vector3 temp = new Vector3();
        temp.x = this.dbgPlaneWidth / 2 * (src.x * -2 + 1);
        temp.y = this.dbgPlaneHeight / 2 * (src.y * 2 - 1);
        temp.z = this.debugPlane.transform.position.z;
        return temp;
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
        switch (this._PlaneMode)
        {
            case PlaneMode.Plane:
                this.UpdateVerticesPlane();
                break;
            case PlaneMode.Cylinder:
                this.UpdateVerticesCylinder();
                break;
            case PlaneMode.PinePlane:
                this.UpdateVerticesPinePlane();
                break;
        }
        switch (this._ImportMode)
        {
            case ImportMode.Quadrangle:
                this.UpdateUVsQuadrangle();
                break;
            case ImportMode.Curve:
                this.UpdateUVsCurve();
                break;
        }

        int i = 0;
        foreach (var p in this._Vertices)
        {
            this.PointObjectDstList[i].transform.localPosition = p;
            this.PointObjectSrcList[i].transform.position = this.getInptPosition(this._UV[i]);
            ++i;
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();
        _Mesh.RecalculateBounds();
    }



    private void UpdateUVsQuadrangle()
    {

        int width = this.Col + 1;
        int height = this.Row + 1;
        Vector2 UV_downVec_R = (this.src_bottomRight - this.src_topRight);
        Vector2 UV_downVec_L = (this.src_bottomLeft - this.src_topLeft);

        for (int y = 0; y < height; y++)
        {
            Vector2 UV_rightVec = ((this.src_topRight + UV_downVec_R * y / this.Row) - (this.src_topLeft + UV_downVec_L * y / this.Row));
            for (int x = 0; x < width; x++)
            {
                _UV[y * width + x] = (this.src_topLeft + UV_downVec_L * y / this.Col + UV_rightVec * x / this.Row);
            }
        }

    }

    private void UpdateVerticesPlane()
    {
        int width = this.Col + 1;
        int height = this.Row + 1;
        Vector3 downVec_R = (this.bottomRight - this.topRight) / (this.Row);
        Vector3 downVec_L = (this.bottomLeft - this.topLeft) / (this.Row);

        for (int y = 0; y < height; y++)
        {
            Vector3 rightVec = ((this.topRight + downVec_R * y) - (this.topLeft + downVec_L * y)) / (this.Col);
            for (int x = 0; x < width; x++)
            {
                _Vertices[y * width + x] = this.topLeft + downVec_L * y + rightVec * x;
            }
        }
    }
    private void UpdateUVsCurve()
    {
        int width = this.Col + 1;
        int height = this.Row + 1;

        Vector2 CenterTop1 = new Vector2();
        Vector2 CenterTop2 = new Vector2();
        Vector2 CenterBtm1 = new Vector2();
        Vector2 CenterBtm2 = new Vector2();
        //上
        if (!GetCircleOf2PTAndR(this.src_topLeft, this.src_topRight, _CurveRTop, ref CenterTop1, ref CenterTop2))
        {
            UpdateUVsQuadrangle();
            return;
        }

        if (!GetCircleOf2PTAndR(this.src_bottomLeft, this.src_bottomRight, _CurveRBtm, ref CenterBtm1, ref CenterBtm2))
        {
            UpdateUVsQuadrangle();
            return;
        }

        var CenterTop = (this._UseUpperTop ? CenterTop1 : CenterTop2);
        var CenterBtm = (this._UseUpperBtm ? CenterBtm1 : CenterBtm2);


        var top_start = this.src_topLeft - CenterTop;
        var top_end = this.src_topRight - CenterTop;
        var top_startAngle = Mathf.Deg2Rad * Vector2.Angle(Vector2.right, top_start) * (top_start.y > 0 ? 1 : -1);
        var top_endAngle = Mathf.Deg2Rad * Vector2.Angle(Vector2.right, top_end) * (top_end.y > 0 ? 1 : -1);
        var top_diffAngle = (top_endAngle - top_startAngle) / (float)this.Col;

        var btm_start = this.src_bottomLeft - CenterBtm;
        var btm_end = this.src_bottomRight - CenterBtm;
        var btm_startAngle = Mathf.Deg2Rad * Vector2.Angle(Vector2.right, btm_start) * (btm_start.y > 0 ? 1 : -1);
        var btm_endAngle = Mathf.Deg2Rad * Vector2.Angle(Vector2.right, btm_end) * (btm_end.y > 0 ? 1 : -1);
        var btm_diffAngle = (btm_endAngle - btm_startAngle) / (float)this.Col;

        var tempTop = new Vector2();
        var tempBtm = new Vector2();
        float topAngle;
        float btmAngle;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                topAngle = top_startAngle + top_diffAngle * x;
                tempTop = CenterTop + (new Vector2(Mathf.Cos(topAngle), Mathf.Sin(topAngle))) * top_start.magnitude;
                btmAngle = btm_startAngle + btm_diffAngle * x;
                tempBtm = CenterBtm + (new Vector2(Mathf.Cos(btmAngle), Mathf.Sin(btmAngle))) * btm_start.magnitude;

                _UV[y * width + x] = tempTop + (tempBtm - tempTop) * y / (float)Row;
            }
        }
    }

    private bool GetCircleOf2PTAndR(
                            Vector2 pt1,            // 円上の第１点
                            Vector2 pt2,            // 円上の第２点
                            float r,              // 円の半径
                            ref Vector2 pc1,           // 第１円の中心
                            ref Vector2 pc2)           // 第２円の中心
    {
        bool stat = true;
        Vector2 pt3;
        float d, l1, dx, dy;

        pt3.x = (pt1.x + pt2.x) * 0.5f;
        pt3.y = (pt1.y + pt2.y) * 0.5f;

        r *= r;
        l1 = (pt2.x - pt3.x) * (pt2.x - pt3.x) + (pt2.y - pt3.y) * (pt2.y - pt3.y);

        if (r >= l1)
        {
            d = Mathf.Sqrt(r / l1 - 1.0f);
            dx = d * (pt2.y - pt3.y);
            dy = d * (pt2.x - pt3.x);

            pc1.x = pt3.x + dx;
            pc1.y = pt3.y - dy;

            pc2.x = pt3.x - dx;
            pc2.y = pt3.y + dy;
        }
        else
        {
            stat = false;
        }
        return stat;
    }

    private void UpdateVerticesCylinder()
    {
        int width = this.Col + 1;
        int height = this.Row + 1;


        float diffAngle = (_AngleFinish - _AngleStart) / this.Col;
        float currentAngle = 0;
        Vector3 temp = new Vector3();
        for (int x = 0; x < width; x++)
        {
            currentAngle = _AngleStart + diffAngle * x;
            temp.x = _Radius * Mathf.Cos(currentAngle);
            temp.z = _Radius * Mathf.Sin(currentAngle);

            for (int y = 0; y < height; y++)
            {
                temp.y = _Height * y / (float)Row;
                _Vertices[y * width + x] = temp;
            }
        }
    }


    private void UpdateVerticesPinePlane()
    {
        int width = this.Col + 1;
        int height = this.Row + 1;


        float diffAngle = (_AngleFinish - _AngleStart) / this.Col;
        float diffRadius = (_Radius - _InnerRadius) / this.Row;
        float currentRadius = 0;
        float currentAngle = 0;
        Vector3 temp = new Vector3();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                currentAngle = _AngleStart + diffAngle * x;
                currentRadius = _InnerRadius + diffRadius * y;
                temp.x = currentRadius * Mathf.Cos(currentAngle);
                temp.z = currentRadius * Mathf.Sin(currentAngle);
                temp.y = 0;
                _Vertices[y * width + x] = temp;
            }
        }
    }


}
