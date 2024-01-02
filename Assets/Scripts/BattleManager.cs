using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("--Player--")]
    PlayerManager[] players;
    PlayerManager targetPlayer;
    PlayerManager player;

    [Header("--Enemy--")]
    EnemyManager[] enemys;
    EnemyManager targetEnemy;
    EnemyManager enemy;

    [Header("--UI--")]
    public Image arrowImage;
    public Image battleInfoImage;
    public Button[] buttons;
    public Image EenmyInfoImage;
    public Image PlayerInfoImage;
    public Texture2D cursurIcon;
    public GameObject buttonsParent;
    public Image buttonInfoImage;
    public Text buttonInfoText;
    public Text battleInfoText;

    Image[] buttonsImage = new Image[3];
    public Text EnemyInfoText;
    public Text PlayerInfoText;

    [Header("--State--")]
    public BattleState state;

    [Header("--Object--")]
    GameObject[] charObjects;
    public enum BattleState
    {
        main, skillselect, targetselect, attack, Check, changePlayer, Win, Lose,
        oneTargetSkill, allTargetSkill, Heal, allTargetSkillAttack, oneTargetSkillAttack, useHeal,
        allTargetSkillCheck
    }

    [Header("--Info--")]
    int gold;
    public int myTurn;
    public int deadCount;
    Collider2D target;
    public bool trigger;

    [Header("--GameData--")]
    public GameData[] gameData;

    private void Awake()
    {
       // AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, true);

    }
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        battleInfoText = battleInfoImage.GetComponentInChildren<Text>();
        Debug.Log(battleInfoText.name);
        buttonInfoText = buttonInfoImage.GetComponentInChildren<Text>();
        EnemyInfoText = EenmyInfoImage.GetComponentInChildren<Text>();
        PlayerInfoText = PlayerInfoImage.GetComponentInChildren<Text>();

        buttonsParent.gameObject.SetActive(false);
        battleInfoImage.gameObject.SetActive(false);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPlaying)
            return;

             switch (StateManager.instance.currentState)
            {
                case StateManager.BattleState.PlayerTurn:
                switch (state)
                {
                    case BattleState.main:
                        players = RealizePlayer();
                        enemys = RealizeEnemy();
                        battleInfoImage.gameObject.SetActive(true);
                        battleInfoText.text = string.Format("{0}�� ���� �����Դϴ�.", players[myTurn].playerName);
                        ChangeState(BattleState.skillselect);
                        break;
                    case BattleState.skillselect:
                        battleInfoText.text = string.Format("�ൿ�� �����ϼ���.");
                        ButtonInfo();
                        if (gameData[myTurn].playerCurrentHp < 10)
                        {
                            buttons[1].interactable = false;
                        }

                        buttonsParent.gameObject.SetActive(true);
                        arrowImage.gameObject.SetActive(true);

                        RectTransform rect = buttonsParent.transform.GetComponent<RectTransform>();
                        buttonsParent.transform.position = new Vector3(players[myTurn].transform.position.x+3.5f,
                            players[myTurn].transform.position.y+1.5f,
                            players[myTurn].transform.position.z);


                        break;
                    case BattleState.allTargetSkill:
                        battleInfoText.text = string.Format("ü���� �Ҹ��� ��� ������ ���ظ� �����ϴ�.");
                        ChangeState(BattleState.allTargetSkillAttack);

                        break;
                    case BattleState.oneTargetSkill:
                        battleInfoText.text = string.Format("ü���� �Ҹ��� ���� ������ ���մϴ�. \n���� ����� �����ϼ���.");
                        target = SelectEnemy();
                        if (target != null)
                        {
                            targetEnemy = target.gameObject.GetComponent<EnemyManager>();
                            ChangeState(BattleState.oneTargetSkillAttack);
                        }
                        break;
                    case BattleState.Heal:
                        battleInfoText.text = string.Format("�ڰ�ȸ���� �õ��մϴ�.");
                        ChangeState(BattleState.useHeal);
                        break;
                    case BattleState.allTargetSkillAttack:
                        battleInfoText.text = string.Format("��� ������ {0}��ŭ�� ���ظ� �����ϴ�.", gameData[myTurn].playerAttackDamage);

                        if (!trigger)
                        {
                            gameData[myTurn].playerCurrentHp -= 10;
                            players[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        players[myTurn].AllTargetAttack(enemys, 1);

                        if (!players[myTurn].SetAttack)
                            ChangeState(BattleState.allTargetSkillCheck);

                        break;
                    case BattleState.allTargetSkillCheck:
                        if (!trigger)
                        {
                            for (int i = 0; i < enemys.Length; i++)
                            {
                                if (enemys[i].currentHp <= 0)
                                {
                                    deadCount++;
                                }
                            }
                            trigger = true;
                        }
                        if (deadCount > 0)
                        {
                            battleInfoText.text = string.Format("{0}�� ��ų �������� ���� ���������ϴ�.", players[myTurn].playerName);
                            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                            {
                                ChangeState(BattleState.changePlayer);
                            }
                            else
                            {
                                battleInfoText.text = "�������� �¸��߽��ϴ�!";
                                int enemyType = PlayerPrefs.GetInt("colliderEnemyType");
                                switch (enemyType)
                                {
                                    case 0:
                                        gold = Random.Range(60, 80);
                                        break;
                                    case 1:
                                        gold = Random.Range(70, 90);
                                        break;
                                    case 2:
                                        gold = Random.Range(80, 100);
                                        break;
                                }

                                ChangeState(BattleState.Win);
                             }
                        }
                        else
                        {
                            trigger = false;
                            state = BattleState.changePlayer;
                        }
                        
                        break;
                    case BattleState.oneTargetSkillAttack:
                        battleInfoText.text = string.Format("�� {0}���� {1}��ŭ�� ���ظ� �����ϴ�.", targetEnemy.enemyName, players[myTurn].attackDamage * 2f);

                        if (!trigger)
                        {
                            gameData[myTurn].playerCurrentHp -= 10;
                            players[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        players[myTurn].Attack(targetEnemy, 1);

                        if (!players[myTurn].SetAttack)
                            ChangeState(BattleState.Check);
                        break;
                    case BattleState.useHeal:
                        if (!trigger)
                        {
                            players[myTurn].SetHeal = true;
                            battleInfoText.text = string.Format("{0}�� ü���� ȸ���߽��ϴ�.", (gameData[myTurn].skillDamage));
                            gameData[myTurn].playerCurrentHp += (gameData[myTurn].skillDamage);
                            Transform effect = Instantiate(players[myTurn].effectPrefabs[2], players[myTurn].transform).transform;
                            effect.transform.position = new Vector3(players[myTurn].transform.position.x,
                                players[myTurn].transform.position.y-0.5f,
                                players[myTurn].transform.position.z);
                            trigger = true;
                        }
                        ChangeState(BattleState.changePlayer);
                        break;
                    case BattleState.targetselect:
                        battleInfoText.text = string.Format("����� �����ϼ���.");

                        PlyerInfo();
                        target = SelectEnemy();
                        if (target != null)
                        {
                            targetEnemy = target.gameObject.GetComponent<EnemyManager>();
                            state = BattleState.attack;
                        }
                        break;
                    case BattleState.attack:
                        battleInfoText.text = string.Format("{0}�� ���ظ� �������ϴ�.",gameData[myTurn].playerAttackDamage);

                        if(!trigger)
                        {
                            //����(ü�°���)
                            players[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        players[myTurn].Attack(targetEnemy,0);

                        if (!players[myTurn].SetAttack)
                            ChangeState(BattleState.Check);

                        break;
                    case BattleState.Check:
                        if (targetEnemy.currentHp <= 0)
                        {
                            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                            {
                                battleInfoText.text = string.Format("����ִ� ���� �����ϹǷ� ������ �����մϴ�.", gameData[myTurn].playerAttackDamage);

                                ChangeState(BattleState.changePlayer);
                            }
                            else
                            {
                                battleInfoText.text = string.Format("�������� �¸��߽��ϴ�!", gameData[myTurn].playerAttackDamage);

                                int enemyType = PlayerPrefs.GetInt("colliderEnemyType");
                                switch (enemyType)
                                {
                                    case 0:
                                        gold = Random.Range(60, 80);
                                        break;
                                    case 1:
                                        gold = Random.Range(70, 90);
                                        break;
                                    case 2:
                                        gold = Random.Range(80, 100);
                                        break;
                                }
                                ChangeState(BattleState.Win);

                            }
                        }
                        else
                        {
                            state = BattleState.changePlayer;
                        }
                        break;
                    case BattleState.Win:
                        battleInfoText.text = string.Format("{0}��带 �������� ȹ���߽��ϴ�!", gold * gameData[0].currentPlayerNumber);
                        
                        if (Input.GetMouseButtonDown(0))
                        {
                            gameData[0].currentGold = gameData[0].currentGold + (gold * gameData[0].currentPlayerNumber);
                            AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, false);

                            GameManager.instance.ActionFade((int)GameManager.Scenes.FieldScene);
                        }
                        break;
                    case BattleState.changePlayer:
                        //���� �� �������� �÷��̾� ����
                        if (players.Length > myTurn + 1)
                        {
                            battleInfoText.text = string.Format("{0}���� ���� �Ѿ�ϴ�.", players[myTurn + 1].playerName);

                            if (Input.GetMouseButtonDown(0))
                            {
                                myTurn++;
                                state = BattleState.main;
                            }

                            ChangeState(BattleState.main);
                        }
                        else
                        {
                            battleInfoText.text = "�÷��̾��� ������ �������ϴ�. ���� �ѱ�ϴ�";

                            if (Input.GetMouseButtonDown(0))
                            {
                                battleInfoImage.gameObject.SetActive(false);

                                myTurn = 0;
                                state = BattleState.main;
                                StateManager.instance.ChangeTurn(StateManager.BattleState.EnemyTurn);
                            }
                        }

                        break;
                }
                break;
                case StateManager.BattleState.EnemyTurn:
                switch (state)
                {
                    case BattleState.main:
                        enemys = RealizeEnemy();
                        players = SelectPlayer();
                        battleInfoImage.gameObject.SetActive(true);

                        battleInfoText.text = string.Format("{0}�� ���� �����Դϴ�.", enemys[myTurn].enemyName);
                        ChangeState(BattleState.targetselect);
                        break;
                    case BattleState.targetselect:
                        target = players[myTurn].GetComponent<Collider2D>();
                        if (target != null)
                        {
                            targetPlayer = target.gameObject.GetComponent<PlayerManager>();
                            battleInfoText.text = string.Format("��{0}�� ���� ������� {1}�� �����߽��ϴ�.", enemys[myTurn].enemyName, targetPlayer.playerName);
                            ChangeState(BattleState.attack);
                        }
                        break;
                    case BattleState.attack:
                        battleInfoText.text = string.Format("{0}�� ���ݹ޾� {1}�� ���ظ� �Ծ����ϴ�.", targetPlayer.playerName, enemys[myTurn].attackDamage);

                        if (!trigger)
                        {
                            targetPlayer.gameData.playerCurrentHp -= enemys[myTurn].attackDamage;
                            enemys[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        enemys[myTurn].MeleeAttackAndMove(targetPlayer);

                        if (!enemys[myTurn].SetAttack)
                            ChangeState(BattleState.Check);
                        break;
                    case BattleState.Check:
                        if (targetPlayer.gameData.playerCurrentHp <= 0)
                        {
                            battleInfoText.text = string.Format("{0}�� �������� {1}�� ���������ϴ�.", enemys[myTurn].enemyName, targetPlayer.playerName);
                            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                            {
                                ChangeState(BattleState.changePlayer);
                            }
                            else
                            {
                                battleInfoText.text = "�������� �й��߽��ϴ�...";
                                gold = Random.Range(30, 100);
                                ChangeState(BattleState.Lose);

                              
                            }
                        }
                        else
                        {
                            trigger = false;
                            state = BattleState.changePlayer;
                        }
                        break;
                    case BattleState.Lose:
                        battleInfoText.text = "���� �й�� 100 ��带 �Ҿ����ϴ�.\n" + "����� ������ �̵��մϴ�.";
                        if (Input.GetMouseButtonDown(0))
                        {
                            if(gameData[0].currentGold > 100)
                            {
                                gameData[0].currentGold -= 100;
                            }
                            if (gameData[0].currentGold <= 100)
                            {
                                gameData[0].currentGold = 0;
                            }
                            for(int i = 0; i < players.Length; i++)
                            {
                                gameData[i].playerCurrentHp = gameData[i].playerMaxHp;
                            }
                            gameData[0].playerFieldPosition = new Vector2(-9.3f, -9.5f);
                            AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, false);

                            GameManager.instance.ActionFade((int)GameManager.Scenes.FieldScene);
                        }
                        break;
                    case BattleState.changePlayer:
                        //���� �� �������� �÷��̾� ����
                        if (enemys.Length > myTurn + 1)
                        {
                            battleInfoText.text = string.Format("{0}���� ���� �Ѿ�ϴ�.", enemys[myTurn + 1].enemyName);

                            if (Input.GetMouseButtonDown(0))
                            {
                                myTurn++;
                                trigger = false;
                                state = BattleState.main;
                            }
                        }
                        else//���� �� ��ŭ ������ ������ �� ������ �ѱ�
                        {
                            battleInfoText.text = "���� ������ �������ϴ�. ���� �ѱ�ϴ�";
                            if (Input.GetMouseButtonDown(0))
                            {
                                battleInfoImage.gameObject.SetActive(false);
                                myTurn = 0;
                                trigger = false;
                                state = BattleState.main;
                                StateManager.instance.ChangeTurn(StateManager.BattleState.PlayerTurn);
                            }
                        }
                        break;
                }
                break;
            }
    }
   
    void ChangeState(BattleState type)
    {
        if (Input.GetMouseButtonDown(0))
        {
            trigger = false;
            state = type;
        }
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
    public void PlyerInfo()
    {
        Collider2D playerCol = MouseManager.instance.MouseRayCast("Player");
        if (playerCol)
        {
            player = playerCol.gameObject.GetComponent<PlayerManager>();
            PlayerInfoImage.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + 360f,
                Input.mousePosition.y, -Camera.main.transform.position.z));
            PlayerInfoText.text = player.playerName + "\n" + "HP : " + player.currentHp + " / " + player.maxHp + "\n" + "Damage : " + player.attackDamage;
            PlayerInfoImage.gameObject.SetActive(true);
            player.transform.localScale = new Vector3(1.1f, 1.1f, 1f); 
        }
        else if(player)
        {
            player.transform.localScale = Vector3.one;

            PlayerInfoImage.gameObject.SetActive(false);
        }
    }

    public void ButtonInfo()
    {

        Collider2D AttackButtonCol = MouseManager.instance.MouseRayCast("AttackButtonInfo");
        Collider2D SkillButtonCol = MouseManager.instance.MouseRayCast("SkillButtonInfo");
        Collider2D HealButtonCol = MouseManager.instance.MouseRayCast("HealButtonInfo");

        if (AttackButtonCol)
        {
            buttonsImage[0] = AttackButtonCol.GetComponent<Image>();
            buttonsImage[0].transform.localScale = new Vector3(1.1f, 1.1f,1f);
            if (buttonsImage[1])
                buttonsImage[1].transform.localScale = Vector3.one;
            if(buttonsImage[2])
                buttonsImage[2].transform.localScale = Vector3.one;
            buttonInfoImage.gameObject.SetActive(true);
            buttonInfoText.text = string.Format("�� �ϳ����� {0}��ŭ\n ���ظ� �����ϴ�", gameData[myTurn].playerAttackDamage);

        }
        else if (SkillButtonCol)
        {
            buttonsImage[1] = SkillButtonCol.GetComponent<Image>();
            buttonsImage[1].transform.localScale = new Vector3(1.1f,1.1f,1f);
            if(buttonsImage[0])
                buttonsImage[0].transform.localScale = Vector3.one;
            if(buttonsImage[2])
                buttonsImage[2].transform.localScale = Vector3.one;

            buttonInfoImage.gameObject.SetActive(true);
            if (players[myTurn].playerName == "Wizard")
                buttonInfoText.text = string.Format("10�� ü���� �Ҹ��� \n��� ������ {0}��ŭ\n ���ظ� �����ϴ�", gameData[myTurn].playerAttackDamage);
            else
                buttonInfoText.text = string.Format("10�� ü���� �Ҹ��� \n�� �ϳ����� {0}��ŭ\n ���ظ� �����ϴ�", gameData[myTurn].playerAttackDamage * 2f);

        }
        else if (HealButtonCol)
        {
            buttonsImage[2] = HealButtonCol.GetComponent<Image>();
            buttonsImage[2].transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            if (buttonsImage[0])
                buttonsImage[0].transform.localScale = Vector3.one;
            if (buttonsImage[1])
                buttonsImage[1].transform.localScale = Vector3.one;


            buttonInfoText.text = string.Format("{0}��ŭ ü����\n ȸ���մϴ�", gameData[myTurn].skillDamage);
            buttonInfoImage.gameObject.SetActive(true);
        }
        else if(buttonsImage[0] && buttonsImage[1] && buttonsImage[2])
        {
            buttonsImage[0].transform.localScale = Vector3.one;
            buttonsImage[1].transform.localScale = Vector3.one;
            buttonsImage[2].transform.localScale = Vector3.one;

            buttonInfoImage.gameObject.SetActive(false);
        }


        buttonInfoImage.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + 350f,
                Input.mousePosition.y, -Camera.main.transform.position.z));
    }
    public Collider2D SelectEnemy()
    {
        Collider2D col = MouseManager.instance.MouseRayCast("Enemy");

        if (col)
        {
            enemy = col.gameObject.GetComponent<EnemyManager>();
            EenmyInfoImage.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x - 360f,
                Input.mousePosition.y, -Camera.main.transform.position.z));
            EnemyInfoText.text = enemy.enemyName + "\n" + "HP : " + enemy.currentHp + " / " + enemy.maxHp + "\n" + "Damage : " + enemy.attackDamage;
            EenmyInfoImage.gameObject.SetActive(true);
            Cursor.SetCursor(cursurIcon, new Vector2(cursurIcon.width / 2f, cursurIcon.height / 2f), CursorMode.Auto);
            enemy.transform.localScale = new Vector3(-1.1f, 1.1f, 1f);
        }
        else if(enemy)
        {
            enemy.transform.localScale = new Vector3(-1f, 1f, 1f);
            EenmyInfoImage.gameObject.SetActive(false);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        }


        if (Input.GetMouseButtonDown(0) && col != null)
        {
            EenmyInfoImage.gameObject.SetActive(false);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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

        if (players.Length < 2)
            return players;

        if (players[0].attackNumber > players[1].attackNumber)
        {
            PlayerManager save = gameObject.AddComponent<PlayerManager>();
            save = players[1];
            players[1] = players[0];
            players[0] = save;
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

            if (enemys[i].attackOrder > enemys[i + 1].attackOrder)
            {
                EnemyManager save = new EnemyManager();
                save = enemys[i + 1];
                enemys[i + 1] = enemys[i];
                enemys[i] = save;
            }
        }

        return enemys;
    }


    public void AttackClick()
    {
        buttonsParent.SetActive(false);
        state = BattleState.targetselect;
        arrowImage.gameObject.SetActive(false);
         buttonInfoImage.gameObject.SetActive(false);

    }

    public void SkillClick()
    {
        switch(players[myTurn].playerName)
        {
            case "Wizard":
                state = BattleState.allTargetSkill;
                break;
            case "Mini":
            case "Miho":
                state = BattleState.oneTargetSkill;
                break;
        }

        buttonsParent.SetActive(false);

        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);

    }

    public void HealClick()
    {

        buttonsParent.SetActive(false);

        state = BattleState.Heal;
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);

    }
}
