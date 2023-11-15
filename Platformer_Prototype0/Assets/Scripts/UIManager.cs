using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    public SceneFader sceneFader;
    [SerializeField] UnityEngine.UI.Image manaStorage;
    [SerializeField] GameObject deathScreen;
    public HeartController heartCtrl;
    private void Start()
    {
        sceneFader = GetComponentInChildren<SceneFader>();
        heartCtrl = GetComponent<HeartController>();
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

    public IEnumerator ActivateDeathScreen()
    {
        yield return new WaitForSeconds(0.8f);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));

        yield return new WaitForSeconds(0.8f);
        deathScreen.SetActive(true);
    }
    
    public IEnumerator DeactivateDeathScreen()
    {
        yield return new WaitForSeconds(0.5f);
        deathScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

}
