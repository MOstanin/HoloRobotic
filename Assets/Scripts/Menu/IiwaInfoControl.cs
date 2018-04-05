using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Examples.InteractiveElements;
using System;
using HoloToolkit.Unity.InputModule;

public class IiwaInfoControl : InfoControlBase
{
    protected override void Init()
    {
        GameObject slider1 = GameObject.Find("SliderI1");
        GameObject slider2 = GameObject.Find("SliderI2");
        GameObject slider3 = GameObject.Find("SliderI3");
        GameObject slider4 = GameObject.Find("SliderI4");
        GameObject slider5 = GameObject.Find("SliderI5");
        GameObject slider6 = GameObject.Find("SliderI6");
        GameObject slider7 = GameObject.Find("SliderI6");

        Component[] components = slider1.GetComponents(typeof(GestureInteractive));

        base.sliders = new MySliderGestureControl[7];


        base.sliders[0] = (MySliderGestureControl)slider1.GetComponent<GestureInteractive>().Control;
        base.sliders[1] = (MySliderGestureControl)slider2.GetComponent<GestureInteractive>().Control;
        base.sliders[2] = (MySliderGestureControl)slider3.GetComponent<GestureInteractive>().Control;
        base.sliders[3] = (MySliderGestureControl)slider4.GetComponent<GestureInteractive>().Control;
        base.sliders[4] = (MySliderGestureControl)slider5.GetComponent<GestureInteractive>().Control;
        base.sliders[5] = (MySliderGestureControl)slider6.GetComponent<GestureInteractive>().Control;
        base.sliders[6] = (MySliderGestureControl)slider7.GetComponent<GestureInteractive>().Control;
    }
    
}
