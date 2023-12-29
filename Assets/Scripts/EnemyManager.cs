using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public enum Enemy { Enemy1, Enemy2 }
    public Enemy enemyType;
    SpriteRenderer spriter;
    public int attackNumber;

    public float currentHp;
    public float maxHp;
    public float attackDamage;

    public Slider hpBar;
    Slider hpbar;
    // Start is called before the first frame update

    private void Start()
    {
        currentHp = maxHp;
        hpbar = Instantiate(hpBar, GameObject.Find("Canvas").transform);
        //hp바 부모 캔버스로 변경

        //hp바 위치 머리위로 변경
        hpbar.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {
        hpbar.value = currentHp / maxHp;
        //슬라이더 밸류 설정
    }
    private void OnDisable()
    {
        hpbar.gameObject.SetActive(false);
    }

}
