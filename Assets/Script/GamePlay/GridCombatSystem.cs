using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using static TileMap;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UI;

public class GridCombatSystem : MonoBehaviour
{
    public UnitGridCombat unitGridCombat;
    //[HideInInspector]
    public List<UnitGridCombat> unitGridCombatList;
    [HideInInspector]
    public List<UnitGridCombat> unitToRemoveList;
    //private Grid<TilemapObject> grid;
    private TileMapTesting tilemaptesting;
    public int totalRobotDeployed;
    public Button optionsButton;
    public GameObject optionsPanel;
    public GameObject finishGamePanel;
    public GameObject losePanel;
    public GameObject winPanel;

    public State state;
    public State temp;
    public enum State
    {
        //phase 0
        Normal,
        //phase 1
        AllySelected,
        EnemySelected,
        //phase 2
        TargetSelected,
        //phase 3
        Waiting
    }

    private void Awake()
    {
        tilemaptesting = GameObject.Find("TileMapTesting").GetComponent<TileMapTesting>();
        losePanel = GameObject.Find("LoseImage");
        winPanel = GameObject.Find("WinImage");
        finishGamePanel = GameObject.Find("FinishGamePanel");
        optionsButton = GameObject.Find("OptionButton").GetComponent<Button>();
        optionsPanel = GameObject.Find("OptionsPanel");
        state = State.Waiting;
    }
    private void Start()
    {
        finishGamePanel.SetActive(false);
        losePanel.SetActive(false);
        winPanel.SetActive(false);
        optionsPanel.SetActive(false);
        
    }

