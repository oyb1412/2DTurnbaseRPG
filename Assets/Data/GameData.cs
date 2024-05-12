using UnityEngine;

/// <summary>
/// 게임 및 플레이어 기본 데이터 관리
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "Scriptble Object/Data")]
public class GameData : ScriptableObject
{
    //활성화된 플레이어 수
    public int CurrentPlayerNumber;

    //마지막으로 저장된 플레이어 포지션
    public Vector2 PlayerFieldPosition;

    //레벨업에 필요한 골드
    public int LevelUseGold;

    //플레이어 활성화에 필요한 골드
    public int PlusPlayerGold;

    public int CurrentGold;
    public float PlayerCurrentHp;
    public float PlayerMaxHp;
    public float PlayerCurrentMp;
    public float PlayerMaxMp;
    public float PlayerAttackDamage;
    public string PlayerName;
    public float PlayerCurrendExp;
    public float PlayerMaxExp;
    public int PlayerLevel;
    public float SkillDamage;
}
