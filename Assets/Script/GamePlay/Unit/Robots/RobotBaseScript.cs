using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RobotBaseScript : UnitGridCombat
{
    public int deployNumber;
    public bool JustDeployed;
    public int attackPhase;
    public abstract IEnumerator UseUtility(Vector2 tilePosition, Action onUse);
}
