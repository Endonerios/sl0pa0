using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public enum MoveState
{
    None,
    Ground,
    Ceiling,
    Jump,
    JumpDown,
    JumpWallRight,
    JumpWallLeft,
    Air,
    Wall,
    Bounce,
} 

public enum Side
{
    None,
    Top,
    Bottom,
    Left, 
    Right,
}

public enum Direction
{
    None,

    Ground,
    GroundLeft,
    GroundRight,

    JumpUp,
    JumpDown,
    JumpWall,

    InAir,
    InAirLeft,
    InAirRight,

    WallLeft,
    WallLeftUp,
    WallLeftDown,
    WallRight,
    WallRightUp,
    WallRightDown,

    Ceiling,
    CeilingLeft,
    CeilingRight,
}

public enum Reaction
{
    None,
    Death,
    Push,
    GainSalt,
    LoseSalt,
    GetDry,
    GetNormal,
    GetWet,
}

public enum WetnessState
{
    None,
    Dry,
    DrySalty,
    Normal,
    NormalSalty,
    Wet,
}

public enum Element
{
    None,
    SaltPiece,
    Salt,
    Fire,
    Water,
    Electricity,
    Frost,
}

[RequireComponent(typeof(Rigidbody2D))]
public class SlimeController : MonoBehaviour
{
    [SerializeField] LayerMask GroundLayer;
    public Vector2 Move = Vector2.zero;
    public Collider2D col;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;

    [Header("Dry state")]
    [SerializeField] float dryMaxSpeed;
    [SerializeField] float dryAcceleration;
    [SerializeField] float dryFrictionGround;
    [SerializeField] float dryFrictionCeiling;
    [SerializeField] float dryFrictionWall;
    [SerializeField] float dryFrictionWallUp;
    [SerializeField] float dryJumpPower;
    [SerializeField] Reaction[] drySaltPiece;
    [SerializeField] Reaction[] drySalt;
    [SerializeField] Reaction[] dryFire;
    [SerializeField] Reaction[] dryWater;
    [SerializeField] Reaction[] dryElectricity;
    [SerializeField] Reaction[] dryFrost;
    [Header("Salty Dry state")]
    [SerializeField] Reaction[] drySaltySaltPiece;
    [SerializeField] Reaction[] drySaltySalt;
    [SerializeField] Reaction[] drySaltyFire;
    [SerializeField] Reaction[] drySaltyWater;
    [SerializeField] Reaction[] drySaltyElectricity;
    [SerializeField] Reaction[] drySaltyFrost;
    [Header("Normal state")]
    [SerializeField] float normalMaxSpeed;
    [SerializeField] float normalAcceleration;
    [SerializeField] float normalFrictionGround;
    [SerializeField] float normalFrictionCeiling;
    [SerializeField] float normalFrictionWall;
    [SerializeField] float normalFrictionWallUp;
    [SerializeField] float normalJumpPower;
    [SerializeField] Reaction[] normalSaltPiece;
    [SerializeField] Reaction[] normalSalt;
    [SerializeField] Reaction[] normalFire;
    [SerializeField] Reaction[] normalWater;
    [SerializeField] Reaction[] normalElectricity;
    [SerializeField] Reaction[] normalFrost;
    [Header("Salty Normal state")]
    [SerializeField] Reaction[] normalSaltySaltPiece;
    [SerializeField] Reaction[] normalSaltySalt;
    [SerializeField] Reaction[] normalSaltyFire;
    [SerializeField] Reaction[] normalSaltyWater;
    [SerializeField] Reaction[] normalSaltyElectricity;
    [SerializeField] Reaction[] normalSaltyFrost;
    [Header("Wet state")]
    [SerializeField] float wetMaxSpeed;
    [SerializeField] float wetAcceleration;
    [SerializeField] float wetFrictionGround;
    [SerializeField] float wetFrictionCeiling;
    [SerializeField] float wetFrictionWall;
    [SerializeField] float wetFrictionWallUp;
    [SerializeField] float wetJumpPower;
    [SerializeField] Reaction[] wetSaltPiece;
    [SerializeField] Reaction[] wetSalt;
    [SerializeField] Reaction[] wetFire;
    [SerializeField] Reaction[] wetWater;
    [SerializeField] Reaction[] wetElectricity;
    [SerializeField] Reaction[] wetFrost;
    [Header("Sprites")]
    public Sprite slime_inAir;
    public Sprite slime_ground;
    public Sprite slime_ground_right;
    public Sprite slime_ceiling;
    public Sprite slime_ceiling_right;
    public Sprite slime_wall;
    public Sprite slime_wall_slide;
    public Sprite slime_jump;

