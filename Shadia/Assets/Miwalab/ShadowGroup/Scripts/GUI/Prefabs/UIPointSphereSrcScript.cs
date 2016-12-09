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

    public PointType _pointType { set; get; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftControl) || (Input.GetKey(KeyCode.RightControl))))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePos2D = new Vector2(mousePos.x, mousePos.y);
            var myPos = this.gameObject.transform.position;
            var myPos2D = new Vector2(myPos.x, myPos.y);
            if ((myPos2D - mousePos2D).magnitude < 0.3f)
            {
                _isDragged = true;
                _initialMyPosition = myPos;
                _initialMousePosition = mousePos;
            }
        }

        if (Input.GetMouseButtonUp(0) && (Input.GetKey(KeyCode.LeftControl) || (Input.GetKey(KeyCode.RightControl))))
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
    }
}
