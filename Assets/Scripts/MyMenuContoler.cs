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
    private GameObject TrajectoryMenu;

    private GameObject SelectedRobot;

    public GameObject infoTable;
    private GameObject currentInfoTable;

    private Stack<GameObject> menuStack;

    public bool ChangePosOri;
    private bool GridFlag;
    bool isChangePositionManualy;
    bool firstCallInfo;
    // Use this for initialization
    void Start () {
        GridFlag = false;
        ChangePosOri = false;
        isChangePositionManualy = false;
        firstCallInfo = true;

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
        TrajectoryMenu = GameObject.Find("MenuTrajectory");
        
        RobotMenu.SetActive(false);
        ChooseMenu.SetActive(false);
        TrajectoryMenu.SetActive(false);

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
        //if (SelectedRobot != null) { SelectedRobot = null; }

        menuStack.Pop().SetActive(false);
        menuStack.Peek().SetActive(true);
    }

    public void Move()
    {
        if (SelectedRobot != null)
        {
            SelectedRobot.SendMessage("StartMoving");
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

    public void CreateTrajectory()
    {
        ChangeMenu(TrajectoryMenu);
        if (SelectedRobot != null)
        {
            SelectedRobot.SendMessage("CreateTrajectory");
        }
    }

   
    public void ShowInfoRobot()
    {
        if (firstCallInfo)
        {
            currentInfoTable = Instantiate(infoTable, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)), gameObject.transform);

            currentInfoTable.transform.localPosition = new Vector3(0.6f, -0.03f, -0.05f);
            currentInfoTable.transform.localEulerAngles = new Vector3(0, 25, -15);


            GameObject agilusInfo = currentInfoTable.transform.GetChild(0).gameObject;
            GameObject iiwaInfo = currentInfoTable.transform.GetChild(1).gameObject;
            GameObject cartInfo = currentInfoTable.transform.GetChild(2).gameObject;

            cartInfo.SetActive(false);

            if (SelectedRobot.tag == "agilus")
            {
                iiwaInfo.SetActive(false);
            }
            else if (SelectedRobot.tag == "iiwa")
            {
                agilusInfo.SetActive(false);
            }
            firstCallInfo = false;
        }
        else
        {
            firstCallInfo = true;
            Destroy(currentInfoTable);
        }
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
        if (SelectedRobot != null) {
            RobotControll robot = SelectedRobot.GetComponent<RobotControll>();
            robot.SendState(q);
        }
    }

}
