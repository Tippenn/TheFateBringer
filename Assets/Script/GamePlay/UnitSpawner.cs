using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    private Grid<TileMap.TilemapObject> grid;
    private TileMapTesting tileMapTesting;
    private GridCombatSystem gridCombatSystem;
    public List<GameObject> giantList;

    private void Awake()
    {
        tileMapTesting = GameObject.Find("TileMapTesting").GetComponent<TileMapTesting>();
        gridCombatSystem = GameObject.Find("GridCombatSystem").GetComponent<GridCombatSystem>();
    }
    
    public void Initialize()
    {
        grid = tileMapTesting.GetGrid();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                bool unitExist = grid.GetGridObject(x, y).unitExist;
                if (unitExist)
                {
                    UnitGridCombat.Unit unitType = grid.GetGridObject(x, y).unitType;
                    foreach (GameObject prefabUnit in giantList)
                    {
                        UnitGridCombat giantUnit = prefabUnit.GetComponent<UnitGridCombat>();
                        if (unitType == giantUnit.unitType)
                        {
                            SpawnUnit(x, y, prefabUnit);
                            break;
                        }
                    }
                }
                else
                {
                    //Debug.Log("no unit");
                }
            }
        }
    }

    public void SpawnUnit(int x,int y, GameObject prefab)
    {
        // instantiate prefab ke grid
        Grid<TileMap.TilemapObject> grid = tileMapTesting.GetGrid();
        TileMap.TilemapObject tilemapObject = grid.GetGridObject(x,y);
        Vector3 gridposition = tileMapTesting.tilemap.GetGrid().GetWorldPosition(x, y);
        gridposition.x = gridposition.x + grid.GetCellSize() / 2;
        gridposition.y = gridposition.y + grid.GetCellSize() / 2;
        gridposition.z = -3f;
        GameObject hasilprefab = Instantiate(prefab, gridposition, Quaternion.identity);

        //update info dari unitgridCombat
        UnitGridCombat unitGridCombat = hasilprefab.GetComponent<UnitGridCombat>();
        unitGridCombat.health = tilemapObject.unitHealth;
        unitGridCombat.order = tilemapObject.unitOrder;
        //hasilprefab.GetComponent<UnitGridCombat>().health = TounitGridCombat.health;

        // update grid information
        hasilprefab.transform.SetParent(GameObject.Find("Enviroment").transform);
        unitGridCombat = hasilprefab.GetComponent<UnitGridCombat>();
        tileMapTesting.tilemap.GetGrid().GetGridObject(unitGridCombat.GetPosition()).SetUnitGridCombat(unitGridCombat);


    }

}
