using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject axe;
    public Transform firePoint;

    [SerializeField] private LayerMask layerMask;
    
    /* Vertical Movement */
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float wallRunVelocity;
    
    /* Horizontal Movement */
    [SerializeField] private float moveVelocity;
    [SerializeField] private float maxSpeed;
    private bool rightMove;
    private bool leftMove;
    private bool facingRight;
    
    /* Wall Movement */
    [SerializeField] private float wallDistance;
    [SerializeField] private float wallJumpHeight;
    [SerializeField] private float wallJumpWidth;

    /* Player Attributes */
    private BoxCollider2D collider;
    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRender;
    private Animator animator;

    /* Axe Attributes */
    private AxeController axeController;
    private bool axeInUse = false;
    private bool wallHang = false;


    // Start is called before the first frame update
    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        collider = transform.GetComponent<BoxCollider2D>();
        rigidbody = transform.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        axeController = GetComponentInChildren<AxeController>();
        
        rightMove = true;
        leftMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        /* Left Right Base Movement */
        if(Input.GetKey(KeyCode.D) && (rigidbody.velocity.x < maxSpeed) && rightMove)
        {
            rigidbody.velocity += Vector2.right * moveVelocity * Time.deltaTime;
            animator.SetBool("RunR", true);
        }

        if(Input.GetKey(KeyCode.A) && (rigidbody.velocity.x > -maxSpeed) && leftMove)
        {
            rigidbody.velocity += Vector2.left * moveVelocity * Time.deltaTime;
            animator.SetBool("RunR", false);
        }

        /* Axe Input Handling */
        if(Input.GetMouseButtonDown(0) && !axeInUse && !isGrounded())
        {
            axeInUse = true;
            Instantiate(axe, firePoint);
            //axeController.axeThrow(axe);
        }

        /*if(rigidbody.velocity.x <= 0 && facingRight)
            Flip();

        if(rigidbody.velocity.x > 0 && !facingRight)
            Flip();
        */

        /* Grounded Checks */
        if (isGrounded() && isTouchingWallLeft())
        {
            leftMove = false;
            rightMove = true;
            groundReset();

            animator.SetBool("WallHoldGL", true);
            jumpCheck(wallRunVelocity);
        }

        else if (isGrounded() && isTouchingWallRight())
        {
            rightMove = false;
            leftMove = true;
            groundReset();

            animator.SetBool("WallHoldGR", true);
            jumpCheck(wallRunVelocity);
        }

        else if (isGrounded())
        {
            rightMove = true;
            leftMove = true;
            groundReset();

            jumpCheck(jumpVelocity);
        }

        else
        {

        }

        /* Air Checks */
        if (!isGrounded() && isTouchingWallLeft())
        {
            leftMove = false;
            rightMove = true;

            animator.SetBool("WallHangL", true);
            animator.SetBool("WallHoldGL", false);

            //wallHangCheck();
            wallJumpCheck(wallJumpWidth, wallJumpHeight);
        }

        else if (!isGrounded() && isTouchingWallRight())
        {
            leftMove = true;
            rightMove = false;

            animator.SetBool("WallHangR", true);
            animator.SetBool("WallHoldGR", false);

            //wallHangCheck();
            wallJumpCheck(-wallJumpWidth, wallJumpHeight);
        }

        else if(!isGrounded())
        {
            rightMove = true;
            leftMove = true;


            animator.SetBool("WallHangR", false);
            animator.SetBool("WallHangL", false);
            animator.SetBool("JumpR", true);
        }

        else
        {

        }

        /* Debug Statements */
        //Debug.Log(rigidbody.velocity);  
        //Debug.Log(isTouchingWall()); 
    }

    public void retrieveAxe()
    {
        axeInUse = false;
    }

    private void jumpCheck(float height)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.AddForce(new Vector2(0f, height));
            if(height == wallRunVelocity)
            {
                animator.SetTrigger("WallRunL");
                animator.SetTrigger("WallRunR");
            }
        }
    }

    private void wallJumpCheck(float width, float height)
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //rigidbody.AddForce(new Vector2(width, height));
            rigidbody.velocity = new Vector2(width, height);
            wallHang = false;
            rigidbody.WakeUp();
            rigidbody.gravityScale = 4;

            retrieveAxe();
        }
    }

    private void wallHangCheck()
    {
        if(Input.GetKey(KeyCode.E) || wallHang)
        {
            axeInUse = true;

            rigidbody.velocity = Vector2.zero;
            rigidbody.gravityScale = 0;
            rigidbody.Sleep();

            wallHang = true;
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, 0.1f, layerMask);
        //Debug.Log(raycastHit2D.collider);
        return raycastHit2D.collider != null;    
    }

    private bool isTouchingWallLeft()
    {
        RaycastHit2D raycastHitLeft2D = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 
            0f, Vector2.left, wallDistance, layerMask);
        //Debug.Log(raycastHitLeft2D.collider);
        return raycastHitLeft2D.collider != null;
    }

    private bool isTouchingWallRight()
    {
        RaycastHit2D raycastHitRight2D = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 
            0f, Vector2.right, wallDistance, layerMask); 
        //Debug.Log(raycastHitRight2D.collider);
        return raycastHitRight2D.collider != null;
    }

    private void Flip()
    {
        transform.Rotate(0f, 180f, 0f);

        facingRight = !facingRight;
    }

    private void groundReset()
    {
        animator.SetBool("WallHoldGL", false);
        animator.SetBool("WallHoldGR", false);
        animator.SetBool("WallHangR", false);
        animator.SetBool("WallHangL", false);
        animator.SetBool("JumpR", false);
    }
}
