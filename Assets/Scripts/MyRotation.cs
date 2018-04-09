using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class MyRotation : MonoBehaviour, IManipulationHandler
{
    public float RotationSensitivity = 1.0f;

    private float rotationFactorX = 0.0f;
    private float rotationFactorY = 0.0f;
    private float rotationFactorZ = 0.0f;

    private Vector3 Delta = Vector3.zero;

    // For ball only
    [SerializeField]
    private GameObject parent;

    private bool flag;


    // Use this for initialization
    void Start()
    {
        //Robot = GameObject.Find("IIWA1");
        flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (flag)
        {
            PerformRotation();
        }
    }

    public void StartRotation()
    {
        flag = true;
    }
    public void StopRotation()
    {
        flag = false;
    }

    private void PerformRotation()
    {
        if (Delta == Vector3.zero)
        {
            return;
        }

        // This will help control the amount of rotation.
        // Taking the delta along the horizontal axis movement.
        rotationFactorX = Delta.x * RotationSensitivity;
        rotationFactorY = Delta.y * RotationSensitivity;
        rotationFactorZ = Delta.z * RotationSensitivity;

        if (parent == null)
        {
            // Rotate object along the Y axis using.
            transform.Rotate(new Vector3(0, -1 * rotationFactorX, 0));
        }
        else
        {
            parent.transform.Rotate(new Vector3(0, -1 * rotationFactorX, 0));
            parent.transform.Rotate(new Vector3(-1*rotationFactorZ, 0, 0));
            parent.transform.Rotate(new Vector3(0, 0, 1 * rotationFactorY));
        }
    }

    public void OnManipulationCanceled(ManipulationEventData eventData)
    {
        Delta = Vector3.zero;
        InputManager.Instance.OverrideFocusedObject = null;
    }

    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        Delta = Vector3.zero;
        InputManager.Instance.OverrideFocusedObject = null;
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        InputManager.Instance.OverrideFocusedObject = gameObject;
        Delta = eventData.CumulativeDelta;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        Delta = eventData.CumulativeDelta;
    }
}
