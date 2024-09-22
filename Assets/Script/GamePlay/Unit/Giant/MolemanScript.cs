using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TileMap;

public class MolemanScript : GiantBaseScript
{
    public Vector2Int attackLocation;
    public ShootDirection targetDirection;
    public enum ShootDirection
    {
        Left, Right, Up, Down
    }
    private void Awake()
    {
        GetAllComponent();
        AddToList();
    }
    private void Start()
    {        
        AssignUnit();
        makingsure();
        UpdateDisplay();
    }

    public override void AssignUnit()
    {
        animator.SetBool("Idle", true);
        team = Team.Enemy;
        unitType = Unit.Moleman;
    }
    public override void UnitSelected(Action isSelected)
    {
        showMolemanVisual();
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
                waitingTime = 0;
                attackLocation = SearchTargetLocation(target);
                state = GiantBaseScript.State.Aggro;
            }
            else
            {
                waitingTime++;
                afkDisplay.UpdateNumber(waitingTime);
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
                yield return StartCoroutine(AttackTarget(attackLocation));
                target = null;
                state = GiantBaseScript.State.Waiting;
            }
            //kalo engga, balik aggro
            else
            {
                attackLocation = SearchTargetLocation(target);
                state = GiantBaseScript.State.Aggro;
            }            
        }
        onFinish?.Invoke();
    }

    public Vector2Int SearchTargetLocation(RobotBaseScript robotBaseScript)
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(base.GetPosition());
        Vector2Int targetPosition = grid.GetXY(robotBaseScript.GetPosition());

        Vector2Int attackLocation = new Vector2Int(targetPosition.x-unitPosition.x,targetPosition.y-unitPosition.y);
        return attackLocation;
    }

    public RobotBaseScript SearchForTarget()
    {
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(GetPosition());

        int unitX = unitPosition.x;
        int unitY = unitPosition.y;

        RobotBaseScript robotBaseScript = target;

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TileMap.TilemapObject tilemapObject = grid.GetGridObject(x, y);
                //kalo in range
                if (Mathf.Abs(unitX - x) < 3 && Mathf.Abs(unitY - y) < 3)
                {
                    //kena block
                    if (tilemapObject.isBlocking == true)
                    {
                        UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
                        //yang ngeblock unit
                        if (unitGridCombat)
                        {
                            //kalo unitnya musuhnya moleman (kita)
                            if(unitGridCombat.team == UnitGridCombat.Team.Ally)
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
                    }
                }
            }
        }

        return robotBaseScript;
    }

    public IEnumerator AttackTarget(Vector2Int attackLocation)
    {
        animator.SetTrigger("DigIn");
        animator.SetBool("Idle", false);
        AudioManager audioManager = AudioManager.Instance;
        audioManager.PlaySFX(audioManager.moleman);
        yield return new WaitForSeconds(1);
        //ngambil data yang diperlukan
        Grid<TileMap.TilemapObject> grid = tilemapTesting.GetGrid();
        Vector2Int unitPosition = grid.GetXY(GetPosition());
        Vector2Int targetPosition = unitPosition + attackLocation;
        TileMap.TilemapObject tilemapObject = grid.GetGridObject(targetPosition.x, targetPosition.y);
        
        //kalo masih dalem map
        if(tilemapObject != null)
        {
            UnitGridCombat unitGridCombat = tilemapObject.GetUnitGridCombat();
            //animasi


            //ngedamage tilenya
            if (unitGridCombat)
            {
                unitGridCombat.TakeDamage(1);
            }

            //cek bisa pindah posisi ke target apa engga
            if (tilemapObject.isBlocking == false)
            {              
                TeleportTo(targetPosition, () => { });                
            }
            animator.SetTrigger("DigOut");
        }
        //kalo diluar map
        else
        {

        }
        animator.SetBool("Idle", true);

    }

    public void showMolemanVisual()
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
                    else if (Mathf.Abs(unitX - x) < 3 && Mathf.Abs(unitY - y) < 3)
                    {
                        //kena block
                        if (tilemapObject.isBlocking == true)
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
        }
        
        //kalo udah ada target
        if (state == GiantBaseScript.State.Aggro)
        {
            //musuh
            TileMap.TilemapObject tilemapObject = grid.GetGridObject(unitX + attackLocation.x, unitY + attackLocation.y);
            tilemapObject.SetAttackDisplay(TileMap.TilemapObject.AttackDisplay.EnemyAttack);
            //dirisendiri
            tilemapObject = grid.GetGridObject(unitX, unitY);
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
        else if (unit1.JustDeployed == true)
        {
            return unit1;
        }
        else if (unit2.JustDeployed == true)
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
