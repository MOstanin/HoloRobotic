using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System;
using UnityEngine.XR.WSA.Input;

public class AppManager : Singleton<AppManager> {

    [SerializeField]
    private GameObject iiwa;
    [SerializeField]
    private GameObject Agilus;
    [SerializeField]
    private GameObject pointerPrefab;
    private GameObject pointer;
    
    private LinkedList<GameObject> RobotMass;

    public GameObject SelectedRobot { private set; get; }

    enum State {Line1, Line2, Acr1, Acr2, NoAction };
    State state;

    public bool AccessToClick;

    private void Start()
    {
        state = State.NoAction;
        AccessToClick = false; ;
        
    }


    public void ClearSelecredRobot()
    {
        CloseInfoRobot();
        SelectedRobot = null;
    }

    public void AddIIWA()
    {
        GameObject newIIWA = Instantiate(iiwa);

        if (RobotMass != null)
        {
            RobotMass.AddLast(newIIWA);
        }
        else
        {
            RobotMass = new LinkedList<GameObject>();
            RobotMass.AddFirst(newIIWA);
        }
        newIIWA.name = "IIWA" + RobotMass.Count;
        newIIWA.SetActive(true);
    }

    public void AddAgilus()
    {
        GameObject newAgilus = Instantiate(Agilus);
        
        if (RobotMass != null)
        {
            RobotMass.AddLast(newAgilus);
        }
        else
        {
            RobotMass = new LinkedList<GameObject>();
            RobotMass.AddFirst(newAgilus);
        }
        newAgilus.name = "Agilus" + RobotMass.Count;
        newAgilus.SetActive(true);
    }

    public void DestroyRobot()
    {
        if (SelectedRobot != null)
        {
            RobotMass.Remove(SelectedRobot);
            Destroy(SelectedRobot);
        }
    }

    public void RobotSelected(GameObject robot)
    {
        if (SelectedRobot != null)
        {
            CloseInfoRobot();
        }
        SelectedRobot = robot;
        MyMenuContoler.Instance.RobotMenuCall(robot.tag);
    }

    public float[] RobotState()
    {
        if (SelectedRobot != null)
        {
            RobotControll robot = SelectedRobot.GetComponent<RobotControll>();
            return robot.ReadState();
        }
        else
        {
            // better send error
            return null;
        }
    }

    public void SendStateToRobot(float[] q)
    {
        if (SelectedRobot != null)
        {
            RobotControll robot = SelectedRobot.GetComponent<RobotControll>();
            robot.SendState(q);
        }
    }

    public void ShowInfoRobot()
    {
        SelectedRobot.SendMessage("ShowInfoRobot");
    }

    public void CloseInfoRobot()
    {
        if (SelectedRobot != null)
        {
            SelectedRobot.SendMessage("CloseInfoRobot");
        }
    }

    public void ShowInfoRobotCartezian()
    {
        SelectedRobot.SendMessage("ShowInfoRobotCartezian");
    }

    public void DrawTrajectory()
    {
        if (pointer == null)
        {
            pointer = Instantiate(pointerPrefab);
            pointer.name = "Pointer";
            pointer.SetActive(false);
            pointer.SetActive(true);
        }
        else
        {
            pointer.SetActive(true);
        }
    }

    public void StopDrawing()
    {
        //pointer.SetActive(false);
        if (pointer != null)
        {
            Destroy(pointer);
        }
    }
    
    public void SaveTrajectoty()
    {
        TrajectoryData.Instance.SaveTrajecroty();
    }

    public void DrawLine()
    {
        state = State.Line1;
        AccessToClick = true; ;
    }

    public void BallClick(GameObject ball)
    {

        if (state == State.Line1)
        {

            ball1 = ball;
            state = State.Line2;

        }
        else if (state == State.Line2)
        {

            ball2 = ball;
            TrajectoryData.Instance.DrawLine(ball1, ball2);
            ball1 = null;
            ball2 = null;
            state = State.NoAction;
            AccessToClick = false;

        }
        else if (state == State.Acr1)
        {
            ball1 = ball;
            state = State.Acr2;

        }
        else if (state == State.Acr2)
        {
            ball2 = ball;

            
            TrajectoryData.Instance.DrawCircle(ball1, ball2);
            ball1 = null;
            ball2 = null;
            state = State.NoAction;
            AccessToClick = false;
        }
    }
    GameObject ball1;
    GameObject ball2;


    public void DrawArc()
    {
        state = State.Acr1;
        AccessToClick = true; ;
    }
}