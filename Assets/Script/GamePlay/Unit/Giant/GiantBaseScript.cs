using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class GiantBaseScript : UnitGridCombat
{
    public RobotBaseScript target;
    public bool targetFound;
    public int waitingTime;
    public State state = State.Waiting;
    public enum State
    {
        Waiting,
        Aggro
    }
    public void AddToList()
    {
        //Debug.Log(this.name);
        gridCombatSystem.unitGridCombatList.Add(this);        
    }
    public abstract IEnumerator ExecuteAI(Action onFinish);
    
}