    public void Initialize()
    {
        unitToRemoveList = new List<UnitGridCombat>();
        unitGridCombatList = unitGridCombatList.OrderBy(s => s.order).ToList();
        bool edit = false;
        if(edit == true)
        {
            foreach (UnitGridCombat unitGridCombat in unitGridCombatList)
            {
                //Debug.Log("ada unit");
                //snap in place
                Grid<TileMap.TilemapObject> grid = tilemaptesting.GetGrid();
                TileMap.TilemapObject tilemapObject = grid.GetGridObject(unitGridCombat.GetPosition());
                Vector3 gridposition = grid.GetWorldPosition(tilemaptesting.tilemap.GetTilemapLocation(unitGridCombat.GetPosition()).x, tilemaptesting.tilemap.GetTilemapLocation(unitGridCombat.GetPosition()).y);
                gridposition.x = gridposition.x + grid.GetCellSize() / 2;
                gridposition.y = gridposition.y + grid.GetCellSize() / 2;
                
                unitGridCombat.SnapInPlace(gridposition);

                //masukin informasi ke grid
                tilemaptesting.tilemap.GetGrid().GetGridObject(unitGridCombat.GetPosition()).SetUnitGridCombat(unitGridCombat);
                //Debug.Log(unitGridCombat.health);
            }
        }
        else
        {
            state = State.Normal;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Normal:
                if (Input.GetMouseButtonDown(0))
                {
                    Grid<TileMap.TilemapObject> grid = tilemaptesting.GetGrid();
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    TileMap.TilemapObject tilemapObject = grid.GetGridObject(mouseWorldPosition);
                    //kalo tekan diluar map
                    if (tilemapObject == null)
                    {
                        
                    }
                    //tekan didalam map
                    else
                    {                        
                        // kalo tilenya ada unit
                        unitGridCombat = tilemapObject.GetUnitGridCombat();
                        if (unitGridCombat != null)
                        {
                            //unitnya temen
                            if(unitGridCombat.team == UnitGridCombat.Team.Ally)
                            {
                                unitGridCombat.UnitSelected(() =>
                                {
                                    state = State.AllySelected;
                                });
                            }
                            //unitnya musuh
                            if (unitGridCombat.team == UnitGridCombat.Team.Enemy)
                            {
                                unitGridCombat.UnitSelected(() =>
                                {
                                    state = State.EnemySelected;
                                });
                            }
                        }
                        //tekan yang gaada unitnya
                        else
                        {
                            Debug.Log("No unit");
                        }
                    }

                    CheckForState();
                }
                break;
            case State.AllySelected:
                if (Input.GetMouseButtonDown(0))
                {
                    AttackVisual attackVisual = GameObject.Find("AttackVisual").GetComponent<AttackVisual>();
                    Grid<TileMap.TilemapObject> grid = tilemaptesting.GetGrid();
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    TileMap.TilemapObject tilemapObject = grid.GetGridObject(mouseWorldPosition);
                    RobotBaseScript robot = unitGridCombat as RobotBaseScript;
                    //kalo attack phasenya 1(YANG BUKAN FLINGER)
                    if(robot.attackPhase == 1)
                    {
                        //kalo tekan diluar map
                        if (tilemapObject == null)
                        {
                            attackVisual.HideHeatMapVisual();
                            state = State.Normal;
                        }
                        //kalo tekan didalam map
                        else
                        {
                            //kalo tekan diri sendiri
                            if (tilemapObject.attackDisplay == TilemapObject.AttackDisplay.TheUnitItself)
                            {
                                RobotBaseScript robotBaseScript = unitGridCombat as RobotBaseScript;
                                
                                if (robot.unitType == UnitGridCombat.Unit.Artillery)
                                {
                                    state = State.Waiting;
                                    StartCoroutine(robotBaseScript.UseUtility(new Vector2(tilemapObject.x, tilemapObject.y), () =>
                                    {
                                        StartCoroutine(TurnOverride());
                                    }));
                                }
                                else
                                {
                                    attackVisual.HideHeatMapVisual();
                                    state = State.Normal;
                                }
                                
                            }
                            //kalo tekan tile dalam range
                            else if (tilemapObject.attackDisplay == TilemapObject.AttackDisplay.OnSight)
                            {
                                RobotBaseScript robotBaseScript = unitGridCombat as RobotBaseScript;
                                state = State.Waiting;
                                StartCoroutine(robotBaseScript.UseUtility(new Vector2(tilemapObject.x, tilemapObject.y), () =>
                                {                                   
                                    StartCoroutine(TurnOverride());
                                }));
                            }
                            //kalo tekan tile diluar range
                            else
                            {
                                UnitGridCombat unitPressed = tilemapObject.GetUnitGridCombat();
                                //tekan tile yang ada unitnya
                                if (unitPressed != null)
                                {
                                    unitGridCombat = unitPressed;
                                    //unitnya temen
                                    if (unitGridCombat.team == UnitGridCombat.Team.Ally)
                                    {
                                        unitGridCombat.UnitSelected(() =>
                                        {
                                            state = State.AllySelected;
                                        });
                                    }
                                    //unitnya musuh
                                    if (unitGridCombat.team == UnitGridCombat.Team.Enemy)
                                    {

                                        unitGridCombat.UnitSelected(() =>
                                        {
                                            state = State.EnemySelected;
                                        });
                                    }
                                }
                                //tekan tile yang gaada unitnya
                                else
                                {
                                    attackVisual.HideHeatMapVisual();
                                    state = State.Normal;
                                }
                            }
                        }
                    }
                    //kalo attack phasenya 2 (CUMA FLINGER)
                    else
                    {
                        //kalo tekan diluar map
                        if (tilemapObject == null)
                        {
                            attackVisual.HideHeatMapVisual();
                            state = State.Normal;
                        }
                        //kalo tekan didalam map
                        else
                        {
                            //kalo tekan diri sendiri
                            if (tilemapObject.attackDisplay == TilemapObject.AttackDisplay.TheUnitItself)
                            {
                                attackVisual.HideHeatMapVisual();
                                state = State.Normal;
                            }
                            //kalo tekan tile dalam range
                            else if (tilemapObject.attackDisplay == TilemapObject.AttackDisplay.OnSight)
                            {
                                RobotBaseScript robotBaseScript = unitGridCombat as RobotBaseScript;
                                StartCoroutine(robotBaseScript.UseUtility(new Vector2(tilemapObject.x, tilemapObject.y), () =>
                                {
                                    state = State.TargetSelected;
                                }));
                            }
                            //kalo tekan tile diluar range
                            else
                            {
                                UnitGridCombat unitPressed = tilemapObject.GetUnitGridCombat();
                                //tekan tile yang ada unitnya
                                if (unitPressed != null)
                                {
                                    unitGridCombat = unitPressed;
                                    //unitnya temen
                                    if (unitGridCombat.team == UnitGridCombat.Team.Ally)
                                    {
                                        unitGridCombat.UnitSelected(() =>
                                        {
                                            state = State.AllySelected;
                                        });
                                    }
                                    //unitnya musuh
                                    if (unitGridCombat.team == UnitGridCombat.Team.Enemy)
                                    {

                                        unitGridCombat.UnitSelected(() =>
                                        {
                                            state = State.EnemySelected;
                                        });
                                    }
                                }
                                //tekan tile yang gaada unitnya
                                else
                                {
                                    attackVisual.HideHeatMapVisual();
                                    state = State.Normal;
                                }
                            }
                        }
                    }
                    
                    CheckForState();
                }
                break;
            case State.EnemySelected:
                if (Input.GetMouseButtonDown(0))
                {
                    AttackVisual attackVisual = GameObject.Find("AttackVisual").GetComponent<AttackVisual>();
                    Grid<TileMap.TilemapObject> grid = tilemaptesting.GetGrid();
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    TileMap.TilemapObject tilemapObject = grid.GetGridObject(mouseWorldPosition);
                    //kalo tekan diluar map
                    if (tilemapObject == null)
                    {
                        attackVisual.HideHeatMapVisual();
                        state = State.Normal;
                    }
                    //kalo tekan didalam map
                    else
                    {
                        //kalo tekan diri sendiri
                        if (tilemapObject.attackDisplay == TilemapObject.AttackDisplay.TheUnitItself)
                        {
                            attackVisual.HideHeatMapVisual();
                            state = State.Normal;
                        }
                        //kalo tekan tile manapun
                        else
                        {
                            UnitGridCombat unitPressed = tilemapObject.GetUnitGridCombat();
                            //tekan tile yang ada unitnya
                            if (unitPressed != null)
                            {
                                unitGridCombat = unitPressed;
                                //unitnya temen
                                if (unitGridCombat.team == UnitGridCombat.Team.Ally)
                                {
                                    unitGridCombat.UnitSelected(() =>
                                    {
                                        state = State.AllySelected;
                                    });
                                }
                                //unitnya musuh
                                if (unitGridCombat.team == UnitGridCombat.Team.Enemy)
                                {

                                    unitGridCombat.UnitSelected(() =>
                                    {
                                        state = State.EnemySelected;
                                    });
                                }
                            }
                            //tekan tile yang gaada unitnya
                            else
                            {
                                attackVisual.HideHeatMapVisual();
                                state = State.Normal;
                            }
                        }
                    }
                    CheckForState();
                }
                break;
            case State.TargetSelected:
                if (Input.GetMouseButtonDown(0))
                {
                    AttackVisual attackVisual = GameObject.Find("AttackVisual").GetComponent<AttackVisual>();
                    Grid<TileMap.TilemapObject> grid = tilemaptesting.GetGrid();
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    TileMap.TilemapObject tilemapObject = grid.GetGridObject(mouseWorldPosition);
                    RobotBaseScript robot = unitGridCombat as RobotBaseScript;
                    //kalo tekan diluar map
                    if (tilemapObject == null)
                    {
                        attackVisual.HideHeatMapVisual();
                        state = State.Normal;
                    }
                    //kalo tekan didalam map
                    else
                    {
                        //kalo tekan diri sendiri
                        if (tilemapObject.attackDisplay == TilemapObject.AttackDisplay.TheUnitItself)
                        {
                            attackVisual.HideHeatMapVisual();
                            state = State.Normal;
                        }
                        //kalo tekan tile dalam range
                        else if (tilemapObject.attackDisplay == TilemapObject.AttackDisplay.OnSight)
                        {
                            RobotBaseScript robotBaseScript = unitGridCombat as RobotBaseScript;
                            FlingerScript flingerScript = robotBaseScript as FlingerScript;
                            state = State.Waiting;
                            StartCoroutine(flingerScript.UseSecondUtility(new Vector2(tilemapObject.x, tilemapObject.y), () =>
                            {
                                StartCoroutine(TurnOverride());
                            }));
                        }
                        //kalo tekan tile diluar range
                        else
                        {
                            RobotBaseScript robotBaseScript = unitGridCombat as RobotBaseScript;
                            FlingerScript flingerScript = robotBaseScript as FlingerScript;
                            flingerScript.condition = FlingerScript.Condition.Normal;
                            UnitGridCombat unitPressed = tilemapObject.GetUnitGridCombat();
                            //tekan tile yang ada unitnya
                            if (unitPressed != null)
                            {
                                
                                unitGridCombat = unitPressed;
                                //unitnya temen
                                if (unitGridCombat.team == UnitGridCombat.Team.Ally)
                                {
                                    unitGridCombat.UnitSelected(() =>
                                    {
                                        state = State.AllySelected;
                                    });
                                }
                                //unitnya musuh
                                if (unitGridCombat.team == UnitGridCombat.Team.Enemy)
                                {

                                    unitGridCombat.UnitSelected(() =>
                                    {
                                        state = State.EnemySelected;
                                    });
                                }
                            }
                            //tekan tile yang gaada unitnya
                            else
                            {
                                attackVisual.HideHeatMapVisual();
                                state = State.Normal;
                            }
                        }
                    }

                    CheckForState();
                }
                break;
            case State.Waiting:
                break;
        }
        //if (Input.GetMouseButtonDown(1))
        //{
        //    Grid<TileMap.TilemapObject> grid = tilemaptesting.GetGrid();
        //    TileMap tilemap = tilemaptesting.tilemap;

        //    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    tilemap.GetTilemapInformation(mouseWorldPosition);
            
        //}
    }

