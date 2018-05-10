using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;


public class CubTranslation : MonoBehaviour, IManipulationHandler{

    [SerializeField]
    private GameObject parent;

    //public float TranslationSensitivity;

    private Vector3 Delta = Vector3.zero;
    private Vector3 Offset = Vector3.zero;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        PerformTranslation();

    }

    private void PerformTranslation()
    {
        if (Delta == Vector3.zero)
        {
            return;
        }

        if (parent == null)
        {
            //transform.Translate(new Vector3(translationFactorX, translationFactorY, translationFactorZ), Space.World);
            transform.position = Offset + Delta;
        }
        else
        {
            //parent.transform.Translate(new Vector3(translationFactorX, translationFactorY, translationFactorZ), Space.World);
            parent.transform.position = Offset + Delta;
            //Debug.Log(Delta.magnitude.ToString());
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

        if (parent.name == "CubeSmall")
        {
            parent.SendMessage("UpdateScale");
        }
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
