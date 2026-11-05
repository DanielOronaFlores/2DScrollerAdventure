using UnityEngine;

[RequireComponent(typeof(UnityEngine.Animator), typeof(PlayerController), typeof(PlayerInput))]
public class PlayerAnimator : MonoBehaviour
{
    private UnityEngine.Animator animator;
    private PlayerController controller;
    private PlayerInput input;

    private void Awake()
    {
        animator = GetComponent<UnityEngine.Animator>();
        controller = GetComponent<PlayerController>();
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        animator.SetFloat("HorizontalSpeed", Mathf.Abs(input.Horizontal));
        animator.SetFloat("VerticalSpeed", controller.CurrentVelocity.y);
        animator.SetBool("IsGrounded", controller.IsGrounded);
        
        // _animator.SetBool("IsCrawling", _controller.IsCrawling);
    }
}