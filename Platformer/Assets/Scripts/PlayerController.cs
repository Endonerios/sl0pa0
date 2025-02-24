using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Collider2D col2D;
    [SerializeField] bool IsJumping, IsOnGround, IsBouncing, ShouldBounce;
    [SerializeField] float speed, jump_force;
    [SerializeField] int hp;
    [Range(0,10f)][SerializeField] float bounce_factor, non_bounce_factor;
    [Range(0, 10f)][SerializeField] float friction_factor;
    [SerializeField] Vector2 current_velocity;
    [SerializeField] Transform legs_hitbox;
    [SerializeField] LayerMask ground_layer;
    [SerializeField] float max_coyote_time;
    float coyote_timer;
    bool coyote_time_active;

    public int JumpCount;
    public float TimeCount;
    public int CoinCount;
    public int BounceCount;
    public int RespawnCount;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) && coyote_timer > 0))
        {
            IsJumping = true;
        }
        ShouldBounce = Input.GetKey(KeyCode.LeftShift);
    }

    void FixedUpdate()
    {
        CheckIsOnGround();
        current_velocity = rb.velocity;
        current_velocity.x = Input.GetAxis("Horizontal") * speed;
        if (IsJumping)
        {
            Jump();
        }
        ChangeBounce();
        if (!IsBouncing)
        {
            rb.velocity = current_velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            GameManager.instance.OnCoinPickUp(collision.gameObject.GetComponentInParent<Coin>());
            CoinCount++;
        }
        if (IsBouncing && collision.tag == "Ground")
        {
            BounceCount++;
        }
    }

    void ChangeBounce()
    {
        if (ShouldBounce != IsBouncing)
        {
            float new_bounciness;
            float new_friction;
            if (ShouldBounce)
            {
                new_bounciness = bounce_factor;
                new_friction = 0;
            }
            else
            {
                new_bounciness = non_bounce_factor;
                new_friction = friction_factor;
            }
            col2D.enabled = false;
            rb.sharedMaterial.bounciness = new_bounciness;
            rb.sharedMaterial.friction = new_friction;
            col2D.enabled = true;
            IsBouncing = ShouldBounce;
        }
    }

    void CheckIsOnGround()
    {
        coyote_timer -= Time.fixedTime;
        if (Physics2D.OverlapBox(legs_hitbox.position, legs_hitbox.lossyScale, 0, ground_layer))
        {
            IsOnGround = true;
            coyote_timer = max_coyote_time;
        }
        else
        {
            IsOnGround = false;
        }
    }

    void Jump()
    {
        current_velocity.y = jump_force;
        //Debug.Log("Jumped!");
        IsJumping = false;
        JumpCount++;
    }

    public void RecieveDmg(int dmg = 1)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            Die();
        }
    }

    public void ZeroVelocity()
    {
        rb.velocity = new Vector2(0, 0);
    }

    void Die()
    {
        GameManager.instance.Respawn();
        RespawnCount++;
    } 
}
