using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine;

public class IiwaControl : RobotControll {

    public GameObject link1;
    public GameObject link2;
    public GameObject link3;
    public GameObject link4;
    public GameObject link5;
    public GameObject link6;
    public GameObject link7;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        //link1 = GameObject.Find("link_1");
        //link2 = GameObject.Find("link_2");
        //link3 = GameObject.Find("link_3");
        //link4 = GameObject.Find("link_4");
        //link5 = GameObject.Find("link_5");
        //link6 = GameObject.Find("link_6");
        //link7 = GameObject.Find("link_7");
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override Matrix<float> ForwardKin(float[] q)
    {
        Matrix<float> A0 = MathOperations.CalcMatrixDH(Mathf.PI, 0, 0, 0);
        Matrix<float> A1 = MathOperations.CalcMatrixDH(q[0], 360, 0, Mathf.PI / 2);
        Matrix<float> A2 = MathOperations.CalcMatrixDH(q[1], 0, 0, -Mathf.PI / 2);
        Matrix<float> A3 = MathOperations.CalcMatrixDH(q[2], 420, 0, Mathf.PI / 2);
        Matrix<float> A4 = MathOperations.CalcMatrixDH(q[3], 0, 0, -Mathf.PI / 2);
        Matrix<float> A5 = MathOperations.CalcMatrixDH(q[4], 400, 0, Mathf.PI / 2);
        Matrix<float> A6 = MathOperations.CalcMatrixDH(q[5], 0, 0, -Mathf.PI / 2);
        Matrix<float> A7 = MathOperations.CalcMatrixDH(q[6], 130, 0, 0);

        Matrix<float> T7 = A1 * A2 * A3 * A4 * A5 * A6 * A7;

        return T7;
    }

    public override float[] InversKin(Matrix<float> Tgoal, float[] q0)
    {
        q = q0;
        return InversKin(Tgoal);
    }

    public override float[] InversKin(Matrix<float> Tgoal)
    {
        q = new float[] { 0, 0, 0, 0, 0, 0, 0 };
        //Tgoal[1, 3] = -Tgoal[1, 3];
        //Tgoal[0, 3] = -Tgoal[0, 3];

        //Tgoal = Tgoal * MathOperations.CalcMatrixRx(0) * MathOperations.CalcMatrixRy(0) * MathOperations.CalcMatrixRz();
        float diff_r = 0;
        float diif_o = 0;
        int c = 0;
        do
        {
            Matrix<float> end_effector_matrix = ForwardKin(q);

            float[] error = CalcErorr(Tgoal, end_effector_matrix);

            diff_r = error[0] * error[0] + error[1] * error[1] + error[2] * error[2];
            diif_o = error[3] * error[3] + error[4] * error[4] + error[5] * error[5];

            Vector<float> del_q = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 0, 0, 0, 0 });
            float[] del_q2 = new float[] { 0, 0, 0, 0, 0, 0, 0 };

