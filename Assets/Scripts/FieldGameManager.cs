using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FieldGameManager : MonoBehaviour
{
    [Header("--Instance--")]
    public static FieldGameManager Instance;
    public bool isPlaying;

    [Header("--Scene--")]
    public Scenes scenes;
    public enum Scenes { FieldScene, BattleScene, VillageScene}
    int nextScene;

    [Header("--UI--")]
    public Image fadeImage;
    public float fadeCount;
    public bool fadeOn;
    public Text goldText;
    public GameObject players;
    Text[] playerLevelText;
    Image[] playerIconImage;

    [Header("--GameData--")]
    public GameData[] gameData;

    [Header("--PlayerInfo--")]
    public int currentPlyerNum;
    public int currentPlayerGold;
    public FieldPlayerManager player;
    // Start is called before the first frame update
    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        AudioManager.instance.PlayerBgm(AudioManager.Bgm.Field, true);

    }

    void Update()
    {
        adjustFade();
    }
    void Init()
    {
        playerLevelText = new Text[players.gameObject.transform.childCount];
        playerIconImage = new Image[players.gameObject.transform.childCount];

        playerLevelText = players.GetComponentsInChildren<Text>();
        playerIconImage = players.GetComponentsInChildren<Image>();
        fadeImage.gameObject.SetActive(true);
        fadeCount = 1f;
        isPlaying = true;
        Instance = this;
        currentPlyerNum = gameData[0].currentPlayerNumber;
        currentPlayerGold = gameData[0].currentGold;
        goldText.text = currentPlayerGold.ToString();
        for(int i = 0; i< players.gameObject.transform.childCount;i++)
               playerLevelText[i].text = "Lv." + gameData[0].playerLevel;

        if (gameData[0].currentPlayerNumber == 1)
        {
            playerIconImage[1].gameObject.SetActive(false);
            playerIconImage[2].gameObject.SetActive(false);

        }
        else if (gameData[0].currentPlayerNumber == 2)
        {
            playerIconImage[2].gameObject.SetActive(false);
        }
    }

    void adjustFade()
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
                if (nextScene == (int)Scenes.BattleScene)
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
