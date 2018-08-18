using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaLight : MonoBehaviour {

    public float step;
    public float range;
    float initVal;
    bool ascending;
    Light lavaLight;

	// Use this for initialization
	void Start () {
        lavaLight = transform.GetComponent<Light>();
        initVal = lavaLight.intensity;
	}
	
	// Update is called once per frame
	void Update () {

        if(ascending && lavaLight.intensity > initVal + (range/2))
        {
            ascending = false;
        }
        if(!ascending && lavaLight.intensity < initVal - (range / 2))
        {
            ascending = true;
        }

        if (ascending)
        {
            lavaLight.intensity += step;
        }
        else
        {
            lavaLight.intensity -= step;
        }
	}
}
