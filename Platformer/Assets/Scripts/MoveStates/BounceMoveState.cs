using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceMoveState : SlimeMoveState
{
    public override void EnterState(SlimeController slime)
    {
        Debug.Log("Entered Bounce State!");
        slime.rb.sharedMaterial.bounciness = 0;
        slime.col.enabled = false;
        slime.col.enabled = true;
        slime.rb.gravityScale = 1f;
        slime.ChangeDirection(Direction.InAirRight);
    }

    public override void UpdateState(SlimeController slime)
    {

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            //slime.rb.sharedMaterial.bounciness = 0;
            //slime.col.enabled = false;
            //slime.col.enabled = true;
            slime.EnterMoveState(MoveState.Air);
            return;
        }
    }

    public override void FixedUpdateState(SlimeController slime, Vector2 move_vector)
    {
        
    }

    public override void ReactToGround(Side side, SlimeController slime)
    {
        switch (side)
        {
            case Side.Bottom://sticks to the ground
                slime.EnterMoveState(MoveState.Ground);
                break;
            case Side.Top://sticks to the ceiling
                slime.EnterMoveState(MoveState.Ceiling);
                break;
            case Side.Left://sticks to a left wall
                slime.EnterMoveState(MoveState.Wall);
                break;
            case Side.Right://sticks to a right wall
                slime.EnterMoveState(MoveState.Wall);
                break;
        }
    }
}
