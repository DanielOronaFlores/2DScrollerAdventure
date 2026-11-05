public class GroundState : PlayerState
{
    public GroundState(PlayerController playerController, PlayerInput playerInput, PlayerAnimator playerAnimator) 
    : base(playerController, playerInput, playerAnimator) 
    { 
    }

    public override void Enter(){}

    public override void LogicUpdate()
    {
        playerController.HandleStance();

        if (playerInput.JumpPressed && playerInput.Vertical >= 0) //Transitiction to Jump!
        {
            playerController.ExecuteJump();
            playerController.StateMachine.ChangeState(playerController.AirState);
        }
        else if (!playerController.IsGrounded) //Falling from Jump
        {
            playerController.StateMachine.ChangeState(playerController.AirState);
        }
    }

    public override void PhysicsUpdate()
    {
        playerController.ApplyMovement();
    }
}