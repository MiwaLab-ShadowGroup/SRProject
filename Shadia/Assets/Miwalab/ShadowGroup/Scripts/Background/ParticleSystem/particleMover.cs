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

        public int ID { set; get; }
        public float Regist { set; get; }


        // Use this for initialization
        void Start()
        {
            
            this._RHAngle = new Vector3(UnityEngine.Random.Range(1f,10f), UnityEngine.Random.Range(0f, 2.5f), UnityEngine.Random.Range(0f, 3.14f));
            Regist = -0.1f;
        }

        // Update is called once per frame
        void Update()
        {
            this.AddForce(_RHAngleVel * Regist);

            _RHAngleVel += _RHAngleacc;
            _RHAngle += _RHAngleVel;
            _RHAngleacc.Set(0, 0, 0);

            this.reset();

            _position.x = _RHAngle.x * Mathf.Cos(_RHAngle.z);
            _position.z = _RHAngle.x * Mathf.Sin(_RHAngle.z);
            _position.y = _RHAngle.y;
            this.transform.localPosition = _position;
        }

        private void reset()
        {
            if(_RHAngle.y < -2.0f)
            {
                _RHAngle.y += 4.0f;
            }
            if (_RHAngle.y >2.0f)
            {
                _RHAngle.y -= 4.0f;
            }

            if (_RHAngle.x < -6.0f)
            {
                _RHAngle.x += 10.0f;
            }
            if (_RHAngle.x > 4.0f)
            {
                _RHAngle.x -= 10.0f;
            }

            //if (_RHAngle.z < 1.0f)
            //{
            //    _RHAngle.z += 8.0f;
            //}
            //if (_RHAngle.z > 9.0f)
            //{
            //    _RHAngle.z -= 8.0f;
            //}


            if (this._RHAngleVel.magnitude > 0.01f)
            {
                this._RHAngleVel.Normalize();
                this._RHAngleVel *= 0.01f;
            }
        }

        public void AddForce(Vector3 f)
        {
            _RHAngleacc += f;
        }
        public void AddForce(float x, float y)
        {
            _RHAngleacc.z += x;
            _RHAngleacc.y += y;
        }
    }
}
