using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class AssignManGER : MonoBehaviour
{
    [Header("--Prefabs--")]
    public GameObject[] playerPrefabs;
    public GameObject[] enemyPrefabs;

    [Header("--GameData--")]
    public GameData[] gameData;

    private void Awake()
    {
        AssignCharObject();
    }

    void AssignCharObject()
    {
        int playerNum = gameData[0].currentPlayerNumber;

        for (int i = 0; i < playerNum; i++)
        {
            if (gameData[i].playerCurrentHp > 0)
            {
                Transform trans = Instantiate(playerPrefabs[i]).transform;
                trans.position = new Vector2(-6f, (1f - i));
            }
        }

        for (int i = 0; i < playerNum; i++)
        {
            Transform greedTrans = Instantiate(enemyPrefabs[i]).transform;
            greedTrans.position = new Vector2(6f, (1 - i));
        }
    }
}
