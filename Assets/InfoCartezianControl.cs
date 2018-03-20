using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Examples.InteractiveElements;

public class InfoCartezianControl : InfoControlBase
{
    protected override void Init()
    {
        GameObject slider1 = GameObject.Find("SliderX");
        GameObject slider2 = GameObject.Find("SliderY");
        GameObject slider3 = GameObject.Find("SliderZ");
        GameObject slider4 = GameObject.Find("SliderA");
        GameObject slider5 = GameObject.Find("SliderB");
        GameObject slider6 = GameObject.Find("SliderC");

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
