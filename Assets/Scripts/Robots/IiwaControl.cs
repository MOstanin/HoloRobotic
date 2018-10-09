using System.Collections;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using UnityEngine;

public class IiwaControl : RobotControll
{

    public GameObject link1;
    public GameObject link2;
    public GameObject link3;
    public GameObject link4;
    public GameObject link5;
    public GameObject link6;
    public GameObject link7;

    private void Awake()
    {
        q = new float[] { 0, 0, 0, 90 * Mathf.PI / 180, 0, 90 * Mathf.PI / 180, 0 };
        SendState(q);
    }

    public override float[] InversKin(Matrix<float> Tgoal)
    {
        float[] q = new float[] { 0, 0, 0, 0, 0, 0, 0 };
        return InversKin(Tgoal, q);
    }

    public override float[] InversKin(Matrix<float> Tgoal, float[] q0)
    {
        //float[] q2 = ReadState();
        //q = new float[] { 0, 0, 0, 90 * Mathf.PI / 180, 0, 90 * Mathf.PI / 180, 0 };
        //q = new float[] { 0, 0, 0, 0, 0, 0, 0 };
        //Tgoal[1, 3] = -Tgoal[1, 3];
        //Tgoal[0, 3] = -Tgoal[0, 3];
        //Tgoal = ForwardKin(q2);

        Tgoal = Tgoal * MathOperations.MatrixRy4(Mathf.PI / 2) * MathOperations.MatrixRz4(Mathf.PI / 2);
        //Debug.Log(Tgoal.ToString());
        Matrix<float> end_effector_matrix = ForwardKin(q);
        //Debug.Log(end_effector_matrix.ToString());
        float diff_r = 0;
        float diif_o = 0;
        int c = 0;
        do
        {
           end_effector_matrix = ForwardKin(q);

            float[] error = CalcErorr(Tgoal, end_effector_matrix);

            diff_r = error[0] * error[0] + error[1] * error[1] + error[2] * error[2];
            diif_o = error[3] * error[3] + error[4] * error[4] + error[5] * error[5];
            

            Vector<float> del_q = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 0, 0, 0, 0 });
            float[] del_q2 = new float[] { 0, 0, 0, 0, 0, 0, 0 };

            if (diff_r > 1 || diif_o > 0.1F)
            {
                del_q2 = MoveSNS(q, error);
                //del_q2 = MovePINV(q, error);
                for (int i = 0; i < 7; i++)
                {
                    //q[i] = (i!=3)? (q[i] + del_q2[i] * 0.05f) : (q[i] - del_q2[i] * 0.05f);
                    q[i] = q[i] + del_q2[i] * 0.02f;
                }
            }
            
