using UnityEngine;
using System.Collections;

public class WhaleController : MonoBehaviour {

    public OceanSurfaceRenderer oceanSurfaceRenderer;
    public int oceanSurfaceMaskIndex = 0;
    public float oceanSurfaceWaveStrength = 1f;

    public float speed = 0f;
    public float rotate = 5.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire2"))
        {
            speed += 0.02f;
            rotate += 0.02f;
        }
        if (Input.GetButton("Fire1"))
        {
            speed -= 0.02f;
            rotate -= 0.02f;
        }


        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        float l = Input.GetAxis("Fire5");
        float r = Input.GetAxis("Fire6");
        //transform.localPosition += new Vector3(Time.deltaTime * speed, 0, 0);
        transform.Rotate(-v * rotate, (r - l) * rotate, -h * rotate);

        transform.position += transform.forward * Time.deltaTime * speed;
        //transform.Translate(transform.forward * speed);

        var prev = transform.position;
        var curr = transform.position + (-Vector3.forward * speed * Time.deltaTime);
        var coll = GetComponent<CapsuleCollider>();
        var radius = transform.TransformVector(Vector3.right * coll.radius).x;

        oceanSurfaceRenderer.BeginForce();
        oceanSurfaceRenderer.AddCircleMovement(prev, curr, oceanSurfaceWaveStrength, radius, 1 << oceanSurfaceMaskIndex);
        oceanSurfaceRenderer.EndForce();

    }
}