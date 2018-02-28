using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Examples.InteractiveElements;
using System;
using HoloToolkit.Unity.InputModule;

public class AgilusInfoControl : MonoBehaviour {

    private SliderGestureControl[] sliders;

    float[] q;
    bool readFlag;


    // Use this for initialization
    void Start () {
        readFlag = true;

        GameObject slider1 = GameObject.Find("SliderA1");
        GameObject slider2 = GameObject.Find("SliderA2");
        GameObject slider3 = GameObject.Find("SliderA3");
        GameObject slider4 = GameObject.Find("SliderA4");
        GameObject slider5 = GameObject.Find("SliderA5");
        GameObject slider6 = GameObject.Find("SliderA6");

        Component[] components =  slider1.GetComponents(typeof(GestureInteractive));

        sliders = new SliderGestureControl[6];


        sliders[0] = (SliderGestureControl) slider1.GetComponent<GestureInteractive>().Control;
        sliders[1] = (SliderGestureControl) slider2.GetComponent<GestureInteractive>().Control;
        sliders[2] = (SliderGestureControl) slider3.GetComponent<GestureInteractive>().Control;
        sliders[3] = (SliderGestureControl) slider4.GetComponent<GestureInteractive>().Control;
        sliders[4] = (SliderGestureControl) slider5.GetComponent<GestureInteractive>().Control;
        sliders[5] = (SliderGestureControl) slider6.GetComponent<GestureInteractive>().Control;

       
        q = MyMenuContoler.Instance.RobotState();


        //q = MyMenuContoler.Instance.RobotState();
        for (int i = 0; i < sliders.Length; i++)
        {
            sliders[i].SetSliderValue(q[i] * 180 / Mathf.PI);
        }
    }


    // Update is called once per frame
    void Update () {


        if (!readFlag)
        {
           
        }
        else
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                q[i] = sliders[i].SliderValue * Mathf.PI / 180 ;
            }
            MyMenuContoler.Instance.SendStateToRobot(q);
        }
    }
    
    public float[] SlidersState()
    {
        return q;
    }

    public void SliderStateUpdated()
    {
        readFlag = false;
    }
}
