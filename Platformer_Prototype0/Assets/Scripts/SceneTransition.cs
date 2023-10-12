using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transitionTo; //Represents the scene to transition to

    [SerializeField] private Transform startPoint; //Defines the player's entry point in the scene

    [SerializeField] private Vector2 exitDirection; //Specifies the direction for the player's exit

    [SerializeField] private float exitTime; //Determines the time it takes for the player to exit the scene transition



    // Start is called before the first frame update
    private void Start()
    {
        if (GameManager.Instance.transitionedFromScene == transitionTo)
        {
            PlayerController.Instance.transform.position = startPoint.position;

            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
            StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            Debug.Log("SCENCE TRANSITION PLAYER DETECTED");
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            PlayerController.Instance.pState.cutscene = true;

           // SceneManager.LoadScene(transitionTo);
            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
            
        }
    }
}