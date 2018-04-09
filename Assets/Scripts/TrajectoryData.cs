using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class TrajectoryData : Singleton<TrajectoryData>
{

    public GameObject ball;
    private ArrayList Trajectory;
    private ArrayList TrajectoriesMass;

    public void AddPoint(GameObject point)
    {
        if (Trajectory != null)
        {
            Trajectory.Add(point);

        }
    }

    public void CreatePoint()
    {
        GameObject ball = Instantiate(this.ball,this.transform);
        //ball.transform.Translate(new Vector3(0.1f, 0.1f, 0.1f));
        ball.SendMessage("StartTranslation");

        if (Trajectory != null)
        {
            Trajectory.Add(ball);
            
        }
    }
    public void CreateTrajectory()
    {
        if (Trajectory != null)
        {
            ClearTraject();

        }
        else
        {
            Trajectory = new ArrayList();
        }
    }

    public ArrayList GetTrajectory()
    {
        /*
        if (TrajectoriesMass == null)
        {
            Debug.Log("TrajectoriesMass = null");
            return null;
        }
        return (ArrayList)TrajectoriesMass[TrajectoriesMass.Count-1];
        */
        return Trajectory;
    }

    public void SaveTrajecroty()
    {
        if (TrajectoriesMass != null)
        {
            TrajectoriesMass.Add(Trajectory);
            //ClearTraject();
        }
        else
        {
            TrajectoriesMass = new ArrayList();
            TrajectoriesMass.Add(Trajectory);
            //ClearTraject();
        }
    }

    private void ClearTraject()
    {
        if (Trajectory != null)
        {
            for (int i = 0; i < Trajectory.Count; i++)
            {
                Destroy((GameObject)Trajectory[i]);
            }
            Trajectory.Clear();

        }
    }
}
