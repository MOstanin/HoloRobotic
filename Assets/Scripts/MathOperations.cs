using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class MathOperations {

    public static float AngleRound(float q)
    {

        return Mathf.Atan2(Mathf.Sin(q), Mathf.Cos(q));
    }
    public static Matrix<float> CalcMatrixRz(float s)
    {
        return Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {Mathf.Cos(s), -Mathf.Sin(s), 0},
                {Mathf.Sin(s), Mathf.Cos(s),  0},
                {0,            0,             1}
            });
    }

    public static Matrix<float> CalcMatrixRy(float o)
    {
        return Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {Mathf.Cos(o),  0, Mathf.Sin(o)},
                {0,             1, 0},
                {-Mathf.Sin(o), 0, Mathf.Cos(o)}
            });
    }

    public static Matrix<float> CalcMatrixRx(float p)
    {
        return Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {1, 0,              0},
                {0, Mathf.Cos(p),  -Mathf.Sin(p)},
                {0, Mathf.Sin(p),  Mathf.Cos(p)}
            });
    }

    public static Matrix<float> CalcMatrixT(float x, float y, float z)
    {
        return Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {1, 0, 0, x},
                {0, 1, 0, y},
                {0, 0, 1, z},
                {0, 0, 0, 1}
            });
    }

    public static Matrix<float> CalcMatrixT(float x, float y, float z, float s, float o, float p)
    {
        Matrix<float> R = CalcMatrixRz(s) * CalcMatrixRy(o) * CalcMatrixRx(p);

        return Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {R[0,0], R[0,1], R[0,2], x},
                {R[1,0], R[1,1], R[1,2], y},
                {R[2,0], R[2,1], R[2,2], z},
                {0,      0,      0,      1}
            });
    }

    public static Matrix<float> MatrixRz4(float s)
    {
        return CalcMatrixT(0, 0, 0, s, 0, 0);
    }

    public static Matrix<float> MatrixRy4(float o)
    {
        return CalcMatrixT(0, 0, 0, 0, o, 0);
    }

    public static Matrix<float> MatrixRx4(float p)
    {
        return CalcMatrixT(0, 0, 0, 0, 0, p);
    }

    public static Matrix<float> MatrixTz4(float z)
    {
        return CalcMatrixT(0, 0, z);
    }

    public static Matrix<float> MatrixTy4(float y)
    {
        return CalcMatrixT(0, y, 0);
    }

    public static Matrix<float> MatrixTx4(float x)
    {
        return CalcMatrixT(x, 0, 0);
    }

    public static Matrix<float> CalcMatrixDH(float q, float d, float a, float alph)
    {
        return Matrix<float>.Build.DenseOfArray(new float[,]
            {
                { MyRround(Mathf.Cos(q)), MyRround(-Mathf.Cos(alph)*Mathf.Sin(q)),  MyRround(Mathf.Sin(alph)*Mathf.Sin(q)), MyRround(a*Mathf.Cos(q)) },
                { MyRround(Mathf.Sin(q)),  MyRround(Mathf.Cos(alph)*Mathf.Cos(q)), MyRround(-Mathf.Sin(alph)*Mathf.Cos(q)), MyRround(a*Mathf.Sin(q))},
                { 0,                                    MyRround(Mathf.Sin(alph)),               MyRround(Mathf.Cos(alph)),              MyRround(d)},
                { 0,                                                            0,                             0,                            1}
            });
    }

    public static float MyRround(float n)
    {
        return (Mathf.Round(n * 1000)) / 1000;

    }


    public static float[] CalcErorr(Matrix<float> goal, Matrix<float> end_effector)
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


        err[3] = (R[0, 0] * d2 + R[0, 1] * d1 + R[0, 2] * d3) * 2;
        err[4] = (R[1, 0] * d2 + R[1, 1] * d1 + R[1, 2] * d3) * 2;
        err[5] = (R[2, 0] * d2 + R[2, 1] * d1 + R[2, 2] * d3) * 2;

        return err;
    }

    public static Matrix<float> MatrixUnityToRobot()
    {
        return MatrixRobotToUnity().Transpose();
    }

    public static Matrix<float> MatrixRobotToUnity()
    {
        return Matrix<float>.Build.DenseOfArray(new float[,]
            {
                { -1, 0,  0,0 },
                { 0,  0,  1,0 }, 
                { 0, -1,  0,0 },
                { 0,  0,  0,1 }
            });
    }

    public static Matrix<float> LeftToRight(Matrix<float> Tleft)
    {
        Matrix<float> Tright = Tleft.Clone();

        Tright[2, 3] = Tleft[1, 3];
        Tright[1, 3] = Tleft[2, 3];

        //Tright[row, column]

        Tright[1, 0] = Tleft[2, 0];
        Tright[2, 0] = Tleft[1, 0];
        Tright[0, 1] = Tleft[0, 2];
        Tright[1, 1] = Tleft[2, 2];
        Tright[2, 1] = Tleft[1, 2];
        Tright[0, 2] = Tleft[0, 1];
        Tright[1, 2] = Tleft[2, 1];
        Tright[2, 2] = Tleft[1, 1];
               
        return Tright;
    }

    public static Matrix<float> Tdx()
    {
       return Matrix<float>.Build.DenseOfArray(new float[,] {
        {0,0,0,1},
        {0,0,0,0},
        {0,0,0,0},
        {0,0,0,0} });
    }
    public static Matrix<float> Tdy()
    {
        return Matrix<float>.Build.DenseOfArray(new float[,] {
        {0,0,0,0},
        {0,0,0,1},
        {0,0,0,0},
        {0,0,0,0} });
    }
    public static Matrix<float> Tdz()
    {
        return Matrix<float>.Build.DenseOfArray(new float[,] {
        {0,0,0,0},
        {0,0,0,0},
        {0,0,0,1},
        {0,0,0,0} });
    }
    public static Matrix<float> Rdx(float q)
    {
        return Matrix<float>.Build.DenseOfArray(new float[,]
    {
                {0, 0,              0,            0},
                {0, -Mathf.Sin(q), -Mathf.Cos(q), 0},
                {0, Mathf.Cos(q),  -Mathf.Sin(q), 0},
                {0,            0,             0,  0}
    });
    }
    public static Matrix<float> Rdy(float q)
    {
        return Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {-Mathf.Sin(q),  0, Mathf.Cos(q), 0},
                {0,             0,            0,  0},
                {-Mathf.Cos(q), 0, -Mathf.Sin(q), 0},
                {0,             0,            0, 0}
            });
    }
    public static Matrix<float> Rdz(float q)
    {
        return Matrix<float>.Build.DenseOfArray(new float[,]
            {
                {-Mathf.Sin(q), -Mathf.Cos(q), 0, 0},
                {Mathf.Cos(q), -Mathf.Sin(q),  0, 0},
                {0,            0,             0, 0},
                {0,            0,             0, 0}
            });
    }
}
