using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour {

    [SerializeField]
    private GameObject ballMenu;
    [SerializeField]
    private GameObject ballFigure;
    [SerializeField]
    private GameObject oriYZ;

    enum State { ManualPos, NormalPos, OriYZ, OriX, Wait}
    State state;

	// Use this for initialization
	void Start () {
        state = State.Wait;
        ballMenu.SetActive(false);

    }

    public void ChangePosition()
    {
        state = State.ManualPos;
        ballFigure.SendMessage("StartTranslation");
        ballMenu.SetActive(false);
    }
    public void PositionNormal()
    {
        state = State.NormalPos;
        ballFigure.SendMessage("AllowPlacing");
        ballMenu.SetActive(false);
    }
    public void OrientationZY()
    {
        state = State.OriYZ;
        oriYZ.SendMessage("StartOrientationYZ");
        ballMenu.SetActive(false);
    }
    public void OrientationX()
    {
        state = State.OriX;
        ballFigure.SendMessage("StartRotation");
        ballMenu.SetActive(false);
    }

    public void ClickOnBall()
    {
        switch (state)
        {
            case State.Wait:
                {
                    ballMenu.SetActive(!ballMenu.activeSelf);
                    break;
                }
            case State.ManualPos:
                {
                    ballFigure.SendMessage("StopTranslation");
                    state = State.Wait;
                    break;
                }
            case State.NormalPos:
                {
                    ballFigure.SendMessage("StopPlacingCall");
                    state = State.Wait;
                    break;
                }
            case State.OriYZ:
                {
                    oriYZ.SendMessage("StopOrientationYZ");
                    state = State.Wait;
                    break;
                }
            case State.OriX:
                {
                    ballFigure.SendMessage("StopRotation");
                    state = State.Wait;
                    break;
                }
        }
    }
}
