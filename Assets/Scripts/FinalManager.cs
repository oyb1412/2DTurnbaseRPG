using UnityEngine;

/// <summary>
/// 게임 종료 시 데이터 초기화
/// </summary>
public class FinalManager : MonoBehaviour
{
    [Header("--GameData--")]
    [SerializeField]private GameData[] gameDatas;

    private void OnApplicationQuit()
    {
        DataInit();
    }

    void DataInit()
    {
        for (int i = 0; i < 3; i++)
        {
            gameDatas[i].CurrentPlayerNumber = gameDatas[i + 3].CurrentPlayerNumber;

            gameDatas[i].CurrentGold = gameDatas[i + 3].CurrentGold;
            gameDatas[i].PlayerCurrentHp = gameDatas[i + 3].PlayerCurrentHp;
            gameDatas[i].PlayerMaxHp = gameDatas[i + 3].PlayerMaxHp;
            gameDatas[i].PlayerCurrentMp = gameDatas[i + 3].PlayerCurrentMp;
            gameDatas[i].PlayerMaxMp = gameDatas[i + 3].PlayerMaxMp;
            gameDatas[i].PlayerCurrendExp = gameDatas[i + 3].PlayerCurrendExp;
            gameDatas[i].PlayerMaxExp = gameDatas[i + 3].PlayerMaxExp;
            gameDatas[i].PlayerAttackDamage = gameDatas[i + 3].PlayerAttackDamage;
            gameDatas[i].PlayerFieldPosition = gameDatas[i + 3].PlayerFieldPosition;
            gameDatas[i].LevelUseGold = gameDatas[i + 3].LevelUseGold;
            gameDatas[i].PlusPlayerGold = gameDatas[i + 3].PlusPlayerGold;
            gameDatas[i].PlayerLevel = gameDatas[i + 3].PlayerLevel;
            gameDatas[i].SkillDamage = gameDatas[i + 3].SkillDamage;
            gameDatas[i].PlayerName = gameDatas[i + 3].PlayerName;

        }
    }
}
