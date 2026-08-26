using System.Collections.Generic;
using UnityEngine;

public class SlashSpecialController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private HashSet<Collider2D> enemyCollider = new HashSet<Collider2D>();
    private PlayerHealth ph;
    public float damage = 28;
    public bool isBuffactive = false;
    void Start()
    {
        ph = FindAnyObjectByType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (ph.specialpowerBuff && !isBuffactive)
        {
            damage *= 2;
            isBuffactive = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") && !enemyCollider.Contains(collision))
        {
            collision.GetComponent<EnemyController>().DamageDealt(damage);
            enemyCollider.Add(collision);
        }
    }
}
