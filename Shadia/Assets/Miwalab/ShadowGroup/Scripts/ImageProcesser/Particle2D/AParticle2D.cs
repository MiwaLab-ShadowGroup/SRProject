using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Miwalab.ShadowGroup.ImageProcesser.Particle2D
{
    public abstract class AParticle2D
    {
        public enum DeadType
        {
            Top,
            Bottom,
            Left,
            Right
        }

        public float Size { set; get; }
        public Vector2 Position { set; get; }
        public Vector2 Vellocity { set; get; }
        public Vector2 Accell { set; get; }

        private Scalar _Color;
        public Scalar Color { set { _Color = value; } get { return _Color; } }
        public bool Alive { set; get; }
        public DeadType _DeadType { set; get; }
        public float Friction { set; get; }

        protected Point point = new Point();

        public AParticle2D()
        {
            Size = 1f;
            Position = new Vector2();
            Accell = new Vector2();
            Vellocity = new Vector2();
            Color = new Scalar();
            Alive = true;
        }
        public void AddForce(Vector2 f)
        {
            Accell += f;
        }
        public void Update()
        {
            if (Alive)
            {
                Vellocity += Accell;
                Position += Vellocity;
                Accell = new Vector2(0, 0);
            }
        }
        public void DeadCheck(int width, int height)
        {
            if (this.Position.x < 0)
            {
                Alive = false;
                _DeadType = DeadType.Left;
            }
            else if (this.Position.x > width)
            {
                Alive = false;
                _DeadType = DeadType.Right;
            }
            else if (this.Position.y < 0)
            {
                Alive = false;
                _DeadType = DeadType.Top;
            }
            else if (this.Position.y > height)
            {
                Alive = false;
                _DeadType = DeadType.Bottom;
            }
        }

        public void CutOffVellocity(float MaxValue)
        {
            if (this.Vellocity.magnitude > MaxValue)
            {
                this.Vellocity = this.Vellocity.normalized * MaxValue;
            }
        }

        public void Revirth(int width, int height)
        {
            if (!this.Alive)
            {
                Alive = true;
                switch (_DeadType)
                {
                    case DeadType.Top:
                        this.Position += new Vector2(0, height);
                        break;
                    case DeadType.Bottom:
                        this.Position -= new Vector2(0, height);
                        break;
                    case DeadType.Left:
                        this.Position += new Vector2(width, 0);
                        break;
                    case DeadType.Right:
                        this.Position -= new Vector2(width, 0);
                        break;
                }
            }
        }

        public void GraduallyChangeColorTo(Scalar target, double potion)
        {
            _Color.Val0 += (target.Val0 - _Color.Val0) * potion;
            _Color.Val1 += (target.Val1 - _Color.Val1) * potion;
            _Color.Val2 += (target.Val2 - _Color.Val2) * potion;
            _Color.Val3 += (target.Val3 - _Color.Val3) * potion;
        }



        public abstract void DrawShape(ref Mat mat);

        public void PutText(ref Mat mat, string str, double scale)
        {
            mat.PutText(str, this.point, OpenCvSharp.FontFace.Vector0, scale, Scalar.White);
        }

        internal void RevirthRandom(int width, int height)
        {
            if (!this.Alive)
            {
                Alive = true;
                Position = new UnityEngine.Vector2(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));

            }
        }
    }

}
