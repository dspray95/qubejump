using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbPatrol : MonoBehaviour {

    
    public Transform undergroundRight;
    public Transform undergroundLeft;
    public Transform hillRight;
    public Transform hillLeft;
    public Transform patrolRight;
    public Transform patrolLeft;
    public Transform groundedTop;
    public Transform groundedBottom;
    public LayerMask groundLayer;
    public Transform trajectoy;
    private Rigidbody2D rb;

    public float jumpHeight;
    public float moveSpeed;
    public float reactionDistance;
    public bool direction = false; //False = left, true = right bool used for elegant reversing of direction 
    public bool jumping = false;
    public float jumpingTarget;
    public bool grounded;
    
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
    //Racasts to check if we have a drop off, a jumpable object, an unjumpable object, or a hazard coming up
    //Then adjusts movement accordingly
	void Update () {
        grounded = Physics2D.OverlapArea(groundedTop.position, groundedBottom.position, groundLayer);
        //Do Raycasting
        Patrol();
        //Work from results
        float moveDirection = direction ? 1 : -1;
        
        if(jumping && ((transform.position.x > jumpingTarget && direction) || (transform.position.x < jumpingTarget && !direction)))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (grounded)
            {
                jumping = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        }

    }

    public void Patrol()
    {
        RaycastHit2D undergroundRay = direction ? Physics2D.Raycast(undergroundLeft.position, new Vector2(1, 0), groundLayer) : Physics2D.Raycast(undergroundRight.position, new Vector2(-1, 0), groundLayer);
        RaycastHit2D hillRay = direction ? Physics2D.Raycast(hillLeft.position, new Vector2(1, 0), groundLayer) : Physics2D.Raycast(hillRight.position, new Vector2(-1, 0), groundLayer);
        RaycastHit2D patrolRay = direction ? Physics2D.Raycast(patrolRight.position, new Vector2(1, 0), groundLayer) : Physics2D.Raycast(patrolLeft.position, new Vector2(-1, 0), groundLayer);

        Debug.DrawRay(patrolLeft.position, new Vector2(-1, 0), Color.blue);
        Debug.DrawRay(patrolRight.position, new Vector2(1, 0), Color.yellow);

        if (patrolRay)
        {
            if (patrolRay.collider.name.Contains("ceiling") || patrolRay.collider.name.Contains("ground"))
            {
                float patrolDistance = direction ? patrolRay.collider.transform.position.x - transform.position.x : transform.position.x - patrolRay.collider.transform.position.x;
                Debug.Log(patrolDistance + " target x:" + patrolRay.collider.transform.position.x + " current x:" + transform.position.x + " moving:" + direction);

                if (patrolDistance < reactionDistance)
                {
                    if (hillRay)
                    {
                        float hillDistance = direction ? hillRay.collider.transform.position.x - transform.position.x : transform.position.x - patrolRay.collider.transform.position.x;
                        if (hillDistance < reactionDistance)
                        {
                            direction = !direction;
                        }
                        else if(grounded)
                        { 
                            Jump(patrolRay.collider.transform);
                        }
                    }
                    else if (grounded)
                    {
                        Jump(patrolRay.collider.transform);
                    }
                }
            }
        }
        if (undergroundRay)
        {
            float distance = direction ? transform.position.x - undergroundRay.collider.transform.position.x : undergroundRay.collider.transform.position.x - transform.position.x;
            Debug.Log(distance);
            if (distance < reactionDistance)
            {
                direction = !direction;
            }
        }
    }

    public void Jump(Transform target)
    {
        for (float i = 0; i < moveSpeed; i += 0.5f)
        {
            GetTrajectory(25, 0.1f, new Vector2(i, jumpHeight));
        }

        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        jumpingTarget = target.position.x;
        jumping = true;
        Debug.Log("jumping to " + jumpingTarget);
    }

    public List<Vector3> GetTrajectory(int steps, float timeStep, Vector3 initial)
    {
        bool validLanding = false;
        List<Vector3> trajectoryPositions = new List<Vector3>();
        for (int i = 0; i < steps; i++)
        {
            float arcPositionTime = timeStep * i;
            Vector2 arcPosition = initial * arcPositionTime;
            arcPosition += 0.5f * Physics2D.gravity * (arcPositionTime * arcPositionTime);
            arcPosition += new Vector2(transform.position.x, transform.position.y);

            if (i > 1) //Skip the first few segments to avoid collisions with the ground/self
            {
                RaycastHit2D cast = Physics2D.Linecast(trajectoryPositions[i - 1], arcPosition);
                if (cast)
                {
                    if (cast.transform.name.Contains("ground") && !cast.transform.name.Contains("under") && cast.collider.isTrigger)
                    {
                        validLanding = true;
                        Debug.Log("valid landing: " + initial);
                        break;
                    }
                    else //Invalid if we aren't going to land on top of a ground tile
                    {
                        validLanding = false;
                        break;
                    }
                }
            }
            trajectoryPositions.Add(arcPosition);
        }
        return trajectoryPositions;
    }
}
