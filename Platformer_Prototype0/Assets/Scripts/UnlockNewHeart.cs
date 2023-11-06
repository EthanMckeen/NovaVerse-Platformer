using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockNewHeart : MonoBehaviour
{
    void Start()
    {
        if(PlayerController.Instance.maxHealth == PlayerController.Instance.maxTotalHealth)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.Instance.maxHealth++;
            PlayerController.Instance.Health++;
            PlayerController.Instance.onHealthChangedCallback();
            UIManager.Instance.heartCtrl.UpdateHeartsHUD();
            AudioManager.Instance.PlayCharSFX(AudioManager.Instance.popPickUpSound);
            Destroy(gameObject);
        }
    }
}
