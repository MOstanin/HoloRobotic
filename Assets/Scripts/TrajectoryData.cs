using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using MathNet.Numerics.LinearAlgebra;

public class TrajectoryData : Singleton<TrajectoryData>
{
    public Trajectory trajectory;
    public GameObject ball;
    public GameObject viaPoint;
    private List<GameObject> PathP2P;
    private List<Trajectory> TrajectoriesMass;

    public void AddViaPoint(GameObject point)
    {
        if (PathP2P != null)
        {
            PathP2P.Add(point);
        }
        else
        {
            PathP2P = new List<GameObject>();
            PathP2P.Add(point);
        }
    }

    public void AddP2PTraject(GameObject ball)
    {
        trajectory.AddPoint2PointTrajectory(ball, PathP2P);
        PathP2P.Clear();
    }

    public void CreateMainPoint()
    {
        GameObject ball = Instantiate(this.ball, this.transform);
        ball.name = "Ball";

        if (trajectory != null)
        {
            trajectory.AddMainPoint(ball);

        }
        else
        {
            trajectory = new Trajectory(ball);
        }
        TextMesh text = ball.GetComponentInChildren<TextMesh>();
        text.text = (trajectory.NumMainPoint()).ToString();
    }

    public void CreateTrajectory()
    {
        if (trajectory != null)
        {
            trajectory.Destroy();
            trajectory = new Trajectory();
            //TrajectoriesMass.Add(trajectory);
        }
        else
        {

            trajectory = new Trajectory();
        }
    }

    public List<GameObject> GetTrajectory()
    {
        /*
        if (TrajectoriesMass == null)
        {
            Debug.Log("TrajectoriesMass = null");
            return null;
        }
        return (ArrayList)TrajectoriesMass[TrajectoriesMass.Count-1];
        */
        return trajectory.getTrajectoty();
    }

    public void SaveTrajecroty()
    {
        if (TrajectoriesMass != null)
        {
            TrajectoriesMass.Add(trajectory);
            //ClearTraject();
        }
        else
        {
            TrajectoriesMass = new List<Trajectory>();
            TrajectoriesMass.Add(trajectory);
            //ClearTraject();
        }
    }

    public void DrawLine(GameObject ball1, GameObject ball2)
    {
        if (PathP2P == null)
        {
            PathP2P = new List<GameObject>();
        }
        else
        {
            PathP2P.Clear();
        }

        Vector3 p1 = ball1.transform.position;
        Vector3 p2 = ball2.transform.position;

        float del = 0.01f;
        int n = (int)((p2 - p1).magnitude / del);

        for (int i = 1; i < n - 1; i++)
        {
            GameObject point = Instantiate(viaPoint, TrajectoryData.Instance.transform);
            point.transform.position = p1 + (p2 - p1) * i / n;
            PathP2P.Add(point);
        }

        AddP2PTraject(ball1);
    }

