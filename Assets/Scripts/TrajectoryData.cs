using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class TrajectoryData : Singleton<TrajectoryData>
{
    public Trajectory trajectory;
    public GameObject ball;
    private List<GameObject> PathP2P;
    private List<Trajectory> TrajectoriesMass;

    public void AddPoint(GameObject point)
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
        GameObject ball = Instantiate(this.ball,this.transform);
        ball.name = "Ball";

        if (trajectory != null)
        {
            trajectory.AddPoint(ball);

        }
        else
        {
            trajectory = new Trajectory(ball);
        }
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
}
