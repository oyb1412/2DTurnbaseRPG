using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �� ���� ����
/// </summary>
public class SpawnManager : MonoBehaviour
{
    private int spawnPointCount;
    private Transform[] ponts;

    [Header("--EnemyPrefabs--")]
    [SerializeField] private GameObject enemyPrefabs;
    void Start()
    {
        Init();
    }

    void Init() {
        spawnPointCount = GameObject.FindGameObjectsWithTag("SpawnPoint").Length;
        ponts = transform.GetComponentsInChildren<Transform>();

        if (GameObject.FindGameObjectsWithTag("FieldEnemy").Length == 0) {
            for (int i = 1; i < spawnPointCount + 1; i++) {
                Instantiate(enemyPrefabs, ponts[i]);
            }
        }
    }

    void Update()
    {
        if (!FieldGameManager.Instance.IsPlaying)
            return;

        if (SceneManager.GetActiveScene().name != "FieldScene")
            return;

        EnemyAutoCreate();
    }

    /// <summary>
    /// �� ���� ���� ���Ϸ� �پ�� �� �� ����
    /// </summary>
    void EnemyAutoCreate()
    {
        if (GameObject.FindGameObjectsWithTag("FieldEnemy").Length + 2 < spawnPointCount)
        {
            while (true)
            {
                int ran = Random.Range(0, transform.childCount);
                Vector2 enemyPos = ponts[ran].transform.position;
                Vector2 playerPos = FieldGameManager.Instance.Player.transform.position;
                Vector2 dir = enemyPos - playerPos;

                if (dir.magnitude > 5f)
                {

                    Instantiate(enemyPrefabs, enemyPos, Quaternion.identity);

                    break;
                }
            }
        }
    }
    
}
