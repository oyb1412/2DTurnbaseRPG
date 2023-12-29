using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PlayerManager : MonoBehaviour
{
    public float currentHp;
    public float maxHp;
    public float attackDamage;
    public int attackNumber;

    public Slider hpBar;
    Slider hpbar;
    private void Start()
    {
        currentHp = maxHp;

        currentHp = maxHp;
        hpbar = Instantiate(hpBar, GameObject.Find("Canvas").transform);
        //hp바 부모 캔버스로 변경

        //hp바 위치 머리위로 변경
        hpbar.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
    }
    private void Update()
    {
        hpbar.value = currentHp / maxHp;
        //슬라이더 밸류 설정
    }


}
