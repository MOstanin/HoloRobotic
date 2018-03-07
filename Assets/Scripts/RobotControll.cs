using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using MathNet.Numerics.LinearAlgebra;
using HoloToolkit.Examples.InteractiveElements;

public abstract class RobotControll : MonoBehaviour, IInputClickHandler, IRobot {


    public GameObject ball;

    private ArrayList Trajectory; 

    //q is goal state
    protected float[] q;
    protected float[] current_q;
    protected Vector3 goalPos;
    protected Vector3 goalOri;
    protected int PointNum;


    private float speed = 0.4f;
    private bool MovingFlag;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        //GameObject menu = GameObject.Find("Menu");
        MyMenuContoler.Instance.RobotMenuCall(this.gameObject);

    }
    protected virtual void Start()
    {
        q = ReadState();
        current_q = (float[]) q.Clone();
        MovingFlag = false;
    }

    protected virtual void Update()
    {
        if (MovingFlag)
        {
            if (!QisEquale())
            {
                RobotMoving();
            }
        }
        CheckMovingFlag();
    }

    protected void CheckMovingFlag()
    {
        if (MovingFlag)
        {
            if (QisEquale())
            {
                PointNum = PointNum + 1;

                if (PointNum < Trajectory.Count)
                {
                   // goalPos = ((Vector3[])Trajectory[PointNum])[0];
                   // goalOri = ((Vector3[])Trajectory[PointNum])[1];


                    goalPos = ((GameObject)Trajectory[PointNum]).transform.localPosition * 1000;
                    goalOri = ((GameObject)Trajectory[PointNum]).transform.localEulerAngles * Mathf.PI / 180;

                    q = InversKin(goalPos, goalOri);

                }
                else
                {
                    MovingFlag = false;
                }
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


    public void CreatePoint()
    {
        Matrix<float> EE = ForwardKin(ReadState());
        
        ball = Instantiate(ball, new Vector3(0,0,0), Quaternion.Euler(new Vector3(0, 0, 0)),gameObject.transform);

        float scale = gameObject.transform.localScale.x;
        //ball.transform.localScale = new Vector3(10 / (scale*scale), 10 / (scale * scale), 10 / (scale * scale));
//        Vector3 pos = new Vector3(EE[0,3]/1000, (EE[2, 3])/1000, (EE[1, 3])/1000);
        Vector3 pos = new Vector3(-0.2f, 1, 0);

        ball.transform.localPosition = pos;
        ball.transform.Rotate(0, 180, 0);

        ball.SendMessage("StartTranslation");

        if (Trajectory != null)
        {
            
            pos = ball.transform.localPosition * 1000;
            Vector3 ori = ball.transform.localEulerAngles * Mathf.PI / 180;
            //Trajectory.Add(new Vector3[] { pos, ori});

            Trajectory.Add(ball);


        }


    }

    public abstract Matrix<float> ForwardKin(float[] q);
    public abstract float[] InversKin(Matrix<float> Tgoal, float[] q0);
    public abstract void SendState(float[] q);
    public abstract float[] ReadState();
    public abstract float[] InversKin(Matrix<float> Tgoal);
    public abstract Matrix<float> CreareMatrixT(Vector3 pos, Vector3 ori);

    public float[] InversKin(Vector3 pos, Vector3 ori)
    {
        Matrix<float> Tgoal = CreareMatrixT(pos, ori);
        
        return InversKin(Tgoal);
    }
    
    public void StartMoving()
    {
        MovingFlag = true;

        if (Trajectory != null)
        {
            PointNum = 0;
            //goalPos = ((Vector3[])Trajectory[PointNum])[0];
            //goalOri = ((Vector3[])Trajectory[PointNum])[1];
            goalPos = ((GameObject) Trajectory[PointNum]).transform.localPosition * 1000;
            goalOri = ((GameObject)Trajectory[PointNum]).transform.localEulerAngles * Mathf.PI / 180;



            q = InversKin(goalPos, goalOri);

        }
    }


    public void Move()
    {
        Vector3 pos = ball.transform.localPosition * 1000;
        Vector3 ori = ball.transform.localEulerAngles * Mathf.PI / 180;
        
        //Matrix<float> Tgoal = CreareMatrixT(pos, ori);
        //Debug.Log(Tgoal.ToString());

        q = InversKin(pos,ori);

        //Debug.Log(ForwardKin(InversKin(Tgoal)));
    }

    public void CreateTrajectory()
    {
        if (Trajectory != null)
        {
            Trajectory.Clear();
        }
        else
        {
            Trajectory = new ArrayList();
        }
    }

}
