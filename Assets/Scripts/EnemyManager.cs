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

    IEnumerator MeleeAttackAnimation(PlayerManager player)
    {
        if (!AnimationTrigger)
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
    public override void MeleeAttackAndMove(PlayerManager player)
    {
        if (SetAttack)
        {
            if (!attackTrigger)
            {
                if (transform.position.x >= 6)
                    dir = (player.transform.position - transform.position).normalized;

                if (transform.position.x > -5.5f)
                {
                    animator.SetTrigger("Move");
                    transform.Translate(new Vector3(dir.x, dir.y, dir.z) * Time.deltaTime * speed);
                }
                else
                {
                    StartCoroutine(MeleeAttackAnimation(player));
                }
            }


            if (attackTrigger)
            {
                if (transform.position.x < 6f)
                {
                    transform.localScale = Vector3.one;
                    animator.SetTrigger("Move");
                    transform.Translate(new Vector3(-dir.x, -dir.y, dir.z) * Time.deltaTime * speed);
                }
                else
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                    transform.position = new Vector3(6f, transform.position.y, 0f);
                    SetAttack = false;
                    attackTrigger = false;
                    AnimationTrigger = false;
                    animator.SetTrigger("Idle");

                }
            }

        }




    }


}
