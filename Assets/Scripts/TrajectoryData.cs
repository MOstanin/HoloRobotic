using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;

public class TrajectoryData : Singleton<TrajectoryData>
{

    public GameObject ball;
    private ArrayList Trajectory;


    public void CreatePoint()
    {
        ball = Instantiate(ball,this.transform);
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
            Trajectory.Clear();
        }
        else
        {
            Trajectory = new ArrayList();
        }
    }

    public ArrayList GetTrajectory()
    {
        return Trajectory;
    }

}
