using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Displacement/Twirl")]
    public class Twirl : ImageEffectBase
    {
        public Vector2 radius = new Vector2(0.3F,0.3F);
        [Range(0.0f,360.0f)]
        public float angle = 0f;
        public Vector2 center = new Vector2 (0.5F, 0.5F);
        public float speed = 3f;

        //private void Start()
        //{
        //    angle = Mathf.Cos(Time.time * speed) * 2f;

        //}

        private void Update()
        {
             angle += Mathf.Sin(Time.time * speed) * 0.5f; 

        }

    
    // Called by camera to apply image effect
    void OnRenderImage (RenderTexture source, RenderTexture destination)
        {
            ImageEffects.RenderDistortion (material, source, destination, angle, center, radius);
        }
    }
}
