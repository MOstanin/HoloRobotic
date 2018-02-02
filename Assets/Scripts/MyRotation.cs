using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using System;

public class MyRotation : MonoBehaviour, INavigationHandler
{
    public float RotationSensitivity = 5.0f;

    private float rotationFactor = 0.0f;
    private Vector3 navigationDelta = Vector3.zero;
    public GameObject Robot;

    public bool flag;


    // Use this for initialization
    void Start()
    {
        //Robot = GameObject.Find("IIWA1");
    }

    // Update is called once per frame
    void Update()
    {
        if (Robot != null)
        {
            PerformRotation();
        }
    }

    private void PerformRotation()
    {
        if (navigationDelta == Vector3.zero)
        {
            return;
        }

        // This will help control the amount of rotation.
        // Taking the delta along the horizontal axis movement.
        rotationFactor = navigationDelta.x * RotationSensitivity;

        // Rotate object along the Y axis using.
        Robot.transform.Rotate(new Vector3(0, -1 * rotationFactor, 0));
    }

    public void OnNavigationCanceled(NavigationEventData eventData)
    {
        navigationDelta = Vector3.zero;
        InputManager.Instance.OverrideFocusedObject = null;
    }

    public void OnNavigationCompleted(NavigationEventData eventData)
    {
        navigationDelta = Vector3.zero;
        InputManager.Instance.OverrideFocusedObject = null;
    }

    public void OnNavigationStarted(NavigationEventData eventData)
    {
        InputManager.Instance.OverrideFocusedObject = gameObject;
        navigationDelta = eventData.NormalizedOffset;
    }

    public void OnNavigationUpdated(NavigationEventData eventData)
    {
        navigationDelta = eventData.NormalizedOffset;
    }
}
