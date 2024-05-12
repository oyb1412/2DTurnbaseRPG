using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ���� �� ����
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
    //���̵� �̹���
    [SerializeField] private Image fadeImage;
    //�÷��̾� ������Ʈ
    [SerializeField] private GameObject player;
    //���� ������
    [SerializeField] private GameData[] gameData;
    //�÷��̾� ���� ��� �ؽ�Ʈ
    private Text[] playerLevelText;
    //�÷��̾� ������ ��� �̹���
    private Image[] playerIconImage;
    //�÷��̾� ü�� �� ���� ��� �����̴�
    private Slider[,] PlayerIconSlider;
    //���̵� Ÿ�̸�
    private float fadeCount;
    //���̵� ����
    private bool fadeOn;
    //���� ��(����)
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
    /// �÷��̾� UI �ʱ�ȭ
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
    /// ���̵� ���İ� ����
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
    /// ���̵� ����
    /// </summary>
    /// <param name="scene">�̵��� ��</param>
    public void ActionFade(int scene)
    {
        fadeOn = true;
        nextScene = scene;
    }

    
}
