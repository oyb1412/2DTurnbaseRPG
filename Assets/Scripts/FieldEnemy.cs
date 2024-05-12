using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Field �� �ֳʹ� ����
/// </summary>
public class FieldEnemy : MonoBehaviour
{
    #region Variable
    private float nextMoveX;
    private float nextMoveY;
    private bool moveTrigger = false;
    private float moveTimer = 0;
    private SpriteRenderer spriter;
    private Rigidbody2D rigid;
    private Animator animator;
    [SerializeField] private float speed;
    #endregion 
    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        rigid = spriter.GetComponent<Rigidbody2D>();
        gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
    private void FixedUpdate()
    {
        if (!FieldGameManager.Instance.IsPlaying)
            return;
        
        //�ٸ� ������ �̵� �� ����
        if (SceneManager.GetActiveScene().name != "FieldScene")
        {
            transform.localScale = new Vector3(0f,0f,0f);
            return;
        }

        //�̵� ���°� �ƴҽ� Idle�ִϸ��̼� ���
        if (nextMoveX == 0 && nextMoveY == 0)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Idle");
        }
        //�̵� �����Ͻ� Walking�ִϸ��̼� ���
        else if (nextMoveX != 0 || nextMoveY != 0)
        {
            animator.SetBool("IsWalking", true);
            animator.SetTrigger("Move");
        }

        //Flip ����
        if (nextMoveX < 0)
            transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
        else if (nextMoveX > 0)
            transform.localScale = new Vector3(0.5f, 0.5f, 1f);


        if (!moveTrigger)
        {
            RandomMove();
            moveTrigger = true;
        }
        if (moveTimer <= 1f) 
        {
            moveTimer += Time.deltaTime;
            rigid.MovePosition(rigid.position + new Vector2(nextMoveX, nextMoveY) * speed * Time.deltaTime);
        }
        if(moveTimer > 1f && moveTimer <= 3f)
        {
            moveTimer += Time.deltaTime;
            nextMoveX = 0;
            nextMoveY = 0;
        }
        if (moveTimer > 3f)
        {
            moveTimer = 0;
            moveTrigger = false;
        }
    }

    /// <summary>
    /// ������ ��ġ �̵�
    /// </summary>
    private void RandomMove()
    {
        int ranMove = Random.Range(0, 4);
        switch(ranMove)
        {
            case 0:
                nextMoveX = -2f;
                nextMoveY = 0f;

                break;
            case 1:
                nextMoveX = 2f;
                nextMoveY = 0f;

                break;
            case 2:
                nextMoveY = -2f;
                nextMoveX = 0f;

                break;
            case 3:
                nextMoveY = 2f;
                nextMoveX = 0f;

                break;
        }
    }
}
