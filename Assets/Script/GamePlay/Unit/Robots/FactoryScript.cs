using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TileMap;

public class FactoryScript : RobotBaseScript
{
    public GameObject decoy;
    public GameObject decoyBullet;

    private void Awake()
    {
        GetAllComponent();
        AssignUnit();
    }
    private void Start()
    {
        UpdateDisplay();
    }

    public override void AssignUnit()
    {
        StartCoroutine(CheckForFacing());
        team = Team.Ally;
        base.unitType = Unit.Factory;
        base.health = 2;
        deployNumber = gridCombatSystem.totalRobotDeployed;
        JustDeployed = true;
        attackPhase = 1;
    }

    public override void UnitSelected(Action isSelected)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.unitOnClick);
        showFactoryVisual();
        isSelected?.Invoke();
    }

    public override IEnumerator UseUtility(Vector2 FiringDirection, Action onUse)
    {
        attackVisual.HideHeatMapVisual();

        CheckForDirection(FiringDirection);
        yield return StartCoroutine(CheckForFacing());
        yield return StartCoroutine(AttackEnemy(FiringDirection));
        
        onUse?.Invoke();
    }

    public void CheckForDirection(Vector2 FiringDirection)
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(base.GetPosition());

        int unitX = unitPosition.x;
        int unitY = unitPosition.y;
        if (FiringDirection.x > unitX)
        {
            facing = Direction.Right;
        }
        //kiri
        else if (FiringDirection.x < unitX)
        {
            facing = Direction.Left;
        }

        //atas  
        if (FiringDirection.y > unitY)
        {
            facing = Direction.Up;
        }
        //bawah
        else if (FiringDirection.y < unitY)
        {
            facing = Direction.Down;
        }
    }

    public void showFactoryVisual()
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(base.GetPosition());

        int unitX = unitPosition.x;
        int unitY = unitPosition.y;

        // bersihin semua dulu
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TileMap.TilemapObject tilemapObject = grid.GetGridObject(x, y);
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
            }
        }

        //ke kiri
        for (int x = unitX; x >= 0; x--)
        {
            TileMap.TilemapObject tilemapObject = grid.GetGridObject(x, unitY);
            //kalo x engga di tempat unitnya
            if (x != unitX)
            {
                //kalo ngeblock
                if (tilemapObject.isBlocking)
                {
                    UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                    //yang ngeblock unit
                    if (unitGridCombat)
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                    }
                    //yang ngeblock terrain
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                    }
                    break;
                }
                //kalo ga ngeblock
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                //Debug.Log(tilemapObject.GetTilemapLocationFloat());
            }
            //kalo x ditempat unitnya
            else
            {
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
            }
        }

        //ke kanan
        for (int x = unitX; x < grid.GetWidth(); x++)
        {
            TileMap.TilemapObject tilemapObject = grid.GetGridObject(x, unitY);
            //kalo x engga di tempat unitnya
            if (x != unitX)
            {
                //kalo ngeblock
                if (tilemapObject.isBlocking)
                {
                    UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                    //yang ngeblock unit
                    if (unitGridCombat)
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                    }
                    //yang ngeblock terrain
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                    }
                    break;
                }
                //kalo ga ngeblock
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                //Debug.Log(tilemapObject.GetTilemapLocationFloat());
            }
            //kalo x di tempat unitnya
            else
            {
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
            }
        }

        //ke bawah
        for (int y = unitY; y >= 0; y--)
        {
            TileMap.TilemapObject tilemapObject = grid.GetGridObject(unitX, y);
            //Debug.Log(tilemapObject.GetTilemapLocationFloat());
            if (y != unitY)
            {
                if (tilemapObject.isBlocking)
                {
                    UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                    //yang ngeblock unit
                    if (unitGridCombat)
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                    }
                    //yang ngeblock terrain
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                    }
                    break;
                }
                //kalo ga ngeblock
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
            }
            else
            {
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
            }
        }

        //ke atas
        for (int y = unitY; y < grid.GetHeight(); y++)
        {
            TileMap.TilemapObject tilemapObject = grid.GetGridObject(unitX, y);
            //Debug.Log(tilemapObject.GetTilemapLocationFloat());
            if (y != unitY)
            {
                if (tilemapObject.isBlocking)
                {
                    UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                    //yang ngeblock unit
                    if (unitGridCombat)
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                    }
                    //yang ngeblock terrain
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                    }
                    break;
                }
                //kalo ga ngeblock
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
            }
            else
            {
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
            }
        }

        attackVisual.ShowHeatMapVisual();

    }

    public Vector2 SearchNearestEnemy(Vector2 FiringDirection)
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(base.GetPosition());
        TileMap.TilemapObject tilemapObject;
        UnitGridCombat unitGridCombat;

        int unitX = unitPosition.x;
        int unitY = unitPosition.y;
        //nyari direction tembakan
        //kanan
        if (FiringDirection.x > unitX)
        {
            //kalo ketemu unit
            for (int x = unitX + 1; x < grid.GetWidth(); x++)
            {
                tilemapObject = grid.GetGridObject(x, unitY);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (unitGridCombat)
                {
                    return new Vector2(x, unitY);
                }
                else if (tilemapObject.isBlocking)
                {
                    return new Vector2(x, unitY);
                }
            }
            //kalo ga ketemu unit
            return new Vector2(grid.GetWidth() - 1, unitY);
        }
        //kiri
        else if (FiringDirection.x < unitX)
        {
            //kalo ketemu unit
            for (int x = unitX - 1; x >= 0; x--)
            {
                tilemapObject = grid.GetGridObject(x, unitY);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (unitGridCombat)
                {
                    return new Vector2(x, unitY);
                }
                else if (tilemapObject.isBlocking)
                {
                    return new Vector2(x, unitY);
                }
            }
            //kalo ga ketemu unit
            return new Vector2(0, unitY);
        }

        //atas  
        if (FiringDirection.y > unitY)
        {
            //kalo ketemu unit
            for (int y = unitY + 1; y < grid.GetHeight(); y++)
            {
                tilemapObject = grid.GetGridObject(unitX, y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (unitGridCombat)
                {
                    return new Vector2(unitX, y);
                }
                else if (tilemapObject.isBlocking)
                {
                    return new Vector2(unitX, y);
                }
            }
            //kalo ga ketemu unit
            return new Vector2(unitX, grid.GetHeight() - 1);
        }
        //bawah
        else if (FiringDirection.y < unitY)
        {
            //kalo ketemu unit
            for (int y = unitY - 1; y >= 0; y--)
            {
                tilemapObject = grid.GetGridObject(unitX, y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (unitGridCombat)
                {
                    return new Vector2(unitX, y);
                }
                else if (tilemapObject.isBlocking)
                {
                    return new Vector2(unitX, y);
                }
            }
            //kalo ga ketemu unit
            return new Vector2(unitX, 0);
        }

        return Vector2.zero;
    }

    public IEnumerator AttackEnemy(Vector2 enemyLocation)
    {
        AnimateAttackOnce();        
        yield return new WaitForSeconds(0.75f);
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.factory);
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        GameObject bullet = Instantiate(decoyBullet, transform.position, Quaternion.identity);
        SpitterBulletScript spitterBulletScript = bullet.GetComponent<SpitterBulletScript>();
        Vector3 gridposition = grid.GetWorldPosition(Mathf.FloorToInt(enemyLocation.x), Mathf.FloorToInt(enemyLocation.y));
        gridposition.x = gridposition.x + grid.GetCellSize() / 2;
        gridposition.y = gridposition.y + grid.GetCellSize() / 2;
        gridposition.z = -3f;
        yield return StartCoroutine(spitterBulletScript.ShootTo(gridposition));
        SpawnDecoy(Mathf.FloorToInt(enemyLocation.x), Mathf.FloorToInt(enemyLocation.y));
    }

    public void SpawnDecoy(int x, int y)
    {
        Grid<TilemapObject> grid = tilemapTesting.GetGrid();
        Vector3 position = grid.GetWorldPosition(x, y);
        position.x = position.x + grid.GetCellSize() / 2;
        position.y = position.y + grid.GetCellSize() / 2;
        position.z = -3f;

        // update grid information
        GameObject hasilprefab = Instantiate(decoy, position, Quaternion.identity);
        hasilprefab.transform.SetParent(GameObject.Find("Enviroment").transform);
        UnitGridCombat unitGridCombat = hasilprefab.GetComponent<UnitGridCombat>();
        tilemapTesting.tilemap.GetGrid().GetGridObject(unitGridCombat.GetPosition()).SetUnitGridCombat(unitGridCombat);

        //update list di GridCombatSystem
        gridCombatSystem.unitGridCombatList.Add(unitGridCombat);
        gridCombatSystem.totalRobotDeployed++;
    }
}
