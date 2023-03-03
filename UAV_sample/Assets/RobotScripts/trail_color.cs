using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trail_color : MonoBehaviour
{
    // Start is called before the first frame update
    
    GameObject trail_render;
    Material mat;
    
    [ColorUsageAttribute(true, true)]
    [SerializeField]
    Color color = new Color(255,255,0);
    
    void Start()
    {
        
    	trail_render = transform.Find("trial").gameObject;
        mat = trail_render.GetComponent<TrailRenderer>().material; // 获取材质
	mat.color = color;
    }


    // Update is called once per frame
    void Update()
    {
    }
}