            if (diff_r > 1 || diif_o > 0.1F)
            {
                del_q2 = MoveSNS(q, error);

                for (int i = 0; i < 7; i++)
                {
                    q[i] = q[i] + del_q2[i];
                }
            }
            if (c > 1000)
            {
                return new float[] { 0, 0, 0, 0, 0, 0 };

            }
            else
            {
                c++;
            }

        } while ((diff_r > 1 || diif_o > 0.1F));


        //Debug.Log("IIWA IK:" + q.ToString());
        return q;

    }

    private float[] MoveSNS(float[] q_current, float[] error)
    {

        float k = 0.1f;
        float[] speed = new float[] { 85 * k, 85 * k, 100 * k, 75 * k, 130 * k, 135 * k, 135 * k };
        float[] Qmax = new float[] { 170, 130, 170, 130, 170, 130, 175 };
        float[] Qmin = new float[] { -170, -130, -170, -130, -170, -130, -175 };


        for (int i = 0; i < 7; i++)
        {
            Qmax[i] = Mathf.Min((Qmax[i] * Mathf.PI / 180 - q_current[i]) / Time.deltaTime, speed[i] * Mathf.PI / 180);
            Qmin[i] = Mathf.Max((Qmin[i] * Mathf.PI / 180 - q_current[i]) / Time.deltaTime, -speed[i] * Mathf.PI / 180);
        }

        Matrix<float> W = Matrix<float>.Build.DenseDiagonal(7, 1);
        Matrix<float> W2 = Matrix<float>.Build.DenseDiagonal(7, 1);
        Vector<float> qN = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 0, 0, 0, 0 });
        Vector<float> qN2 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 0, 0, 0, 0 });
        float s = 1;
        float s2 = 0;

        bool lim_exceeded;
        float task_scale;

        Vector<float> qSNS;
        Matrix<float> jac = IIWAjacobian(q_current);

        Vector<float> errorSNS = Vector<float>.Build.DenseOfArray(error);

        do
        {
            lim_exceeded = false;


            Matrix<float> JW = jac * W;
            Matrix<float> jac3 = JW.PseudoInverse();


            qSNS = qN + jac3 * (s * errorSNS - jac * qN);
            //qSNS = qN + jac3.Multiply(s * errorSNS - jac.Multiply(qN));

            float max_q = 0;
            int j = 0;
            for (int i = 0; i < 7; i++)
            {
                if (qSNS[i] > Qmax[i] || qSNS[i] < Qmin[i])
                {
                    lim_exceeded = true;

                    if (max_q < Mathf.Abs(qSNS[i]))
                    {
                        j = i;
                        max_q = Mathf.Abs(qSNS[i]);
                    }

                }
            }

            if (lim_exceeded)
            {
                Vector<float> a = jac3.Multiply(errorSNS);
                Vector<float> b = qN - jac3.Multiply(jac.Multiply(qN));

                float[] Smax = new float[7];
                float[] Smin = new float[7];
                int c = 0;
                for (int i = 0; i < 7; i++)
                {
                    if (W[i, i] != 0)
                    {
                        Smin[c] = (Qmin[i] - b[i]) / a[i];
                        Smax[c] = (Qmax[i] - b[i]) / a[i];

                        if (Smax[c] < Smin[c])
                        {
                            float sw = Smax[c];
                            Smax[c] = Smin[c];
                            Smin[c] = sw;
                        }
                        c = c + 1;
                    }
                }

                //find
                //smax_=min(Smax);
                //smin_ =max(Smin);

                float s_min = Smin[0];
                float s_max = Smax[0];
                for (int i = 1; i < 7; i++)
                {
                    if (s_max > Smax[i]) { s_max = Smax[i]; }
                    if (s_min < Smin[i]) { s_min = Smin[i]; }
                }


                if (s_min > s_max || s_max < 0 || s_min > 1)
                {
                    task_scale = 0;
                }
                else
                {
                    task_scale = Mathf.Min(s_max, 1);
                }

                if (task_scale >= s2)
                {
                    s2 = task_scale;
                    W2 = W.Clone();
                    qN2 = qN.Clone();
                }

                W[j, j] = 0;


                if (qSNS[j] > Qmax[j]) { qN[j] = Qmax[j]; }
                if (qSNS[j] < Qmin[j]) { qN[j] = Qmin[j]; }

                JW = jac * W;
                int r = JW.Rank();
                if (r < 6)
                {
                    s = s2;
                    W = W2.Clone();
                    qN = qN2.Clone();
                    lim_exceeded = false;

                    JW = jac * W;
                    Matrix<float> jac2 = JW.PseudoInverse();

                    qSNS = qN + jac2 * (s * errorSNS - jac * qN);
                }

            }

        } while (lim_exceeded);


        return qSNS.AsArray();

    }

    private Matrix<float> IIWAjacobian(float[] q)
    {
        Matrix<float> A0 = MathOperations.CalcMatrixDH(Mathf.PI, 0, 0, 0);
        Matrix<float> A1 = MathOperations.CalcMatrixDH(q[0], 360, 0, Mathf.PI / 2);
        Matrix<float> A2 = MathOperations.CalcMatrixDH(q[1], 0, 0, -Mathf.PI / 2);
        Matrix<float> A3 = MathOperations.CalcMatrixDH(q[2], 420, 0, Mathf.PI / 2);
        Matrix<float> A4 = MathOperations.CalcMatrixDH(q[3], 0, 0, -Mathf.PI / 2);
        Matrix<float> A5 = MathOperations.CalcMatrixDH(q[4], 400, 0, Mathf.PI / 2);
        Matrix<float> A6 = MathOperations.CalcMatrixDH(q[5], 0, 0, -Mathf.PI / 2);
        Matrix<float> A7 = MathOperations.CalcMatrixDH(q[6], 130, 0, 0);

        Matrix<float> T1 = A1;
        Matrix<float> T2 = T1 * A2;
        Matrix<float> T3 = T2 * A3;
        Matrix<float> T4 = T3 * A4;
        Matrix<float> T5 = T4 * A5;
        Matrix<float> T6 = T5 * A6;
        Matrix<float> T7 = T6 * A7;

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
        Vector3 p7 = new Vector3(T7[0, 3], T7[1, 3], T7[2, 3]);

        return Matrix<float>.Build.DenseOfArray(new float[,]{
            {Vector3.Cross (z0, p7 - p0).x, Vector3.Cross (z1, p7 - p1).x , Vector3.Cross (z2, p7 - p2).x, Vector3.Cross (z3, p7 - p3).x, Vector3.Cross (z4, p7 - p4).x, Vector3.Cross (z5, p7 - p5).x, Vector3.Cross (z6, p7 - p6).x},
            {Vector3.Cross (z0, p7 - p0).y, Vector3.Cross (z1, p7 - p1).y , Vector3.Cross (z2, p7 - p2).y, Vector3.Cross (z3, p7 - p3).y, Vector3.Cross (z4, p7 - p4).y, Vector3.Cross (z5, p7 - p5).y, Vector3.Cross (z6, p7 - p6).y},
            {Vector3.Cross (z0, p7 - p0).z, Vector3.Cross (z1, p7 - p1).z , Vector3.Cross (z2, p7 - p2).z, Vector3.Cross (z3, p7 - p3).z, Vector3.Cross (z4, p7 - p4).z, Vector3.Cross (z5, p7 - p5).z, Vector3.Cross (z6, p7 - p6).z},
            {z0.x, z1.x, z2.x, z3.x, z4.x, z5.x, z6.x},
            {z0.y, z1.y, z2.y, z3.y, z4.y, z5.y, z6.y},
            {z0.z, z1.z, z2.z, z3.z, z4.z, z5.z, z6.z}
        });
    }
    
    public override float[] ReadState()
    {
        float[] qr = new float[7];

        qr[0] = link1.transform.localRotation.eulerAngles.y * Mathf.PI / 180;
        qr[1] = link2.transform.localRotation.eulerAngles.z * Mathf.PI / 180;
        qr[2] = link3.transform.localRotation.eulerAngles.y * Mathf.PI / 180;
        qr[3] = link4.transform.localRotation.eulerAngles.z * Mathf.PI / 180;
        qr[4] = link5.transform.localRotation.eulerAngles.y * Mathf.PI / 180;
        qr[5] = link6.transform.localRotation.eulerAngles.z * Mathf.PI / 180;
        qr[6] = link7.transform.localRotation.eulerAngles.y * Mathf.PI / 180;

        for (int i = 0; i < 7; i++)
        {

            if (qr[i] > Mathf.PI)
            {
                qr[i] = qr[i] - Mathf.PI * 2;
            }
            if (qr[i] < -Mathf.PI)
            {
                qr[i] = qr[i] + Mathf.PI * 2;
            }
        }


        return qr;
    }

    public override void SendState(float[] q)
    {
        link1.transform.localRotation = Quaternion.Euler(new Vector3(0, q[0] * 180 / Mathf.PI, 0));
        link2.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, q[1] * 180 / Mathf.PI));
        link3.transform.localRotation = Quaternion.Euler(new Vector3(0, q[2] * 180 / Mathf.PI, 0));
        link4.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, q[3] * 180 / Mathf.PI));
        link5.transform.localRotation = Quaternion.Euler(new Vector3(0, q[4] * 180 / Mathf.PI, 0));
        link6.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, q[5] * 180 / Mathf.PI));
        link7.transform.localRotation = Quaternion.Euler(new Vector3(0, q[6] * 180 / Mathf.PI, 0));
    }

    private float[] CalcErorr(Matrix<float> goal, Matrix<float> end_effector)
    {

        float[] err = new float[6];

        err[0] = goal[0, 3] - end_effector[0, 3];
        err[1] = goal[1, 3] - end_effector[1, 3];
        err[2] = goal[2, 3] - end_effector[2, 3];

        float[] t1 = new float[]{
            Mathf.Atan2(goal[2,1],Mathf.Sqrt(1-goal[2,1]*goal[2,1])),
            Mathf.Atan2(-goal[0,1],goal[1,1]),
            Mathf.Atan2(-goal[2,0],goal[2,2])
        };
        float[] t2 = new float[]{
            Mathf.Atan2(end_effector[2,1],Mathf.Sqrt(1-end_effector[2,1]*end_effector[2,1])),
            Mathf.Atan2(-end_effector[0,1],end_effector[1,1]),
            Mathf.Atan2(-end_effector[2,0],end_effector[2,2])
        };

        Matrix<float> R = Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {0, Mathf.Cos(t1[1]), -Mathf.Sin(t1[1])*Mathf.Cos(t1[0])},
                {0, Mathf.Sin(t1[1]), Mathf.Cos(t1[1])*Mathf.Cos(t1[0])},
                {1, 0,                Mathf.Sin(t1[0])}
            });


        float d1 = t1[0] - t2[0];
        float d2 = t1[1] - t2[1];
        float d3 = t1[2] - t2[2];


        err[3] = (R[0, 0] * d2 + R[0, 1] * d1 + R[0, 2] * d3);
        err[4] = (R[1, 0] * d2 + R[1, 1] * d1 + R[1, 2] * d3);
        err[5] = (R[2, 0] * d2 + R[2, 1] * d1 + R[2, 2] * d3);

        return err;
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
        float o = ori.z;
        float s = ori.y - Mathf.PI;

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

        Matrix<float> R = Rz.Multiply(Ry.Multiply(Rx));

        Matrix<float> T = Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {R[0,0], R[0,1], R[0,2], pos.x},
                {R[1,0], R[1,1], R[1,2], -pos.z},
                {R[2,0], R[2,1], R[2,2], pos.y},
                {0,      0,      0,      1}
            });

        return T;
    }
}
