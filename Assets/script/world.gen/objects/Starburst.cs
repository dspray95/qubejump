using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starburst : MonoBehaviour {

    public bool triggered = false;
    public float step = 11.25f;
    public LayerMask layerMask;
    private List<GameObject> targets;
    public Transform groundLight;

	// Update is called once per frame
	void Update () {
        if (triggered)
        {
            for(float i = -step; i < 360; i += step)
            {
                GetTarget(i) ;
            }
            //Get Targets
            //Create LineRenderers
            //When Linerenders complete, light up ground tile targets
            gameObject.SetActive(false);
        }
	}

    void GetTarget(float angle)
    {
        Vector2 lineEnd; 
        Vector3 ray = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, ray, 10, layerMask);
        Debug.DrawRay(transform.position, ray, Color.yellow, 10);
        //TODO triggered check for tiles so we dont render a bajillion lights 
        if (hit)
        {
            if (hit.collider.gameObject.name.Contains("ground") && !hit.collider.gameObject.name.Contains("under") ||
                hit.collider.gameObject.name.Contains("ceiling") && !hit.collider.gameObject.name.Contains("above")) 
            {
                GroundTile gt = (GroundTile) hit.collider.gameObject.GetComponent("GroundTile");

                if (!gt.triggered)
                {
                    SpriteRenderer sr = hit.collider.gameObject.GetComponent<SpriteRenderer>();
                    Transform.Instantiate(groundLight, new Vector3(hit.transform.position.x, hit.transform.position.y, hit.transform.position.z - 2), Quaternion.identity);
                    sr.color = Color.white;
                    lineEnd = hit.collider.transform.position;
                    gt.triggered = true;
                }
            }
        }

    }
}
