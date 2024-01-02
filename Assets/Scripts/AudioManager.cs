using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--Instance--")]
    public static AudioManager instance;

    [Header("--Bgm--")]
    public Bgm bgm;
    public AudioClip[] bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    public enum Bgm { Field,Battle,Village}

    [Header("--Sfx--")]
    int sfxChannels = 10;
    public AudioClip[] sfxClips;
    public float sfxVolume;
    AudioSource[] sfxPlayers;
    public Sfx sfx;
    public enum Sfx { Walk,GoBattle,GoVillage,UseGold,ExitVillage,Select,StartTurn,MouseOn,WizardAttack,WizardSkill
    ,MihoAttack,MihoSkill,Heal,Lose,Win}

    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
        InitBgm();
        InitSfx();
    }
   
    void InitBgm()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip[(int)Bgm.Field];
    }

    void InitSfx()
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

    public void PlayerBgm(Bgm bgm, bool islive)
    {
        bgmPlayer.clip = bgmClip[(int)bgm];
        if (islive)
            bgmPlayer.Play();
        else
            bgmPlayer.Stop();
    }

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
