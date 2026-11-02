using System.Runtime.CompilerServices;
using System.Security;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Attributes")]
    public float movementSpeed = 7f;
    public float jumpForce = 7f;
    public float crawlSpeed = 2f;

    [Header("GroundDetector")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    [SerializeField] private bool isGrounded;


    [Header("JumpDetector")]
    [SerializeField] private bool isJumping;


    [Header("Collider Morphology")]
    public Vector2 crouchSize = new Vector2(0.5f, 1f);
    public Vector2 crouchOffset = new Vector2(0f, -0.5f);
    public Vector2 crawlSize = new Vector2(1f, 0.5f);
    public Vector2 crawlOffset = new Vector2(0f, -0.75f);


    private Rigidbody2D rigidBody2D;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider2D;
    private Vector2 originalSize;
    private Vector2 originalOffset;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();

        originalSize = capsuleCollider2D.size;
        originalOffset = capsuleCollider2D.offset;
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        playerHorizontalMovement();
        playerVerticalMovement();
        
        runningAnimation();
        jumpAnimation();
        crawlAnimation();  
    }

    void FixedUpdate() 
    {
        float currentSpeed = (verticalInput < 0 && isGrounded) ? crawlSpeed : movementSpeed;
        rigidBody2D.linearVelocity = new Vector2(horizontalInput * currentSpeed, rigidBody2D.linearVelocity.y);
    }

    #region Movement
    private void playerHorizontalMovement()
    {
        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    private void playerVerticalMovement()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isJumping = !isGrounded;


        if (Input.GetButtonDown("Jump") && isGrounded && verticalInput >= 0)
        {
            isJumping = true;
            rigidBody2D.linearVelocity = new Vector2(rigidBody2D.linearVelocity.x, jumpForce);
        }
    }
    #endregion

    #region Animations
    private void crawlAnimation()
    {
        if (verticalInput < 0 && isGrounded)
        {
            if (horizontalInput == 0) 
            {
                // Estado: En cuclillas (Quieto)
                capsuleCollider2D.size = crouchSize;
                capsuleCollider2D.offset = crouchOffset;
            }
            else 
            {
                // Estado: A gatas (Moviéndose)
                capsuleCollider2D.size = crawlSize;
                capsuleCollider2D.offset = crawlOffset;
            }
        }
        else 
        {
            // Estado: De pie
            capsuleCollider2D.size = originalSize;
            capsuleCollider2D.offset = originalOffset;
        }
    }

    private void runningAnimation()
    {
        if (animator != null) animator.SetFloat("HorizontalSpeed", Mathf.Abs(horizontalInput));
    }

    private void jumpAnimation()
    {
        if (animator != null) 
        {
            animator.SetFloat("VerticalSpeed", rigidBody2D.linearVelocity.y);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }
    #endregion


    #region VisualHelpers
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); // Dibuja el círculo de colisión
        }
    }
    #endregion
}