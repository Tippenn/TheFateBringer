using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArtilleryScript : RobotBaseScript
{

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
        team = Team.Ally;
        base.unitType = Unit.Artillery;
        base.health = 1;
        deployNumber = gridCombatSystem.totalRobotDeployed;
        JustDeployed = true;
        attackPhase = 1;
    }

    public override void UnitSelected(Action isSelected)
    {
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.unitOnClick);
        showArtilleryVisual();
        isSelected?.Invoke();
    }

    public override IEnumerator UseUtility(Vector2 FiringDirection,Action onUse)
    {
        attackVisual.HideHeatMapVisual();
        yield return StartCoroutine(AttackEnemy(FiringDirection));
        
        onUse?.Invoke();
    }

    public void showArtilleryVisual()
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

        //set visual artillery
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TileMap.TilemapObject tilemapObject = grid.GetGridObject(x, y);
                //kalo unit itu sendiri
                if (unitX == x && unitY == y)
                {
                    tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
                }
                //kalo in range
                else if(Mathf.Abs(unitX - x) < 3 && Mathf.Abs(unitY - y) < 3)
                {
                    //kena block
                    if (tilemapObject.isBlocking == true)
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
                    }
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                    }                    
                }
                else
                {
                    tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                }
            }
        }

        attackVisual.ShowHeatMapVisual();      
    }

    public IEnumerator AttackEnemy(Vector2 enemyLocation)
    {
        animator.SetTrigger("Attacking");
        yield return new WaitForSeconds(0.5f);
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.artillery);
        yield return new WaitForSeconds(0.5f);
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        TileMap.TilemapObject tilemapObject = grid.GetGridObject(Mathf.RoundToInt(enemyLocation.x), Mathf.RoundToInt(enemyLocation.y));
        
        tilemapObject.artilleryShoot = true;
    }
}
