using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;

using traject_m = RosSharp.RosBridgeClient.Messages.Trajectory.JointTrajectory;


public class TrajectoryReceiver : Subscriber<traject_m> {
    private bool ready;
    private float[][] joint_trajectory;

    private void Awake()
    {
        
    }

    protected override void ReceiveMessage(traject_m message)
    {
        Debug.Log("Get Trajectory");
        if (message.points != null)
        {
            ParseTrajectory(message);
            ready = true;
        }
    }

    private void ParseTrajectory(traject_m message)
    {
        joint_trajectory = new float[message.points.Length][];

        for (int i = 0; i < message.points.Length; i++)
        {
            joint_trajectory[i] = (float[])message.points[i].positions.Clone();
        }
    }


    public float[][] GetJointTrajectory()
    {
        if (!ready)
        {
            Debug.Log("Trajecory don't ready");
            return null;
        }
        return joint_trajectory;
    }
}
