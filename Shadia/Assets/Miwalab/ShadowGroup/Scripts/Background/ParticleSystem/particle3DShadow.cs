using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle3DShadow : MonoBehaviour {


    public GameObject _particleOriginal;

    public int _num;

    public List<GameObject> _particleObjects;

    public Vector3 _scale;

	// Use this for initialization
	void Start () {
        _num = 1500;
        _particleObjects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		if(this._particleObjects.Count > _num)
        {
            this._particleObjects.RemoveRange(_num, this._particleObjects.Count - _num);
        }
        if(this._particleObjects.Count < _num)
        {
            for (int i = 0; i < this._num - this._particleObjects.Count; ++i)
            {
                var _object = Instantiate(_particleOriginal);
                _object.transform.localScale = _scale;
                this._particleObjects.Add(_object);
            }
        }
    }
}
