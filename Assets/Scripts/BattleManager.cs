using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 전투 턴 관리
/// </summary>
public class BattleManager : MonoBehaviour
{
    #region Variable

    //전투 상태
    public enum BattleState {
        main, skillselect, targetselect, attack, Check, changePlayer, Win, Lose,
        oneTargetSkill, allTargetSkill, Heal, allTargetSkillAttack, oneTargetSkillAttack, useHeal,
        allTargetSkillCheck, SkillList
    }

    [Header("--Player--")]
    //모든 플레이어 목록
    private PlayerManager[] players;
    //적에게 타겟이 된 플레이어
    private PlayerManager targetPlayer;
    //마우스 enter된 플레이어
    private PlayerManager player;

    [Header("--Enemy--")]
    //모든 애너미 목록
    private EnemyManager[] enemys;
    //공격 타겟이 된 애너미
    private EnemyManager targetEnemy;
    //마우스 enter된 애너미
    private EnemyManager enemy;

    [Header("--Char--")]
    //모든 유닛 목록
    private CharBase[] chars;

    [Header("--UI--")]
    //스킬 목록 open 버튼
    [SerializeField] private GameObject skillButtons;
    //현재 턴인 유닛을 가리키는 화살표
    [SerializeField] private Image arrowImage;
    //마우스 enter된 유닛 정보 판넬
    [SerializeField] private Image battleInfoImage;
    //스킬 버튼
    [SerializeField] private Button skillButton;
    //적 정보 판넬
    [SerializeField] private Image EnemyInfoImage;
    //플레이어 정보 판넬
    [SerializeField] private Image PlayerInfoImage;
    //커서 아이콘
    [SerializeField] private Texture2D cursurIcon;
    //모든 버튼 부모 오브젝트
    [SerializeField] private GameObject buttonsParent;
    //마우스 enter상태인 버튼 정보 판넬
    [SerializeField] private Image buttonInfoImage;

    //각 스킬 버튼 이미지
    private Image[] skillButtonsImages;
    //각 버튼 이미지
    private Image[] buttonsImage = new Image[2];
    //버튼 정보 출력 텍스트
    private Text buttonInfoText;
    //전투 정보 출력 텍스트
    private Text battleInfoText;
    //적 정보 출력 텍스트
    private Text EnemyInfoText;
    //플레이어 정보 출력 텍스트
    private Text PlayerInfoText;
    //현재 전투 상태
    private BattleState state;
    //획득 골드
    private int gold;
    //전투 순번에 맞는 턴
    private int turn;
    //적 사망 횟수
    private int deadCount;
    //마우스 enter확인용 콜라이더
    private Collider2D target;
    //state변경 가능 여부
    private bool trigger;
    //내 턴에 공격 여부
    private bool attackTrigger;
    //현재 공격중인 플레이어의 턴
    private int currentPlayerNumber;

    [Header("--GameData--")]
    [SerializeField] private GameData[] gameData;

    #endregion

