using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce2_state : StateMachineBehaviour
{
    Rigidbody2D rb;
    bool callOnce;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 _forceDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * FinalBoss.Instance.rotationDirectionToTarget),
            Mathf.Sin(Mathf.Deg2Rad * FinalBoss.Instance.rotationDirectionToTarget));

        rb.AddForce(_forceDirection * 3, ForceMode2D.Impulse);

        FinalBoss.Instance.bounceCollider.SetActive(true);

        if (FinalBoss.Instance.Grounded())
        {
            FinalBoss.Instance.bounceCollider.SetActive(false);
            if (!callOnce)
            {
                FinalBoss.Instance.ResetAllAttacks();
                callOnce = true;
            }
            animator.SetTrigger("Grounded");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Bounce2");
        animator.ResetTrigger("Grounded");
        callOnce = false;
    }
}
