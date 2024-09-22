using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static TileMap;

public class DragDropUnit : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] private GameObject gameObjectPrefab;
    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    public TileMapTesting tilemapTesting;
    private InventorySystem inventorySystem;
    private PossibleDropVisual possibleDropVisual;
    private GridCombatSystem gridCombatSystem;
    private Vector2 originalPosition;
    private bool isOutside;
    private bool isBlocking;
    


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        tilemapTesting = GameObject.Find("TileMapTesting").GetComponent<TileMapTesting>();
        inventorySystem = GetComponentInParent<InventorySystem>();
        possibleDropVisual = GameObject.Find("PossibleDropVisual").GetComponent<PossibleDropVisual>();
        gridCombatSystem = GameObject.Find("GridCombatSystem").GetComponent<GridCombatSystem>();
    }

    private void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
    }
    private void Update()
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        TileMap.TilemapObject tilemapObject = grid.GetGridObject(rectTransform.position);
        if(tilemapObject == null)
        {
            isOutside = true;
            isBlocking = true;
        }
        else
        {
            isBlocking = tilemapObject.isBlocking;
            isOutside = false;
        }
        

        //Debug.Log(isOutside);
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (gridCombatSystem.state == GridCombatSystem.State.Waiting)
            return;
        //Show area yang bisa dan gabisa di drop
        possibleDropVisual.ShowHeatMapVisual();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (gridCombatSystem.state == GridCombatSystem.State.Waiting)
            return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (gridCombatSystem.state == GridCombatSystem.State.Waiting)
            return;
        //ilangin grafik yang bisa dan gabisa di dropnya
        possibleDropVisual.HideHeatMapVisual();
        if (isOutside || isBlocking)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
        else
        {
            // instantiate prefab ke grid
            Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
            TileMap.TilemapObject tilemapObject = grid.GetGridObject(rectTransform.position);
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 gridposition = tilemapTesting.tilemap.GetGrid().GetWorldPosition(tilemapTesting.tilemap.GetTilemapLocation(mouseWorldPosition).x, tilemapTesting.tilemap.GetTilemapLocation(mouseWorldPosition).y);
            gridposition.x = gridposition.x + grid.GetCellSize() / 2;
            gridposition.y = gridposition.y + grid.GetCellSize() / 2;
            gridposition.z = -3f;
            rectTransform.anchoredPosition = originalPosition;

            // update grid information
            GameObject hasilprefab = Instantiate(gameObjectPrefab, gridposition, Quaternion.identity);
            hasilprefab.transform.SetParent(GameObject.Find("Enviroment").transform);
            UnitGridCombat unitGridCombat = hasilprefab.GetComponent<UnitGridCombat>();
            tilemapTesting.tilemap.GetGrid().GetGridObject(unitGridCombat.GetPosition()).SetUnitGridCombat(unitGridCombat);

            //kasi sound
            AudioManager audioManager = AudioManager.Instance;
            audioManager.PlaySFX(audioManager.deployUnit);

            //update list di GridCombatSystem
            gridCombatSystem.unitGridCombatList.Add(unitGridCombat);
            gridCombatSystem.totalRobotDeployed++;

            //Ganti turnnya
            gridCombatSystem.ClickingButton();

            //remove gameobjectnya
            inventorySystem.RemoveItemFromInventory(this.gameObject);

            
            Debug.Log("done");
        }
    }

    public IEnumerator gantiTurn()
    {
        yield return null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {

    }

    public void UpdateOriginalPosition()
    {
        originalPosition = rectTransform.anchoredPosition;
    }
}
