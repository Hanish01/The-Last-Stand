using System.Collections.Generic;
using UnityEngine;

public class BurnSpecialattackController : MonoBehaviour
{
   
    private HashSet<Collider2D> enemyCollider = new HashSet<Collider2D>();
    private PlayerHealth ph;
    public float damage = 20;
    public float damagepersecond = 4f;
    public float burndamgeduration = 8f;
    public bool isbuffactivated = false;
    void Start()
    {
        ph = FindAnyObjectByType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ph.specialpowerBuff && !isbuffactivated)
        {
            damagepersecond = 10;
            isbuffactivated = true;
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !enemyCollider.Contains(collision))
        {
            collision.GetComponent<EnemyController>().DamageDealt(damage);
            collision.GetComponent<EnemyController>().ApplyBurn(damagepersecond, burndamgeduration);
            enemyCollider.Add(collision);
        }
    }
}
