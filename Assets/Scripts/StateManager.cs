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
    public Image turnImage;
    public Text infoText;
    RectTransform rect;
    private void Awake()
    {
        instance = this;
        rect = turnImage.GetComponent<RectTransform>();
    }
    private void Start()
    {
        currentState = BattleState.PlayerTurn;
        StartCoroutine(TurnChange(currentState));
    }
    private void Update()
    {
        if(!GameManager.instance.isPlaying && currentState != BattleState.Win && currentState != BattleState.Lose)
            rect.localPosition += new Vector3(8f, 0f, 0f);


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
        infoText.gameObject.SetActive(false);
        turnImage.gameObject.SetActive(true);
        GameManager.instance.isPlaying = false;
        yield return new WaitForSeconds(2f);
        turnImage.gameObject.SetActive(false);
        infoText.gameObject.SetActive(true);
        rect.localPosition = new Vector3(-1500f, 0f, 0f);
        GameManager.instance.isPlaying = true;

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
