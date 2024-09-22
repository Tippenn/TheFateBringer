using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static LasherScript;
using static TileMap;

public class ClotScript : GiantBaseScript
{
    public Direction attackDirection;
    private void Awake()
    {
        
    }
    private void Start()
    {
        GetAllComponent();        
        AssignUnit();
        makingsure();
        AddToList();
    }

    public override void AssignUnit()
    {
        team = Team.Enemy;
        unitType = Unit.Clot;
        health = 1;
        order = 99;
    }
    public override void UnitSelected(Action isSelected)
    {
        showLasherVisual();
        isSelected?.Invoke();
    }

    public override IEnumerator ExecuteAI(Action onFinish)
    {
        yield return null;
        //Debug.Log("Execute AI");
        if(state == GiantBaseScript.State.Waiting)
        {
            target = SearchForTarget();
            if(target != null)
            {
                attackDirection = SearchForDirection(target);
                state = GiantBaseScript.State.Aggro;
            }
        }
        else
        {
            RobotBaseScript temp = target;
            //nyari target yang baru di deploy
            target = SearchForTarget();
            //kalo sama berarti attack targetnya
            if(target == temp)
            {               
                AttackTarget(attackDirection);
                target = null;
                state = GiantBaseScript.State.Waiting;
            }
            //kalo engga, balik aggro
            else
            {
                attackDirection = SearchForDirection(target);
                state = GiantBaseScript.State.Aggro;
            }            
        }
        onFinish?.Invoke();
    }

    public Direction SearchForDirection(RobotBaseScript robotBaseScript)
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(base.GetPosition());
        Vector2Int targetPosition = grid.GetXY(robotBaseScript.GetPosition());

