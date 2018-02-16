using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using MathNet.Numerics.LinearAlgebra;

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
        Vector3 pos = new Vector3(EE[0,3]/1000, (EE[2, 3])/1000, (EE[1, 3])/1000);
        ball.transform.localPosition = pos;
        ball.transform.Rotate(0, 180, 0);

        ball.SendMessage("StartTranslation");
    }

    public abstract Matrix<float> ForwardKin(float[] q);
    public abstract float[] InversKin(Matrix<float> Tgoal, float[] q0);
    public abstract void SendState(float[] q);
    public abstract float[] ReadState();
    public abstract float[] InversKin(Matrix<float> Tgoal);

    public void Move()
    {
        Vector3 pos = ball.transform.localPosition;


        Matrix<float> Tgoal = MathOperations.CalcMatrixT(-pos.x * 1000, -pos.z * 1000, pos.y * 1000);

        SendState(InversKin(Tgoal));

    }
}
