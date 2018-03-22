using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

[RequireComponent(typeof(Interpolator))]
public class CameraFollower : MonoBehaviour {

    Vector3 offset;
    Vector3 offsetAngles;

    private Interpolator interpolator;

    // Use this for initialization
    void Start () {
        interpolator = gameObject.EnsureComponent<Interpolator>();
        offset = transform.position - Camera.main.transform.position;
        //offsetAngles = transform.rotation.eulerAngles - Camera.main.transform.rotation.eulerAngles;
        //transform.SetParent(Camera.main.transform);
    }

    // Update is called once per frame
    void Update()
    {
        //float a = Camera.main.transform.rotation.eulerAngles.y;
        //transform.position = Camera.main.transform.position + offset ;
        //transform.RotateAround(Vector3.up, Camera.main.transform.rotation.eulerAngles.y);
        //transform.Rotate(0, a, 0, Space.World);

        //transform.rotation = Camera.main.transform.rotation;
        //transform.position = Camera.main.transform.position + offset;
        interpolator.SetTargetPosition(Camera.main.transform.position + offset);
    }

}
