using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Nav;
using UnityEngine;

public class odom_subscriber : MonoBehaviour
{
    // Variables required for ROS communication
    [SerializeField]
    public string odom_topicname = "/odom";
     
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<OdometryMsg>(odom_topicname, odomCallback);
        
        // wheels = new GameObject[4];
        // wheels[0] = transform.Find("Wheel1").gameObject;
    }
    
    //public void cmdCallback(Diablo_CtrlMsg cmd_msg)
    public void odomCallback(OdometryMsg odom_msg) 
    {
    	float new_z = (float)odom_msg.pose.pose.position.y;
    	float new_y = (float)odom_msg.pose.pose.position.z;
    	float new_x = (float)odom_msg.pose.pose.position.x;
    	Vector3 new_position = new Vector3( new_x, new_y, new_z );
    	
    	// 右手系转左手系并且旋转90度
    	float qz    = -(float)odom_msg.pose.pose.orientation.y;
    	float qy    = -(float)odom_msg.pose.pose.orientation.z;
    	float qx    = -(float)odom_msg.pose.pose.orientation.x;
    	float qw    = (float)odom_msg.pose.pose.orientation.w;
    	
    	Quaternion new_rotation = new Quaternion(qx, qy, qz ,qw);
 
    	this.transform.position = new_position;
    	this.transform.rotation = new_rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position += Vector3.up * 0.01f;;
    }
}
