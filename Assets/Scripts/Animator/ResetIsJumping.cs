using UnityEngine;

public class ResetIsJumping : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            animator.transform.GetComponent<HorseController>().HorseLocomotion.IsJumping = false;
    }
}