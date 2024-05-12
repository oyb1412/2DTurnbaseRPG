using UnityEngine;

/// <summary>
/// 모든 사운드 관리
/// </summary>
public class AudioManager : MonoBehaviour
{
    public enum Sfx { Walk, GoScene, UseGold, ExitVillage, Select, WizardAttack, Attack, Heal }
    public enum Bgm { Field, Battle, Village }

    [Header("--Instance--")]
    public static AudioManager instance;

    [Header("--Bgm--")]
    [SerializeField]private AudioClip[] bgmClip;
    [SerializeField]private float bgmVolume;
    private AudioSource bgmPlayer;

    [Header("--Sfx--")]
    private int sfxChannels = 10;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private float sfxVolume;
    private AudioSource[] sfxPlayers;

    private void Awake()
    {
        //싱글톤
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else {
            Destroy(gameObject);
        }
        InitBgm();
        InitSfx();
    }

    /// <summary>
    /// bgm 초기화
    /// </summary>
    private void InitBgm()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip[(int)Bgm.Field];
    }

    /// <summary>
    /// sfx 초기화
    /// </summary>
    private void InitSfx()
    {
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[sfxChannels];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].volume = sfxVolume;
            sfxPlayers[i].bypassListenerEffects = true;
        }
    }

    /// <summary>
    /// bgm 실행 및 중지
    /// </summary>
    /// <param name="bgm">실행 및 중지 할 bgm</param>
    /// <param name="islive">실행 및 중지 여부</param>
    public void PlayerBgm(Bgm bgm, bool islive)
    {
        bgmPlayer.clip = bgmClip[(int)bgm];
        if (islive)
            bgmPlayer.Play();
        else
            bgmPlayer.Stop();
    }

    /// <summary>
    /// sfx 실행
    /// </summary>
    /// <param name="sfx">실행할 sfx</param>
    public void PlayerSfx(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            if (sfxPlayers[i].isPlaying)
                continue;

            sfxPlayers[i].clip = sfxClips[(int)sfx];
            sfxPlayers[i].Play();
            break;
        }
    }
}
