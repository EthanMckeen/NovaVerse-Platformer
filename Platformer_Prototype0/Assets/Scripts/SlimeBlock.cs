using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBlock : MonoBehaviour
{


    public static SlimeBlock instance;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (PlayerController.Instance.pState.slimeKilled)
        {
            gameObject.SetActive(false);
        }
    }
}
