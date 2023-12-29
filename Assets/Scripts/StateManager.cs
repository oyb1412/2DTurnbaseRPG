using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    public static StateManager instance;
    public enum BattleState { PlayerTurn, EnemyTurn, Win, Lose }
    public BattleState currentState;
    public Text turnText;
    private void Awake()
    {
        instance = this;

    }
    private void Start()
    {
        currentState = BattleState.PlayerTurn;
        StartCoroutine(TurnChange(currentState));
    }
    IEnumerator TurnChange(BattleState staate)
    {
        switch(staate)
        {
            case BattleState.PlayerTurn:
                currentState = BattleState.PlayerTurn;
                PlayerTurn();
                turnText.text = "Player Turn";
                break;
                case BattleState.EnemyTurn:
                currentState = BattleState.EnemyTurn;
                EnemyTurn();
                turnText.text = "Enemy Turn";
                break;
            case BattleState.Win:
                currentState = BattleState.Win;
                BattleWin();
                turnText.text = "Win";
                break;
            case BattleState.Lose:
                currentState = BattleState.Lose;
                BattleLose();
                turnText.text = "Lose";
                break;
        }

        yield return new WaitForSeconds(2f);
        turnText.text = "";

    }
    void PlayerTurn()
    {
        //hud�� ���� ������ ǥ�� �� �ڷ�ƾ���� ����
        //�÷��̾��� �� �ľ�
        //�÷��̾� ���� ���� ����(���ڰ� ���� ��)
        //���� ���ݼ����� �÷��̾� ����
        //������ ��� ����
        //���� �� �������� �÷��̾� ����
        //������ ��� ����
        //�÷��̾��� �� ��ŭ ������ ������ �� ������ �ѱ�
        //currentState = BattleState.EnemyTurn;
        //EnemyTurn();

        //���������� ������� ���� �����ϸ� Win�� �ѱ�
        //BattleWin();

    }

    void EnemyTurn()
    {

        //���� �� �ľ�
        //�� ���� ���� ����(���ڰ� ���� ��)
        //���� ���ݼ����� �� ����
        //������ ��� ����(1���� : ü���� ���� �÷��̾�, 2���� : ���ڰ� ���� �÷��̾�)
        //���� �� �������� �� ����
        //������ ��� ����
        //���� �� ��ŭ ������ ������ �������� ������ �÷��̾� ������ �ѱ�

        //���������� ������� �÷��̾ �����ϸ� Lose�� �ѱ�
    }

    void BattleWin()
    {

        //���� ȹ��
    }

    void BattleLose()
    {

        //���� ����
    }

    public void ChangeTurn(BattleState stage)
    {
        StartCoroutine(TurnChange(stage));
    }
}
