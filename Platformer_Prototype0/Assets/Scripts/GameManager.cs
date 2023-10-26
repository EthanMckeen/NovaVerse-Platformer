using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public string transitionedFromScene;

    public Vector2 respawnPoint;
    [SerializeField] public List<RespawnPoint> rPointList;
    [SerializeField] public RespawnPoint rPoint;
    public static GameManager Instance { get; private set; }
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
        //DontDestroyOnLoad(gameObject);
    }

    public void RespawnPlayer()
    {
        Debug.Log("Respawning");
        respawnPoint = rPoint.transform.position;
        PlayerController.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }

    public void setNewPoint(RespawnPoint _rPoint)
    {
        //Debug.Log(_rPoint.gameObject.name);
        if(rPoint != _rPoint)
        {
            if(rPointList.Count > 0)
            {
                for (int i = 0; i < rPointList.Count; i++)
                {
                    rPointList[i].resetColor();
                }
            }
            _rPoint.changeColor();
        }
        rPoint = _rPoint;
    }


    public void SceneChanged()
    {
        rPointList.Clear();
    }
}
