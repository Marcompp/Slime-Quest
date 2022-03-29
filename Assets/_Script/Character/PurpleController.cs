using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleController : MonoBehaviour
{
    [Header("Public Variables")]
    public float speed = 10.0f;
    public float jumpforce = 325;

    private Rigidbody2D rb;
    //Vamos salvar o objeto que atingimos com o raycast aqui
    //private GameObject raycastTarget;
    private Vector3 movingDirection;
    private Animator animator;

    private GameObject player;

    [Header("Private Variables")]
    [SerializeField]private Transform groundCheck;
    //[SerializeField]private Transform groundCheckII;
    [SerializeField]private Transform wallCheck;
    [SerializeField]private Transform wallCheckII;
    [SerializeField]private Transform ceilingCheck;
    [SerializeField]private float radOCircle;
    [SerializeField]private LayerMask whatIsGround;

    private bool facingRight = false;
    private bool onFloor = false;
    private bool onWall = false;
    private bool onWallII = false;
    private bool onCeiling = false;
    private bool goingUp = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        //raycastTarget = null;
        groundCheck = gameObject.transform.Find("groundcheck");
        //groundCheckII = gameObject.transform.Find("groundcheckII");
        wallCheck = gameObject.transform.Find("wallcheck");
        wallCheckII = gameObject.transform.Find("wallcheckII");
        ceilingCheck = gameObject.transform.Find("ceilingcheck");

        if (rb.transform.position.y < 0) {
            goingUp = false;
            speed *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        //HandleInteractions();
        HandleAnimation();
    }

    private void HandleMovement() {
        onFloor = Physics2D.OverlapCircle(groundCheck.position,radOCircle,whatIsGround);
        //Todo
        onWall = Physics2D.OverlapCircle(wallCheck.position,radOCircle,whatIsGround);
        onWallII = Physics2D.OverlapCircle(wallCheckII.position,radOCircle,whatIsGround);
        onCeiling = Physics2D.OverlapCircle(ceilingCheck.position,radOCircle,whatIsGround);
        if (!(onWall || onWallII)) {
            Flip();
        }
        if (!goingUp && (onFloor || !onWallII)) {
            speed*=-1;
            goingUp = true;
        }
        else if (goingUp && (onCeiling || !onWall)) {
            speed*=-1;
            goingUp = false;
        }
        rb.velocity = new Vector2(0,speed);

        Vector2 posicaoViewport = Camera.main.WorldToViewportPoint(transform.position);
        if(posicaoViewport.y < 0)
        {
            Destroy(gameObject);
        }
    }

    private void Flip()
    {
        
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        //Vector3 wallCheckpos = wallCheck.position;
        //wallCheck.transform.position = new Vector3(wallCheck.position.x *(-1),wallCheck.position.y,0);
    }


    private void HandleAnimation() {
        animator.SetBool("isAirborne", true);
        if (onWall || onWallII) {
            animator.SetBool("isSliding", true);
        }
        else {
            animator.SetBool("isSliding", false);
        }
    }

    private void OnDrawGizmos()
    {
       Gizmos.DrawSphere(groundCheck.position, radOCircle);
       Gizmos.DrawSphere(wallCheck.position, radOCircle);
       Gizmos.DrawSphere(wallCheckII.position, radOCircle);
       Gizmos.DrawSphere(ceilingCheck.position, radOCircle);
    }
}
