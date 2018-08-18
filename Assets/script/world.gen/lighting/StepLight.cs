using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepLight : MonoBehaviour {

    public float lightStep;
    Light lightComponent;

    private void Start()
    {
        lightComponent = transform.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update () {
        lightComponent.intensity -= lightStep;
        if(lightComponent.intensity <= 0)
        {
            Destroy(transform.gameObject);
        }
	}
}
