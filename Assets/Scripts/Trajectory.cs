using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Trajectory
{
    private List<GameObject> Points;
    private List<GameObject> FinalTrajectory;

    public Trajectory()
    {

    }
    public Trajectory(GameObject firtsBall)
    {
        Points = new List<GameObject>();
        Points.Add(firtsBall);

    }

    public void AddPoint(GameObject ball)
    {
        if (Points != null)
        {
            Points.Add(ball);
        }
        else
        {
            Points = new List<GameObject>();
            Points.Add(ball);
        }
    }

    public void AddPoint2PointTrajectory(GameObject Ball1, List<GameObject> path)
    {
        int n = Points.IndexOf(Ball1);
        GameObject Ball2 = Points[n + 1];
        Vector3 ori1 = Ball1.transform.eulerAngles;
        Vector3 delta_ori = Ball2.transform.eulerAngles - Ball1.transform.eulerAngles;

        for (int i = 0; i < path.Count; i++)
        {
            path[i].transform.eulerAngles = Ball1.transform.eulerAngles + delta_ori * i / path.Count;
        }

        Points.InsertRange(n+1, path);

    }

    public List<GameObject> getTrajectoty()
    {
        return Points;
    }

    internal void Destroy()
    {
        foreach (GameObject point in Points){
            GameObject.Destroy(point);
        }
    }
}