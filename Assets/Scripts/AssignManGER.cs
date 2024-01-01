using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class AssignManGER : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public GameObject[] enemyPrefabs;
    public GameData gameData;

    private void Awake()
    {

        int playerNum = gameData.currentPlayerNumber;
        int enemyType = PlayerPrefs.GetInt("colliderEnemyType");

        for (int i = 0; i < playerNum; i++)
        {
            Transform trans = Instantiate(playerPrefabs[i]).transform;
            trans.position = new Vector2(-6f, (-0.5f - i * 1.5f));
        }

        for (int i = 0; i < playerNum; i++)
        {
             Transform greedTrans = Instantiate(enemyPrefabs[i]).transform;
             greedTrans.position = new Vector2(6f, (-0.5f - i * 1.5f));
        }
    }
}
