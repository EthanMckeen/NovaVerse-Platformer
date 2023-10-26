using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockSkill : MonoBehaviour
{

    bool used;
    [SerializeField] private bool airJump;
    [SerializeField] private bool dashSkill;
    [SerializeField] private bool iceSkill;
    [SerializeField] private bool poisonSkill;
    [SerializeField] private bool fireSkill;

    // Start is called before the first frame update
    void Start()
    {
        
        if (airJump && PlayerController.Instance.unlockJumpSpell)
        {
            Destroy(gameObject);
        }
        else if (dashSkill && PlayerController.Instance.unlockDashSpell)
        {
            Destroy(gameObject);
        }
        else if (iceSkill && PlayerController.Instance.unlockIceSpell)
        {
            Destroy(gameObject);
        }
        else if (poisonSkill && PlayerController.Instance.unlockPoisonSpell)
        {
            Destroy(gameObject);
        }
        else if (fireSkill && PlayerController.Instance.unlockFireSpell)
        {
            Destroy(gameObject);
        }
        else
        {
            //idk
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !used)
        {
            used = true;

            if (airJump)
            {
                PlayerController.Instance.unlockJumpSpell = true;
            }
            else if (dashSkill)
            {
                PlayerController.Instance.unlockDashSpell = true;
            }
            else if (iceSkill)
            {
                PlayerController.Instance.unlockIceSpell = true;
            }
            else if (poisonSkill)
            {
                PlayerController.Instance.unlockPoisonSpell = true;
            }
            else if (fireSkill)
            {
                PlayerController.Instance.unlockFireSpell = true;
            }
            else
            {
                PlayerController.Instance.unlockJumpSpell = true;
                PlayerController.Instance.unlockDashSpell = true;
                PlayerController.Instance.unlockIceSpell = true;
                PlayerController.Instance.unlockPoisonSpell = true;
                PlayerController.Instance.unlockFireSpell = true;
            }
            AudioManager.Instance.PlayCharSFX(AudioManager.Instance.popPickUpSound);
            Destroy(gameObject);
        }
    }

}
