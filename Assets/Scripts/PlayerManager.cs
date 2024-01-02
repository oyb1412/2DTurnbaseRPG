using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    [Header("--Info--")]
    public Player playerType;
    public float skillDamage;
    public float currentHp;
    public float maxHp;
    public float attackDamage;
    public int attackNumber;
    public string playerName;
    public float speed;
    PlayerManager player;
    public Vector3 dir;
    public enum Player { Wizard, Mini, Miho }

    [Header("--Animation--")]
    public bool SetAttack;
    public bool SetHit;
    public bool SetHeal;
    public bool SetDead;
    public bool attackTrigger;
    bool AnimationTrigger;
    Animator animator;
    public bool Idle;

    [Header("--UI--")]
    public Slider hpBarPrefabs;
    Slider hpbar;

    [Header("--GameData--")]
    public GameData gameData;

    [Header("--Effect-")]
    float dustTimer;
    public GameObject[] effectPrefabs;
    private void Awake()
    {
        Init();
    }
    private void Start()
    {
        StartInit();
    }
    private void Update()
    {
        AdjustAnimation();
        hpbar.value = currentHp / maxHp;
        ShowHpBar();
    }
    private void OnDisable()
    {
        if (hpbar != null)
            hpbar.gameObject.SetActive(false);
    }

    void StartInit()
    {
        playerName = playerType.ToString();

        hpbar = Instantiate(hpBarPrefabs, GameObject.Find("WorldCanvas").transform);

        hpbar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));
        hpbar.gameObject.SetActive(false);
    }
    IEnumerator AttackAnimation(EnemyManager enemy, int effectnum)
    {
        if (!AnimationTrigger)
        {
            animator.SetTrigger("Attack");
            SetEffect(enemy.transform, effectnum);
            enemy.currentHp -= gameData.playerAttackDamage * 2;
            if (enemy.currentHp <= 0)
                enemy.SetDead = true;
            else
                enemy.SetHit = true;
        }
        AnimationTrigger = true;

        yield return new WaitForSeconds(0.5f);
        attackTrigger = true;
    }
    public void AllTargetAttack(EnemyManager[] enemy, int effectnum) 
    {
        if (SetAttack)
        {
            for (int i = 0; i < enemy.Length; i++)
            {
                enemy[i].currentHp -=gameData.playerAttackDamage;

                if (enemy[i].currentHp <= 0)
                    enemy[i].SetDead = true;
                else
                    enemy[i].SetHit = true;


            }
            animator.SetTrigger("Fire");
            SetEffect(enemy[1].transform, effectnum);
        }

        SetAttack = false;

    }
    public void Attack(EnemyManager enemy, int effectnum)
    {
        switch (playerType)
        {
            case Player.Wizard:
                if (SetAttack)
                {
                    animator.SetTrigger("Fire");
                    SetEffect(enemy.transform, effectnum);
                    enemy.currentHp -= gameData.playerAttackDamage;
                    if (enemy.currentHp <= 0)
                        enemy.SetDead = true;
                    else
                        enemy.SetHit = true;
                    SetAttack = false;
                }
                break;
            case Player.Mini:
            case Player.Miho:
                if (SetAttack)
                {
                    if (!attackTrigger)
                    {
                        if(transform.position.x <= -6)
                            dir = (enemy.transform.position - transform.position).normalized;

                        if (transform.position.x < 5.5f)
                        {
                            animator.SetTrigger("Move");
                            transform.Translate(new Vector3(dir.x, dir.y, dir.z) * Time.deltaTime * speed);
                        }
                        else
                        {
                            StartCoroutine(AttackAnimation(enemy,effectnum));
                        }
                    }


                    if (attackTrigger)
                    {
                        if (transform.position.x > -6f)
                        {
                            transform.localScale = new Vector3(-1f, 1f, 1f);
                            animator.SetTrigger("Move");
                            transform.Translate(new Vector3(-dir.x, -dir.y, dir.z) * Time.deltaTime * speed);
                        }
                        else
                        {
                            transform.localScale = Vector3.one;
                            transform.position = new Vector3(-6f, transform.position.y, 0f);
                            SetAttack = false;
                            attackTrigger = false;
                            AnimationTrigger = false;
                            animator.SetTrigger("Idle");

                        }
                    }

                }
                break;
        }
        
    

    }

    void CreateDustEffect()
    {
        dustTimer += Time.deltaTime;
        if (transform.position.x != -6 && dustTimer > 0.5f)
        {
            Transform effect = Instantiate(effectPrefabs[3], transform).transform;
            effect.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, 0f);

            dustTimer = 0;
        }
    }

    void AdjustAnimation()
    {
        CreateDustEffect();
        if (SetDead)
        {
            StartCoroutine(DeadAnimation());
        }

        if (!SetAttack)
        {
            animator.SetTrigger("Idle");
        }

        if (SetHit)
        {
            animator.SetTrigger("Spell");
            SetHit = false;
        }

        if (SetHeal)
        {
            animator.SetTrigger("Attack");
            SetHeal = false;
        }
    }
    void Init()
    {
        Idle = true;
        animator = GetComponent<Animator>();
        skillDamage = gameData.skillDamage;
        currentHp = gameData.playerCurrentHp;
        maxHp = gameData.playerMaxHp;
        attackDamage = gameData.playerAttackDamage;
    }

    void ShowHpBar()
    {
        Collider2D Col = MouseManager.instance.MouseRayCast("Player");
        if (Col)
        {
            player = Col.gameObject.GetComponent<PlayerManager>();
            player.hpbar.gameObject.SetActive(true);
        }
        else if (player != null)
            player.hpbar.gameObject.SetActive(false);

    }
    IEnumerator DeadAnimation()
    {
        animator.SetTrigger("Die");
        SetDead = false;
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    public void SetEffect(Transform pos, int effectnum)
    {
        Instantiate(effectPrefabs[effectnum], pos);
    }
}
