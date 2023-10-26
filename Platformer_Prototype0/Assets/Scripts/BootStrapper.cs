using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BootStrapper 
{
   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]


   public static void Execute()
    {
        if(SceneManager.GetActiveScene().name != "MainMenu")
        {
            Debug.Log("Loaded by the Persistent Obj from Bootstrappper script");
            Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Persistent")));
        }
        else if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            Debug.Log("Loaded by the Persistent Obj from Bootstrappper script");
            Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Persistent")));
            PlayerController.Instance.pState.cutscene = true;
        }
    }
}
