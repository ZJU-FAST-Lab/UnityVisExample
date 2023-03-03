using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using System;
using Random = Unity.Mathematics.Random;

public class LidarPlane : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 position, direction, hit_pos;
    Ray ray;
    RaycastHit hit;
    
    private GameObject father_obj;
    private float fov_2;

	uint scanPointCount;
	private Random random;

	//
	private PointFieldMsg field_x, field_y, field_z;
    
    // Variables required for ROS communication
    [SerializeField]
    private string topic_name = "/point_cloud";
    [SerializeField]
    private string frame_id   = "world";
    [SerializeField]
    private float fov        = 60.0f;
    [SerializeField]
    private float range      = 5.0f;
    [SerializeField]
    private float resolution = 1.0f;
    [SerializeField]
    private float noise_sigma = 0.1f;

	[SerializeField]
    private int   scanRate   = 15;

    // [SerializeField]
	private bool  mergeLocalization = true;
	// ROS Connector
    ROSConnection m_Ros;

	// Time
	private float _timeElapsed = 0f;
  	private float _timeStamp   = 0f;

    void Start()
    {
     // Get ROS connection static instance
        m_Ros = ROSConnection.GetOrCreateInstance();
        m_Ros.RegisterPublisher<PointCloud2Msg>(this.topic_name);
        
        father_obj = this.transform.parent.gameObject;
        fov_2 = fov / 2;

		field_x = new PointFieldMsg();
		field_y = new PointFieldMsg();
		field_z = new PointFieldMsg();

		field_x.name     = "x";
		field_x.offset   = 0;
		field_x.datatype = 7;
		field_x.count    = 1;

		field_y.name     = "y";
		field_y.offset   = 4;
		field_y.datatype = 7;
		field_y.count    = 1;

		field_z.name     = "z";
		field_z.offset   = 8;
		field_z.datatype = 7;
		field_z.count    = 1;

		this.random = new Random(1);

		scanPointCount   = (uint)( (Math.Ceiling(fov/resolution) + 1)* (Math.Ceiling(fov/resolution) + 1));
    }
   
    void scanAndPublish()
    {
 		var pc_msg = new PointCloud2Msg();
		pc_msg.header.frame_id = this.frame_id;
		pc_msg.height    = 1;
		pc_msg.width     = this.scanPointCount;
		pc_msg.fields    = new PointFieldMsg[3];
		pc_msg.fields[0] = this.field_x;
		pc_msg.fields[1] = this.field_y;
		pc_msg.fields[2] = this.field_z;
		pc_msg.is_bigendian = false;
		pc_msg.point_step = 12;
		pc_msg.row_step   = (uint)(12 * this.scanPointCount);
		pc_msg.data       = new byte[pc_msg.row_step];

		BitConverter.GetBytes(3.1);

		float dir_x = 1.0f;
		float dir_y;
    	float dir_z;
		float px;
		float py;
		float pz;
		double distance;
		Vector3 p_lidar, p_world;
		int array_index = 0;
		for(float fov_i = -fov_2 ; fov_i <= fov_2; fov_i += resolution)
		{
			for(float fov_j = -fov_2 ; fov_j <= fov_2; fov_j += resolution)
			{
				dir_y = (float)Math.Cos(fov_j);
				dir_z = (float)Math.Cos(fov_i);
				p_lidar = new Vector3( dir_x , dir_y , dir_z );
				if(mergeLocalization) {
					p_world = father_obj.transform.rotation * p_lidar;
				}
				else{
					p_world = p_lidar;
				}
				ray.direction = p_world;
				if(Physics.Raycast(ray, out hit, range) )
				{
					hit_pos = hit.point;
					distance = Vector3.Distance(hit_pos, position);
					if( distance > 0.6){
						px = hit_pos.x;
						py = hit_pos.y;
						pz = hit_pos.z;
						float rand1 = this.random.NextFloat();
						float rand2 = this.random.NextFloat();
						float rand3 = this.random.NextFloat();
						float cd1    = (float)(rand1 * noise_sigma * distance);
						float cd2    = (float)(rand2 * noise_sigma * distance);
						float cd3    = (float)(rand3 * noise_sigma * distance);
						px += cd1;
						py += cd2;
						pz += cd3;

						pc_msg.data[array_index++] =  BitConverter.GetBytes(px)[0] ;
						pc_msg.data[array_index++] =  BitConverter.GetBytes(px)[1] ;
						pc_msg.data[array_index++] =  BitConverter.GetBytes(px)[2] ;
						pc_msg.data[array_index++] =  BitConverter.GetBytes(px)[3] ;

						pc_msg.data[array_index++] =  BitConverter.GetBytes(pz)[0] ;
						pc_msg.data[array_index++] =  BitConverter.GetBytes(pz)[1] ;
						pc_msg.data[array_index++] =  BitConverter.GetBytes(pz)[2] ;
						pc_msg.data[array_index++] =  BitConverter.GetBytes(pz)[3] ;

						pc_msg.data[array_index++] =  BitConverter.GetBytes(py)[0] ;
						pc_msg.data[array_index++] =  BitConverter.GetBytes(py)[1] ;
						pc_msg.data[array_index++] =  BitConverter.GetBytes(py)[2] ;
						pc_msg.data[array_index++] =  BitConverter.GetBytes(py)[3] ;
					}
				}
			}
		}

    	m_Ros.Publish(this.topic_name, pc_msg);
    }

    // Update is called once per frame
	uint randomSeed = 1;
    void Update()
    {
    	this._timeElapsed    += Time.deltaTime;
    	if(this._timeElapsed > (1f/this.scanRate)) {
    		uint sec = (uint)Math.Truncate(this._timeStamp);
      		uint nanosec = (uint)( (this._timeStamp - sec)*1e+9 );

			position      =  father_obj.transform.position;
	    	ray.origin    =  position;
			this.random.InitState(randomSeed);
			if(randomSeed++ == 0){randomSeed = 1;}
			scanAndPublish();
			// Update time
			this._timeElapsed = 0;
			this._timeStamp = Time.time;
		}
    }
    
    
    
}
