using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform player;
    public float yOffset;
    public float xLim;
    public float yLim;
	// Update is called once per frame
	void Update () {
        //pan camera right if player goes past xLim box
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        float zPos = transform.position.z;

        if(player.transform.position.x > transform.position.x + xLim)
        {
            xPos = player.transform.position.x - xLim;
        }
        //pan camera up or down if player goes higher or lower than yLim box
        if(player.transform.position.y > transform.position.y + yLim)
        {
            yPos = player.transform.position.y - yLim;
        }
        if(player.transform.position.y < transform.position.y - yLim)
        {
            yPos = player.transform.position.y + yLim;
        }
        transform.position = new Vector3(xPos, yPos + yOffset, zPos);
    }
}