        //nyari arah musuh
        //kalo kiri
        if(targetPosition.x < unitPosition.x)
        {
            return Direction.Left;
        }
        //kalo kanan
        else if(targetPosition.x > unitPosition.x)
        {
            return Direction.Right;
        }
        //kalo bawah
        else if(unitPosition.y < unitPosition.y)
        {
            return Direction.Down;
        }
        //kalo atas
        else
        {
            return Direction.Up;
        }

    }

    public RobotBaseScript SearchForTarget()
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(GetPosition());

        int unitX = unitPosition.x;
        int unitY = unitPosition.y;

        RobotBaseScript robotBaseScript = target;
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
                        //unitnya target
                        if (unitGridCombat.team == UnitGridCombat.Team.Ally)
                        {
                            //masih nunggu
                            if(state == GiantBaseScript.State.Waiting)
                            {
                                robotBaseScript = ChooseTarget(robotBaseScript, unitGridCombat as RobotBaseScript);
                            }
                            //udah aggro ke musuh
                            else
                            {
                                robotBaseScript = ChooseNewTarget(robotBaseScript, unitGridCombat as RobotBaseScript);
                            }

                        }
                    }
                    break;
                }
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
                        //unitnya target
                        if (unitGridCombat.team == UnitGridCombat.Team.Ally)
                        {
                            //masih nunggu
                            if (state == GiantBaseScript.State.Waiting)
                            {
                                robotBaseScript = ChooseTarget(robotBaseScript, unitGridCombat as RobotBaseScript);
                            }
                            //udah aggro ke musuh
                            else
                            {
                                robotBaseScript = ChooseNewTarget(robotBaseScript, unitGridCombat as RobotBaseScript);
                            }

                        }
                    }
                    break;
                }
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
                        //unitnya target
                        if (unitGridCombat.team == UnitGridCombat.Team.Ally)
                        {
                            //masih nunggu
                            if (state == GiantBaseScript.State.Waiting)
                            {
                                robotBaseScript = ChooseTarget(robotBaseScript, unitGridCombat as RobotBaseScript);
                            }
                            //udah aggro ke musuh
                            else
                            {
                                robotBaseScript = ChooseNewTarget(robotBaseScript, unitGridCombat as RobotBaseScript);
                            }

                        }
                    }
                    break;
                }
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
                        //unitnya target
                        if (unitGridCombat.team == UnitGridCombat.Team.Ally)
                        {
                            //masih nunggu
                            if (state == GiantBaseScript.State.Waiting)
                            {
                                robotBaseScript = ChooseTarget(robotBaseScript, unitGridCombat as RobotBaseScript);
                            }
                            //udah aggro ke musuh
                            else
                            {
                                robotBaseScript = ChooseNewTarget(robotBaseScript, unitGridCombat as RobotBaseScript);
                            }

                        }
                    }
                    break;
                }
            }
        }

        return robotBaseScript;
    }

    public void AttackTarget(Direction targetDirection)
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(base.GetPosition());
        TileMap.TilemapObject tilemapObject;
        UnitGridCombat unitGridCombat;
        //kalo target dikiri
        if (targetDirection == Direction.Left)
        {
            for(int x  = unitPosition.x-1; x >= 0; x--)
            {
                tilemapObject = grid.GetGridObject(x, unitPosition.y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (unitGridCombat)
                {
                    unitGridCombat.TakeDamage(1);
                    break;
                }
            }
        }
        //kalo target dikanan
        else if(targetDirection == Direction.Right)
        {
            for (int x = unitPosition.x+1; x < grid.GetWidth(); x++)
            {
                tilemapObject = grid.GetGridObject(x, unitPosition.y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (unitGridCombat)
                {
                    unitGridCombat.TakeDamage(1);
                    break;
                }
            }
        }
        //kalo target diatas
        else if(targetDirection == Direction.Up)
        {
            for (int y = unitPosition.y+1; y < grid.GetHeight(); y++)
            {
                tilemapObject = grid.GetGridObject(unitPosition.x, y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (unitGridCombat)
                {
                    unitGridCombat.TakeDamage(1);
                    break;
                }
            }
        }
        //kalo target dibawah
        else
        {
            for (int y = unitPosition.y-1; y >= 0; y--)
            {
                tilemapObject = grid.GetGridObject(unitPosition.x, y);
                unitGridCombat = tilemapObject.GetUnitGridCombat();
                if (unitGridCombat)
                {
                    unitGridCombat.TakeDamage(1);
                    break;
                }
            }
        }
    }

    public void showLasherVisual()
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(GetPosition());

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

        // kalo status nunggu
        if(state == GiantBaseScript.State.Waiting)
        {
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
                            //unitnya temen
                            if (unitGridCombat.team == UnitGridCombat.Team.Enemy)
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                            }
                            //unitnya musuh
                            else
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                            }
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
                            //unitnya temen
                            if (unitGridCombat.team == UnitGridCombat.Team.Enemy)
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                            }
                            //unitnya musuh
                            else
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                            }
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
                            //unitnya temen
                            if (unitGridCombat.team == UnitGridCombat.Team.Enemy)
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                            }
                            //unitnya musuh
                            else
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                            }
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
                            //unitnya temen
                            if (unitGridCombat.team == UnitGridCombat.Team.Enemy)
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                            }
                            //unitnya musuh
                            else
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.OnSight);
                            }
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
        }

        //kalo udah ada target
        if(state == GiantBaseScript.State.Aggro)
        {
            TileMap.TilemapObject tilemapObject;

            //kalo target dikiri
            if (attackDirection == Direction.Left)
            {
                for(int x = unitPosition.x; x >= 0; x--)
                {
                    tilemapObject = grid.GetGridObject(x, unitPosition.y);
                    if (x != unitX)
                    {
                        //kalo ngeblock
                        if (tilemapObject.isBlocking)
                        {
                            UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                            //yang ngeblock unit
                            if (unitGridCombat)
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.EnemyAttack);
                            }
                            //yang ngeblock terrain
                            else
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                            }
                            break;
                        }
                        //kalo ga ngeblock
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.EnemyAttack);
                        //Debug.Log(tilemapObject.GetTilemapLocationFloat());
                    }
                    //kalo x ditempat unitnya
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
                    }

                }
                
            }
            //kalo target dikanan
            else if (attackDirection == Direction.Right)
            {
                for (int x = unitPosition.x; x < grid.GetWidth(); x++)
                {
                    tilemapObject = grid.GetGridObject(x, unitPosition.y);
                    if (x != unitX)
                    {
                        //kalo ngeblock
                        if (tilemapObject.isBlocking)
                        {
                            UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                            //yang ngeblock unit
                            if (unitGridCombat)
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.EnemyAttack);
                            }
                            //yang ngeblock terrain
                            else
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                            }
                            break;
                        }
                        //kalo ga ngeblock
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.EnemyAttack);
                        //Debug.Log(tilemapObject.GetTilemapLocationFloat());
                    }
                    //kalo x ditempat unitnya
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
                    }

                }
            }
            //kalo target diatas
            else if (attackDirection == Direction.Up)
            {
                for (int y = unitPosition.y; y < grid.GetHeight(); y++)
                {
                    tilemapObject = grid.GetGridObject(unitPosition.x, y);
                    if (y != unitY)
                    {
                        //kalo ngeblock
                        if (tilemapObject.isBlocking)
                        {
                            UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                            //yang ngeblock unit
                            if (unitGridCombat)
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.EnemyAttack);
                            }
                            //yang ngeblock terrain
                            else
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                            }
                            break;
                        }
                        //kalo ga ngeblock
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.EnemyAttack);
                        //Debug.Log(tilemapObject.GetTilemapLocationFloat());
                    }
                    //kalo x ditempat unitnya
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
                    }

                }
            }
            //kalo target dibawah
            else if (attackDirection == Direction.Down)
            {
                for (int y = unitPosition.y; y >= 0; y--)
                {
                    tilemapObject = grid.GetGridObject(unitPosition.x, y);
                    if (y != unitY)
                    {
                        //kalo ngeblock
                        if (tilemapObject.isBlocking)
                        {
                            UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                            //yang ngeblock unit
                            if (unitGridCombat)
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.EnemyAttack);
                            }
                            //yang ngeblock terrain
                            else
                            {
                                tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.NotOnSight);
                            }
                            break;
                        }
                        //kalo ga ngeblock
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.EnemyAttack);
                        //Debug.Log(tilemapObject.GetTilemapLocationFloat());
                    }
                    //kalo x ditempat unitnya
                    else
                    {
                        tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
                    }

                }
            }

            //target diri sendiri
            tilemapObject = grid.GetGridObject(unitPosition.x, unitPosition.y);
            tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.TheUnitItself);
        }

        attackVisual.ShowHeatMapVisual();
    }

    public RobotBaseScript ChooseTarget(RobotBaseScript unit1, RobotBaseScript unit2)
    {
        if(unit1 == null)
        {
            return unit2;
        }
        else if(unit1.deployNumber > unit2.deployNumber)
        {
            return unit2;
        }
        else
        {
            return unit1;
        }
    }

    public RobotBaseScript ChooseNewTarget(RobotBaseScript unit1, RobotBaseScript unit2)
    {
        if(unit2.JustDeployed == true)
        {
            return unit2;
        }
        else
        {
            return unit1;
        }
    }
}