            if (c > 400)
            {
                return new float[] { 0, 0, 0, 90 * Mathf.PI / 180, 0, 90 * Mathf.PI / 180, 0 };

            }
            else
            {
                c++;
            }

        } while ((diff_r > 1 || diif_o > 0.1F));

        for (int i = 0; i < 6; i++)
        {
            q[i] = MathOperations.AngleRound(q[i]);
        }//Debug.Log("IIWA IK:" + q.ToString());

        return q;

    }

    private float[] MovePINV(float[] q_current, float[] error)
    {
        Matrix<float> jac = IIWAjacobian(q_current);

        Vector<float> errorV = Vector<float>.Build.DenseOfArray(error);


        Svd<float> svd = jac.Svd();
        //float[] singularVal = new float[6];

        Matrix<float> S = Matrix<float>.Build.DenseIdentity(7, 6);
        for (int i = 0; i < 6; i++)
        {
            if (svd.S[i] != 0)
            {
                S[i, i] = svd.S[i] / (svd.S[i] * svd.S[i] + 1);
            }
            else
            {
                S[i, i] = 1000000000;
            }
        }

        Matrix<float> jac3 = svd.VT.Transpose() * S * svd.U.Transpose();

        Vector<float> qPINV = jac3 * errorV;

        float[] qOut = new float[7];
        for (int i = 0; i < 7; i++)
        {
            qOut[i] = qPINV[i];
        }
        return qOut;
    }

    private float[] MoveSNS(float[] q_current, float[] error)
    {

        float k = 1f;
        float[] speed = new float[] { 85 * k, 85 * k, 100 * k, 75 * k, 130 * k, 135 * k, 135 * k };
        float[] Qmax = new float[] { 170, 120, 170, 120, 170, 120, 170 };
        float[] Qmin = new float[] { -170, -120, -170, -120, -170, -120, -170 };


        for (int i = 0; i < 7; i++)
        {
            Qmax[i] = Mathf.Min((Qmax[i] * Mathf.PI / 180 - q_current[i]) / 0.02f, speed[i] * Mathf.PI / 180);
            Qmin[i] = Mathf.Max((Qmin[i] * Mathf.PI / 180 - q_current[i]) / 0.02f, -speed[i] * Mathf.PI / 180);
        }

        Matrix<float> W = Matrix<float>.Build.DenseDiagonal(7, 1);
        Matrix<float> W2 = Matrix<float>.Build.DenseDiagonal(7, 1);
        Vector<float> qN = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 0, 0, 0, 0 });
        Vector<float> qN2 = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 0, 0, 0, 0 });
        float s = 1;
        float s2 = 0;

        bool lim_exceeded;
        float task_scale;

        Vector<float> qSNS = Vector<float>.Build.DenseOfArray(new float[] { 0, 0, 0, 0, 0, 0, 0 });
        Matrix<float> jac = IIWAjacobian(q_current);

        Vector<float> errorSNS = Vector<float>.Build.DenseOfArray(error);

        Matrix<float> JW = jac * W;
        do
        {
            lim_exceeded = false;
            
            
            //Matrix<float> jac3 = JW.PseudoInverse();
            
            Svd<float> svd  = JW.Svd();
            //float[] singularVal = new float[6];

            Matrix<float> S = Matrix<float>.Build.DenseIdentity(7, 6);
            for (int i = 0; i< 6; i++)
            {
                if (svd.S[i] != 0)
                {
                    //S[i, i] = 1 / (svd.S[i]);
                    S[i, i] = svd.S[i] / (svd.S[i] * svd.S[i] + 1);
                }
                else
                {
                    S[i, i] = 1000000000;
                }
            }

            Matrix<float> jac3 = svd.VT.Transpose() * S * svd.U.Transpose();
            //Matrix<float> jac3 = (JW.Transpose() * JW).Inverse() * JW.Transpose();


            qSNS = qN + jac3 * (errorSNS - jac * qN);
            //qSNS = qN + jac3.Multiply(s * errorSNS - jac.Multiply(qN));

            float max_q = 0;
            int j = 0;
            for (int i = 0; i < 7; i++)
            {
                if (qSNS[i] > Qmax[i] || qSNS[i] < Qmin[i])
                {
                    lim_exceeded = true;

                    if (max_q < Mathf.Abs(qSNS[i]) && W[i, i] != 0)
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

                if (task_scale > s2)
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
                    //Matrix<float> jac2 = JW.PseudoInverse();
                    Svd<float> svd2 = JW.Svd();
                    //float[] singularVal = new float[6];

                    Matrix<float> S2 = Matrix<float>.Build.DenseIdentity(7, 6);
                    for (int i = 0; i < 6; i++)
                    {
                        if (svd.S[i] != 0)
                        {
                            //S2[i, i] = 1 / (svd2.S[i]);
                            S[i, i] = svd.S[i] / (svd.S[i] * svd.S[i] + 1);
                        }
                        else
                        {
                            S2[i, i] = 1E10f;
                        }
                    }

                    Matrix<float> jac2 = svd2.VT.Transpose() * S2 * svd2.U.Transpose();

                    qSNS = qN + jac2 * (s * errorSNS - jac * qN);
                    Debug.Log("qSNS error");
                }


            }

        } while (lim_exceeded);

        float[] qOut = new float[7];
        for (int i = 0; i < 7; i++)
        {
            qOut[i] = qSNS[i];
        }
        return qOut;

    }

    public override float[] ReadState()
    {
        float[] qr = new float[7];

        qr[0] = - link1.transform.localRotation.eulerAngles.y * Mathf.PI / 180;
        qr[1] = link2.transform.localRotation.eulerAngles.z * Mathf.PI / 180;
        qr[2] = - link3.transform.localRotation.eulerAngles.y * Mathf.PI / 180;
        qr[3] =  link4.transform.localRotation.eulerAngles.z * Mathf.PI / 180;
        qr[4] = - link5.transform.localRotation.eulerAngles.y * Mathf.PI / 180;
        qr[5] = link6.transform.localRotation.eulerAngles.z * Mathf.PI / 180;
        qr[6] = - link7.transform.localRotation.eulerAngles.y * Mathf.PI / 180;

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
        // joint 4 was changed for ROS 
        link1.transform.localRotation = Quaternion.Euler(new Vector3(0, - q[0] * 180 / Mathf.PI, 0));
        link2.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, q[1] * 180 / Mathf.PI));
        link3.transform.localRotation = Quaternion.Euler(new Vector3(0, - q[2] * 180 / Mathf.PI, 0));
        link4.transform.localRotation = Quaternion.Euler(new Vector3(0, 0,  -q[3] * 180 / Mathf.PI));
        link5.transform.localRotation = Quaternion.Euler(new Vector3(0, - q[4] * 180 / Mathf.PI, 0));
        link6.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, q[5] * 180 / Mathf.PI));
        link7.transform.localRotation = Quaternion.Euler(new Vector3(0, - q[6] * 180 / Mathf.PI, 0));
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

        err[3] = MathOperations.AngleRound(err[3]);
        err[4] = MathOperations.AngleRound(err[4]);
        err[5] = MathOperations.AngleRound(err[5]);


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

    public override  Matrix<float> ForwardKin(float[] q)
    {
        Matrix<float> T = MathOperations.MatrixRz4(q[0]) * MathOperations.MatrixTz4(360) * MathOperations.MatrixRy4(q[1]) *
            MathOperations.MatrixRz4(q[2]) * MathOperations.MatrixTz4(420) * MathOperations.MatrixRy4(q[3]) *
            MathOperations.MatrixRz4(q[4]) * MathOperations.MatrixTz4(400) * MathOperations.MatrixRy4(q[5]) *
            MathOperations.MatrixRz4(q[6]) * MathOperations.MatrixTz4(130);


        return T;
    }

    private Matrix<float> IIWAjacobian(float[] q)
    {
        return IIWAjacobian2(q);
        Matrix<float> T = ForwardKin(q);
        //q[3] = -q[3];

        T[0, 3] = 0;
        T[1, 3] = 0;
        T[2, 3] = 0;

        
        Matrix<float> Tinv = T.Clone().Inverse();



        Matrix<float> T1 = MathOperations.Rdz(q[0]) * MathOperations.MatrixTz4(360) * MathOperations.MatrixRy4(q[1]) *
            MathOperations.MatrixRz4(q[2]) * MathOperations.MatrixTz4(420) * MathOperations.MatrixRy4(q[3]) *
            MathOperations.MatrixRz4(q[4]) * MathOperations.MatrixTz4(400) * MathOperations.MatrixRy4(q[5]) *
            MathOperations.MatrixRz4(q[6]) * MathOperations.MatrixTz4(130) * Tinv;

        Matrix<float> T2 = MathOperations.MatrixRz4(q[0]) * MathOperations.MatrixTz4(360) * MathOperations.Rdy(q[1]) *
            MathOperations.MatrixRz4(q[2]) * MathOperations.MatrixTz4(420) * MathOperations.MatrixRy4(q[3]) *
            MathOperations.MatrixRz4(q[4]) * MathOperations.MatrixTz4(400) * MathOperations.MatrixRy4(q[5]) *
            MathOperations.MatrixRz4(q[6]) * MathOperations.MatrixTz4(130) * Tinv;

        Matrix<float> T3 = MathOperations.MatrixRz4(q[0]) * MathOperations.MatrixTz4(360) * MathOperations.MatrixRy4(q[1]) *
            MathOperations.Rdz(q[2]) * MathOperations.MatrixTz4(420) * MathOperations.MatrixRy4(q[3]) *
            MathOperations.MatrixRz4(q[4]) * MathOperations.MatrixTz4(400) * MathOperations.MatrixRy4(q[5]) *
            MathOperations.MatrixRz4(q[6]) * MathOperations.MatrixTz4(130) * Tinv;

        Matrix<float> T4 = MathOperations.MatrixRz4(q[0]) * MathOperations.MatrixTz4(360) * MathOperations.MatrixRy4(q[1]) *
            MathOperations.MatrixRz4(q[2]) * MathOperations.MatrixTz4(420) * MathOperations.Rdy(q[3]) *
            MathOperations.MatrixRz4(q[4]) * MathOperations.MatrixTz4(400) * MathOperations.MatrixRy4(q[5]) *
            MathOperations.MatrixRz4(q[6]) * MathOperations.MatrixTz4(130) * Tinv;

        Matrix<float> T5 = MathOperations.MatrixRz4(q[0]) * MathOperations.MatrixTz4(360) * MathOperations.MatrixRy4(q[1]) *
            MathOperations.MatrixRz4(q[2]) * MathOperations.MatrixTz4(420) * MathOperations.MatrixRy4(q[3]) *
            MathOperations.Rdz(q[4]) * MathOperations.MatrixTz4(400) * MathOperations.MatrixRy4(q[5]) *
            MathOperations.MatrixRz4(q[6]) * MathOperations.MatrixTz4(130) * Tinv;

        Matrix<float> T6 = MathOperations.MatrixRz4(q[0]) * MathOperations.MatrixTz4(360) * MathOperations.MatrixRy4(q[1]) *
            MathOperations.MatrixRz4(q[2]) * MathOperations.MatrixTz4(420) * MathOperations.MatrixRy4(q[3]) *
            MathOperations.MatrixRz4(q[4]) * MathOperations.MatrixTz4(400) * MathOperations.Rdy(q[5]) *
            MathOperations.MatrixRz4(q[6]) * MathOperations.MatrixTz4(130) * Tinv;

        Matrix<float> T7 = MathOperations.MatrixRz4(q[0]) * MathOperations.MatrixTz4(360) * MathOperations.MatrixRy4(q[1]) *
            MathOperations.MatrixRz4(q[2]) * MathOperations.MatrixTz4(420) * MathOperations.MatrixRy4(q[3]) *
            MathOperations.MatrixRz4(q[4]) * MathOperations.MatrixTz4(400) * MathOperations.MatrixRy4(q[5]) *
            MathOperations.Rdz(q[6]) * MathOperations.MatrixTz4(130) * Tinv;

        
        return Matrix<float>.Build.DenseOfArray( new float[,]
        {
            { T1[0,3], T2[0,3], T3[0,3], T4[0,3], T5[0,3], T6[0,3], T7[0,3]},
            { T1[1,3], T2[1,3], T3[1,3], T4[1,3], T5[1,3], T6[1,3], T7[1,3]},
            { T1[2,3], T2[2,3], T3[2,3], T4[2,3], T5[2,3], T6[2,3], T7[2,3]},
            { T1[2,1], T2[2,1], T3[2,1], T4[2,1], T5[2,1], T6[2,1], T7[2,1]},
            { T1[0,2], T2[0,2], T3[0,2], T4[0,2], T5[0,2], T6[0,2], T7[0,2]},
            { T1[1,0], T2[1,0], T3[1,0], T4[1,0], T5[1,0], T6[1,0], T7[1,0]},
        });
    }

    private Matrix<float> IIWAjacobian2(float[] q)
    {
        
        float LINK23 = 420;
        float LINK45 = 400;
        float LINK67 = 126;

        // angles
        float c0 = Mathf.Cos(q[0]), s0 = Mathf.Sin(q[0]);
        float c1 = Mathf.Cos(q[1]), s1 = Mathf.Sin(q[1]);
        float c2 = Mathf.Cos(q[2]), s2 = Mathf.Sin(q[2]);       
        float c3 = Mathf.Cos(-q[3]), s3 = Mathf.Sin(-q[3]);
        float c4 = Mathf.Cos(q[4]), s4 = Mathf.Sin(q[4]);       
        float c5 = Mathf.Cos(q[5]), s5 = Mathf.Sin(q[5]);
        float c6 = Mathf.Cos(q[6]), s6 = Mathf.Sin(q[6]);
        // initialization
        Matrix<float> m = Matrix<float>.Build.DenseOfArray(new float[6,7]);
        // find common terms
        float _grp0 = (c0 * s1 * s3 + (c0 * c1 * c2 - s0 * s2) * c3), _grp1 = (c0 * s1 * c3 - (c0 * c1 * c2 - s0 * s2) * s3);
        float _grp2 = (c0 * c2 - s0 * c1 * s2), _grp3 = (s0 * s1 * s3 + (c0 * s2 + s0 * c1 * c2) * c3);
        float _grp4 = (s0 * s1 * c3 - (c0 * s2 + s0 * c1 * c2) * s3), _grp5 = (c1 * s3 - s1 * c2 * c3);
        float _grp6 = (s1 * s2 * s4 + _grp5 * c4) * s5, _grp7 = (s1 * c2 * s3 + c1 * c3);
        float _grp8 = (-c0 * c1 * s2 - s0 * c2);
        m[0, 0] = (-LINK67 * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5)) - LINK45 * _grp4 - LINK23 * s0 * s1;
        m[0, 1] = c0 * (LINK67 * (_grp6 + _grp7 * c5) + LINK45 * _grp7 + LINK23 * c1);
        m[0, 2] = s0 * s1 * (LINK67 * (_grp6 + _grp7 * c5) + LINK45 * _grp7 + LINK23 * c1) - c1 * (LINK67 * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK45 * _grp4 + LINK23 * s0 * s1);
        m[0, 3] = _grp2 * (LINK67 * (_grp6 + _grp7 * c5) + LINK45 * _grp7) - s1 * s2 * (LINK67 * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK45 * _grp4);
        m[0, 4] = ((-s1 * c2 * s3) - c1 * c3) * (LINK67 * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK45 * _grp4) + _grp4 * (LINK67 * (_grp6 + _grp7 * c5) + LINK45 * _grp7);
        m[0, 5] = LINK67 * (_grp5 * s4 - s1 * s2 * c4) * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK67 * (_grp2 * c4 - _grp3 * s4) * (_grp6 + _grp7 * c5);
        m[0, 6] = LINK67 * (_grp6 + _grp7 * c5) * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK67 * ((-_grp6) - _grp7 * c5) * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5);
        m[1, 0] = LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK45 * _grp1 + LINK23 * c0 * s1;
        m[1, 1] = s0 * (LINK67 * (_grp6 + _grp7 * c5) + LINK45 * _grp7 + LINK23 * c1);
        m[1, 2] = c1 * (LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK45 * _grp1 + LINK23 * c0 * s1) - c0 * s1 * (LINK67 * (_grp6 + _grp7 * c5) + LINK45 * _grp7 + LINK23 * c1);
        m[1, 3] = s1 * s2 * (LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK45 * _grp1) + (c0 * c1 * s2 + s0 * c2) * (LINK67 * (_grp6 + _grp7 * c5) + LINK45 * _grp7);
        m[1, 4] = _grp7 * (LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK45 * _grp1) + ((c0 * c1 * c2 - s0 * s2) * s3 - c0 * s1 * c3) * (LINK67 * (_grp6 + _grp7 * c5) + LINK45 * _grp7);
        m[1, 5] = LINK67 * (s1 * s2 * c4 - _grp5 * s4) * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK67 * (_grp0 * s4 - _grp8 * c4) * (_grp6 + _grp7 * c5);
        m[1, 6] = LINK67 * (_grp6 + _grp7 * c5) * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK67 * (_grp6 + _grp7 * c5) * ((-(_grp8 * s4 + _grp0 * c4) * s5) - _grp1 * c5);
        m[2, 0] = 0.0f;
        m[2, 1] = (-s0 * (LINK67 * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK45 * _grp4 + LINK23 * s0 * s1)) - c0 * (LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK45 * _grp1 + LINK23 * c0 * s1);
        m[2, 2] = c0 * s1 * (LINK67 * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK45 * _grp4 + LINK23 * s0 * s1) - s0 * s1 * (LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK45 * _grp1 + LINK23 * c0 * s1);
        m[2, 3] = _grp8 * (LINK67 * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK45 * _grp4) + (s0 * c1 * s2 - c0 * c2) * (LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK45 * _grp1);
        m[2, 4] = _grp1 * (LINK67 * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK45 * _grp4) + ((c0 * s2 + s0 * c1 * c2) * s3 - s0 * s1 * c3) * (LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) + LINK45 * _grp1);
        m[2, 5] = LINK67 * (_grp8 * c4 - _grp0 * s4) * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK67 * (_grp3 * s4 - _grp2 * c4) * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5);
        m[2, 6] = LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) * ((_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5) + LINK67 * ((_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5) * ((-(_grp2 * s4 + _grp3 * c4) * s5) - _grp4 * c5);
        m[3, 0] = 0.0f;
        m[3, 1] = -s0;
        m[3, 2] = c0 * s1;
        m[3, 3] = (-c0 * c1 * s2) - s0 * c2;
        m[3, 4] = c0 * s1 * c3 - (c0 * c1 * c2 - s0 * s2) * s3;
        m[3, 5] = _grp8 * c4 - _grp0 * s4;
        m[3, 6] = (_grp8 * s4 + _grp0 * c4) * s5 + _grp1 * c5;
        m[4, 0] = 0.0f;
        m[4, 1] = c0;
        m[4, 2] = s0 * s1;
        m[4, 3] = c0 * c2 - s0 * c1 * s2;
        m[4, 4] = s0 * s1 * c3 - (c0 * s2 + s0 * c1 * c2) * s3;
        m[4, 5] = _grp2 * c4 - _grp3 * s4;
        m[4, 6] = (_grp2 * s4 + _grp3 * c4) * s5 + _grp4 * c5;
        m[5, 0] = 1.0f;
        m[5, 1] = 0.0f;
        m[5, 2] = c1;
        m[5, 3] = s1 * s2;
        m[5, 4] = s1 * c2 * s3 + c1 * c3;
        m[5, 5] = s1 * s2 * c4 - _grp5 * s4;
        m[5, 6] = _grp6 + _grp7 * c5;
    
        return m;
    }



}
