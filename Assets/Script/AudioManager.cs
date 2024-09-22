using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("----Audio Source----")]
    [SerializeField]AudioSource musicSource;
    [SerializeField]AudioSource SFXSource;

    [Header("----Audio Clip----")]
    public AudioClip backGround;
    public AudioClip unitOnClick;
    public AudioClip unitDamaged;
    public AudioClip buttonClick;
    public AudioClip openOverlay;
    public AudioClip deployUnit;
    public AudioClip cannon;
    public AudioClip artillery;
    public AudioClip factory;
    public AudioClip flinger;
    public AudioClip lasher;
    public AudioClip spitter;
    public AudioClip bulk;
    public AudioClip moleman;
    public AudioClip decoy;
    public AudioClip monsterDeath;
    public AudioClip robotDeath;
    public AudioClip win;
    public AudioClip lose;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // Destroy this instance because the singleton already exists
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Make sure this instance persists across scenes
        }
    }

    private void Start()
    {
        //musicSource.clip = backGround;
        //musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
