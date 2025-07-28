using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class SlimeMoveState
{
    public bool IsControlled;
    public sbyte moveX;
    
    public abstract void EnterState(SlimeController slime);

    public abstract void UpdateState(SlimeController slime);

    public abstract void FixedUpdateState(SlimeController slime, Vector2 move_vector);

    public abstract void ReactToGround(Side side, SlimeController slime);
}
