using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Scenes { FieldScene, BattleScene, VillageScene }

    public static GameManager instance;
    public Image fadeImage;
    public bool fadeTrigger;
    public bool isPlaying;
    public GameObject[] iconPanels;
    public GameData[] gameDatas;
    float fadeCount;
    int nextScene;
    public Slider[,] sldiers;
    public Text[] texts;
    public GameObject[] winPlayerIcon;
    Text[] winPlayerText;
    Slider[,] winPlayerSldiers;
    Image[] iconPanelImge;
 
    // Start is called before the first frame update
    private void Awake()
    {
        fadeImage.gameObject.SetActive(true);
        fadeCount = 1f;
        isPlaying = false;
        fadeTrigger = false;
        instance = this;
        iconPanels[0].gameObject.SetActive(true);
        iconPanels[0].transform.parent.gameObject.SetActive(true);

        if (gameDatas[0].currentPlayerNumber == 2)
        {
            iconPanels[1].gameObject.SetActive(true);
        }
        else if (gameDatas[0].currentPlayerNumber == 3)
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

    // Update is called once per frame
    void Update()
    {
        for(int i = 0;i < iconPanels.Length;i++)
        {
            texts[i].text = "Lv." + gameDatas[i].playerLevel.ToString();
            winPlayerText[i].text = "Lv." + gameDatas[i].playerLevel.ToString();

            if (gameDatas[i].playerCurrentHp <= 0)
                iconPanelImge[i].color = Color.red;
        }

        for (int i = 0; i< gameDatas.Length;i++)
        {
            for(int j = 0; j < 2;j++)
            {
                if (j == 0)
                {
                    sldiers[i, j].value = gameDatas[i].playerCurrentHp / gameDatas[i].playerMaxHp;
                    winPlayerSldiers[i, j].value = gameDatas[i].playerCurrentHp / gameDatas[i].playerMaxHp;

                }
                else
                {
                    sldiers[i, j].value = gameDatas[i].playerCurrenMp / gameDatas[i].playerMaxMp;
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
                isPlaying = true;
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

    public void ActionFade(int scene)
    {
        fadeImage.gameObject.SetActive(true);
        fadeTrigger = true;
        nextScene = scene;
        isPlaying = false;

    }

    public void SetPanel(bool trigger)
    {
        winPlayerIcon[0].gameObject.SetActive(trigger);
        if (gameDatas[0].currentPlayerNumber == 2)
            winPlayerIcon[1].gameObject.SetActive(trigger);
        else if (gameDatas[0].currentPlayerNumber == 3)
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
