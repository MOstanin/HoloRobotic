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
            for (int i = 0; i < Trajectory.Count; i++)
            {
                Destroy((GameObject) Trajectory[i]);
            }
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
