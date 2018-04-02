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

    enum State { NoDrawing,  FirstClick, Drawing, SecondClick, Destroying};
    State state;

    // Use this for initialization
    void Start () {
        interpolator = gameObject.EnsureComponent<Interpolator>();
        offset = new Vector3(0, 0, 0);
        state = State.NoDrawing;

        InteractionManager.InteractionSourceLost += InteractionManager_SourceLost;
        InteractionManager.InteractionSourceDetected += InteractionManager_SourceDetected;
        InteractionManager.InteractionSourceUpdated += InteractionManager_SourceUpdated;
        InteractionManager.InteractionSourcePressed += InteractionManager_SourcePressed;
    }

    private void InteractionManager_SourcePressed(InteractionSourcePressedEventArgs obj)
    {

        switch (state)
        {
            case State.NoDrawing:
                {
                    state = State.Drawing;
                    TrajectoryData.Instance.CreateTrajectory();
                    break;
                }
            case State.Drawing:
                {
                    state = State.Destroying;
                    TrajectoryData.Instance.SaveTrajecroty();
                    
                    break;
                }
        }
    }

    private void InteractionManager_SourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        Vector3 pos = new Vector3(0,1,0);
        if (obj.state.source.kind == InteractionSourceKind.Hand)
        {
            obj.state.sourcePose.TryGetPosition(out pos);

            interpolator.SetTargetPosition(pos+offset);

            if (state == State.Drawing)
            {
                GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.transform.parent = TrajectoryData.Instance.transform;
                point.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                point.transform.position = transform.position;
                TrajectoryData.Instance.AddPoint(point);
            }
      
        }
        
    }

    private void InteractionManager_SourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        
        if (obj.state.source.kind == InteractionSourceKind.Hand)
        {
            offset = Camera.main.transform.forward / 2.2f;

        }
    }

    private void InteractionManager_SourceLost(InteractionSourceLostEventArgs obj)
    {
        if (state == State.Destroying)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
    
}
