using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class VillageGameManager : MonoBehaviour
{
    public enum Scenes { FieldScene, BattleScene, VillageScene }

    public static VillageGameManager instance;
    public Image fadeImage;
    public bool fadeTrigger;
    public bool isPlaying;
    float fadeCount;
    public bool fadeOn;
    int nextScene;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        AudioManager.instance.PlayerBgm(AudioManager.Bgm.Village, true);

        isPlaying = true;
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
}
