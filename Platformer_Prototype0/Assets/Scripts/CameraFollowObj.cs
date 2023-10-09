using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObj : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] private Transform _playerTransform;
    [Header("Flip rotation stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;
    private PlayerController _player;
    private PlayerStateList _pState;

    //private bool _isFacingRight;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<PlayerController>();
        _pState = _playerTransform.gameObject.GetComponent<PlayerStateList>();
        //_isFacingRight = _player.pState.lookingRight;
    }


    private void Update()
    {
        transform.position = _playerTransform.position;
        
    }

    public void CallCamTurn()
    {
        LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine();
    }

    private float DetermineEndRotation()
    {
        if (!_player.pState.lookingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
