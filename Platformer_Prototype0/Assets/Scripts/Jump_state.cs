using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump_state : StateMachineBehaviour
{
    Rigidbody2D rb;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DiveAttack();
    }

    void DiveAttack()
    {
        if (FinalBoss.Instance.diveAttack)
        {
            FinalBoss.Instance.Flip();

            Vector2 _newPos = Vector2.MoveTowards(rb.position, FinalBoss.Instance.moveToPosition,
                FinalBoss.Instance.speed * 3 * Time.fixedDeltaTime);
            rb.MovePosition(_newPos);

            float _distance = Vector2.Distance(rb.position, _newPos);
            if(_distance < 0.1f)
            {
                FinalBoss.Instance.Dive();
            }
        }
    }

    
}
