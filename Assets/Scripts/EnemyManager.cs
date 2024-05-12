using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 애너미 유닛 관리
/// </summary>
public class EnemyManager : CharBase
{
    public enum Enemy { Skul, Mumi, Mumis }
    public Enemy enemyType;

    protected override void Awake()
    {
        base.Awake();
        CurrentHp = MaxHp;
        CharName = enemyType.ToString();
    }

    protected override void Update()
    {
        base.Update();
        craeteDustEffect(6f);
        ShowHpBar(MouseManager.instance.MouseRayCast("Enemy"));
        hpBar.value = CurrentHp / MaxHp;
    }
    #region AnimatonMethod
    /// <summary>
    /// 이동 애니메이션 출력
    /// </summary>
    /// <param name="player">이동 목표</param>
    /// <returns></returns>
    private IEnumerator Co_MoveAnimation(PlayerManager player)
    {
        animator.SetTrigger("Move");
        var tween = transform.DOMove(new Vector3(player.transform.position.x + 0.3f, player.transform.position.y, 1f), 1.5f).SetEase(base.charEase);
        yield return tween.WaitForCompletion();

        StartCoroutine(Co_AttackAnimation(player));
    }

    /// <summary>
    /// 공격 애니메이션 출력
    /// </summary>
    /// <param name="player">공격 대상</param>
    /// <returns></returns>
    private IEnumerator Co_AttackAnimation(PlayerManager player)
    {
        if (!AnimationTrigger && transform.position.x < 0)
        {
            animator.SetTrigger("Attack");

            Instantiate(EffectPrefabs[0], player.transform);
            CreateDamage(player,AttackDamage);
            player.gameData.PlayerCurrentHp -= AttackDamage;

            if (player.gameData.PlayerCurrentHp <= 0)
                player.SetDead = true;
            else
                player.SetHit = true;

            AudioManager.instance.PlayerSfx(AudioManager.Sfx.Attack);

        }

        AnimationTrigger = true;

        yield return new WaitForSeconds(0.5f);
        attackTrigger = true;
    }

    /// <summary>
    /// 공격 후 되돌아오는 애니메이션 출력
    /// </summary>
    private IEnumerator Co_ReturnAnimation()
    {
        transform.localScale = Vector3.one;
        animator.SetTrigger("Move");
        var tween = transform.DOMove(Dir, 1.5f).SetEase(base.charEase);
        yield return tween.WaitForCompletion();
        transform.localScale = new Vector3(-1f, 1f, 1f);
        SetAttack = false;
        attackTrigger = false;
        AnimationTrigger = false;
        animator.SetTrigger("Idle");
    }
    #endregion

    /// <summary>
    /// 공격
    /// </summary>
    /// <param name="player">공격 대상</param>
    public override void Attack(PlayerManager player)
    {
        if (SetAttack)
        {
            if (!attackTrigger)
                StartCoroutine(Co_MoveAnimation(player));
            else
                StartCoroutine(Co_ReturnAnimation());
        }
    }
}
