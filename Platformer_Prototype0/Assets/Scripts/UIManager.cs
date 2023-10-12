using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    public SceneFader sceneFader;
    [SerializeField] UnityEngine.UI.Image manaStorage;
    private void Start()
    {
        sceneFader = GetComponentInChildren<SceneFader>();
    }
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

    public void UpdateManaBall(float Mana)
    {
        manaStorage.fillAmount = Mana;
    }

}
