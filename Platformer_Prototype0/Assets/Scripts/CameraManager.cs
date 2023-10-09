using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] public CinemachineVirtualCamera[] _allVirtualCameras;

    [Header("Controls for YLerp for jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallPanTime = 0.35f;
    public float _fallSpeedThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYpanCoroutine;

    private CinemachineVirtualCamera _currentCam;
    private CinemachineFramingTransposer _framingTransposer;
    private CinemachineConfiner2D camBinds;

    private float _normYPanAmount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i< _allVirtualCameras.Length; i++)
        {
            if (_allVirtualCameras[i].enabled)
            {
                //set current active cam
                _currentCam = _allVirtualCameras[i];
                //set framing transposer
                _framingTransposer = _currentCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        //set Ydamp amount
        _normYPanAmount = _framingTransposer.m_YDamping;
    }



    #region Lerp the Y damping

    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYpanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        //determin end amount
        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        //Lerp the pan amount
        float ellapsedTime = 0f;
        while(ellapsedTime < _fallPanTime)
        {
            ellapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (ellapsedTime / _fallPanTime));
            _framingTransposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }


    #endregion



}
