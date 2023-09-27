using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Dummy : Enemy
{
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y),
                speed * Time.deltaTime);
        }
    }

    public override void EnemyHit(float _dmgDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_dmgDone, _hitDirection, _hitForce);
    }
}
