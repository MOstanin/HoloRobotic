using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using MathNet.Numerics.LinearAlgebra;
using HoloToolkit.Examples.InteractiveElements;
using System;

public abstract class RobotControll : MonoBehaviour, IInputClickHandler, IRobot {

    
    public GameObject InfoTable;
    protected GameObject currentInfoTable;

    private ArrayList Trajectory; 

    //q is goal state
    protected float[] q;
    protected float[] current_q;
    protected Vector3 goalPos;
    protected Vector3 goalOri;
    protected int PointNum;

    enum State { MoveTrajectory, NoMoving, MoveInfoTable }

    State state;

    private float speed = 0.4f;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        //GameObject menu = GameObject.Find("Menu");
        AppManager.Instance.RobotSelected(this.gameObject);

    }

    protected virtual void Start()
    {
        q = ReadState();
        current_q = (float[]) q.Clone();
        state = State.NoMoving;
    }

    protected virtual void Update()
    {
        switch (state){
            case State.MoveTrajectory:
                {
                    if (!QisEquale())
                    {
                        RobotMoving();
                    }
                    SendInfoTable();
                    break;
                }
            case State.MoveInfoTable:
                {
                    ReadInfoTable();
                    if (!QisEquale())
                    {
                        RobotMoving();
                    }
                    break;
                }
            case State.NoMoving:
                {
                    SendInfoTable();
                    break;
                }
        }
        UpdateState();
        UpdateMoving();

    }

    private void UpdateState()
    {
        if (currentInfoTable != null)
        {
            InfoControlBase infoControlBase = currentInfoTable.GetComponentInChildren<InfoControlBase>();
            if (infoControlBase.ControlMode)
            {
                state = State.MoveInfoTable;
            }
            else
            {
                if (state == State.MoveInfoTable)
                {
                    state = State.NoMoving;
                }
            }
        }
    }

    private void SendInfoTable()
    {
        if (currentInfoTable != null)
        {
            switch (currentInfoTable.tag)
            {
                case "robot":
                    {
                        InfoControlBase infoControlBase = currentInfoTable.GetComponentInChildren<InfoControlBase>();
                        infoControlBase.ReadRobotState(current_q);
                        break;
                    }
                case "cartezian":
                    {
                        InfoControlBase infoControlBase = currentInfoTable.GetComponentInChildren<InfoControlBase>();
                        float[] mas = ReadStateEE();
                        infoControlBase.ReadCartezianState(mas);
                        break;
                    }
            }
        }
    }

    private void ReadInfoTable()
    {
        if (currentInfoTable != null)
        {
            switch (currentInfoTable.tag)
            {
                case "robot":
                    {
                        InfoControlBase infoControlBase =  currentInfoTable.GetComponentInChildren<InfoControlBase>();
                        q = infoControlBase.InfoState();
                        break;
                    }
                case "cartezian":
                    {
                        break;
                    }
            }
        }
    }

    protected void UpdateMoving()
    {
        switch (state)
        {
            case State.MoveTrajectory:
                {
                    if (QisEquale())
                    {
                        PointNum = PointNum + 1;

                        if (PointNum < Trajectory.Count)
                        {


                            Transform ballTransform = ((GameObject)Trajectory[PointNum]).transform;
                            goalPos = transform.InverseTransformPoint(ballTransform.position) * 1000;
                            goalOri.x = -(transform.eulerAngles.x - ballTransform.eulerAngles.x) * Mathf.PI / 180;
                            goalOri.y = -(transform.eulerAngles.y - ballTransform.eulerAngles.y) * Mathf.PI / 180;
                            goalOri.z = -(transform.eulerAngles.z - ballTransform.eulerAngles.z) * Mathf.PI / 180;

                            q = InversKin(goalPos, goalOri);

                        }
                        else
                        {
                            state = State.NoMoving;
                        }
                    }
                    break;
                }
                
        }
        
    }

    protected bool QisEquale()
    {
        for (int i = 0; i < q.Length; i++)
        {
            if (q[i]!= current_q[i]) { return false; }
        }
        return true;
    }

    protected void RobotMoving()
    {
        float[] del_q = new float[q.Length];
        for (int i = 0; i < q.Length; i++)
        {
            del_q[i] = q[i] - current_q[i];
        }

        float maxDel = Mathf.Max(del_q);
        float minDel = Mathf.Min(del_q);

        float s = Mathf.Max(Mathf.Abs(maxDel), Mathf.Abs(minDel));

        if (s > speed * Time.deltaTime)
        {
            for (int i = 0; i < q.Length; i++)
            {
                current_q[i] = current_q[i] + speed * Time.deltaTime * del_q[i] / s;
            }
        }
        else
        {
            for (int i = 0; i < q.Length; i++)
            {
                current_q[i] = current_q[i] + del_q[i];
            }
        }
        SendState(current_q);

    }

    public abstract Matrix<float> ForwardKin(float[] q);
    public abstract float[] InversKin(Matrix<float> Tgoal, float[] q0);
    public abstract void SendState(float[] q);
    public abstract float[] ReadState();
    public abstract float[] InversKin(Matrix<float> Tgoal);
    public abstract Matrix<float> CreareMatrixT(Vector3 pos, Vector3 ori);

    private float[] ReadStateEE()
    {
        Matrix<float> T = ForwardKin(current_q);
        float[] m = new float[6];
        m[0] = T[0, 3];
        m[1] = T[1, 3];
        m[2] = T[2, 3];

        m[3] = Mathf.Atan2(T[1, 0], T[0, 0]);
        m[5] = Mathf.Atan2(T[2, 1], T[2, 2]);

        m[4] = Mathf.Atan2(-T[2, 0], T[0, 0] / Mathf.Cos(m[3]));
        
        return m;
    }

    public float[] InversKin(Vector3 pos, Vector3 ori)
    {
        Matrix<float> Tgoal = CreareMatrixT(pos, ori);
        
        return InversKin(Tgoal);
    }
    
    public void StartMoving()
    {
        

        Trajectory = TrajectoryData.Instance.GetTrajectory();

        if (Trajectory != null)
        {
            PointNum = 0;
            state = State.MoveTrajectory;

            Transform ballTransform = ((GameObject)Trajectory[PointNum]).transform;
            goalPos = transform.InverseTransformPoint(ballTransform.position) * 1000;
            goalOri.x = -(transform.eulerAngles.x - ballTransform.eulerAngles.x) * Mathf.PI / 180;
            goalOri.y = -(transform.eulerAngles.y - ballTransform.eulerAngles.y) * Mathf.PI / 180;
            goalOri.z = -(transform.eulerAngles.z - ballTransform.eulerAngles.z) * Mathf.PI / 180;

            q = InversKin(goalPos, goalOri);

        }
    }
    
    public void Move()
    {
        
    }

    public void ShowInfoRobot()
    {

        currentInfoTable = Instantiate(InfoTable, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)), 
            MyMenuContoler.Instance.gameObject.transform);

        currentInfoTable.transform.localPosition = new Vector3(0.6f, -0.03f, -0.05f);
        currentInfoTable.transform.localEulerAngles = new Vector3(0, 25, -15);


        GameObject agilusInfo = currentInfoTable.transform.GetChild(0).gameObject;
        GameObject iiwaInfo = currentInfoTable.transform.GetChild(1).gameObject;
        GameObject cartInfo = currentInfoTable.transform.GetChild(2).gameObject;
        currentInfoTable.tag = "robot";
        cartInfo.SetActive(false);

        if (this.tag == "agilus")
        {
            iiwaInfo.SetActive(false);
        }
        else if (this.tag == "iiwa")
        {
            agilusInfo.SetActive(false);
        }

    }

    public void CloseInfoRobot()
    {
        Destroy(currentInfoTable);
    }

    public void AllowPlacing()
    {
        state = State.NoMoving;
    }

    public void ShowInfoRobotCartezian()
    {
        currentInfoTable = Instantiate(InfoTable, new Vector3(0, 0, 0), Quaternion.Euler(new Vector3(0, 0, 0)),
            MyMenuContoler.Instance.gameObject.transform);

        currentInfoTable.transform.localPosition = new Vector3(0.6f, -0.03f, -0.05f);
        currentInfoTable.transform.localEulerAngles = new Vector3(0, 25, -15);


        GameObject agilusInfo = currentInfoTable.transform.GetChild(0).gameObject;
        GameObject iiwaInfo = currentInfoTable.transform.GetChild(1).gameObject;
        GameObject cartInfo = currentInfoTable.transform.GetChild(2).gameObject;
        currentInfoTable.tag = "cartezian";
        
        iiwaInfo.SetActive(false);
        
        agilusInfo.SetActive(false);
        
    }

}
