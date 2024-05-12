using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 마을 씬 관리
/// </summary>
public class VillageGameManager : MonoBehaviour
{
    #region Variable
    public enum Scenes
    {
        TutorialScene, FieldScene, BattleScene, VillageScene
    }

    [Header("--Instance--")]
    public static VillageGameManager instance;

    [Header("--UI--")]
    //페이드 이미지
    [SerializeField] private Image fadeImage;
    //플레이어 오브젝트
    [SerializeField] private GameObject player;
    //게임 데이터
    [SerializeField] private GameData[] gameData;
    //플레이어 레벨 출력 텍스트
    private Text[] playerLevelText;
    //플레이어 아이콘 출력 이미지
    private Image[] playerIconImage;
    //플레이어 체력 및 마나 출력 슬라이더
    private Slider[,] PlayerIconSlider;
    //페이드 타이머
    private float fadeCount;
    //페이드 여부
    private bool fadeOn;
    //다음 씬(정수)
    private int nextScene;
    #endregion
    #region InitMethod
    private void Awake()
    {
        instance = this;
        Init();
    }
    void Start()
    {
        AudioManager.instance.PlayerBgm(AudioManager.Bgm.Village, true);
    }

    /// <summary>
    /// 플레이어 UI 초기화
    /// </summary>
    void Init() {
        PlayerIconSlider = new Slider[gameData.Length, gameData.Length - 1];
        playerLevelText = new Text[player.gameObject.transform.childCount];
        playerIconImage = new Image[player.gameObject.transform.childCount];

        int count = 0;
        for (int i = 0; i < gameData.Length; i++) {
            for (int j = 0; j < gameData.Length - 1; j++) {
                PlayerIconSlider[i, j] = player.GetComponentsInChildren<Slider>()[count];
                count++;

            }

        }
        playerLevelText = player.GetComponentsInChildren<Text>();
        count = 0;
        for (int i = 0; i < playerIconImage.Length; i++) {
            playerIconImage[i] = player.GetComponentsInChildren<Image>()[count];
            if (gameData[i].PlayerCurrentHp <= 0)
                playerIconImage[i].color = Color.red;

            count += 5;
        }

        fadeImage.gameObject.SetActive(true);
        fadeCount = 1f;
        for (int i = 0; i < player.gameObject.transform.childCount; i++)
            playerLevelText[i].text = "Lv." + gameData[i].PlayerLevel;

        if (gameData[0].CurrentPlayerNumber == 1) {
            playerIconImage[1].gameObject.SetActive(false);
            playerIconImage[2].gameObject.SetActive(false);

        } else if (gameData[0].CurrentPlayerNumber == 2) {
            playerIconImage[2].gameObject.SetActive(false);
        }
    }
    #endregion
    void Update()
    {
        AdjustFade();

        int count = 0;
        for (int i = 0; i < gameData.Length; i++)
        {
            for (int j = 0; j < gameData.Length - 1; j++)
            {
                if (j == 0)
                    PlayerIconSlider[i, j].value = gameData[count].PlayerCurrentHp / gameData[count].PlayerMaxHp;
                else
                    PlayerIconSlider[i, j].value = gameData[count].PlayerCurrentMp / gameData[count].PlayerMaxMp;

            }

            if (gameData[count].PlayerCurrentHp > 0)
                playerIconImage[count].color = new Color(1, 1, 1, 1);

            count++;

        }

        for (int i = 0; i < player.gameObject.transform.childCount; i++)
            playerLevelText[i].text = "Lv." + gameData[i].PlayerLevel;
    }

    /// <summary>
    /// 페이드 알파값 조정
    /// </summary>
    void AdjustFade()
    {
        if (!fadeOn)
        {
            fadeCount -= 0.01f;
            fadeImage.color = new Color(0, 0, 0, fadeCount);
            if (fadeCount <= 0)
            {
                fadeImage.gameObject.SetActive(false);

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
            }
        }
    }

    /// <summary>
    /// 페이드 돌입
    /// </summary>
    /// <param name="scene">이동할 씬</param>
    public void ActionFade(int scene)
    {
        fadeOn = true;
        nextScene = scene;
    }

    
}
