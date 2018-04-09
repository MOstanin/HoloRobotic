using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Examples.InteractiveElements;

public abstract class InfoControlBase : MonoBehaviour {


    protected MySliderGestureControl[] sliders;

    //protected float[] q;

    public bool ControlMode;

    protected abstract void Init();
    // Use this for initialization
    protected virtual void Awake() {
        ControlMode = false;
        Init();
    }

    // Update is called once per frame
    protected virtual void Update() {

    }

    public float[] InfoState()
    {
        float[] q = new float[sliders.Length];
        for (int i = 0; i < sliders.Length; i++)
        {
            q[i] = sliders[i].SliderValue * Mathf.PI / 180; ;
        }
        return q;
    }

    public void ReadRobotState(float[] q)
    {
        //q = AppManager.Instance.RobotState();
        if (q.Length == this.sliders.Length)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                sliders[i].SetSliderValue(q[i] * 180 / Mathf.PI);
            }
        }
        else
        {
            Debug.Log("Sliders number doesn't corresponds to joins num.");
        }
    }

    public void ReadCartezianState(float[] mas)
    {
        //q = AppManager.Instance.RobotState();

        for (int i = 0; i < 3; i++)
        {
            sliders[i].SetSliderValue(mas[i]);
        }
        for (int i = 3; i < 6; i++)
        {
            sliders[i].SetSliderValue(mas[i] * 180 / Mathf.PI);
        }

    }

    public void ControlModeOn()
    {
        ControlMode = true;
    }

    public void ControlModeOff()
    {
        ControlMode = false;
    }
}
