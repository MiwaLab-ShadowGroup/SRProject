using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.Background.ParticleSystem
{
    public class particleMover : MonoBehaviour
    {
        public Vector3 _RHAngle = new Vector3();
        public Vector3 _RHAngleVel = new Vector3();
        public Vector3 _RHAngleacc = new Vector3();
        public Vector3 _position = new Vector3();


        // Use this for initialization
        void Start()
        {
            this.transform.position = _position;
        }

        // Update is called once per frame
        void Update()
        {
            _position.x = _RHAngle.x * Mathf.Cos(_RHAngle.z);
            _position.z = _RHAngle.x * Mathf.Sin(_RHAngle.z);

            _position.y = _RHAngle.y;
        }
    }
}
