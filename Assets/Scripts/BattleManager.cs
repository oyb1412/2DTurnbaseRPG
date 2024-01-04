using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.SceneManagement;

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
    public GameObject skillButtons;
    Image[] skillButtonsImages;
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
    Image[] buttonsImage = new Image[2];
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
        allTargetSkillCheck,SkillList
    }

    [Header("--Info--")]
    int gold;
    public int myTurn;
    public int deadCount;
    Collider2D target;
    public bool trigger;
    public bool attackTrigger;
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
        skillButtonsImages = new Image[gameData.Length];
        for (int i = 0; i < gameData.Length; i++)
            skillButtonsImages[i] = skillButtons.GetComponentsInChildren<Image>()[i*2];
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
                battleInfoImage.gameObject.SetActive(true);
                    state = BattleState.main;
                    if (chars[0].GetComponentInChildren<PlayerManager>())
                        StateManager.instance.currentState = StateManager.BattleState.PlayerTurn;
                     else
                        StateManager.instance.currentState = StateManager.BattleState.EnemyTurn;
                
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
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0}'s Turn", chars[myTurn].charName), 1);
                            StartCoroutine(ChangeState(BattleState.skillselect, 1.5f));
                            trigger = true;
                        }
                        break;
                    case BattleState.skillselect:
                        ButtonInfo();
                        if (gameData[currentPlayerNumber].playerCurrenMp < 10)
                        {
                            buttons[1].interactable = false;
                        }

                        buttonsParent.gameObject.SetActive(true);
                        arrowImage.gameObject.SetActive(true);

                        buttonsParent.transform.position = new Vector3(chars[myTurn].transform.position.x+3.5f,
                            chars[myTurn].transform.position.y+1.5f,
                            chars[myTurn].transform.position.z);

                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText("Select Your Action",1);
                            trigger = true;
                        }

                        break;
                    case BattleState.SkillList:
                        SkillButtonInfo();
                        skillButtons.transform.position = new Vector3(chars[myTurn].transform.position.x + 3.5f,
                            chars[myTurn].transform.position.y + 1.5f,
                            chars[myTurn].transform.position.z);
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText("Select your Skill", 1);
                            trigger = true;
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            SkillReturn();
                        }
                        break;
                    case BattleState.allTargetSkill:

                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("MP -10 {0} Damage All Enemies", gameData[currentPlayerNumber].playerAttackDamage), 1);
                            StartCoroutine(ChangeState(BattleState.allTargetSkillAttack, 1.5f));

                            trigger = true;
                        }
                        break;
                    case BattleState.oneTargetSkill:
                        target = SelectEnemy();
                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("MP -10 {0} Damage One Enemy", gameData[currentPlayerNumber].playerAttackDamage * 2), 1);
                            trigger = true;
                        }
                        if (target != null)
                        {
                            targetEnemy = target.gameObject.GetComponent<EnemyManager>();
                            trigger = false;
                            state = BattleState.oneTargetSkillAttack;
                        }
                        break;
                    case BattleState.Heal:
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("MP -10 {0} HP Self Healing", gameData[currentPlayerNumber].skillDamage), 1);
                            trigger = true;
                        }
                        ChangeState(BattleState.useHeal);
                        break;
                    case BattleState.allTargetSkillAttack:

                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0} Damage All Enemies", gameData[currentPlayerNumber].playerAttackDamage), 1);
                            gameData[currentPlayerNumber].playerCurrenMp -= 10;
                            chars[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        chars[myTurn].AllTargetAttack(enemys, 3);

                        if (!chars[myTurn].SetAttack && !attackTrigger)
                        {
                            StartCoroutine(ChangeState(BattleState.allTargetSkillCheck, 1.5f));
                            attackTrigger = true;
                        }

                        break;
                    case BattleState.allTargetSkillCheck:
                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("Enemy Dead a {0}'s Attack", chars[myTurn].charName), 1);
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


                            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                            {
                                trigger = false;
                                state = BattleState.changePlayer;
                            }
                            else
                            {
                                if(trigger)
                                {
                                    battleInfoText.text = "Win!!!";
                                    trigger = false;
                                }

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

                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0} Damage to {1}", chars[myTurn].attackDamage * 2f, targetEnemy.charName),1);
                            gameData[currentPlayerNumber].playerCurrenMp -= 10;
                            chars[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        chars[myTurn].Attack(targetEnemy, 2, 2);
                        if (!chars[myTurn].SetAttack && !attackTrigger)
                        {
                            StartCoroutine(ChangeState(BattleState.Check, 1.5f));
                            attackTrigger = true;
                        }
                        break;
                    case BattleState.useHeal:
                        if (!trigger)
                        {
                            gameData[currentPlayerNumber].playerCurrenMp -= 10;
                            chars[myTurn].SetHeal = true;
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0} HP Recovery", (gameData[currentPlayerNumber].skillDamage)),1);

                            gameData[currentPlayerNumber].playerCurrentHp += gameData[currentPlayerNumber].skillDamage;
                            if (gameData[currentPlayerNumber].playerCurrentHp >= gameData[currentPlayerNumber].playerMaxHp)
                                gameData[currentPlayerNumber].playerCurrentHp = gameData[currentPlayerNumber].playerMaxHp;

                                Transform effect = Instantiate(chars[myTurn].effectPrefabs[0], chars[myTurn].transform).transform;
                            effect.transform.position = new Vector3(chars[myTurn].transform.position.x,
                                chars[myTurn].transform.position.y+0.5f,
                                chars[myTurn].transform.position.z);
                            trigger = true;

                            StartCoroutine(ChangeState(BattleState.changePlayer, 1.5f));

                        }
                        break;
                    case BattleState.targetselect:
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText("Select a Target",1);
                            trigger = true;
                        }
                        PlyerInfo();
                        target = SelectEnemy();
                        if (target != null)
                        {
                            targetEnemy = target.gameObject.GetComponent<EnemyManager>();
                            state = BattleState.attack;
                            trigger = false;
                        }
                        break;
                    case BattleState.attack:

                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0} Damage to {1}", gameData[currentPlayerNumber].playerAttackDamage, targetEnemy.charName), 1);
                            chars[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        chars[myTurn].Attack(targetEnemy,1, 1);
                        if (!chars[myTurn].SetAttack && !attackTrigger)
                        {
                            attackTrigger = true;
                            StartCoroutine(ChangeState(BattleState.Check, 1.5f));
                        }

                        break;
                    case BattleState.Check:
                        if (targetEnemy.currentHp <= 0)
                        {
                            if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                            {
                                chars = chars.Where(x => null != x).ToArray();
                                trigger = false;
                                state = BattleState.changePlayer;
                            }
                            else
                            {
                                if(!trigger)
                                {
                                    battleInfoText.text = "";
                                    battleInfoText.DOText("Win!!!", 1);
                                    trigger = true;
                                }

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
                                if (Input.GetMouseButtonDown(0))
                                {
                                    GameManager.instance.SetPanel(true);

                                    gameData[0].currentGold = gameData[0].currentGold + (gold * gameData[0].currentPlayerNumber);
                                    for (int i = 0; i < players.Length; i++)
                                    {

                                        gameData[i].PlayerCurrendExp += gold + 10;
                                        if (gameData[i].PlayerCurrendExp >= gameData[i].PlayerMaxExp)
                                        {
                                            gameData[i].PlayerCurrendExp = 0;
                                            gameData[i].playerLevel++;
                                            gameData[i].playerAttackDamage += 2;
                                            gameData[i].skillDamage += 2;

                                        }
                                    }
                                    trigger = false;
                                    state = BattleState.Win;
                                }

                            }
                        }
                        else
                        {
                            trigger = false;
                            state = BattleState.changePlayer;
                        }
                        break;
                    case BattleState.Win:
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("Get {0} Gold! Get {1} Exp!", gold * gameData[0].currentPlayerNumber, gold + 10), 1);
                            trigger = true;
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            GameManager.instance.SetPanel(false);
                            //AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, false);
                            GameManager.instance.ActionFade((int)GameManager.Scenes.FieldScene);
                        }
                        break;
                    case BattleState.changePlayer:

                        //순번이 캬라수를 넘어가면 순번0으로 초기화
                        if (myTurn +1 >= chars.Length)
                            myTurn = -1;

                            if (chars.Length > myTurn + 1)
                            {

                            if (chars[myTurn + 1].GetComponent<EnemyManager>())
                            {
                                    myTurn++;
                                    attackTrigger = true;
                                    state = BattleState.main;
                                    StateManager.instance.currentState = StateManager.BattleState.EnemyTurn;
                            }
                            else
                            {
                                    myTurn++;
                                    attackTrigger = true;
                                    state = BattleState.main;
                            }
                            attackTrigger = false;

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
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0}'s Turn", chars[myTurn].charName), 1);
                            trigger = true;
                            StartCoroutine(ChangeState(BattleState.targetselect, 1.5f));
                        }
                        break;
                    case BattleState.targetselect:
                        players = SelectPlayer(); //데이터 기준으로
                        target = players[0].GetComponent<Collider2D>();
        
                        if (target != null)
                        {
                            targetPlayer = target.gameObject.GetComponent<PlayerManager>();
                            state = BattleState.attack;
                        }
                        break;
                    case BattleState.attack:

                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0}'s got {1} Damage", targetPlayer.charName, chars[myTurn].attackDamage), 1);
                            chars[myTurn].SetAttack = true;
                            trigger = true;
                        }
                        chars[myTurn].Attack(targetPlayer);
                        if (!chars[myTurn].SetAttack && !attackTrigger)
                        {
                            attackTrigger = true;
                            StartCoroutine(ChangeState(BattleState.Check, 1.0f));
                        }

                        break;
                    case BattleState.Check:
                        if (targetPlayer.gameData.playerCurrentHp <= 0)
                        {
                            chars = chars.Where(x => null != x).ToArray(); //null 배열만 지움
                            //if(!trigger)
                            //{
                            //    battleInfoText.text = "";
                            //    battleInfoText.DOText(string.Format("{0}'s Attack {1}'s Dead", chars[myTurn].charName, targetPlayer.charName), 1);
                            //    trigger = true;
                            //}
                            if (GameObject.FindGameObjectsWithTag("Player").Length > 0)
                            {
                                trigger = false;
                                state = BattleState.changePlayer;
                            }
                            else
                            {
                                if(!trigger)
                                {
                                    battleInfoText.text = "";
                                    battleInfoText.text = "Lose...";
                                    trigger = true;
                                    StartCoroutine(ChangeState(BattleState.Lose, 1.5f));
                                }
                            }
                        }
                        else
                        {
                            trigger = false;
                            state = BattleState.changePlayer;
                        }
                        break;
                    case BattleState.Lose:
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText("Lost 100 Gold and EXP...", 1);
                            GameManager.instance.SetPanel(false);
                            trigger = true;
                        }
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
                            for(int i = 0; i < gameData.Length; i++)
                            {
                                gameData[i].PlayerCurrendExp = 0;
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
                            if (chars[myTurn + 1].GetComponent<EnemyManager>())
                            {
                                    myTurn++;
                                    trigger = false;
                                    state = BattleState.main; 
                            }
                            else
                            {
                                    myTurn++;
                                    trigger = false;
                                    state = BattleState.main;
                                    StateManager.instance.currentState = StateManager.BattleState.PlayerTurn;
                            }
                        }
                        break;
                }
                break;
            }
    }


    IEnumerator ChangeState(BattleState state, float time, StateManager.BattleState mainstate)
    {
        yield return new WaitForSeconds(time);
        trigger = false;
        StateManager.instance.currentState = mainstate;
        this.state = state;
    }
    IEnumerator ChangeState(BattleState state, float time)
    {
        yield return new WaitForSeconds(time);
        trigger = false;
        attackTrigger = false;
        this.state = state;
    }
    void Init()
    {
        battleInfoText = battleInfoImage.GetComponentInChildren<Text>();
        buttonInfoText = buttonInfoImage.GetComponentInChildren<Text>();
        EnemyInfoText = EenmyInfoImage.GetComponentInChildren<Text>();
        PlayerInfoText = PlayerInfoImage.GetComponentInChildren<Text>();

        buttonsParent.gameObject.SetActive(false);
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
        if (AttackButtonCol)
        {
            buttonsImage[0] = AttackButtonCol.GetComponent<Image>();
            buttonsImage[0].transform.localScale = new Vector3(1.1f, 1.1f,1f);
            if (buttonsImage[1])
                buttonsImage[1].transform.localScale = Vector3.one;
            buttonInfoImage.gameObject.SetActive(true);
            buttonInfoText.text = string.Format("{0} Damage One Enemy", gameData[currentPlayerNumber].playerAttackDamage);
        }
        else if (SkillButtonCol)
        {
            buttonsImage[1] = SkillButtonCol.GetComponent<Image>();
            buttonsImage[1].transform.localScale = new Vector3(1.1f,1.1f,1f);
            if(buttonsImage[0])
                buttonsImage[0].transform.localScale = Vector3.one;

            buttonInfoImage.gameObject.SetActive(true);
                buttonInfoText.text = "Open the SkillList";

        }
        else if(buttonsImage[0] && buttonsImage[1] )
        {
            buttonsImage[0].transform.localScale = Vector3.one;
            buttonsImage[1].transform.localScale = Vector3.one;

            buttonInfoImage.gameObject.SetActive(false);
        }


        buttonInfoImage.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + 350f,
                Input.mousePosition.y, -Camera.main.transform.position.z));
    }

    public void SkillButtonInfo()
    {

        Collider2D Skill1ButtonCol = MouseManager.instance.MouseRayCast("Skill1");
        Collider2D Skill2ButtonCol = MouseManager.instance.MouseRayCast("Skill2");
        Collider2D Skill3ButtonCol = MouseManager.instance.MouseRayCast("Skill3");

        if (Skill1ButtonCol)
        {
            skillButtonsImages[0] = Skill1ButtonCol.GetComponent<Image>();
            skillButtonsImages[0].transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            if (skillButtonsImages[1])
                skillButtonsImages[1].transform.localScale = Vector3.one;
            if (skillButtonsImages[2])
                skillButtonsImages[2].transform.localScale = Vector3.one;
            buttonInfoImage.gameObject.SetActive(true);
            buttonInfoText.text = string.Format("MP -10\n {0} Damage One Enemy", gameData[currentPlayerNumber].playerAttackDamage * 2);

        }
        else if (Skill2ButtonCol)
        {
            skillButtonsImages[1] = Skill2ButtonCol.GetComponent<Image>();
            skillButtonsImages[1].transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            if (skillButtonsImages[0])
                skillButtonsImages[0].transform.localScale = Vector3.one;
            if (skillButtonsImages[2])
                skillButtonsImages[2].transform.localScale = Vector3.one;

            buttonInfoImage.gameObject.SetActive(true);
                buttonInfoText.text = string.Format("MP -10\n {0} Damage All Enemies", gameData[currentPlayerNumber].playerAttackDamage);

        }
        else if (Skill3ButtonCol)
        {
            skillButtonsImages[2] = Skill3ButtonCol.GetComponent<Image>();
            skillButtonsImages[2].transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            if (skillButtonsImages[0])
                    skillButtonsImages[0].transform.localScale = Vector3.one;
            if (skillButtonsImages[1])
                skillButtonsImages[1].transform.localScale = Vector3.one;


            buttonInfoText.text = string.Format("MP -10\n {0} HP Self Healing", gameData[currentPlayerNumber].skillDamage);
            buttonInfoImage.gameObject.SetActive(true);
        }
        else if (skillButtonsImages[0] && skillButtonsImages[1] && skillButtonsImages[2])
        {
            skillButtonsImages[0].transform.localScale = Vector3.one;
            skillButtonsImages[1].transform.localScale = Vector3.one;
            skillButtonsImages[2].transform.localScale = Vector3.one;

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
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
        trigger = false;
        state = BattleState.targetselect;
    }

    public void SkillClick()
    {
        buttonsParent.SetActive(false);
        skillButtons.SetActive(true);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
        state = BattleState.SkillList;
    }
    void SkillReturn()
    {
        buttonsParent.SetActive(true);
        skillButtons.SetActive(false);
        arrowImage.gameObject.SetActive(true);
        buttonInfoImage.gameObject.SetActive(true);
        trigger = false;

        state = BattleState.skillselect;
    }

    public void ChoiceSkill1()
    {
        buttonsParent.SetActive(false);
        skillButtons.SetActive(false);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
        trigger = false;

        state = BattleState.oneTargetSkill;
    }
    public void ChoiceSkill2()
    {
        buttonsParent.SetActive(false);
        skillButtons.SetActive(false);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
        trigger = false;

        state = BattleState.allTargetSkill;
    }
    public void ChoiceSkill3()
    {
        buttonsParent.SetActive(false);
        skillButtons.SetActive(false);
        trigger = false;

        state = BattleState.Heal;
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
    }
}
