using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class AgilusControl : RobotControll{

    public GameObject link1;
    public GameObject link2;
    public GameObject link3;
    public GameObject link4;
    public GameObject link5;
    public GameObject link6;

    //float[] q;



    // Use this for initialization
    protected override void Start () {

        base.Start();

        link1 = GameObject.Find("link1");
        link2 = GameObject.Find("link2");
        link3 = GameObject.Find("link3");
        link4 = GameObject.Find("link4");
        link5 = GameObject.Find("link5");
        link6 = GameObject.Find("link6");

        
        
    }

    protected override void Update()
    {
        
    }


    public override Matrix<float> ForwardKin(float[] q)
    {

        Matrix<float> H = MathOperations.MatrixTz4(400) * MathOperations.MatrixRz4(q[0]) * MathOperations.MatrixTx4(25) *
            MathOperations.MatrixRy4(q[1]-Mathf.PI/2) * MathOperations.MatrixTx4(560) * MathOperations.MatrixRy4(q[2]+Mathf.Atan2(515,35)) *
            MathOperations.MatrixTx4(Mathf.Sqrt(35*35+515*515)) * MathOperations.MatrixRy4(Mathf.PI / 2 - Mathf.Atan2(515, 35)) *
            MathOperations.MatrixRx4(q[3]) * MathOperations.MatrixRy4(q[4]) * MathOperations.MatrixRx4(q[5]) * MathOperations.MatrixTx4(80);

        return H;
    }

    private Matrix<float> ForwardKin2(float[] q)
    {

        Matrix<float> T1 = MathOperations.CalcMatrixDH(q[0], 400, 25, Mathf.PI / 2);
        Matrix<float> T2 = MathOperations.CalcMatrixDH(q[1] + Mathf.PI / 2, 0, 560, 0);
        Matrix<float> T3 = MathOperations.CalcMatrixDH(q[2], 0, 35, Mathf.PI / 2);
        Matrix<float> T4 = MathOperations.CalcMatrixDH(q[3], 515, 0, -Mathf.PI / 2);
        Matrix<float> T5 = MathOperations.CalcMatrixDH(q[4], 0, 0, Mathf.PI / 2);
        Matrix<float> T6 = MathOperations.CalcMatrixDH(q[5], 80, 0, 0);

        Matrix<float> H = T1 * T2 * T3 * T4 * T5 * T6;
        return H;
    }

    private Matrix<float> AGILUSjacobian(float[] q)
    {

        Matrix<float> A1 = MathOperations.CalcMatrixDH(q[0], 400, 25, Mathf.PI / 2);
        Matrix<float> A2 = MathOperations.CalcMatrixDH(q[1] + Mathf.PI / 2, 0, 560, 0);
        Matrix<float> A3 = MathOperations.CalcMatrixDH(q[2], 0, 35, Mathf.PI / 2);
        Matrix<float> A4 = MathOperations.CalcMatrixDH(q[3], 515, 0, -Mathf.PI / 2);
        Matrix<float> A5 = MathOperations.CalcMatrixDH(q[4], 0, 0, Mathf.PI / 2);
        Matrix<float> A6 = MathOperations.CalcMatrixDH(q[5], 80, 0, 0);

        Matrix<float> T1 = A1;
        Matrix<float> T2 = A1.Multiply(A2);
        Matrix<float> T3 = A1.Multiply(A2.Multiply(A3));
        Matrix<float> T4 = A1.Multiply(A2.Multiply(A3.Multiply(A4)));
        Matrix<float> T5 = A1.Multiply(A2.Multiply(A3.Multiply(A4.Multiply(A5))));
        Matrix<float> T6 = A1.Multiply(A2.Multiply(A3.Multiply(A4.Multiply(A5.Multiply(A6)))));

        Vector3 z0 = new Vector3(0, 0, 1);
        Vector3 z1 = new Vector3(T1[0, 2], T1[1, 2], T1[2, 2]);
        Vector3 z2 = new Vector3(T2[0, 2], T2[1, 2], T2[2, 2]);
        Vector3 z3 = new Vector3(T3[0, 2], T3[1, 2], T3[2, 2]);
        Vector3 z4 = new Vector3(T4[0, 2], T4[1, 2], T4[2, 2]);
        Vector3 z5 = new Vector3(T5[0, 2], T5[1, 2], T5[2, 2]);
        Vector3 z6 = new Vector3(T6[0, 2], T6[1, 2], T6[2, 2]);

        Vector3 p0 = new Vector3(0, 0, 0);
        Vector3 p1 = new Vector3(T1[0, 3], T1[1, 3], T1[2, 3]);
        Vector3 p2 = new Vector3(T2[0, 3], T2[1, 3], T2[2, 3]);
        Vector3 p3 = new Vector3(T3[0, 3], T3[1, 3], T3[2, 3]);
        Vector3 p4 = new Vector3(T4[0, 3], T4[1, 3], T4[2, 3]);
        Vector3 p5 = new Vector3(T5[0, 3], T5[1, 3], T5[2, 3]);
        Vector3 p6 = new Vector3(T6[0, 3], T6[1, 3], T6[2, 3]);

        Matrix<float> J =  Matrix<float>.Build.DenseOfArray(new float[,]{
            {Vector3.Cross (z0, p6 - p0).x, Vector3.Cross (z1, p6 - p1).x , Vector3.Cross (z2, p6 - p2).x, Vector3.Cross (z3, p6 - p3).x, Vector3.Cross (z4, p6 - p4).x, Vector3.Cross (z5, p6 - p5).x},
            {Vector3.Cross (z0, p6 - p0).y, Vector3.Cross (z1, p6 - p1).y , Vector3.Cross (z2, p6 - p2).y, Vector3.Cross (z3, p6 - p3).y, Vector3.Cross (z4, p6 - p4).y, Vector3.Cross (z5, p6 - p5).y},
            {Vector3.Cross (z0, p6 - p0).z, Vector3.Cross (z1, p6 - p1).z , Vector3.Cross (z2, p6 - p2).z, Vector3.Cross (z3, p6 - p3).z, Vector3.Cross (z4, p6 - p4).z, Vector3.Cross (z5, p6 - p5).z},
            {z0.x, z1.x, z2.x, z3.x, z4.x, z5.x},
            {z0.y, z1.y, z2.y, z3.y, z4.y, z5.y},
            {z0.z, z1.z, z2.z, z3.z, z4.z, z5.z}
        });

        return J;
    }

    public override float[] InversKin(Matrix<float> Tgoal)
    {
        //Tgoal = Tgoal * MathOperations.MatrixRz4(Mathf.PI);

        Matrix<float> T0 = Tgoal * MathOperations.MatrixTx4(80).Inverse();


        float q1 = Mathf.Atan2(T0[1, 3], T0[0, 3]);
        float a = Mathf.Sqrt(T0[1, 3] * T0[1, 3] + T0[0, 3] * T0[0, 3])-25;
        float b = T0[2, 3] - 400;
        float c3 = (a * a + b * b - 560 * 560 - 516 * 516) / (2 * 560 * 516);
        float s3 = Mathf.Sqrt(1 - c3 * c3);
        float del_q3 = Mathf.Atan2(515, 35);
        float q3 = Mathf.Atan2(s3, c3) - del_q3;
        float q2 = -Mathf.Atan2(516 * Mathf.Sin(q3 + del_q3), (560 + 516 * Mathf.Cos(q3 + del_q3))) + Mathf.Atan2(a, b);

        Matrix<float> T123 = MathOperations.MatrixTz4(400) * MathOperations.MatrixRz4(q1) * MathOperations.MatrixTx4(25) *
            MathOperations.MatrixRy4(q2 - Mathf.PI / 2) * MathOperations.MatrixTx4(560) * MathOperations.MatrixRy4(q3 + Mathf.Atan2(515, 35)) *
            MathOperations.MatrixTx4(Mathf.Sqrt(35 * 35 + 515 * 515)) * MathOperations.MatrixRy4(Mathf.PI / 2 - Mathf.Atan2(515, 35));
        Matrix<float> T456 = T123.Inverse() * T0;

        
        float q4, q5, q6;

        float nx = MathOperations.MyRround(T456[0, 0]);
        float ny = MathOperations.MyRround(T456[1, 0]);
        float nz = MathOperations.MyRround(T456[2, 0]);

        float ax = MathOperations.MyRround(T456[0, 2]);
        float sx = MathOperations.MyRround(T456[0, 1]);

        if (nx != 1) {

            q4 = Mathf.Atan2(ny, -nz);
            q6 = Mathf.Atan2(sx, ax);
            if (ax != 0)
            {
                q5 = Mathf.Atan2(ax, nx * Mathf.Abs( Mathf.Cos(q6)));
            }
            else
            {
                q5 = Mathf.Atan2(sx, nx * Mathf.Abs(Mathf.Sin(q6)));
            }
        }
        else
        {
            q5 = Mathf.Acos(T456[0, 0]);
            q4 = 0;
            q6 = Mathf.Atan2(T456[2, 1], T456[1, 1]);
        }

        q = new float[] { q1, q2, q3, q4, q5 , q6 };
        return q;
    }

    public override float[] ReadState()
    {
        float[] qr = new float[6];

        qr[0] = -link1.transform.localRotation.eulerAngles.z * Mathf.PI / 180;
        qr[1] = -(link2.transform.localRotation.eulerAngles.y - 90) * Mathf.PI / 180;
        qr[2] = -(link3.transform.localRotation.eulerAngles.y + 90) * Mathf.PI / 180;
        qr[3] = link4.transform.localRotation.eulerAngles.x * Mathf.PI / 180;
        qr[4] = -link5.transform.localRotation.eulerAngles.y * Mathf.PI / 180;
        qr[5] = link6.transform.localRotation.eulerAngles.x * Mathf.PI / 180;
        
        for(int i = 0; i < 6; i++)
        {
            qr[i] = MathOperations.AngleRound(qr[i]);
        }
        return qr;
    }

    public override void SendState(float[] q)
    {
        link1.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -q[0] * 180 / Mathf.PI));
        link2.transform.localRotation = Quaternion.Euler(new Vector3(0, (-q[1] + Mathf.PI/2) * 180 / Mathf.PI, 0));
        link3.transform.localRotation = Quaternion.Euler(new Vector3(0, (-q[2] - Mathf.PI/2) * 180 / Mathf.PI, 0));
        link4.transform.localRotation = Quaternion.Euler(new Vector3(q[3] * 180 / Mathf.PI, 0, 0));
        link5.transform.localRotation = Quaternion.Euler(new Vector3(0, -q[4] * 180 / Mathf.PI, 0));
        link6.transform.localRotation = Quaternion.Euler(new Vector3(q[5] * 180 / Mathf.PI, 0, 0));
    }

    public override float[] InversKin(Matrix<float> Tgoal, float[] q0)
    {
        q = q0;
        return InversKin(Tgoal);
    }

    public override Matrix<float> CreareMatrixT(Vector3 pos, Vector3 ori)
    {

        if (ori.x > Mathf.PI)
        {
            ori.x = ori.x - Mathf.PI * 2;
        }
        if (ori.x < -Mathf.PI)
        {
            ori.x = ori.x + Mathf.PI * 2;
        }
        if (ori.y > Mathf.PI)
        {
            ori.y = ori.y - Mathf.PI * 2;
        }
        if (ori.y < -Mathf.PI)
        {
            ori.y = ori.y + Mathf.PI * 2;
        }
        if (ori.z > Mathf.PI)
        {
            ori.z = ori.z - Mathf.PI * 2;
        }
        if (ori.z < -Mathf.PI)
        {
            ori.z = ori.z + Mathf.PI * 2;
        }


        float p = -ori.x;
        float o = -ori.z;
        float s = -ori.y - Mathf.PI;

        Matrix<float> Rz = Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {Mathf.Cos(s), -Mathf.Sin(s), 0},
                {Mathf.Sin(s), Mathf.Cos(s),  0},
                {0,            0,             1}
            });
        Matrix<float> Ry = Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {Mathf.Cos(o),  0, Mathf.Sin(o)},
                {0,             1, 0},
                {-Mathf.Sin(o), 0, Mathf.Cos(o)}
            });
        Matrix<float> Rx = Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {1, 0,              0},
                {0, Mathf.Cos(p),  -Mathf.Sin(p)},
                {0, Mathf.Sin(p),  Mathf.Cos(p)}
            });

        Matrix<float> R = Rz * Rx * Ry;

        Matrix<float> T = Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {R[0,0], R[0,1], R[0,2], -pos.x},
                {R[1,0], R[1,1], R[1,2], -pos.z},
                {R[2,0], R[2,1], R[2,2], pos.y},
                {0,      0,      0,      1}
            });

        return T;
    }
}
