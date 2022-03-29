using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    [Header("Public Variables")]
    public float speed = 10.0f;
    public float jumpforce = 325;
    public float xwallforce = 200;
    public float ywallforce = 255;
    public float xknockbackforce = 300;
    public float yknockbackforce = 120;
    public float wallJumpTime = 2;
    public float knockbackTime = 0.01f;

    private Rigidbody2D rb;
    //Vamos salvar o objeto que atingimos com o raycast aqui
    private GameObject raycastTarget;
    private Vector3 movingDirection;
    private Animator animator;

    [Header("Private Variables")]
    [SerializeField]private Transform groundCheck;
    [SerializeField]private Transform groundCheckII;
    [SerializeField]private Transform wallCheck;
    [SerializeField]private float radOCircle;
    [SerializeField]private LayerMask whatIsGround;

    private bool facingRight = true;
    private bool onFloor = false;
    private bool onWall = false;
    private bool wallJumping = false;
    private bool knockback = false;

    //private Color coloration;
    GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        raycastTarget = null;
        groundCheck = gameObject.transform.Find("groundcheck");
        groundCheckII = gameObject.transform.Find("groundcheckII");
        wallCheck = gameObject.transform.Find("wallcheck");

        gm = GameManager.GetInstance();
    }

    // Update is called once per frame
    private void Update() {
        if (gm.reset) {
            Reset();
            gm.reset = false;
        }
        if (gm.gameState != GameManager.GameState.GAME) return;
        HandleMovement();
        HandleInteractions();
        HandleAnimation();

        if(Input.GetKeyDown(KeyCode.Escape) && gm.gameState == GameManager.GameState.GAME) {
            gm.ChangeState(GameManager.GameState.PAUSE);
        }
    }

    
    private void FixedUpdate() {
        LayerMask mascara = LayerMask.GetMask("Collectable");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 2, mascara);

        //Se houve colisão
        if (hit.collider != null) {
            print(hit.collider.name);
            raycastTarget = hit.collider.gameObject;
        }
        else { //Quando o raycast para de atingir o objeto limpo a referência.
            raycastTarget = null;
        }
    }

    private void HandleMovement() {
        onFloor = (Physics2D.OverlapCircle(groundCheck.position,radOCircle,whatIsGround) || Physics2D.OverlapCircle(groundCheckII.position,radOCircle,whatIsGround));
        //Todo
        float hAxis = Input.GetAxis("Horizontal");
        //float vAxis = Input.GetAxis("Vertical");

        onWall = (Physics2D.OverlapCircle(wallCheck.position,radOCircle,whatIsGround) && ((Mathf.Abs(rb.velocity.x) > 0.1) || hAxis != 0));

        //Debug.Log($"velocity: {rb.velocity.x}");

        //movingDirection = new Vector3(hAxis, rb.velocity.y).normalized;
        //Vector3 direction = new Vector3(hAxis, vAxis).normalized;

        if (wallJumping && (onFloor /*|| onWall*/)) {
            wallJumping = false;
        }

        if (!wallJumping && !knockback) {
            rb.velocity = new Vector2(hAxis * speed,rb.velocity.y);
        }


        if(Input.GetButtonDown("Jump")){
            if (onFloor) {
                animator.SetTrigger("Jump");
                rb.AddForce(Vector3.up * jumpforce);
            }
            else if (onWall) {
                CancelInvoke("EndWallJump");
                animator.SetTrigger("Jump");
                rb.velocity = new Vector2(0,0);
                if (facingRight) {
                    hAxis = -1;
                }
                else {
                    hAxis = 1;
                }
                Flip();
                rb.AddForce(new Vector2(xwallforce*(hAxis),ywallforce));
                wallJumping = true;
                Invoke("EndWallJump",wallJumpTime);
            }
        }

        //rb.MovePosition(transform.position + movingDirection * speed * Time.deltaTime);

        if ((rb.velocity.x > 0.5 && !facingRight) || (rb.velocity.x < -0.5 && facingRight) && !knockback)
        {
            Flip();
        }
        
        Vector2 posicaoViewport = Camera.main.WorldToViewportPoint(transform.position);
        if(posicaoViewport.y < 0)
        {
            Death();
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

    private void EndWallJump() {
        if (wallJumping) {
            wallJumping = false;
            rb.velocity = new Vector2(rb.velocity.x,0);
        }
    }

    private void Knockback() {
            float direction = 1;
            CancelInvoke("EndKnockback");
            animator.SetBool("isDamaged",true);
            rb.velocity = new Vector2(0,0);
            if (facingRight) {
                direction = -1;
            }
            else {
                direction = 1;
            }
            rb.AddForce(new Vector2(xknockbackforce*(direction),yknockbackforce));
            knockback = true;
            Invoke("EndKnockback",knockbackTime);
    }

    private void EndKnockback() {
        animator.SetBool("isDamaged",false);
        if (knockback) {
            knockback = false;
        }
    }

    private void Death()
    {
        gm.vidas -=1;
        if(gm.vidas <= 0 && gm.gameState == GameManager.GameState.GAME)
        {
            gm.ChangeState(GameManager.GameState.ENDGAME);
        }    
        else {
            Reset();
        }    
    }

    private void Reset()
    {
        animator.SetBool("isDamaged", false);
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        transform.position = new Vector3(-5.4f, -3f, 0);
        if (!facingRight) {
            Flip();
        }
        gm.hp = 2;
    }

    
    private void HandleInteractions() {
        if (raycastTarget) {
            if (Input.GetAxis("Interaction") != 0) {
                GameObject temp = raycastTarget;
                raycastTarget = null;
                Destroy(temp);
            }
        }
    }

    
    private void HandleAnimation() {
        if (onFloor) {
            animator.SetBool("isAirborne", false);
            if (rb.velocity.x != 0) {
                animator.SetBool("isMoving", true);
            }
            else {
                animator.SetBool("isMoving", false);
            }
        }
        else {
            
            animator.SetBool("isAirborne", true);
            if (rb.velocity.y > 0) {
                animator.SetBool("isAscending", true);
            }
            else {
                animator.SetBool("isAscending", false);
            }

            if (onWall) {
                animator.SetBool("isSliding", true);
            }
            else {
                animator.SetBool("isSliding", false);
                animator.ResetTrigger("Jump");
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            gm.hp -=1;
            if (gm.hp < 1) {
                animator.SetBool("isDamaged", true);
                Death();
            }
            else {
                Knockback();
            }
        }
    }

   /*private void OnDrawGizmos()
   {
       Gizmos.DrawSphere(groundCheck.position, radOCircle);
       Gizmos.DrawSphere(groundCheckII.position, radOCircle);
       Gizmos.DrawSphere(wallCheck.position, radOCircle);
   }*/
}
