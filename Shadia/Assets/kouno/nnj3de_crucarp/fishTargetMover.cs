using UnityEngine;
using System.Collections;

public class fishTargetMover : MonoBehaviour {

    public float speed = 10f;
    public float preSpeed;
    public float rotationSmooth = 0.5f;
    public float preRotationSmooth;
    public float levelSizeX = 20f;
    public float levelSizeZ = 20f;
    public float randomLevel = 0.2f;
    private Collider col;
    private Vector3 targetPosition;
    private int count =  0;
    private float dif;
    private Vector3 prePosition;

    private float changeTargetSqrDistance = 30f;
    private void Start()
    {
        speed += Random.Range(-speed * randomLevel, speed * randomLevel);
        rotationSmooth += Random.Range(-rotationSmooth * randomLevel, rotationSmooth * randomLevel);

        preSpeed = speed;
        preRotationSmooth = rotationSmooth;
        targetPosition = GetRandomPositionOnLevel();
        prePosition = GameObject.Find("TargetCube").transform.position;
        //targetPosition = GameObject.Find("TargetCube").transform.position;
    }

    private void Update()
    {

        // 目標地点との距離が小さければ、次のランダムな目標地点を設定する
        //float sqrDistanceToTarget = Vector3.SqrMagnitude(transform.position - targetPosition);

        //if (sqrDistanceToTarget < changeTargetSqrDistance)
        //{
        //    targetPosition = GetRandomPositionOnLevel();
        //}


        

        dif = Mathf.Abs( (prePosition - GameObject.Find("TargetCube").transform.position).magnitude);
        if (dif < 0.2) {
            count++;
        }
        else
        {
            count = 0;
        }

        if (count > 200)
        {
            GetComponent<Collider>().enabled = false;
            targetPosition = GameObject.Find("TargetCube").transform.position;
            rotationSmooth = 0.9f;
        }
        else
        {
            GetComponent<Collider>().enabled = true;
            // 目標地点との距離が小さければ、次のランダムな目標地点を設定する
            float sqrDistanceToTarget = Vector3.SqrMagnitude(transform.position - targetPosition);
            if (sqrDistanceToTarget < changeTargetSqrDistance)
            {
                targetPosition = GetRandomPositionOnLevel();
                speed = preSpeed;
                rotationSmooth = preRotationSmooth;
            }
        }




        // 目標地点の方向を向く
        Quaternion targetRotation = Quaternion.LookRotation((targetPosition - transform.position));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmooth);

        // 前方に進む
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        prePosition = GameObject.Find("TargetCube").transform.position;

    }

    public Vector3 GetRandomPositionOnLevel()
    {

        return new Vector3(Random.Range(-levelSizeX, levelSizeX), this.transform.position.y, Random.Range(-levelSizeZ, levelSizeZ));
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            targetPosition = transform.position - (prePosition - transform.position)*3;
            speed *= 2;
            rotationSmooth *= 3;

        }
    }
}
