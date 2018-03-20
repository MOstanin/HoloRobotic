﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using System;

public class AppManager : Singleton<AppManager> {

    [SerializeField]
    private GameObject iiwa;
    [SerializeField]
    private GameObject Agilus;

    [SerializeField]
    private GameObject InfoTable;

    private GameObject currentInfoTable;

    private LinkedList<GameObject> RobotMass;

    public GameObject SelectedRobot { private set; get; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

    public void RobotSelected(GameObject robot)
    {
        if (SelectedRobot != null)
        {
            CloseInfoRobot();
        }
        SelectedRobot = robot;
        MyMenuContoler.Instance.RobotMenuCall();
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
        SelectedRobot.SendMessage("CloseInfoRobot");
    }

    public void ShowInfoRobotCartezian()
    {
        SelectedRobot.SendMessage("ShowInfoRobotCartezian");
    }
}