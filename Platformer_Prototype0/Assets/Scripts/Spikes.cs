using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    float damage = 100f;

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            Debug.Log("SCENCE TRANSITION PLAYER DETECTED");
            Attack();
        }
    }

    public virtual void Attack()
    {

        PlayerController.Instance.TakeDamage(damage);

    }
}
