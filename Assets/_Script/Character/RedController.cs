using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedController : MonoBehaviour
{
    [Header("Public Variables")]
    public float speed = 10.0f;
    public float jumpforce = 325;
    public float cooldownTime = 4;
    public GameObject Fireball;

    private Rigidbody2D rb;
    //Vamos salvar o objeto que atingimos com o raycast aqui
    //private GameObject raycastTarget;
    private Vector3 movingDirection;
    private Animator animator;

    private GameObject player;

    [Header("Private Variables")]
    [SerializeField]private Transform groundCheck;
    [SerializeField]private Transform groundCheckII;
    [SerializeField]private Transform wallCheck;
    [SerializeField]private float radOCircle;
    [SerializeField]private LayerMask whatIsGround;

    private bool facingRight = false;
    private bool onFloor = false;
    private bool cooldown = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        //raycastTarget = null;
        groundCheck = gameObject.transform.Find("groundcheck");
        groundCheckII = gameObject.transform.Find("groundcheckII");
        wallCheck = gameObject.transform.Find("wallcheck");
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        //HandleInteractions();
        HandleAnimation();
        if (!cooldown) {
            Instantiate(Fireball, wallCheck.position, Quaternion.identity, transform);
            cooldown = true;
            Invoke("EndCooldown",cooldownTime);
        }
    }

    private void HandleMovement() {
        onFloor = (Physics2D.OverlapCircle(groundCheck.position,radOCircle,whatIsGround) || Physics2D.OverlapCircle(groundCheckII.position,radOCircle,whatIsGround));
        //Todo

        //rb.MovePosition(transform.position + movingDirection * speed * Time.deltaTime);

        /*if ((transform.position.x > player.transform.position.x && facingRight) || (transform.position.x < player.transform.position.x && !facingRight))
        {
            Flip();
        }*/
        
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

    private void EndCooldown() {
        if (cooldown) {
            cooldown = false;
        }
    }

    private void HandleAnimation() {
        if (onFloor) {
            animator.SetBool("isAirborne", false);
        }
        else {
            animator.SetBool("isAirborne", true);
            if (rb.velocity.y > 0) {
                animator.SetBool("isAscending", true);
            }
            else {
                animator.SetBool("isAscending", false);
            }
        }
    }

    /*private void OnDrawGizmos()
    {
       Gizmos.DrawSphere(groundCheck.position, radOCircle);
       Gizmos.DrawSphere(groundCheckII.position, radOCircle);
       //Gizmos.DrawSphere(wallCheck.position, radOCircle);
    }*/
}
