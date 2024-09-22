using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public TileMapTesting tileMapTesting;
    public GameObject tutorialPanel;
    public Image tutorialImage;
    public TMP_Text tutorialText;
    public TMP_Text tutorialTitle;
    public Button nextButton;
    public Button prevButton;
    public Button exitButton;
    public TutorialInfo[] tutorialInfoArray;
    public TutorialInfo tutorialInfo;
    public int totalPanel;
    public int currentPanel;
      
    private void Awake()
    {
        tutorialPanel = GameObject.Find("TutorialPanel");
        tutorialImage = GameObject.Find("TutorialImage").GetComponent<Image>();
        tutorialText = GameObject.Find("DialogText").GetComponent<TMP_Text>();
        tutorialTitle = GameObject.Find("TutorialTitleText").GetComponent<TMP_Text>();
        tileMapTesting = GameObject.Find("TileMapTesting").GetComponent<TileMapTesting>();
        nextButton = GameObject.Find("NextButtonTutorial").GetComponent <Button>();
        prevButton = GameObject.Find("PrevButtonTutorial").GetComponent <Button>();
        exitButton = GameObject.Find("ExitButtonTutorial").GetComponent <Button>();
        tutorialInfo = CheckForTutorial();
    }
    void Start()
    {
        if (tutorialInfo != null)
        {
            ExecuteTutorial();
        }
        else
        {
            PlayGame();
        }
    }

    
    void Update()
    {
        
    }

    public void ExecuteTutorial()
    {
        totalPanel = tutorialInfo.tutorialText.Count();
        currentPanel = 0;
        CheckForButton();
        UpdateTutorialView();
    }
    
    public void UpdateTutorialView()
    {
        tutorialText.text = tutorialInfo.tutorialText[currentPanel];
        tutorialTitle.text = tutorialInfo.tutorialTitle[currentPanel];
        tutorialImage.sprite = tutorialInfo.tutorialImage[currentPanel];
    }

    public void NextClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        currentPanel++;
        CheckForButton();
        UpdateTutorialView();
    }

    public void PrevClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        currentPanel--;
        CheckForButton();
        UpdateTutorialView();
    }

    public void ExitClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        PlayGame();
    }

    public void CheckForButton()
    {
        if(currentPanel == totalPanel - 1)
        {
            exitButton.interactable = true;
            prevButton.interactable = false;
            nextButton.interactable = false;
        }
        else if(currentPanel == 0)
        {
            exitButton.interactable = false;
            prevButton.interactable = false;
            nextButton.interactable = true;
        }
        else if(currentPanel == totalPanel-1)
        {
            nextButton.interactable = false;
            prevButton.interactable = true;
            exitButton.interactable = true;
        }
        else
        {
            exitButton.interactable = false;
            prevButton.interactable = true;
            nextButton.interactable = true;
        }
    }
    public void PlayGame()
    {
        tileMapTesting.Initialize();
        tutorialPanel.SetActive(false);
    }

    public TutorialInfo CheckForTutorial()
    {
        for(int i = 0;i < tutorialInfoArray.Length; i++)
        {
            if (tutorialInfoArray[i].level == GameManager.levelPlayed)
            {
                return tutorialInfoArray[i];
            }
        }
        return null;
    }


    [System.Serializable]
    public class TutorialInfo{
        public int level;
        public string[] tutorialTitle;       
        public Sprite[] tutorialImage;
        public string[] tutorialText;
    }
}
