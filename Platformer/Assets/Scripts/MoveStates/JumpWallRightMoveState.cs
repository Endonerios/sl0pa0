using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpWallRightMoveState : JumpMoveState
{
    public override void EnterState(SlimeController slime)
    {
        Debug.Log("Entered Jump Wall Right State!");
        slime.rb.gravityScale = 1f;
        slime.rb.AddForce(Vector2.right * slime.CurrentSlimeState.JumpPower);
    }

    public override void ReactToGround(Side touching_side, SlimeController slime)
    {
        switch (touching_side)
        {
            case Side.Bottom://sticks to the ceiling
                slime.EnterMoveState(MoveState.Ground);
                break;
            case Side.Right://sticks to right wall
                slime.EnterMoveState(MoveState.Wall);
                break;
        }
    }
}
