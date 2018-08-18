using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hopper : MonoBehaviour {

    public Transform groundedTopLeft;
    public Transform groundedBottomRight;
    public LayerMask groundLayer;
    private GameObject player;
    private Rigidbody2D rb;
    public float moveSpeed;
    public float jumpHeight;
    public float fleeDistance;
    public  bool grounded;
    private float distanceToPlayer; 
    
	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        grounded = Physics2D.OverlapArea(groundedTopLeft.position, groundedBottomRight.position, groundLayer);
        distanceToPlayer = (transform.position.x - player.transform.position.x) + (transform.position.y - player.transform.position.y);
        
        if(distanceToPlayer < fleeDistance)//Hop away
        {
            //set direction to moving away from the player
            float moveDirection = transform.position.x > player.transform.position.x ? 1 : -1; //Get direction
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y); //move away
            rb.gravityScale = 1;
            //If we're grounded hop
            if (grounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            }
        }
        else if(grounded)
        {
            rb.velocity = new Vector2(0, 0);    
        }

	}
}