    public void ClickingButton()
    {
        StartCoroutine(TurnOverride());
    }

    public IEnumerator TurnOverride()
    {
        Debug.Log("Turn Musuh");
        //mastiin state waiting(button -_-)
        state = State.Waiting;
        optionsButton.enabled = false;
        bool enemyisAlive = false;
        //execute semua AI
        foreach (UnitGridCombat unitGridCombat in unitGridCombatList)
        {
            GiantBaseScript giantBaseScript = unitGridCombat as GiantBaseScript;
            if (giantBaseScript != null && giantBaseScript.isDead == false)
            {
                enemyisAlive = true;
                //Debug.Log("execute AI");
                yield return StartCoroutine(giantBaseScript.ExecuteAI(() => { }));   
                CheckForFlee(giantBaseScript);
            }
        }

        //cek kalo menang
        if(enemyisAlive == false)
        {
            yield return StartCoroutine(GameWin());
        }

        //Proc Semua Enviroment (artillery shot doang as of right now)
        Grid<TileMap.TilemapObject> grid = tilemaptesting.GetGrid();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TileMap.TilemapObject tilemapObject = grid.GetGridObject(x, y);
                if (tilemapObject.artilleryShoot)
                {
                    
                    UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                    if(unitGridCombat != null)
                    {
                        unitGridCombat.TakeDamage(1);
                    }                    
                    tilemapObject.artilleryShoot = false;
                }
            }
        }

        //nge removenya baru sekarang
        foreach (UnitGridCombat unitGridCombat in unitToRemoveList)
        {
            unitGridCombatList.Remove(unitGridCombat);
            Destroy(unitGridCombat.gameObject);
        }
        unitToRemoveList = new List<UnitGridCombat>();

        //ngilangin status justDeployed
        foreach (UnitGridCombat unitGridCombat in unitGridCombatList)
        {
            //Debug.Log("ada unit");
            RobotBaseScript robotBaseScript = unitGridCombat as RobotBaseScript;
            if(robotBaseScript != null)
            {
                robotBaseScript.JustDeployed = false;
                
            }
        }

        //balikin control ke player
        //Debug.Log("Turn Kita");
        state = State.Normal;
        optionsButton.enabled = true;
    }

    public void GetInfo(TilemapObject tilemapObject)
    {
        if (tilemapObject == null)
        {
            Debug.Log("Click Diluar Map");
        }
        else
        {
            if (tilemapObject.GetUnitGridCombat() == null)
            {
                Debug.Log("gaada orang");
            }
            else
            {
                Debug.Log(tilemapObject.GetUnitGridCombat().name);
            }
        }
    }

    public void CheckForFlee(GiantBaseScript giantBaseScript)
    {
        Debug.Log("Check flee");
        if(giantBaseScript.waitingTime >= 3)
        {
            Debug.Log("flee");
            GameLose();
        }
    }

    public void GameLose()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.lose);
        state = State.Waiting;
        finishGamePanel.SetActive(true);
        losePanel.SetActive(true);
    }

    public void RetryCLicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        LevelLoader levelLoader = GameObject.Find("LoadingScreen").GetComponent<LevelLoader>();
        levelLoader.LoadScene("GamePlayScene");
    }

    public void ExitClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        LevelLoader levelLoader = GameObject.Find("LoadingScreen").GetComponent<LevelLoader>();
        levelLoader.LoadScene("MapScene");
    }

    public IEnumerator GameWin()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.win);
        state = State.Waiting;
        if(GameManager.levelUnlocked == GameManager.levelPlayed)
        {
            PlayerPrefs.SetInt("LevelUnlock", GameManager.levelUnlocked+1);
            GameManager.levelUnlocked = PlayerPrefs.GetInt("LevelUnlock");
        }
        finishGamePanel.SetActive(true);
        winPanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        LevelLoader levelLoader = GameObject.Find("LoadingScreen").GetComponent<LevelLoader>();
        levelLoader.LoadScene("MapScene");
    }

    public void OptionClicked()
    {
        temp = state;
        state = State.Waiting;
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.openOverlay);
        optionsPanel.SetActive(true);
    }

    public void ExitOptionClicked()
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.buttonClick);
        state = temp;
        optionsPanel.SetActive(false);
    }

    public void CheckForState()
    {
        //Debug.Log(state);
    }

}
