using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object3DOperation : Miwalab.ShadowGroup.Operation.AOperationCommand
{

    public float _H =0;
    public float _V = 0;
    public Object3DOperation()
    {
        this._H = 0;
        this._V = 0;
    }
    public Object3DOperation(float _H, float _V)
    {
        this._H = _H;
        this._V = _V;
    }
}
