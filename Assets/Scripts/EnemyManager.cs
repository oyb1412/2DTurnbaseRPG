using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using static EnemyManager;
using static PlayerManager;

public class EnemyManager : MonoBehaviour
{
    [Header("--Animation--")]
    Animator animator;
    public bool SetAttack;
    public bool SetHit;
    public bool SetDead;
    public bool attackTrigger;
    bool AnimationTrigger;

    [Header("--UI--")]
    Slider hpBar;
    public Slider hpBarPrefabs;

    [Header("--Info--")]
    public int attackOrder;
    public string enemyName;
    public float currentHp;
    public float maxHp;
    public float attackDamage;
    public float moveSpeed = 10f;
    public Vector3 dir;
    EnemyManager enemy;
    public enum Enemy { Skul, Mumi, Mumis }
    public Enemy enemyType;

    [Header("--Effect--")]
    public GameObject AttackPrefabs;
    public GameObject dustPrefabs;
    float dustTimer;

    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Init();
    }
    // Update is called once per frame

    void Update()
    {
        craeteDustEffect();
        adjustAnimation();
        hpBar.value = currentHp / maxHp;
        ShowHpBar();
    }

    private void OnDisable()
    {
        if (hpBar != null)
            hpBar.gameObject.SetActive(false);
    }

    void Init()
    {
        enemyName = enemyType.ToString();
        currentHp = maxHp;

        hpBar = Instantiate(hpBarPrefabs, GameObject.Find("WorldCanvas").transform);

        hpBar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z));

        hpBar.gameObject.SetActive(false);
    }
    void adjustAnimation()
    {
        if (!SetAttack)
        {
            animator.SetTrigger("Idle");
        }
        if (SetDead)
        {
            StartCoroutine(DeadAnimation());
        }
        if (SetHit)
        {
            animator.SetTrigger("Spell");
            SetHit = false;
        }
    }
    void craeteDustEffect()
    {
        dustTimer += Time.deltaTime;
        if (transform.position.x != 6 && dustTimer > 0.5f)
        {
            Transform effect = Instantiate(dustPrefabs, transform).transform;
            effect.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, 0f);

            dustTimer = 0;
        }
    }
    IEnumerator DeadAnimation()
    {
        animator.SetTrigger("Die");
        SetDead = false;
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    IEnumerator MeleeAttackAnimation(PlayerManager player)
    {
        if (!AnimationTrigger)
        {
            animator.SetTrigger("Attack");
            Instantiate(AttackPrefabs, player.transform);

            if (player.currentHp <= 0)
                player.SetDead = true;
            else
                player.SetHit = true;
        }
        AnimationTrigger = true;

        yield return new WaitForSeconds(0.5f);
        attackTrigger = true;
    }
    public void MeleeAttackAndMove(PlayerManager player)
    {
        if (SetAttack)
        {

            if (!attackTrigger)
            {
                if (transform.position.x >= 6)
                    dir = (player.transform.position - transform.position).normalized;

                if (transform.position.x > -5.5f)
                {
                    animator.SetTrigger("Move");
                    transform.Translate(new Vector3(dir.x, dir.y, dir.z) * Time.deltaTime * moveSpeed);
                }
                else
                {
                    StartCoroutine(MeleeAttackAnimation(player));
                }
            }


            if (attackTrigger)
            {
                if (transform.position.x < 6f)
                {
                    transform.localScale = Vector3.one;
                    animator.SetTrigger("Move");
                    transform.Translate(new Vector3(-dir.x, -dir.y, dir.z) * Time.deltaTime * moveSpeed);
                }
                else
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                    transform.position = new Vector3(6f, transform.position.y, 0f);
                    SetAttack = false;
                    attackTrigger = false;
                    AnimationTrigger = false;
                    animator.SetTrigger("Idle");

                }
            }

        }




    }
    void ShowHpBar()
    {
        Collider2D Col = MouseManager.instance.MouseRayCast("Enemy");
        if(Col)
        {
            enemy = Col.gameObject.GetComponent<EnemyManager>();
            enemy.hpBar.gameObject.SetActive(true);
        }
        else if(enemy != null)
            enemy.hpBar.gameObject.SetActive(false);

    }
}
