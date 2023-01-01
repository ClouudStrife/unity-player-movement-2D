using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    [SerializeField]
    private Rigidbody2D playerRB;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private Transform wallCheckTransform;
    [SerializeField]
    private Transform groundCheckTransform;
    [SerializeField]
    private float wallCheckRadius;
    [SerializeField]
    private float groundCheckRadius;
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private Vector2 wallJumpDirection;
    [SerializeField]
    private float wallSlidingSpeed;
    [SerializeField]
    private float wallGrabbingSpeed;
    [SerializeField]
    private float wallJumpForce;
    [SerializeField]
    private float startMovementSpeed;
    [SerializeField]
    private float jumpMovementSpeed;
    [SerializeField]
    private float maxMovementSpeed;
    
    

    private float playerMovementSpeed;
    private float jumpSpeed = 10f;
    private int jumpingCount = 0;
    private Vector2 movement;
    private bool onWall = false;
    private bool canMove = true;

    private bool canGrab = true;
    private bool isGrabbing = false;
    private float grabbingStartTime = 3.0f;
    private float grabbingTime;

    private bool onGround = false;
    private bool wallSliding = false;
    private bool facingRight = true;
    private int facingDirection = 1;
    private float coyoteTime;

    [SerializeField]
    private float coyoteStartTime;
    

    private const int BONUS_GRAV = 10;
    private const int FALL_PENALTY = 35;
    private const int ACCELERATE_RATE = 550;

    //Animation States
    const string PLAYER_IDLE = "player_idle";
    const string PLAYER_RUN = "player_run";
    


    void Start(){
        playerMovementSpeed = startMovementSpeed;
        coyoteTime = coyoteStartTime;  

    }

    void Update(){

        //Get player movement vector
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Check if player is touching wall
        onWall = Physics2D.OverlapCircle(wallCheckTransform.position, wallCheckRadius, wallLayer);

        //Check if player is on ground
        onGround = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayer);


        //Facing direction
        if(movement.x < 0 && facingRight || movement.x > 0 && !facingRight)
            Flip();


        //Accelerate
        if(movement.x != 0 && onGround){
            if(playerMovementSpeed < maxMovementSpeed)
                playerMovementSpeed += ACCELERATE_RATE * Time.deltaTime;
            else
                playerMovementSpeed = maxMovementSpeed;

            playerAnimator.Play(PLAYER_RUN);
        }
        else{
            playerMovementSpeed = startMovementSpeed;
            playerAnimator.Play(PLAYER_IDLE);
        }
            
        //Player grab condition check
        if(onGround){
            canGrab = true;
            grabbingTime = grabbingStartTime;
            coyoteTime = coyoteStartTime;
        }else{
            coyoteTime -= Time.deltaTime;
        }

        //Check if player can jump
        if(onGround || onWall || coyoteTime > 0 ){
            if(Input.GetButtonDown("Jump")){
                Jump();
            }
        }
       
            

        //Player grabbing wall check
        if(isGrabbing){
            if(grabbingTime <= 0)
                canGrab = false;
            else
                grabbingTime -= Time.deltaTime;
        }

        //Player wall sliding check
        if(onWall && !onGround && playerRB.velocity.y < 0 && movement.x != 0)
            wallSliding = true;
        else
            wallSliding = false;
        

        //Adjust velocity of player based on his rb.velocity.y
        if(!onGround){
            if(playerRB.velocity.y > 0.01f)
                playerMovementSpeed = jumpMovementSpeed;
            else
                playerMovementSpeed = jumpMovementSpeed - FALL_PENALTY;


            //Applying gravity bonus limiting height of the jump
            Vector3 vel = playerRB.velocity;
            vel.y -= BONUS_GRAV * Time.deltaTime;
            playerRB.velocity = vel;
        }

        //Debug.Log(jumpingCount);  
    }

    void FixedUpdate(){
        if(canMove)
            Move();
    }

    void Flip(){
        Vector3 scale = gameObject.transform.localScale;
        scale.x *= -1;
        gameObject.transform.localScale = scale;
        facingRight = !facingRight;
        facingDirection *= -1;
    }

    void Move(){
        //Wall slide
        if(wallSliding && !(Input.GetButton("Grab"))){
            playerRB.velocity = new Vector2(playerRB.velocity.x, -wallSlidingSpeed);
            isGrabbing = false;
        } 
        //Wall grab
        else if(onWall && Input.GetButton("Grab")){

            if(canGrab){
                playerRB.velocity = new Vector2(0, movement.y * wallGrabbingSpeed);
                isGrabbing = true;
            }
        }
        //Walk           
        else{
            playerRB.velocity = new Vector2(movement.x * playerMovementSpeed * Time.fixedDeltaTime, playerRB.velocity.y);
            isGrabbing = false;
        }
            
    }

    void Jump(){
        //Adds jump force to the player rb
        if(onWall){
            Vector2 jumpDirection = new Vector2(wallJumpDirection.x * -facingDirection, wallJumpDirection.y);
            playerRB.velocity = Vector2.zero;
            playerRB.velocity += jumpDirection * wallJumpForce;
            
            //Stop player's movement for a while
            StartCoroutine("StopMove");

        }
        else{
            playerRB.velocity = new Vector2(playerRB.velocity.x, 0);
            playerRB.velocity += Vector2.up * jumpSpeed;
        }

        
    }

    IEnumerator StopMove(){
        canMove = false;
        
        yield return new WaitForSeconds(.25f);
        
        canMove = true;
    }

    void ReturnMovement(){
        canMove = true;
    }

    public void ApplyForce(){
        Vector3 dir = new Vector3(1,2.5f,0);
        float force = 5f;
        canMove = false;
        playerRB.velocity = Vector2.zero;
        playerRB.AddForce(dir * force, ForceMode2D.Impulse);
        Invoke("ReturnMovement", 1.0f);
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
        Gizmos.DrawWireSphere(wallCheckTransform.position, wallCheckRadius);
    }
}
