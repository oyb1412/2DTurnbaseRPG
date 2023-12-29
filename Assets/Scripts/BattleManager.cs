using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public PlayerManager[] players;
    public EnemyManager[] enemys;
    SpriteRenderer enemySpriter;
    EnemyManager targetEnemy;
    PlayerManager targetPlayer;
    public int myTurn;
    Collider2D target;
    bool trigger;
    //열거형으로 현재 플레이어의 상세한 state를 설정
    public enum BattleState {main, select,attack,changePlayer}
    public BattleState state;
    public Text battleInfo;
    // Start is called before the first frame update
    void Start()
    {
        //모든 플레이어 저장
    }
    
    // Update is called once per frame
    void Update()
    {
 

            switch (StateManager.instance.currentState)
        {
            case StateManager.BattleState.PlayerTurn:
                switch (state)
                {
                    case BattleState.main:
                        StartCoroutine(MainCorutine());
                        break;
                    case BattleState.select:
                        //공격할 대상 선택(마우스 클릭)
                        target = SelectEnemy();
                        if (target != null)
                        {
                            StartCoroutine(PlayerSelectCroutine());
                        }
                        break;
                    case BattleState.attack:
                        //공격(체력감소)
                        if (!trigger)
                        {
                            targetEnemy.currentHp -= players[myTurn].attackDamage;
                            if (targetEnemy.currentHp <= 0)
                                targetEnemy.gameObject.SetActive(false);

                            trigger = true;
                        }

                        StartCoroutine(PlayerAttackCroutine());

                        //공격으로 targetEnemy.currentHp가 0 이하가 되면 die코루틴 실행,
                        if (targetEnemy.currentHp <= 0)
                        {
                            //die코루틴 내부에서는 적 오브젝트를 지우고 그에 걸맞는 텍스트 출력후
                            StartCoroutine(EnemyDieCroutine());

                            //남아있는 적의 숫자를 확인

                            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                            {
                                //남아있는 적의 수가 1 이상인 경우 changeplayer로 이동
                                StartCoroutine(ContinuationCroutine("생존한 적이 존재하므로 전투를 계속합니다"));

                            }
                            else //남아있는 적의 수가 0인 경우 StateManager.BattleState.Win으로 이동.
                            {
                                StartCoroutine(WinCorutine());//여기에서 남은 적 오브젝트 전부 삭제 or 오브젝트 풀링으로 이용
                            }
                        }
                        else
                        {
                            StartCoroutine(ContinuationCroutine(string.Format("{0}를 공격해 {1}의 피해를 입혔습니다.", targetEnemy.name, players[myTurn].attackDamage)));
                        }
                        break;
                    case BattleState.changePlayer:
                        //공격 후 다음순번 플레이어 선택
                        if (players.Length > myTurn + 1)
                        {
                            StartCoroutine(PlayerChangeCroutine());
                        }
                        else//플레이어의 수 만큼 공격이 끝나면 적 턴으로 넘김
                        {
                            StartCoroutine(PlayerTurnChangeCroutine());
                        }
                        break;
                }
                break;

            case StateManager.BattleState.EnemyTurn:
                switch (state)
                {
                    case BattleState.select:
                        enemys = RealizeEnemy();
                        players = SelectPlayer();
                        //공격할 대상 선택
                        target = players[myTurn].GetComponent<Collider2D>();
                        if (target != null)
                        {
                            StartCoroutine(EnemySelectCroutine());
                        }
                        break;
                    case BattleState.attack:
                        //공격(체력감소)
                        if (!trigger)
                        {
                            targetPlayer.currentHp -= enemys[myTurn].attackDamage;
                            trigger = true;
                        }
                        StartCoroutine(EnemyAttackCroutine());

                        //targetPlayer.currentHp가 0 이하가 되면 die코루틴 실행,
                        if (targetPlayer.currentHp <= 0)
                        {
                            //die코루틴 내부에서는 플레이어 오브젝트를 지우고 그에 걸맞는 텍스트 출력후
                            StartCoroutine(PlayerDieCroutine());
                            //남아있는 플레이어의 숫자를 확인
                            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                            {
                                //남아있는 플레이어의 수가 1 이상인 경우 changeplayer로 이동
                                StartCoroutine(ContinuationCroutine("생존한 플레이어가 존재하므로 전투를 계속합니다"));
                            }
                            else
                            {
                                //남아있는 플레이어의 수가 0인 경우 StateManager.BattleState.Lose으로 이동.
                                StartCoroutine(LoseCorutine());
                            }
                        }
                        else
                        {
                            StartCoroutine(ContinuationCroutine(string.Format("{0}를 공격해 {1}의 피해를 입혔습니다.", targetPlayer.name, enemys[myTurn].attackDamage)));

                        }
                        break;
                    case BattleState.changePlayer:
                        //공격 후 다음순번 플레이어 선택
                        if (enemys.Length > myTurn + 1)
                        {
                            StartCoroutine(EnemyChangeCroutine());
                        }
                        else//적의 수 만큼 공격이 끝나면 적 턴으로 넘김
                        {
                            StartCoroutine(EnemyTurnChangeCroutine());
                        }
                        break;
                }
                break;
        }
    }
    IEnumerator WinCorutine()
    {
        battleInfo.text = "전투에서 승리했습니다!";

        yield return new WaitForSeconds(1f);
        //foreach (var enemy in enemys) 
        //{
        //    Destroy(enemy.gameObject);
        //}
        StateManager.instance.ChangeTurn(StateManager.BattleState.Win);

    }

    IEnumerator LoseCorutine()
    {
        battleInfo.text = "전투에서 패배했습니다...";

        yield return new WaitForSeconds(1f);
        StateManager.instance.ChangeTurn(StateManager.BattleState.Lose);

    }

    IEnumerator ContinuationCroutine(string text)
    {
        battleInfo.text = text;

        yield return new WaitForSeconds(1f);
        state = BattleState.changePlayer;

    }

    IEnumerator EnemyDieCroutine()
    {
        battleInfo.text = string.Format("{0}의 공격으로 {1}이 쓰러졌습니다.", players[myTurn].name, targetEnemy.name);
       // Destroy(targetEnemy.gameObject);

        yield return new WaitForSeconds(1f);

    }

    IEnumerator PlayerDieCroutine()
    {
        battleInfo.text = string.Format("{0}의 공격으로 {1}이 쓰러졌습니다.", enemys[myTurn].name, targetPlayer.name);
        targetPlayer.gameObject.SetActive(false) ;
        //Destroy(targetPlayer.gameObject);

        yield return new WaitForSeconds(1f);

    }

    IEnumerator MainCorutine()
    {
        players = RealizePlayer();
        enemys = RealizeEnemy();
        battleInfo.text = string.Format("{0}의 공격 순서입니다.", players[myTurn].name);
        yield return new WaitForSeconds(1f);
        state = BattleState.select;

    }
    IEnumerator PlayerTurnChangeCroutine()
    {
        battleInfo.text = "플레이어의 공격이 끝났습니다. 턴을 넘깁니다";
        yield return new WaitForSeconds(1f);
        myTurn = 0;
        trigger = false;
        state = BattleState.select;
        StateManager.instance.ChangeTurn(StateManager.BattleState.EnemyTurn);
    }
    IEnumerator PlayerChangeCroutine()
    {
        battleInfo.text = string.Format("{0}의 공격 순서입니다.", players[myTurn + 1].name);
        yield return new WaitForSeconds(1f);
        if (trigger)
        {
            myTurn++;
            trigger = false;
        }
        state = BattleState.main;
    }
    IEnumerator PlayerAttackCroutine()
    {
        battleInfo.text = string.Format("{0}를 공격해 {1}의 피해를 입혔습니다.", targetEnemy.name, players[myTurn].attackDamage);


        yield return new WaitForSeconds(1f);
    }
    IEnumerator PlayerSelectCroutine()
    {
        targetEnemy = target.gameObject.GetComponent<EnemyManager>();
        battleInfo.text = string.Format("공격 대상으로 {0}를 선택했습니다.", targetEnemy.name);
        yield return new WaitForSeconds(1f);
        state = BattleState.attack;
    }

    IEnumerator EnemyTurnChangeCroutine()
    {
        battleInfo.text = "적의 공격이 끝났습니다. 턴을 넘깁니다";
        yield return new WaitForSeconds(1f);
        myTurn = 0;
        trigger = false;
        state = BattleState.main;
        StateManager.instance.ChangeTurn(StateManager.BattleState.PlayerTurn);
    }
    IEnumerator EnemyChangeCroutine()
    {
        battleInfo.text = string.Format(" 적{0}의 공격 순서입니다.", enemys[myTurn + 1]);
        yield return new WaitForSeconds(1f);
        if (trigger)
        {
            myTurn++;
            trigger = false;
        }
        state = BattleState.select;
    }
    IEnumerator EnemyAttackCroutine()
    {
        battleInfo.text = string.Format("적 {0}이 {1}를 공격해 {2}의 피해를 입혔습니다.", enemys[myTurn], targetPlayer.name, enemys[myTurn].attackDamage);
        yield return new WaitForSeconds(1f);
    }
    IEnumerator EnemySelectCroutine()
    {
        targetPlayer = target.gameObject.GetComponent<PlayerManager>();

        battleInfo.text = string.Format("적{0}이 공격 대상으로 {1}를 선택했습니다.",enemys[myTurn], targetPlayer.name);

        yield return new WaitForSeconds(1f);
        state = BattleState.attack;
    }
    PlayerManager[] SelectPlayer()
    {
        //플레이어 배열 순서 변경(체력이 낮은 순)
        for (int i = 0; i < players.Length; i++)
        {
            if (i + 1 >= players.Length)
                break;

            if (players[i].currentHp > players[i + 1].currentHp)
            {
                PlayerManager save = new PlayerManager();
                save = players[i + 1];
                players[i + 1] = players[i];
                players[i] = save;
            }
        }

        return players;
    }
    public Collider2D SelectEnemy()
    {
        Collider2D col;

        if (col = MouseManager.instance.MouseRayCast("Enemy"))
        {
            enemySpriter = col.gameObject.GetComponent<SpriteRenderer>();
            enemySpriter.color = new Color(0, 0, 0, 1);
        }
        else if (enemySpriter != null)
            enemySpriter.color = new Color(1, 1, 1, 1);

        if (Input.GetMouseButtonDown(0) && col != null)
        {
            enemySpriter.color = new Color(1, 1, 1, 1);
            return col;
        }
        else
            return null;
    }
    PlayerManager[] RealizePlayer()
    {
        //플레이어의 수 파악
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();

        //플레이어 공격 순서 지정(숫자가 낮은 순)
        for (int i = 0; i < players.Length; i++)
        {
            if (i + 1 >= players.Length)
                break;

            if (players[i].attackNumber > players[i + 1].attackNumber)
            {
                PlayerManager save = new PlayerManager();
                save = players[i + 1];
                players[i + 1] = players[i];
                players[i] = save;
            }
        }

        return players;
    }

    EnemyManager[] RealizeEnemy()
    {
        //적의 수 파악
        EnemyManager[] enemys = FindObjectsOfType<EnemyManager>();

        //적 공격 순서 지정(숫자가 낮은 순)
        for (int i = 0; i < enemys.Length; i++)
        {
            if (i + 1 >= enemys.Length)
                break;

            if (enemys[i].attackNumber > enemys[i + 1].attackNumber)
            {
                EnemyManager save = new EnemyManager();
                save = enemys[i + 1];
                enemys[i + 1] = enemys[i];
                enemys[i] = save;
            }
        }

        return enemys;
    }

}