    #region InitMethod
    private void Awake()
    {
       AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, true);
       state = BattleState.main;
    }
    void Start()
    {
        Init();
    }

    void Init() {
        skillButtonsImages = new Image[gameData.Length];
        for (int i = 0; i < gameData.Length; i++)
            skillButtonsImages[i] = skillButtons.GetComponentsInChildren<Image>()[i * 2];

        battleInfoText = battleInfoImage.GetComponentInChildren<Text>();
        buttonInfoText = buttonInfoImage.GetComponentInChildren<Text>();
        EnemyInfoText = EnemyInfoImage.GetComponentInChildren<Text>();
        PlayerInfoText = PlayerInfoImage.GetComponentInChildren<Text>();

        buttonsParent.gameObject.SetActive(false);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
    }
    #endregion

    #region UpdateMethod
    private void Update()
    {
        if (!GameManager.instance.IsPlaying)
            return;

            //현재 턴 상태에 맞는 행동 실행
            switch (StateManager.instance.CurrentState)
            {
                //전투 시작 시
                case StateManager.BattleState.StartTurn:

                //모든 유닛을 공격순번 순으로 배열에 대입
                chars = RealizeChar();
                battleInfoImage.gameObject.SetActive(true);
                    state = BattleState.main;
                //첫 공격예정 유닛의 턴
                    if (chars[0].GetComponentInChildren<PlayerManager>())
                        StateManager.instance.CurrentState = StateManager.BattleState.PlayerTurn;
                     else
                        StateManager.instance.CurrentState = StateManager.BattleState.EnemyTurn;
                
                break;

                //플레이어 턴
                case StateManager.BattleState.PlayerTurn:
                switch (state)
                {
                    case BattleState.main:
                        players = RealizePlayer();
                        enemys = RealizeEnemy();

                        for (int i = 0;i<players.Length; i++)
                        {
                            if (chars[turn].CharName == gameData[i].PlayerName)
                            {
                                currentPlayerNumber = i;
                                break;
                            }
                        }
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0}'s Turn", chars[turn].CharName), 1);
                            StartCoroutine(Co_ChangeState(BattleState.skillselect, 1.5f));
                            trigger = true;
                        }
                        break;
                    case BattleState.skillselect:
                        ButtonInfo();
                        if (gameData[currentPlayerNumber].PlayerCurrentMp < 10)
                        {
                            skillButton.interactable = false;
                        }

                        buttonsParent.gameObject.SetActive(true);
                        arrowImage.gameObject.SetActive(true);

                        buttonsParent.transform.position = new Vector3(chars[turn].transform.position.x+3.5f,
                            chars[turn].transform.position.y+1.5f,
                            chars[turn].transform.position.z);

                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText("Select Your Action",1);
                            trigger = true;
                        }

                        break;
                    case BattleState.SkillList:
                        SkillButtonInfo();
                        skillButtons.transform.position = new Vector3(chars[turn].transform.position.x + 3.5f,
                            chars[turn].transform.position.y + 1.5f,
                            chars[turn].transform.position.z);
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText("Select your Skill", 1);
                            trigger = true;
                        }
                        if (Input.GetMouseButtonDown(1))
                        {
                            AudioManager.instance.PlayerSfx(AudioManager.Sfx.Select);
                            SkillReturn();
                        }
                        break;
                    case BattleState.allTargetSkill:

                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("MP -10 {0} Damage All Enemies", gameData[currentPlayerNumber].PlayerAttackDamage), 1);
                            StartCoroutine(Co_ChangeState(BattleState.allTargetSkillAttack, 1.5f));

                            trigger = true;
                        }
                        break;
                    case BattleState.oneTargetSkill:
                        target = SelectEnemy();
                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("MP -10 {0} Damage One Enemy", gameData[currentPlayerNumber].PlayerAttackDamage * 2), 1);
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
                            battleInfoText.DOText(string.Format("MP -10 {0} HP Self Healing", gameData[currentPlayerNumber].SkillDamage), 1);
                            trigger = true;
                        }
                        ChangeState(BattleState.useHeal);
                        break;
                    case BattleState.allTargetSkillAttack:

                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0} Damage All Enemies", gameData[currentPlayerNumber].PlayerAttackDamage), 1);
                            gameData[currentPlayerNumber].PlayerCurrentMp -= 10;
                            chars[turn].SetAttack = true;
                            trigger = true;
                        }
                        chars[turn].AllTargetAttack(enemys, 3);

                        if (!chars[turn].SetAttack && !attackTrigger)
                        {
                            StartCoroutine(Co_ChangeState(BattleState.allTargetSkillCheck, 1.5f));
                            attackTrigger = true;
                        }

                        break;
                    case BattleState.allTargetSkillCheck:
                        if (!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("Enemy Dead a {0}'s Attack", chars[turn].CharName), 1);
                            for (int i = 0; i < enemys.Length; i++)
                            {
                                if (enemys[i].CurrentHp <= 0)
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
                            battleInfoText.DOText(string.Format("{0} Damage to {1}", chars[turn].AttackDamage * 2f, targetEnemy.CharName),1);
                            gameData[currentPlayerNumber].PlayerCurrentMp -= 10;
                            chars[turn].SetAttack = true;
                            trigger = true;
                        }
                        chars[turn].Attack(targetEnemy, 2, 2);
                        if (!chars[turn].SetAttack && !attackTrigger)
                        {
                            StartCoroutine(Co_ChangeState(BattleState.Check, 1.5f));
                            attackTrigger = true;
                        }
                        break;
                    case BattleState.useHeal:
                        if (!trigger)
                        {
                            AudioManager.instance.PlayerSfx(AudioManager.Sfx.Heal);

                            gameData[currentPlayerNumber].PlayerCurrentMp -= 10;
                            chars[turn].SetHeal = true;
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0} HP Recovery", (gameData[currentPlayerNumber].SkillDamage)),1);

                            gameData[currentPlayerNumber].PlayerCurrentHp += gameData[currentPlayerNumber].SkillDamage;
                            if (gameData[currentPlayerNumber].PlayerCurrentHp >= gameData[currentPlayerNumber].PlayerMaxHp)
                                gameData[currentPlayerNumber].PlayerCurrentHp = gameData[currentPlayerNumber].PlayerMaxHp;

                                Transform effect = Instantiate(chars[turn].EffectPrefabs[0], chars[turn].transform).transform;
                            effect.transform.position = new Vector3(chars[turn].transform.position.x,
                                chars[turn].transform.position.y+0.5f,
                                chars[turn].transform.position.z);
                            trigger = true;

                            StartCoroutine(Co_ChangeState(BattleState.changePlayer, 1.5f));

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
                            battleInfoText.DOText(string.Format("{0} Damage to {1}", gameData[currentPlayerNumber].PlayerAttackDamage, targetEnemy.CharName), 1);
                            chars[turn].SetAttack = true;
                            trigger = true;
                        }
                        chars[turn].Attack(targetEnemy,1, 1);
                        if (!chars[turn].SetAttack && !attackTrigger)
                        {
                            attackTrigger = true;
                            StartCoroutine(Co_ChangeState(BattleState.Check, 1.5f));
                        }

                        break;
                    case BattleState.Check:
                        if (targetEnemy.CurrentHp <= 0)
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
                                    AudioManager.instance.PlayerSfx(AudioManager.Sfx.Select);

                                    GameManager.instance.SetPanel(true);

                                    gameData[0].CurrentGold = gameData[0].CurrentGold + (gold * gameData[0].CurrentPlayerNumber);
                                    for (int i = 0; i < players.Length; i++)
                                    {

                                        gameData[i].PlayerCurrendExp += gold + 10;
                                        if (gameData[i].PlayerCurrendExp >= gameData[i].PlayerMaxExp)
                                        {
                                            gameData[i].PlayerCurrendExp = gameData[i].PlayerMaxExp - gameData[i].PlayerCurrendExp;
                                            gameData[i].PlayerLevel++;
                                            gameData[i].PlayerAttackDamage += 2;
                                            gameData[i].SkillDamage += 2;

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
                            battleInfoText.DOText(string.Format("Get {0} Gold! Get {1} Exp!", gold * gameData[0].CurrentPlayerNumber, gold + 10), 1);
                            trigger = true;
                        }
                        if (Input.GetMouseButtonDown(0))
                        {

                            AudioManager.instance.PlayerSfx(AudioManager.Sfx.GoScene);

                            GameManager.instance.SetPanel(false);
                            AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, false);
                            GameManager.instance.ActionFade((int)GameManager.Scenes.FieldScene);
                        }
                        break;
                    case BattleState.changePlayer:

                        //순번이 캬라수를 넘어가면 순번0으로 초기화
                        if (turn +1 >= chars.Length)
                            turn = -1;

                            if (chars.Length > turn + 1)
                            {

                            if (chars[turn + 1].GetComponent<EnemyManager>())
                            {
                                    turn++;
                                    attackTrigger = true;
                                    state = BattleState.main;
                                    StateManager.instance.CurrentState = StateManager.BattleState.EnemyTurn;
                            }
                            else
                            {
                                    turn++;
                                    attackTrigger = true;
                                    state = BattleState.main;
                            }
                            attackTrigger = false;

                        }
                        break;
                }
                break;
                
                //애너미 턴
                case StateManager.BattleState.EnemyTurn:
                switch (state)
                {
                    case BattleState.main:
                        players = RealizePlayer();
                        enemys = RealizeEnemy();
                        if(!trigger)
                        {
                            battleInfoText.text = "";
                            battleInfoText.DOText(string.Format("{0}'s Turn", chars[turn].CharName), 1);
                            trigger = true;
                            StartCoroutine(Co_ChangeState(BattleState.targetselect, 1.5f));
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
                            battleInfoText.DOText(string.Format("{0}'s got {1} Damage", targetPlayer.CharName, chars[turn].AttackDamage), 1);
                            chars[turn].SetAttack = true;
                            trigger = true;
                        }
                        chars[turn].Attack(targetPlayer);
                        if (!chars[turn].SetAttack && !attackTrigger)
                        {
                            attackTrigger = true;
                            StartCoroutine(Co_ChangeState(BattleState.Check, 1.0f));
                        }

                        break;
                    case BattleState.Check:
                        if (targetPlayer.gameData.PlayerCurrentHp <= 0)
                        {
                            chars = chars.Where(x => null != x).ToArray(); //null 배열만 지움
                            
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
                                    StartCoroutine(Co_ChangeState(BattleState.Lose, 1.5f));
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
                            AudioManager.instance.PlayerSfx(AudioManager.Sfx.GoScene);

                            if (gameData[0].CurrentGold > 100)
                            {
                                gameData[0].CurrentGold -= 100;
                            }
                            if (gameData[0].CurrentGold <= 100)
                            {
                                gameData[0].CurrentGold = 0;
                            }
                            for(int i = 0; i < gameData.Length; i++)
                            {
                                gameData[i].PlayerCurrendExp = 0;
                                gameData[i].PlayerCurrentHp = gameData[i].PlayerMaxHp;
                            }
                            gameData[0].PlayerFieldPosition = new Vector2(-9.3f, -9.5f);
                            AudioManager.instance.PlayerBgm(AudioManager.Bgm.Battle, false);
                            GameManager.instance.ActionFade((int)GameManager.Scenes.FieldScene);
                        }
                        break;
                    case BattleState.changePlayer:

                        if (turn + 1 >= chars.Length)
                            turn = -1;
                        //공격 후 다음순번 플레이어 선택
                        if (chars.Length > turn + 1)
                        {
                            if (chars[turn + 1].GetComponent<EnemyManager>())
                            {
                                    turn++;
                                    trigger = false;
                                    state = BattleState.main; 
                            }
                            else
                            {
                                    turn++;
                                    trigger = false;
                                    state = BattleState.main;
                                    StateManager.instance.CurrentState = StateManager.BattleState.PlayerTurn;
                            }
                        }
                        break;
                }
                break;
            }
    }

    #endregion

    /// <summary>
    /// 일정 시간 후 상태 변경
    /// </summary>
    /// <param name="state">변경할 상태</param>
    /// <param name="time">시간</param>
    /// <returns></returns>
    private IEnumerator Co_ChangeState(BattleState state, float time)
    {
        yield return new WaitForSeconds(time);
        trigger = false;
        attackTrigger = false;
        this.state = state;
    }

    /// <summary>
    /// 마우스 클릭 시 상태 변경
    /// </summary>
    /// <param name="type">변경할 상태</param>
    private void ChangeState(BattleState type)
    {
        if (Input.GetMouseButtonDown(0))
        {
            AudioManager.instance.PlayerSfx(AudioManager.Sfx.Select);
            trigger = false;
            state = type;
        }
    }

    /// <summary>
    /// 적이 공격할 플레이어 순번
    /// </summary>
    /// <returns>체력이 낮은 순서대로 정렬된 플레이어</returns>
    private PlayerManager[] SelectPlayer()
    {
        //플레이어 배열 순서 변경(체력이 낮은 순)
        for (int i = 0; i < players.Length; i++)
        {
            if (i + 1 >= players.Length)
                break;

            if (players[i].CurrentHp > players[i + 1].CurrentHp)
            {
                PlayerManager save = new PlayerManager();
                save = players[i + 1];
                players[i + 1] = players[i];
                players[i] = save;
            }
        }

        return players;
    }

    #region ButtonEventMethod
    /// <summary>
    /// 마우스 enter시 플레이어 정보 출력
    /// </summary>
    private void PlyerInfo()
    {
        Collider2D playerCol = MouseManager.instance.MouseRayCast("Player");
        if (playerCol)
        {
            player = playerCol.gameObject.GetComponent<PlayerManager>();
            PlayerInfoImage.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x + 360f,
                Input.mousePosition.y, -Camera.main.transform.position.z));
            PlayerInfoText.text = player.CharName + "\n" + "HP : " + player.CurrentHp + " / " + player.MaxHp + "\n" + "Damage : " + player.AttackDamage;
            PlayerInfoImage.gameObject.SetActive(true);
            player.transform.localScale = new Vector3(1.1f, 1.1f, 1f); 
        }
        else if(player)
        {
            player.transform.localScale = Vector3.one;
            PlayerInfoImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 마우스 enter시 각 행동 정보 출력
    /// </summary>
    private void ButtonInfo()
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
            buttonInfoText.text = string.Format("{0} Damage One Enemy", gameData[currentPlayerNumber].PlayerAttackDamage);
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

    /// <summary>
    /// 마우스 enter 시 각 스킬 정보 출력
    /// </summary>
    private void SkillButtonInfo()
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
            buttonInfoText.text = string.Format("MP -10\n {0} Damage One Enemy", gameData[currentPlayerNumber].PlayerAttackDamage * 2);

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
                buttonInfoText.text = string.Format("MP -10\n {0} Damage All Enemies", gameData[currentPlayerNumber].PlayerAttackDamage);

        }
        else if (Skill3ButtonCol)
        {
            skillButtonsImages[2] = Skill3ButtonCol.GetComponent<Image>();
            skillButtonsImages[2].transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            if (skillButtonsImages[0])
                    skillButtonsImages[0].transform.localScale = Vector3.one;
            if (skillButtonsImages[1])
                skillButtonsImages[1].transform.localScale = Vector3.one;


            buttonInfoText.text = string.Format("MP -10\n {0} HP Self Healing", gameData[currentPlayerNumber].SkillDamage);
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

    /// <summary>
    /// 마우스 enter시 적 정보 출력
    /// </summary>
    /// <returns></returns>
    private Collider2D SelectEnemy()
    {
        Collider2D col = MouseManager.instance.MouseRayCast("Enemy");

        if (col)
        {
            enemy = col.gameObject.GetComponent<EnemyManager>();
            EnemyInfoImage.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x - 360f,
                Input.mousePosition.y, -Camera.main.transform.position.z));
            EnemyInfoText.text = enemy.CharName + "\n" + "HP : " + enemy.CurrentHp + " / " + enemy.MaxHp + "\n" + "Damage : " + enemy.AttackDamage;
            EnemyInfoImage.gameObject.SetActive(true);
            Cursor.SetCursor(cursurIcon, new Vector2(cursurIcon.width / 2f, cursurIcon.height / 2f), CursorMode.Auto);
            enemy.transform.localScale = new Vector3(-1.1f, 1.1f, 1f);
        }
        else if(enemy)
        {
            enemy.transform.localScale = new Vector3(-1f, 1f, 1f);
            EnemyInfoImage.gameObject.SetActive(false);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        }


        if (Input.GetMouseButtonDown(0) && col != null)
        {
            AudioManager.instance.PlayerSfx(AudioManager.Sfx.Select);

            EnemyInfoImage.gameObject.SetActive(false);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return col;
        }
        else
            return null;
    }
    #endregion

    #region ShuffleMethod
    /// <summary>
    /// 플레이어를 공격 순위에 맞게 재배열
    /// </summary>
    private PlayerManager[] RealizePlayer()
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

    /// <summary>
    /// 애너미를 공격 순위에 맞게 재배열
    /// </summary>
    private EnemyManager[] RealizeEnemy()
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

    /// <summary>
    /// 모든 유닛을 공격 순위에 맞게 재배열
    /// </summary>
    private CharBase[] RealizeChar()
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
    #endregion

    #region ClickEventMethod
    /// <summary>
    /// 어택 버튼 클릭(콜백으로 호출)
    /// </summary>
    public void AttackClick()
    {
        buttonsParent.SetActive(false);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
        trigger = false;
        state = BattleState.targetselect;
    }

    /// <summary>
    /// 스킬 버튼 클릭(콜백으로 호출)
    /// </summary>
    public void SkillClick()
    {
        buttonsParent.SetActive(false);
        skillButtons.SetActive(true);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
        state = BattleState.SkillList;
    }

    /// <summary>
    /// 스킬창에서 되돌아가기 클릭
    /// </summary>
    void SkillReturn()
    {
        buttonsParent.SetActive(true);
        skillButtons.SetActive(false);
        arrowImage.gameObject.SetActive(true);
        buttonInfoImage.gameObject.SetActive(true);
        trigger = false;

        state = BattleState.skillselect;
    }

    /// <summary>
    /// 스킬1 선택 클릭(콜백으로 호출)
    /// </summary>
    public void ChoiceSkill1()
    {
        buttonsParent.SetActive(false);
        skillButtons.SetActive(false);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
        trigger = false;

        state = BattleState.oneTargetSkill;
    }

    /// <summary>
    /// 스킬2 선택 클릭(콜백으로 호출)
    /// </summary>
    public void ChoiceSkill2()
    {
        buttonsParent.SetActive(false);
        skillButtons.SetActive(false);
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
        trigger = false;

        state = BattleState.allTargetSkill;
    }

    /// <summary>
    /// 스킬3 선택 클릭(콜백으로 호출)
    /// </summary>
    public void ChoiceSkill3()
    {
        buttonsParent.SetActive(false);
        skillButtons.SetActive(false);
        trigger = false;

        state = BattleState.Heal;
        arrowImage.gameObject.SetActive(false);
        buttonInfoImage.gameObject.SetActive(false);
    }
    #endregion
}
