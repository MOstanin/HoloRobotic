using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

public class MyMenuContoler :  Singleton<MyMenuContoler>
{


    private GameObject MainMenu;
    private GameObject RobotMenu;
    private GameObject ChooseMenu;
    private GameObject TrajectoryMenu;
    private GameObject TrajectoryData;

    private Stack<GameObject> menuStack;

    public bool ChangePosOri;
    private bool GridFlag;
    bool isChangePositionManualy;
    bool InfoRobotIsClosed;
    bool InfoCartIsClosed;
    private bool DrawingIsRuning;

    // Use this for initialization
    void Start () {
        GridFlag = false;
        ChangePosOri = false;
        isChangePositionManualy = false;
        InfoRobotIsClosed = true;
        InfoCartIsClosed = true;
        DrawingIsRuning = false;

        MainMenu = GameObject.Find("MainMenu");
        RobotMenu = GameObject.Find("MenuRobot");
        ChooseMenu = GameObject.Find("MenuChoose");
        TrajectoryMenu = GameObject.Find("MenuTrajectory");
        TrajectoryData = GameObject.Find("TrajectoryData");


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
        AppManager.Instance.AddIIWA();
    }

    public void AddAgilus()
    {
        AppManager.Instance.AddAgilus();
    }

    public void AddRobot()
    {
        ChangeMenu(ChooseMenu);
    }

    private void ChangeMenu(GameObject menu)
    {
        if (menuStack.Peek().name != menu.name)
        {
            menuStack.Peek().SetActive(false);
            menuStack.Push(menu);
            menu.SetActive(true);
        }
    }

    public void Back()
    {
        //if (SelectedRobot != null) { SelectedRobot = null; }
        
        menuStack.Pop().SetActive(false);
        menuStack.Peek().SetActive(true);

        if (menuStack.Peek() != RobotMenu)
        {
            AppManager.Instance.ClearSelecredRobot();
        }
        AppManager.Instance.StopDrawing();
    }

    public void Move()
    {
        if (AppManager.Instance.SelectedRobot != null)
        {
            AppManager.Instance.SelectedRobot.SendMessage("StartMoving");
        }
    }

    public void RobotMenuCall(string tag)
    {
        ChangeMenu(RobotMenu);
        switch (tag){
            case "iiwa":
                {
                    TextMesh textMesh =  GameObject.Find("RobotLable").GetComponent<TextMesh>();
                    textMesh.text = "IIWA";
                    break;
                }
            case "agilus":
                {
                    TextMesh textMesh = GameObject.Find("RobotLable").GetComponent<TextMesh>();
                    textMesh.text = "Agilus";
                    break;
                }
        }
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
        if (AppManager.Instance.SelectedRobot != null)
        {
            AppManager.Instance.SelectedRobot.SendMessage("AllowPlacing");
        }
    }

    public void ChangePositionManualy()
    {
        isChangePositionManualy = !isChangePositionManualy;
        if (AppManager.Instance.SelectedRobot != null)
        {
            if (isChangePositionManualy)
            {
                AppManager.Instance.SelectedRobot.SendMessage("StartTranslation");
                AppManager.Instance.SelectedRobot.SendMessage("StartRotation");
            }
            else
            {
                AppManager.Instance.SelectedRobot.SendMessage("StopTranslation");
                AppManager.Instance.SelectedRobot.SendMessage("StopRotation");
            }
                
        }
    }
    
    public void AddPoint()
    {
        
        TrajectoryData.SendMessage("CreateMainPoint");
       
    }

    public void CreateTrajectory()
    {
        ChangeMenu(TrajectoryMenu);
        TrajectoryData.SendMessage("CreateTrajectory");
        

    }

    public void ShowInfoRobot()
    {
        if (InfoRobotIsClosed)
        {
            if (!InfoCartIsClosed)
            {
                InfoCartIsClosed = true;
                AppManager.Instance.CloseInfoRobot();
            }
            AppManager.Instance.ShowInfoRobot();
            InfoRobotIsClosed = false;
        }
        else
        {
            InfoRobotIsClosed = true;
            AppManager.Instance.CloseInfoRobot();
        }
    }

    public void ShowInfoRobotCartezian()
    {
        if (InfoCartIsClosed)
        {
            if (!InfoRobotIsClosed) {
                InfoRobotIsClosed = true;
                AppManager.Instance.CloseInfoRobot();
            }
            AppManager.Instance.ShowInfoRobotCartezian();
            InfoCartIsClosed = false;
        }
        else
        {
            InfoCartIsClosed = true;
            AppManager.Instance.CloseInfoRobot();
        }
    }

    public void DestroyRobot()
    {
        AppManager.Instance.DestroyRobot();
        Back();
    }

    public void DrawTrajectory()
    {
        
        if (!DrawingIsRuning)
        {
            AppManager.Instance.DrawTrajectory();
            DrawingIsRuning = true;
            //change label
        }
        else
        {
            AppManager.Instance.StopDrawing();
            DrawingIsRuning = false;
            //change label
        }
        
        //AppManager.Instance.DrawTrajectory();
        
    }

    public void SaveTrajectoty()
    {
        AppManager.Instance.SaveTrajectoty();
        Back();
    }

    public void DrawLine()
    {
        AppManager.Instance.DrawLine();
    }

    public void DrawArc()
    {
        AppManager.Instance.DrawArc();
    }

    public void DeleteSegment()
    {
        AppManager.Instance.DeleteSegment();
    }

    public void Scale()
    {
        AppManager.Instance.Scale();
    }
}
