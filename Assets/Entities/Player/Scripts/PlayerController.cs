using UnityEngine;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(PlayerAnimator), typeof(GroundDetector))]
public class PlayerController : MonoBehaviour
{
    #region Configurations
    [Header("Debug")]
    [SerializeField] private string currentStateName;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float crawlSpeed = 2f;
    [SerializeField] private float jumpForce = 7f;

    [Header("Collider Morphology")]
    [SerializeField] private Vector2 crouchSize = new Vector2(0.5f, 1f);
    [SerializeField] private Vector2 crouchOffset = new Vector2(0f, -0.5f);
    [SerializeField] private Vector2 crawlSize = new Vector2(1f, 0.5f);
    [SerializeField] private Vector2 crawlOffset = new Vector2(0f, -0.75f);
    
    private Vector2 originalSize;
    private Vector2 originalOffset;
    private float jumpCooldownTimer = 0f;
    #endregion

    #region Public Propierties
    public bool IsGrounded => groundDetector.IsGrounded;
    public bool IsCrawling { get; private set; }
    public Vector2 CurrentVelocity => rigidBody.linearVelocity;
    #endregion

    #region State Machine
    public PlayerStateMachine StateMachine { get; private set; }
    public GroundState GroundState { get; private set; }
    public AirState AirState { get; private set; }
    #endregion

    #region Components
    private PlayerAnimator playerAnimator;    
    private Rigidbody2D rigidBody;
    private CapsuleCollider2D capsuleCollider;
    private PlayerInput playerInput;
    private GroundDetector groundDetector;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<PlayerAnimator>();
        groundDetector = GetComponent<GroundDetector>();

        originalSize = capsuleCollider.size;
        originalOffset = capsuleCollider.offset;

        StateMachine = new PlayerStateMachine();
        GroundState = new GroundState(this, playerInput, playerAnimator);
        AirState = new AirState(this, playerInput, playerAnimator);
    }

    public void Start()
    {
        StateMachine.Initialize(GroundState);
    }

    private void Update()
    {
        HandleJumpCooldown();
        HandleFacingDirection();
        
        StateMachine.CurrentState.LogicUpdate();
        currentStateName = StateMachine.CurrentState.GetType().Name;
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdate();
    }
    #endregion

    #region Action Methods
    private void HandleJumpCooldown()
    {
        if (IsGrounded && jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
        else if (!IsGrounded)
        {
            jumpCooldownTimer = 0.1f;
        }
    }

    public void ExecuteJump()
    {
        if (jumpCooldownTimer <= 0)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);
        }
    }

    public void HandleStance()
    {
        IsCrawling = IsGrounded && playerInput.Vertical < 0;

        if (IsCrawling)
        {
            capsuleCollider.size = playerInput.Horizontal == 0 ? crouchSize : crawlSize;
            capsuleCollider.offset = playerInput.Horizontal == 0 ? crouchOffset : crawlOffset;
        }
        else
        {
            capsuleCollider.size = originalSize;
            capsuleCollider.offset = originalOffset;
        }
    }

    public void HandleFacingDirection()
    {
        if (playerInput.Horizontal > 0)
            transform.localScale = Vector3.one;
        else if (playerInput.Horizontal < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void ApplyMovement()
    {
        float currentSpeed = IsCrawling ? crawlSpeed : movementSpeed;
        rigidBody.linearVelocity = new Vector2(playerInput.Horizontal * currentSpeed, rigidBody.linearVelocity.y);
    }
    #endregion
}