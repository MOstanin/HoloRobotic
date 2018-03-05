using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using MathNet.Numerics.LinearAlgebra;
using HoloToolkit.Examples.InteractiveElements;

public abstract class RobotControll : MonoBehaviour, IInputClickHandler, IRobot {


    public GameObject ball;

    //q is goal state
    protected float[] q;
    protected float[] current_q;

    private float speed = 0.4f;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        GameObject menu = GameObject.Find("Menu");
        MyMenuContoler.Instance.RobotMenuCall(this.gameObject);

    }
    protected virtual void Start()
    {
        q = ReadState();
        current_q = (float[]) q.Clone();
    }

    protected virtual void Update()
    {
        if (!QisEquale())
        {
            RobotMoving();
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
