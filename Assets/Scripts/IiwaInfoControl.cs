﻿using System.Collections;
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

        base.sliders = new SliderGestureControl[7];


        base.sliders[0] = (SliderGestureControl)slider1.GetComponent<GestureInteractive>().Control;
        base.sliders[1] = (SliderGestureControl)slider2.GetComponent<GestureInteractive>().Control;
        base.sliders[2] = (SliderGestureControl)slider3.GetComponent<GestureInteractive>().Control;
        base.sliders[3] = (SliderGestureControl)slider4.GetComponent<GestureInteractive>().Control;
        base.sliders[4] = (SliderGestureControl)slider5.GetComponent<GestureInteractive>().Control;
        base.sliders[5] = (SliderGestureControl)slider6.GetComponent<GestureInteractive>().Control;
        base.sliders[6] = (SliderGestureControl)slider7.GetComponent<GestureInteractive>().Control;
    }
    
}
