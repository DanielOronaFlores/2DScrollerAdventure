using Unity.VisualScripting;

public abstract class PlayerState
{
    protected PlayerController playerController;
    protected PlayerInput playerInput;
    protected PlayerAnimator playerAnimator;

    public PlayerState(PlayerController playerController, PlayerInput playerInput, PlayerAnimator playerAnimator)
    {
        this.playerController = playerController;
        this.playerInput = playerInput;
        this.playerAnimator = playerAnimator;
    }

    public virtual void Enter() {}
    public virtual void LogicUpdate() {}
    public virtual void PhysicsUpdate() {}
    public virtual void Exit() {}
}