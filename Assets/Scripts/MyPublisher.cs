using System;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;
using RosSharp;

using msgs = RosSharp.RosBridgeClient.Messages;
using std_msgs = RosSharp.RosBridgeClient.Messages.Standard.String;
using pose_msg = RosSharp.RosBridgeClient.Messages.Geometry.PoseStamped;
using Point_msg = RosSharp.RosBridgeClient.Messages.Geometry.Point;
using Quaternion_msg = RosSharp.RosBridgeClient.Messages.Geometry.Quaternion;
using jointPos_msg = RosSharp.RosBridgeClient.Messages.Iiwa.JointPosition;

[RequireComponent(typeof(RosConnector))]

public class MyPublisher : MonoBehaviour
{
    public string TopicIIWA = "/iiwa/command/JointPosition";

    protected RosSocket rosSocket;
    protected string publicationIdIIWA;
    protected string publicationIdPlato;

    private float OldTime;
    private bool Flag;

    float[][] qList;
    int c;


    private void Start()
    {
        rosSocket = GetComponent<RosConnector>().RosSocket;
        
        publicationIdIIWA = rosSocket.Advertise<jointPos_msg>(TopicIIWA);

        OldTime = Time.time;
        Flag = false;
        c = 0;
    }

    private void Update()
    {
        if ((Time.time - OldTime > 0.2f) && Flag)
        {
            if ((Time.time - OldTime < 2f) && (c == 1))
            {
                return;
            }
            PublishJointsIIWA();
            OldTime = Time.time;
        }
    }

    public void PublishJointsIIWA()
    {
        if (qList != null){
            rosSocket.Publish(publicationIdIIWA, CreateJointsMessageIIWA(qList[c]));
            c++;
            if (c == qList.Length)
            {
                Flag = false;
                c = 0;
            }
        }
    }

    jointPos_msg CreateJointsMessageIIWA(float[] q)
    {
        jointPos_msg m = new jointPos_msg();
        m.position.a1 = q[0];
        m.position.a2 = q[1];
        m.position.a3 = q[2];
        m.position.a4 = -q[3];
        m.position.a5 = q[4];
        m.position.a6 = q[5];
        //m.position.a7 = q[6];
        m.position.a7 = 0;

        return m;
    }

    public void PublishPoseIIWA()
    {
        GameObject worldFrame = GameObject.Find("IIWA");
        GameObject goalFrame = GameObject.Find("BallFigure");

        pose_msg pose = CreatePoseMessageIIWA(worldFrame.transform, goalFrame.transform);
        rosSocket.Publish(publicationIdIIWA, pose);
        
    }


    pose_msg CreatePoseMessageIIWA(Transform base_T, Transform goal_T)
    {
        pose_msg pose = new pose_msg();

        Vector3 pos = goal_T.position - base_T.position;
        Quaternion ori = Quaternion.Inverse(base_T.rotation) * goal_T.rotation;

        ori = TransformExtensions.Unity2Ros(ori);
        pos = TransformExtensions.Unity2Ros(pos);



        Point_msg point = new Point_msg
        {
            x = -pos.x,
            y = -pos.y,
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

        //DateTime.UtcNow.Millisecond;
        HeaderExtensions.Update(pose.header);

        pose.header.stamp.secs = DateTime.UtcNow.Second; ;
        pose.header.stamp.nsecs = DateTime.UtcNow.Millisecond;

        pose.header.frame_id = "map";
        pose.pose.position = point;
        pose.pose.orientation = quaternion;

        return pose;
    }


    private void OnDestroy()
    {
        rosSocket.Unadvertise(publicationIdIIWA);
    }

    private void GetQ()
    {
        qList = AppManager.Instance.GetQ().ToArray();
    }

    public void StartSendTrajectory()
    {
        GetQ();
        if (qList != null)
        {
            Flag = true;
        }
        else
        {
            Debug.Log("qList null");
        }
    }
}
