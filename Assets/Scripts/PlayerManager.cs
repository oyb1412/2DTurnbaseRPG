using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static EnemyManager;
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


    IEnumerator AttackAnimation(EnemyManager[] enemy, int effectnum, int type)
    {
        if (!AnimationTrigger)
        {
            int h = Random.Range(0, enemy.Length);
            animator.SetTrigger("Attack");
            GameObject trans = new GameObject();
            trans.transform.position = new Vector3(enemy[h].transform.position.x, 0f, 1f);
            SetEffect(trans.transform, effectnum, type);
            for (int i = 0; i < enemy.Length; i++)
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
    IEnumerator MoveAnimation(EnemyManager[] enemy, int effectnum, int type)
    {
        animator.SetTrigger("Move");
        int h = Random.Range(0,enemy.Length);
        var tween = transform.DOMove(new Vector3(enemy[h].transform.position.x - 0.3f, enemy[h].transform.position.y, 1f), 1.5f);
        yield return tween.WaitForCompletion();
        StartCoroutine(AttackAnimation(enemy, effectnum, 1));
    }

    IEnumerator ReturnAnimation()
    {
        transform.localScale = new Vector3(-1f, 1f, 1f);
        animator.SetTrigger("Move");
        var tween = transform.DOMove(dir, 1.5f);
        yield return tween.WaitForCompletion();
        transform.localScale = Vector3.one;
        SetAttack = false;
        attackTrigger = false;
        AnimationTrigger = false;
        animator.SetTrigger("Idle");
    }

    IEnumerator MoveAnimation(EnemyManager enemy, int effectnum, int type)
    {
        animator.SetTrigger("Move");
        var tween = transform.DOMove(new Vector3(enemy.transform.position.x - 0.3f, enemy.transform.position.y, 1.5f), 1);
        yield return tween.WaitForCompletion();
        StartCoroutine(AttackAnimation(enemy, effectnum, type));
    }

    IEnumerator AttackAnimation(EnemyManager enemy, int effectnum, int type)
    {
        if (!AnimationTrigger)
        {
            animator.SetTrigger("Attack");
            CreateDamage(enemy, gameData.playerAttackDamage * type);

            SetEffect(enemy.transform, effectnum, type);
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
    public override void AllTargetAttack(EnemyManager[] enemy, int effectnum) 
    {
        switch (playerType)
        {
            case Player.Wizard:
                if (SetAttack)
                {
                    GameObject trans = new GameObject();
                    trans.transform.position = new Vector3(enemy[0].transform.position.x, 0f, 1f);
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
                    SetEffect(trans.transform, effectnum, 1);
                }

                SetAttack = false;
                break;
            case Player.Mini:
            case Player.Miho:
                if (SetAttack)
                {
                    if (!attackTrigger)
                        StartCoroutine(MoveAnimation(enemy,effectnum,1));
                    else
                        StartCoroutine(ReturnAnimation());
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
                    SetEffect(enemy.transform, effectnum, type);
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
                        StartCoroutine(MoveAnimation(enemy, effectnum, type));
                    if (attackTrigger)
                        StartCoroutine(ReturnAnimation());
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


 

}
