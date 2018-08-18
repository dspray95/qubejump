using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour {

    public float step;
    public float range;
    bool ascending;
    SpriteRenderer sr;

    // Use this for initialization
    void Start()
    {
        sr = transform.GetComponent<SpriteRenderer>();

        sr.color = new Color(Random.Range(1 - range, 1), Random.Range(1 - range, 1), sr.color.b, sr.color.a);
    }

    // Update is called once per frame
    void Update()
    {
        ColorCycle();
    }

    void ColorCycle()
    {
        if (ascending && sr.color.r >= 1)
        {
            ascending = false;
        }
        if (!ascending && sr.color.r < 1 - range)
        {
            ascending = true;
        }

        if (ascending)
        {
            sr.color = new Color(sr.color.r + step, sr.color.g + step, sr.color.b, sr.color.a);
        }
        else
        {
            sr.color = new Color(sr.color.r - step, sr.color.g - step, sr.color.b, sr.color.a);
        }
    }
}
