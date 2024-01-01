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
        //hud에 누구 턴인지 표시 후 코루틴으로 제거
        //플레이어의 수 파악
        //플레이어 공격 순서 지정(숫자가 낮은 순)
        //현재 공격순번인 플레이어 선택
        //공격할 대상 선택
        //공격 후 다음순번 플레이어 선택
        //공격할 대상 선택
        //플레이어의 수 만큼 공격이 끝나면 적 턴으로 넘김
        //currentState = BattleState.EnemyTurn;
        //EnemyTurn();

        //공격유무에 상관없이 적이 전멸하면 Win로 넘김
        //BattleWin();

    }

    void EnemyTurn()
    {

        //적의 수 파악
        //적 공격 순서 지정(숫자가 낮은 순)
        //현재 공격순번인 적 선택
        //공격할 대상 선택(1순위 : 체력이 적은 플레이어, 2순위 : 숫자가 낮은 플레이어)
        //공격 후 다음순번 적 선택
        //공격할 대상 선택
        //적의 수 만큼 공격이 끝나고 전멸하지 않으면 플레이어 턴으로 넘김

        //공격유무에 상관없이 플레이어가 전멸하면 Lose로 넘김
    }

    void BattleWin()
    {

        //보상 획득
    }

    void BattleLose()
    {

        //게임 종료
    }

    public void ChangeTurn(BattleState stage)
    {
        StartCoroutine(TurnChange(stage));
    }
}
