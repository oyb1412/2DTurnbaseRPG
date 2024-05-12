using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Village 씬에서의 버튼 관리
/// </summary>
public class ButtonManager : MonoBehaviour
{
    [Header("--GameData--")]
    [SerializeField]private GameData[] gameDatas;

    [Header("--Button--")]
    //각 아이템 구매 버튼
    [SerializeField] private Button[] itemButton;

    [Header("--Text--")]
    [SerializeField] private Text currentGoldText;
    //아이템 구매에 필요한 골드
    private Text[] needGoldTexts;
    private void Awake()
    {
        needGoldTexts = new Text[itemButton.Length]; 
        for (int i = 0;i<itemButton.Length;i++)
        {
            needGoldTexts[i] = itemButton[i].GetComponentsInChildren<Text>()[1];
        }
        needGoldTexts[0].text = string.Format("need {0} gold", 100);
    }

    private void Update() {
        ButtonText();
        ButtonInteractable();
    }

    #region CallbackMethod
    /// <summary>
    /// HP회복 아이템(콜백으로 호출)
    /// </summary>
    public void MaxHpClick()
    {
        AudioManager.instance.PlayerSfx(AudioManager.Sfx.UseGold);

        for (int i = 0; i < gameDatas.Length; i++)
        {
            gameDatas[i].PlayerCurrentHp = gameDatas[i].PlayerMaxHp;
            gameDatas[i].PlayerCurrentMp = gameDatas[i].PlayerMaxMp;

        }
        gameDatas[0].CurrentGold -= 100;
    }



    /// <summary>
    /// 레벨업 아이템 구매(콜백으로 호출)
    /// </summary>
    public void LevelUpClick()
    {
        AudioManager.instance.PlayerSfx(AudioManager.Sfx.UseGold);

        for (int i = 0; i < gameDatas.Length; i++)
        {
            if (gameDatas[0].CurrentPlayerNumber > i)
            {
                if (gameDatas[i].PlayerCurrentHp > 0)
                {
                    gameDatas[i].PlayerLevel++;
                    gameDatas[i].PlayerAttackDamage += 2;
                    gameDatas[i].PlayerMaxHp += 10;
                    gameDatas[i].PlayerMaxMp += 5;
                    gameDatas[i].SkillDamage += 3;
                }
            }
        }
        gameDatas[0].CurrentGold -= gameDatas[0].LevelUseGold;
    }

    /// <summary>
    /// 플레이어 유닛 추가 아이템 구매(콜백으로 호출)
    /// </summary>
    public void PlusPlayerClick()
    {
        AudioManager.instance.PlayerSfx(AudioManager.Sfx.UseGold);

        gameDatas[0].CurrentPlayerNumber++;
        gameDatas[0].CurrentGold -= gameDatas[0].PlusPlayerGold;

    }

    /// <summary>
    /// 마을 나가기(콜백으로 호출)
    /// </summary>
    public void ReturnField()
    {
         AudioManager.instance.PlayerSfx(AudioManager.Sfx.GoScene);
        VillageGameManager.instance.ActionFade((int)VillageGameManager.Scenes.FieldScene);
        AudioManager.instance.PlayerBgm(AudioManager.Bgm.Village, false);
    }
    #endregion

    /// <summary>
    /// 각 아이템 구매에 필요한 텍스트 출력
    /// </summary>
    private void ButtonText()
    {
        needGoldTexts[1].text = string.Format("need {0} gold", gameDatas[0].LevelUseGold);
        needGoldTexts[2].text = string.Format("need {0} gold", gameDatas[0].PlusPlayerGold);
        currentGoldText.text = gameDatas[0].CurrentGold.ToString();
    }

    /// <summary>
    /// 골드 부족시 버튼 비활성화
    /// </summary>
    private void ButtonInteractable()
    {
        if (gameDatas[0].PlayerCurrentHp == gameDatas[0].PlayerMaxHp &&
            gameDatas[1].PlayerCurrentHp == gameDatas[1].PlayerMaxHp &&
            gameDatas[2].PlayerCurrentHp == gameDatas[2].PlayerMaxHp ||
            gameDatas[0].CurrentGold < 100)
        {
            itemButton[0].interactable = false;
        }

        if (gameDatas[0].CurrentGold < gameDatas[0].LevelUseGold)
        {
            itemButton[1].interactable = false;
        }

        if (gameDatas[0].CurrentGold < gameDatas[0].PlusPlayerGold || gameDatas[0].CurrentPlayerNumber >= 3)
        {
            itemButton[2].interactable = false;
        }
    }
}
