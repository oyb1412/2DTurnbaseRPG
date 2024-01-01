using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Scriptble Object/Data")]
public class GameData : ScriptableObject
{
    public int currentGold;
    public int currentPlayerNumber;
    public float playerCurrentHp;
    public float playerMaxHp;
    public float playerAttackDamage;
    public Vector2 playerFieldPosition;
    public int levelUseGold;
    public int plusPlayerGold;
    public int playerLevel;
    public float skillDamage;
}
