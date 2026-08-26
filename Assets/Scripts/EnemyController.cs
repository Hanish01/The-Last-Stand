using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator EnemyAnim;
    public float maxHealth = 100;
    public float currHealth;
    public bool IsAlive = true;

    //plaayer follow
    private GameObject player;
    public float speed = 7;
    private Rigidbody2D EnemyRb;

    public float attackRange = 1f;
    public bool isAttacking = false;
    public bool isJumping = false;
    public bool canJump = true;

    public float jumpForce = 5f;
    public float jumpCoolDownTime = 1.5f;
    public bool UseJumpAnimation = true;

    //attacking/damaging

    public GameObject attackPoint;
    public float radius = 1.0f;
    public LayerMask Player;
    public float damage = 20;
    public bool canAttack = true;

    //stun
    public float stunTime = 0.4f;
    public bool isStunned = false;


    //slider for health
    public Slider slider;

    //burning
    public GameObject burnEffect;
    public bool isBurning = false;
    public SpriteRenderer SR;
    public GameObject burnpoint;
    public AudioManager AM;


    void Start()
    {
        player = GameObject.Find("Player");
        EnemyRb = GetComponent<Rigidbody2D>();
        EnemyAnim = GetComponent<Animator>();
        currHealth = maxHealth;
        slider.maxValue = maxHealth;
        SR = GetComponent<SpriteRenderer>();
        AM = AudioManager.Instance;
    }




    void DealDamage()
    {
        
        Collider2D[] collider = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, Player);
        foreach(Collider2D hit in collider)
        {
            PlayerHealth ph = hit.gameObject.GetComponent<PlayerHealth>();
            if (ph!= null && ph.isAlive)
            {
                Debug.Log("Player hittttt");
                ph.TakeDamage(damage, hit.transform.position);
            }

        }

    }


   public void ApplyBurn(float damage, float time)
    {
        if(!isBurning && IsAlive)
        {
            StartCoroutine(BurnDamageDealer(damage, time));
        }
        


    }


    IEnumerator BurnDamageDealer(float damage, float time)
    {
        isBurning = true;
        float timer = 0f;

        while(timer<time)
        {
            BurnDamageDealt(damage);
            SR.color = new Color(1f, 0.5f, 0f);

            yield return new WaitForSeconds(0.15f);
            AM.PlaySFX(AM.EnemyOnfire);

            SR.color = Color.white;

            yield return new WaitForSeconds(0.85f);
            
            timer++;
        }

        isBurning = false;
    }
    // Update is called once per frame
    void Update()
    {
        burnEffect.SetActive(true);
        if (!IsAlive || isStunned)
        {
            return ;
        }
        float Distance = Mathf.Abs(player.transform.position.x - transform.position.x);
        
        if( !isAttacking && Distance > attackRange)
        {
            float dir = player.transform.position.x > transform.position.x ? 1 : -1;
            EnemyRb.linearVelocity = new Vector2(dir * speed, EnemyRb.linearVelocity.y);
            
        }
        else if(!isJumping)
        {
            EnemyRb.linearVelocity = new Vector2(0, EnemyRb.linearVelocity.y);
            
        }
        if(Distance<= attackRange &&!isAttacking && !isJumping && canAttack)
        {
            StartCoroutine(Attack());
            
        }
        if(canJump && !isJumping && player.transform.position.y > transform.position.y +2f)
        {
            StartCoroutine("JumpCoolDown");
            float dir = player.transform.position.x > transform.position.x ? 1 : -1;
            StartCoroutine("Waitasec");
            EnemyRb.linearVelocity= new Vector2( dir * speed , jumpForce);

            EnemyAnim.SetBool("Jump", true);
            isJumping = true;

        }
        if(!isAttacking)
        {
            Vector3 temp = transform.localScale;
            if (player.transform.position.x > transform.position.x)
            {
                temp.x = Mathf.Abs(temp.x);
            }
            else
            {
                temp.x = -Mathf.Abs(temp.x);
            }
            transform.localScale = temp;
        }
        
            
        


       

        HandleAiranimation();
        HandleBurnanimation();
        EnemyAnim.SetFloat("Speed", Mathf.Abs( EnemyRb.linearVelocity.x ));

    }

    void HandleBurnanimation()
    {
        if(isBurning)
        {
            StartCoroutine(Waitasec());
            burnEffect.SetActive(true);
            burnEffect.transform.position = burnpoint.transform.position;

            Debug.Log("Enemy Burning");
        }
        else
        {
            burnEffect.SetActive(false);
            SR.color = Color.white;
        }
    }

    IEnumerator Waitasec()
    {
        yield return new WaitForSeconds(1);
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        canAttack = false;

        float dir = player.transform.position.x > transform.position.x ? 1 : -1;
        Vector3 temp = transform.localScale;
        temp.x = dir > 0 ? Mathf.Abs(temp.x) : -Mathf.Abs(temp.x);
        transform.localScale = temp;
        yield return null;

        EnemyAnim.SetTrigger("Attacking");
        StartCoroutine("AttackCooldown");
        yield return new WaitForSeconds(2);
        isAttacking = false;
    }

    IEnumerator AttackCooldown()
    {
        
        yield return new WaitForSeconds(1);
        canAttack = true;
    }
    IEnumerator JumpCoolDown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCoolDownTime);
        canJump = true;
    }
    public bool DamageDealt(float damage)
    {
        if(!IsAlive)
        {
            return false;
        }
        currHealth = currHealth - damage;
        AudioClip[] HurtAud = { AM.EnemyHurt1, AM.EnemyHurt2, AM.EnemyHurt3 };
        int x = Random.Range(0, 3);
        AudioClip hurt = HurtAud[x];
        AM.PlayEnemyHurt(hurt);
        slider.value = currHealth;
        Hit();
        if (currHealth <= 0)
        {

            Death();
            return true;
        }
        
        return false;

    }

    public bool BurnDamageDealt(float damage)
    {
        if (!IsAlive)
        {
            return false;
        }
        currHealth = currHealth - damage;
        slider.value = currHealth;
        if (currHealth <= 0)
        {

            Death();
            return true;
        }

        return false;

    }
    public void Hit()
    {
        EnemyAnim.SetTrigger("Hit");
        StartCoroutine("Stun");
        
    }

    IEnumerator Stun()
    {
        isStunned = true;
        yield return new WaitForSeconds(stunTime);
        isStunned = false;

    }
    public void Death()
    {
        if(!IsAlive)
        {
            return;
        }
        IsAlive = false;
       
        AM.PlayDeathSFX();

        EnemyAnim.SetTrigger("Death");
        EnemyRb.linearVelocity = Vector2.zero;
        EnemyRb.simulated = false;// stops all physocs
        Destroy(gameObject, 3f);

    }

    private void HandleAiranimation()
    {
        if(!UseJumpAnimation)
        {
            return;
        }
        if (EnemyRb.linearVelocity.y > 0.1)
        {
            EnemyAnim.SetBool("Jump", true);
            EnemyAnim.SetBool("Fall", false);
        }
        else if (EnemyRb.linearVelocity.y < -0.1)
        {
            EnemyAnim.SetBool("Fall", true);
            EnemyAnim.SetBool("Jump", false);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        
            isJumping = false;
        if (UseJumpAnimation)
        {
            EnemyAnim.SetBool("Jump", false);
            EnemyAnim.SetBool("Fall", false);
        }
        
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.gameObject.transform.position, radius);
    }

    
}
