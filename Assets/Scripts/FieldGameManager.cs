using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FieldGameManager : MonoBehaviour
{
    public enum Scenes { FieldScene, BattleScene, VillageScene}
    public Scenes scenes;
    public static FieldGameManager Instance;
    public Image fadeImage;
    public float fadeCount;
    public bool isPlaying;
    public bool fadeOn;
    public int currentPlyerNum;
    public FieldEnemy.EnemyType colliderEnemyType;
    public int gold;
    int nextScene;
    public Text goldText;
    public GameData[] gameData;
    public FieldPlayerManager player;
    public GameObject[] images;
    public Text[] texts;
    // Start is called before the first frame update
    private void Awake()
    {
        fadeImage.gameObject.SetActive(true);
        fadeCount = 1f;
        isPlaying = true;
        Instance = this;
        currentPlyerNum = gameData[0].currentPlayerNumber;
        gold = gameData[0].currentGold;
        goldText.text = gold.ToString();
        texts[0].text = "Lv." + gameData[0].playerLevel;
        if (gameData[0].currentPlayerNumber == 2)
        {
            texts[1].text = "Lv." + gameData[0].playerLevel;
            images[0].gameObject.SetActive(true);
        }
        else if (gameData[0].currentPlayerNumber == 3)
        {
            texts[1].text = "Lv." + gameData[0].playerLevel;
            images[0].gameObject.SetActive(true);
            texts[2].text = "Lv." + gameData[0].playerLevel;
            images[1].gameObject.SetActive(true);
        }
    }
    private void Start()
    {
        AudioManager.instance.PlayerBgm(AudioManager.Bgm.Field, true);

    }
    // Update is called once per frame
    void Update()
    {

            if (!fadeOn)
            {

                fadeCount -= 0.01f;
                fadeImage.color = new Color(0, 0, 0, fadeCount);
                if (fadeCount <= 0)
                {
                    isPlaying = true;
                    fadeCount = 0;
                }
            }

            if (fadeOn)
            {
                fadeCount += 0.01f;
                fadeImage.color = new Color(0, 0, 0, fadeCount);
                if (fadeCount >= 1)
                {
                     PlayerPrefs.SetInt("colliderEnemyType", (int)colliderEnemyType);
                     if(nextScene == (int)Scenes.BattleScene)
                         Destroy(player.DestroyEnemy().gameObject);
                     SceneManager.LoadScene(nextScene);
                     fadeOn = false;
                     isPlaying = true;
                 }
            }
        
    }

    public void ActionFade(int scene)
    {
        isPlaying = false;
        fadeOn = true;
        nextScene = scene;
    }


}
