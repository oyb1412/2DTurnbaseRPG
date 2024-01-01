using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameData[] gameDatas;
    public Text[] goldTexts;
    public Button[] itemButton;
    int useGold;
    private void Awake()
    {
        goldTexts[0].text = string.Format("{0}°ñµå ÇÊ¿ä", 100);
    }
    public void MaxHpClick()
    {
        for (int i = 0; i < gameDatas.Length; i++)
        {
            gameDatas[i].playerCurrentHp = gameDatas[i].playerMaxHp;
        }
        gameDatas[0].currentGold -= 100;
    }

    public void LevelUpClick()
    {
        for (int i = 0; i < gameDatas.Length; i++)
        {
            gameDatas[i].playerLevel++;
            gameDatas[i].playerAttackDamage += 2;
            gameDatas[i].playerMaxHp += 10;
            gameDatas[i].skillDamage += 1;
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

    private void Update()
    {
        goldTexts[1].text = string.Format("{0}°ñµå ÇÊ¿ä", gameDatas[0].levelUseGold);
        goldTexts[2].text = string.Format("{0}°ñµå ÇÊ¿ä", gameDatas[0].plusPlayerGold);
        goldTexts[3].text = gameDatas[0].currentGold.ToString();

        if (gameDatas[0].playerCurrentHp == gameDatas[0].playerMaxHp &&
            gameDatas[1].playerCurrentHp == gameDatas[1].playerMaxHp &&
            gameDatas[2].playerCurrentHp == gameDatas[2].playerMaxHp)
        {
            itemButton[0].interactable = false;
        }

        if (gameDatas[0].currentGold < 100)
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
