using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ExitButton()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }

    public void StartGame()
    {
        //SceneManager.LoadScene("Game_Hub");
        Debug.Log("SCENCE TRANSITION PLAYER DETECTED");
        GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

        PlayerController.Instance.pState.cutscene = true;

        StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, "Game_Hub"));
    }
}
