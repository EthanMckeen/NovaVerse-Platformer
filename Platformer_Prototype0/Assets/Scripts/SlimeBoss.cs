using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : Enemy
{

    [SerializeField] private GameObject blockade;


    protected override void Start()
    {
        if (PlayerController.Instance.pState.slimeKilled)
        {
            //if player has killed dont respawn
            gameObject.SetActive(false);
            blockade.SetActive(false);
        }
        base.Start();
        rb.gravityScale = 12f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (this.health <= 0)
        {
            //if its dead break blockade to progress
            Destroy(blockade);
            //add the playercheck 
            PlayerController.Instance.pState.slimeKilled = true;
        }
        base.Update();
        if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
    }
}
