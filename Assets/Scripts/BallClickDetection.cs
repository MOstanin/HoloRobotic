using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class BallClickDetection : MonoBehaviour,IInputClickHandler {

    [SerializeField]
    private GameObject parent;
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (eventData.selectedObject == gameObject)
        {
            parent.SendMessage("ClickOnBall");
        }
        
    }
}
