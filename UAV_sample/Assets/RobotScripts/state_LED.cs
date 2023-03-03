using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Std;
using UnityEngine;

public class state_LED : MonoBehaviour
{
    // Start is called before the first frame update
    
    GameObject[] components = new GameObject[2];
    
    Color green = new Color(0,255,0);
    Color red   = new Color(255,0,0);
    Color blue  = new Color(0,0,255);
    
    [SerializeField]
    public string state_topicname = "/outlier_state";
    
    [SerializeField]
    int drone_id = 0;
    
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<Int32MultiArrayMsg>(state_topicname, stateCallback);
        components[0] = transform.GetChild(0).gameObject;
        components[1] = transform.GetChild(1).gameObject;
        
    }
    
    public void stateCallback(Int32MultiArrayMsg state_msg) 
    {
    	int[] dats = state_msg.data;
    	if( dats[0] != drone_id ){return ; } 
    	
    	if( dats[2] == 1 ) {
    	    Light l = components[0].GetComponent<Light>();
    	    l.color = red;
    	    l.intensity = 0.002f;
    	    Material mat = components[1].GetComponent<MeshRenderer>().material; // 获取材质
	    mat.color = red;
	}
    	else{
	    	if( dats[1] == 0 ){
	    	// green
	    	    Light l = components[0].GetComponent<Light>();
	    	    l.color = green;
	    	    l.intensity = 0.002f;
	    	    Material mat = components[1].GetComponent<MeshRenderer>().material; // 获取材质
		    mat.color = green;
	    	}
	    	else if( dats[1] == 1 ){
	    	// red
	    	    Light l = components[0].GetComponent<Light>();
	    	    l.color = blue;
	    	    l.intensity = 0.002f;
	    	    Material mat = components[1].GetComponent<MeshRenderer>().material; // 获取材质
		    mat.color = blue;
	    	}
	}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
