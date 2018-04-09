using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class MenuBall : MonoBehaviour {

    [SerializeField]
    private GameObject parentBall;

    private Interpolator interpolator;

	// Use this for initialization
	void Start () {
        interpolator = gameObject.GetComponent<Interpolator>();

    }
	
	// Update is called once per frame
	void Update () {
        interpolator.SetTargetRotation(Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up));
	}

    public void ChangePosition(){
        parentBall.GetComponent<BallManager>().ChangePosition();
    }
    public void PositionNormal()
    {
        parentBall.GetComponent<BallManager>().PositionNormal();
    }
    public void OrientationZY()
    {
        parentBall.GetComponent<BallManager>().OrientationZY();
    }
    public void OrientationX()
    {
        parentBall.GetComponent<BallManager>().OrientationX();
    }
}
