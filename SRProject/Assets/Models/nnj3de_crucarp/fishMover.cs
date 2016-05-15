using UnityEngine;
using System.Collections;

public class fishMover : MonoBehaviour {
   
    public float speed = 10f;
    public float rotationSmooth = 0.5f;
    public float levelSizeX = 20f;
    public float levelSizeZ = 20f;

    private Vector3 targetPosition;


    private float changeTargetSqrDistance = 30f;
    private void Start()
    {
        targetPosition = GetRandomPositionOnLevel();

    }

    private void Update()
    {

        // 目標地点との距離が小さければ、次のランダムな目標地点を設定する
        float sqrDistanceToTarget = Vector3.SqrMagnitude(transform.position - targetPosition);
        if (sqrDistanceToTarget < changeTargetSqrDistance)
        {
            targetPosition = GetRandomPositionOnLevel();
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
        
        return new Vector3(Random.Range(-levelSizeX, levelSizeX), this.transform.position.y, Random.Range(-levelSizeZ, levelSizeZ));
    }

}
