using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFloorMover : MonoBehaviour
{


    public float speed = 10f;
    public float cornerSharp = 0.5f;
    public float moveRadOut = 20f;
    public float moveRadIn = 20f;
    private Vector3 targetPosition;
    private bool Jump;

    private float sqrDistanceToTarget;
    private float changeTargetSqrDistance = 0.01f;
    private float cont = 0;
    private Vector3 prePos;

    public bool circleMove = false;
    private float direction;
    private float radDist;


    private float myRadius;
    private float myRadian;


    private void Start()
    {
        targetPosition = GetRandomPositionOnCircle();

    }

    private void Update()
    {

        if (Input.GetKeyUp(KeyCode.J))
        {
            int i = Random.Range(0, 100);
            if (i < 10)
            {
                Jump = true;
                prePos = this.transform.position;

            }
        }

        if (Jump)
        {
            this.transform.position += new Vector3(0, Mathf.Sin(cont) * 0.1f, 0);
            cont += Mathf.PI / 15;
            if (cont >= 2 * Mathf.PI)
            {
                cont = 0;
                Jump = false;
                this.transform.TransformPoint(prePos);
            }

        }



        // 目標地点との距離が小さければ、次のランダムな目標地点を設定する
        sqrDistanceToTarget = Vector3.SqrMagnitude(transform.position - targetPosition);

        if (sqrDistanceToTarget < changeTargetSqrDistance)
        {
            targetPosition = GetRandomPositionOnCircle();
        }

        //目的地まで直行
        if (!this.circleMove)
        {
            // 目標地点の方向を向く
            Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position));
            //targetRotation = targetRotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * cornerSharp);

            // 前方に進む
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        //迂回して目的地へ
        else
        {
            this.transform.position += new Vector3( Mathf.Cos(Mathf.PI * speed * Time.deltaTime * this.direction), 0, Mathf.Sin(Mathf.PI * speed * Time.deltaTime * this.direction));
            radDist = Vector3.Magnitude(targetPosition) - Vector3.Magnitude(this.transform.position);



           /// this.transform.position += new Vector3(this.transform.position -  );

            // 目標地点の方向を向く
            Quaternion targetRotation = Quaternion.LookRotation((new Vector3(0,0,0) - transform.position));
            this.transform.Rotate(0,90 * this.direction,0);

        }


    }


    public Vector3 GetRandomPositionOnCircle()
    {
        float radius = Random.Range(this.moveRadIn, this.moveRadOut);
        float radian = Random.Range(0, 2 * Mathf.PI);
        this.direction = Mathf.Sign(Random.Range(-1,1));
       
        return new Vector3(radius * Mathf.Cos(radian), this.transform.position.y, radius * Mathf.Sin(radian));
    }



}

