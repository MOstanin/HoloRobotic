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
        UpdateMoving();
    }

    protected void UpdateMoving()
    {
        if (MovingFlag)
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

        Trajectory = TrajectoryData.Instance.GetTrajectory();

        if (Trajectory != null)
        {
            PointNum = 0;

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
        Vector3 pos = ball.transform.localPosition * 1000;
        Vector3 ori = ball.transform.localEulerAngles * Mathf.PI / 180;
        
        //Matrix<float> Tgoal = CreareMatrixT(pos, ori);
        //Debug.Log(Tgoal.ToString());

        q = InversKin(pos,ori);

        //Debug.Log(ForwardKin(InversKin(Tgoal)));
    }


}
