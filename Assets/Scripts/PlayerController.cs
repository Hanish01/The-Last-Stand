using System.Collections;
using System.Runtime.Serialization.Formatters;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 2.0f;
    private Rigidbody2D playerRb;
    public float gravityModifier;

    private bool isJumping = false;
    public float jumpForce = 4;
    private bool CanjumpAttack = true;

    private int comboStep = 0;
    private bool comboWindow = false;
    private bool isJumpAttacking = false;

    private bool isSpecialAttacking = false;
    public GameObject slashEffect;
    public GameObject slashStartingPosition;

    public bool isDashing = false;
    //public bool canDash = true;
    public int MaxDash =1;
    public int remainingDash;

    public float dashSpeed = 10;
    public float dashDuration = 0.2f;
    public float dashCoolDown = 1;
    public TrailRenderer[] tr;

    public Transform AttackPoint;
    public float attackrange = 0.5f;
    public LayerMask EnemiesList;

    public float Damage = 10f;

    private PlayerHealth ph;

    public Transform groundCheck;
    public float GroundCheckdist = 0.3f;
    public LayerMask Ground;

    public bool isGrounded;
    
    private Animator playerAnimator;
    


    //invisibility
    public bool CangoInvisibilty = false;
    public bool isInvincible = false;
    public bool canuseInvisibility = false;
    private SpriteRenderer SR;
    public int invincibilityTime = 3;
    public GameObject Inv_Icon;
    public int invicibilitycooldown = 5;

    //Crit 
    public int Criticalchance = 0;
    public float CriticalMultiplier = 2f;

    //Specia; attack

    public float SpecialAttackCooldown = 3f;
    public bool canUseSpecial = true;


    //rageMode
    public bool RagemodeBuff = false;
    public float RageModeDuration = 5f;
    public float RageModecooldown = 20f;
    public bool IsRageActive = false;
    public bool CanuseRage = true;
    public GameObject aura;
    public GameObject rageMeter;

    //sfx
    private AudioManager AM;
    public bool inputdis;
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerRb.gravityScale = gravityModifier;
        ph = GetComponent<PlayerHealth>();
        playerAnimator = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();
        remainingDash = MaxDash;
        AM = AudioManager.Instance;
    }

    void Update()
    {

        if(inputdis)
        {
            return;
        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, GroundCheckdist, Ground);

        float horizontalInput = Input.GetAxis("Horizontal");

        if (!IsAttacking() && !isDashing && !isSpecialAttacking)
        {
            if (!ph.isPushed)
            {
                float xValue = movementSpeed * horizontalInput;
                playerRb.linearVelocity = new Vector2(movementSpeed * horizontalInput, playerRb.linearVelocity.y);
                
            }
        }

        if(Mathf.Abs(playerRb.linearVelocity.x) > 0.1f && isGrounded)
        {
            AM.PlayLoop(AM.moveloop);
        }
        else
        {
            AM.StopLoop();
        }

            playerAnimator.SetFloat("speed", Mathf.Abs(playerRb.linearVelocity.x));

        if (!isDashing && !isSpecialAttacking)
        {
            Vector3 scale = transform.localScale;

            if (horizontalInput > 0) scale.x = Mathf.Abs(scale.x);
            else if (horizontalInput < 0) scale.x = -Mathf.Abs(scale.x);

            transform.localScale = scale;

            if (Input.GetButtonDown("Jump") && isGrounded)
                Jump();

            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!isGrounded && CanjumpAttack)
                    JumpAttack();
                else
                    HandleAttack();
            }

            if (Input.GetKeyDown(KeyCode.V) && canUseSpecial)
                StartCoroutine("specialattackcaller");

            if(Input.GetKeyDown(KeyCode.I) && CangoInvisibilty && canuseInvisibility && !isInvincible && !IsRageActive)
            {
                StartCoroutine(Invincible());
            }
        }

        if(IsRageActive)
        {
            rageMeter.SetActive(false);
        }
        else
        {
            rageMeter.SetActive(RagemodeBuff && CanuseRage);
        }


        if(isInvincible)
        {
            Inv_Icon.SetActive(false);
        }
        else if(canuseInvisibility)
        {
            Inv_Icon.SetActive(!isInvincible );
        }

        if (Input.GetKeyDown(KeyCode.Z) && remainingDash > 0 && !isDashing)
            StartCoroutine(Dash());

        if(Input.GetKeyDown(KeyCode.R)&&RagemodeBuff && !IsRageActive && CanuseRage && !isInvincible)
        {
            StartCoroutine("RageModeActivator");
        }

        HandleAiranimation();
        HandleInvincible();


        if (isGrounded)
        {
            isJumping = false;
            CanjumpAttack = true;
            isJumpAttacking = false;
        }

