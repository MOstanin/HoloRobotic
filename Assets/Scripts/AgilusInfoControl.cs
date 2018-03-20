using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Examples.InteractiveElements;

public class AgilusInfoControl : InfoControlBase
{
    protected override void Init()
    {
        GameObject slider1 = GameObject.Find("SliderA1");
        GameObject slider2 = GameObject.Find("SliderA2");
        GameObject slider3 = GameObject.Find("SliderA3");
        GameObject slider4 = GameObject.Find("SliderA4");
        GameObject slider5 = GameObject.Find("SliderA5");
        GameObject slider6 = GameObject.Find("SliderA6");

        Component[] components = slider1.GetComponents(typeof(GestureInteractive));

        base.sliders = new SliderGestureControl[6];


        base.sliders[0] = (SliderGestureControl)slider1.GetComponent<GestureInteractive>().Control;
        base.sliders[1] = (SliderGestureControl)slider2.GetComponent<GestureInteractive>().Control;
        base.sliders[2] = (SliderGestureControl)slider3.GetComponent<GestureInteractive>().Control;
        base.sliders[3] = (SliderGestureControl)slider4.GetComponent<GestureInteractive>().Control;
        base.sliders[4] = (SliderGestureControl)slider5.GetComponent<GestureInteractive>().Control;
        base.sliders[5] = (SliderGestureControl)slider6.GetComponent<GestureInteractive>().Control;
    }
    
}
