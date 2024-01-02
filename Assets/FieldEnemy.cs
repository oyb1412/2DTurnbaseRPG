using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FieldEnemy : MonoBehaviour
{
    public enum EnemyType
    {
        Skul,
        Mumi,
        Mumis
    }
    public EnemyType enemyType;
    public float nextMoveX;
    public float nextMoveY;
    bool moveTrigger = false;
    float moveTimer = 0;
    public float speed;
    SpriteRenderer spriter;
    Rigidbody2D rigid;
    Animator animator;
    // Start is called before the first frame update


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
        if (!FieldGameManager.Instance.isPlaying)
            return;



        
        if (SceneManager.GetActiveScene().name != "FieldScene")
        {
            transform.localScale = new Vector3(0f,0f,0f);
            return;
        }



        if (nextMoveX == 0 && nextMoveY == 0)
        {
            animator.SetBool("IsWalking", false);
            animator.SetTrigger("Idle");
        }
        else if (nextMoveX != 0 || nextMoveY != 0)
        {
            animator.SetBool("IsWalking", true);
            animator.SetTrigger("Move");
        }
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
    void RandomMove()
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
