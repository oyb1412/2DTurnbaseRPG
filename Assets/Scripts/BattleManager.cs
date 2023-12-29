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
    //���������� ���� �÷��̾��� ���� state�� ����
    public enum BattleState {main, select,attack,changePlayer}
    public BattleState state;
    public Text battleInfo;
    // Start is called before the first frame update
    void Start()
    {
        //��� �÷��̾� ����
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
                        //������ ��� ����(���콺 Ŭ��)
                        target = SelectEnemy();
                        if (target != null)
                        {
                            StartCoroutine(PlayerSelectCroutine());
                        }
                        break;
                    case BattleState.attack:
                        //����(ü�°���)
                        if (!trigger)
                        {
                            targetEnemy.currentHp -= players[myTurn].attackDamage;
                            if (targetEnemy.currentHp <= 0)
                                targetEnemy.gameObject.SetActive(false);

                            trigger = true;
                        }

                        StartCoroutine(PlayerAttackCroutine());

                        //�������� targetEnemy.currentHp�� 0 ���ϰ� �Ǹ� die�ڷ�ƾ ����,
                        if (targetEnemy.currentHp <= 0)
                        {
                            //die�ڷ�ƾ ���ο����� �� ������Ʈ�� ����� �׿� �ɸ´� �ؽ�Ʈ �����
                            StartCoroutine(EnemyDieCroutine());

                            //�����ִ� ���� ���ڸ� Ȯ��

                            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                            {
                                //�����ִ� ���� ���� 1 �̻��� ��� changeplayer�� �̵�
                                StartCoroutine(ContinuationCroutine("������ ���� �����ϹǷ� ������ ����մϴ�"));

                            }
                            else //�����ִ� ���� ���� 0�� ��� StateManager.BattleState.Win���� �̵�.
                            {
                                StartCoroutine(WinCorutine());//���⿡�� ���� �� ������Ʈ ���� ���� or ������Ʈ Ǯ������ �̿�
                            }
                        }
                        else
                        {
                            StartCoroutine(ContinuationCroutine(string.Format("{0}�� ������ {1}�� ���ظ� �������ϴ�.", targetEnemy.name, players[myTurn].attackDamage)));
                        }
                        break;
                    case BattleState.changePlayer:
                        //���� �� �������� �÷��̾� ����
                        if (players.Length > myTurn + 1)
                        {
                            StartCoroutine(PlayerChangeCroutine());
                        }
                        else//�÷��̾��� �� ��ŭ ������ ������ �� ������ �ѱ�
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
                        //������ ��� ����
                        target = players[myTurn].GetComponent<Collider2D>();
                        if (target != null)
                        {
                            StartCoroutine(EnemySelectCroutine());
                        }
                        break;
                    case BattleState.attack:
                        //����(ü�°���)
                        if (!trigger)
                        {
                            targetPlayer.currentHp -= enemys[myTurn].attackDamage;
                            trigger = true;
                        }
                        StartCoroutine(EnemyAttackCroutine());

                        //targetPlayer.currentHp�� 0 ���ϰ� �Ǹ� die�ڷ�ƾ ����,
                        if (targetPlayer.currentHp <= 0)
                        {
                            //die�ڷ�ƾ ���ο����� �÷��̾� ������Ʈ�� ����� �׿� �ɸ´� �ؽ�Ʈ �����
                            StartCoroutine(PlayerDieCroutine());
                            //�����ִ� �÷��̾��� ���ڸ� Ȯ��
                            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                            {
                                //�����ִ� �÷��̾��� ���� 1 �̻��� ��� changeplayer�� �̵�
                                StartCoroutine(ContinuationCroutine("������ �÷��̾ �����ϹǷ� ������ ����մϴ�"));
                            }
                            else
                            {
                                //�����ִ� �÷��̾��� ���� 0�� ��� StateManager.BattleState.Lose���� �̵�.
                                StartCoroutine(LoseCorutine());
                            }
                        }
                        else
                        {
                            StartCoroutine(ContinuationCroutine(string.Format("{0}�� ������ {1}�� ���ظ� �������ϴ�.", targetPlayer.name, enemys[myTurn].attackDamage)));

                        }
                        break;
                    case BattleState.changePlayer:
                        //���� �� �������� �÷��̾� ����
                        if (enemys.Length > myTurn + 1)
                        {
                            StartCoroutine(EnemyChangeCroutine());
                        }
                        else//���� �� ��ŭ ������ ������ �� ������ �ѱ�
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
        battleInfo.text = "�������� �¸��߽��ϴ�!";

        yield return new WaitForSeconds(1f);
        //foreach (var enemy in enemys) 
        //{
        //    Destroy(enemy.gameObject);
        //}
        StateManager.instance.ChangeTurn(StateManager.BattleState.Win);

    }

    IEnumerator LoseCorutine()
    {
        battleInfo.text = "�������� �й��߽��ϴ�...";

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
        battleInfo.text = string.Format("{0}�� �������� {1}�� ���������ϴ�.", players[myTurn].name, targetEnemy.name);
       // Destroy(targetEnemy.gameObject);

        yield return new WaitForSeconds(1f);

    }

    IEnumerator PlayerDieCroutine()
    {
        battleInfo.text = string.Format("{0}�� �������� {1}�� ���������ϴ�.", enemys[myTurn].name, targetPlayer.name);
        targetPlayer.gameObject.SetActive(false) ;
        //Destroy(targetPlayer.gameObject);

        yield return new WaitForSeconds(1f);

    }

    IEnumerator MainCorutine()
    {
        players = RealizePlayer();
        enemys = RealizeEnemy();
        battleInfo.text = string.Format("{0}�� ���� �����Դϴ�.", players[myTurn].name);
        yield return new WaitForSeconds(1f);
        state = BattleState.select;

    }
    IEnumerator PlayerTurnChangeCroutine()
    {
        battleInfo.text = "�÷��̾��� ������ �������ϴ�. ���� �ѱ�ϴ�";
        yield return new WaitForSeconds(1f);
        myTurn = 0;
        trigger = false;
        state = BattleState.select;
        StateManager.instance.ChangeTurn(StateManager.BattleState.EnemyTurn);
    }
    IEnumerator PlayerChangeCroutine()
    {
        battleInfo.text = string.Format("{0}�� ���� �����Դϴ�.", players[myTurn + 1].name);
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
        battleInfo.text = string.Format("{0}�� ������ {1}�� ���ظ� �������ϴ�.", targetEnemy.name, players[myTurn].attackDamage);


        yield return new WaitForSeconds(1f);
    }
    IEnumerator PlayerSelectCroutine()
    {
        targetEnemy = target.gameObject.GetComponent<EnemyManager>();
        battleInfo.text = string.Format("���� ������� {0}�� �����߽��ϴ�.", targetEnemy.name);
        yield return new WaitForSeconds(1f);
        state = BattleState.attack;
    }

    IEnumerator EnemyTurnChangeCroutine()
    {
        battleInfo.text = "���� ������ �������ϴ�. ���� �ѱ�ϴ�";
        yield return new WaitForSeconds(1f);
        myTurn = 0;
        trigger = false;
        state = BattleState.main;
        StateManager.instance.ChangeTurn(StateManager.BattleState.PlayerTurn);
    }
    IEnumerator EnemyChangeCroutine()
    {
        battleInfo.text = string.Format(" ��{0}�� ���� �����Դϴ�.", enemys[myTurn + 1]);
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
        battleInfo.text = string.Format("�� {0}�� {1}�� ������ {2}�� ���ظ� �������ϴ�.", enemys[myTurn], targetPlayer.name, enemys[myTurn].attackDamage);
        yield return new WaitForSeconds(1f);
    }
    IEnumerator EnemySelectCroutine()
    {
        targetPlayer = target.gameObject.GetComponent<PlayerManager>();

        battleInfo.text = string.Format("��{0}�� ���� ������� {1}�� �����߽��ϴ�.",enemys[myTurn], targetPlayer.name);

        yield return new WaitForSeconds(1f);
        state = BattleState.attack;
    }
    PlayerManager[] SelectPlayer()
    {
        //�÷��̾� �迭 ���� ����(ü���� ���� ��)
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
        //�÷��̾��� �� �ľ�
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();

        //�÷��̾� ���� ���� ����(���ڰ� ���� ��)
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
        //���� �� �ľ�
        EnemyManager[] enemys = FindObjectsOfType<EnemyManager>();

        //�� ���� ���� ����(���ڰ� ���� ��)
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
