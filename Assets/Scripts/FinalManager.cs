using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalManager : MonoBehaviour
{
    [Header("--GameData--")]
    public GameData[] gameDatas;
    // Start is called before the first frame update
    private void Awake()
    {
        DataInit();

    }

    private void OnApplicationQuit()
    {
    }

    void DataInit()
    {
        for (int i = 0; i < 3; i++)
        {
            gameDatas[i].currentPlayerNumber = gameDatas[i + 3].currentPlayerNumber;

            gameDatas[i].currentGold = gameDatas[i + 3].currentGold;
            gameDatas[i].playerCurrentHp = gameDatas[i + 3].playerCurrentHp;
            gameDatas[i].playerMaxHp = gameDatas[i + 3].playerMaxHp;
            gameDatas[i].playerCurrenMp = gameDatas[i + 3].playerCurrenMp;
            gameDatas[i].playerMaxMp = gameDatas[i + 3].playerMaxMp;
            gameDatas[i].PlayerCurrendExp = gameDatas[i + 3].PlayerCurrendExp;
            gameDatas[i].PlayerMaxExp = gameDatas[i + 3].PlayerMaxExp;
            gameDatas[i].playerAttackDamage = gameDatas[i + 3].playerAttackDamage;
            gameDatas[i].playerFieldPosition = gameDatas[i + 3].playerFieldPosition;
            gameDatas[i].levelUseGold = gameDatas[i + 3].levelUseGold;
            gameDatas[i].plusPlayerGold = gameDatas[i + 3].plusPlayerGold;
            gameDatas[i].playerLevel = gameDatas[i + 3].playerLevel;
            gameDatas[i].skillDamage = gameDatas[i + 3].skillDamage;
            gameDatas[i].PlayerName = gameDatas[i + 3].PlayerName;

        }
    }
}
