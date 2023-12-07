using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaOrbHandler : MonoBehaviour
{

    public bool usedMana;
    public List<GameObject> manaOrbs;
    public List<Image> orbFills;

    public float countDown = 3f;
    float totalManaPool;
    // Start is called before the first frame update
    void Start()
    {
        for( int i = 0; i < PlayerController.Instance.manaOrbs; i++)
        {
            manaOrbs[i].SetActive(true);
        }
        for (int i = 0; i < 3; i++)
        {
            if (manaOrbs[0].activeSelf)
                orbFills[i].fillAmount = 1;
            else
                orbFills[i].fillAmount = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < PlayerController.Instance.manaOrbs; i++)
        {
            manaOrbs[i].SetActive(true);
        }
        if (manaOrbs[0].activeSelf)
        {
            CashInMana();
        }
    }

    public void UpdateMana(float _manaGainFrom)
    {
        for (int i = 0; i < PlayerController.Instance.manaOrbs; i++)
        {
            if(manaOrbs[i].activeInHierarchy && orbFills[i].fillAmount < 1)
            {
                orbFills[i].fillAmount += _manaGainFrom;
                break;
            }
        }
    }

    void CashInMana()
    {
        if(usedMana && PlayerController.Instance.Mana <= 1)
        {
            countDown -= Time.deltaTime;
            //Debug.Log(countDown);
        }

        if(countDown <= 0)
        {
            usedMana = false;
            countDown = 3;

            totalManaPool = (orbFills[0].fillAmount += orbFills[1].fillAmount += orbFills[2].fillAmount) *.33f;
            //Debug.Log(totalManaPool);
            float manaNeeded = 1 - PlayerController.Instance.Mana;

            if (manaNeeded > 0)
            {
             
                    PlayerController.Instance.Mana += manaNeeded;
                    for (int i = 0; i < orbFills.Count; i++)
                    {
                        orbFills[i].fillAmount = 0;
                    }

                    float addBackTotal = (totalManaPool - manaNeeded) / 0.33f;
                    while (addBackTotal > 0)
                    {
                        UpdateMana(addBackTotal);
                        addBackTotal -= 1;
                    }
                
            }
            else
            {
                PlayerController.Instance.Mana += totalManaPool;
                for (int i = 0; i < orbFills.Count; i++)
                {
                    orbFills[i].fillAmount = 0;
                }
            }
        }
    }
}
