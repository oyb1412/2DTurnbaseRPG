using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    public int enemyCount;
    int spawnPointCount;
    public GameObject[] enemyPrefabs;
    Transform[] ponts;
    // Start is called before the first frame update
    void Start()
    {
        spawnPointCount = GameObject.FindGameObjectsWithTag("SpawnPoint").Length;
        ponts = transform.GetComponentsInChildren<Transform>();

        if (GameObject.FindGameObjectsWithTag("FieldEnemy").Length == 0)
        {
            int count = 0;

            for (int i = 1; i < spawnPointCount + 1; i++)
            {
                Instantiate(enemyPrefabs[count], ponts[i]);
                count++;
                if (count > 2)
                    count = 0;
            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!FieldGameManager.Instance.isPlaying)
            return;

        if (SceneManager.GetActiveScene().name != "FieldScene")
            return;

        if (GameObject.FindGameObjectsWithTag("FieldEnemy").Length + 2 < spawnPointCount)
        { 
            while(true)
            {
                int ran = Random.Range(0, transform.childCount);
                Vector2 enemyPos = ponts[ran].transform.position;
                Vector2 playerPos = FieldGameManager.Instance.player.transform.position;
                Vector2 dir = enemyPos - playerPos;

                if(dir.magnitude > 5f)
                {
                    int ranEnemy = Random.Range(0, enemyPrefabs.Length);

                    Instantiate(enemyPrefabs[ranEnemy], ponts[ran]);

                    break;
                }
            }
        }
    }
}
