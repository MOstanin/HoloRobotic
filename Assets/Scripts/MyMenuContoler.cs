using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

public class MyMenuContoler :  Singleton<MyMenuContoler>
{

    private GameObject iiwa;
    private GameObject Agilus;
    private GameObject MainMenu;
    private GameObject RobotMenu;
    private GameObject ChooseMenu;

    private GameObject SelectedRobot;

    private Stack<GameObject> menuStack;

    public bool ChangePosOri;
    private bool GridFlag;
    bool isChangePositionManualy;
    // Use this for initialization
    void Start () {
        GridFlag = false;
        ChangePosOri = false;
        isChangePositionManualy = false;

        iiwa = GameObject.Find("IIWA");
        if (iiwa != null)
        {
            iiwa.SetActive(false);
        }
        Agilus = GameObject.Find("Agilus");
        if (Agilus != null)
        {
            Agilus.SetActive(false);
        }
        MainMenu = GameObject.Find("MainMenu");
        RobotMenu = GameObject.Find("MenuRobot");
        ChooseMenu = GameObject.Find("MenuChoose");
        
        RobotMenu.SetActive(false);
        ChooseMenu.SetActive(false);

        menuStack = new Stack<GameObject>();
        menuStack.Push(MainMenu);

        //ChangeMenu(RobotMenu);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Exit()
    {
        Application.Quit();
    }
    
    public void AddIIWA()
    {
        GameObject newIIWA = Instantiate(iiwa);
        newIIWA.name = "IIWA" + 1.ToString();
        newIIWA.SetActive(true);
    }

    public void AddAgilus()
    {
        GameObject newAgilus = Instantiate(Agilus);
        newAgilus.name = "Agilus" + 1.ToString();
        newAgilus.SetActive(true);
    }

    public void AddRobot()
    {
        ChangeMenu(ChooseMenu);
    }

    private void ChangeMenu(GameObject menu)
    {
        menuStack.Peek().SetActive(false);
        menuStack.Push(menu);
        menu.SetActive(true);
    }

    public void Back()
    {
        if (SelectedRobot != null) { SelectedRobot = null; }

        menuStack.Pop().SetActive(false);
        menuStack.Peek().SetActive(true);
    }

    public void Move()
    {
        if (SelectedRobot != null)
        {
            SelectedRobot.SendMessage("Move");
        }
    }
    public void RobotMenuCall(GameObject robot)
    {
        ChangeMenu(RobotMenu);

        SelectedRobot = robot;

    }
    public void ShowGrid()
    {
        GridFlag = !GridFlag;

        if (SpatialMappingManager.Instance != null)
        {
            SpatialMappingManager.Instance.DrawVisualMeshes = GridFlag;
            if (GridFlag)
            {
                SpatialMappingManager.Instance.StartObserver();
            }
            else
            {
                SpatialMappingManager.Instance.StopObserver();
            }
        } 

    }

    public void ChangePosition()
    {
        if (SelectedRobot != null)
        {
            SelectedRobot.SendMessage("AllowPlacing");
        }
    }

    
    public void ChangePositionManualy()
    {
        isChangePositionManualy = !isChangePositionManualy;
        if (SelectedRobot != null)
        {
            if (isChangePositionManualy)
            {
                SelectedRobot.SendMessage("StartTranslation");
                SelectedRobot.SendMessage("StartRotation");
            }
            else
            {
                SelectedRobot.SendMessage("StopTranslation");
                SelectedRobot.SendMessage("StopRotation");
            }
                
        }
    }



    public void AddPoint()
    {
        if (SelectedRobot != null)
        {
            SelectedRobot.SendMessage("CreatePoint");
        }
    }
}
