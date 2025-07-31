using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AirMoveState : SlimeMoveState
{
    public override void EnterState(SlimeController slime)
    {
        Debug.Log("Entered Air State!");
        slime.rb.gravityScale = 1f;

        slime.rb.sharedMaterial.bounciness = 1;
        slime.col.enabled = false;
        slime.col.enabled = true;
        //slime.sr.sprite = slime.slime_inAir;
    }

    public override void UpdateState(SlimeController slime)
    {
        moveX = 0;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            IsControlled = false;
            slime.EnterMoveState(MoveState.Bounce);
            return;
        }
        if (Input.GetKey(KeyCode.D))
        {  
            slime.ChangeDirection(Direction.InAirRight);
            moveX++;
            IsControlled = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            slime.ChangeDirection(Direction.InAirLeft);
            moveX--;
            IsControlled = true;
        }
        
        if (moveX == 0)
        {
            slime.ChangeDirection(Direction.InAir);
            IsControlled = false;
        }
    }

    public override void FixedUpdateState(SlimeController slime, Vector2 move_vector)
    {
        if (IsControlled)
        {
            move_vector.x = Mathf.Clamp(
               move_vector.x + slime.CurrentSlimeState.Acceleration * moveX,
               -slime.CurrentSlimeState.MaxSpeed,
               slime.CurrentSlimeState.MaxSpeed);
        }
        slime.Move = move_vector;
    }

    public override void ReactToGround(Side side, SlimeController slime)
    {
        //switch (side)
        //{
        //    case Side.Bottom://sticks to the ground
        //        slime.EnterMoveState(MoveState.Ground);
        //        break; 
        //    case Side.Top://sticks to the ceiling
        //        slime.EnterMoveState(MoveState.Ceiling);
        //        break;
        //    case Side.Left://sticks to a left wall
        //        slime.EnterMoveState(MoveState.Wall);
        //        break;
        //    case Side.Right://sticks to a right wall
        //        slime.EnterMoveState(MoveState.Wall);
        //        break;
        //}
    }
}