//        Debug.Log(
//    "RageBuff: " + RagemodeBuff +
//    " IsRageActive: " + IsRageActive +
//    " CanuseRage: " + CanuseRage +
//    " isInvincible: " + isInvincible
//);
    }
    IEnumerator RageFlash()
    {
        SR.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        SR.color = new Color(0.6679245f, 0.3264008f, 0.341f);
        yield return new WaitForSeconds(0.1f);
    }


    IEnumerator CamshakeRage()
    {
        Transform cam = Camera.main.transform;
        Vector3 orgpos = cam.position;
        for(int i=0;i<20;i++)
        {
            float x = Random.Range(-0.3f, 0.2f);
            float y = Random.Range(-0.4f, 0.2f);

            cam.position = new Vector3(orgpos.x + x, orgpos.y +  y, orgpos.z);
            yield return null;

        }

        cam.position = orgpos;
    }

    IEnumerator RageModeActivator()
    {
        AM.PlaySFX(AM.Rage);

        StartCoroutine("RageFlash");

        StartCoroutine("CamshakeRage");
        IsRageActive = true;
        
        CanuseRage = false;
        float normspeed = movementSpeed;
        int normcrit = Criticalchance;
        movementSpeed *= 1.4f;
        Criticalchance = 100;
        aura.SetActive(true);
        SR.color = new Color(0.6679245f, 0.3264008f, 0.341f);
        
        yield return new WaitForSeconds(RageModeDuration);
        IsRageActive = false;
        movementSpeed = normspeed;
        SR.color = Color.white;
        Criticalchance = normcrit;
        
        aura.SetActive(false);
        yield return new WaitForSeconds(RageModecooldown);
        AM.PlaySFX(AM.BuffoffCooldown);
        CanuseRage = true;

    }




    bool IsAttacking()
    {
        return comboWindow || comboStep != 0 || isJumpAttacking;
    }
    
    void HandleInvincible()
    {
        
        
        if (isInvincible)
        {
            SR.color = new Color(0.3f, 1f, 1f, 0.3f);
        } 
        else if(!IsRageActive)
        {
            SR.color = Color.white;
        }
        

    }

    void JumpAttack()
    {
        playerAnimator.SetTrigger("JumpAttack");
        CanjumpAttack = false;
        isJumpAttacking = true;
    }

   
        
    
    IEnumerator Invincible()
    {
        AM.PlaySFX(AM.invicible);
        isInvincible = true;
        canuseInvisibility = false;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
        yield return new WaitForSeconds(invicibilitycooldown);
        AM.PlaySFX(AM.BuffoffCooldown);
        canuseInvisibility = true;
    }
    void HandleAttack()
    {
        if (!comboWindow)
        {
            comboStep = 1;
            Attack(comboStep);
            StartCoroutine(ComboWindow());
        }
        else
        {
            comboStep = 2;
            Attack(comboStep);
            comboWindow = false;
        }
    }

    void Attack(int index)
    {
       
        playerAnimator.SetTrigger("Attack");
        playerAnimator.SetInteger("ComboIndex", index);
        playerRb.linearVelocity = new Vector2(0, playerRb.linearVelocity.y);
       
       
    }
    
   

    IEnumerator ComboWindow()
    {
        comboWindow = true;
        yield return new WaitForSeconds(0.35f);
        comboWindow = false;
        comboStep = 0;
    }

    void Jump()
    {
        isJumping = true;
        
        AM.PlaySFX(AM.jump);
        playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void HandleAiranimation()
    {
        if (!isGrounded)
        {
            if (playerRb.linearVelocity.y > 0.1f)
            {
                playerAnimator.SetBool("Jump", true);
                playerAnimator.SetBool("Fall", false);
            }
            else if (playerRb.linearVelocity.y < -0.1f)
            {
                playerAnimator.SetBool("Fall", true);
                playerAnimator.SetBool("Jump", false);
            }
        }
        else
        {
            playerAnimator.SetBool("Fall", false);
            playerAnimator.SetBool("Jump", false);
        }
    }

    IEnumerator specialattackcaller()
    {
        AM.PlaySFX(AM.Specialattack);
        canUseSpecial = false;
        SpecialAttack();
        
        yield return new  WaitForSeconds(SpecialAttackCooldown);
        canUseSpecial = true;

    }

    void SpecialAttack()
    {
        
        isSpecialAttacking = true;
        playerAnimator.SetTrigger("SpecialSkill");

        GameObject slash = Instantiate(slashEffect, slashStartingPosition.transform.position, Quaternion.identity);

        Vector3 scale = slash.transform.localScale;
        scale.x = transform.localScale.x > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        slash.transform.localScale = scale;

        Invoke(nameof(ResetSpecialAttack), 0.8f);
    }

    void ResetSpecialAttack()
    {
        isSpecialAttacking = false;
    }

    IEnumerator Dash()
    {
        remainingDash--;
        isDashing = true;
        AM.PlaySFX(AM.dash);
        foreach (TrailRenderer t in tr)
            t.emitting = true;

        int playerMask = LayerMask.NameToLayer("Player");
        int enemyMask = LayerMask.NameToLayer("Enemy");

        Physics2D.IgnoreLayerCollision(playerMask, enemyMask, true);

        float originalGravity = playerRb.gravityScale;
        playerRb.gravityScale = 0;

        float dir = transform.localScale.x > 0 ? 1 : -1;
        playerRb.linearVelocity = new Vector2(dashSpeed * dir, 0);

        yield return new WaitForSeconds(dashDuration);

        playerRb.gravityScale = originalGravity;
        isDashing = false;

        Physics2D.IgnoreLayerCollision(playerMask, enemyMask, false);

        foreach (TrailRenderer t in tr)
            t.emitting = false;

        yield return new WaitForSeconds(dashCoolDown);
        remainingDash = MaxDash; 
    }
    
    IEnumerator ImpactPause()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.07f);
        Time.timeScale = 1f;
    }

    IEnumerator CameraShake()
    {
        Transform cam = Camera.main.transform;
        Vector3 originalPos = cam.position;
        for(int i=0;i< 11;i++)
        {
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);

            cam.position = new Vector3
                (
                    originalPos.x + x,
                    originalPos.y + y,
                    originalPos.z
                );
            yield return null;
        }
        cam.position = originalPos;

    }

    void DealDamage()
    {
        float finalDamage = Damage;
        float chance = Random.Range(0f, 100f);
        bool isCrit = chance <= Criticalchance;
        if(isCrit)
        {
            finalDamage *= CriticalMultiplier;// only once it shoule be activated
        }


        Collider2D[] Enemies = Physics2D.OverlapCircleAll(AttackPoint.position, attackrange, EnemiesList);

        foreach (Collider2D enemy in Enemies)
        {
            EnemyController EC= enemy.GetComponent<EnemyController>();
            if(isCrit)
            {
                
                AM.PlayCritSfx(AM.Crit);
                StartCoroutine("ImpactPause");
         

                StartCoroutine("CameraShake");
            }
            bool killed = EC.DamageDealt(finalDamage);
            if(killed && ph.canhealonKill)
            {
                ph.HealOnKill(4);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, GroundCheckdist);
        Gizmos.DrawWireSphere(AttackPoint.position, attackrange);
    }

    //sfx for sword slash

    public void SwordSfx1()
    {
        AM.PlaySFX(AM.attackslash1);
    }
    public void SwordSfx2()
    {
        AM.PlaySFX(AM.attackslash2);
    }
}