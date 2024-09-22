using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CannonScript : RobotBaseScript
{
    public GameObject prefab;

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
        base.unitType = Unit.Cannon;
        base.health = 1;
        deployNumber = gridCombatSystem.totalRobotDeployed;       
        JustDeployed = true;
        attackPhase = 1;
    }

    public override void UnitSelected(Action isSelected)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.unitOnClick);
        showCannonVisual();
        isSelected?.Invoke();
    }

    public override IEnumerator UseUtility(Vector2 FiringDirection,Action onUse)
    {
        attackVisual.HideHeatMapVisual();

        Vector2 enemyLocation = SearchNearestEnemy(FiringDirection);
        yield return StartCoroutine(CheckForFacing());
        yield return new WaitForSeconds(0.25f);
        yield return StartCoroutine(AttackEnemy(enemyLocation));
               
        onUse?.Invoke();
    }

    public void showCannonVisual()
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
            facing = Direction.Right;
            //kalo ketemu unit
            for(int x = unitX+1; x < grid.GetWidth(); x++)
            {
                tilemapObject = grid.GetGridObject(x,unitY);
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
        else if(FiringDirection.x < unitX)
        {
            facing = Direction.Left;
            //kalo ketemu unit
            for (int x = unitX-1; x >= 0; x--)
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
            facing = Direction.Up;
            //kalo ketemu unit
            for (int y = unitY+1; y < grid.GetHeight(); y++)
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
            return new Vector2(unitX, grid.GetHeight()-1);
        }
        //bawah
        else if(FiringDirection.y < unitY)
        {
            facing = Direction.Down;
            //kalo ketemu unit
            for (int y = unitY-1; y >= 0; y--)
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
        yield return new WaitForSeconds(0.5f);
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.cannon);
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        TileMap.TilemapObject tilemapObject = grid.GetGridObject(Mathf.RoundToInt(enemyLocation.x), Mathf.RoundToInt(enemyLocation.y));
        //tilemapObject.GetTilemapInformation();
        UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
        
        //Debug.Log("masuk1");
        if (unitGridCombat)
        {
            
            GameObject bullet = Instantiate(prefab, transform.position, Quaternion.identity);
            SpitterBulletScript spitterBulletScript = bullet.GetComponent<SpitterBulletScript>();
            yield return StartCoroutine(spitterBulletScript.ShootTo(unitGridCombat.GetPosition()));
            unitGridCombat.TakeDamage(1);
        }
        else
        {   
            Vector3 position = grid.GetWorldPosition(Mathf.RoundToInt(enemyLocation.x), Mathf.RoundToInt(enemyLocation.y));
            position.x = position.x + grid.GetCellSize() / 2;
            position.y = position.y + grid.GetCellSize() / 2;
            position.z = -3f;
            GameObject bullet = Instantiate(prefab, transform.position, Quaternion.identity);
            SpitterBulletScript spitterBulletScript = bullet.GetComponent<SpitterBulletScript>();
            yield return StartCoroutine(spitterBulletScript.ShootTo(position));
        }
    }
}
