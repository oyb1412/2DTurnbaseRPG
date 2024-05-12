using UnityEngine;

/// <summary>
/// ��Ʋ �� ���� ����
/// </summary>
public class AssignUnit : MonoBehaviour
{
    [Header("--Prefabs--")]
    [SerializeField]private GameObject[] playerPrefabs;
    [SerializeField]private GameObject[] enemyPrefabs;

    [Header("--GameData--")]
    [SerializeField]private GameData[] gameData;

    private void Awake()
    {
        AssignCharObject();
    }

    /// <summary>
    /// ��Ʋ �� ���� �� ���� ����
    /// </summary>
    private void AssignCharObject()
    {
        int playerNum = gameData[0].CurrentPlayerNumber;

        for (int i = 0; i < playerNum; i++)
        {
            if (gameData[i].PlayerCurrentHp > 0)
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
