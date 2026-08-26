using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float MaxHealth = 100;
    public float Currhealth;
    public bool isAlive = true;
    private Animator PlayAnim;
    private Rigidbody2D Playerrb;
    public float pushbackForce = 7f;


    public bool isPushed = false;
    public float Knockbacktime = 0.25f;


    public Slider HealthSlider;
    private PlayerController PC;

    public bool specialpowerBuff = false;

    public bool canhealonKill = false;

    public GameObject Burnslash;

    public AudioManager AM;
    //matter for buttons buffs
    public List<string> Healthtitles =new List<string>(){
        "Damage Increase",
        "Heal Your Health",
        "Increase Your maxHealth",
        "Enhanced Special Attack",
        "Double Dash",
        "Reduces Special Attack Cooldown"

    };
    public List<string> Healthdesc = new List<string>()
   {
        "Your Damage will be increased By 1.5x",
        "Health will be healed by 50%",
        "Your health will be buffed, MaxHealth Increases",
        "Empowers your Special Attack, doubling its damage",
        "You will be able to Dash Twice",
        "Special Attack cooldown will be reduced by half"
    };




    //specialBuffs

    public List<string> Specialtitles = new List<string>(){
        "Heal ON KILL",
        "Critical HITS",
        "INVISIBILITY",
        "INVISIBILITY duration Increases",
        "BURNING Special Slash",
        "RAGE MODE!"
    };
    public List<string> Specialdesc = new List<string>()
   {
        "Everytime You kill a enemy , Your health increases",
        "every noraml attack has a 30% chance to be a critical HIT",
        "Press ' I ' to Become Invincible ,so no enemy can do damage to YOU",
        "The duaration of Invincibility will be doubled" +
        "\n if invincibility is unlocked before",
        "A Slash which will burn the enemies for a few seconds \n Your special attack will change into the burn version",
        "ON clicking 'R' \n Your Movememt speed increases and You do criticals, no one can damage you for a few seconds"
    };




    public List <Action> Healthfunct;
    public List<Action> Specialfunct;
    void Start()
    {
        Currhealth = MaxHealth;
        HealthSlider.maxValue = MaxHealth;
        HealthSlider.value = Currhealth;
        PlayAnim = GetComponent<Animator>();
        Playerrb = GetComponent<Rigidbody2D>();
        PC = GetComponent<PlayerController>();
        AM = AudioManager.Instance;
        Healthfunct = new List<Action>()
        {
            DamageBuff,
            HealBuff,
            HealthBuff,
            SpecialAttackBuff,
            DoubleDash,
            ReduceSpecialCooldown
        };
        Specialfunct = new List<Action>()
        {
            HealonKillBuff,
            CritBuff,
            Invisibiity,
            IncreaseInvisibilityDuration,
            BurningSlash,
            RageBuff

        };
    }


    // Update is called once per frame
    void Update()
    {
        if(Currhealth <=0 )
        {
            Dead();
        }
    }
     
    public void HealonKillBuff()
    {
        canhealonKill = true;
    }
    public void HealOnKill(int val)
    {
        Currhealth += val;
        Currhealth = Mathf.Clamp(Currhealth, 0, MaxHealth);
        HealthSlider.value = Currhealth;
    }
    public void CritBuff()
    {
        PC.Criticalchance = 30;
    }
    public void DamageBuff()
    {
        PC.Damage *= 1.5f;
    }

    public void DoubleDash()
    {
        PC.MaxDash = 2;
        PC.remainingDash = 2;
    }

    public void ReduceSpecialCooldown()
    {
        PC.SpecialAttackCooldown = PC.SpecialAttackCooldown / 2;
    }

    public void HealBuff()
    {
        Currhealth = Currhealth + 50;
        Currhealth = Mathf.Clamp(Currhealth, 0, MaxHealth);
        HealthSlider.value = Currhealth;
    }




    public void BurningSlash()
    {
        PC.slashEffect = Burnslash;
        Healthfunct.RemoveAt(3);
        Healthtitles.RemoveAt(3);
        Healthdesc.RemoveAt(3);
    }
    public void Invisibiity()
    {
        PC.CangoInvisibilty = true;
        PC.canuseInvisibility = true;
    }

    public void HealthBuff()
    {
        RectTransform Rt = HealthSlider.GetComponent<RectTransform>();
        Rt.sizeDelta = new Vector2(300, 31);
        Rt.position = Rt.position + new Vector3(50f, 0, 0);
        MaxHealth = 150;
        Currhealth = 150;
        HealthSlider.maxValue = MaxHealth;
        HealthSlider.value = Currhealth;
        AM.PlaySFX(AM.BuffoffCooldown);
    }
    public void SpecialAttackBuff()
    {
        specialpowerBuff = true;
    }
    public void IncreaseInvisibilityDuration()
    {
        PC.invincibilityTime *= 2;
    }


    public void RageBuff()
    {
        PC.RagemodeBuff = true;
    }
    public void TakeDamage(float damage, Vector2 enemyPos)
    {
        if(!isAlive || PC.isInvincible || PC.IsRageActive)
        {
            return;
        }
        AM.PlayPlayerHurtSfx(AM.TakeHit);
        
        Currhealth = Currhealth - damage;
        HealthSlider.value = Currhealth;
        Hit();
        float dir = Mathf.Sign(transform.position.x - enemyPos.x);
        StartCoroutine(pushback(dir));
       
        Debug.Log("Damage Taken");
    }

    IEnumerator pushback(float dir)
    {
        isPushed = true;
       

        Playerrb.linearVelocity = new Vector2(dir * pushbackForce, Playerrb.linearVelocity.y);
        yield return new WaitForSeconds(Knockbacktime);
        isPushed = false;


        
    }
    public void Hit()
    {
        PlayAnim.SetTrigger("Hit");
    }
    public void Dead()
    {
        if(!isAlive)
        {
            return;
        }
        isAlive = false;
        PlayAnim.SetTrigger("Death") ;
        Playerrb.linearVelocity = Vector2.zero;
        Playerrb.simulated = false;
        GetComponent<Collider2D>().enabled = false;
        FindAnyObjectByType<GameManager>().gameOver();

    }
}
