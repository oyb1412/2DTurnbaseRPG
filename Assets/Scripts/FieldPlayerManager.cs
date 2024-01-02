using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerManager;

public class FieldPlayerManager : MonoBehaviour
{
    [Header("--Info--")]
    public float speed;
    public Vector2 moveDir;
    Rigidbody2D rigid;

    [Header("--GameData--")]
    public GameData gameData;

    [Header("--Enemy--")]
    public FieldEnemy enemy;

    [Header("--Effect--")]
    public GameObject dustPrefab;
    float dustTimer;

    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        dustPrefab.gameObject.GetComponent<Renderer>().sortingOrder = 3;
    }
    private void Start()
    {
        transform.position = gameData.playerFieldPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!FieldGameManager.Instance.isPlaying)
            return;

        PlayerAnimation();
    }

    private void FixedUpdate()
    {
        if (!FieldGameManager.Instance.isPlaying)
            return;

        rigid.MovePosition(rigid.position + moveDir.normalized * speed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (!FieldGameManager.Instance.isPlaying)
            return;

        CreateDustEffect();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!FieldGameManager.Instance.isPlaying)
            return;

        CollisionObject(collision);
    }

    void CollisionObject(Collision2D collision)
    {
        if (collision.collider.CompareTag("FieldEnemy"))
        {
            enemy = collision.gameObject.GetComponent<FieldEnemy>();
            gameData.playerFieldPosition = transform.position;

            FieldGameManager.Instance.ActionFade((int)FieldGameManager.Scenes.BattleScene);
            AudioManager.instance.PlayerBgm(AudioManager.Bgm.Field, false);
        }

        if (collision.collider.CompareTag("FieldVillage"))
        {
            Vector3 villagePos = collision.transform.position;
            Vector3 nextDir = villagePos - transform.position;
            nextDir = nextDir.normalized;
            gameData.playerFieldPosition = transform.position + nextDir * 2f;
            FieldGameManager.Instance.ActionFade((int)FieldGameManager.Scenes.VillageScene);
            AudioManager.instance.PlayerBgm(AudioManager.Bgm.Field, false);
        }
    }

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
        return enemy;
    }

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
        else if (moveDir.magnitude == 1)
        {
            animator.SetBool("IsWalking", true);
            animator.SetTrigger("Move");
        }
    }

}
