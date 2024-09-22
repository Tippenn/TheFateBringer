using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileMapTesting : MonoBehaviour
{
    public string fileNumber;
    
    public TileMap tilemap;
    public GameplayTileInformation[] tilemapInfo;
    [SerializeField] TileMapVisual tilemapVisualAct1;
    [SerializeField] TileMapVisual tilemapVisualAct2;
    [SerializeField] TileMapVisual tilemapVisualAct3;
    [SerializeField] PossibleDropVisual possibleDropVisual;
    [SerializeField] AttackVisual attackVisual;
    private TileMap.TilemapObject.TilemapSprite tilemapSprite;
    public InventorySystem inventorySystem;
    public GridCombatSystem gridCombatSystem;
    public BackGroundHandler backGroundHandler;
    public UnitSpawner unitSpawner;
    public TMP_Text tileinfo;
    public bool debug;
    private void Awake()
    {
        inventorySystem = GameObject.Find("InventoryTab").GetComponent<InventorySystem>();
        gridCombatSystem = GameObject.Find("GridCombatSystem").GetComponent<GridCombatSystem>();
        unitSpawner = GameObject.Find("GridCombatSystem").GetComponent<UnitSpawner>();
        //tileinfo = GameObject.Find("TileInfo").GetComponent<TMP_Text>();
        backGroundHandler = GameObject.Find("BackGroundGameplay").GetComponent<BackGroundHandler>();
    }

    private void Start()
    {
        int levelPlayed = GameManager.levelPlayed;
        int tileWidth = 0;
        int tileHeight = 0;
        foreach (GameplayTileInformation tileinfo in tilemapInfo)
        {
            if (tileinfo.FromToLevel.x <= levelPlayed && levelPlayed <= tileinfo.FromToLevel.y)
            {
                tileWidth = tileinfo.tileSize.x;
                tileHeight = tileinfo.tileSize.y;
                break;
            }
        }

        //Debug.Log(tileWidth + " " + tileHeight);

        if (tileHeight == 0 || tileWidth == 0)
        {
            Debug.Log("Mana bisa gitu anjg");
        }

        tilemap = new TileMap(tileWidth, tileHeight, 7f, new Vector3(-(tileWidth * 7f) / 2, (-(tileHeight * 7f) / 2)+5f));
        if(levelPlayed < 10)
        {
            backGroundHandler.SetBackground(0);
            tilemap.SetTilemapVisual(tilemapVisualAct1);
            
        }
        else if(levelPlayed < 20)
        {
            backGroundHandler.SetBackground(1);
            tilemap.SetTilemapVisual(tilemapVisualAct2);
        }
        else
        {
            backGroundHandler.SetBackground(2);
            tilemap.SetTilemapVisual(tilemapVisualAct3);
        }
        
        tilemap.SetPossibleDropVisual(possibleDropVisual);
        tilemap.SetAttackVisual(attackVisual);
        tilemap.Load("TileInfo/save_"+levelPlayed);
        inventorySystem.Load("InventoryInfo/save_" +levelPlayed);
    }
    public void Initialize()
    {
        unitSpawner.enabled = true;
        if (unitSpawner.enabled == true)
        {
            unitSpawner.Initialize();
        }

        if (gridCombatSystem.enabled == true)
        {
            gridCombatSystem.Initialize();
        }
    }
    public Grid<TileMap.TilemapObject> GetGrid()
    {
        return tilemap.GetGrid();
    }
    private void Update()
    {
        if(debug == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                tilemap.SetTilemapSprite(mouseWorldPosition, tilemapSprite);

            }
            


            if (Input.GetKeyDown(KeyCode.T))
            {
                tilemapSprite = TileMap.TilemapObject.TilemapSprite.None;
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                tilemapSprite = TileMap.TilemapObject.TilemapSprite.Ground;
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                tilemapSprite = TileMap.TilemapObject.TilemapSprite.Forest;
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                tilemapSprite = TileMap.TilemapObject.TilemapSprite.Mountain;
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                tilemapSprite = TileMap.TilemapObject.TilemapSprite.GroundDark;
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                tilemapSprite = TileMap.TilemapObject.TilemapSprite.ForestDark;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                tilemapSprite = TileMap.TilemapObject.TilemapSprite.MountainDark;
            }
            if (Input.GetKeyDown(KeyCode.G))
            {
                tilemapSprite = TileMap.TilemapObject.TilemapSprite.Dirt;
            }


            if (Input.GetKeyDown(KeyCode.S))
            {
                tilemap.Save(false);
                Debug.Log("Save no Overwrite");
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                inventorySystem.Save(false);
                Debug.Log("Save no Overwrite");
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                tilemap.Save(true);
                Debug.Log("Save Overwrite");
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                inventorySystem.Save(false);
                Debug.Log("Save no Overwrite");
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                string loadFile = "TileInfo/save_" + fileNumber;
                Debug.Log("TileInfo/"+loadFile);
                tilemap.Load(loadFile);
                
                Debug.Log("Load");
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                string loadFile = "InventoryInfo/save_" + fileNumber;
                Debug.Log("InventoryInfo/" + loadFile);
                inventorySystem.Load(loadFile);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tilemap.GetTilemapInformation(mouseWorldPosition, tileinfo);
        }

    }

    [System.Serializable]
    public class GameplayTileInformation
    {
        public Vector2Int tileSize;
        public Vector2Int FromToLevel;
    }
}
