using UnityEngine;

/// <summary>
/// �� �� ���� ����
/// </summary>
public class StateManager : MonoBehaviour
{
    [Header("--Instance--")]
    public static StateManager instance;

    [Header("--StateInfo--")]
    public BattleState CurrentState;
    public enum BattleState { StartTurn, PlayerTurn, EnemyTurn, Win, Lose }

    private void Awake()
    {
        instance = this;
 
    }
    private void Start()
    {
        CurrentState = BattleState.StartTurn;
    }
}
