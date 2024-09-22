using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TileMap;

public class DecoyScript : RobotBaseScript
{

    private void Awake()
    {
        GetAllComponent();
        AssignUnit();
    }
    private void Start()
    {
        UpdateDisplay();
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.decoy);
    }

    public override void AssignUnit()
    {
        team = Team.Ally;
        base.unitType = Unit.Decoy;
        base.health = 1;
        deployNumber = gridCombatSystem.totalRobotDeployed;
        JustDeployed = true;
        attackPhase = 1;
    }

    public override void UnitSelected(Action isSelected)
    {
        showDecoyVisual();
        isSelected?.Invoke();
    }

    public override IEnumerator UseUtility(Vector2 FiringDirection, Action onUse)
    {
        yield return null;
    }

    public void showDecoyVisual()
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        TileMap.TilemapObject tilemapObject;
        Vector2Int unitPosition = grid.GetXY(base.GetPosition());

        int unitX = unitPosition.x;
        int unitY = unitPosition.y;

        // bersihin semua dulu
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                tilemapObject = grid.GetGridObject(x, y);
                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
            }
        }
        tilemapObject = grid.GetGridObject(unitX, unitY);
        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);

        attackVisual.ShowHeatMapVisual();

    }

}
