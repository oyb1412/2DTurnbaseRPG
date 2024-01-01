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
    float fadeCount;
    int nextScene;

    // Start is called before the first frame update
    private void Awake()
    {
        fadeImage.gameObject.SetActive(true);
        fadeCount = 1f;
        isPlaying = false;
        fadeTrigger = false;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(!fadeTrigger)
        {
            fadeCount -= 0.01f;
            fadeImage.color = new Color(0, 0, 0, fadeCount);
            if(fadeCount < 0)
            {
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
}
