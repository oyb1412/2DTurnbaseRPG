using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 필드 씬 관리
/// </summary>
public class FieldGameManager : MonoBehaviour
{
    #region Variable
    public enum Scenes { TutorialScene, FieldScene, BattleScene, VillageScene }
    [Header("--Instance--")]
    public static FieldGameManager Instance;
    //게임 진행 상황 여부
    public bool IsPlaying { get; private set; }
    //다음 씬(정수)
    private int nextScene;

    [Header("--UI--")]
    //페이드 이미지
    [SerializeField]private Image fadeImage;
    //페이드 카운트(0~1.0)
    private float fadeCount;
    //페이드 온 여부
    private bool fadeOn;
    //골드 표기 텍스트
    [SerializeField] private Text goldText;
    //플레이어 이미지 패런츠 판넬
    [SerializeField] private GameObject players;
    //플레이어 레벨 표기 텍스트
    private Text[] playerLevelText;
    //플레이어 아이콘 표기 이미지
    private Image[] playerIconImage;
    //플레이어 체력 및 마나 표기 슬라이더
    [SerializeField] private Slider[,] PlayerIconSlider;
    [Header("--GameData--")]
    //게임 데이터
    [SerializeField] private GameData[] gameData;

    [Header("--PlayerInfo--")]
    //보유중인 플레이어 골드
    private int currentPlayerGold;
    //현재 조작중인 플레이어
    public FieldPlayerManager Player;
    #endregion

    #region InitMethod
    private void Awake()
    {
        Instance = this;
        Init();
    }

    private void Start()
    {
        AudioManager.instance.PlayerBgm(AudioManager.Bgm.Field, true);

    }

    /// <summary>
    /// 플레이어 정보 UI 초기화
    /// </summary>
    void Init() {
        PlayerIconSlider = new Slider[gameData.Length, gameData.Length - 1];
        playerLevelText = new Text[players.gameObject.transform.childCount];
        playerIconImage = new Image[players.gameObject.transform.childCount];

        int count = 0;
        for (int i = 0; i < gameData.Length; i++) {
            for (int j = 0; j < gameData.Length - 1; j++) {
                PlayerIconSlider[i, j] = players.GetComponentsInChildren<Slider>()[count];
                count++;

            }

        }
        playerLevelText = players.GetComponentsInChildren<Text>();
        count = 0;
        for (int i = 0; i < playerIconImage.Length; i++) {
            playerIconImage[i] = players.GetComponentsInChildren<Image>()[count];
            if (gameData[i].PlayerCurrentHp <= 0)
                playerIconImage[i].color = Color.red;

            count += 5;
        }

        fadeImage.gameObject.SetActive(true);
        fadeCount = 1f;
        IsPlaying = true;
        currentPlayerGold = gameData[0].CurrentGold;
        goldText.text = currentPlayerGold.ToString();
        for (int i = 0; i < players.gameObject.transform.childCount; i++)
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
        adjustFade();

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
            count++;

        }
    }

    /// <summary>
    /// 현재 상태에 맞게 페이드 on & off
    /// </summary>
    void adjustFade()
    {
        if (!fadeOn)
        {

            fadeCount -= 0.01f;
            fadeImage.color = new Color(0, 0, 0, fadeCount);
            if (fadeCount <= 0)
            {
                IsPlaying = true;
                fadeCount = 0;
            }
        }

        if (fadeOn)
        {
            if (fadeCount == 0)
            {
                AudioManager.instance.PlayerSfx(AudioManager.Sfx.GoScene);
            }

            fadeCount += 0.01f;
            fadeImage.color = new Color(0, 0, 0, fadeCount);

            if (fadeCount >= 1)
            {
                if (nextScene == (int)Scenes.BattleScene)
                    Destroy(Player.DestroyEnemy().gameObject);

                SceneManager.LoadScene(nextScene);
                fadeOn = false;
                IsPlaying = true;
            }
        }
    }

    /// <summary>
    /// 페이드 시작
    /// </summary>
    /// <param name="scene">이동할 씬</param>
    public void ActionFade(int scene)
    {
        IsPlaying = false;
        fadeOn = true;
        nextScene = scene;
    }
}
