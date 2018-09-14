using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp;


using traject_m = RosSharp.RosBridgeClient.Messages.Geometry.PoseArray;
using pose_m = RosSharp.RosBridgeClient.Messages.Geometry.Pose;
using Point_msg = RosSharp.RosBridgeClient.Messages.Geometry.Point;
using Quaternion_msg = RosSharp.RosBridgeClient.Messages.Geometry.Quaternion;

public class TrajectorySender : Publisher<traject_m> {

    private void Awake()
    {
        
    }

    public void PublishTrajectory()
    {
        GameObject baseFrame = AppManager.Instance.SelectedRobot;
        List<GameObject> trajectoryList =  TrajectoryData.Instance.GetTrajectory();
        traject_m traject_msg = new traject_m()
        {
            header = new RosSharp.RosBridgeClient.Messages.Standard.Header()
            {
                frame_id = "world"
            },
            poses = new pose_m[trajectoryList.Count]
        };

        for(int i = 0; i < trajectoryList.Count; i++)
        {
            traject_msg.poses[i] = CreatePoseMessage(baseFrame.transform, trajectoryList[i].transform);
        }

        Publish(traject_msg);

    }
    

    pose_m CreatePoseMessage(Transform base_T, Transform goal_T)
    {
        pose_m pose = new pose_m();

        Vector3 pos = goal_T.position - base_T.position;
        Quaternion ori = Quaternion.Inverse(base_T.rotation) * goal_T.rotation;

        ori = TransformExtensions.Unity2Ros(ori);
        pos = TransformExtensions.Unity2Ros(pos);



        Point_msg point = new Point_msg
        {
            x = -pos.y,
            y = pos.x,
            z = pos.z
        };

        float d = Mathf.Sqrt(ori.x * ori.x + ori.y * ori.y + ori.z * ori.z + ori.w * ori.w);
        Quaternion_msg quaternion = new Quaternion_msg
        {
            x = ori.x / d,
            y = ori.y / d,
            z = ori.z / d,
            w = ori.w / d

        };
        
        pose.position = point;
        pose.orientation = quaternion;

        return pose;
    }



}
