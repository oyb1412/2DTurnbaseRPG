using System.Linq;
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

    [Header("--Char--")]
    public CharBase[] chars;

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
    public int currentPlayerNumber;
    int dead;

    [Header("--GameData--")]
    public GameData[] gameData;

    private void Awake()
    {
       // AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, true);
       state = BattleState.main;
    }
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.isPlaying)
            return;

             switch (StateManager.instance.currentState)
            {
                case StateManager.BattleState.StartTurn:

                chars = RealizeChar();
                battleInfoText.text = string.Format("공격속도가 가장 빠른 {0}부터 공격합니다", chars[0].charName);
                if (Input.GetMouseButtonDown(0))
                {
                    state = BattleState.main;
                    if (chars[0].GetComponentInChildren<PlayerManager>())
                        StateManager.instance.currentState = StateManager.BattleState.PlayerTurn;
                     else
                        StateManager.instance.currentState = StateManager.BattleState.EnemyTurn;
                }
                break;
                case StateManager.BattleState.PlayerTurn:
                switch (state)
                {
                    case BattleState.main:
                        players = RealizePlayer();
                        enemys = RealizeEnemy();

                        for (int i = 0;i<players.Length; i++)
                        {
                            if (chars[myTurn].charName == gameData[i].PlayerName)
                            {
                                currentPlayerNumber = i;
                                break;
                            }
                        }
                        battleInfoText.text = string.Format("{0}의 공격 순서입니다.", chars[myTurn].charName);
                        ChangeState(BattleState.skillselect);
                        break;
                    case BattleState.skillselect:
                        battleInfoText.text = "행동을 선택하세요.";
                        ButtonInfo();
                        if (gameData[currentPlayerNumber].playerCurrentHp < 10)
                        {
                            buttons[1].interactable = false;
                        }

                        buttonsParent.gameObject.SetActive(true);
                        arrowImage.gameObject.SetActive(true);

                        RectTransform rect = buttonsParent.transform.GetComponent<RectTransform>();
                        buttonsParent.transform.position = new Vector3(chars[myTurn].transform.position.x+3.5f,
                            chars[myTurn].transform.position.y+1.5f,
                            chars[myTurn].transform.position.z);

                        break;
                    case BattleState.allTargetSkill:
                        battleInfoText.text = "체력을 소모해 모든 적에게 피해를 입힙니다.";
                        ChangeState(BattleState.allTargetSkillAttack);

                        break;
                    case BattleState.oneTargetSkill:
                        battleInfoText.text = "체력을 소모해 강한 공격을 가합니다. \n공격 대상을 선택하세요.";
                        target = SelectEnemy();
                        if (target != null)
                        {
                            targetEnemy = target.gameObject.GetComponent<EnemyManager>();
                            ChangeState(BattleState.oneTargetSkillAttack);
                        }
                        break;
                    case BattleState.Heal:
                        battleInfoText.text = "자가회복을 시도합니다.";
                        ChangeState(BattleState.useHeal);
                        break;
                    case BattleState.allTargetSkillAttack:
                        battleInfoText.text = string.Format("모든 적에게 {0}만큼의 피해를 입힙니다.", gameData[currentPlayerNumber].playerAttackDamage);

                        if (!trigger)
                        {
                            gameData[currentPlayerNumber].playerCurrentHp -= 10;
                            chars[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        chars[myTurn].AllTargetAttack(enemys, 1);

                        if (!chars[myTurn].SetAttack)
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
                            chars = chars.Where(x => null != x).ToArray(); //null 배열만 지움


                            battleInfoText.text = string.Format("{0}의 스킬 공격으로 적이 쓰러졌습니다.", chars[myTurn].charName);
                            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                            {
                                chars = RealizeChar();
                                ChangeState(BattleState.changePlayer);
                            }
                            else
                            {
                                battleInfoText.text = "전투에서 승리했습니다!";
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
                        battleInfoText.text = string.Format("적 {0}에게 {1}만큼의 피해를 입힙니다.", targetEnemy.charName, chars[myTurn].attackDamage * 2f);

                        if (!trigger)
                        {
                            gameData[currentPlayerNumber].playerCurrentHp -= 10;
                            chars[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        chars[myTurn].Attack(targetEnemy, 1);

                        if (!chars[myTurn].SetAttack)
                            ChangeState(BattleState.Check);
                        break;
                    case BattleState.useHeal:
                        if (!trigger)
                        {
                            chars[myTurn].SetHeal = true;
                            battleInfoText.text = string.Format("{0}의 체력을 회복했습니다.", (gameData[currentPlayerNumber].skillDamage));
                            gameData[currentPlayerNumber].playerCurrentHp += (gameData[currentPlayerNumber].skillDamage);
                            Transform effect = Instantiate(chars[myTurn].effectPrefabs[2], chars[myTurn].transform).transform;
                            effect.transform.position = new Vector3(chars[myTurn].transform.position.x,
                                chars[myTurn].transform.position.y-0.5f,
                                chars[myTurn].transform.position.z);
                            trigger = true;
                        }
                        ChangeState(BattleState.changePlayer);
                        break;
                    case BattleState.targetselect:
                        battleInfoText.text = "대상을 선택하세요.";

                        PlyerInfo();
                        target = SelectEnemy();
                        if (target != null)
                        {
                            targetEnemy = target.gameObject.GetComponent<EnemyManager>();
                            state = BattleState.attack;
                        }
                        break;
                    case BattleState.attack:
                        battleInfoText.text = string.Format("{0}의 피해를 입혔습니다.",gameData[currentPlayerNumber].playerAttackDamage);

                        if(!trigger)
                        {
                            //공격(체력감소)
                            chars[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        chars[myTurn].Attack(targetEnemy,0);

                        if (!chars[myTurn].SetAttack)
                            ChangeState(BattleState.Check);

                        break;
                    case BattleState.Check:
                        if (targetEnemy.currentHp <= 0)
                        {
                            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                            {
                                battleInfoText.text = "적을 쓰러뜨렸지만, \n살아있는 적이 존재하므로 전투를 지속합니다.";
                                chars = chars.Where(x => null != x).ToArray(); //null 배열만 지움
                                ChangeState(BattleState.changePlayer);
                            }
                            else
                            {
                                battleInfoText.text = "전투에서 승리했습니다!";

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
                        battleInfoText.text = string.Format("{0}골드를 보상으로 획득했습니다!", gold * gameData[0].currentPlayerNumber);
                        
                        if (Input.GetMouseButtonDown(0))
                        {
                            gameData[0].currentGold = gameData[0].currentGold + (gold * gameData[0].currentPlayerNumber);
                            //AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, false);

                            GameManager.instance.ActionFade((int)GameManager.Scenes.FieldScene);
                        }
                        break;
                    case BattleState.changePlayer:

                        //순번이 캬라수를 넘어가면 순번0으로 초기화
                        if (myTurn +1 >= chars.Length)
                            myTurn = -1;

                            //공격 후 다음순번 캬랴 선택
                            if (chars.Length > myTurn + 1)
                            {
                                battleInfoText.text = string.Format("{0}에게 턴이 넘어갑니다.", chars[myTurn + 1].charName);
                            //다음순번 캬라가 적이면 currentstate 바꾸기
                            if (chars[myTurn+1].GetComponent<EnemyManager>())
                                {
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        myTurn ++;
                                        state = BattleState.main;
                                        StateManager.instance.currentState = StateManager.BattleState.EnemyTurn;
                                    }
                                }
                                //다음순번 캬라가 적이 아니면 currentstate 그대로
                                else
                                {
                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        myTurn++;
                                        state = BattleState.main;
                                    }
                                }
                            }
                        break;
                }
                break;
                case StateManager.BattleState.EnemyTurn:
                switch (state)
                {
                    case BattleState.main:
                        players = RealizePlayer();
                        enemys = RealizeEnemy();
                        battleInfoText.text = string.Format("{0}의 공격 순서입니다.", chars[myTurn].charName);
                        ChangeState(BattleState.targetselect);
                        break;
                    case BattleState.targetselect:
                        players = SelectPlayer(); //데이터 기준으로
                        target = players[0].GetComponent<Collider2D>();
  
                        if (target != null)
                        {
                            targetPlayer = target.gameObject.GetComponent<PlayerManager>();
                            battleInfoText.text = string.Format("적{0}가 공격 대상으로 {1}를 선택했습니다.", chars[myTurn].charName, targetPlayer.charName);
                            ChangeState(BattleState.attack);
                        }
                        break;
                    case BattleState.attack:
                        battleInfoText.text = string.Format("{0}가 공격받아 {1}의 피해를 입었습니다.", targetPlayer.charName, chars[myTurn].attackDamage);

                        if (!trigger)
                        {
                            targetPlayer.gameData.playerCurrentHp -= chars[myTurn].attackDamage;
                            chars[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        chars[myTurn].MeleeAttackAndMove(targetPlayer);

                        if (!chars[myTurn].SetAttack)
                            ChangeState(BattleState.Check);
                        break;
                    case BattleState.Check:
                        if (targetPlayer.gameData.playerCurrentHp <= 0)
                        {
                            chars = chars.Where(x => null != x).ToArray(); //null 배열만 지움

                            battleInfoText.text = string.Format("{0}의 공격으로 {1}가 쓰러졌습니다.", chars[myTurn].charName, targetPlayer.charName);
                            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                            {
                                chars = RealizeChar();

                                ChangeState(BattleState.changePlayer);
                            }
                            else
                            {
                                battleInfoText.text = "전투에서 패배했습니다...";
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
                        battleInfoText.text = "전투 패배로 100 골드를 잃었습니다.\n" + "가까운 마을로 이동합니다.";
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
                            //AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, false);

                            GameManager.instance.ActionFade((int)GameManager.Scenes.FieldScene);
                        }
                        break;
                    case BattleState.changePlayer:

                        if (myTurn + 1 >= chars.Length)
                            myTurn = -1;
                        //공격 후 다음순번 플레이어 선택
                        if (chars.Length > myTurn + 1)
                        {
                            battleInfoText.text = string.Format("{0}에게 턴이 넘어갑니다.", chars[myTurn + 1].charName);
                            if (chars[myTurn + 1].GetComponent<EnemyManager>())
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    myTurn++;
                                    trigger = false;
                                    state = BattleState.main;
                                }
                            }
                            else
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    myTurn++;
                                    trigger = false;
                                    state = BattleState.main;
                                    StateManager.instance.currentState = StateManager.BattleState.PlayerTurn;
                                }
                            }
                        }
                        break;
                }
                break;
            }
    }
    void Init()
    {
        battleInfoText = battleInfoImage.GetComponentInChildren<Text>();
        buttonInfoText = buttonInfoImage.GetComponentInChildren<Text>();
        EnemyInfoText = EenmyInfoImage.GetComponentInChildren<Text>();
        PlayerInfoText = PlayerInfoImage.GetComponentInChildren<Text>();

        buttonsParent.gameObject.SetActive(false);
        battleInfoImage.gameObject.SetActive(true);
        battleInfoText.gameObject.SetActive(true);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
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


    public void PlyerInfo()
    {
        Collider2D playerCol = MouseManager.instance.MouseRayCast("Player");
        if (playerCol)
        {
            player = playerCol.gameObject.GetComponent<PlayerManager>();
            PlayerInfoImage.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + 360f,
                Input.mousePosition.y, -Camera.main.transform.position.z));
            PlayerInfoText.text = player.charName + "\n" + "HP : " + player.currentHp + " / " + player.maxHp + "\n" + "Damage : " + player.attackDamage;
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
            buttonInfoText.text = string.Format("적 하나에게 {0}만큼\n 피해를 입힙니다", gameData[currentPlayerNumber].playerAttackDamage);

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
            if (chars[myTurn].charName == "Wizard")
                buttonInfoText.text = string.Format("10의 체력을 소모해 \n모든 적에게 {0}만큼\n 피해를 입힙니다", gameData[currentPlayerNumber].playerAttackDamage);
            else
                buttonInfoText.text = string.Format("10의 체력을 소모해 \n적 하나에게 {0}만큼\n 피해를 입힙니다", gameData[currentPlayerNumber].playerAttackDamage * 2f);

        }
        else if (HealButtonCol)
        {
            buttonsImage[2] = HealButtonCol.GetComponent<Image>();
            buttonsImage[2].transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            if (buttonsImage[0])
                buttonsImage[0].transform.localScale = Vector3.one;
            if (buttonsImage[1])
                buttonsImage[1].transform.localScale = Vector3.one;


            buttonInfoText.text = string.Format("{0}만큼 체력을\n 회복합니다", gameData[currentPlayerNumber].skillDamage);
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
            EnemyInfoText.text = enemy.charName + "\n" + "HP : " + enemy.currentHp + " / " + enemy.maxHp + "\n" + "Damage : " + enemy.attackDamage;
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
        //플레이어의 수 파악
        PlayerManager[] players = FindObjectsOfType<PlayerManager>();
        for (int j = 0; j < players.Length; j++)
        {
            for(int h = 0;h< players.Length -1; h++)
            {
                if (players[h].attackOrder < players[h + 1].attackOrder)
                {
                    PlayerManager save = new PlayerManager();
                    save = players[h + 1];
                    players[h + 1] = players[h];
                    players[h] = save;
                }
            }
        }

        return players;

    }

    EnemyManager[] RealizeEnemy()
    {
        //적의 수 파악
        EnemyManager[] enemys = FindObjectsOfType<EnemyManager>();

        //적 공격 순서 지정(숫자가 낮은 순)
        for (int j = 0; j < enemys.Length; j++)
        {
            for (int h = 0; h < enemys.Length - 1; h++)
            {

                if (enemys[h].attackOrder > enemys[h + 1].attackOrder)
                {
                    EnemyManager save = new EnemyManager();
                    save = enemys[h + 1];
                    enemys[h + 1] = enemys[h];
                    enemys[h] = save;
                }
            }
        }

        return enemys;
    }

    CharBase[] RealizeChar()
    {
        //적의 수 파악
        CharBase[] charObj = FindObjectsOfType<CharBase>();

        //적 공격 순서 지정(숫자가 낮은 순)
        for (int j = 0; j < charObj.Length; j++)
        {
            for (int h = 0; h < charObj.Length - 1; h++)
            {

                if (charObj[h].attackOrder < charObj[h + 1].attackOrder)
                {
                    CharBase save = new CharBase();
                    save = charObj[h + 1];
                    charObj[h + 1] = charObj[h];
                    charObj[h] = save;
                }
            }
        }
        return charObj;
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
        switch(chars[myTurn].charName)
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
