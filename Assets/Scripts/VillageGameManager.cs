using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class VillageGameManager : MonoBehaviour
{
    public enum Scenes { FieldScene, BattleScene, VillageScene }

    [Header("--Instance--")]
    public static VillageGameManager instance;
    public bool isPlaying;

    [Header("--UI--")]
    public Image fadeImage;
    public GameObject players;
    Text[] playerLevelText;
    Image[] playerIconImage;
    Slider[,] PlayerIconSlider;
    public GameData[] gameData;

    public bool fadeTrigger;
    float fadeCount;
    public bool fadeOn;
    int nextScene;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        Init();
    }
    void Start()
    {
        //AudioManager.instance.PlayerBgm(AudioManager.Bgm.Village, true);

        isPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
        AdjustFade();

        int count = 0;
        for (int i = 0; i < gameData.Length; i++)
        {
            for (int j = 0; j < gameData.Length - 1; j++)
            {
                if (j == 0)
                    PlayerIconSlider[i, j].value = gameData[count].playerCurrentHp / gameData[count].playerMaxHp;
                else
                    PlayerIconSlider[i, j].value = gameData[count].playerCurrenMp / gameData[count].playerMaxMp;

            }

            if (gameData[count].playerCurrentHp > 0)
                playerIconImage[count].color = new Color(1, 1, 1, 1);

            count++;

        }
    }
    void AdjustFade()
    {
        if (!fadeOn)
        {
            fadeCount -= 0.01f;
            fadeImage.color = new Color(0, 0, 0, fadeCount);
            if (fadeCount <= 0)
            {
                fadeImage.gameObject.SetActive(false);

                isPlaying = true;
                fadeCount = 0;
            }
        }

        if (fadeOn)
        {
            fadeImage.gameObject.SetActive(true);

            fadeCount += 0.01f;
            fadeImage.color = new Color(0, 0, 0, fadeCount);
            if (fadeCount >= 1)
            {
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

    void Init()
    {
        PlayerIconSlider = new Slider[gameData.Length, gameData.Length - 1];
        playerLevelText = new Text[players.gameObject.transform.childCount];
        playerIconImage = new Image[players.gameObject.transform.childCount];

        int count = 0;
        for (int i = 0; i < gameData.Length; i++)
        {
            for (int j = 0; j < gameData.Length - 1; j++)
            {
                PlayerIconSlider[i, j] = players.GetComponentsInChildren<Slider>()[count];
                count++;

            }

        }
        playerLevelText = players.GetComponentsInChildren<Text>();
        count = 0;
        for (int i = 0; i < playerIconImage.Length; i++)
        {
            playerIconImage[i] = players.GetComponentsInChildren<Image>()[count];
            if (gameData[i].playerCurrentHp <= 0)
                playerIconImage[i].color = Color.red;

            count += 5;
        }

        fadeImage.gameObject.SetActive(true);
        fadeCount = 1f;
        isPlaying = true;
        for (int i = 0; i < players.gameObject.transform.childCount; i++)
            playerLevelText[i].text = "Lv." + gameData[i].playerLevel;

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
}
