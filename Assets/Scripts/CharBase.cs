using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using static EnemyManager;
using static FieldEnemy;

public class CharBase : MonoBehaviour
{
    [Header("--DOTween--")]
    public Ease charEase;

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
    public Text damageText;

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
        dir = transform.position;

    }
    // Update is called once per frame
    virtual protected void Update()
    {
        adjustAnimation();
        hpBar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x,
            transform.position.y + 1f, transform.position.z));

    }

    private void OnDisable()
    {
        if (hpBar != null)
            hpBar.gameObject.SetActive(false);

        Destroy(gameObject);
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

    virtual public void Attack(EnemyManager enemy, int effectnum, int type) { }

    virtual public void Attack(PlayerManager player) { }

    protected void CreateDamage(CharBase target, float damagenum)
    {
        Text damage = Instantiate(damageText, GameObject.Find("WorldCanvas").transform);
        damage.transform.position = Camera.main.WorldToScreenPoint(new Vector3(target.transform.position.x + 1.3f,
            target.transform.position.y, target.transform.position.z));
        damage.text = "-" + damagenum.ToString();
    }



    public void SetEffect(Transform pos, int effectnum, int type)
    {
        Transform effect = Instantiate(effectPrefabs[effectnum], pos).transform;
        effect.transform.localScale = effect.transform.localScale * type;
    }
}
