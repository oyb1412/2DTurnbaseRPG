using System.Collections;
using UnityEngine;
using DG.Tweening;
public class PlayerManager : CharBase
{
    public enum Player { Wizard, Mini, Miho }
    public Player playerType;
 
    protected override void Awake()
    {
        base.Awake();
        Init();
        CharName = playerType.ToString();
    }

    protected override void Init() {
        CurrentHp = gameData.PlayerCurrentHp;
        MaxHp = gameData.PlayerMaxHp;
        AttackDamage = gameData.PlayerAttackDamage;
    }

    protected override void Update()
    {
        base.Update();
        craeteDustEffect(-6f);

        ShowHpBar(MouseManager.instance.MouseRayCast("Player"));

        if (SetHeal)
        {
            animator.SetTrigger("Attack");
            SetHeal = false;
        }
        hpBar.value = gameData.PlayerCurrentHp / gameData.PlayerMaxHp;

    }
    #region AnimationMethod
    /// <summary>
    /// ���� ���� �ִϸ��̼� �ڷ�ƾ
    /// </summary>
    /// <param name="enemy">���� ���</param>
    /// <param name="effectnum">����Ʈ ��ȣ</param>
    IEnumerator Co_AttackAnimation(EnemyManager[] enemy, int effectnum, int type)
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
                base.CreateDamage(enemy[i], gameData.PlayerAttackDamage);

                enemy[i].CurrentHp -= gameData.PlayerAttackDamage;
                if (enemy[i].CurrentHp <= 0)
                    enemy[i].SetDead = true;
                else
                    enemy[i].SetHit = true;
            }
            AudioManager.instance.PlayerSfx(AudioManager.Sfx.Attack);


        }
        AnimationTrigger = true;

        yield return new WaitForSeconds(0.5f);
        attackTrigger = true;
    }

    /// <summary>
    /// ���� ���ݿ� �̵� �ִϸ��̼� �ڷ�ƾ
    /// </summary>
    /// <param name="enemy">���� ���</param>
    /// <param name="effectnum">����Ʈ ��ȣ</param>
    /// <returns></returns>
    IEnumerator Co_MoveAnimation(EnemyManager[] enemy, int effectnum)
    {
        animator.SetTrigger("Move");
        int h = Random.Range(0,enemy.Length);
        var tween = transform.DOMove(new Vector3(enemy[h].transform.position.x - 0.3f, enemy[h].transform.position.y, 1f), 1.5f);
        yield return tween.WaitForCompletion();

        StartCoroutine(Co_AttackAnimation(enemy, effectnum, 1));
    }

    /// <summary>
    /// �̵� �� �ǵ��ƿ��� �ִϸ��̼� �ڷ�ƾ
    /// </summary>
    IEnumerator Co_ReturnAnimation()
    {
        transform.localScale = new Vector3(-1f, 1f, 1f);
        animator.SetTrigger("Move");
        var tween = transform.DOMove(Dir, 1.5f);
        yield return tween.WaitForCompletion();
        transform.localScale = Vector3.one;
        SetAttack = false;
        attackTrigger = false;
        AnimationTrigger = false;
        animator.SetTrigger("Idle");
    }

    /// <summary>
    /// �̵� �ִϸ��̼� �ڷ�ƾ
    /// </summary>
    /// <param name="enemy">�̵� ���</param>
    /// <param name="effectnum">����Ʈ ��ȣ</param>
    /// <returns></returns>
    IEnumerator Co_MoveAnimation(EnemyManager enemy, int effectnum, int type)
    {
        animator.SetTrigger("Move");
        var tween = transform.DOMove(new Vector3(enemy.transform.position.x - 0.3f, enemy.transform.position.y, 1.5f), 1);
        yield return tween.WaitForCompletion();

        StartCoroutine(Co_AttackAnimation(enemy, effectnum, type));
    }

    /// <summary>
    /// ���� �ִϸ��̼� �ڷ�ƾ
    /// </summary>
    /// <param name="enemy">���� ���</param>
    /// <param name="effectnum">����Ʈ ��ȣ</param>
    IEnumerator Co_AttackAnimation(EnemyManager enemy, int effectnum, int type)
    {
        if (!AnimationTrigger)
        {
            animator.SetTrigger("Attack");
            CreateDamage(enemy, gameData.PlayerAttackDamage * type);

            SetEffect(enemy.transform, effectnum, type);
            enemy.CurrentHp -= gameData.PlayerAttackDamage * type;
            if (enemy.CurrentHp <= 0)
                enemy.SetDead = true;
            else
                enemy.SetHit = true;

            AudioManager.instance.PlayerSfx(AudioManager.Sfx.Attack);

        }
        AnimationTrigger = true;

        yield return new WaitForSeconds(0.5f);
        attackTrigger = true;
    }
    #endregion

    #region AttackMethod

    /// <summary>
    /// ���� ��ų �޼ҵ�
    /// </summary>
    /// <param name="enemy">Ÿ�� ��</param>
    /// <param name="effectnum">����Ʈ ��ȣ</param>
    public override void AllTargetAttack(EnemyManager[] enemy, int effectnum) 
    {
        switch (playerType)
        {
            case Player.Wizard:
                if (SetAttack)
                {
                    AudioManager.instance.PlayerSfx(AudioManager.Sfx.WizardAttack);

                    GameObject trans = new GameObject();
                    trans.transform.position = new Vector3(enemy[0].transform.position.x, 0f, 1f);
                    for (int i = 0; i < enemy.Length; i++)
                    {
                        enemy[i].CurrentHp -= gameData.PlayerAttackDamage;
                        base.CreateDamage(enemy[i], gameData.PlayerAttackDamage);
                        if (enemy[i].CurrentHp <= 0)
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
                        StartCoroutine(Co_MoveAnimation(enemy,effectnum));
                    else
                        StartCoroutine(Co_ReturnAnimation());
                }

                break;

        }
    }
    /// <summary>
    /// ���� �� ���� �޼ҵ�
    /// </summary>
    /// <param name="enemy">���� ���</param>
    /// <param name="effectnum">����Ʈ ��ȣ</param>
    public override void Attack(EnemyManager enemy, int effectnum, int type)
    {
        switch (playerType)
        {
            case Player.Wizard:
                if (SetAttack)
                {
                    AudioManager.instance.PlayerSfx(AudioManager.Sfx.WizardAttack);

                    animator.SetTrigger("Fire");
                    SetEffect(enemy.transform, effectnum, type);
                    CreateDamage(enemy, gameData.PlayerAttackDamage * type);
                    enemy.CurrentHp -= gameData.PlayerAttackDamage * type;
                    if (enemy.CurrentHp <= 0)
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
                        StartCoroutine(Co_MoveAnimation(enemy, effectnum, type));
                    if (attackTrigger)
                        StartCoroutine(Co_ReturnAnimation());
                }
                break;
        }
    }
    #endregion
}
