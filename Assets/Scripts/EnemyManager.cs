using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using static EnemyManager;
using static PlayerManager;

public class EnemyManager : MonoBehaviour
{
    public enum Enemy { Skul, Mumi, Mumis }
    public Enemy enemyType;
    SpriteRenderer spriter;
    public int attackNumber;
    public string enemyName;
    public float currentHp;
    public float maxHp;
    public float attackDamage;
    bool hpbarTrigger;
    public Slider hpBar;
    EnemyManager enemy;
    Slider hpbar;
    Animator animator;
    public bool SetAttack;
    public bool SetHit;
    public bool SetDead;
    public float speed = 10f;
    public bool attackTrigger;
    public Vector3 dir;
    public GameObject AttackPrefabs;
    public GameObject dustPrefabs;
    bool AnimationTrigger;
    float dustTimer;
    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        enemyName = enemyType.ToString();
        currentHp = maxHp;

        hpbar = Instantiate(hpBar, GameObject.Find("WorldCanvas").transform);

        hpbar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y+0.8f, transform.position.z));

        hpbar.gameObject.SetActive(false);
    }
    // Update is called once per frame
    IEnumerator AttackAnimation(PlayerManager player)
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
    public void Attack(PlayerManager player)
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
                            transform.Translate(new Vector3(dir.x, dir.y, dir.z) * Time.deltaTime * speed);
                        }
                        else
                        {
                            StartCoroutine(AttackAnimation(player));
                        }
                    }


                    if (attackTrigger)
                    {
                        if (transform.position.x < 6f)
                        {
                            transform.localScale = Vector3.one;
                            animator.SetTrigger("Move");
                            transform.Translate(new Vector3(-dir.x, -dir.y, dir.z) * Time.deltaTime * speed);
                        }
                        else
                        {
                            transform.localScale = new Vector3(-1f,1f,1f);
                            transform.position = new Vector3(6f, transform.position.y, 0f);
                            SetAttack = false;
                            attackTrigger = false;
                            AnimationTrigger = false;
                            animator.SetTrigger("Idle");

                        }
                    }

                }
        



    }
    void Update()
    {
        dustTimer += Time.deltaTime;
        if (transform.position.x != 6 && dustTimer > 0.5f)
        {
            Transform effect = Instantiate(dustPrefabs, transform).transform;
            effect.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, 0f);

            dustTimer = 0;
        }

        if (!SetAttack)
        {
            animator.SetTrigger("Idle");
        }
        if (SetDead)
        {
            StartCoroutine(DeadAnimation());
        }
        if(SetHit)
        {
            animator.SetTrigger("Spell");
            SetHit = false;
        }


        hpbar.value = currentHp / maxHp;
        OpenHpBar();

        //슬라이더 밸류 설정
    }
    IEnumerator DeadAnimation()
    {
        animator.SetTrigger("Die");
        SetDead = false;
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if (hpbar != null)
            hpbar.gameObject.SetActive(false);
    }

    void OpenHpBar()
    {
        Collider2D Col = MouseManager.instance.MouseRayCast("Enemy");
        if(Col)
        {
            enemy = Col.gameObject.GetComponent<EnemyManager>();
            enemy.hpbar.gameObject.SetActive(true);
        }
        else if(enemy != null)
            enemy.hpbar.gameObject.SetActive(false);

    }
}
