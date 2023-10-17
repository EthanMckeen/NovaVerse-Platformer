using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CutsceneManager : MonoBehaviour
{

    public static CutsceneManager instance;


    [SerializeField] private GameObject Boss;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private string cinamatic;
    [SerializeField] private CinemachineVirtualCamera cutsceneCAM;
    public bool isCutscenePlaying = false;
    private Coroutine _startCutscene;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Debug.Log("Play the Cutscene");
            _startCutscene = StartCoroutine(StartCutscene());
            gameObject.GetComponent<Collider2D>().enabled = false;
        }

    }



    private IEnumerator StartCutscene()
    {
        //which cutscene
        switch (cinamatic)
        {
            case "slimeboss":
                if(PlayerController.Instance.pState.slimeKilled == false)
                {
                    isCutscenePlaying = true;
                    PlayerController.Instance.pState.cutscene = true;
                    Instantiate(Boss, spawnPoint.position, Quaternion.identity);
                    SlimeBoss.Instance.cutscene = true;
                    //swap to cutscene cam
                    UIManager.Instance.GetComponent<Canvas>().enabled = false;
                    yield return new WaitForSeconds(1.2f);
                    yield return StartCoroutine(CameraManager.instance.CutsceneCoroutine(cutsceneCAM, 3.5f, 2f));
                    Debug.Log("Returned");
                    PlayerController.Instance.pState.cutscene = false;
                    UIManager.Instance.GetComponent<Canvas>().enabled = true;
                    SlimeBoss.Instance.cutscene = false;
                    Debug.Log("CutSceneIsFinished");
                }
                break;
        }



        yield return null;
    }
}
