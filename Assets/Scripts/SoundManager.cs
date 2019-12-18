using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;

public class SoundManager : MonoBehaviour
{
    #region Public Fields
    public static SoundManager Instance;
    #endregion

    public AudioSource SceneAudioSource;//全場景音效
    public AudioSource EffectsSource;//個人音效

    public float fadeTime = 1;
	public AudioClip ClikUI;
    public AudioClip BGM;
    public AudioClip TeachBGM;
    public AudioClip Occupation;
    public AudioClip Win;
    public AudioClip Lose;
    public AudioClip TowerFall;
    public AudioClip Bounce;
    public AudioClip FinishGame;

    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }
    public void PlayEffectSound(AudioClip clip)//個人音效
    {
        EffectsSource.PlayOneShot(clip);
    }
    public void StopEffectSound()//停止個人音效
    {
        EffectsSource.Stop();
    }


    // Play a single clip through the music source.
    public void PlaySceneEffect(AudioClip clip)//全場景音效
    {
        SceneAudioSource.PlayOneShot(clip);
    }
    public void PlaySceneBGM(AudioClip clip){
        SceneAudioSource.Stop();
        SceneAudioSource.clip = clip;
        SceneAudioSource.Play();
    }

    public void FadeOutBGM()
    {
        StartCoroutine(lerpBGMvolumeScale(fadeTime, "Out"));
    }

    public void FadeInBGM()
    {
        StartCoroutine(lerpBGMvolumeScale(fadeTime, "In"));
    }

    IEnumerator lerpBGMvolumeScale(float FadeTime, String InOut)
    {
        float startVolume = SceneAudioSource.volume;

        while (InOut == "Out")
        {
            SceneAudioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            if (SceneAudioSource.volume <= 0)
            { 
                SceneAudioSource.Stop();
                SceneAudioSource.volume = 1.0f;
                break;
            }
            yield return null;
        }

        while (InOut == "In")
        {
            SceneAudioSource.volume += startVolume * Time.deltaTime / FadeTime;
            yield return null;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if(SceneManager.GetActiveScene().buildIndex ==1 && EffectsSource == null){
            
        // }
    }
}
