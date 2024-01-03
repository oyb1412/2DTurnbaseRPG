using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : CharBase
{
    [Header("--Info--")]
    public float skillDamage;//고정


    public enum Player { Wizard, Mini, Miho }//고정
    public Player playerType;//고정


 
    protected override void Awake()
    {
        base.Awake();
        Init();
        charName = playerType.ToString();
    }

    protected override void Update()
    {
        base.Update();
        base.craeteDustEffect(-6f);

        base.ShowHpBar(MouseManager.instance.MouseRayCast("Player"));//다른자식과 다른내용의 함수

        if (base.SetHeal)
        {
            base.animator.SetTrigger("Attack");
            base.SetHeal = false;
        }
        hpBar.value = gameData.playerCurrentHp / gameData.playerMaxHp;

    }



    IEnumerator AttackAnimation(EnemyManager enemy, int effectnum, int type)
    {
        if (!AnimationTrigger)
        {
            animator.SetTrigger("Attack");
            base.CreateDamage(enemy, gameData.playerAttackDamage * type);

            SetEffect(enemy.transform, effectnum);
            enemy.currentHp -= gameData.playerAttackDamage * type;
            if (enemy.currentHp <= 0)
                enemy.SetDead = true;
            else
                enemy.SetHit = true;
        }
        AnimationTrigger = true;

        yield return new WaitForSeconds(0.5f);
        attackTrigger = true;
    }

    IEnumerator AllAttackAnimation(EnemyManager[] enemy, int effectnum)
    {
        if (!AnimationTrigger)
        {
            animator.SetTrigger("Attack");
            SetEffect(enemy[Mathf.Max(0,1)].transform, effectnum);
            for(int i = 0; i < enemy.Length; i++)
            {
                base.CreateDamage(enemy[i], gameData.playerAttackDamage);

                enemy[i].currentHp -= gameData.playerAttackDamage;
                if (enemy[i].currentHp <= 0)
                    enemy[i].SetDead = true;
                else
                    enemy[i].SetHit = true;
            }

        }
        AnimationTrigger = true;

        yield return new WaitForSeconds(0.5f);
        attackTrigger = true;
    }
    public override void AllTargetAttack(EnemyManager[] enemy, int effectnum) 
    {
        switch (playerType)
        {
            case Player.Wizard:
                if (SetAttack)
                {
                    for (int i = 0; i < enemy.Length; i++)
                    {
                        enemy[i].currentHp -= gameData.playerAttackDamage;
                        base.CreateDamage(enemy[i], gameData.playerAttackDamage);
                        if (enemy[i].currentHp <= 0)
                            enemy[i].SetDead = true;
                        else
                            enemy[i].SetHit = true;

                    }
                    animator.SetTrigger("Fire");
                    SetEffect(enemy[0].transform, effectnum);
                }
                SetAttack = false;
                break;
            case Player.Mini:
            case Player.Miho:
                if (SetAttack)
                {
                    if (!attackTrigger)
                    {
                        if (transform.position.x <= -6)
                            dir = (enemy[Mathf.Max(0,1)].transform.position - transform.position).normalized;

                        if (transform.position.x < 5.5f)
                        {
                            animator.SetTrigger("Move");
                            transform.Translate(new Vector3(dir.x, dir.y, dir.z) * Time.deltaTime * speed);
                        }
                        else
                        {
                            StartCoroutine(AllAttackAnimation(enemy, effectnum));
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
    public override void Attack(EnemyManager enemy, int effectnum, int type)
    {
        switch (playerType)
        {
            case Player.Wizard:
                if (SetAttack)
                {
                    animator.SetTrigger("Fire");
                    SetEffect(enemy.transform, 0);
                    base.CreateDamage(enemy, gameData.playerAttackDamage * type);
                    enemy.currentHp -= gameData.playerAttackDamage * type;
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
                            StartCoroutine(AttackAnimation(enemy,effectnum, type));
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
  
    protected override void Init()
    {
        skillDamage = gameData.skillDamage;
        currentHp = gameData.playerCurrentHp;
        maxHp = gameData.playerMaxHp;
        attackDamage = gameData.playerAttackDamage;
    }


 
    public void SetEffect(Transform pos, int effectnum)
    {
        Instantiate(effectPrefabs[effectnum], pos);
    }
}
