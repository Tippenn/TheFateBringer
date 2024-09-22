using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    public int actClicked;
    public int levelDisplayed;
    public Vector2Int levelLimitDisplayer;

    public LevelDisplayAnimation levelDisplayAnimation;
    public GameObject nextButton;
    public GameObject prevButton;
    public GameObject LevelSelect;
    public TMP_Text LevelName;
    public Image LevelImage;
    public Image LevelBorder;
    public Button LevelClick;
    public InventoryList inventoryList;

    public LevelData[] levelData;
    private void Awake()
    {
        LevelSelect = GameObject.Find("LevelSelect");
        LevelName = GameObject.Find("LevelName").GetComponent<TMP_Text>();
        LevelImage = GameObject.Find("LevelImage").GetComponent<Image>();
        LevelBorder = GameObject.Find("Border").GetComponent<Image>();
        LevelClick = GameObject.Find("Border").GetComponent<Button>();
        nextButton = GameObject.Find("NextButton");
        prevButton = GameObject.Find("PrevButton");
        levelDisplayAnimation = GameObject.Find("LevelImage").GetComponent<LevelDisplayAnimation>();
        inventoryList = GameObject.Find("InventoryList").GetComponent<InventoryList>();
    }

    private void Start()
    {
        LevelSelect.SetActive(false);
    }

    public void BackButton()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        LevelImage.sprite = null;
        LevelSelect.SetActive(false);
    }

    public void NextButton()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        if (levelDisplayed+1 > levelLimitDisplayer.x)
        {
            prevButton.SetActive(true);
        }

        if (levelDisplayed+1 >= levelLimitDisplayer.y)
        {
            nextButton.SetActive(false);
        }
        levelDisplayed++;
        UpdateDisplay();
        
    }

    public void PrevButton()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        if (levelDisplayed-1 < levelLimitDisplayer.y)
        {
            nextButton.SetActive(true);
        }

        if (levelDisplayed-1 <= levelLimitDisplayer.x)
        {
            prevButton.SetActive(false);
        }
        levelDisplayed--;
        UpdateDisplay();
    }
    
    public void LevelClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        GameManager.levelPlayed = levelDisplayed;
        LevelLoader levelLoader = GameObject.Find("LoadingScreen").GetComponent<LevelLoader>();
        levelLoader.LoadScene("GameplayScene");
    }

    public void LevelSelectOpened()
    {       
        switch(actClicked)
        {
            case 1:
                levelDisplayed = 0;
                levelLimitDisplayer = new Vector2Int(0, 9);
                break;
            case 2:
                levelDisplayed = 10;
                levelLimitDisplayer = new Vector2Int(10, 19);
                break;
            case 3:
                levelDisplayed = 20;
                levelLimitDisplayer = new Vector2Int(20, 34);
                break;
        }       
        LevelSelect.SetActive(true);
        prevButton.SetActive(false);
        nextButton.SetActive(true);
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if(levelDisplayed <= GameManager.levelUnlocked)
        {
            //Debug.Log("masuk");
            LevelName.text = levelData[levelDisplayed].levelName;
            LevelImage.sprite = levelData[levelDisplayed].levelImage;          
            LevelClick.interactable = true;
            inventoryList.Load("InventoryInfo/save_" + levelDisplayed);

        }
        else
        {
            LevelName.text = "level Locked";
            LevelClick.interactable = false;
            inventoryList.ClearUnit();
        }
        levelDisplayAnimation.UpdateLevelDisplayView();
        
    }

    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public Sprite levelImage;
    }
}
