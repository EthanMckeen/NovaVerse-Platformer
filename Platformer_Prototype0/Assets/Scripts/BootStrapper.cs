using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BootStrapper 
{
   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]


   public static void Execute()
    {
        Debug.Log("Loaded by the Persistent Obj from Bootstrappper script");
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Persistent")));
    }
}
