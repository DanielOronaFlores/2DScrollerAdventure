public class AirState : PlayerState
{
    public AirState(PlayerController playerController, PlayerInput playerInput, PlayerAnimator playerAnimator) 
    : base(playerController, playerInput, playerAnimator) 
    { 
    }

    public override void LogicUpdate()
    {
        if (playerController.IsGrounded)
        {
            playerController.StateMachine.ChangeState(playerController.GroundState);
        }
    }

    public override void PhysicsUpdate()
    {
        playerController.ApplyMovement();
    }
}