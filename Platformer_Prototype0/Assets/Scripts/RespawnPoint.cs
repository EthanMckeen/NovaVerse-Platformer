using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField] private bool natural;
    public bool interacted;
    SpriteRenderer sr;
    Color defCol;
    bool canInteract = false;
    private void Start()
    {
        GameManager.Instance.rPointList.Add(gameObject.GetComponent<RespawnPoint>());
        if (GetComponent<SpriteRenderer>() && !natural)
        {
            sr = GetComponent<SpriteRenderer>();
            defCol = sr.color;
        }
        
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && canInteract)
        {
            GameManager.Instance.setNewPoint(gameObject.GetComponent<RespawnPoint>());
            
        }
        else if (natural && canInteract)
        {
            GameManager.Instance.setNewPoint(gameObject.GetComponent<RespawnPoint>());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = true;
        }
        else
        {
            canInteract = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
        }
    }

    public void resetColor()
    {
        if (sr != null)
        {
            sr.color = defCol;
        }
    }

    public void changeColor()
    {
        if (sr != null)
        {
            sr.color = Color.green;
        }
    }
}
