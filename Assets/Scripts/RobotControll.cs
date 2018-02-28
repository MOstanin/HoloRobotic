using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using MathNet.Numerics.LinearAlgebra;
using HoloToolkit.Examples.InteractiveElements;

public abstract class RobotControll : MonoBehaviour, IInputClickHandler, IRobot {


    public GameObject ball;


    protected float[] q;

    public void OnInputClicked(InputClickedEventData eventData)
    {
        GameObject menu = GameObject.Find("Menu");
        MyMenuContoler.Instance.RobotMenuCall(this.gameObject);

    }
    protected virtual void Start()
    {
        q = ReadState();
    }

    protected virtual void Update()
    {

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

        //Matrix<float> Tgoal = MathOperations.CalcMatrixT(-pos.x, -pos.z, pos.y, -ori.y + Mathf.PI, -ori.z, -ori.x);

        //Debug.Log(ball.transform.localToWorldMatrix.ToString());

        Matrix<float> Tgoal = CreareMatrixT(pos, ori);
        Debug.Log(Tgoal.ToString());

        SendState(InversKin(pos,ori));

        //Debug.Log(ForwardKin(InversKin(Tgoal)));

    }
    


    public void MoveLink(int i, float angle)
    {

        if (q !=  null) {
            q[i] = q[i] + angle;
            SendState(q);
        }
    }
    public void MoveLink(string link)
    {
        if (link != null) { MoveLink(int.Parse(link), Mathf.PI / 20); }
    }

}
