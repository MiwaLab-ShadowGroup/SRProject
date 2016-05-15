using UnityEngine;
using System.Collections;

public class WhaleBehaviourScript : MonoBehaviour {

        public int oceanSurfaceMaskIndex = 0;
        public float oceanSurfaceWaveStrength = 1f;

        public float speed = 10f;
        public float rotationSmooth = 0.5f;

        public OceanSurfaceRenderer oceanSurfaceRenderer;
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
            Quaternion targetRotation = Quaternion.LookRotation(-(targetPosition - transform.position));
            //targetRotation = targetRotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmooth);

            // 前方に進む
            transform.Translate(- Vector3.forward * speed * Time.deltaTime);

        var prev = transform.position;
        var curr = transform.position + (-Vector3.forward * speed * Time.deltaTime);
        var coll = GetComponent<CapsuleCollider>();
        var radius = transform.TransformVector(Vector3.right * coll.radius).x;

        oceanSurfaceRenderer.BeginForce();
        oceanSurfaceRenderer.AddCircleMovement(prev, curr, oceanSurfaceWaveStrength, radius, 1 << oceanSurfaceMaskIndex);
        oceanSurfaceRenderer.EndForce();

    }


        public Vector3 GetRandomPositionOnLevel()
        {
            float levelSize = 100f;
            return new Vector3(Random.Range(-levelSize, levelSize), 0, Random.Range(-levelSize, levelSize));
        }
        
}