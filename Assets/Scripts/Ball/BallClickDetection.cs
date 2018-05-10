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
            parent.GetComponent<AudioSource>().Play();
            if (AppManager.Instance.AccessToClick)
            {
                AppManager.Instance.BallClick(parent);
                return;
                
            }
            parent.SendMessage("ClickOnBall");
        }
    }

    public GameObject GetParentBall()
    {
        return parent;
    }
}
