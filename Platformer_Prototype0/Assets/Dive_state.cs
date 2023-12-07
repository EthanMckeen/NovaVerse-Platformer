using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dive_state : StateMachineBehaviour
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
        FinalBoss.Instance.divingCollider.SetActive(true);

        if (FinalBoss.Instance.Grounded())
        {
            FinalBoss.Instance.divingCollider.SetActive(false);
            if (!callOnce)
            {
                callOnce = true;
                FinalBoss.Instance.DivingPillars();
                animator.SetBool("Dive", false);
                FinalBoss.Instance.ResetAllAttacks();               
            }

        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        callOnce = false;
    }
   

  
}
