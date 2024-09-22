using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectManager : MonoBehaviour
{
    public LevelSelectManager levelSelectManager;
    public GameObject optionsPanel;
    public GameObject fog;

    private void Awake()
    {
        levelSelectManager = GameObject.Find("LevelSelectManager").GetComponent<LevelSelectManager>();
        optionsPanel = GameObject.Find("OptionsPanel");
        fog = GameObject.Find("FogMapCloser");
    }
    private void Start()
    {
        optionsPanel.SetActive(false);
        if(GameManager.levelUnlocked > 19)
        {
            Debug.Log("Unlock Act 3");
            fog.transform.localPosition = new Vector3(550f,0f,0f);
        }
        else if(GameManager.levelUnlocked > 9)
        {
            Debug.Log("Unlock Act 2");
            fog.transform.localPosition = new Vector3(350f, 0f, 0f);
        }
        else
        {
            Debug.Log("Unlock Act 1");
            fog.transform.localPosition = new Vector3(150f, 0f, 0f);
        }
    }
    public void MapClicked(int mapId)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        OpenLevelSelect(mapId);
    }

    public void OpenLevelSelect(int mapId)
    {
        levelSelectManager.actClicked = mapId;
        levelSelectManager.LevelSelectOpened();  
    }

    public void OptionButtonClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        optionsPanel.SetActive(true);
    }

    public void ExitOptionsButtonClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        optionsPanel.SetActive(false);
    }
}
