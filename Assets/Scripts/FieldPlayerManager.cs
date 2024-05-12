using UnityEngine;

/// <summary>
/// 필드 씬 플레이어 관리
/// </summary>
public class FieldPlayerManager : MonoBehaviour
{
    #region Variable
    [Header("--Info--")]
    //플레이어 이동 속도
    [SerializeField]private float speed;
    //플레이어 이동 방향
    private Vector2 moveDir;
    private Rigidbody2D rigid;
    //이동 사운드 출력 타이머
    private float walkAudioTimer;
    [Header("--GameData--")]
    //게임 데이터
    [SerializeField]private GameData gameData;

    [Header("--Enemy--")]
    //충돌 애너미 보관
    private FieldEnemy enemy;

    [Header("--Effect--")]
    //발먼지 프리펩
    [SerializeField] private GameObject dustPrefab;
    //발먼지 이펙트 출력 타이머
    private float dustTimer;

    private Animator animator;

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        dustPrefab.gameObject.GetComponent<Renderer>().sortingOrder = 3;
    }
    private void Start()
    {
        transform.position = gameData.PlayerFieldPosition;
    }

    #region UpdateMethod
    void Update()
    {
        if (!FieldGameManager.Instance.IsPlaying)
            return;

        PlayerAnimation();
        walkAudioTimer += Time.deltaTime;

        if (walkAudioTimer > 0.3f && moveDir.magnitude > 0)
        {
            AudioManager.instance.PlayerSfx(AudioManager.Sfx.Walk);
            walkAudioTimer = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!FieldGameManager.Instance.IsPlaying)
            return;

        rigid.MovePosition(rigid.position + moveDir.normalized * speed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (!FieldGameManager.Instance.IsPlaying)
            return;

        CreateDustEffect();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!FieldGameManager.Instance.IsPlaying)
            return;

        CollisionObject(collision);
    }
    #endregion

    /// <summary>
    /// 충돌한 오브젝트에 맞는 씬으로 이동
    /// </summary>
    /// <param name="collision">충돌한 오브젝트</param>
    void CollisionObject(Collision2D collision)
    {
        if (collision.collider.CompareTag("FieldEnemy"))
        {
            enemy = collision.gameObject.GetComponent<FieldEnemy>();
            gameData.PlayerFieldPosition = transform.position;

            FieldGameManager.Instance.ActionFade((int)FieldGameManager.Scenes.BattleScene);
            AudioManager.instance.PlayerBgm(AudioManager.Bgm.Field, false);
        }

        if (collision.collider.CompareTag("FieldVillage"))
        {
            Vector3 villagePos = collision.transform.position;
            Vector3 nextDir = villagePos - transform.position;
            nextDir = nextDir.normalized;
            gameData.PlayerFieldPosition = transform.position + nextDir * 2f;
            FieldGameManager.Instance.ActionFade((int)FieldGameManager.Scenes.VillageScene);
            AudioManager.instance.PlayerBgm(AudioManager.Bgm.Field, false);
        }
    }

    /// <summary>
    /// 발먼지 이펙트 출력
    /// </summary>
    void CreateDustEffect()
    {
        dustTimer += Time.deltaTime;
        if (moveDir.magnitude > 0 && dustTimer > 0.5f)
        {
            Transform effect = Instantiate(dustPrefab, transform).transform;
            effect.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, 0f);

            dustTimer = 0;
        }
    }

    public FieldEnemy DestroyEnemy()
    {
        return enemy == null ? null : enemy;
    }

    /// <summary>
    /// 플레이어 이동 애니메이션 출력
    /// </summary>
    void PlayerAnimation()
    {
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.y = Input.GetAxisRaw("Vertical");

        if (moveDir.x > 0)
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        if (moveDir.x < 0)
            transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);


        if (moveDir.magnitude == 0)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Idle");
        }
        else
        {
            animator.SetBool("IsWalking", true);
            animator.SetTrigger("Move");
        }
    }

}
