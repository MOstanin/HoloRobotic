using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;




public class MyTranslation : MonoBehaviour, IManipulationHandler
{
    public float TranslationSensitivity = 0.3f;

    private float translationFactorX = 0.0f;
    private float translationFactorY = 0.0f;
    private float translationFactorZ = 0.0f;

    private Vector3 Delta = Vector3.zero;

    [SerializeField]
    private GameObject parent;
    

    private bool flag;

    void Start()
    {
        flag = false;
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
        if (Delta == Vector3.zero)
        {
            return;
        }
        
        translationFactorX = Delta.x * TranslationSensitivity;
        translationFactorY = Delta.y * TranslationSensitivity;
        translationFactorZ = Delta.z * TranslationSensitivity;


        if (parent == null)
        {
            transform.Translate(new Vector3(translationFactorX, translationFactorY, translationFactorZ), Space.World);
        }
        else
        {
            parent.transform.Translate(new Vector3(translationFactorX, translationFactorY, translationFactorZ), Space.World);
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
