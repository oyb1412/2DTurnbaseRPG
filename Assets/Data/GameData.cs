using UnityEngine;

/// <summary>
/// ���� �� �÷��̾� �⺻ ������ ����
/// </summary>
[CreateAssetMenu(fileName = "Data", menuName = "Scriptble Object/Data")]
public class GameData : ScriptableObject
{
    //Ȱ��ȭ�� �÷��̾� ��
    public int CurrentPlayerNumber;

    //���������� ����� �÷��̾� ������
    public Vector2 PlayerFieldPosition;

    //�������� �ʿ��� ���
    public int LevelUseGold;

    //�÷��̾� Ȱ��ȭ�� �ʿ��� ���
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
