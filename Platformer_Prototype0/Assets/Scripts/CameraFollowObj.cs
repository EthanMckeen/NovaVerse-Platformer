using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObj : MonoBehaviour
{
    public static CameraFollowObj Instance { get; private set; }

    [Header("Refrences")]
    //[SerializeField]  private Transform _playerTransform;
    [Header("Flip rotation stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;
    private GameObject _player;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        _player = PlayerController.Instance.gameObject;
    }


    private void Update()
    {
        transform.position = PlayerController.Instance.transform.position;
        
    }

    public void CallCamTurn()
    {
        //Debug.Log(!PlayerController.Instance.pState.lookingRight);
        LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine(); 
    }

    private float DetermineEndRotation()
    {
        if (!PlayerController.Instance.pState.lookingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
