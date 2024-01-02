using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StateManager : MonoBehaviour
{
    [Header("--Instance--")]
    public static StateManager instance;

    [Header("--StateInfo--")]
    public BattleState currentState;
    public enum BattleState { PlayerTurn, EnemyTurn, Win, Lose }

    [Header("--UI--")]
    Text turnText;
    public Image turnImage;
    public Text infoText;
    RectTransform rect;
    private void Awake()
    {
        instance = this;
        rect = turnImage.GetComponent<RectTransform>();
        turnText = turnImage.GetComponentInChildren<Text>();
        turnImage.gameObject.SetActive(false);
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
                turnText.text = "Player Turn";
                break;
                case BattleState.EnemyTurn:
                currentState = BattleState.EnemyTurn;
                turnText.text = "Enemy Turn";
                break;
            case BattleState.Win:
                currentState = BattleState.Win;
                turnText.text = "Win";
                break;
            case BattleState.Lose:
                currentState = BattleState.Lose;
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
 
    public void ChangeTurn(BattleState stage)
    {
        StartCoroutine(TurnChange(stage));
    }
}
