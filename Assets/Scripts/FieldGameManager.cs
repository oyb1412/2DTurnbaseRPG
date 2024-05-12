using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// �ʵ� �� ����
/// </summary>
public class FieldGameManager : MonoBehaviour
{
    #region Variable
    public enum Scenes { TutorialScene, FieldScene, BattleScene, VillageScene }
    [Header("--Instance--")]
    public static FieldGameManager Instance;
    //���� ���� ��Ȳ ����
    public bool IsPlaying { get; private set; }
    //���� ��(����)
    private int nextScene;

    [Header("--UI--")]
    //���̵� �̹���
    [SerializeField]private Image fadeImage;
    //���̵� ī��Ʈ(0~1.0)
    private float fadeCount;
    //���̵� �� ����
    private bool fadeOn;
    //��� ǥ�� �ؽ�Ʈ
    [SerializeField] private Text goldText;
    //�÷��̾� �̹��� �з��� �ǳ�
    [SerializeField] private GameObject players;
    //�÷��̾� ���� ǥ�� �ؽ�Ʈ
    private Text[] playerLevelText;
    //�÷��̾� ������ ǥ�� �̹���
    private Image[] playerIconImage;
    //�÷��̾� ü�� �� ���� ǥ�� �����̴�
    [SerializeField] private Slider[,] PlayerIconSlider;
    [Header("--GameData--")]
    //���� ������
    [SerializeField] private GameData[] gameData;

    [Header("--PlayerInfo--")]
    //�������� �÷��̾� ���
    private int currentPlayerGold;
    //���� �������� �÷��̾�
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
    /// �÷��̾� ���� UI �ʱ�ȭ
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
    /// ���� ���¿� �°� ���̵� on & off
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
    /// ���̵� ����
    /// </summary>
    /// <param name="scene">�̵��� ��</param>
    public void ActionFade(int scene)
    {
        IsPlaying = false;
        fadeOn = true;
        nextScene = scene;
    }
}
