using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using static EnemyManager;

public class EnemyManager : CharBase
{
    public enum Enemy { Skul, Mumi, Mumis }
    public Enemy enemyType;


    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();//애니메이터 겟컴포넌트
        currentHp = maxHp;
        charName = enemyType.ToString();
    }

    protected override void Update()
    {
        base.Update();//부모 update 실행
        base.craeteDustEffect(6f);
        base.ShowHpBar(MouseManager.instance.MouseRayCast("Enemy"));//다른자식과 다른내용의 함수
        hpBar.value = currentHp / maxHp;
    }

    IEnumerator MoveAnimation(PlayerManager player)
    {
        animator.SetTrigger("Move");
        var tween = transform.DOMove(new Vector3(player.transform.position.x + 0.3f, player.transform.position.y, 1f), 1.5f).SetEase(base.charEase);
        yield return tween.WaitForCompletion();
        StartCoroutine(AttackAnimation(player));
    }

    IEnumerator AttackAnimation(PlayerManager player)
    {
        if (!AnimationTrigger && transform.position.x < 0)
        {
            animator.SetTrigger("Attack");
            Instantiate(effectPrefabs[0], player.transform);
            base.CreateDamage(player,attackDamage);
            player.gameData.playerCurrentHp -= attackDamage;

            if (player.gameData.playerCurrentHp <= 0)
                player.SetDead = true;
            else
                player.SetHit = true;
        }
        AnimationTrigger = true;

        yield return new WaitForSeconds(0.5f);
        attackTrigger = true;
    }

    IEnumerator ReturnAnimation()
    {
        transform.localScale = Vector3.one;
        animator.SetTrigger("Move");
        var tween = transform.DOMove(dir, 1.5f).SetEase(base.charEase);
        yield return tween.WaitForCompletion();
        transform.localScale = new Vector3(-1f, 1f, 1f);
        SetAttack = false;
        attackTrigger = false;
        AnimationTrigger = false;
        animator.SetTrigger("Idle");
    }
    public override void Attack(PlayerManager player)
    {
        if (SetAttack)
        {
            if (!attackTrigger)
                StartCoroutine(MoveAnimation(player));
            else
                StartCoroutine(ReturnAnimation());
        }
    }
}
