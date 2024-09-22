using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int levelUnlocked = 0;
    public static int levelPlayed = 0;

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
        levelUnlocked = PlayerPrefs.GetInt("LevelUnlock");
        Debug.Log("Level Unlocked: "+ levelUnlocked);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
