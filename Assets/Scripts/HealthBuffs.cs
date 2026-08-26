using System;
using UnityEngine;

public class HealthBuffs : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    PlayerHealth Ph ;
    public string[] titles =
    {
        "Damage Increase",
        "Heal Your Health",
        "Increase Your maxHealth",
        "Increase Special Attack Damage"
    };
    public string[] desc =
   {
        "Your Damage will be Doubled",
        "Health will be healed by 50%",
        "Your health will be buffed, MaxHealth Increases",
        "Doubles Your Special Attack Damage"
    };

 
  
    void Start()
    { 
        Ph = FindAnyObjectByType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
