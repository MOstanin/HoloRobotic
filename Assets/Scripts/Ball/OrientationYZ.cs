using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class OrientationYZ : MonoBehaviour,  IManipulationHandler{

    [SerializeField]
    private GameObject parentBall;

    public float TranslationSensitivity = 0.003f;

    private float translationFactorX = 0.0f;
    private float translationFactorY = 0.0f;
    private float translationFactorZ = 0.0f;

    private Vector3 Delta = Vector3.zero;

    private MeshRenderer mesh;
    private SphereCollider collider;
    public bool flag;

    void Start()
    {
        
        mesh = gameObject.GetComponent<MeshRenderer>();
        collider = gameObject.GetComponent<SphereCollider>();
        StopOrientationYZ();

    }

    void Update()
    {
        if (flag)
        {
            PerformRotation();
        }
    }

    public void StartOrientationYZ()
    {
        flag = true;
        mesh.enabled = true;
        collider.enabled = true;
        
    }
    public void StopOrientationYZ()
    {
        flag = false;
        mesh.enabled = false;
        collider.enabled = false;
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

        Vector3 vector = parentBall.transform.position - gameObject.transform.position;

        float zAngle = Mathf.Atan2(vector.y - translationFactorY, vector.x - translationFactorX);
        float yAngle = Mathf.Atan2(-(vector.z - translationFactorZ), (vector.y - translationFactorY) / Mathf.Sin(zAngle));
        parentBall.transform.eulerAngles = new Vector3(0, yAngle * 180 / Mathf.PI, zAngle * 180 / Mathf.PI);
        gameObject.transform.Translate(new Vector3(translationFactorX, translationFactorY, translationFactorZ), Space.World);
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