    SlimeMoveState CurrentMoveState;
    public Side CurrentSide = Side.None;
    Direction CurrentDirection = Direction.None;
    float CurrentScale;

    public SlimeMoveState[] MoveStates =
    {
        new GroundMoveState(),
        new CeilingMoveState(),
        new JumpMoveState(),
        new JumpDownMoveState(),
        new JumpWallRightMoveState(),
        new JumpWallLeftMoveState(),
        new AirMoveState(),
        new WallMoveState(),
        new BounceMoveState(),
    };

    WetnessState CurrentWetnessState;

    public SlimeState CurrentSlimeState;

    public SlimeState[] SlimeStates;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        EnterMoveState(MoveState.Ground);
        CreateStates();
        ChangeSlimeState(WetnessState.Normal);
    }

    public void SwitchSide(Side new_side)
    {
        
    }

    public void CheckSides()
    {
        float slime_pos_x = gameObject.transform.position.x;
        float slime_pos_y = gameObject.transform.position.y;
        //Debug.Log($"Slime pos: ({slime_pos_x}, {slime_pos_y})");
        //Bottom
        RaycastHit2D Bot = Physics2D.BoxCast(new Vector2(slime_pos_x, slime_pos_y - 0.5f * CurrentScale), new Vector2(0.9f, 0.1f) * CurrentScale, angle: 0, Vector2.down, 0, GroundLayer);
        //Top
        RaycastHit2D Top = Physics2D.BoxCast(new Vector2(slime_pos_x, slime_pos_y + 0.5f * CurrentScale), new Vector2(0.9f, 0.1f) * CurrentScale, angle: 0, Vector2.up, 0, GroundLayer);
        //Left
        RaycastHit2D Left = Physics2D.BoxCast(new Vector2(slime_pos_x - 0.5f * CurrentScale, slime_pos_y), new Vector2(0.1f, 0.9f) * CurrentScale, angle: 0, Vector2.left, 0, GroundLayer);
        //Right
        RaycastHit2D Right = Physics2D.BoxCast(new Vector2(slime_pos_x + 0.5f * CurrentScale, slime_pos_y), new Vector2(0.1f, 0.9f) * CurrentScale, angle: 0, Vector2.right, 0, GroundLayer);
        if (Bot.collider)
        {
            //Debug.DrawLine(new Vector2(slime_pos_x, slime_pos_y - 0.5f), Bot.point, Color.red, 2f);
            CurrentSide = Side.Bottom;
            CurrentMoveState.ReactToGround(Side.Bottom, this);
        }
        else if (Top.collider)
        {
            CurrentSide = Side.Top;
            CurrentMoveState.ReactToGround(Side.Top, this);
        }
        else if (Right.collider)
        {
            CurrentSide = Side.Right;
            CurrentMoveState.ReactToGround(Side.Right, this);
        }
        else if (Left.collider)
        {
            CurrentSide = Side.Left;
            CurrentMoveState.ReactToGround(Side.Left, this);
        }
        else
        {
            CurrentSide = Side.None;
            CurrentMoveState.ReactToGround(Side.None, this);
        }
        
    } 

    void Update()
    {
        CurrentMoveState.UpdateState(this);
    }

    void FixedUpdate()
    {
        Move = rb.velocity;
        CurrentMoveState.FixedUpdateState(this, Move);
        rb.velocity = Move;
        CheckSides();
    }

    public void EnterMoveState(MoveState new_move_state)
    {
        SlimeMoveState new_state = MoveStates[(int)new_move_state - 1];
        new_state.EnterState(this);
        CurrentMoveState = new_state;
    }

    public void ChangeDirection(Direction new_direction)
    {
        if (new_direction == CurrentDirection)
        {
            return;
        }
        else
        {
            switch (new_direction)
            {
                case Direction.None:
                    break;
                case Direction.Ground:
                    sr.sprite = slime_ground;
                    sr.flipY = false;
                    break;
                case Direction.GroundRight:
                    sr.sprite = slime_ground_right;
                    sr.flipX = false;
                    sr.flipY = false;
                    break;
                case Direction.GroundLeft:
                    sr.sprite = slime_ground_right;
                    sr.flipX = true;
                    sr.flipY = false;
                    break;
                case Direction.JumpUp:
                    sr.sprite = slime_jump;
                    break;
                case Direction.JumpDown:
                    sr.sprite = slime_jump;
                    sr.flipY = true;
                    break;
                case Direction.InAir:
                    sr.sprite = slime_inAir;
                    break;
                case Direction.InAirRight:
                    sr.sprite = slime_inAir;
                    sr.flipX = false;
                    break;
                case Direction.InAirLeft:
                    sr.sprite = slime_inAir;
                    sr.flipX = true;
                    break;
                case Direction.Ceiling:
                    sr.sprite = slime_ceiling;
                    break;
                case Direction.CeilingRight:
                    sr.sprite = slime_ceiling_right;
                    sr.flipX = false;
                    break;
                case Direction.CeilingLeft:
                    sr.sprite = slime_ceiling_right;
                    sr.flipX = true;
                    break;
                case Direction.WallRight:
                    sr.sprite = slime_wall;
                    sr.flipX = false;
                    sr.flipY = false;
                    break;
                case Direction.WallRightUp:
                    sr.sprite = slime_wall_slide;
                    sr.flipY = true;
                    sr.flipX = false;
                    break;
                case Direction.WallRightDown:
                    sr.sprite = slime_wall_slide;
                    sr.flipX = false;
                    break;
                case Direction.WallLeft:
                    sr.sprite = slime_wall;
                    sr.flipX = true;
                    sr.flipY = false;
                    break;
                case Direction.WallLeftUp:
                    sr.sprite = slime_wall_slide;
                    sr.flipY = true;
                    sr.flipX = true;
                    break;
                case Direction.WallLeftDown:
                    sr.sprite = slime_wall_slide;
                    sr.flipX = true;
                    break;
            }
        }
    }

    public void ResetSlime()
    {
        rb.velocity = Vector2.zero;
        Move = Vector2.zero;
    }

    void CreateStates()
    {
        SlimeStates = new SlimeState[]
        {
            new SlimeState(dryMaxSpeed, dryAcceleration, dryFrictionGround, dryFrictionCeiling, dryFrictionWall, dryFrictionWallUp, 
            dryJumpPower, drySaltPiece, drySalt, dryFire, dryWater, dryElectricity, dryFrost),

            new SlimeState(dryMaxSpeed, dryAcceleration, dryFrictionGround, dryFrictionCeiling, dryFrictionWall, dryFrictionWallUp,
            dryJumpPower, drySaltySaltPiece, drySaltySalt, drySaltyFire, drySaltyWater, drySaltyElectricity, drySaltyFrost),

            new SlimeState(normalMaxSpeed, normalAcceleration, normalFrictionGround, normalFrictionCeiling, normalFrictionWall, 
            normalFrictionWallUp, normalJumpPower, normalSaltPiece, normalSalt, normalFire, normalWater, normalElectricity, normalFrost),

             new SlimeState(normalMaxSpeed, normalAcceleration, normalFrictionGround, normalFrictionCeiling, normalFrictionWall,
            normalFrictionWallUp, normalJumpPower, normalSaltySaltPiece, normalSaltySalt, normalSaltyFire, normalSaltyWater, normalSaltyElectricity, normalSaltyFrost),

            new SlimeState(wetMaxSpeed, wetAcceleration, wetFrictionGround, wetFrictionCeiling, wetFrictionWall, wetFrictionWallUp, 
            wetJumpPower, wetSaltPiece, wetSalt, wetFire, wetWater, wetElectricity, wetFrost),
        };
    }

    void ReactToElement(Element element)
    {
        switch (element)
        {
            case Element.SaltPiece:
                React(ref CurrentSlimeState.SaltPiece);
                break;
            case Element.Salt:
                React(ref CurrentSlimeState.Salt);
                break;
            case Element.Fire:
                React(ref CurrentSlimeState.Fire);
                break;
            case Element.Water:
                React(ref CurrentSlimeState.Water);
                break;
            case Element.Electricity:
                React(ref CurrentSlimeState.Electricity);
                break;
            case Element.Frost:
                React(ref CurrentSlimeState.Frost);
                break;
        }
        void React(ref Reaction[] reactions)
        {
            foreach (Reaction reaction in reactions)
            {
                switch (reaction)
                {
                    case Reaction.Death:
                        Die();
                        break;
                    case Reaction.Push:
                        GetPushed();
                        break;
                    case Reaction.GainSalt:
                        ChangeSalt(true);
                        break;
                    case Reaction.LoseSalt:
                        ChangeSalt(false);
                        break;
                    case Reaction.GetDry:
                        ChangeSlimeState(WetnessState.Dry);
                        break;
                    case Reaction.GetNormal:
                        ChangeSlimeState(WetnessState.Normal);
                        break;
                    case Reaction.GetWet:
                        ChangeSlimeState(WetnessState.Wet);
                        break;
                }
            }
        }
    }

    void GetPushed()
    {
        Debug.Log($"{gameObject.name} got pushed!");
    }

    void ChangeSalt(bool new_salt)
    {
        string txt = $"Current Wetness state: {CurrentWetnessState}, ";
        if (new_salt == false)
        {
            if (CurrentWetnessState == WetnessState.NormalSalty)
            {
                ChangeSlimeState(WetnessState.Normal);
            }
            else if (CurrentWetnessState == WetnessState.DrySalty)
            {
                ChangeSlimeState(WetnessState.Dry);
            }
            txt += $"{gameObject.name} is NOT salty";
        }
        else 
        {
            if (CurrentWetnessState == WetnessState.Normal)
            {
                ChangeSlimeState(WetnessState.NormalSalty);
            }
            else if (CurrentWetnessState == WetnessState.Dry) 
            {
                ChangeSlimeState(WetnessState.DrySalty);
            }
            txt += $"{gameObject.name} is salty";
        }
        Debug.Log(txt);
    }

    void ChangeSlimeState(WetnessState newSlimeState)
    {
        Debug.Log($"{gameObject.name} changed slime (wetness) state!");
        CurrentSlimeState = SlimeStates[(int)newSlimeState - 1];
        CurrentWetnessState = newSlimeState;
        switch (newSlimeState)
        {
            case WetnessState.DrySalty:
            case WetnessState.Dry:
                gameObject.transform.localScale = new Vector2(0.7f, 0.7f);
                break;
            case WetnessState.NormalSalty:
            case WetnessState.Normal:
                gameObject.transform.localScale = new Vector2(1f, 1f);
                break;
            case WetnessState.Wet:
                gameObject.transform.localScale = new Vector2(1.2f, 1.2f);
                break;
        }
        CurrentScale = gameObject.transform.localScale.x;
    }

    void Die()
    {
        GameManager.instance.Respawn();
    }

    void IdentifyElement(Collider2D col)
    {
        if (col.tag == "SaltPiece")
        {
            ReactToElement(Element.SaltPiece);
        }
        else if (col.tag == "Salt")
        {
            ReactToElement(Element.Salt);
        }
        else if (col.tag == "Fire")
        {
            ReactToElement(Element.Fire);
        }
        else if (col.tag == "Water")
        {
            ReactToElement(Element.Water);
        }
        else if (col.tag == "Electricity")
        {
            ReactToElement(Element.Electricity);
        }
        else if (col.tag == "Frost")
        {
            ReactToElement(Element.Frost);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IdentifyElement(collision);
    }
}
