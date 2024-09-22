using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button continueButton;
    public GameObject optionsPanel;
    public GameObject newgamePanel;
    public GameObject exitPanel;

    private void Awake()
    {
        continueButton = GameObject.Find("Continue").GetComponent<Button>();
        optionsPanel = GameObject.Find("OptionsPanel");
        exitPanel = GameObject.Find("ExitPanel");
        newgamePanel = GameObject.Find("NewGamePanel");
    }

    private void Start()
    {
        CheckForContinue();
        TurnOffAllPanel();
    }
    #region NewGamePressed
    public void NewGamePressed()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        if (CheckForNewGame())
        {
            SceneManager.LoadScene("MapScene");
        }
        else
        {
            audioManager = AudioManager.Instance;
            audioManager.PlaySFX(audioManager.openOverlay);
            newgamePanel.SetActive(true);
        }
        
    }
    public bool CheckForNewGame()
    {

        if (GameManager.levelUnlocked == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    public void YesNewGamePressed()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        PlayerPrefs.SetInt("LevelUnlock", 0);
        GameManager.levelUnlocked = PlayerPrefs.GetInt("LevelUnlock");
        SceneManager.LoadScene("MapScene");
    }
    public void NoNewGamePressed()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        newgamePanel.SetActive(false);
    }
    #endregion

    #region ContinuePressed
    public void ContinuePressed()
    {
        SceneManager.LoadScene("MapScene");
    }
    public void CheckForContinue()
    {
        if (GameManager.levelUnlocked == 0)
        {
            continueButton.interactable = false;
        }
        else
        {
            continueButton.interactable = true;
        }
    }
    #endregion

    #region OptionsPressed
    public void OptionsPressed()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        optionsPanel.SetActive(true);
    }
    public void OptionExitPressed()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        optionsPanel.SetActive(false);
    }

    #endregion

    #region ExitPressed
    public void ExitPressed()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        exitPanel.SetActive(true);
    }
    public void YesExitPressed()
    {
        Application.Quit();
    }
    public void NoExitPressed()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        exitPanel.SetActive(false);
    }
    #endregion

    #region normal function
    public void TurnOffAllPanel()
    {
        optionsPanel.SetActive(false);
        exitPanel.SetActive(false);
        newgamePanel.SetActive(false);
    }
    #endregion
    
    
}
