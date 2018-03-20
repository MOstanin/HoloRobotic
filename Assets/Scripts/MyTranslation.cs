using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;




public class MyTranslation : MonoBehaviour, INavigationHandler
{
    public float TranslationSensitivity = 0.003f;

    private float translationFactorX = 0.0f;
    private float translationFactorY = 0.0f;
    private float translationFactorZ = 0.0f;

    private Vector3 navigationDelta = Vector3.zero;
    

    public bool flag;

    void Start()
    {
        if (this.gameObject.tag == "ball")
        {
            flag = true;
        }
        else
        {
            flag = false;

        }
    }

    void Update()
    {
        if (flag)
        {
            PerformRotation();
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

    private void PerformRotation()
    {
        if (navigationDelta == Vector3.zero)
        {
            return;
        }
        
        translationFactorX = navigationDelta.x * TranslationSensitivity;
        translationFactorY = navigationDelta.y * TranslationSensitivity;
        translationFactorZ = navigationDelta.z * TranslationSensitivity;


        transform.Translate(new Vector3(translationFactorX, translationFactorY, translationFactorZ),Space.World);
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
