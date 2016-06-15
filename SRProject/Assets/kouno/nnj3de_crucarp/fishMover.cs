using UnityEngine;
using System.Collections;

public class fishMover : MonoBehaviour {

    public float speed = 10f;
    public float rotationSmooth = 0.5f;
    public float levelSizeX = 20f;
    public float levelSizeZ = 20f;
    private Vector3 targetPosition;
    private bool Gone;
    private bool Jump;
    public GameObject jellyfish;
    public Camera cam;


    private float changeTargetSqrDistance = 0.01f;
    private float cont=0;
    private Vector3 prePos;
    private void Start()
    {
        targetPosition = GetRandomPositionOnLevel();
        Gone = false;

    }

    private void Update()
    {
        //奥に逃げる
        if (Input.GetKeyDown(KeyCode.G))
        {
            Gone = true;
            targetPosition = new Vector3(this.transform.position.x, this.transform.position.y, 10);
            jellyfish.SetActive(false);
        }
        //初期値に戻る
        if (Input.GetKeyDown(KeyCode.B))
        {
            Gone = false;
            targetPosition = GetRandomPositionOnLevel();
            jellyfish.SetActive(true);
        }
        //左右に分かれる
        if (Input.GetKeyDown(KeyCode.S))
        {
            Gone = true;
            if(this.transform.position.x > 0)
            {
                targetPosition = new Vector3(10, this.transform.position.y, this.transform.position.z);

            }
            else
            {
                targetPosition = new Vector3(-10, this.transform.position.y, this.transform.position.z);
            }
            jellyfish.SetActive(false);

        }
        //中心に集まる
        if (Input.GetKeyDown(KeyCode.C))
        {
            Gone = true;
            
            targetPosition = new Vector3(0, this.transform.position.y, -0.3f);
           
            jellyfish.SetActive(false);

        }
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
            this.transform.position += new Vector3(0,  Mathf.Sin(cont)*0.1f, 0) ;
            cont+= Mathf.PI/15;
            if (cont >= 2 * Mathf.PI)
            {
                cont = 0;
                Jump = false;
                this.transform.TransformPoint(prePos);
            }

        }
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    preHeight = this.cam.transform.position.y;
        //    targetHeight = this.cam.transform.position.y - 10f ;
        //    Jump = true;

        //}
        //if (Jump)
        //{
        //    if(this.cam.transform.position.y > targetHeight)
        //    {
        //        this.cam.transform.Translate(new Vector3(0, -1, 0) * 0.5f * Time.deltaTime);
        //    }
        //    else {
        //        Jump = false;
        //    }
        //}
        //if(this.cam.transform.position.y < preHeight)
        //{
        //    this.cam.transform.Translate(new Vector3(0, 1, 0) * 0.5f * Time.deltaTime);
        //}




        if (!Gone)
        {
            // 目標地点との距離が小さければ、次のランダムな目標地点を設定する
            float sqrDistanceToTarget = Vector3.SqrMagnitude(transform.position - targetPosition);
            if (sqrDistanceToTarget < changeTargetSqrDistance)
            {
                targetPosition = GetRandomPositionOnLevel();
            }
        }
        // 目標地点の方向を向く
        Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position));
        //targetRotation = targetRotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmooth);

        // 前方に進む
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        

    }


    public Vector3 GetRandomPositionOnLevel()
    {
        
        return new Vector3(Random.Range(-levelSizeX, levelSizeX), this.transform.position.y, Random.Range(0, levelSizeZ));
    }



}
