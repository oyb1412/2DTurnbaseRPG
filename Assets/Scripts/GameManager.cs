using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ���� �Ŵ���
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Variable
    public enum Scenes { TutorialScene, FieldScene, BattleScene, VillageScene }

    public static GameManager instance;
    //���̵� �̹���
    [SerializeField]private Image fadeImage;
    //���̵� ����
    private bool fadeTrigger;
    //���� ���� ����
    public bool IsPlaying { get; private set; }
    //�÷��̾� ������ �ǳ�
    [SerializeField]private GameObject[] iconPanels;
    //���� ������
    [SerializeField] private GameData[] gameDatas;
    //���̵� Ÿ�̸�
    private float fadeCount;
    //���� ��(����)
    private int nextScene;
    //�÷��̾� ü�� �� ���� ��� �����̴�
    private Slider[,] sldiers;
    //�÷��̾� ���� ��� �ؽ�Ʈ
    private Text[] texts;
    //�¸��� ��µǴ� �÷��̾� ������ 
    [SerializeField] private GameObject[] winPlayerIcon;
    //�¸��� ��µǴ� �÷��̾� ���� �ؽ�Ʈ
    private Text[] winPlayerText;
    //�¸��� ��µǴ� �÷��̾� ����ġ �����̴�
    private Slider[,] winPlayerSldiers;
    //�÷��̾� ������ �̹���
    private Image[] iconPanelImge;
    #endregion

    /// <summary>
    /// ���� �÷��̾� UI �ʱ�ȭ
    /// </summary>
    private void Awake()
    {
        fadeImage.gameObject.SetActive(true);
        fadeCount = 1f;
        IsPlaying = false;
        fadeTrigger = false;
        instance = this;
        iconPanels[0].gameObject.SetActive(true);
        iconPanels[0].transform.parent.gameObject.SetActive(true);

        if (gameDatas[0].CurrentPlayerNumber == 2)
        {
            iconPanels[1].gameObject.SetActive(true);
        }
        else if (gameDatas[0].CurrentPlayerNumber == 3)
        {
            iconPanels[1].gameObject.SetActive(true);
            iconPanels[2].gameObject.SetActive(true);
        }


        iconPanelImge = new Image[iconPanels.Length];
        sldiers = new Slider[iconPanels.Length, iconPanels.Length -1];
        winPlayerSldiers = new Slider[winPlayerIcon.Length , winPlayerIcon.Length -1];

        winPlayerSldiers[0,0] = winPlayerIcon[0].GetComponentsInChildren<Slider>()[0];
        winPlayerSldiers[0, 1] = winPlayerIcon[0].GetComponentsInChildren<Slider>()[1];

        winPlayerSldiers[1, 0] = winPlayerIcon[1].GetComponentsInChildren<Slider>()[0];
        winPlayerSldiers[1, 1] = winPlayerIcon[1].GetComponentsInChildren<Slider>()[1];

        winPlayerSldiers[2, 0] = winPlayerIcon[2].GetComponentsInChildren<Slider>()[0];
        winPlayerSldiers[2, 1] = winPlayerIcon[2].GetComponentsInChildren<Slider>()[1];

        sldiers[0, 0] = iconPanels[0].GetComponentsInChildren<Slider>()[0];
        sldiers[0, 1] = iconPanels[0].GetComponentsInChildren<Slider>()[1];

        sldiers[1, 0] = iconPanels[1].GetComponentsInChildren<Slider>()[0];
        sldiers[1, 1] = iconPanels[1].GetComponentsInChildren<Slider>()[1];

        sldiers[2, 0] = iconPanels[2].GetComponentsInChildren<Slider>()[0];
        sldiers[2, 1] = iconPanels[2].GetComponentsInChildren<Slider>()[1];

        texts = new Text[iconPanels.Length];
        winPlayerText = new Text[winPlayerIcon.Length];
        for(int i = 0; i < iconPanels.Length; i++)
        {
            texts[i] = iconPanels[i].GetComponentInChildren<Text>();
            winPlayerText[i] = winPlayerIcon[i].GetComponentInChildren<Text>();
            iconPanelImge[i] = iconPanels[i].GetComponentsInChildren<Image>()[1];
        }

 
    }

    void Update()
    {
        for(int i = 0;i < iconPanels.Length;i++)
        {
            texts[i].text = "Lv." + gameDatas[i].PlayerLevel.ToString();
            winPlayerText[i].text = "Lv." + gameDatas[i].PlayerLevel.ToString();

            if (gameDatas[i].PlayerCurrentHp <= 0)
                iconPanelImge[i].color = Color.red;
        }

        for (int i = 0; i< gameDatas.Length;i++)
        {
            for(int j = 0; j < 2;j++)
            {
                if (j == 0)
                {
                    sldiers[i, j].value = gameDatas[i].PlayerCurrentHp / gameDatas[i].PlayerMaxHp;
                    winPlayerSldiers[i, j].value = gameDatas[i].PlayerCurrentHp / gameDatas[i].PlayerMaxHp;

                }
                else
                {
                    sldiers[i, j].value = gameDatas[i].PlayerCurrentMp / gameDatas[i].PlayerMaxMp;
                    winPlayerSldiers[i, j].value = gameDatas[i].PlayerCurrendExp / gameDatas[i].PlayerMaxExp;
                }
            }
        }

        if (!fadeTrigger)
        {
            fadeCount -= 0.01f;
            fadeImage.color = new Color(0, 0, 0, fadeCount);
            if(fadeCount < 0)
            {
                IsPlaying = true;
                fadeImage.gameObject.SetActive(false);
                fadeCount = 0;
            }
        }

        if (fadeTrigger)
        {
            fadeCount += 0.01f;
            fadeImage.color = new Color(0, 0, 0, fadeCount);
            if (fadeCount > 1)
            {
                SceneManager.LoadScene(nextScene);
                fadeCount = 1;
                fadeTrigger = false;
            }
        }
    }

    /// <summary>
    /// ���̵� ���� �� �̵� �� ����
    /// </summary>
    /// <param name="scene">�̵��� ��</param>
    public void ActionFade(int scene)
    {
        fadeImage.gameObject.SetActive(true);
        fadeTrigger = true;
        nextScene = scene;
        IsPlaying = false;

    }

    /// <summary>
    /// �¸��� UI ���
    /// </summary>
    /// <param name="trigger"></param>
    public void SetPanel(bool trigger)
    {
        winPlayerIcon[0].gameObject.SetActive(trigger);
        if (gameDatas[0].CurrentPlayerNumber == 2)
            winPlayerIcon[1].gameObject.SetActive(trigger);
        else if (gameDatas[0].CurrentPlayerNumber == 3)
        {
            winPlayerIcon[1].gameObject.SetActive(trigger);
            winPlayerIcon[2].gameObject.SetActive(trigger);
        }

        for (int i = 0; i<iconPanels.Length; i++)
        {
            iconPanels[i].gameObject.SetActive(false);
        }
        iconPanels[0].transform.parent.gameObject.SetActive(false);
        winPlayerIcon[0].transform.parent.gameObject.SetActive(trigger);
    }
}
