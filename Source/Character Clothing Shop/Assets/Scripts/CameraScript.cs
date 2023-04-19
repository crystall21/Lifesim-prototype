using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject followTarget;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    public static CameraScript instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, -10), ref velocity, smoothTime);
    }
}
