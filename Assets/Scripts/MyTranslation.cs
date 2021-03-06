﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;




public class MyTranslation : MonoBehaviour, IManipulationHandler
{
    public float TranslationSensitivity;

    private float translationFactorX = 0.0f;
    private float translationFactorY = 0.0f;
    private float translationFactorZ = 0.0f;

    private Vector3 Delta = Vector3.zero;
    private Vector3 Offset = Vector3.zero;

    [SerializeField]
    private GameObject parent;
    

    public bool flag;

    void Start()
    {
    }

    void Update()
    {
        if (flag)
        {
            PerformTranslation();
        }
    }

    public void StartTranslation()
    {
        flag = true;
    }
    public void StopTranslation()
    {
        flag = false;
    }

    private void PerformTranslation()
    {
        if (Delta == Vector3.zero)
        {
            return;
        }
        
        translationFactorX = Delta.x * TranslationSensitivity;
        translationFactorY = Delta.y * TranslationSensitivity;
        translationFactorZ = Delta.z * TranslationSensitivity;


        if (parent == null)
        {
            //transform.Translate(new Vector3(translationFactorX, translationFactorY, translationFactorZ), Space.World);
            transform.position = Offset + new Vector3(translationFactorX, translationFactorY, translationFactorZ);
        }
        else
        {
            //parent.transform.Translate(new Vector3(translationFactorX, translationFactorY, translationFactorZ), Space.World);
            parent.transform.position = Offset + new Vector3(translationFactorX, translationFactorY, translationFactorZ);
            Debug.Log(Delta.magnitude.ToString());
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
        if (parent == null)
        {
            Offset = gameObject.transform.position;
        }
        else
        {
            Offset = parent.transform.position;
        }
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
        Delta = eventData.CumulativeDelta;
    }
}
