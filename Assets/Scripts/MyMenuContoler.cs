using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class MyMenuContoler :  Singleton<MyMenuContoler>
{

    private GameObject iiwa;
    private GameObject Agilus;
    private GameObject MainMenu;
    private GameObject RobotMenu;
    private GameObject ChooseMenu;

    private Stack<GameObject> menuStack;

    public bool ChangePosOri;
    // Use this for initialization
    void Start () {

        ChangePosOri = false;

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
        menuStack.Pop().SetActive(false);
        menuStack.Peek().SetActive(true);
    }

    public void Move()
    {
        
    }
    public void RobotMenuCall()
    {
        ChangeMenu(RobotMenu);

    }
}
