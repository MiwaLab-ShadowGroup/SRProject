using UnityEngine;
using System.Collections;


public class UIPointSphereSrcScript : MonoBehaviour
{
    public enum PointType
    {
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom,
        Other
    }

    private bool _isDragged;
    private Vector3 _initialMyPosition;
    private Vector3 _initialMousePosition;


    public ShadowMeshRenderer _Renderer { set; get; }
    public int _uvIndex { set; get; }
    private MeshRenderer _MeshRenderer { set; get; }
    private bool _IsSelected { set; get; }
    private Color _PrevColor { set; get; }

    public PointType _pointType { set; get; }

    // Use this for initialization
    void Start()
    {
        this._MeshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl)&& Input.GetKey(KeyCode.LeftShift) && this._IsSelected)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new Vector2(mousePos.x, mousePos.y);
            var myPos = this.gameObject.transform.position;
            var myPos2D = new Vector2(myPos.x, myPos.y);

            _isDragged = true;
            _initialMyPosition = myPos;
            _initialMousePosition = mousePos;
        }
        else if (Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftControl) || (Input.GetKey(KeyCode.RightControl))))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new Vector2(mousePos.x, mousePos.y);
            var myPos = this.gameObject.transform.position;
            var myPos2D = new Vector2(myPos.x, myPos.y);
            if ((myPos2D - mousePos2D).magnitude < 0.3f)
            {

                this._IsSelected = !this._IsSelected;

                if (this._IsSelected)
                {
                    _PrevColor = this._MeshRenderer.material.color;
                    this._MeshRenderer.material.color = Color.cyan;
                }
                else
                {
                    this._MeshRenderer.material.color = _PrevColor;
                }
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragged = false;
        }

        if (_isDragged)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var pos = mousePos - _initialMousePosition;

            var posnow = _initialMyPosition + pos;

            _Renderer.setInptPositionInverse(posnow, this._uvIndex, _pointType);

        }

        

        if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKey(KeyCode.Space)))
        {
            this._IsSelected =false;

            if (this._IsSelected)
            {
                _PrevColor = this._MeshRenderer.material.color;
                this._MeshRenderer.material.color = Color.cyan;
            }
            else
            {
                this._MeshRenderer.material.color = _PrevColor;
            }
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.Backspace))
        {
            if (_IsSelected)
            {
                this._Renderer.resetInptPositionInverse(this._uvIndex);
            }
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.A))
        {
            this._IsSelected = true;

            if (this._IsSelected)
            {
                _PrevColor = this._MeshRenderer.material.color;
                this._MeshRenderer.material.color = Color.cyan;
            }
            else
            {
                this._MeshRenderer.material.color = _PrevColor;
            }
        }

    }
}
