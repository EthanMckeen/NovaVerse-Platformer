using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lunge_state : StateMachineBehaviour
{
    Rigidbody2D rb;
    bool striked = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.gravityScale = 0;
        int _dir = FinalBoss.Instance.facingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * (FinalBoss.Instance.speed * 3), 0f);

        if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= FinalBoss.Instance.attackRange)
        {
            Hit(FinalBoss.Instance.SideAtkTransform, FinalBoss.Instance.SideAtkArea);
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] _objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0);
        for (int i = 0; i < _objectsToHit.Length; i++)
        {
            if (_objectsToHit[i].GetComponent<PlayerController>() != null && !striked)
            {
                striked = true;
                _objectsToHit[i].GetComponent<PlayerController>().TakeDamage(FinalBoss.Instance.damage);
            }
        }
    }


}
