using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ButtonManager : MonoBehaviour
{
    [Header("--GameData--")]
    public GameData[] gameDatas;

    [Header("--Button--")]
    public Button[] itemButton;

    [Header("--Text--")]
    public Text currentGoldText;
    Text[] needGoldTexts;
    private void Awake()
    {
        needGoldTexts = new Text[itemButton.Length]; 
        for (int i = 0;i<itemButton.Length;i++)
        {
            needGoldTexts[i] = itemButton[i].GetComponentsInChildren<Text>()[1];
        }
        needGoldTexts[0].text = string.Format("{0}°ñµå ÇÊ¿ä", 100);
    }
    public void MaxHpClick()
    {
        for (int i = 0; i < gameDatas.Length; i++)
        {
            gameDatas[i].playerCurrentHp = gameDatas[i].playerMaxHp;
            gameDatas[i].playerCurrenMp = gameDatas[i].playerMaxMp;

        }
        gameDatas[0].currentGold -= 100;
    }

    private void Update()
    {
        ButtonText();
        ButtonInteractable();
    }
    public void LevelUpClick()
    {
        for (int i = 0; i < gameDatas.Length; i++)
        {
            if (gameDatas[0].currentPlayerNumber > i)
            {
                if (gameDatas[i].playerCurrentHp > 0)
                {
                    gameDatas[i].playerLevel++;
                    gameDatas[i].playerAttackDamage += 2;
                    gameDatas[i].playerMaxHp += 10;
                    gameDatas[i].playerMaxMp += 5;
                    gameDatas[i].skillDamage += 3;
                }
            }
        }
        gameDatas[0].currentGold -= gameDatas[0].levelUseGold;
    }

    public void PlusPlayerClick()
    {
        gameDatas[0].currentPlayerNumber++;
        gameDatas[0].currentGold -= gameDatas[0].plusPlayerGold;

    }

    public void ReturnField()
    {
        VillageGameManager.instance.ActionFade((int)VillageGameManager.Scenes.FieldScene);
        AudioManager.instance.PlayerBgm(AudioManager.Bgm.Village, false);
    }



    void ButtonText()
    {
        needGoldTexts[1].text = string.Format("{0}°ñµå ÇÊ¿ä", gameDatas[0].levelUseGold);
        needGoldTexts[2].text = string.Format("{0}°ñµå ÇÊ¿ä", gameDatas[0].plusPlayerGold);
        currentGoldText.text = gameDatas[0].currentGold.ToString();
    }

    void ButtonInteractable()
    {
        if (gameDatas[0].playerCurrentHp == gameDatas[0].playerMaxHp &&
            gameDatas[1].playerCurrentHp == gameDatas[1].playerMaxHp &&
            gameDatas[2].playerCurrentHp == gameDatas[2].playerMaxHp ||
            gameDatas[0].currentGold < 100)
        {
            itemButton[0].interactable = false;
        }


        if (gameDatas[0].currentGold < gameDatas[0].levelUseGold)
        {
            itemButton[1].interactable = false;
        }

        if (gameDatas[0].currentGold < gameDatas[0].plusPlayerGold || gameDatas[0].currentPlayerNumber >= 3)
        {
            itemButton[2].interactable = false;
        }
    }
}
