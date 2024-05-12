using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾�, �ֳʹ� �θ� Ŭ����
/// </summary>
public class CharBase : MonoBehaviour
{
    #region Variable
    [Header("--DOTween--")]
    //�̵� ȿ�� Ease
    [SerializeField]protected Ease charEase;
    
    [Header("--Info--")]
    [HideInInspector]public int attackOrder;
    public string CharName { get; protected set; }
    [HideInInspector]public float CurrentHp;
    [field:SerializeField]public float MaxHp { get; protected set; }
    [field: SerializeField] public float AttackDamage { get; protected set; }
    public Vector3 Dir { get; private set; }

    [Header("--Animation--")]
    protected Animator animator;
    protected bool attackTrigger;
    protected bool AnimationTrigger;
    [HideInInspector] public bool SetHit;
    [HideInInspector] public bool SetDead;
    [HideInInspector] public bool SetHeal;
    [HideInInspector] public bool SetAttack;

    [Header("--GameData--")]
    [HideInInspector]public GameData gameData;

    [Header("--UI--")]
    protected Slider hpBar;
    [SerializeField]private Slider hpBarPrefabs;
    [SerializeField]private Text damageText;

    [Header("--Effect--")]
    [SerializeField]private GameObject dustPrefabs;
    public GameObject[] EffectPrefabs;
    private float dustTimer;
    private CharBase charObj;
    #endregion
    #region InitMethod
    virtual protected void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        CommonInit();
        Dir = transform.position;
    }

    /// <summary>
    /// hp bar���� �� ��ġ ����
    /// </summary>
    private void CommonInit() {
        hpBar = Instantiate(hpBarPrefabs, GameObject.Find("WorldCanvas").transform);
        hpBar.transform.position = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));
        hpBar.gameObject.SetActive(false);
    }
    #endregion

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

    

    /// <summary>
    /// ��� �ִϸ��̼� ���
    /// </summary>
    /// <returns></returns>
    private IEnumerator Co_DeadAnimation()
    {
        animator.SetTrigger("Die");
        SetDead = false;
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �� ���º� �ִϸ��̼� ���
    /// </summary>
    private void adjustAnimation()
    {
        if (!SetAttack)
        {
            animator.SetTrigger("Idle");
        }
        if (SetDead)
        {
            StartCoroutine(Co_DeadAnimation());
        }
        if (SetHit)
        {
            animator.SetTrigger("Spell");
            SetHit = false;
        }
    }

    virtual protected void Init() { }

    /// <summary>
    /// ���콺 enter�� hp�� ���
    /// </summary>
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

    /// <summary>
    /// ������ �ؽ�Ʈ ���
    /// </summary>
    protected void CreateDamage(CharBase target, float damagenum) {
        Text damage = Instantiate(damageText, GameObject.Find("WorldCanvas").transform);
        damage.transform.position = Camera.main.WorldToScreenPoint(new Vector3(target.transform.position.x + 1.3f,
            target.transform.position.y, target.transform.position.z));
        damage.text = "-" + damagenum.ToString();
    }

    #region Attack&Skill Method
    /// <summary>
    /// ��� �� Ÿ�� ��ų
    /// </summary>
    virtual public void AllTargetAttack(EnemyManager[] enemy, int effectnum) { }

    /// <summary>
    /// ���� �� Ÿ�� ����
    /// </summary>
    virtual public void Attack(EnemyManager enemy, int effectnum, int type) { }

    /// <summary>
    /// ���� �� Ÿ�� ����
    /// </summary>
    virtual public void Attack(PlayerManager player) { }
    #endregion


    #region Effect Method
    /// <summary>
    /// ���� �� ��ų ����Ʈ ���
    /// </summary>
    public void SetEffect(Transform pos, int effectnum, int type)
    {
        Transform effect = Instantiate(EffectPrefabs[effectnum], pos).transform;
        effect.transform.localScale = effect.transform.localScale * type;
    }

    /// <summary>
    /// �̵��� �߸��� ����Ʈ ����
    /// </summary>
    protected void craeteDustEffect(float pos) {
        dustTimer += Time.deltaTime;
        if (transform.position.x != pos && dustTimer > 0.5f) {
            Transform effect = Instantiate(dustPrefabs, transform).transform;
            effect.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, 0f);

            dustTimer = 0;
        }
    }
    #endregion
}
