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
    private Coroutine _panCameraCoroutine;
    private Coroutine _cutsceneCameraCoroutine;

    private CinemachineVirtualCamera _currentCam;
    private CinemachineFramingTransposer _framingTransposer;
    private CinemachineConfiner2D camBinds;

    private float _normYPanAmount;

    private Vector2 _startingTrackdObjectOffset;


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
        //set the starting position of the tracked object offset
        _startingTrackdObjectOffset = _framingTransposer.m_TrackedObjectOffset;
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

    #region Pan Camera

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        //handle pan from trigger
        if (!panToStartingPos)
        {
            //set dir and dist
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.right;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.left;
                    break;
                default:
                    break;
            }

            endPos *= panDistance;

            startingPos = _startingTrackdObjectOffset;

            endPos += startingPos;
        }
        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackdObjectOffset;
        }

        float elapsedTime = 0f;
        while(elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));
            _framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }
    #endregion

    #region Swap Camera
    public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        //if curCam is on the left and exit was on the right
        if (_currentCam == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            //activate new camera
            cameraFromRight.enabled = true;
            //deactivate old camera
            cameraFromLeft.enabled = false;

            //set new curCam and transp
            _currentCam = cameraFromRight;

            _framingTransposer = _currentCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        //if curCam is on the right and exit was on the left
        else if (_currentCam == cameraFromRight && triggerExitDirection.x < 0f)
        {
            //activate new camera
            cameraFromLeft.enabled = true;
            //deactivate old camera
            cameraFromRight.enabled = false;

            //set new curCam and transp
            _currentCam = cameraFromRight;

            _framingTransposer = _currentCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }


    #endregion

    #region CutsceneRoll
   /* public void RollCutscene(CinemachineVirtualCamera cutsceneCamera, float cutsceneEnterTime, float cutsceneExitTime)
    {
        _cutsceneCameraCoroutine = StartCoroutine(CutsceneCoroutine(cutsceneCamera, cutsceneEnterTime, cutsceneExitTime));

    }*/


    public IEnumerator CutsceneCoroutine(CinemachineVirtualCamera cutsceneCamera, float cutsceneEnterTime, float cutsceneExitTime)
    {
        CinemachineVirtualCamera startCamera = _currentCam;
        cutsceneCamera.enabled = true;
        startCamera.enabled = false;
        _currentCam = cutsceneCamera;
        _framingTransposer = _currentCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        Debug.Log("EnteringCutscene");
        yield return new WaitForSeconds(cutsceneEnterTime);
        startCamera.enabled = true;
        cutsceneCamera.enabled = false;
        _currentCam = cutsceneCamera;
        _framingTransposer = _currentCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        Debug.Log("ExitingCutscene");
        yield return new WaitForSeconds(cutsceneExitTime);
        CutsceneManager.instance.isCutscenePlaying = false;
    }

    #endregion
}
