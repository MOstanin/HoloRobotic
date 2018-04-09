using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine.XR.WSA.Input;
using System;

[RequireComponent(typeof(Interpolator))]
public class PointerController : MonoBehaviour {

    private Interpolator interpolator;
    Vector3 offset;
    public GameObject ball;
    public GameObject prim;

    enum State { NoDrawing,  FirstClick, Drawing, SecondClick, Destroying};
    State state;

    // Use this for initialization
    void Start () {
        interpolator = gameObject.EnsureComponent<Interpolator>();

        offset = new Vector3(0, 0, 0);
        InteractionManager.InteractionSourceLost += InteractionManager_SourceLost;
        InteractionManager.InteractionSourceDetected += InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceUpdated += InteractionManager_SourceUpdated;
        InteractionManager.InteractionSourcePressed += InteractionManager_SourcePressed;
    }
    private void OnEnable()
    {
        state = State.NoDrawing;
        
    }

    private void InteractionManager_SourcePressed(InteractionSourcePressedEventArgs obj)
    {

        switch (state)
        {
            case State.NoDrawing:
                {
                    if (gameObject.activeSelf == true)
                    {
                        state = State.FirstClick;
                    }
                    
                    break;
                }
            case State.FirstClick:
                {
                    state = State.Drawing;
                    TrajectoryData.Instance.CreateTrajectory();
                    break;
                }
            case State.Drawing:
                {
                    state = State.NoDrawing;
                    //TrajectoryData.Instance.SaveTrajecroty();
                    break;
                }
        }
    }

    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        if (state != State.NoDrawing)
        {
            
            Vector3 pos = new Vector3(0, 1, 0);
            if (obj.state.source.kind == InteractionSourceKind.Hand)
            {
                obj.state.sourcePose.TryGetPosition(out pos);

                interpolator.SetTargetPosition(pos + offset);

                if (state == State.Drawing)
                {

                    GameObject point = Instantiate(prim, TrajectoryData.Instance.transform);

                    Vector3 a = transform.position;
                    point.transform.position = a;
                    point.transform.Rotate(0, 0, -90);
                    TrajectoryData.Instance.AddPoint(point);
                }

            }
        }
    }

    private void InteractionManager_SourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        if (state != State.NoDrawing)
        {
            if (obj.state.source.kind == InteractionSourceKind.Hand)
            {
                offset = Camera.main.transform.forward / 2;

            }
        }
    }

    private void InteractionManager_SourceLost(InteractionSourceLostEventArgs obj)
    {
        if (state == State.NoDrawing)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
    
}
