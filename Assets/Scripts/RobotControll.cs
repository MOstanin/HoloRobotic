using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class RobotControll : MonoBehaviour, IInputClickHandler {

    public void OnInputClicked(InputClickedEventData eventData)
    {
        GameObject menu = GameObject.Find("Menu");
        MyMenuContoler.Instance.RobotMenuCall();

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