    public void DrawCircle(GameObject ball1, GameObject ball2, GameObject ball3)
    {
        if (ball2 == null)
        {
            Destroy(this.point);
            point = null;
            DrawCircle(ball1, ball3);
            return;
        }
        if (PathP2P == null)
        {
            PathP2P = new List<GameObject>();
        }
        else
        {
            PathP2P.Clear();
        }

        Vector3 p1 = ball1.transform.position;
        Vector3 p2 = ball2.transform.position;
        Vector3 p3 = ball3.transform.position;

        Matrix<float> D = Matrix<float>.Build.DenseOfArray(new float[,]
        {
            { p1.x, p1.y, p1.z},
            { p2.x, p2.y, p2.z},
            { p3.x, p3.y, p3.z}
        });

        Matrix<float> D1 = Matrix<float>.Build.DenseOfArray(new float[,]
        {
            { 1, p1.y, p1.z},
            { 1, p2.y, p2.z},
            { 1, p3.y, p3.z}
        });

        Matrix<float> D2 = Matrix<float>.Build.DenseOfArray(new float[,]
        {
            { p1.x, 1, p1.z},
            { p2.x, 1, p2.z},
            { p3.x, 1, p3.z}
        });

        Matrix<float> D3 = Matrix<float>.Build.DenseOfArray(new float[,]
        {
            { p1.x, p1.y, 1},
            { p2.x, p2.y, 1},
            { p3.x, p3.y, 1}
        });

        float a =  D1.Determinant() / D.Determinant();
        float b =  D2.Determinant() / D.Determinant();
        float c =  D3.Determinant() / D.Determinant();

        Vector3 x2D = p2 - p1;

        Matrix<float> y2D_ = Matrix<float>.Build.DenseOfArray(new float[,] {
            { 0, -c, b },
            { c, 0, -a },
            { -b, a, 0 },
        }) * Matrix<float>.Build.DenseOfArray(new float[,] {
            { x2D.x},
            { x2D.y},
            { x2D.z}
        });
        Matrix<float> x2D_ = Matrix<float>.Build.DenseOfArray(new float[,]
        {
            { x2D.x, x2D.y, x2D.z }
        });
        //Matrix<float> test = x2D_ * y2D_;

        y2D_ = y2D_.Transpose();

        Matrix<float> x3_ = Matrix<float>.Build.DenseOfArray(new float[,] {
            { 0, (p1-p3).z, -(p1-p3).y},
            { -(p1-p3).z, 0, (p1-p3).x},
            { (p1-p3).y, -(p1-p3).x, 0}
        }) * y2D_.Transpose() / (Mathf.Sqrt(y2D_[0, 0] * y2D_[0, 0] + y2D_[0, 1] * y2D_[0, 1] + y2D_[0, 2] * y2D_[0, 2]));
        x3_ = x3_.Transpose();
        
        
        float x3 = Mathf.Sqrt(x3_[0, 0] * x3_[0, 0] + x3_[0, 1] * x3_[0, 1] + x3_[0, 2] * x3_[0, 2]);

        Matrix<float> y3_ = Matrix<float>.Build.DenseOfArray(new float[,] {
            { 0, (p1-p3).z, -(p1-p3).y},
            { -(p1-p3).z, 0, (p1-p3).x},
            { (p1-p3).y, -(p1-p3).x, 0}
        }) * x2D_.Transpose() / (Mathf.Sqrt(x2D_[0, 0] * x2D_[0, 0] + x2D_[0, 1] * x2D_[0, 1] + x2D_[0, 2] * x2D_[0, 2]));
        y3_ = y3_.Transpose();

        float y3_sing = ((p1 - p3).x * y2D_[0, 0] + (p1 - p3).x * y2D_[0, 0] + (p1 - p3).x * y2D_[0, 0] )/
            (Mathf.Sqrt(y2D_[0, 0] * y2D_[0, 0] + y2D_[0, 1] * y2D_[0, 1] + y2D_[0, 2] * y2D_[0, 2])*(p1-p3).magnitude);
        float y3;
        if (y3_sing > 0)
        {
            y3 = -Mathf.Sqrt(y3_[0, 0] * y3_[0, 0] + y3_[0, 1] * y3_[0, 1] + y3_[0, 2] * y3_[0, 2]);
        }
        else
        {
            y3 = Mathf.Sqrt(y3_[0, 0] * y3_[0, 0] + y3_[0, 1] * y3_[0, 1] + y3_[0, 2] * y3_[0, 2]);
        }
        //float test2 = Mathf.Sqrt(x3 * x3 + y3 * y3);
        float x1 = 0, y1 = 0;
        float x2 = (p2 - p1).magnitude, y2 = 0;
        //float x3 = x3_[0, 0], y3 = y3_[0, 0];


        float x0 = -0.5f * (y2 * (x3 * x3 + y3 * y3) + y3 * (0 - x2 * x2 - y2 * y2)) / (x2 * y3 + x3 * (-y2));
        float y0 = 0.5f * (x2 * (x3 * x3 + y3 * y3) + x3 * (0 - x2 * x2 - y2 * y2)) / (x2 * y3 + x3 * (-y2));

        float R = Mathf.Sqrt( x0 * x0 + y0 * y0);    

        float a_ = (p1 - p2).magnitude;
        float b_ = (p1 - p3).magnitude;
        float c_ = (p2 - p3).magnitude;

        //float R_test = a_ * b_ * c_ / Mathf.Sqrt((a_ + b_ + c_)* (-a_ + b_ + c_) * (a_ - b_ + c_) * (a_ + b_ - c_));

        float phi1 = Mathf.Atan2(-y0 / R, -x0 / R);
        float phi2 = Mathf.Atan2((y3 - y0) / R, (x3 - x0) / R);

        float l_arc = Mathf.Abs(R * (phi2 - phi1));

        float del = 0.01f;
        int n = (int)(l_arc / del);

        for (int i = 1; i < n - 1; i++)
        {
            float x = x0 + R * Mathf.Cos(phi1 + (phi2 - phi1) * i / n);
            float y = y0 + R * Mathf.Sin(phi1 + (phi2 - phi1) * i / n);
            Matrix<float> p_ = x2D_ * x / (Mathf.Sqrt(x2D_[0, 0] * x2D_[0, 0] + x2D_[0, 1] * x2D_[0, 1] + x2D_[0, 2] * x2D_[0, 2])) +
                y2D_ * y / (Mathf.Sqrt(y2D_[0, 0] * y2D_[0, 0] + y2D_[0, 1] * y2D_[0, 1] + y2D_[0, 2] * y2D_[0, 2]));
            

            GameObject point = Instantiate(viaPoint, TrajectoryData.Instance.transform);
            point.transform.position = p1 + new Vector3(p_[0, 0], p_[0, 1], p_[0, 2]);
            PathP2P.Add(point);

        }

        AddP2PTraject(ball1);
        Destroy(this.point);
        this.point = null;
    }

    public void DrawCircle(GameObject ball1, GameObject ball3)
    {
        GameObject point = Instantiate(viaPoint, TrajectoryData.Instance.transform);
        point.transform.position = ball1.transform.position + (ball3.transform.position - ball1.transform.position)/2  + new Vector3(0, 0.1f, 0.1f);
        //DrawCircle(ball1, point, ball3);
        //Destroy(point);
        this.ball1 = ball1;
        this.ball3 = ball3;
        this.point = point;
    }

    public void DrawCircle()
    {
        DrawCircle(ball1, point, ball3);
        this.ball1 = null;
        this.ball3 = null;
        this.point = null;
    }
    
    GameObject ball1;
    GameObject ball3;
    GameObject point;


    public void DeleteSegment(GameObject ball1, GameObject ball2)
    {
        trajectory.DeleteSegments(ball1, ball2);
    }
}
