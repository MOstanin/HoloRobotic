using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Trajectory
{
    private List<GameObject> Points;
    private List<GameObject> FinalTrajectory;
    int n;

    public Trajectory()
    {
        n = 0;
    }
    public Trajectory(GameObject firtsBall)
    {
        Points = new List<GameObject>();
        Points.Add(firtsBall);
        
    }
    public int NumMainPoint()
    {
        return n;
    }

    public void AddMainPoint(GameObject ball)
    {
        if (Points != null)
        {
            Points.Add(ball);
            n = n + 1;
        }
        else
        {
            Points = new List<GameObject>();
            Points.Add(ball);
            n = 1;
        }
    }

    public void AddPoint2PointTrajectory(GameObject Ball1, List<GameObject> path)
    {
        int n = Points.IndexOf(Ball1);
        GameObject Ball2 = Points[n + 1];
        Vector3 ori1 = Ball1.transform.eulerAngles;
        float delta_ori = Quaternion.Angle(Ball1.transform.rotation,Ball2.transform.rotation);
        
        for (int i = 0; i < path.Count; i++)
        {
            //path[i].transform.eulerAngles = Ball1.transform.eulerAngles + delta_ori * i / path.Count;
            path[i].transform.rotation = Quaternion.RotateTowards(Ball1.transform.rotation, Ball2.transform.rotation, delta_ori * i / path.Count);
        }

        Points.InsertRange(n+1, path);

    }

    public List<GameObject> getTrajectoty()
    {
        return Points;
    }

    public void Destroy()
    {
        if (Points!= null)
        {
            foreach (GameObject point in Points)
            {
                GameObject.Destroy(point);
            }
        }
    }

    private void Destroy(List<GameObject> removedList)
    {
        foreach (GameObject point in removedList)
        {
            GameObject.Destroy(point);
        }
    }

    public void DeleteSegments(GameObject ball1, GameObject ball2)
    {
        int n1 = Points.IndexOf(ball1);
        int n2 = Points.IndexOf(ball2);
        
        List<GameObject> removedList = Points.GetRange(n1 + 1, n2 - n1 - 1);
        Destroy(removedList);

        Points.RemoveRange(n1+1, n2 - n1 - 1);
    }
}