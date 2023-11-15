using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicOrbPickup : MonoBehaviour
{
    void Start()
    {
        if (PlayerController.Instance.manaOrbs >= 3)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            PlayerController.Instance.manaOrbs++;
            AudioManager.Instance.PlayCharSFX(AudioManager.Instance.popPickUpSound);
            Destroy(gameObject);
        }
    }
}
