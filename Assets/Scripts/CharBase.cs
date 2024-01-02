using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using static FieldEnemy;

public class CharBase : MonoBehaviour
{
    [Header("--Info--")]
    public int attackOrder;
    public string charName;
    public float currentHp;
    public float maxHp;
    public float attackDamage;
    public float speed;
    public Vector3 dir;

    [Header("--Animation--")]
    protected Animator animator;
    public bool SetAttack;
    public bool SetHit;
    public bool SetDead;
    public bool SetHeal;
    public bool attackTrigger;
    protected bool AnimationTrigger;

    [Header("--GameData--")]
    public GameData gameData;//고정

    [Header("--UI--")]
    public Slider hpBar;
    public Slider hpBarPrefabs;

    [Header("--Effect--")]
    public GameObject dustPrefabs;
    public GameObject[] effectPrefabs;//고정
    protected float dustTimer;

    protected CharBase charObj;

    // Start is called before the first frame update
    virtual protected void Awake()
    {
        speed = 10f;
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        CommonInit();
    }
    // Update is called once per frame
    virtual protected void Update()
    {
        adjustAnimation();
    }

    private void OnDisable()
    {
        if (hpBar != null)
            hpBar.gameObject.SetActive(false);
    }

    protected void craeteDustEffect(float pos)
    {
        dustTimer += Time.deltaTime;
        if (transform.position.x != pos && dustTimer > 0.5f)
        {
            Transform effect = Instantiate(dustPrefabs, transform).transform;
            effect.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, 0f);

            dustTimer = 0;
        }
    }

    private IEnumerator DeadAnimation()
    {
        animator.SetTrigger("Die");
        SetDead = false;
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    private void adjustAnimation()
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

    virtual protected void Init() { }

    protected void ShowHpBar(Collider2D col)
    {
        if (col)
        {
            charObj = col.gameObject.GetComponent<CharBase>();
            charObj.hpBar.gameObject.SetActive(true);
        }
        else if (charObj != null)
            charObj.hpBar.gameObject.SetActive(false);
    }

    private void CommonInit()
    {
        hpBar = Instantiate(hpBarPrefabs, GameObject.Find("WorldCanvas").transform);
        hpBar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));
        hpBar.gameObject.SetActive(false);
    }

    virtual public void AllTargetAttack(EnemyManager[] enemy, int effectnum) { }

    virtual public void Attack(EnemyManager enemy, int effectnum) { }

    virtual public void MeleeAttackAndMove(PlayerManager player) { }


}
