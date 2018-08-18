using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    public float jumpHeight;
    public float moveDirection;
    private float defaultMoveSpeed;
    private float defaultJumpHeight;
    public bool grounded;
    private bool controlsEnabled;
    public int currentChunk = 0;
    public WorldManager parentWorld;
    public Rigidbody2D rb;
    public Transform groundLight; 

    //Used for checking if the pc is grounded
    public Transform groundedTopLeft;
    public Transform groundedBottomRight;
    public LayerMask groundLayer;
    //PowerUp functionality
    public float powerupCountDown = 200f;
    private bool isPoweredUp = false;
    private PowerUp currentPowerUp = PowerUp.EMPTY;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        moveDirection = 0f;
        controlsEnabled = true;
        defaultMoveSpeed = moveSpeed;
        defaultJumpHeight = jumpHeight;
	}
	
	// Update is called once per frame
	void Update () {
        grounded = Physics2D.OverlapArea(groundedTopLeft.position, groundedBottomRight.position, groundLayer);

        if (controlsEnabled)
        {
            moveDirection = Input.GetAxis("Horizontal");
            if (Input.GetKeyDown(KeyCode.UpArrow) && grounded)
            {
                ControllerJump();
            }
            ControllerMove();
        }

        if (isPoweredUp)
        {
            if (powerupCountDown < 0)
            {
                moveSpeed = defaultMoveSpeed;
                jumpHeight = defaultJumpHeight;
                isPoweredUp = false;
                powerupCountDown = 200f;
            }
            powerupCountDown--;
        }
	}

    public int GetCurrentChunk()
    { 
        return (int)System.Math.Floor(transform.position.x / parentWorld.chunkSize);
    }

    void ControllerJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
    }

    void ControllerMove()
    {
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
    }

    void ControllerSink()
    {
        rb.transform.Translate(new Vector3(0, -50, 0) * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.transform.name.Contains("lava"))
        {
            controlsEnabled = false;
            ControllerSink();
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.name.Contains("ground") || col.transform.name.Contains("ceiling"))
        {
            GroundTile ground = col.transform.GetComponent<GroundTile>();
            if (!ground.triggered)
            {
                col.transform.GetComponent<SpriteRenderer>().color = new Color(0.53f, 1, 0.81f);
                Transform lightTransform = Transform.Instantiate(groundLight, new Vector3(col.transform.position.x, col.transform.position.y, col.transform.position.z - 2), Quaternion.identity);
                ground.triggered = true;
            }
        }
        else if (col.transform.name.Contains("coin"))
        {
            col.gameObject.SetActive(false);
        }
        else if (col.transform.name.Contains("starburst"))
        {
            Starburst sb = (Starburst)col.gameObject.GetComponent("Starburst");
            sb.triggered = true;
        }
        else if (col.transform.name.Contains("speed_up"))
        {
            StartPowerup(PowerUp.SPEED_UP);
            col.gameObject.SetActive(false);
        }
        else if (col.transform.name.Contains("speed_down"))
        {
            StartPowerup(PowerUp.SPEED_DOWN);
            col.gameObject.SetActive(false);

        }
        else if (col.transform.name.Contains("jump_boost"))
        {
            StartPowerup(PowerUp.JUMP_BOOST);
            col.gameObject.SetActive(false);
        }
    }

    void StartPowerup(PowerUp pu)
    {
        //Disable previous powerup if we're already using one
        if (isPoweredUp)
        {
            isPoweredUp = false;
            moveSpeed = defaultMoveSpeed;
            jumpHeight = defaultJumpHeight;
            powerupCountDown = 200f;
        }

        switch(pu)
        {
            case (PowerUp.JUMP_BOOST):
                jumpHeight *= 1.25f;
                break;
            case (PowerUp.SPEED_UP):
                moveSpeed *= 1.5f;
                break;
            case (PowerUp.SPEED_DOWN):
                moveSpeed *= 0.5f;
                break;
        }
        isPoweredUp = true;
    }
}
