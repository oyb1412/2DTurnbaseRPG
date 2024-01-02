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
    public enum BattleState { StartTurn, PlayerTurn, EnemyTurn, Win, Lose }


    private void Awake()
    {
        instance = this;
        GameManager.instance.isPlaying = true;
 
    }
    private void Start()
    {
        currentState = BattleState.StartTurn;
    }
 
}
